using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AICharacter))]
[RequireComponent(typeof(NavMeshAgent))]
public class NodeWalker : MonoBehaviour {
    public NodeNetwork nodeNetwork;
    public int currentNode;
}
