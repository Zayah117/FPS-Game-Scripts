using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSkybox : MonoBehaviour {
    [SerializeField] float speed;

    float rotation;
	
	void Update () {
        rotation += speed * Time.deltaTime;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
	}
}
