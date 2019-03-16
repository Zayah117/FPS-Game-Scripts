using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DamageText : MonoBehaviour, IPooledObject {
    [SerializeField] GameObject textObject;
    [HideInInspector] public Animator animator;
    TextMesh textMesh;

    Vector3 offset;
    Vector3 randomOffset;

    void Awake() {
        textMesh = textObject.GetComponent<TextMesh>();
        animator = GetComponent<Animator>();
        offset = new Vector3(0, 1f, 0);
    }

    public void OnObjectSpawn() {
        transform.position += offset;

        randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        transform.position += randomOffset;
    }

    public void SetDamageText(string text) {
        textMesh.text = text;
    }
}
