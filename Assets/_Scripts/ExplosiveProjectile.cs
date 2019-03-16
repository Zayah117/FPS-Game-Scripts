using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile {
    [SerializeField] float radius;
    [SerializeField] int explosiveDamage; // Explosive projectiles do normal damage PLUS explosive damage
    [SerializeField] ParticleSystem explosionPrefab;

    public override void OnTriggerEnter(Collider other) {
        // Similar to base trigger enter

        Character hitCharacter = other.GetComponent<Character>();
        if (hitCharacter) {
            if (hitCharacter == character) {
                return;
            }
            hitCharacter.TakeDamage(damage, character);
        }
        Explosions.Explosion(transform.position, radius, explosiveDamage, explosionPrefab, character);
        DestroySaveable();
    }
}
