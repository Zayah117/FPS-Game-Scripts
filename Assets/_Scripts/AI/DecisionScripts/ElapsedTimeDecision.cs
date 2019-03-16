using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/ElapsedTime")]
public class ElapsedTimeDecision : Decision {
    public override bool Decide(StateController controller) {
        return TimeElapsed(controller, controller.ai.elapseTime);
    }

    bool TimeElapsed(StateController controller, float time) {
        return controller.CheckIfCountDownElapsed(time);
    }
}
