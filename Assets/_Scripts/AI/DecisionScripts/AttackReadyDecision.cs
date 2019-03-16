using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackReady")]
public class AttackReadyDecision : Decision {
    public override bool Decide(StateController controller) {
        return AttackReady(controller);
    }

    bool AttackReady(StateController controller) {
        // return controller.ai.attackCycleReady && Managers.Gameplay.TokensAvailable() && controller.ai.fov.visibleTargets.Contains(controller.target);
        return controller.ai.AttackReady();
    }
}
