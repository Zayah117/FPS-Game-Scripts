using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Weapon {
    public override void Fire() {
        base.Fire();
        if (attackCooldown <= 0) {
            float distance = Vector3.Distance(character.transform.position, character.targetLocation);
            FirePhysicsProjectileAutomaticArc(character.weaponMuzzle, character.targetLocation, 50f);
            ResetCooldown();
        }
    }
}
