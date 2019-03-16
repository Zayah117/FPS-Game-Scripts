using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(GuidComponent))]
public abstract class SaveableObject : MonoBehaviour {
    // [HideInInspector] public bool loadedObject = false;
    // [HideInInspector] public bool instantiated = false;

    public int UnityInstanceID {
        get {
            return gameObject.GetInstanceID();
        }
    }

    public Guid guid {
        get {
            return GetComponent<GuidComponent>().GetGuid();
        }
    }

    public byte[] serializedGuid {
        get {
            return GetComponent<GuidComponent>().GetGuid().ToByteArray();
        }
    }

    public string sceneName {
        get {
            return gameObject.scene.name;
        }
    }

    private void OnEnable() {
        Messenger.AddListener("subscribe", SubscribeManager);
    }

    private void OnDisable() {
        Messenger.RemoveListener("subscribe", SubscribeManager);
    }

    public virtual void Start () {
        // If the object is loaded from a save file it is subscribed to manager in SaveLoadManager.cs (in OnSceneLoad)
        /*
        if (!instantiated) {
            if (Managers.SaveLoad.sceneLoadedManually && !loadedObject) {
                Destroy(gameObject);
            } else if (!Managers.SaveLoad.sceneLoadedManually && !loadedObject) { 
                SubscribeManager();
            }
        }
        */
        // SubscribeManager();
	}

    public abstract GameData Save(GameData gameData, int id);

    public abstract void Load(GameData gameData, int id);

    protected void SubscribeManager() {
        Managers.SaveLoad.saveableObjects.Add(this);
    }

    protected void UnsubscribeManager() {
        Managers.SaveLoad.saveableObjects.Remove(this);
    }

    public void DestroySaveable() {
        UnsubscribeManager();
        Destroy(gameObject);
    }
}
