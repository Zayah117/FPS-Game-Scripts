using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Stand")]
public class StandAction : ActionAI {
    public override void Act(StateController controller) {
        Stand(controller);
    }

    void Stand(StateController controller) {
        // controller.ai.agent.isStopped = true;
        controller.ai.astarAgent.isStopped = true;
    }
}
