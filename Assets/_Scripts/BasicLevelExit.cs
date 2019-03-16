using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicLevelExit : MonoBehaviour {
    [SerializeField] string sceneName = "";

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Managers.SaveLoad.LoadScene(sceneName);
        }
    }
}
