using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TargetActive")]
public class TargetActiveDecision : Decision {
    public override bool Decide(StateController controller) {
        return TargetIsActive(controller);
    }

    bool TargetIsActive(StateController controller) {
        // if (controller.target.gameObject.activeSelf && controller.target.gameObject != null) {
        if (controller.target != null) {
            return true;
        } else {
            controller.target = null;
            return false;
        }
    }

}
