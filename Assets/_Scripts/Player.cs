using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerController))]
public class Player : Character {
    [SerializeField] Camera cam;
    [SerializeField] Camera fpvCam;

    float currentCamBobAmount;
    float camBobAmount = 0.03f;
    public float camBobAnimation;
    Animator mainPlayerAnim;

    [HideInInspector] public PlayerController playerController;
    CharacterController characterController;
    public WeaponSwitchController switchController; // ONLY SETTING THIS PUBLIC TEMPORARILY FOR THE HUD

    [SerializeField] float jumpSpeed;
    Vector3 _rotation;
    float gravity = -9.81f;
    float gravityScale = 5f;
    float terminalVelocity = -100.0f;
    float minFall = -1.5f;
    float verticalSpeed;
    float minLookVert = -80; // originally set to -90
    float maxLookVert = 80; // originally set to 90
    float cameraCrouchPosition = 0.528f; // 0.6 * original position
    float cameraOriginalPosition = 0.88f; // 0.75f;
    float cameraXRotation;
    float cameraZRotation;
    float effectiveSpeed;
    Vector3 velocity;
    Vector3 bonusVelocity;
    float acceleration;
    bool moveLock;
    Vector3 tempVelocity;
    bool bonusVelocityQueued;
    Vector3 snapRotation;
    bool snapRotationQueued;
    Vector3 snapVelocity;
    bool snapVelocityQueued;
    bool sliding;
    bool touchingSomething;

    Vector3 horizontalVelocity {
        get {
            return new Vector3(velocity.x, 0, velocity.z);
        }
    }

    public Vector3 Velocity {
        get {
            return velocity;
        }
    }

    bool reloadTrigger = false;
    bool meleeTrigger = false;
    bool grenadeTrigger = false;
    bool zoomTrigger = false;
    bool jumpTrigger = false;
    bool queueJumpTrigger = false;
    bool ammoPickupReloadTrigger = false;
    bool jumpQueueLock = true;
    float playerPowerLevel;
    int playerCombo;

    bool shieldDrainActive;
    Coroutine shieldDrainCoroutine;

    [HideInInspector] public bool crouched = false;
    [HideInInspector] public bool hitGround = false;

    [SerializeField] Weapon emptyWeaponPrefab; 
    public int grenadeCount;
    public int maxGrenades;
    [SerializeField] GameObject grenadeMuzzle;
    [HideInInspector] public int[] ammoClips;
    Weapon meleeWeapon;
    Weapon grenadeLauncher;

    public int armor; // Public for hud
    [SerializeField] int maxArmor;

    public int shield; // Public for hud
    [SerializeField] int maxShield;

    List<int> collectedKeys;

    private ControllerColliderHit contact;
    private ControllerColliderHit currentContact;

    InventoryWeapon[] weapons;

    IInteractable currentInteractable;

    PlayerData loadedData;

    // FUNCTIONS
    public override void Start() {
        // base.Start();
        // Make sure this start method is similar to the one used in Character.cs

        // Subscribe to manager every scene load because previous Player is cleared.
        // SubscribeManager();

        // If scene was loaded manually, load information from gameData
        /*
        if (SaveLoadManager.instance.sceneLoadedManually) {
            SaveLoadManager.instance.LoadSpecific(GetComponent<SaveableObject>());
        }
        */
        // TODO: Save variables like health, ammo, between levels, but set location and rotation to default

        anim = characterMesh.GetComponent<Animator>();
        mainPlayerAnim = this.GetComponent<Animator>();

        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        switchController = GetComponent<WeaponSwitchController>();

        weapons = switchController.inventoryWeapons;

        // For testing purposes only
        // Locking and instantiating weapons
        // weapons[0].locked = false;
        if (HasWeapon()) {
            InstantiateWeapon(weapons[0].weaponPrefab);
        } else {
            InstantiateWeapon(emptyWeaponPrefab);
        }

        // Create weapon clips with full ammo
        ammoClips = new int[weapons.Length];
        for (int i = 0; i < weapons.Length; i++) {
            ammoClips[i] = weapons[i].weaponPrefab.clipsize;
        }

        // Instantiate melee weapon
        meleeWeapon = Instantiate(switchController.meleeWeapon.weaponPrefab);
        meleeWeapon.character = this;

        // Instantiate grenade launcher
        grenadeLauncher = Instantiate(switchController.grenadeLauncher.weaponPrefab);
        grenadeLauncher.character = this;

        effectiveSpeed = speed;

        collectedKeys = new List<int>();

        currentInteractable = null;

        // Apply loaded data
        if (Managers.SaveLoad.sceneLoadMethod == SceneLoadMethod.ManualLoad) {
            ApplyLoadedData();
        }
    }

