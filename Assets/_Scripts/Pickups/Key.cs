using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : StaticPickup {
    [SerializeField] int keyIndex;

    protected override void Activate() {
        player.AddKey(keyIndex);
        ActivateFinish();
    }
}
