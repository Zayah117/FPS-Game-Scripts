using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeHitbox : MonoBehaviour {
    [SerializeField] ChargingAI chargingAI;

    private void OnTriggerEnter(Collider other) {
        Character character = other.GetComponent<Character>();
        if (character != null && character != chargingAI) {
            chargingAI.DealChargeDamage(character);
        }
    }
}
