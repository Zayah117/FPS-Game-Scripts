using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePickup : StaticPickup {
    [SerializeField] int grenadeValue;

    protected override void Activate() {
        if (!player.AtMaxGrenades()) {
            player.AddGrenades(grenadeValue);
            ActivateFinish();
        }
    }
}
