using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class WeaponSwitchController : MonoBehaviour {
    public InventoryWeapon[] inventoryWeapons;
    public InventoryWeapon meleeWeapon;
    public InventoryWeapon grenadeLauncher;

    Player player;
    public int currentWeaponIndex = 0;

    enum CurrentAttack { Normal, Melee, Grenade, Switch };

    CurrentAttack currentAttack = CurrentAttack.Normal;

    void Start() {
        player = GetComponent<Player>();
        player.characterMesh = inventoryWeapons[0].weaponMesh;
    }

    void SetActiveWeapon(InventoryWeapon newWeapon) {
        // Set all meshes to false
        for (int i = 0; i < inventoryWeapons.Length; i++) {
            inventoryWeapons[i].weaponMesh.SetActive(false);
        }
        meleeWeapon.weaponMesh.SetActive(false);
        grenadeLauncher.weaponMesh.SetActive(false);

        // Update weapon prefab
        player.weaponPrefab = newWeapon.weaponPrefab;
        player.UpdateWeaponInstance(player.weaponPrefab);

        // Activate weapon mesh
        newWeapon.weaponMesh.SetActive(true);
        player.characterMesh = newWeapon.weaponMesh;
        player.UpdateAnim();

        // Update flash muzzle location and muzzle light
        player.SetWeaponFlashObjects(newWeapon.weaponFlashMuzzle, newWeapon.weaponFlashLight);
    }

    public bool SwitchCurrentWeapon(int weaponIndex) {
        if (player.OutOfAmmo()) {
            return false;
        }
        // As long as the weapon is not locked, switch and return true
        if (!inventoryWeapons[weaponIndex].locked && currentAttack == CurrentAttack.Normal && (player.HasAmmo(weaponIndex) || player.HasAmmoPool(weaponIndex))) {
            currentWeaponIndex = weaponIndex;
            currentAttack = CurrentAttack.Switch; // Will switch back to normal next frame (ensures that weapon prefab and mesh get updated)
            return true;
        // Else return false
        } else {
            return false;
        }
    }

    // Used in player UpdateAmmoPool and player Load
    public void ForceSwitchCurrentWeapon(int weaponIndex) {
        if (!inventoryWeapons[weaponIndex].locked) {
            currentWeaponIndex = weaponIndex;
            currentAttack = CurrentAttack.Switch;
        }
    }

    public void ActiveWeaponController(bool meleeTrigger, bool grenadeTrigger) {
        if (!player.FireAnimationPlaying() && !player.ReloadAnimationPlaying()) {
            if (grenadeTrigger) {
                if (currentAttack != CurrentAttack.Grenade) {
                    SetActiveWeapon(grenadeLauncher);
                    currentAttack = CurrentAttack.Grenade;
                }
            } else if (meleeTrigger) {
                if (currentAttack != CurrentAttack.Melee) {
                    SetActiveWeapon(meleeWeapon);
                    currentAttack = CurrentAttack.Melee;
                }
            } else {
                if (currentAttack != CurrentAttack.Normal) {
                    SetActiveWeapon(inventoryWeapons[currentWeaponIndex]);
                    currentAttack = CurrentAttack.Normal;
                }
            }
        }
    }

    public void NextWeapon(int index, bool previousWeapon = false) {
        if (currentAttack == CurrentAttack.Normal && !player.ReloadAnimationPlaying()) {
            if (previousWeapon) {
                index -= 1;
                if (index < 0) {
                    index = inventoryWeapons.Length - 1;
                }
            } else {
                index += 1 ;
                if (index > inventoryWeapons.Length - 1) {
                    index = 0;
                }
            }
            // If the weapon is locked, use recursion and move to next weapon
            // Loops back to the current weapon if there is only one weapon unlocked
            if (!SwitchCurrentWeapon(index)) {
                if (previousWeapon) {
                    NextWeapon(index, true);
                } else {
                    NextWeapon(index);
                }
            }
        }
    }

    public bool AttackSetNormal() {
        if (currentAttack == CurrentAttack.Normal) {
            return true;
        } else {
            return false;
        }
    }

    public bool AttackSetMelee() {
        if (currentAttack == CurrentAttack.Melee) {
            return true;
        } else {
            return false;
        }
    }

    public bool AttackSetGrenade() {
        if (currentAttack == CurrentAttack.Grenade) {
            return true;
        } else {
            return false;
        }
    }

    public bool AttackSetSwitch() {
        if (currentAttack == CurrentAttack.Switch) {
            return true;
        } else {
            return false;
        }
    }

    public void DryFireSound() {
        inventoryWeapons[currentWeaponIndex].weaponPrefab.DryFireSound();
    }
}
