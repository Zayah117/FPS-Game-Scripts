using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    public GameObject mainPanel;
    public GameObject savePanel;
    public GameObject loadPanel;
    public GameObject optionsPanel;

    public GameObject saveContentBox;
    public GameObject loadContentBox;
    public GameObject saveButtonPrefab;
    public GameObject loadButtonPrefab;

    public void ResumeGame() {
        Managers.PauseMenu.UnpauseGame();
    }

    public void LoadScene(string scene) {
        Managers.SaveLoad.LoadScene(scene);
    }

    public void OpenSaveMenu() {
        foreach (Transform child in saveContentBox.transform) {
            Destroy(child.gameObject);
        }

        mainPanel.SetActive(false);
        savePanel.SetActive(true);

        GameData[] gameData = Managers.SaveLoad.SaveFilesGameData();
        Array.Sort(gameData, (x, y) => DateTime.Compare(y.saveFileInfo.dateTime, x.saveFileInfo.dateTime));

        foreach (GameData d in gameData) {
            GameObject button = Instantiate(saveButtonPrefab, saveContentBox.transform);
            button.GetComponent<SaveLoadButton>().gameData = d;
            Text text = button.transform.GetChild(0).GetComponent<Text>();
            text.text = d.saveFileInfo.sceneName + " - " + d.saveFileInfo.dateTime;
        }
    }

    public void OpenLoadMenu() {
        foreach (Transform child in loadContentBox.transform) {
            Destroy(child.gameObject);
        }

        mainPanel.SetActive(false);
        loadPanel.SetActive(true);

        GameData[] gameData = Managers.SaveLoad.SaveFilesGameData();
        Array.Sort(gameData, (x, y) => DateTime.Compare(y.saveFileInfo.dateTime, x.saveFileInfo.dateTime));

        foreach (GameData d in gameData) {
            GameObject button = Instantiate(loadButtonPrefab, loadContentBox.transform);
            button.GetComponent<SaveLoadButton>().gameData = d;
            Text text = button.transform.GetChild(0).GetComponent<Text>();
            text.text = d.saveFileInfo.sceneName + " - " + d.saveFileInfo.dateTime;
        }
    }

    public void SaveGame() {
        Managers.SaveLoad.NewSave();
        ResumeGame();
    }

    // Not sure if needed
    public void LoadGame(GameData gameData) {
        Managers.SaveLoad.Load(gameData);
    }

    public void OpenOptionsMenu() {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void BackToMainPanel() {
        savePanel.SetActive(false);
        loadPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void Quit() {
        Application.Quit();
    }
}
