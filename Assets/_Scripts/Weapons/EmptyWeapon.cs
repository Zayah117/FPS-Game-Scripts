using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyWeapon : Weapon {
    public override void Fire() {
        Debug.Log("Empty Weapon");
    }
}
