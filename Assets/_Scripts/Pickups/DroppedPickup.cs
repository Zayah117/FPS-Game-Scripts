using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedPickup : PhysicsPickup {
    [SerializeField] int shieldValue;
    [SerializeField] float powerValue;

    protected override void Start() {
        base.Start();
        StartCoroutine(JumpRoutine(2f, 0.5f));
    }

    protected override void Activate() {
        player.AddShield(shieldValue);
        // Managers.Gameplay.AddPowerX(powerValue);
        DestroyPickup();
    }


    IEnumerator JumpRoutine(float time, float timeMod) {
        while (true) {
            float randomTime = Random.Range(time - timeMod, time + timeMod);
            yield return new WaitForSeconds(randomTime);
            Jump();
        }
    }

    void Jump() {
        physicsObject.AddForce(new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), Random.Range(-300, 300)));
        physicsObject.AddTorque(new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), Random.Range(-300, 300)));
    }
}
