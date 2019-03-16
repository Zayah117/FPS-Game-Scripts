using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameplayManager : MonoBehaviour, IGameManager {

    public ManagerStatus status { get; set; }

    [HideInInspector] public Player player;
    [HideInInspector] public float powerLevelX;
    [HideInInspector] public int combo;
    [HideInInspector] public int attackingEnemies;
    [HideInInspector] public List<AICharacter> aICharacters;
    public int availableAttackTokens = 1;

    public float powerLevelY {
        get {
            return GetYLogarithmicFunction(powerLevelX);
        }
        set {
            powerLevelX = GetXLogarithmicFunction(value);
        }
    }

    public float comboPower {
        get {
            return ComboPower();
        }
    }

    public float comboTimePercent {
        get {
            return comboTimeRemaining / comboTime;
        }
    }

    public float powerLevelDrain = 0.01f;
    public float comboTime = 1f;
    float comboTimeRemaining;

    Coroutine comboRoutine = null;

    void Update() {
        AddPowerY(-(powerLevelDrain * Time.deltaTime));
        DrainCombo();

        // AI attacks
        List<AICharacter> preparedForAttackAIs = new List<AICharacter>();
        preparedForAttackAIs = aICharacters.Where(x => x.InAttackPosition() == true).ToList() as List<AICharacter>;
        preparedForAttackAIs = preparedForAttackAIs.OrderBy(x => x.DistanceToTarget()).ToList();

        int tokensCurrentlyAvailable = availableAttackTokens - attackingEnemies;
        if (tokensCurrentlyAvailable > 0 && preparedForAttackAIs.Count > 0) {
            int iterations = tokensCurrentlyAvailable;
            if (tokensCurrentlyAvailable > preparedForAttackAIs.Count) {
                iterations = preparedForAttackAIs.Count;
            }
            for (int i = 0; i < iterations; i++) {
                preparedForAttackAIs[i].attackLocked = false;
            }
        }

        // Debug.Log(preparedForAttackAIs.Count);
        // Debug.Log(attackingEnemies);
    }

    public void Startup() {
        Debug.Log("Gameplay Manager starting...");

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        aICharacters.Clear();
        attackingEnemies = 0; // For manual loading (might only be needed as a temporary fix)

        status = ManagerStatus.Started;
    }

    public bool TokensAvailable() {
        if (attackingEnemies >= availableAttackTokens) {
            return false;
        } else {
            return true;
        }
    }

    public void AddCombo(int amount = 1) {
        combo += amount;
        /*
        if (comboRoutine != null) {
            StopCoroutine(comboRoutine);
        }
        comboRoutine = StartCoroutine(ResetComboAfterTime());
        */
        comboTimeRemaining = comboTime;
    }

    void DrainCombo() {
        comboTimeRemaining -= Time.deltaTime;
        if (combo > 0  && comboTimeRemaining < 0) {
            ResetCombo();
        }
    }

    IEnumerator ResetComboAfterTime() {
        yield return new WaitForSeconds(comboTime);
        ResetCombo();
    }

    void ResetCombo() {
        combo = 0;
    }

    float ComboPower() {
        if (combo >= 10) {
            return 0.5f;
        } else if (combo >= 5){
            return 0.25f;
        } else {
            return 0;
        }
        /*
        if (combo >= 20) {
            return 1f;
        } else if (combo >= 10) {
            return 0.66f;
        } else if (combo >= 5) {
            return 0.33f;
        } else {
            return 0;
        }
        */
    }

    public void AddPowerX(float power) {
        powerLevelX = Mathf.Clamp01(powerLevelX + power);
    }

    public void AddPowerY(float power) {
        powerLevelY = Mathf.Clamp01(powerLevelY + power);
    }

    // Logarithmic curve 
    // y - 0.95 = log base 10(x + 0.1)
    float GetYLogarithmicFunction(float x) {
        return Mathf.Clamp01(Mathf.Log10(x + 0.1f) + 0.95f);
    }

    float GetXLogarithmicFunction(float y) {
        return Mathf.Pow(10, (y - 0.95f)) - 0.1f;
    }

    float GetYQuadraticFunction(float a, float b, float c, float x) {
        return a * Mathf.Pow(x, 2) + b * x + c;
    }
}
