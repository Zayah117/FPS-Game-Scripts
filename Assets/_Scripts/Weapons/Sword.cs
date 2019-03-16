using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : PlayerWeapon {
    public override void Fire() {
        base.Fire();
        if (attackCooldown <= 0) {
            FireHitBox(character.meleeWeaponHitBoxOrigin, 2f);
            ResetCooldown();
        }
    }
}
