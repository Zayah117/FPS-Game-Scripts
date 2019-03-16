using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Charge")]
public class ChargeAction : ActionAI {

    public override void Act(StateController controller) {
        // Begin charge if it hasn't started yet
        if (controller.target != null) {
            if (!controller.ai.currentlyAttacking) {
                controller.ai.BorrowAttackToken();
            }

            ChargeChase(controller);
        }
    }

    void ChargeChase(StateController controller) {
        // controller.ai.agent.isStopped = false;
        controller.ai.astarAgent.isStopped = false;
        if (controller.target != null) {
            // controller.ai.agent.destination = controller.target.position;
            controller.ai.astarAgent.destination = controller.target.position;
        }
    }
}
