using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaDoor : Door {
    public override void Update() {
        if (PlayerIntersected() && !locked) {
            Open();
        } else {
            if (!remainOpen) {
                Close();
            }
        }
    }
}
