using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSystem : MonoBehaviour {
    [SerializeField] ArenaDoor[] doors;
    [SerializeField] ArenaSpawner[] spawners;
    [SerializeField] List<AICharacter> subscribedUnits;
    [HideInInspector] public Transform aiTarget;

    float endCheckTime = 3;

    void Awake() {
        foreach (ArenaSpawner spawner in spawners) {
            spawner.system = this;
        }
    }

    public void BeginArena() {
        LockDoors();
        EnableSpawners();
        StartCoroutine(CheckSubscribedUnitsAndSpawnQueue());
    }

    public void EndArena() {
        UnlockDoors();
        DisableSpawners();
    }

    void LockDoors() {
        for (int i = 0; i < doors.Length; i++) {
            doors[i].Lock();
        }
    }

    void UnlockDoors() {
        for (int i = 0; i < doors.Length; i++) {
            doors[i].Unlock();
        }
    }

    void EnableSpawners() {
        for (int i = 0; i < spawners.Length; i++) {
            spawners[i].EnableSpawner();
        }
    }

    // Might refactor so this destroys spawners rather than just delete them.
    void DisableSpawners() {
        for (int i = 0; i < spawners.Length; i++) {
            spawners[i].DisableSpawner();
        }
    }

    IEnumerator CheckSubscribedUnitsAndSpawnQueue() {
        while (true) {
            yield return new WaitForSeconds(endCheckTime);
            if (SystemOver()) {
                EndArena();
                StopCoroutine(CheckSubscribedUnitsAndSpawnQueue());
            }
        }
    }

    bool SystemOver() {
        foreach (ArenaSpawner spawner in spawners) {
            if (!spawner.QueueEnd() || spawner.ObjectsAreSubscribed()) {
                return false;
            }
        }
        foreach (AICharacter unit in subscribedUnits) {
            if (unit != null) {
                return false;
            }
        }
        return true;
    }
}
