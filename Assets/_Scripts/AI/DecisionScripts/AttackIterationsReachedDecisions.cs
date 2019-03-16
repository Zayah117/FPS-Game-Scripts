using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackIterationsReached")]
public class AttackIterationsReachedDecisions : Decision {
    public override bool Decide(StateController controller) {
        return AttackIterationsReached(controller);
    }

    bool AttackIterationsReached(StateController controller) {
        return controller.ai.ReachedAttackIterations();
    }
}
