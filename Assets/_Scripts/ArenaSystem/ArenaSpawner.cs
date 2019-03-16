using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ArenaSpawner : Spawner {
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject[] spawnQueue;
    [HideInInspector] public ArenaSystem system;
    int queueIndex;

    void Awake() {
        // Clamp each spawn point to nav mesh
        foreach (GameObject spawnPoint in spawnPoints) {
            spawnPoint.AddComponent<NavmeshClamp>();
            spawnPoint.GetComponent<NavmeshClamp>().ClosestPositionClamp();
        }
    }

    protected override void Spawn() {
        GameObject myObject;
        SaveableResource saveable = spawnQueue[queueIndex].GetComponent<SaveableResource>();
        Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        if (saveable != null) {
            myObject = SaveableResource.InstantiateSaveableResource(saveable, spawnPosition, Quaternion.identity).gameObject;
        } else {
            myObject = Instantiate(spawnQueue[queueIndex], spawnPosition, Quaternion.identity);
        }

        StateController stateController = myObject.GetComponent<StateController>();
        if (stateController) {
            stateController.target = system.aiTarget;
        }
        
        queueIndex += 1;
        myObject.AddComponent<SpawnableObject>().spawner = this;
        SubscribeObject(myObject);
        spawnQueueReady = false;
    }

    protected override IEnumerator SpawnCheck() {
        while (true) {
            if (spawnQueueReady && MaxSpawns() == false && !QueueEnd()) {
                Spawn();
                StartCoroutine(SpawnQueue(time));
            }
            yield return new WaitForSeconds(1);
        }
    }

    public bool QueueEnd() {
        if (queueIndex >= spawnQueue.Length) {
            return true;
        } else {
            return false;
        }
    }

    public bool ObjectsAreSubscribed() {
        if (objectList.Count > 0) {
            return true;
        } else {
            return false;
        }
    }
}
