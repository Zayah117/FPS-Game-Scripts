using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
// using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(RichAI))]
[RequireComponent(typeof(FieldOfView))]
public class AICharacter : Character, IHittable {
    // [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public IAstarAI astarAgent;
    [HideInInspector] public RichAI richAgent;
    [HideInInspector] public Seeker seeker;
    [HideInInspector] public FieldOfView fov;
    StateController stateController;

    public float wanderDistance = 10f;
    public float wanderVariance = 5f;
    public float elapseTime = 1f;
    public float allyAggroDistance = 15f;
    public int attackIterations = 1;
    public float attackCycleCooldown = 3f;
    public float preferredAttackRange = 40f;
    public bool attackCycleReady;
    int currentAttackIteration = 0;

    public ParticleSystem hitParticle;
    public ParticleSystem hitBloodSplatterParticle;
    public Rigidbody goreParticle;
    public Color bloodColor;

    [SerializeField] GameObject[] drops;

    [SerializeField] float killScore;

    [HideInInspector] public bool stunned;

    public bool currentlyAttacking; // For attack tokens
    public bool attackLocked = true;

    void Awake() {
        // agent = GetComponent<NavMeshAgent>();
        astarAgent = GetComponent<IAstarAI>();
        richAgent = GetComponent<RichAI>();
        seeker = GetComponent<Seeker>();

        fov = GetComponent<FieldOfView>();
        stateController = GetComponent<StateController>();

        // agent.speed = speed;
        astarAgent.maxSpeed = speed;

        attackCycleReady = true;
    }

    public override void Start() {
        base.Start();
        Managers.Gameplay.aICharacters.Add(this);
        CheckForAggro();
    }

    public override void FireWeapon() {
        base.FireWeapon();
        IncrementAttackIteration();
    }

    public void FaceTarget(Transform target) {
        if (target) {
            Vector3 pos = transform.InverseTransformPoint(target.transform.position);
            pos = new Vector3(pos.x, 0, pos.z);
            Vector3 direction = pos.normalized;
            float angle = Vector3.SignedAngle(Vector3.forward, pos, transform.up);

            if (Vector3.Dot(transform.up, Vector3.up) < 0) {
                angle = -angle;
            }

            transform.RotateAround(transform.position, transform.up, angle);
        }
    }

