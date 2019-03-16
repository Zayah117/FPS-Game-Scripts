using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObject : MonoBehaviour {
    public Spawner spawner;

    public void Unsubscribe() {
        spawner.UnsubscribeObject(this.gameObject);
    }
}
