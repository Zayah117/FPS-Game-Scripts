using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeNetwork : MonoBehaviour {
    public Vector3[] nodes = new Vector3[2];

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1.5f);
        for (int i = 0; i < nodes.Length; i++) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(nodes[i] + transform.position, 1.5f);
        }
    }
}
