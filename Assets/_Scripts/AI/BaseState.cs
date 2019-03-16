using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/BaseState")]
public class BaseState : State {
    public bool resetMovePositionOnExit = true;
    public ActionAI[] actions;
    public string animationTrigger;

    public void UpdateState(StateController controller) {
        DoActions(controller);
        CheckTransitions(controller);
    }

    void DoActions(StateController controller) {
        for (int i = 0; i < actions.Length; i++) {
            actions[i].Act(controller);
        }
    }
}
