using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Detect")]
public class DetectDecision : Decision {
    public override bool Decide(StateController controller) {
        return Detect(controller);
    }

    bool Detect(StateController controller) {
        if (controller.ai.fov.visibleTargets.Count > 0) {
            controller.ai.Aggro(controller.ai.fov.visibleTargets[0]);
            return true;
        // not sure if this works properly in all causes, may cause bugs
        } else if (controller.target) {
            return true;
        } else {
            return false;
        }
    }
}
