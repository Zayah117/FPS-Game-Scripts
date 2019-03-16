using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Stunned")]
public class StunnedDecision: Decision {
    public override bool Decide(StateController controller) {
        return controller.ai.stunned; 
    }
}
