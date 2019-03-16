using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : Door {
    public int keyIndex;

    public override void Update() {
        if (PlayerIntersected() && Managers.Gameplay.player.HasKey(keyIndex)) {
            Open();
        } else {
            if (!remainOpen) {
                Close();
            }
        }
    }
}
