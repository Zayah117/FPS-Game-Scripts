using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLift : MonoBehaviour {
    public float force = 100;

    void OnTriggerEnter(Collider other) {
        Player player = other.GetComponent<Player>();
        if (player != null) {
            Vector3 velocity = gameObject.transform.up * force;
            player.AddBonusVelocity(velocity);
        }
    }
}