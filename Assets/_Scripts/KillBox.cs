using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.gameObject == Managers.Gameplay.player.gameObject) {
            other.GetComponent<Player>().Die(other.GetComponent<Character>());
        }
    }
}
