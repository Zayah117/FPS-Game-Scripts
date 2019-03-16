using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitScan : PlayerWeapon {
    public override void Fire() {
        base.Fire();
        if (attackCooldown <= 0) {
            FireHitScan(character.hitscanOrigin.position, character.hitscanOrigin.forward, raycastDistance);
            ResetCooldown();
        }
    }
}
