using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject loadGamePanel;
    public GameObject settingsPanel;

    public GameObject loadContentBox;
    public GameObject loadButtonPrefab;

    public void PlayScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public void LevelSelect() {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void LoadGame() {
        foreach (Transform child in loadContentBox.transform) {
            Destroy(child.gameObject);
        }

        mainMenuPanel.SetActive(false);
        loadGamePanel.SetActive(true);

        GameData[] gameData = Managers.SaveLoad.SaveFilesGameData();
        Array.Sort(gameData, (x, y) => DateTime.Compare(y.saveFileInfo.dateTime, x.saveFileInfo.dateTime));

        foreach (GameData d in gameData) {
            GameObject button = Instantiate(loadButtonPrefab, loadContentBox.transform);
            button.GetComponent<SaveLoadButton>().gameData = d;
            Text text = button.transform.GetChild(0).GetComponent<Text>();
            text.text = d.saveFileInfo.sceneName + " - " + d.saveFileInfo.dateTime;
        }
    }

    public void Settings() {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMainMenu(GameObject panel) {
        panel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void Quit() {
        Application.Quit();
    }
}