    public override void Update() {
        // base.Update();

        if (AttackSetNormal()) {
            AttackLoop();
        } else if (AttackSetSwitch()) { // Fixes infinite firing bug when switching and firing at the same time
            SetFireAnimation(false);
        }

        // Rotate capsule (camera is rotated in PlayerController)
        if (snapRotationQueued) {
            SnapRotation();
        }
        transform.rotation = Quaternion.Euler(_rotation);

        if (!Managers.PauseMenu.paused) {
            Movement();
        }

        if (HasWeapon()) {
            // Reload
            ReloadLoop();

            // Make sure active weapon is enabled
            switchController.ActiveWeaponController(meleeTrigger, grenadeTrigger);

            // Zoom
            ZoomLoop();

            // Check if anything in melee box 
            if (AttackSetMelee()) {
                if (CheckMeleeBox(meleeWeaponHitBoxOrigin, 2f) && meleeTrigger) {
                    anim.SetBool("Attack", true);
                } else {
                    anim.SetBool("Attack", false);
                }
            }

            // For managing grenade launcher animations
            if (AttackSetGrenade()) {
                if (grenadeTrigger) {
                    anim.SetBool("Attack", true);
                } else {
                    anim.SetBool("Attack", false);
                }
            }

            // Update Animation Speed
            UpdateAnimSpeed();
        }

        ShieldDrainCheck();
        SetInteractable();
        CheckMovingPlatform();
    }

    void Movement() {
        // Update powelLevel
        /*
        playerPowerLevel = Managers.Gameplay.powerLevelY;
        effectiveSpeed = speed * (1.0f + playerPowerLevel);
        */
        playerPowerLevel = Managers.Gameplay.comboPower;
        effectiveSpeed = speed * (1.0f + playerPowerLevel);

        // Check hit ground
        hitGround = false;
        RaycastHit hit;
        if (verticalSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit, 10.0f, Managers.LayerMasks.PlayerHitGroundLayerMask)) {
            float check = (characterController.height + characterController.radius) / 1.8f; // was 1.9, lowered to account for walking up steps (so the player doesn't slide while climbing stairs)
            hitGround = hit.distance <= check;
        }

        // Cut speed in half if crouched
        if (crouched && hitGround) {
            effectiveSpeed *= 0.5f;
        }

        // Get Movement (set velocity)
        Vector3 moveInput = playerController.GetMoveInput();
        Vector3 normalizedInput = moveInput.normalized;
        Vector3 normalizedInputWithSpeed = normalizedInput * effectiveSpeed;

        // Add acceleration to movement
        if (normalizedInput.magnitude > 0) {
            if (moveLock == false) {
                moveLock = true;
            }
            acceleration = Mathf.MoveTowards(acceleration, 1, 8 * Time.deltaTime);
        } else {
            if (moveLock == true) {
                moveLock = false;
                tempVelocity = velocity;
            }
            acceleration = Mathf.MoveTowards(acceleration, 0, 16 * Time.deltaTime);
        }
        normalizedInputWithSpeed *= acceleration;

        // Determine how much control player has over horizontal movement
        // If player is on ground or is sliding, they will have full control
        // If they are in the air they will have limited control over direction
        if (hitGround || sliding || touchingSomething) {
            if (normalizedInput.magnitude > 0) {
                velocity = normalizedInputWithSpeed;
            } else {
                velocity = acceleration * tempVelocity;
            }
            if (sliding) {
                sliding = false;
            }
        } else {
            float airControl = 200f;
            // If moveInput more than zero AND...
            // Player is not going to be slowed down by moving toward velocity direction at a slow speed...
            // Then have limited directional control
            if (normalizedInput.magnitude > 0 && !(Vector3.Angle(horizontalVelocity, normalizedInputWithSpeed) < 40 && horizontalVelocity.magnitude > effectiveSpeed * 1.1f)) {
                velocity = Vector3.MoveTowards(velocity, normalizedInputWithSpeed, airControl * Time.deltaTime);
            }
        }

