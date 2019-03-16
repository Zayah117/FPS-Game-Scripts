using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimationHelper : MonoBehaviour {
    public AICharacter aiCharacter;

    public void ResetStun() {
        aiCharacter.ResetStun();
    }
}
