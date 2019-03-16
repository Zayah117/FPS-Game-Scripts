using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/NodeWalk")]
public class NodeWalkAction : ActionAI {
    public override void Act(StateController controller) {
        NodeWalk(controller);
    }

    void NodeWalk(StateController controller) {
        NodeWalker nodeWalker = controller.GetComponent<NodeWalker>();

        if (nodeWalker == null) {
            Debug.LogError("StateController cannot take the NodeWalk action without a NodeWalker Compenent attached to character.");
        } else if (nodeWalker.nodeNetwork == null) {
            Debug.LogError("Selected node network does not have nodeNetwork assigned.");
        } else if (nodeWalker.nodeNetwork.nodes.Length <= 1) {
            Debug.LogError("Node network must have at least 2 nodes.");
        } else {
            // controller.ai.agent.isStopped = false;
            controller.ai.astarAgent.isStopped = false;
            if (!controller.movePositionSet) {
                nodeWalker.currentNode = 0;
                // controller.ai.agent.SetDestination(nodeWalker.nodeNetwork.nodes[nodeWalker.currentNode] + nodeWalker.nodeNetwork.transform.position);
                controller.ai.astarAgent.destination = nodeWalker.nodeNetwork.nodes[nodeWalker.currentNode] + nodeWalker.nodeNetwork.transform.position;
                controller.movePositionSet = true;
            }
            // if (Vector3.Distance(controller.ai.transform.position, controller.ai.agent.destination) <= controller.ai.agent.stoppingDistance + 1) {
            if (Vector3.Distance(controller.ai.transform.position, controller.ai.astarAgent.destination) <= controller.ai.richAgent.endReachedDistance + 1) {
                nodeWalker.currentNode += 1;
                if (nodeWalker.currentNode >= nodeWalker.nodeNetwork.nodes.Length) {
                    nodeWalker.currentNode = 0;
                }
                // controller.ai.agent.SetDestination(nodeWalker.nodeNetwork.nodes[nodeWalker.currentNode] + nodeWalker.nodeNetwork.transform.position);
                controller.ai.astarAgent.destination = nodeWalker.nodeNetwork.nodes[nodeWalker.currentNode] + nodeWalker.nodeNetwork.transform.position;
            }
        }
    }
}
