using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingAI : AICharacter {
    [SerializeField] int chargeDamage = 10;
    [SerializeField] float chargeSpeed = 20f;
    [SerializeField] float chargeAcceleration = 10f;
    [SerializeField] Collider chargeCollider;

    /*
    [HideInInspector] public bool chargeBegunState;
    [HideInInspector] public bool p;

    bool chargeEnabled;
    bool highSpeedReached;
    */
    public bool chargeBegunState;
    public bool p;

    [SerializeField] bool chargeEnabled;
    [SerializeField] bool highSpeedReached;

    float defaultSpeed;
    float defaultAcceleration;

    float VelocityMagnitude {
        get { return richAgent.velocity.magnitude; }
    }

    float Speed {
        get { return astarAgent.maxSpeed; }
        set { astarAgent.maxSpeed= value; }
    }

    float Acceleration {
        get { return richAgent.acceleration; }
        set { richAgent.acceleration = value; }
    }

    public override void Start() {
        base.Start();
        defaultSpeed = speed;
        defaultAcceleration = Acceleration;
        chargeCollider.enabled = false;
    }

    public override void Update() {
        base.Update();

        if (chargeEnabled) {
            if (!highSpeedReached) {
                if (VelocityMagnitude >= (chargeSpeed * 0.95f)) {
                    highSpeedReached = true;
                    EnableChargeAcceleration();
                }
            } else {
                if (VelocityMagnitude <= speed) {
                    EndCharge();
                }
            }

        }
    }

    public void BeginCharge() {
        chargeEnabled = true;
        chargeBegunState = true;
        highSpeedReached = false;
        EnableChargeSpeed();
        chargeCollider.enabled = true;
    }

    public void EndCharge() {
        chargeEnabled = false;
        ResetSpeedAcceleration();
        chargeCollider.enabled = false;
    }

    void EnableChargeSpeed() {
        Speed = chargeSpeed;
    }

    void EnableChargeAcceleration() {
        Acceleration = chargeAcceleration;
    }

    void ResetSpeedAcceleration() {
        Speed = defaultSpeed;
        Acceleration = defaultAcceleration;
    }

    public bool ChargeComplete() {
        if ((chargeBegunState && !chargeEnabled) || p) {
            if (p) {
                EndCharge();
            }
            p = false;
            return true;
        } else {
            return false;
        }
    }

    void EndChargePrematurely() {
        if (chargeEnabled && highSpeedReached) {
            p = true;
        }
    }

    public void DealChargeDamage(Character other) {
        other.TakeDamage(chargeDamage, this);
        EndChargePrematurely();
    } 

    public override void TakeDamage(int damage, Character offendingCharacter) {
        base.TakeDamage(damage, offendingCharacter);
    }

    public override void BorrowAttackToken() {
        base.BorrowAttackToken();
        if (!chargeBegunState) {
            BeginCharge();
        }
    }

    public override void Stun() {
        if (chargeBegunState) {
            EndChargePrematurely();
        } else {
            base.Stun();
        }
    }

    public override void OnExitState() {
        base.OnExitState();
        chargeBegunState = false;
    }
}
