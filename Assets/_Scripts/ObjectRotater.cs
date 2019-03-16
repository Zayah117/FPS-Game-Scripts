using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotater : MonoBehaviour {
    [SerializeField] float speed;

    float rotation;
	
	void Update () {
        rotation += speed * Time.deltaTime;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotation, transform.localEulerAngles.z);
	}
}
