using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(WeaponSwitchController))]
public class PlayerController : MonoBehaviour {
    [SerializeField] float _mouseSensitivity;

    Player player;
    WeaponSwitchController switchController;

    bool dryLock;

    private void Awake() {
        // Put this in awake in order to fix null reference bug
        switchController = GetComponent<WeaponSwitchController>();
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player = GetComponent<Player>();
    }
	
	void Update () {
        if (!Managers.PauseMenu.paused) {
            PlayerInputs();
        }

        // Pause Game
        if (Input.GetKeyDown(KeyCode.P)) {
            Managers.PauseMenu.PauseSwitch();
        }

        // Exit Game
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Managers.PauseMenu.PauseSwitch();
            // Application.Quit();
        }

	}

    // For use in update
    void PlayerInputs() {
        // Horizontal Rotation
        player.Rotate(new Vector3(0, GetMouseInput()[0], 0));

        // Vertical Rotation
        player.RotateCamera(new Vector3(-GetMouseInput()[1], 0, 0));

        // Camera Roll
        player.UpdateCameraRoll();

        // Reload
        // Pretty long if statement if I do say so myself (check if there's ammo left and if clip is full)
        // Reload is above Fire in case Fire needs to set reload trigger
        if (Input.GetKeyDown(KeyCode.R) && player.HasWeapon() && player.switchController.inventoryWeapons[switchController.currentWeaponIndex].ammoPool > 0 && player.ammoClips[switchController.currentWeaponIndex] < switchController.inventoryWeapons[switchController.currentWeaponIndex].weaponPrefab.clipsize) {
            player.SetReloadTrigger(true);
        // } else {
        } else if (Input.GetKeyUp(KeyCode.R)) {
            player.SetReloadTrigger(false);
        }

        // Melee
        if (Input.GetKey(KeyCode.F) || Input.GetMouseButton(3) || player.OutOfAmmo()) {
            player.SetMeleeTrigger(true);
        } else {
            player.SetMeleeTrigger(false);
        }

        // Fire
        if (Input.GetMouseButton(0) && player.HasWeapon()) {
            // Check for ammo
            if (player.HasAmmo(switchController.currentWeaponIndex)) {
                player.SetPlayerTarget();
                player.SetFireTrigger(true);
            // If there's no ammo in the clip
            } else {
                if (!player.FireAnimationPlaying() && !player.ReloadAnimationPlaying() && !player.GetReloadTrigger() && !switchController.AttackSetMelee()) {
                    switchController.DryFireSound();
                    // If we have extra ammo
                    if (player.HasAmmoPool(switchController.currentWeaponIndex)) {
                        player.SetReloadTrigger(true);
                    // Otherwise go to next weapon
                    } else if (!player.OutOfAmmo()) {
                        switchController.NextWeapon(switchController.currentWeaponIndex);
                    }
                }
                player.SetFireTrigger(false);
            }
        } else {
            player.SetFireTrigger(false);
        }

        // Grenades
        if (Input.GetKey(KeyCode.G)) {
            player.SetPlayerTarget();
            player.SetGrenadeTrigger(true);
        } else {
            player.SetGrenadeTrigger(false);
        }

        // Zoom
        if (Input.GetMouseButton(1)) {
            player.SetZoomTrigger(true);
        } else {
            player.SetZoomTrigger(false);
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            player.crouched = !player.crouched;
        }

        // Jump
        if (Input.GetButtonDown("Jump")) {
            player.SetJumpTrigger(true);
            player.UnlockJumpQueue();
        } else {
            player.SetJumpTrigger(false);
        }

        // Jump Queue
        if (Input.GetButton("Jump")) {
            player.SetQueueJumpTrigger(true);
        } else {
            player.SetQueueJumpTrigger(false);
        }

        // WeaponSwitch
        if (Input.GetKeyDown(KeyCode.T) || Input.GetAxis("Mouse ScrollWheel") < 0f) {
            switchController.NextWeapon(switchController.currentWeaponIndex);
        } else if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            switchController.NextWeapon(switchController.currentWeaponIndex, true);
        }

        // Interactable Objects
        if (Input.GetKeyDown(KeyCode.E)) {
            player.Interact();
        }

        // Saving and Loading
        if (Input.GetKeyDown(KeyCode.F5)) {
            Managers.SaveLoad.NewSave();
        } else if (Input.GetKeyDown(KeyCode.F9)) {
            Debug.LogWarning("Quick Load not implemented.");
            // SaveLoadManager.instance.Load();
        }
    }

    public Vector3 GetMoveInput() {
        float xMovement = Input.GetAxisRaw("Horizontal");
        float zMovement = Input.GetAxisRaw("Vertical");

        // Multiply inputs by local transform and return that value
        return player.LocalTransformMovement(xMovement, zMovement);
    }

    public Vector3 GetMoveInputRaw() {
        float xMovement = Input.GetAxisRaw("Horizontal");
        float zMovement = Input.GetAxisRaw("Vertical");

        return new Vector3(xMovement, 0, zMovement);
    }

    float[] GetMouseInput() {
        float horizontalRotation = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float verticalRotation = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        return new float[] { horizontalRotation, verticalRotation };
    }
}
