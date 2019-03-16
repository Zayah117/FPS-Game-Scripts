using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSpawner : Spawner {
    [SerializeField] GameObject spawnObject;
    [SerializeField] float spawnRadius = 1f;

    void Start() {
        EnableSpawner();
    }

    protected override void Spawn() {
        GameObject myObject;
        SaveableResource saveable = spawnObject.GetComponent<SaveableResource>();
        if (saveable != null) {
            myObject = SaveableResource.InstantiateSaveableResource(saveable, RandomPointContstant(transform.position, (int)spawnRadius*1000), Quaternion.identity).gameObject;
        } else {
            myObject = Instantiate(spawnObject, RandomPointContstant(transform.position, (int)spawnRadius*1000), Quaternion.identity);
        }
        myObject.AddComponent<SpawnableObject>().spawner = this;
        SubscribeObject(myObject);
        spawnQueueReady = false;
    }
}
