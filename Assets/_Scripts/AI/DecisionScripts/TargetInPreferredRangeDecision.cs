using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/TargetInPreferredRange")]
public class TargetInPreferredRangeDecision : Decision {
    public override bool Decide(StateController controller) {
        return (TargetInRange(controller, controller.ai.preferredAttackRange) && TargetIsVisible(controller));
    }

    bool TargetInRange(StateController controller, float range) {
        float distance = Vector3.Distance(controller.gameObject.transform.position, controller.target.transform.position);
        if (distance < range) {
            return true;
        } else {
            return false;
        }
    }

    bool TargetIsVisible(StateController controller) {
        if (controller.ai.fov.visibleTargets.Contains(controller.target)) {
            return true;
        } else {
            return false;
        }
    }
}
