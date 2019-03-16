using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour {

    public BaseState currentState;
    public BaseState remainState;
    public BaseState previousState;
    public BaseState stunnedState;
	public bool aiActive = true;

    [HideInInspector] public AICharacter ai;
    public Transform target;
    [HideInInspector] public float stateTimeElapsed;
    [HideInInspector] public bool movePositionSet;

    AIAnimationController aiAnimationController;

    // Holder to keep track of previous state
    BaseState previousStateVar;

	void Awake () 
	{
        ai = GetComponent<AICharacter>();
        aiAnimationController = GetComponent<AIAnimationController>();
	}

    void Start() {
        // Be sure to play the correct animation on start
        if (aiAnimationController) {
            SetAIAnimation(currentState.animationTrigger);
        }
    }

    void Update() {
        if (!aiActive) {
            return;
        }
        currentState.UpdateState(this);
    }

    public void TransitionToState(State nextState) {
        BaseState nextStateBase = nextState.GetBaseState();
        if (nextStateBase != remainState) {
            // Can probably refactor this a bit
            if (nextStateBase == previousState) {
                BaseState temp = currentState;

                currentState = previousStateVar;

                // For stun lock
                if (temp != stunnedState) {
                    previousStateVar = temp;
                }

            } else {
                // For stun lock
                if (currentState != stunnedState) {
                    previousStateVar = currentState;
                }
                currentState = nextStateBase;
            }
            if (aiAnimationController) {
                SetAIAnimation(currentState.animationTrigger);
            }
            // Debug.Log(currentState.name);
            OnExitState();
        }
    }

    public void SetAIAnimation(string animationString) {
        aiAnimationController.TriggerAnimation(animationString);
    }

    public bool CheckIfCountDownElapsed(float duration) {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    void OnExitState() {
        stateTimeElapsed = 0;
        if (previousStateVar.resetMovePositionOnExit) {
            movePositionSet = false;
        }
        ai.OnExitState();
    }

}