using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] float _maxFireRatePerSecond = 100.0f;
    [SerializeField] float _spread;
    [SerializeField] int _damage;
    [SerializeField] protected float raycastDistance = 1000.0f;
    [HideInInspector] public float attackCooldown;

    public enum WeaponType {
        Hitscan,
        Projectile,
        PhysicsProjectile,
        Melee
    }

    public WeaponType weaponType;
    [SerializeField] protected AudioClip fireSound;
    [SerializeField] float fireSoundVolume = 1f;
    public Projectile projectile;
    public GameObject physicsProjectile;
    public GameObject hitSphere;

    [HideInInspector] public Character character;

    public float maxFireRatePerSecond {
        get {
            return _maxFireRatePerSecond;
        }
        set {
            _maxFireRatePerSecond = value;
        }
    }

    public float spread {
        get {
            return _spread;
        }
        set {
            _spread = value;
        }
    }

    public float hitscanSpread {
        get {
            return _spread / 100.0f;
        }
    }

    public int damage {
        get {
            return _damage;
        }
        set {
            _damage = value;
        }
    }

    void Update() { // Laser.cs has it's own update funciton that needs to include this (not overridden, TEMPORARY)
        attackCooldown -= Time.deltaTime;
    }

    public virtual void Fire() {
        if (fireSound != null) { // Temporary if
            FireSound(character.transform.position);
        }
    }

    public void FireProjectile(Transform spawnLocation, Vector3 targetLocation) {
        // Projectile spawnedProjectile = (Projectile)SaveableResource.InstantiateSaveableResource(projectile, spawnLocation.position, spawnLocation.rotation);
        Debug.LogWarning("spawnedProjectile is null");
        Projectile spawnedProjectile = null;

        spawnedProjectile.character = character;
        spawnedProjectile.damage = damage;
        spawnedProjectile.transform.LookAt(targetLocation);

        Vector3 spreadTransform = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        spawnedProjectile.transform.rotation = spawnedProjectile.transform.rotation * Quaternion.Euler(spreadTransform);
    }

    public void FirePhysicsProjectile(Transform spawnLocation, Vector3 targetLocation, float force) {
        // TODO: Make spawned projectile a saveable object
        GameObject spawnedProjectile = Instantiate(physicsProjectile, spawnLocation.position, spawnLocation.rotation);
        spawnedProjectile.transform.LookAt(targetLocation);

        // If the spawnedProjectile is a grenade, set character
        Grenade grenade = spawnedProjectile.GetComponent<Grenade>();
        if (grenade != null) {
            grenade.character = character;
        }

        // Spread
        Vector3 spreadTransform = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        spawnedProjectile.transform.rotation = spawnedProjectile.transform.rotation * Quaternion.Euler(spreadTransform);

        Rigidbody rb = spawnedProjectile.GetComponent<Rigidbody>();
        if (rb) {
            rb.AddForce(spawnedProjectile.transform.forward * force);
        }
    }

    public void FirePhysicsProjectileAutomaticArc(Transform spawnLocation, Vector3 targetLocation, float maxSpeed) {
        // TODO: Make spawned projectile a saveable object
        GameObject spawnedProjectile = Instantiate(physicsProjectile, spawnLocation.position, spawnLocation.rotation);

        // If the spawnedProjectile is a grenade, set character
        Grenade grenade = spawnedProjectile.GetComponent<Grenade>();
        if (grenade != null) {
            grenade.character = character;
        }

        // Fancy math stuff to calculate velocity and fire projectile in arc
        // https://gamedev.stackexchange.com/a/114547
        Vector3 toTarget = targetLocation - spawnLocation.position;

        // Set up the terms we need to solve the quadratic equations
        float gSquared = Physics.gravity.sqrMagnitude;
        float b = maxSpeed * maxSpeed + Vector3.Dot(toTarget, Physics.gravity);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

        if (discriminant < 0) {
            Debug.LogError("Target not in range.");
        }

        float discRoot = Mathf.Sqrt(discriminant);

        // Highest shot with the given max speed:
        float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        // Most direction shot with givin max speed:
        float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        // Lowest-speed arc available:
        float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

        float T = T_lowEnergy;

        // Convert from time-to-hit to a launch velocity:
        Vector3 velocity = toTarget / T - Physics.gravity * T / 2f;

        // Spread
        Vector3 spreadTransform = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
        velocity = velocity + spreadTransform;

        // Apply the calculated velocity
        Rigidbody rb = spawnedProjectile.GetComponent<Rigidbody>();
        if (rb) {
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    public virtual void FireHitScan(Vector3 origin, Vector3 direction, float maxDistance) {
        RaycastHit hit;

        direction.x += Random.Range(-hitscanSpread, hitscanSpread);
        direction.y += Random.Range(-hitscanSpread, hitscanSpread);
        direction.z += Random.Range(-hitscanSpread, hitscanSpread);

        if (Physics.Raycast(origin, direction, out hit, maxDistance, -1, QueryTriggerInteraction.Ignore)) {
            if (!(hit.transform.gameObject.name == character.gameObject.name)) {
                GameObject hitObject = hit.transform.gameObject;
                if (hitSphere != null) {
                    Instantiate(hitSphere, hit.point, Quaternion.identity);
                }

                // Spawn particle
                IHittable hittable;
                hittable = hitObject.GetComponent<IHittable>();
                if (hittable != null) {
                    hittable.EmitHitParticle(hit.point, origin);
                }

                // Take damage
                IDamagable damagable;
                damagable = hitObject.GetComponent<IDamagable>();
                if (damagable != null) {
                    damagable.TakeDamage(damage, character);
                }
            }
        }
    }

    public void FireHitBox(Transform origin, float boxSize) {
        Collider[] hitColliders = Physics.OverlapBox(origin.position, new Vector3(boxSize / 2, boxSize / 2, boxSize / 2), Quaternion.identity, -1);
        foreach (Collider hit in hitColliders) {
            Vector3 targetDirection = (hit.transform.position - character.transform.position);
            if (Vector3.Dot(targetDirection, character.transform.forward) < 0) {
                continue;
            }

            IHittable hittable;
            hittable = hit.GetComponent<IHittable>();
            if (hittable != null && hit.gameObject != character.gameObject) {
                hittable.EmitHitParticle(hit.transform.position, character.transform.position); // May need to fix hit position
            }

            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null) {
                Character c = hit.GetComponent<Character>();
                if (c != character) {
                    damageable.TakeDamage(damage, character);
                }
            }
        }
    }
    
    public void ResetCooldown() {
        attackCooldown = 1f / maxFireRatePerSecond;
    }

    // Sound functions
    public virtual void FireSound(Vector3 position) {
        Managers.Audio.PlaySound3D(fireSound, position, fireSoundVolume);
    }
}
