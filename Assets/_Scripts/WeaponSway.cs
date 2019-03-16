using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour {
    float mouseX;
    float mouseY;

    Quaternion rotationVector;

    public float slerpSpeed;
    public float speedMultiplier;
    public float maxRotation;


    void Update() {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        Vector2 rot = new Vector2(mouseX, mouseY).normalized;

        float xRot = Mathf.Clamp(-rot.x * speedMultiplier, -maxRotation, maxRotation);
        float yRot = Mathf.Clamp(rot.y * speedMultiplier, -maxRotation, maxRotation);

        rotationVector = Quaternion.Euler(yRot, xRot, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, rotationVector, slerpSpeed * Time.deltaTime);
    }
}
