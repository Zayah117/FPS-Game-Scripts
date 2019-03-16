using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ChargeComplete")]
public class ChargeCompleteDecision : Decision {
    public override bool Decide(StateController controller) {
        return ChargeComplete(controller);
    }

    bool ChargeComplete(StateController controller) {
        ChargingAI chargingAI = controller.GetComponent<ChargingAI>();
        if (chargingAI != null) {
            return chargingAI.ChargeComplete();
        } else {
            Debug.LogWarning("Controller does not have ChargingAI component.");
            return false;
        }

    }
}
