using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "PluggableAI/State")]
public class State : ScriptableObject {
    public State parentState;
    public Transition[] transitions;
    int NumParents {
        get { return GetNumParents(this); }
    }

    private void OnEnable() {
        // Default values for scriptable object
        if (transitions == null) {
            transitions = new Transition[1];

            for (int i = 0; i < transitions.Length; i++) {
                transitions[i] = new Transition();
                transitions[i].decisions = new DecisionStructure[1];
            }
        } else {
            foreach (Transition transition in transitions) {
                for (int i = 0; i < transition.decisions.Length; i++) {
                    if (transition.decisions[i] == null) {
                        transition.decisions[i] = new DecisionStructure();
                    }
                }
            }
        }
    }

    protected void CheckTransitions(StateController controller) {
        // First check parent transitions
        if (parentState != null) {
            parentState.CheckTransitions(controller);
        }

        // Check this states transitions
        for (int i = 0; i < transitions.Length; i++) {
            CheckTransition(controller, i);
        }
    }

    void CheckTransition(StateController controller, int index) {
        if (controller.currentState == this || this.ParentOf(this, controller.currentState)) {
            for (int j = 0; j < transitions[index].decisions.Length; j++) {
                bool decisionSucceeded = transitions[index].decisions[j].decision.Decide(controller);

                // Reverse decisionSucceded if value was supposed to be false
                if (transitions[index].decisions[j].value == false) {
                    decisionSucceeded = !decisionSucceeded;
                }

                // If type is set to ANY and any one of the decisions succeed go to true
                if (transitions[index].decisionType == DecisionType.ANY && decisionSucceeded) {
                    controller.TransitionToState(transitions[index].trueState);
                    return;
                }

                // If type is set to ALL and any of the decisions fail go to false
                if (transitions[index].decisionType == DecisionType.ALL && !decisionSucceeded) {
                    controller.TransitionToState(transitions[index].falseState);
                    return;
                }

                /*
                // Debug.Log("DECISION: (in state " + this.name + ") " + transitions[i].decision.ToString() + " = " + decisionSucceeded.ToString());
                if (decisionSucceeded) {
                    controller.TransitionToState(transitions[i].trueState);
                } else {
                    controller.TransitionToState(transitions[i].falseState);
                }
                */
            }

            // If type is set to ALL and all succeed go to true
            if (transitions[index].decisionType == DecisionType.ALL) {
                controller.TransitionToState(transitions[index].trueState);
                return;
            // If type is set to ANY and none succeed go to false
            } else {
                controller.TransitionToState(transitions[index].falseState);
                return;
            }
        }
    }

    public BaseState GetBaseState() {
        if (IsBaseState(this)) {
            return (BaseState)this;
        } else if (IsBubbleState(this)) {
            BubbleState bs = (BubbleState)this;
            if (bs.entryState.GetType() == typeof(BaseState)) {
                return (BaseState)bs.entryState;
            } else {
                return bs.entryState.GetBaseState();
            }
        } else {
            Debug.LogError("GetBaseState() is returning null. This most likely means you are using a state that is not of type BaseState or BubbleState.");
            return null;
        }
    }

    public bool ParentOf(State stateToTest, State state) {
        if (state.parentState == null) {
            return false;
        }

        if (state.parentState == stateToTest) {
            return true;
        } else {
            return ParentOf(stateToTest, state.parentState);
        }
    }

    // Needs testing, currently unused
    int GetNumParents(State state, int num = 0) {
        if (parentState == null) {
            return num;
        } else {
            num += 1;
            num = GetNumParents(parentState, num);
            return num;
        }
    }

    public static bool IsBaseState(State state) {
        if (state.GetType() == typeof(BaseState)) {
            return true;
        } else {
            return false;
        }
    }

    public static bool IsBubbleState(State state) {
        if (state.GetType() == typeof(BubbleState)) {
            return true;
        } else {
            return false;
        }
    }

    /*
    public static BaseState GetBaseState(State state) {
        if (IsBaseState(state)) {
            return (BaseState)state;
        }
        return null;
    }

    public static BubbleState GetBubbleState(State state) {
        if (IsBubbleState(state)) {
            return (BubbleState)state;
        }
        return null;
    }
    */
}
