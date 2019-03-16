using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationHelper : MonoBehaviour {
    public Character character;

    public virtual void FireCharacterWeapon() {
        character.FireWeapon();
    }
}
