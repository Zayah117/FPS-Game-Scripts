using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMaskManager : MonoBehaviour {
    public LayerMask DefaultRaycastLayerMask;
    public LayerMask PlayerRaycastLayerMask;
    public LayerMask PlayerHitGroundLayerMask;
    public LayerMask WorldObjectLayerMask;
    public LayerMask InteractableLayerMask;
}
