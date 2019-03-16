using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon {
    LineRenderer laserEffect;

    void Start() {
        foreach (Transform child in character.transform) {
            if (child.tag == "LaserEffect") {
                laserEffect = child.gameObject.GetComponent<LineRenderer>();
            }
        }
        laserEffect.gameObject.SetActive(false);
    }

    void Update() {
        attackCooldown -= Time.deltaTime; // From original update function

        var points = new Vector3[2];

        points[1] = laserEffect.transform.InverseTransformPoint(character.targetLocation);

        laserEffect.SetPositions(points);

        if (laserEffect.gameObject.activeSelf) {
            if (!character.GetComponent<AICharacter>().currentlyAttacking) {
                laserEffect.gameObject.SetActive(false);
                }
        } 
    }

    public override void Fire() {
        if (!laserEffect.gameObject.activeSelf) {
            laserEffect.gameObject.SetActive(true);
        }
        base.Fire();
        if (attackCooldown <= 0) {
            Vector3 direction = (character.targetLocation - character.hitscanOrigin.position).normalized;
            FireHitScan(character.hitscanOrigin.position, direction, raycastDistance);
            ResetCooldown();
        }
    }
}
