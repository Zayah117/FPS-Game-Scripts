using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : Weapon {
    public override void Fire() {
        base.Fire();
        if (attackCooldown <= 0) {
            FireProjectile(character.weaponMuzzle, character.targetLocation);
            ResetCooldown();
        }
    }
}
