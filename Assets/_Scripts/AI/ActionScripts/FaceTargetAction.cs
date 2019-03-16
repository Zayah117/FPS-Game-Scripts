using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/FaceTarget")]
public class FaceTargetAction : ActionAI {
    public override void Act(StateController controller) {
        FaceTarget(controller);
    }

    void FaceTarget(StateController controller) {
        if (controller.target != null) {
            // controller.ai.agent.isStopped = true;
            controller.ai.astarAgent.isStopped = true;
            controller.ai.FaceTarget(controller.target);
        }
    }
}
