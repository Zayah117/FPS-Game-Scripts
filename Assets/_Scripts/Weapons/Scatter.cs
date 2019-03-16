using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatter : Weapon {
    [SerializeField] int pellets = 8;

    public override void Fire() {
        base.Fire();
        if (attackCooldown <= 0) {
            for (int i = 0; i < pellets; i++) {
                FireProjectile(character.weaponMuzzle, character.targetLocation);
            }

            ResetCooldown();
        }
    }
}
