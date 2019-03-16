using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MoveRandom")]
public class MoveRandomAction : ActionAI {
    public override void Act(StateController controller) {
        MoveRandom(controller);
    }

    void MoveRandom(StateController controller) {
        // controller.ai.agent.isStopped = false;
        controller.ai.astarAgent.isStopped = false;
        if (!controller.movePositionSet) {
            // controller.ai.agent.SetDestination(AICharacter.RandomNavSphere(controller.ai.transform.position, controller.ai.wanderDistance));
            controller.ai.astarAgent.destination = AICharacter.RandomPointCircle(controller.ai.transform.position, controller.ai.wanderDistance);
            controller.movePositionSet = true;
        }
    }
}
