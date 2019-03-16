using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntry : MonoBehaviour {
    [SerializeField] Portal portal;

    void OnTriggerEnter(Collider other) {
        Player player = other.GetComponent<Player>();
        if (player) {
            portal.Teleport(player);
        }
    }
}
