using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosions : MonoBehaviour {
    public static void Explosion(Vector3 explosionOrigin, float explosionRadius, int damage, ParticleSystem particlePrefab, Character offendingCharacter) {
        // Enable explosion Effects
        ParticleSystem particleEffect = Instantiate(particlePrefab, explosionOrigin, Quaternion.identity);

        // Scale particle effect to match radius (default effect has a radius of about 5)
        particleEffect.transform.localScale = particleEffect.transform.localScale * (explosionRadius / 5);
        
        foreach (Transform child in particleEffect.transform) {
            child.transform.localScale = child.transform.localScale * (explosionRadius / 5);
        }

        // Play explosion sound
        Managers.Audio.PlayRandomExplosion(explosionOrigin);

        Collider[] hitColliders = Physics.OverlapSphere(explosionOrigin, explosionRadius);
        foreach (Collider collider in hitColliders) {
            GameObject hitObject = collider.gameObject;

            // If the hit object is damagable
            IDamagable damageable = hitObject.GetComponent<IDamagable>();
            if (damageable != null) {
                RaycastHit hit;
                Vector3 direction = (damageable.ColliderPosition - explosionOrigin).normalized;
                float distance = Vector3.Distance(explosionOrigin, damageable.ColliderPosition);

                // And if object is in line of sight
                if (Physics.Raycast(explosionOrigin, direction, out hit, distance)) {
                    if (hit.transform.gameObject == hitObject) {
                        float raycastHitDistance = Vector3.Distance(explosionOrigin, hit.point);
                        // Calculate damage
                        float damageModifier = 1f - (raycastHitDistance / explosionRadius);
                        int totalDamage = (int)(damage * damageModifier);

                        if (totalDamage < 0) {
                            Debug.LogError("Explosion damage came out as a negative integer. (" + totalDamage.ToString() + ") Should only be positive integer.");
                        }

                        // Do Damage
                        damageable.TakeDamage(totalDamage, offendingCharacter);
                    }
                }
            }
        }
    }
}
