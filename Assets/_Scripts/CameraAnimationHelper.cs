using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimationHelper : MonoBehaviour {
    public void Footstep() {
        Managers.Audio.PlayRandomFootstep();
    }
}
