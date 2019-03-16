using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbableItem : MonoBehaviour {
    [SerializeField] GameObject mainObject;
    [SerializeField] float activateDistance = 5f;
    float speed = 20f;
    bool isEnabled = false;

    void Start() {
        StartCoroutine(CheckEnable());
    }

    void Update() {
        if (isEnabled) {
            mainObject.transform.position = Vector3.Lerp(mainObject.transform.position, Managers.Gameplay.player.transform.position, speed * Time.deltaTime);
        }
    }

    void ActivateAbsorb() {
        isEnabled = true;
        Rigidbody rb = mainObject.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
        }
    }

    IEnumerator CheckEnable() {
        while (true) {
            yield return new WaitForSeconds(0.2f);
            if (Vector3.Distance(Managers.Gameplay.player.transform.position, transform.position) < activateDistance) {
                ActivateAbsorb();
                StopCoroutine(CheckEnable());
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activateDistance);
    }
}
