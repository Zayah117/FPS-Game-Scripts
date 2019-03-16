using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(GameplayManager))]
[RequireComponent(typeof(PauseMenuManager))]
[RequireComponent(typeof(SaveLoadManager))]
[RequireComponent(typeof(ObjectPoolerManager))]
public class Managers : MonoBehaviour {
    public static AudioManager Audio { get; private set; }
    public static GameplayManager Gameplay { get; private set; }
    public static PauseMenuManager PauseMenu { get; private set; }
    public static SaveLoadManager SaveLoad { get; private set; }
    public static ObjectPoolerManager Pooler { get; private set; }

    public static LayerMaskManager LayerMasks;

    List<IGameManager> _startSequence;

    static Managers instance;

    public static bool InMainMenu {
        get { return SceneManager.GetActiveScene().name == "Menu"; }
    }

    void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != null) {
            Destroy(gameObject);
        }

        if (Audio == null) {
            Audio = GetComponent<AudioManager>();
        }
        if (Gameplay == null) {
            Gameplay = GetComponent<GameplayManager>();
        }
        if (PauseMenu == null) {
            PauseMenu = GetComponent<PauseMenuManager>();
        }
        if (SaveLoad == null) {
            SaveLoad = GetComponent<SaveLoadManager>();
        }
        if (Pooler == null) {
            Pooler = GetComponent<ObjectPoolerManager>();
        }
        if (LayerMasks == null) {
            LayerMasks = GetComponent<LayerMaskManager>();
        }

        _startSequence = new List<IGameManager>();
        if (!InMainMenu) {
            _startSequence.Add(Gameplay);
            _startSequence.Add(PauseMenu);
            _startSequence.Add(Pooler);
        }
        _startSequence.Add(Audio);
        _startSequence.Add(SaveLoad);

        StartCoroutine(StartupManagers());
    }

    IEnumerator StartupManagers() {
        foreach (IGameManager manager in _startSequence) {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules) {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence) {
                if (manager.status == ManagerStatus.Started) {
                    numReady++;
                }
            }

            if (numReady > lastReady) {
                Debug.Log("Manager startup progress: " + numReady + "/" + numModules);
                // Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
            }

            yield return null;
        }

        Debug.Log("All managers started up");
        // Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
    }
}
