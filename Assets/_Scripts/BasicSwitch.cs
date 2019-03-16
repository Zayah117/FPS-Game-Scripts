using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldTrigger))]
public class BasicSwitch : MonoBehaviour, IInteractable
{
    public void Interact() {
        GetComponent<WorldTrigger>().TriggerEvent();
    }
}
