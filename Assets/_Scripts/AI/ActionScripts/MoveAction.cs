using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Move")]
public class MoveAction : ActionAI {
    public override void Act(StateController controller) {
        Move(controller);
    }

    void Move(StateController controller) {
        // controller.ai.agent.isStopped = false;
        controller.ai.astarAgent.isStopped = false;
    }
}
