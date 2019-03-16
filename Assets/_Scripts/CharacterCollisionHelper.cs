using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollisionHelper : MonoBehaviour, IDamagable
{
    [SerializeField] Character character;

    public Collider DamagableCollider {
        get {
            return character.DamagableCollider;
        }
    }

    public Vector3 ColliderPosition {
        get {
            return character.ColliderPosition;
        }
    }

    public void TakeDamage(int damage, Character offendingCharacter) {
        character.TakeDamage(damage, offendingCharacter);
    }
}
