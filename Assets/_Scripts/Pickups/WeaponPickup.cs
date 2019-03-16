using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : StaticPickup {
    [SerializeField] int weaponIndex;

    protected override void Activate() {
        player.PickupWeapon(weaponIndex);
        ActivateFinish();
    }
}
