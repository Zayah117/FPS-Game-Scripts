using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : StaticPickup {
    [SerializeField] int ammoValue;
    [SerializeField] int index;

    protected override void Activate() {
        player.UpdateAmmoPool(ammoValue, index);
        ActivateFinish();
    }
}
