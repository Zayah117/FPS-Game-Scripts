using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PointBehindTarget : MonoBehaviour {
    Transform target;
    Vector3 destination;
    NavMeshAgent agent;
    NavMeshPath path;
    public float behindDistance = 10;
    public LayerMask layermask;

    void Start() {
        target = Managers.Gameplay.player.transform;
        destination = new Vector3();
        path = new NavMeshPath();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 targetLocation = transform.position + direction * (distance + behindDistance);
        destination = new Vector3(targetLocation.x, target.position.y, targetLocation.z);
        NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        if (Input.GetKeyDown(KeyCode.Y)) {
            agent.SetDestination(destination);
        }
        agent.SetDestination(destination);
        Debug.Log(agent.velocity.magnitude);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(destination, 1f);

        Gizmos.color = Color.red;
        if (path != null) {
            for (int i = 0; i < path.corners.Length; i ++) {
                Gizmos.DrawSphere(path.corners[i], 0.5f);
            }
        }
    }
}
