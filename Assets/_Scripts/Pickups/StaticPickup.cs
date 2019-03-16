using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPickup : Pickup {
    [SerializeField] GameObject mesh;
    [SerializeField] float rotationSpeed;
    [SerializeField] protected bool respawn;
    [SerializeField] float respawnTime;

    void Update() {
        mesh.transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * rotationSpeed * 360, Space.World);
    }

    protected void EnablePickup() {
        mainObject.GetComponent<Collider>().enabled = true;
        mesh.GetComponent<MeshRenderer>().enabled = true;
    }

    protected void DisablePickup() {
        mainObject.GetComponent<Collider>().enabled = false;
        mesh.GetComponent<MeshRenderer>().enabled = false;
    }

    protected IEnumerator BeginRespawn() {
        yield return new WaitForSeconds(respawnTime);
        EnablePickup();
    }

    protected void ActivateFinish() {
        if (respawn) {
            DisablePickup();
            StartCoroutine(BeginRespawn());
        } else {
            DestroyPickup();
        }
    }
}