        // If snap velocity is supposed to be used this frame,
        // overwrite previous calculations and set velocity to snapVelocity
        if (snapVelocityQueued) {
            SnapVelocity();
        }

        // Check if player has hit his head 
        bool hitHead = false;
        RaycastHit myHit;
        if (Physics.Raycast(transform.position, Vector3.up, out myHit, 10f, Managers.LayerMasks.WorldObjectLayerMask, QueryTriggerInteraction.Ignore)) {
            float check = (characterController.height + characterController.radius) / 1.95f;
            hitHead = myHit.distance <= check;

            // Make sure player doesn't hit his head... on himself (layermask should fix)
            if (myHit.transform.name == this.name) {
                hitHead = false;
            }
        }

        // If I hit my head while on the ground, go to crouch
        if (hitHead && hitGround) {
            crouched = true;
        }

        // Crouch - will probably need some refactoring
        if (crouched && hitGround) {
            characterController.height = Mathf.Lerp(characterController.height, 1.2f, 5 * Time.deltaTime);
        } else {
            float lastHeight = characterController.height;
            characterController.height = Mathf.Lerp(characterController.height, 2.0f, 5 * Time.deltaTime);

            Vector3 position = transform.position;
            position.y += (characterController.height - lastHeight) / 2;
            transform.position = position;
        }

        // Update camera position
        SetCameraPosition();

        // y Velocity
        if (hitGround) {
            if (queueJumpTrigger && !jumpQueueLock) {
                verticalSpeed = jumpSpeed;
                Managers.Audio.PlayJumpSound(1);
                jumpQueueLock = true;
            } else {
                verticalSpeed = minFall;
            }

            // Original jump code
            /*
            if (jumpTrigger) {
                verticalSpeed = jumpSpeed;
            } else {
                verticalSpeed = minFall;
            }
            */
        } else {
            // If I hit my head while in the air, set vertical speed to -vertical speed (kinda bouncy)
            if (hitHead) {
                verticalSpeed = -verticalSpeed * 0.5f;
            }
            verticalSpeed += gravity * gravityScale * Time.deltaTime;
            if (verticalSpeed < terminalVelocity) {
                verticalSpeed = terminalVelocity;
            }

            // Raycasting didn't detect ground but capsule is colliding (for sliding)
            if (characterController.isGrounded) {
                sliding = true;
                if (Vector3.Dot(velocity, contact.normal) < 0) {
                    velocity = contact.normal * speed;
                } else {
                    velocity += contact.normal * speed;
                }
            }
        }

        // Set y velocity
        velocity.y = verticalSpeed;

        // Bonus velocity 
        // velocity += GetBonusVelocity();
        if (bonusVelocityQueued) {
            velocity = GetBonusVelocity();
        }
        verticalSpeed = velocity.y;

        // Reset touching something variable (gets set again on characterController.Move())
        touchingSomething = false;

        // Final move
        characterController.Move((velocity) * Time.deltaTime);

        // Animate weapon sway
        if (HasWeapon() && (AttackSetNormal() || AttackSetMelee())) {
            if (hitGround && horizontalVelocity.magnitude > 0.1) {
                anim.SetBool("IsMoving", true);
            } else {
                anim.SetBool("IsMoving", false);
            }
        }

