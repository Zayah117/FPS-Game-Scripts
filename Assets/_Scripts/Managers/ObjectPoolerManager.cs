﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public void Startup() {
        Debug.Log("Object Pooler Manager starting...");

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

        status = ManagerStatus.Started;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, float seconds) {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Pool tag " + tag + " does not exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObject != null) {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        StartCoroutine(SetInactive(objectToSpawn, seconds));

        return objectToSpawn;
    }

    IEnumerator SetInactive(GameObject spawedObject, float seconds) {
        yield return new WaitForSeconds(seconds);
        spawedObject.SetActive(false);
    }
}
