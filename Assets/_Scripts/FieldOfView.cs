using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=rQG9aUWarwE&list=PLFt_AvWsXl0dohbtVgHDNmgZV_UY7xZv7
public class FieldOfView : MonoBehaviour {
    /*
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    */
    public ViewAngle[] views;

    [SerializeField] float yModifier;

    public LayerMask targetMask;
    public LayerMask allyMask;
    public LayerMask obstacalMask;

    public bool isActive;

    [HideInInspector]public List<Transform> visibleTargets = new List<Transform>();

    public Vector3 ViewPosition {
        get { return new Vector3(transform.position.x, transform.position.y + yModifier, transform.position.z); }
    }

    void Start() {
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            // Debug.Log(visibleTargets.Count);
        }
    }

    void FindVisibleTargets() {
        visibleTargets.Clear();

        foreach(ViewAngle view in views) {
            if (!isActive && view.viewType == ViewAngle.ViewType.Active) {
                return;
            }
            float viewRadius = view.viewRadius;
            float viewAngle = view.viewAngle;

            // Get everything within radius
            Collider[] targetsInViewRadius = Physics.OverlapSphere(ViewPosition, viewRadius, targetMask);

            for (int i=0; i < targetsInViewRadius.Length; i++) {
                Transform target = targetsInViewRadius[i].transform;
                // As long as target is not already in list of targets, shoot a raycast to check line of sight
                if (!visibleTargets.Contains(target)) {
                    Vector3 dirToTarget = (target.position - ViewPosition).normalized;
                    if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2) {
                        float dstToTarget = Vector3.Distance(ViewPosition, target.position);

                        if (!Physics.Raycast(ViewPosition, dirToTarget, dstToTarget, obstacalMask)) {
                           visibleTargets.Add(target);
                        }
                    }
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
