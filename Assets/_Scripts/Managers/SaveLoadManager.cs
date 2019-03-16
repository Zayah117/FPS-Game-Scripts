using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class SaveLoadManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; set; }

    public SceneLoadMethod sceneLoadMethod;

    string datapath;

    // [HideInInspector] public bool sceneLoadedManually = false;
    public List<SaveableObject> saveableObjects;

    public void Startup() {
        Debug.Log("Save/Load Manager starting...");

        saveableObjects = new List<SaveableObject>();
        datapath = Application.persistentDataPath + "/SaveFiles/";

        status = ManagerStatus.Started;
    }

    List<SaveableObject> SaveableObjectsExcludePlayer {
        get { return saveableObjects.Where(s => s.gameObject.tag != "Player").ToList() as List<SaveableObject>; }
    }

    SaveableObject PlayerSaveable {
        get { return Managers.Gameplay.player.GetComponent<SaveableObject>(); }
    }

    GameData pGameData;

    /*
    void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != null) {
            Destroy(gameObject);
        }

        saveableObjects = new List<SaveableObject>();
        datapath = Application.persistentDataPath + "/SaveFiles/";
    }
    */

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Should not be needed but implemented as a precaution
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Messenger.Broadcast("subscribe");

        // Default scene load just returns

        if (pGameData == null) {
            Debug.Log("Default scene loaded.");
            sceneLoadMethod = SceneLoadMethod.Default;
            return;
        }

        Debug.Log("Loading data from pGamedata.");
        for (int i = 0; i < pGameData.saveableObjectGuids.Length; i++) {
            // Debug.Log(saveableObjects[i].guid.ToString());
            // Debug.Log(pGameData.saveableObjectGuids[i]);
            SaveableObject saveableObject = SaveableObjectsExcludePlayer.First(s => s.guid.ToString() == pGameData.saveableObjectGuids[i]);
            saveableObject.Load(pGameData, 0);
        }

        // Load Player
        PlayerSaveable.Load(pGameData, -1);



        /*
        if (sceneLoadedManually) {
            // Spawn all saved resources and add them to saveable list
            // Player is subscribed through Player.cs Start() method
            for (int i = 0; i < pGameData.spawnResources.Length; i++) {
                GameObject gameObject = Instantiate(Resources.Load(pGameData.spawnResources[i], typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
                gameObject.GetComponent<SaveableObject>().loadedObject = true;

                saveableObjects.Add(gameObject.GetComponent<SaveableObject>());
            }

            // Load all saveable objects
            for (int i = 0; i < saveableObjects.Count; i++) {
                saveableObjects[i].Load(pGameData, i);
            }

            // Load Player
            PlayerSaveable.Load(pGameData, -1);
        }
        */


        Managers.PauseMenu.UnpauseGame();
    }

    public void NewSave() {
        if (!Directory.Exists(datapath)) {
            Directory.CreateDirectory(datapath);
        }

        BinaryFormatter bf = new BinaryFormatter();
        string fileName = SceneManager.GetActiveScene().name + "_" + UniqueID() + ".dat";
        FileStream file = File.Create(datapath + fileName);

        GameData gameData = new GameData();

        gameData.saveFileInfo.fileName = fileName;
        gameData.saveFileInfo.sceneName = SceneManager.GetActiveScene().name;
        gameData.saveFileInfo.dateTime = DateTime.Now;

        // string[] resources = new string[SaveableObjectsExcludePlayer.Count];
        string[] guids = new string[SaveableObjectsExcludePlayer.Count];

        gameData.sceneData.sceneName = SceneManager.GetActiveScene().name;

        // Save all non-player saveables
        for (int i = 0; i < SaveableObjectsExcludePlayer.Count; i++) {
            Debug.Log("Adding " + SaveableObjectsExcludePlayer[i].gameObject.name + " to resources. ID: " + i.ToString());
            // resources[i] = SaveableObjectsExcludePlayer[i].gameObject.name.Replace("(Clone)", "").Trim(); // Trim "(Clone)" off the end of object name
            guids[i] = SaveableObjectsExcludePlayer[i].guid.ToString();
            gameData = SaveableObjectsExcludePlayer[i].Save(gameData, i);
        }
        // Save player
        PlayerSaveable.Save(gameData, -1);

        // Save Guids
        gameData.saveableObjectGuids = guids;

        // Save resources
        // gameData.spawnResources = resources;

        bf.Serialize(file, gameData);
        file.Close();
    }

    public void OverwriteSave(string saveFileName) {
        if (!Directory.Exists(datapath)) {
            Directory.CreateDirectory(datapath);
        }

        BinaryFormatter bf = new BinaryFormatter();
        string fileName = saveFileName;
        FileStream file = File.Create(datapath + fileName);

        GameData gameData = new GameData();

        gameData.saveFileInfo.fileName = fileName;
        gameData.saveFileInfo.sceneName = SceneManager.GetActiveScene().name;
        gameData.saveFileInfo.dateTime = DateTime.Now;

        string[] resources = new string[SaveableObjectsExcludePlayer.Count];

        gameData.sceneData.sceneName = SceneManager.GetActiveScene().name;

        // Save all non-player saveables
        for (int i = 0; i < SaveableObjectsExcludePlayer.Count; i++) {
            Debug.Log("Adding " + SaveableObjectsExcludePlayer[i].gameObject.name + " to resources. ID: " + i.ToString());
            resources[i] = SaveableObjectsExcludePlayer[i].gameObject.name.Replace("(Clone)", "").Trim(); // Trim "(Clone)" off the end of object name
            gameData = SaveableObjectsExcludePlayer[i].Save(gameData, i);
        }
        // Save player
        PlayerSaveable.Save(gameData, -1);

        // Save resources
        // gameData.spawnResources = resources;

        bf.Serialize(file, gameData);
        file.Close();
    }

    public void Load(string filename) {
        if(File.Exists(datapath + filename)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(datapath + filename, FileMode.Open);
            Debug.Log(File.GetLastWriteTime(datapath + filename));
            GameData gameData = (GameData)bf.Deserialize(file);
            file.Close();

            pGameData = gameData;

            /*
            if (!sceneLoadedManually) {
                sceneLoadedManually = true;
            }

            // Clear saveable object list
            saveableObjects.Clear();
            */

            // Load Scene
            LoadScene(gameData.sceneData.sceneName); // OnSceneLoaded function runs on scene load
        }
    }

    public void Load(GameData gameData) {
        pGameData = gameData;

        /*
        if (!sceneLoadedManually) {
            sceneLoadedManually = true;
        }

        // Clear saveable object list
        saveableObjects.Clear();
        */

        // Load Scene
        LoadScene(gameData.sceneData.sceneName); // OnSceneLoaded function runs on scene load
    }

    public GameData[] SaveFilesGameData() {
        GameData[] gameData;

        BinaryFormatter bf = new BinaryFormatter();
        string[] files = Directory.GetFiles(datapath);
        gameData = new GameData[files.Length];

        for (int i = 0; i < files.Length; i ++) {
            FileStream file = File.Open(files[i], FileMode.Open);
            GameData d = (GameData)bf.Deserialize(file);
            gameData[i] = d;
            file.Close();
        }

        return gameData;
    }

    // LOAD ALL SCENES USING THE SAVELOADMANAGER
    public void LoadScene(string scene) {
        saveableObjects.Clear();
        sceneLoadMethod = SceneLoadMethod.ManualLoad;
        SceneManager.LoadScene(scene);
    }

    public string UniqueID() {
        return Guid.NewGuid().ToString();
    }
}

