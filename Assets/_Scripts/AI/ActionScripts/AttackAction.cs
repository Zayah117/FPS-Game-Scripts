using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : ActionAI {
    public override void Act(StateController controller) {
        Attack(controller);
    }

    void Attack(StateController controller) {
        if (controller.target != null) {
            if (!controller.ai.currentlyAttacking) {
                controller.ai.BorrowAttackToken();
            }
            // controller.ai.agent.isStopped = true;
            controller.ai.astarAgent.isStopped = true;
            controller.ai.FaceTarget(controller.target);

            // For leading bullets
            /*
            Player player = controller.target.GetComponent<Player>();
            if (player != null) {
                controller.ai.targetLocation = AICharacter.FirstOrderIntercept(controller.ai.transform.position, Vector3.zero, 20f, controller.target.position, new Vector3(player.Velocity.x, 0, player.Velocity.z));
            } else {
                controller.ai.targetLocation = controller.target.position;
            }
            */
            
            controller.ai.targetLocation = controller.target.position;
            controller.ai.SetFireTrigger(true);
        }
    }
}
