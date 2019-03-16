using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TargetVisible")]
public class TargetVisibleDecision : Decision {
    public override bool Decide(StateController controller) {
        return TargetIsVisible(controller);
    }

    bool TargetIsVisible(StateController controller) {
        if (controller.ai.fov.visibleTargets.Contains(controller.target)) {
            return true;
        } else {
            return false;
        }
    }

}
