using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScatter : PlayerWeapon {
    [SerializeField] int pellets = 8;
    Dictionary<IDamagable, int> targetHitInfo = new Dictionary<IDamagable, int>();
    public LayerMask GetHitsRaycastLayerMask;

    public override void Fire() {
        base.Fire();
        if (attackCooldown <= 0) {
            targetHitInfo.Clear();

            // Collect all hits
            for (int i = 0; i < pellets; i++) {
                GetHit(character.hitscanOrigin.position, character.hitscanOrigin.forward, raycastDistance);
            }

            // Do damage to each character hit
            foreach(KeyValuePair<IDamagable, int> entry in targetHitInfo) {
                entry.Key.TakeDamage(entry.Value, character);
            }
            ResetCooldown();
        }
    }

    IDamagable GetHit(Vector3 origin, Vector3 direction, float maxDistance) {
        RaycastHit hit;

        direction.x += Random.Range(-hitscanSpread, hitscanSpread);
        direction.y += Random.Range(-hitscanSpread, hitscanSpread);
        direction.z += Random.Range(-hitscanSpread, hitscanSpread);

        if (Physics.Raycast(origin, direction, out hit, maxDistance, GetHitsRaycastLayerMask, QueryTriggerInteraction.Ignore)) {
            if (!(hit.transform.gameObject.name == character.gameObject.name)) {
                GameObject hitObject = hit.transform.gameObject;
                Instantiate(hitSphere, hit.point, Quaternion.identity);

                // Spawn particle
                IHittable hittable;
                hittable = hitObject.GetComponent<IHittable>();
                if (hittable != null) {
                    hittable.EmitHitParticle(hit.point, origin);
                }

                int calculatedDamage;
                float distance = hit.distance;

                if (distance < damageDropStart) {
                    calculatedDamage = damage;
                } else if (distance > damageDropEnd) {
                    calculatedDamage = minDamage;
                } else {
                    float space = damageDropEnd - damageDropStart;
                    float percent = (distance - damageDropStart) / space;
                    float floatDamage = (((float)damage - minDamage) * (1.0f - percent)) + minDamage;
                    calculatedDamage = Mathf.FloorToInt(floatDamage);
                }

                // Add damagable to targetHitInfo
                IDamagable damagable;
                damagable = hitObject.GetComponent<IDamagable>();
                if (damagable != null) {
                    if (!targetHitInfo.ContainsKey(damagable)) {
                        targetHitInfo.Add(damagable, calculatedDamage);
                    } else {
                        targetHitInfo[damagable] += calculatedDamage;
                    }
                }
            }
        }
        return null;
    }
}
