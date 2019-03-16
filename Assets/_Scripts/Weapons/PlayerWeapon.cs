using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon {
    Player player;
    
    public int clipsize;

    [SerializeField] protected float damageDropStart;
    [SerializeField] protected float damageDropEnd;
    [SerializeField] protected int minDamage;

    [SerializeField] AudioClip dryFireSound;

    void Awake() {
        player = Managers.Gameplay.player;
    }

    public override void FireHitScan(Vector3 origin, Vector3 direction, float maxDistance) {
        RaycastHit hit;

        direction.x += Random.Range(-hitscanSpread, hitscanSpread);
        direction.y += Random.Range(-hitscanSpread, hitscanSpread);
        direction.z += Random.Range(-hitscanSpread, hitscanSpread);

        if (Physics.Raycast(origin, direction, out hit, maxDistance, Managers.LayerMasks.PlayerRaycastLayerMask, QueryTriggerInteraction.Ignore)) {
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

                // Take damage
                IDamagable damagable;
                damagable = hitObject.GetComponent<IDamagable>();
                if (damagable != null) {
                    damagable.TakeDamage(calculatedDamage, character);
                }
            }
        }
    }

    // Sound functions overriden to 2D
    public override void FireSound(Vector3 position = new Vector3()) {
        Managers.Audio.PlaySound2DRandomPitch(fireSound);
    }

    public void DryFireSound() {
        Managers.Audio.PlaySound2D(dryFireSound);
    }
}
