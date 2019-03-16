using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable {
    Collider DamagableCollider { get; }
    Vector3 ColliderPosition { get; }
    void TakeDamage(int damage, Character offendingCharacter);
}
