using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
public class ChaseAction : ActionAI {
    public override void Act(StateController controller) {
        Chase(controller);
    }

    void Chase(StateController controller) {
        // controller.ai.agent.isStopped = false;
        controller.ai.astarAgent.isStopped = false;
        if (controller.target != null) {
            // controller.ai.agent.destination = controller.target.position;
            controller.ai.astarAgent.destination = controller.target.position;
        }
    }
}
