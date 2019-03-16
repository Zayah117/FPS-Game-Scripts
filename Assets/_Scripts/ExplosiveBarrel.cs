using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamagable {
    [SerializeField] float radius;
    [SerializeField] int damage;
    [SerializeField] ParticleSystem explosionParticle;
    bool hit;

    public Collider DamagableCollider {
        get {
            return GetComponent<Collider>();
        }
    }

    public Vector3 ColliderPosition {
        get {
            return GetComponent<Collider>().bounds.center;
        }
    }

    public void TakeDamage(int damage, Character offendingCharacter) {
        if (!hit) {
            hit = true;
            StartCoroutine(Explode(offendingCharacter));
        }
    }

    IEnumerator Explode(Character offendingCharacter) {
        yield return new WaitForSeconds(0.1f);
        Explosions.Explosion(transform.position, radius, damage, explosionParticle, offendingCharacter);
        Destroy(gameObject);
    }
}
