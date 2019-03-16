using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateController))]
public class AIAnimationController : MonoBehaviour {
    Animator anim;
    StateController stateController;

    void Start() {
        stateController = GetComponent<StateController>();
        anim = stateController.ai.characterMesh.GetComponent<Animator>();
    }

    public void TriggerAnimation(string animationString) {
        if (anim) {
            anim.SetTrigger(animationString);
        }
    }
}
