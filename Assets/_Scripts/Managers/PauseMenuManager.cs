using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; set; }

    [HideInInspector] public HudCanvas hudCanvas;
    [HideInInspector] public bool paused;

    public void Startup() {
        Debug.Log("Pause Menu Manager starting...");

        hudCanvas = GameObject.FindGameObjectWithTag("PlayerHud").GetComponent<HudCanvas>();

        status = ManagerStatus.Started;
    }

    public void PauseSwitch() {
        if (paused) {
            UnpauseGame();
        } else {
            PauseGame();
        }
    }

    public void PauseGame() {
        Time.timeScale = 0;
        hudCanvas.hudPanel.SetActive(false);
        hudCanvas.pauseMenuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        paused = true;
    }

    public void UnpauseGame() {
        Time.timeScale = 1;
        hudCanvas.hudPanel.SetActive(true);
        hudCanvas.pauseMenuPanel.SetActive(false);
        hudCanvas.pauseMenuPanel.GetComponent<PauseMenu>().savePanel.SetActive(false);
        hudCanvas.pauseMenuPanel.GetComponent<PauseMenu>().loadPanel.SetActive(false);
        // TODO: Close options panel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        paused = false;
    }
}
