using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHelper : WeaponAnimationHelper {
    Player player;

    void Start() {
        player = character.GetComponent<Player>();
    }

    public override void FireCharacterWeapon() {
        base.FireCharacterWeapon();
    }

    public void Reload() {
        player.Reload();
        player.SetReloadTrigger(false);
    }

    public void Melee() {
        player.Melee();
    }

    public void ToggleMeleeAnimation() {
        player.ToggleMeleeAnimation();
    }

    public void LaunchGrenade() {
        player.Grenade();
    }

    public void Footstep() {
        Managers.Audio.PlayRandomFootstep();
    }
}
