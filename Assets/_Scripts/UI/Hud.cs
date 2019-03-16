using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {
    Player player;
    [SerializeField] Text shieldText;
    [SerializeField] Text healthText;
    [SerializeField] Text armorText;
    [SerializeField] Text ammoText;
    [SerializeField] Text comboText;
    [SerializeField] Text grenadeText;
    [SerializeField] Slider progressBar;
    [SerializeField] Slider comboBar;
    [SerializeField] Text interactText;

    void Start() {
        player = Managers.Gameplay.player;
    }

    void Update() {
        shieldText.text = "SHIELD: " + player.shield.ToString();
        healthText.text = "HEALTH: " + player.health.ToString();
        armorText.text = "ARMOR: " + player.armor.ToString();
        ammoText.text = "AMMO: " + player.ammoClips[player.switchController.currentWeaponIndex].ToString() + "/" + player.switchController.inventoryWeapons[player.switchController.currentWeaponIndex].ammoPool.ToString();
        grenadeText.text = "GRENADES: " + player.grenadeCount.ToString();
        comboText.text = Managers.Gameplay.combo.ToString();
        comboBar.value = Managers.Gameplay.comboTimePercent;
        progressBar.value = Managers.Gameplay.powerLevelY;
        interactText.enabled = Managers.Gameplay.player.CheckInteractable();
    }
}
