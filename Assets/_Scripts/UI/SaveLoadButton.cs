using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadButton : MonoBehaviour {
    [HideInInspector] public GameData gameData = new GameData();

    public void OverwriteSave() {
        Managers.SaveLoad.OverwriteSave(gameData.saveFileInfo.fileName);
        Managers.PauseMenu.UnpauseGame();
    }

    public void LoadGame() {
        Managers.SaveLoad.Load(gameData);
    }
}
