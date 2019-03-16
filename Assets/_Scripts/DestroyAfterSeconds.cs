using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour {
    [SerializeField] float seconds;

	void Start () {
        StartCoroutine(DestroyAfterTime());
	}

    IEnumerator DestroyAfterTime() {
        yield return new WaitForSeconds(seconds);
        SaveableObject saveable = gameObject.GetComponent<SaveableObject>();
        if (saveable != null) {
            saveable.DestroySaveable();
        } else {
            Destroy(gameObject);
        }
    }
}
