using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : SaveableObject, IDamagable {
    [SerializeField] float _speed;
    [SerializeField] int _health;
    [SerializeField] int _maxHealth;
    public bool isDead = false;
    public GameObject characterMesh;
    public Collider characterCollider;
    [HideInInspector] public Weapon equippedWeapon;
    public Transform weaponMuzzle;
    [SerializeField] protected Transform flashMuzzle;
    [SerializeField] protected GameObject muzzleLight;
    public Transform hitscanOrigin;
    public Transform meleeWeaponHitBoxOrigin;
    public Weapon weaponPrefab;
    [SerializeField] string weaponFlashTag;

    [HideInInspector] public Vector3 targetLocation;

    protected Animator anim;
    // public bool fireAnimationPlaying = false;
    bool fireTrigger = false;

    public float speed {
        get {
            return _speed;
        }
        set {
            _speed = value;
        }
    }

    public int health {
        get {
            return _health;
        }
        set {
            _health = value;
        }
    }

    public int maxHealth {
        get {
            return _maxHealth;
        }
        set {
            _maxHealth = value;
        }
    }

    public Collider DamagableCollider {
        get {
            return characterCollider;
        }
    }

    public Vector3 ColliderPosition {
        get {
            return characterCollider.bounds.center;
        }
    }

    public override void Start() {
        base.Start();
        InstantiateWeapon(weaponPrefab);
        anim = characterMesh.GetComponent<Animator>();
    }

    public virtual void Update() {
        AttackLoop();
    }

    public void InstantiateWeapon(Weapon weaponPrefab) {
        equippedWeapon = Instantiate(weaponPrefab);
        equippedWeapon.character = GetComponent<Character>();
    }

    public virtual void AttackLoop() {
        bool weaponCooledDown = equippedWeapon.attackCooldown <= 0;

        if (fireTrigger && weaponCooledDown) {
            SetFireAnimation(true);
        } else {
            SetFireAnimation(false);
        }
    }

    protected void SetFireAnimation(bool b) {
        anim.SetBool("IsFiring", b);
    }

    public bool FireAnimationPlaying() {
        AnimatorStateInfo stateInfo = characterMesh.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Attack")) {
            return true;
        } else {
            return false;
        }
    }

    protected void WeaponFlash() {
        if (weaponFlashTag != null && flashMuzzle != null) {
            GameObject spawnedObject = Managers.Pooler.SpawnFromPool(weaponFlashTag, flashMuzzle.position, Quaternion.identity, 0.5f);
            spawnedObject.transform.parent = flashMuzzle.transform;
        }
    }

    protected void MuzzleLight() {
        if (muzzleLight != null) {
            StopCoroutine(MuzzleLightOff());
            muzzleLight.GetComponent<Light>().enabled = true;
            StartCoroutine(MuzzleLightOff());
        }
    }

    IEnumerator MuzzleLightOff() {
        yield return new WaitForSeconds(0.05f);
        muzzleLight.GetComponent<Light>().enabled = false;
    }

    public virtual void FireWeapon() {
        equippedWeapon.Fire();
        WeaponFlash();
        MuzzleLight();
    }

    public void SetFireTrigger(bool b) {
        fireTrigger = b;
    }

    public virtual void TakeDamage(int damage, Character offendingCharacter) {
        health -= damage;
        if (health <= 0) {
            Die(offendingCharacter);
        }
    }

    public virtual void Heal(int value) {
        health += value;
        if (health > maxHealth) {
            health = maxHealth;
        }
    }

    public virtual void Die(Character offendingCharacter) {
        if (!isDead) {
            Destroy(equippedWeapon.gameObject);
            Destroy(this.gameObject);
            isDead = true;
        }
    }
}
