using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour {
    [SerializeField] int maxObjects;
    [SerializeField] protected float time;

    protected bool spawnQueueReady = true;

    protected List<GameObject> objectList = new List<GameObject>();

    public void EnableSpawner() {
        StartCoroutine(SpawnCheck());
    }

    public void DisableSpawner() {
        StopCoroutine(SpawnCheck());
    }

    protected virtual void Spawn() {
        Debug.Log("Default spawn function.");
    }

    protected bool MaxSpawns() {
        return objectList.Count >= maxObjects;
    }

    protected virtual IEnumerator SpawnCheck() {
        while (true) {
            if (spawnQueueReady && MaxSpawns() == false) {
                Spawn();
                StartCoroutine(SpawnQueue(time));
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    protected IEnumerator SpawnQueue(float time) {
        yield return new WaitForSeconds(time);
        spawnQueueReady = true;
    }

    public void SubscribeObject(GameObject myObject) {
        objectList.Add(myObject);
    }

    public void UnsubscribeObject(GameObject myObject) {
        objectList.Remove(myObject);
    }

    public Vector3 RandomPoint(Vector3 origin, float distance) {
        return AICharacter.RandomPointCircle(origin, distance);
    }

    public Vector3 RandomPointContstant(Vector3 origin, int searchLength) {
        return AICharacter.RandomPointConstantPath(origin, searchLength);
    }

    /*
    public Vector3 RandomPointConnected(Vector3 origin, float distance) {
        NavMeshPath path = new NavMeshPath();
        for (int i = 0; i < 10; i++) {
            Vector3 pos = AICharacter.RandomNavSphere(origin, distance);
            if (NavMesh.CalculatePath(transform.position, pos, NavMesh.AllAreas, path)) {
                if (path.status == NavMeshPathStatus.PathComplete) {
                    return pos;
                }
            }
        }
        Debug.LogWarning("Could not find connected location to spawn object.");
        return transform.position;
    }
    */
}
