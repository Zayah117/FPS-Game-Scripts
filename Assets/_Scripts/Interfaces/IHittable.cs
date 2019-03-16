using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable {
    void EmitHitParticle(Vector3 pos, Vector3 origin);
}
