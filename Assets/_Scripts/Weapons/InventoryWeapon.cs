using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class InventoryWeapon {
    public PlayerWeapon weaponPrefab;
    public GameObject weaponMesh;
    public Transform weaponFlashMuzzle;
    public GameObject weaponFlashLight;
    public bool locked;
    public int ammoPool;
}
