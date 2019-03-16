using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour {
    [SerializeField] ArenaSystem arenaSystem;

    void OnTriggerEnter(Collider other) {
        Player player = other.GetComponent<Player>();
        if (player) {
            BeginArena();
            Destroy(this.gameObject);
        }
    }

    void BeginArena() {
        arenaSystem.aiTarget = Managers.Gameplay.player.transform; // Set arena system target
        arenaSystem.BeginArena();
    }
}
