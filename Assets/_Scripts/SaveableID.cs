using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class SaveableID : MonoBehaviour
{
    static Dictionary<string, SaveableID> allGuids = new Dictionary<string, SaveableID>();

    public string uniqueId;

    #if UNITY_EDITOR

    void Update() {
        if (Application.isPlaying) {
            return;
        }

        string sceneName = gameObject.scene.name + "_";

        // If object is not part of the scene then it is a prefab
        if (sceneName == null) {
            return;
        }

        bool hasSceneNameAtBeginning = (uniqueId != null && uniqueId.Length > sceneName.Length && uniqueId.Substring(0, sceneName.Length) == sceneName);
        bool anotherComponentHasID = (uniqueId != null && allGuids.ContainsKey(uniqueId) && allGuids[uniqueId] != this);

        if (!hasSceneNameAtBeginning || anotherComponentHasID) {
            uniqueId = sceneName + Guid.NewGuid();
            EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        if (!allGuids.ContainsKey(uniqueId)) {
            allGuids.Add(uniqueId, this);
        }
    }

    void OnDestroy() {
        allGuids.Remove(uniqueId);
    }

    #endif
}
