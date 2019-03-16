using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    [SerializeField] WorldEvent worldEventObject;

    public void TriggerEvent() {
        if (worldEventObject != null) {
            worldEventObject.InvokeEvent();
        } else {
            Debug.LogWarning("No WorldEvent assigned to WorldTrigger.");
        }
    }
}
