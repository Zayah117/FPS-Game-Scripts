using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPickup: StaticPickup {
    [SerializeField] int armorValue;

    protected override void Activate() {
        if (!player.AtMaxArmor()) {
            player.AddArmor(armorValue);
            ActivateFinish();
        }
    }
}