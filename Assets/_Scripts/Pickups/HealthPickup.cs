using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : StaticPickup {
    [SerializeField] int healValue;
    [SerializeField] bool overheal;

    protected override void Activate() {
        if (!overheal && player.AtMaxHealth()) {
            return;
        }

        player.Heal(healValue);
        ActivateFinish();
    }
}
