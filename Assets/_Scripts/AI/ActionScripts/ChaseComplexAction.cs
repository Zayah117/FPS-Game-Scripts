using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "PluggableAI/Actions/ChaseComplex")]
public class ChaseComplexAction : ActionAI {
    public override void Act(StateController controller) {
        Chase(controller);
    }

    void Chase(StateController controller) {
        controller.ai.astarAgent.isStopped = false;
        if (controller.target != null && !controller.movePositionSet) {
            Vector3[] path = AICharacter.CalculatePath(controller.ai.seeker, controller.transform.position, controller.target.transform.position);
            Vector3 pointA = AICharacter.FindPointAlongPath(path, controller.ai.wanderDistance);
            Vector3 pointB = AICharacter.RandomPointCircle(pointA, controller.ai.wanderVariance);
            pointB = new Vector3(pointB.x, pointA.y, pointB.z); // Set pointB y equal to pointA y
            controller.ai.astarAgent.destination = pointB;
            controller.movePositionSet = true;

            /*
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(controller.transform.position, controller.target.transform.position, NavMesh.AllAreas, path)) {
                Vector3 pointA = AICharacter.FindPointAlongPath(path.corners, controller.ai.wanderDistance);
                Vector3 pointB = AICharacter.RandomNavSphere(pointA, controller.ai.wanderVariance);
                pointB = new Vector3(pointB.x, pointA.y, pointB.z); // Set pointB y equal to pointA y
                controller.ai.astarAgent.destination = pointB;
                controller.movePositionSet = true;
            }
            */
        }
    }
}
