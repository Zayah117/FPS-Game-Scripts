using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition {
    // public Decision decision;
    public DecisionStructure[] decisions;
    public DecisionType decisionType;
    public State trueState;
    public State falseState;
}

[System.Serializable]
public class DecisionStructure {
    public Decision decision;
    public bool value = true;
}

public enum DecisionType {
    ALL,
    ANY
};
