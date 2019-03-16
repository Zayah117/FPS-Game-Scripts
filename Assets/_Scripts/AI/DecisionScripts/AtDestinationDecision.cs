using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AtDestination")]
public class AtDestinationDecision : Decision {
    public override bool Decide(StateController controller) {
        return AtDestination(controller);
    }

    bool AtDestination(StateController controller) {
        // return controller.ai.agent.remainingDistance <= controller.ai.agent.stoppingDistance;
        return controller.ai.astarAgent.remainingDistance <= controller.ai.richAgent.endReachedDistance;
    }
}
