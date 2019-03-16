using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager {
    /// <summary>
    /// UNUSED CLASS, PLAYER INFO HANDLED IN GAMEPLAY MANAGER
    /// </summary>
    public ManagerStatus status { get; private set; }

    public static PlayerManager instance;

    public void Startup() {
        Debug.Log("Player Manager starting...");

        if (instance == null) {
            instance = this;
        }

        status = ManagerStatus.Started;
    }

    public GameObject player;
}
