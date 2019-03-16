using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AllyInWay")]
public class AllyInWayDecision : Decision {

    public override bool Decide(StateController controller) {
        return AllyInWay(controller);
    }

    bool AllyInWay(StateController controller) {
        return !controller.ai.TargetPathClearOfAllies(true);
    }
}