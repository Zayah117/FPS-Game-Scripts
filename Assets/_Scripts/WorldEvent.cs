using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldEvent : MonoBehaviour
{
    [SerializeField] UnityEvent myEvent;
    public void InvokeEvent() {
        myEvent.Invoke();
    }
}