[Serializable]
public class GameData {
    public SaveFileInfo saveFileInfo = new SaveFileInfo();
    public string[] saveableObjectGuids;
    // public string[] spawnResources;
    public PlayerData playerData = new PlayerData();
    public SceneData sceneData = new SceneData();
    public List<AICharacterData> aICharacterData = new List<AICharacterData>();
    public List<ProjectileData> projectileData = new List<ProjectileData>();
    
    public void SetPlayerData(PlayerData data) {
        playerData = data;
    }
}

[Serializable]
public class SceneData {
    public string sceneName;
}

[Serializable]
public class SaveFileInfo {
    public string fileName;
    public string sceneName;
    public DateTime dateTime;
}

[Serializable]
public class SaveableObjectBaseData {
    public string guid;
}

[Serializable]
public class PlayerData : SaveableObjectBaseData {
    public int health;
    public int armor;
    public int grenades;
    public int weaponIndex;
    public bool crouched;

    public int[] clips;
    public int[] ammo;
    public bool[] weaponLocks;
    public int[] collectedKeys;

    public Vector3Surrogate position;
    public Vector3Surrogate rotation;
    public Vector3Surrogate camRotation;
    public Vector3Surrogate velocity;
}

[Serializable]
public class AICharacterData : SaveableObjectBaseData {
    public int id;
    public Vector3Surrogate position;
    public Vector3Surrogate rotation;
    public int health;
}

[Serializable]
public class ProjectileData : SaveableObjectBaseData {
    public int id;
    public Vector3Surrogate position;
    public Vector3Surrogate rotation;
}

[Serializable]
public class Vector3Surrogate {
    public float x, y, z;

    public Vector3 ConvertedToVector3 {
        get {
            return new Vector3(x, y, z);
        }
    }

    public static Vector3Surrogate ConvertFromVector3(Vector3 vector3) {
        Vector3Surrogate surrogate = new Vector3Surrogate();

        surrogate.x = vector3.x;
        surrogate.y = vector3.y;
        surrogate.z = vector3.z;

        return surrogate;
    }
}

[Serializable]
public class QuaternionSurrogate {
    public float x, y, z, w;

    public Quaternion ConvertedToQuaternion {
        get {
            return new Quaternion(x, y, z, w);
        }
    }

    public static QuaternionSurrogate ConvertFromQuaternion(Quaternion quaternion) {
        QuaternionSurrogate surrogate = new QuaternionSurrogate();

        surrogate.x = quaternion.x;
        surrogate.y = quaternion.y;
        surrogate.z = quaternion.z;
        surrogate.w = quaternion.w;

        return surrogate;
    }
}

public enum SceneLoadMethod {
    Default,
    ManualLoad,
    LevelTransition
}