    public void FaceTargetOld(Transform target) {
        if (target) {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 50f);
        }
    }

    public bool IsAggroed() {
        return stateController.target != null;
    }

    public void Aggro(Transform target) {
        fov.isActive = true;
        stateController.target = target;

        AggroAICharacters();
    }

    // I think this function is probably a mess (but not a huge mess)
    public void AggroAICharacters() {
        StateController myStateController = GetComponent<StateController>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, allyAggroDistance);
        for (int i = 0; i < colliders.Length; i++) {
            AICharacter aiCharacter = colliders[i].GetComponent<AICharacter>();
            if (aiCharacter != null && !aiCharacter.fov.isActive) {
                aiCharacter.Aggro(myStateController.target);
            }
        }
    }

    public void CheckForAggro() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, allyAggroDistance);
        for (int i = 0; i < colliders.Length; i++) {
            AICharacter aiCharacter = colliders[i].GetComponent<AICharacter>();
            if (aiCharacter != null) {
                StateController otherStateController = aiCharacter.GetComponent<StateController>();
                if (otherStateController.target != null) {
                    Aggro(otherStateController.target);
                }
            }
        }
    }

    // Astar Navigation methods
    public static Vector3 RandomPointCircle(Vector3 origin, float radius) {
        var point = Random.insideUnitSphere * radius;

        point.y = 0;
        point += origin;
        return point;
    }

    public static Vector3 RandomPointConstantPath(Vector3 origin, int searchLength) {
        ConstantPath path = ConstantPath.Construct(origin, searchLength);

        AstarPath.StartPath(path);
        path.BlockUntilCalculated();

        var randomPoint = PathUtilities.GetPointsOnNodes(path.allNodes, 1)[0];
        return randomPoint;
    }

    public static Vector3[] CalculatePath(Seeker seeker, Vector3 sourcePosition, Vector3 targetPosition) {
        Path path = seeker.StartPath(sourcePosition, targetPosition);
        path.BlockUntilCalculated();
        Vector3[] vectorPath = path.vectorPath.ToArray();
        return vectorPath;
    }

    // Navigation methods
    // Gets me a random point on the nav mesh close to the origin
    public static Vector3 RandomNavSphere(Vector3 origin, float range) {
        Vector3 randomPoint = origin + Random.insideUnitSphere * range;
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1000f, UnityEngine.AI.NavMesh.AllAreas)) {
            return hit.position;
        } else {
            Debug.LogWarning("RandomNavSphere could not find hit point.");
            return Vector3.zero;
        }
    }

    // Not uniform for characters that are rotated - not implemented in AI action behaviors
    public static Vector3 RandomNavCircle(Vector3 origin, float range) {
        Vector2 ciclePoint = new Vector2(origin.x, origin.z) + Random.insideUnitCircle * range;
        Vector3 randomPoint = new Vector3(ciclePoint.x, origin.y, ciclePoint.y);
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1000f, UnityEngine.AI.NavMesh.AllAreas)) {
            return hit.position;
        } else {
            Debug.LogWarning("RandomNavCircle could not find hit point.");
            return Vector3.zero;
        }
    }

    public static Vector3 RandomNavHemisphereTowardTarget(Vector3 origin, Vector3 targetLocation, float distance, float angle, int layermask) {
        float minRadius = 1f;
        Vector3 direction = (origin - targetLocation);
        Vector3 randomDirection = (GetPointOnUnitSphereCap(direction, angle, minRadius, distance) * -1f); // Toward target
        randomDirection += origin;
        randomDirection = new Vector3(randomDirection.x, targetLocation.y, randomDirection.z);
        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

    public static Vector3 RandomNavHemisphereAwayTarget(Vector3 origin, Vector3 targetLocation, float distance, float angle, int layermask) {
        float minRadius = 1.5f;
        Vector3 direction = (origin - targetLocation);
        Vector3 randomDirection = (GetPointOnUnitSphereCap(direction, angle, minRadius, distance));
        randomDirection += origin;
        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

    public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle, float minRadius, float maxRadius) {
        var angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        var PointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
        return targetDirection * V * Random.Range(minRadius, maxRadius);
    }
    
    public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle, float minRadius, float maxRadius) {
        return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle, minRadius, maxRadius);
    }

    public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity) {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime(shotSpeed, targetRelativePosition, targetRelativeVelocity);
        return targetPosition + t * (targetRelativeVelocity);
    }

    public static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity) {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f) {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f) {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            } else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        } else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }

    public void EmitHitParticle(Vector3 pos, Vector3 origin) {
        ParticleSystem bloodEffect = Instantiate(hitParticle, pos, Quaternion.identity);
        ParticleSystem.MainModule bloodEffectMM = bloodEffect.main;
        bloodEffectMM.startColor = bloodColor;

        ParticleSystem bloodDrops = Instantiate(hitBloodSplatterParticle, pos, Quaternion.identity);
        ParticleSystem.MainModule bloodDropsMM = bloodDrops.main;
        bloodDropsMM.startColor = bloodColor;
        bloodDrops.transform.LookAt(origin);
        bloodDrops.Play();
    }

    public override void TakeDamage(int damage, Character offendingCharacter) {
        base.TakeDamage(damage, offendingCharacter);
        GameObject damageTextObject = Managers.Pooler.SpawnFromPool("DamageText", transform.position, Quaternion.identity, 0.4f);
        DamageText damageText = damageTextObject.GetComponent<DamageText>();
        damageText.transform.LookAt(Managers.Gameplay.player.transform);
        damageText.SetDamageText(damage.ToString());
        damageText.animator.Play("DamageTextAnimation", -1, 0f);
        Stun();
        if(!IsAggroed()) {
            Aggro(offendingCharacter.transform);
        }

        if (offendingCharacter.tag == "Player" && health > 0) {
            Managers.Audio.PlayHitTargetSound(4);
        }
    }

    public virtual void Stun() {
        stunned = true;
        stateController.TransitionToState(stateController.stunnedState);
    }

    public void ResetStun() {
        stunned = false;
    }

    public override void Die(Character offendingCharacter)
    {
        if (!isDead) {
            isDead = true;

            // Simple death functionality
            /*
            StateController stateController = GetComponent<StateController>();
            if (stateController)
            {
                stateController.aiActive = false;
            }

            Collider collider = GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }
            */

            // agent.enabled = false;
            astarAgent.canMove = false;

            // Instantiate drop items
            if (drops.Length > 0) {
                for (int i = 0; i < drops.Length; i++) {
                    GameObject drop = Instantiate(drops[i], transform.position, Quaternion.identity);
                    Rigidbody rb = drop.GetComponent<Rigidbody>();
                    if (rb != null) {
                        rb.AddForce(new Vector3(Random.Range(-300, 300), Random.Range(200, 400), Random.Range(-300, 300)));
                        rb.AddTorque(new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), Random.Range(-300, 300)));
                    }
                }
            }

            // Set mesh inactive
            // characterMesh.SetActive(false);

            // Instantiate gore
            for (int i = 0; i < 5; i++) {
                Rigidbody gp = Instantiate(goreParticle, transform.position, Quaternion.identity);
                Renderer rend = gp.GetComponent<Renderer>();
                rend.material.SetColor("_Color", bloodColor);
                gp.transform.localScale = new Vector3(gp.transform.localScale.x * Random.Range(0.5f, 1f), gp.transform.localScale.y * Random.Range(0.5f, 1f), gp.transform.localScale.z * Random.Range(0.5f, 1f));
                gp.AddForce(new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), Random.Range(-300, 300)));
            }

            // If the enemy came from a spawner, unsubscribe from spawner list on death
            SpawnableObject spawnable = GetComponent<SpawnableObject>();
            if (spawnable) {
                spawnable.Unsubscribe();
            }

            // Unsubscribe from manager
            Managers.Gameplay.aICharacters.Remove(this);

            // Add points to GameManager powerLevel
            // GameplayManager.instance.AddPowerX(killScore);

            // Death animation and cleanup
            // StartCoroutine(DeadAnimation());
            // StartCoroutine(DestroyAfterTime(3));

            if (offendingCharacter.tag == "Player") {
                Managers.Audio.PlayKillTargetSound(2);
                Managers.Gameplay.AddCombo();
            }

            // Destroy(gameObject);
            ReturnAttackToken();
            Destroy(equippedWeapon.gameObject);
            DestroySaveable();
        }
    }

    public virtual void BorrowAttackToken() {
        if (!currentlyAttacking) {
            currentlyAttacking = true;
            attackCycleReady = false;
            Managers.Gameplay.attackingEnemies += 1;
        }
    }

    public void ReturnAttackToken() {
        if (currentlyAttacking) {
            currentlyAttacking = false;
            ResetAttackIterations();
            StartCoroutine(StartAttackCycleCooldown(attackCycleCooldown));
            // attackLocked = true;
            Managers.Gameplay.attackingEnemies -= 1;
        }
    }

    public bool InAttackPosition() {
        return attackCycleReady && fov.visibleTargets.Contains(stateController.target) && TargetPathClearOfAllies(true);
    }

    public bool AttackReady() {
        return !attackLocked;
    }

    public float DistanceToTarget() {
        return Vector3.Distance(transform.position, stateController.target.transform.position);
    }

    public bool TargetPathClearOfAllies(bool ignoreAlliesBehind) {
        RaycastHit hit;

        Vector3 directionToTarget = (stateController.target.position - transform.position).normalized;

        if (Physics.SphereCast(weaponMuzzle.position, 1f, directionToTarget, out hit, 1000f, fov.allyMask)) {
            if (ignoreAlliesBehind && Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) > Vector3.Distance(transform.position, stateController.target.position)) {
                return true;
            } else {
                return false;
            }
        } else {
            return true;
        }
    }

    public static Vector3 FindPointAlongPath(Vector3[] path, float distanceToTravel) {
        if (distanceToTravel < 0) {
            return path[0];
        }

        // Loop through each corner in path
        for (int i = 0; i < path.Length -1; i++) {
            // If the distance between the next two points is less than the distance you have left to travel
            if (distanceToTravel <= Vector3.Distance(path[i], path[i +1])) {
                // Calculate the point that is the correct distance between the two points and return it
                Vector3 directionToTravel = path[i + 1] - path[i];
                directionToTravel.Normalize();
                return (path[i] + (directionToTravel * distanceToTravel));
            } else {
                // Otherwise subtract the distance between those 2 points from the distance left to travel
                distanceToTravel -= Vector3.Distance(path[i], path[i + 1]);
            }
        }

        // if the distance to travel is greater than the distance of the path, return the final point
        return path[path.Length - 1];
    }

    public bool ReachedAttackIterations() {
        if (currentAttackIteration >= attackIterations) {
            return true;
        } else {
            return false;
        }
    }

    public void ResetAttackIterations() {
        currentAttackIteration = 0;
    }

    public void IncrementAttackIteration() {
        currentAttackIteration += 1;
    }

    // Used in StateController
    public virtual void OnExitState() {
        // May not want to set fire trigger on every state
        SetFireTrigger(false);
        attackLocked = true;
        if (currentlyAttacking) {
            ReturnAttackToken();
        }
    }

    IEnumerator StartAttackCycleCooldown(float time) {
        yield return new WaitForSeconds(time);
        attackCycleReady = true;
    }

    IEnumerator DeadAnimation() {
        yield return new WaitForSeconds(2);
        while (true) {
            characterMesh.transform.localScale = Vector3.Lerp(characterMesh.transform.localScale, Vector3.zero, Time.deltaTime * 5);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DestroyAfterTime(float time) {
        yield return new WaitForSeconds(time);
        Destroy(equippedWeapon.gameObject);
        Destroy(gameObject);
    }

    public override GameData Save(GameData gameData, int id) {
        AICharacterData aICharacterData = new AICharacterData();

        aICharacterData.id = id;
        aICharacterData.position = Vector3Surrogate.ConvertFromVector3(transform.position);
        aICharacterData.rotation = Vector3Surrogate.ConvertFromVector3(transform.rotation.eulerAngles);
        aICharacterData.health = health;

        gameData.aICharacterData.Add(aICharacterData);
        return gameData;
    }

    public override void Load(GameData gameData, int id) {
        AICharacterData aICharacterData = gameData.aICharacterData.First(x => x.id == id);

        health = aICharacterData.health;
        transform.position = aICharacterData.position.ConvertedToVector3;
        // agent.Warp(aICharacterData.position.ConvertedToVector3);
        astarAgent.Teleport(aICharacterData.position.ConvertedToVector3);
        transform.eulerAngles = aICharacterData.rotation.ConvertedToVector3;
    }
}
