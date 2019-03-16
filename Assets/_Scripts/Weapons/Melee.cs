using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon {
    public override void Fire() {
        base.Fire();
        if (attackCooldown <= 0) {
            FireHitBox(character.meleeWeaponHitBoxOrigin, 2f);
            ResetCooldown();
        }
    }
}
