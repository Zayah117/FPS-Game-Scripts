using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : SaveableObject {
    [SerializeField] float _speed;
    int _damage;

    Rigidbody rb;
    [HideInInspector] public Character character;

    public float speed {
        get {
            return _speed;
        }
        set {
            _speed = value;
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

    public override void Start() {
        base.Start();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update() {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    public virtual void OnTriggerEnter(Collider other) {
        Character hitCharacter = other.GetComponent<Character>();
        if (hitCharacter) {
            if (hitCharacter == character) {
                return;
            }
            hitCharacter.TakeDamage(damage, character);
        }
        DestroySaveable();
    }

    public override GameData Save(GameData gameData, int id) {
        ProjectileData projectileData = new ProjectileData();

        projectileData.id = id;
        projectileData.position = Vector3Surrogate.ConvertFromVector3(transform.position);
        projectileData.rotation = Vector3Surrogate.ConvertFromVector3(transform.rotation.eulerAngles);

        gameData.projectileData.Add(projectileData);
        return gameData;
    }

    public override void Load(GameData gameData, int id) {
        ProjectileData projectileData = gameData.projectileData.First(x => x.id == id);

        transform.position = projectileData.position.ConvertedToVector3;
        transform.eulerAngles = projectileData.rotation.ConvertedToVector3;
    }
}
