using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MoveTowardTargetRandom")]
public class MoveTowardTargetRandomAction : ActionAI {
    public override void Act(StateController controller) {
        MoveRandomTowardTarget(controller);
    }

    void MoveRandomTowardTarget(StateController controller) {
        // controller.ai.agent.isStopped = false;
        controller.ai.astarAgent.isStopped = false;
        if (!controller.movePositionSet) {
            // controller.ai.agent.SetDestination(AICharacter.RandomNavHemisphereTowardTarget(controller.ai.transform.position, controller.target.position, controller.ai.wanderDistance, 10f, -1));
            controller.ai.astarAgent.destination = AICharacter.RandomNavHemisphereTowardTarget(controller.ai.transform.position, controller.target.position, controller.ai.wanderDistance, 10f, -1);
            controller.movePositionSet = true;
        }
    }
}
