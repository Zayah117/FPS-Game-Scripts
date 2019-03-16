using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ViewAngle {
    public enum ViewType {
        Idle,
        Active
    }

    public float viewRadius;
    [Range(0, 360)]public float viewAngle;
    public ViewType viewType = ViewType.Idle;

    public ViewAngle(float _viewRadius, float _viewAngle, ViewType _viewType) {
        this.viewRadius = _viewRadius;
        this.viewAngle = _viewAngle;
        this.viewType = _viewType;
    }
}
