using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {
    [SerializeField] Portal brother;
    public Transform exit;

    public void Teleport(Player player) {
        player.transform.position = brother.exit.position;
        player.QueueSnapRotation(brother.transform.localEulerAngles);
        player.QueueSnapVelocity(brother.exit.transform.forward);
    }
}
