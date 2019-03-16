using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicsProjectile : PlayerWeapon {
    [SerializeField] float force = 2000f;
    public override void Fire() {
        base.Fire();
        if (attackCooldown <= 0) {
            FirePhysicsProjectile(character.weaponMuzzle, character.targetLocation, force);
            ResetCooldown();
        }
    }
}
