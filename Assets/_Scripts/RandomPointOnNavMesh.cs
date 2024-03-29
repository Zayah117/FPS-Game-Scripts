﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomPointOnNavMesh : MonoBehaviour {
    public float range = 10f;

    void Update() {
        Vector3 point;
        if (RandomPoint(transform.position, range, out point)) {
            Debug.DrawRay(point, Vector3.up, Color.red, 1f);
        } else {
            Debug.LogWarning("Could not find nav mesh point.");
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result) {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1000f, NavMesh.AllAreas)) {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
