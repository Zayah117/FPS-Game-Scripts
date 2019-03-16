using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Wander")]
public class WanderAction : ActionAI {
    public override void Act(StateController controller) {
        Wander(controller);
    }

    void Wander(StateController controller) {
        controller.ai.astarAgent.isStopped = false;
        // Set a brand new move position at the beginning of the state
        if (!controller.movePositionSet) {
            // controller.ai.agent.SetDestination(AICharacter.RandomNavSphere(controller.ai.transform.position, controller.ai.wanderDistance));
            controller.ai.astarAgent.destination = AICharacter.RandomPointCircle(controller.ai.transform.position, controller.ai.wanderDistance);
            controller.movePositionSet = true;
        }
        if (Vector3.Distance(controller.ai.transform.position, controller.ai.astarAgent.destination) <= controller.ai.richAgent.endReachedDistance + 1) {
            // controller.ai.agent.SetDestination(AICharacter.RandomNavSphere(controller.ai.transform.position, controller.ai.wanderDistance));
            controller.ai.astarAgent.destination = AICharacter.RandomPointCircle(controller.ai.transform.position, controller.ai.wanderDistance);
        }
    }
}