        // Head bobbing
        if (hitGround && horizontalVelocity.magnitude > 0.1) {
            UpdateCameraBob(true);
            SyncWeaponAnimationTime();
        } else {
            UpdateCameraBob(false);
        }
    }

    public void SyncWeaponAnimationTime() {
        float normalizedTime = mainPlayerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float normalizedTimeInCurrentLoop = normalizedTime - Mathf.Floor(normalizedTime);
        anim.SetFloat("AnimationTime", normalizedTimeInCurrentLoop);
    }

    void ReloadLoop() {
        if (AttackSetNormal()) {
            if (reloadTrigger || ammoPickupReloadTrigger) {
                anim.SetBool("IsReloading", true);
            } else {
                anim.SetBool("IsReloading", false);
            }
        }
    }

    public bool ReloadAnimationPlaying() {
        AnimatorStateInfo stateInfo = characterMesh.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Reload")) {
            return true;
        } else {
            return false;
        }
    }

    void ZoomLoop() {
        if (zoomTrigger) {
            cam.fieldOfView = 45;
            fpvCam.fieldOfView = 25;
        } else {
            cam.fieldOfView = 90;
            fpvCam.fieldOfView = 50;
        }
    }

    void Move(Vector3 movement) {
        characterController.Move(movement);
    }

    public void Rotate(Vector3 rotation) {
        _rotation += rotation;
    }

    void SnapRotation() {
        _rotation = snapRotation;
        snapRotationQueued = false;
    }

    public void QueueSnapRotation(Vector3 rotation) {
        snapRotation = new Vector3(0, rotation.y, 0);
        snapRotationQueued = true;
    }

    public void SnapVelocity() {
        velocity = snapVelocity;
        snapVelocityQueued = false;
    }

    public void QueueSnapVelocity(Vector3 desiredDirection) {
        snapVelocity = desiredDirection.normalized * velocity.magnitude;
        snapVelocityQueued = true;
    }

    public void RotateCamera(Vector3 degree) {
        cameraXRotation += degree.x;
        cameraXRotation = Mathf.Clamp(cameraXRotation, minLookVert, maxLookVert);
        cam.transform.localRotation = Quaternion.Euler(cameraXRotation, 0, 0);
    }

    public override void FireWeapon() {
        base.FireWeapon();
        UpdateAmmo(-1, switchController.currentWeaponIndex);
    }

    public void Melee() {
        meleeWeapon.Fire();
    }

    // Duplicate code exists here and in Weapon.cs (FireHitBox). The same magic numbers are used here and in Sword.cs.
    bool CheckMeleeBox(Transform origin, float boxSize) {
        // YOU CAN MAKE THIS FUNCTION MORE EFFICIENT MY USING A LAYER MASK INSTEAD OF -1
        Collider[] hitColliders = Physics.OverlapBox(origin.position, new Vector3(boxSize / 2, boxSize / 2, boxSize / 2), Quaternion.identity, -1);
        foreach (Collider hit in hitColliders) {
            Vector3 targetDirection = (hit.transform.position - transform.position);
            if (Vector3.Dot(targetDirection, transform.forward) < 0) {
                continue;
            }

            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null) {
                Character c = hit.GetComponent<Character>();
                if (c != this) {
                    return true;
                }
            }
        }
        return false;
    }

    public void Grenade() {
        if (grenadeCount > 0) {
            grenadeLauncher.Fire();
            WeaponFlash();
            MuzzleLight();
            grenadeCount -= 1;
        }
    }

    public void ToggleMeleeAnimation() {
        anim.SetBool("Toggle", !anim.GetBool("Toggle"));
    }

    public void SetReloadTrigger(bool b) {
        reloadTrigger = b;
    }

    public bool GetReloadTrigger() {
        return reloadTrigger;
    }

    public void SetMeleeTrigger(bool b) {
        meleeTrigger = b;
    }

    public void SetGrenadeTrigger(bool b) {
        grenadeTrigger = b;
    }

    public void SetZoomTrigger(bool b) {
        zoomTrigger = b;
    }

    public void SetJumpTrigger(bool b) {
        jumpTrigger = b;
    }

    public void SetQueueJumpTrigger(bool b) {
        queueJumpTrigger = b;
    }

    public override void AttackLoop() {
        if (HasWeapon()) {
            base.AttackLoop();
        }
    }

    public void Reload() {
        int currentWeaponIndex = switchController.currentWeaponIndex;
        PlayerWeapon currentSelectedWeapon = switchController.inventoryWeapons[currentWeaponIndex].weaponPrefab;

        int ammoLeft = ammoClips[currentWeaponIndex];
        int ammoSpace = currentSelectedWeapon.clipsize - ammoLeft;

        if (weapons[currentWeaponIndex].ammoPool < ammoSpace) {
            ammoClips[currentWeaponIndex] += weapons[currentWeaponIndex].ammoPool;
            weapons[currentWeaponIndex].ammoPool = 0;
        } else {
            ammoClips[currentWeaponIndex] = currentSelectedWeapon.clipsize;
            weapons[currentWeaponIndex].ammoPool -= ammoSpace;
        }
        ammoPickupReloadTrigger = false;
    }

    public void SetPlayerTarget() {
        // Raycast for projectile target location
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1000.0f, Managers.LayerMasks.PlayerRaycastLayerMask)) {
            targetLocation = hit.point;
        // If the player is not pointing at anything
        } else {
            targetLocation = cam.transform.position + cam.transform.forward * 1000.0f;
        }
    }

    public void SetInteractable() {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, Managers.LayerMasks.InteractableLayerMask)) {
            currentInteractable = hit.transform.gameObject.GetComponent<IInteractable>();
        } else {
            currentInteractable = null;
        }
    }

    public bool CheckInteractable() {
        if (currentInteractable == null) {
            return false;
        } else {
            return true;
        }
    }

    public void Interact() {
        if (CheckInteractable()) {
            currentInteractable.Interact();
        }
    }

    void CheckMovingPlatform() {
        if (currentContact != null && currentContact.gameObject.tag == "MovingPlatform") {
            transform.parent = currentContact.transform;
        } else {
            transform.parent = null;
        }

        // Reset currentContact
        currentContact = null;
    }

    public bool HasWeapon() {
        if (weapons.Length > 0) {
            for (int i = 0; i < weapons.Length; i++) {
                if (weapons[i].locked == false) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool HasWeapon(int index) {
        if (weapons.Length > 0) {
            if (weapons[index].locked == false) {
                return true;
            }
        }
        return false;
    }

    public bool HasAmmo(int index) {
        if (ammoClips[index] > 0) {
            return true;
        } else {
            return false;
        }
    }

    public bool HasAmmoPool(int index) {
        if (weapons[index].ammoPool > 0) {
            return true;
        } else {
            return false;
        }
    }

    public bool OutOfAmmo() {
        for (int i = 0; i < weapons.Length; i ++) {
            if ((weapons[i].ammoPool > 0 || ammoClips[i] > 0) && !weapons[i].locked) {
                return false;
            }
        }
        return true;
    }

    public void AddKey(int index) {
        if (!collectedKeys.Contains(index)) {
            collectedKeys.Add(index);
        }
    }

    public bool HasKey(int index) {
        if (collectedKeys.Contains(index)) {
            return true;
        } else {
            return false;
        }
    }

    public void PickupWeapon(int index) {
        // If I don't have weapon, pick it up
        if (!HasWeapon(index)) {
            if (characterMesh.activeSelf == false) {
                characterMesh.SetActive(true);
            }
            switchController.inventoryWeapons[index].locked = false;
            switchController.SwitchCurrentWeapon(index);
        // Otherwise increase ammo
        } else {
            UpdateAmmoPool(weapons[index].weaponPrefab.clipsize, index);
        }
    }

    public void UpdateAmmo(int value, int index) {
        ammoClips[index] += value;
    }

    public void UpdateAmmoPool(int value, int index) {
        // If I was completely out of ammo, switch to that weapon.
        if (OutOfAmmo()) {
            anim.SetBool("Attack", false); // Fix infinite attack with sword bug
            switchController.ForceSwitchCurrentWeapon(index);
            ammoPickupReloadTrigger = true;
        }
        weapons[index].ammoPool += value;
    }

    public void UpdateWeaponInstance(Weapon weaponPrefab) {
        // This will probably need refactoring later.
        // I won't want to destroy weapon instances.
        // But I've used this for so long and it has worked well so...
        Destroy(equippedWeapon.gameObject);
        InstantiateWeapon(weaponPrefab);
    }

    public Vector3 LocalTransformMovement(float xMovement, float zMovement) {
        Vector3 horizontalMovement = transform.right * xMovement;
        Vector3 verticalMovement = transform.forward * zMovement;

        return horizontalMovement + verticalMovement;
    }

    public Vector3 LocalTransformMovement(Vector3 movementVector) {
        Vector3 horizontalMovement = transform.right * movementVector.x;
        Vector3 verticalMovement = transform.forward * movementVector.z;

        return horizontalMovement + verticalMovement;
    }

    void SetCameraPosition(){
        if (crouched) {
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, Mathf.Lerp(cam.transform.localPosition.y, cameraCrouchPosition, 5 * Time.deltaTime), cam.transform.localPosition.z);
        } else {
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, Mathf.Lerp(cam.transform.localPosition.y, cameraOriginalPosition, 5 * Time.deltaTime), cam.transform.localPosition.z);
        }
        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y + currentCamBobAmount, cam.transform.localPosition.z);
    }

    void UpdateCameraBob(bool b) {
        if (b) {
            mainPlayerAnim.SetBool("AnimationEnabled", true);
            currentCamBobAmount = camBobAnimation * camBobAmount;
        } else {
            mainPlayerAnim.SetBool("AnimationEnabled", false);
            currentCamBobAmount = Mathf.Lerp(currentCamBobAmount, 0, 20 * Time.deltaTime);
        }
    }

    public void UpdateCameraRoll() {
        float rollSpeed = 8;
        float maxRotation = 1;
        float xMovement = playerController.GetMoveInputRaw().x;
        float zMovement = playerController.GetMoveInputRaw().z;

        if (xMovement == -1 && zMovement != 0) {
            cameraZRotation = Mathf.MoveTowards(cameraZRotation, maxRotation * 0.5f, rollSpeed * Time.deltaTime);
        } else if (xMovement == 1 && zMovement != 0) {
            cameraZRotation = Mathf.MoveTowards(cameraZRotation, -maxRotation * 0.5f, rollSpeed * Time.deltaTime);
        } else if (xMovement == -1) {
            cameraZRotation = Mathf.MoveTowards(cameraZRotation, maxRotation, rollSpeed * Time.deltaTime);
        } else if (xMovement == 1) {
            cameraZRotation = Mathf.MoveTowards(cameraZRotation, -maxRotation, rollSpeed * Time.deltaTime);
        } else {
            cameraZRotation = Mathf.MoveTowards(cameraZRotation, 0, rollSpeed * Time.deltaTime);
        }

        cam.transform.localRotation = Quaternion.Euler(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, cameraZRotation);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        contact = hit;
        currentContact = hit;
        touchingSomething = true;
    }

    public override void Heal(int value) {
        health += value;
        if (health > maxHealth) {
            health = maxHealth;
        }
    }

    public void AddShield(int value) {
        shield += value;
        if (AtMaxShield()) {
            shield = maxShield;
        }
    }

    public bool AtMaxShield() {
        if (shield >= maxShield) {
            return true;
        } else {
            return false;
        }
    }

    public void AddArmor(int value) {
        armor += value;
        if (AtMaxArmor()) {
            armor = maxArmor;
        }
    }

    public bool AtMaxArmor() {
        if (armor >= maxArmor) {
            return true;
        } else {
            return false;
        }
    }

    public void AddGrenades(int value) {
        grenadeCount += value;
        if (AtMaxGrenades()) {
            grenadeCount = maxGrenades;
        }
    }

    public bool AtMaxGrenades() {
        if (grenadeCount >= maxGrenades) {
            return true;
        } else {
            return false;
        }
    }

    public bool AtMaxHealth() {
        if (health >= maxHealth) {
            return true;
        } else {
            return false;
        }
    }

    void ShieldDrainCheck() {
        if (shield > 0 && !shieldDrainActive) {
            shieldDrainCoroutine = StartCoroutine(DrainShield());
            shieldDrainActive = true;
        } else if (shield <= 0 && shieldDrainActive) {
            StopCoroutine(shieldDrainCoroutine);
            shieldDrainActive = false;
        }
    }

    IEnumerator DrainShield() {
        while (true) {
            yield return new WaitForSeconds(1f);
            shield -= 1;
            if (shield < 0) {
                Debug.LogWarning("Player shield variable less than 0.");
                shield = 0;
            }
        }
    }

    public void TakeDamageRaw(int damage) {
        health -= damage;
        if (health <= 0) {
            Die(null);
        } else {
            Managers.Audio.PlayHitSound(1);
        }
    }

    public override void TakeDamage(int damage, Character offendingCharacter) {
        if (shield < damage) {
            damage -= shield;
            shield = 0;

            float armorCalc = (float)damage * 0.66666f;
            float healthCalc = (float)damage - armorCalc;

            int armorDamage = Mathf.CeilToInt(armorCalc);
            int healthDamage = Mathf.FloorToInt(healthCalc);

            health -= healthDamage;
            armor -= armorDamage;

            if (armor < 0) {
                health += armor;
                armor = 0;
            }
        } else {
            shield -= damage;
        }

        if (health <= 0) {
            Die(offendingCharacter);
        } else {
            Managers.Audio.PlayHitSound(1);
        }

        // Debug.Log(damage.ToString() + ": " + healthDamage.ToString() + ", " + armorDamage.ToString());

        // float powerDamage = (float)damage / 100.0f;
        float powerDamage = 0.25f; // For testing
        Managers.Gameplay.AddPowerY(-powerDamage);
    }

    public override void Die(Character offendingCharacter) {
        Managers.Audio.PlayDeathSound(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateAnim() {
        anim = characterMesh.GetComponent<Animator>();
    }

    public void SetWeaponFlashObjects(Transform muzzle, GameObject light) {
        flashMuzzle = null;
        muzzleLight = null;
        if (muzzle != null) {
            flashMuzzle = muzzle;
        }
        if (light != null) {
            muzzleLight = light;
        }
    }

    void UpdateAnimSpeed() {
        anim.speed = 1 + playerPowerLevel;
    }

    bool AttackSetNormal() {
        return switchController.AttackSetNormal();
    }

    bool AttackSetMelee() {
        return switchController.AttackSetMelee();
    }

    bool AttackSetGrenade() {
        return switchController.AttackSetGrenade();
    }

    bool AttackSetSwitch() {
        return switchController.AttackSetSwitch();
    }

    public void AddBonusVelocity(Vector3 _velocity) {
        bonusVelocity = _velocity;
        bonusVelocityQueued = true;
    }

    Vector3 GetBonusVelocity() {
        if (bonusVelocityQueued) {
            bonusVelocityQueued = false;
            return bonusVelocity;
        } else {
            return Vector3.zero;
        }
    }

    public void UnlockJumpQueue() {
        jumpQueueLock = false;
    }

    public override GameData Save(GameData gameData, int id) {
        PlayerData playerData = new PlayerData();

        playerData.health = health;
        playerData.armor = armor;
        playerData.grenades = grenadeCount;
        playerData.weaponIndex = switchController.currentWeaponIndex;
        playerData.crouched = crouched;

        playerData.clips = new int[ammoClips.Length];
        playerData.ammo = new int[weapons.Length];
        playerData.weaponLocks = new bool[weapons.Length];
        for (int i = 0; i < weapons.Length; i++) {
            playerData.clips[i] = ammoClips[i];
            playerData.ammo[i] = weapons[i].ammoPool;
            playerData.weaponLocks[i] = weapons[i].locked;
        }

        // Positioning
        playerData.position = Vector3Surrogate.ConvertFromVector3(transform.position);
        playerData.rotation = Vector3Surrogate.ConvertFromVector3(_rotation);
        playerData.camRotation = Vector3Surrogate.ConvertFromVector3(cam.transform.localEulerAngles);
        playerData.velocity = Vector3Surrogate.ConvertFromVector3(velocity);

        gameData.SetPlayerData(playerData);
        return gameData;
    }

    public override void Load(GameData gameData, int id) {
        PlayerData playerData = gameData.playerData;

        loadedData = playerData;
    }

    void ApplyLoadedData() {
        health = loadedData.health;
        armor = loadedData.armor;
        grenadeCount = loadedData.grenades;
        switchController.ForceSwitchCurrentWeapon(loadedData.weaponIndex);
        crouched = loadedData.crouched;

        for (int i = 0; i < weapons.Length; i++) {
            ammoClips[i] = loadedData.clips[i];
            weapons[i].ammoPool = loadedData.ammo[i];
            weapons[i].locked = loadedData.weaponLocks[i];
        }

        transform.position = loadedData.position.ConvertedToVector3;
        _rotation.y = loadedData.rotation.ConvertedToVector3.y;
        cameraXRotation = loadedData.camRotation.ConvertedToVector3.x;
        velocity = loadedData.velocity.ConvertedToVector3;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(targetLocation, 1);
    }
}
