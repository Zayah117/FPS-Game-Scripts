using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
public class MovingPlatform : MonoBehaviour {
    [SerializeField] GameObject duplicate;
    bool forward = true;
    Vector3 beginPos;
    Vector3 endPos;

    [SerializeField] float speed = 2;
    float startTime;
    float travelLength;
    [SerializeField] bool looping;

    void Start() {
        beginPos = transform.position;
        endPos = duplicate.transform.position;
        Destroy(duplicate);

        startTime = Time.time;
        travelLength = Vector3.Distance(beginPos, endPos);
    }

    void Update() {
        Movement();
	}

    void Movement() {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionJourney = distanceCovered / travelLength;

        if (looping) {
            // Looping movement
            if (forward == true) {
                transform.position = Vector3.Lerp(beginPos, endPos, fractionJourney);
            } else {
                transform.position = Vector3.Lerp(endPos, beginPos, fractionJourney);
            }

            if (distanceCovered >= travelLength) {
                forward = !forward;
                startTime = Time.time;
            }
        } else {
            // Standard movement
            transform.position = Vector3.Lerp(beginPos, endPos, fractionJourney);
        }
    }
}
