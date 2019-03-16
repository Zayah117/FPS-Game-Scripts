using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {
    protected Player player;
    [SerializeField] protected GameObject mainObject;
    /*
    [HideInInspector] public bool isEnabled = false;

    [SerializeField] float pickupDistance = 0.2f;
    */

    /*
    void Start() {
        player = GameplayManager.instance.player;
    }
    */

    /*
    void Update() {
        if (isEnabled) {
            if (Vector3.Distance(player.transform.position, transform.position) < pickupDistance) {
                Activate();
                Destroy(this.gameObject);
            }
        }
    }
    */

    protected virtual void Start() {
        player = Managers.Gameplay.player;
    }

    void OnTriggerEnter(Collider other) {
        if (IsPlayer(other)) {
            Activate();
        }
    }

    bool IsPlayer(Collider other) {
        Player playerComponent = other.GetComponent<Player>();
        if (playerComponent != null) {
            if (playerComponent == player) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    protected virtual void Activate() {
        Debug.Log("Activate!");
    }

    protected void DestroyPickup() {
        Destroy(mainObject);
    }
}
