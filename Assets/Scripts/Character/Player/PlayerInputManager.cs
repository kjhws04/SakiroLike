using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SA
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        public PlayerManager player;

        PlayerControls playerControls;

        [Header("Camera Movement Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        [Header("Lock On Input")]
        [SerializeField] bool lockOnInput;
        [SerializeField] bool lockOnLeftInput;
        [SerializeField] bool lockOnRightInput;
        private Coroutine lockOnCoroutine;

        [Header("Character Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("Character Acion Input")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput = false;
        [SerializeField] bool jumpInput = false;
        [SerializeField] bool switchLeftWeaponInput = false;
        [SerializeField] bool switchRightWeaponInput = false;

        [Header("Character Attack Input")]
        [SerializeField] bool lightAttack = false;
        [SerializeField] bool heavyAttack = false;
        [SerializeField] bool holdAttack = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnSceneChanged;

            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;

                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            else
            {
                instance.enabled = false;

                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                // Player & Camera Movement Input
                playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

                // Action Input
                playerControls.PlayerActions.Dodge.performed += ctx => dodgeInput = true;
                playerControls.PlayerActions.Jump.performed += ctx => jumpInput = true;
                playerControls.PlayerActions.Sprint.performed += ctx => sprintInput = true;
                playerControls.PlayerActions.Sprint.canceled += ctx => sprintInput = false;
                playerControls.PlayerActions.SwitchLeftWeapon.performed += ctx => switchLeftWeaponInput = true;
                playerControls.PlayerActions.SwitchRightWeapon.performed += ctx => switchRightWeaponInput = true;

                // Attack Input
                playerControls.PlayerActions.LightAttack.performed += ctx => lightAttack = true;
                playerControls.PlayerActions.HeavyAttack.performed += ctx => heavyAttack = true;
                playerControls.PlayerActions.HoldAttack.performed += ctx => holdAttack = true;
                playerControls.PlayerActions.HoldAttack.canceled += ctx => holdAttack = false;

                // Lock On Input
                playerControls.PlayerActions.LockOn.performed += ctx => lockOnInput = true;
                playerControls.PlayerActions.SeekLeftLockOntarget.performed += ctx => lockOnLeftInput = true;
                playerControls.PlayerActions.SeekRightLockOntarget.performed += ctx => lockOnRightInput = true;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();

            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();

            HandleLightAttackInput();
            HandleHeavyAttackInput();
            HandleHoldAttackInput();
            HandleSwitchRightWeaponInput();
            HandleSwitchLeftWeaponInput();

            HandleLockOnInput();
            HandleLockOnSwithTargetInput();
        }

        #region Movement
        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1;
            }

            if (player == null)
                return;

            if (!player.playerNetworkManager.isLockOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimationManager.UpdateanimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                player.playerAnimationManager.UpdateanimatorMovementParameters(horizontalInput, verticalInput, player.playerNetworkManager.isSprinting.Value);
            }
        }

        private void HandleCameraMovementInput()
        {
            cameraHorizontalInput = cameraInput.x;
            cameraVerticalInput = cameraInput.y;
        }
        #endregion

        #region Actions
        private void HandleDodgeInput()
        {
            if (dodgeInput)
            {
                dodgeInput = false;

                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleSprintInput()
        {
            if (sprintInput)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if (jumpInput)
            {
                jumpInput = false;

                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }

        private void HandleSwitchRightWeaponInput()
        {
            if (switchRightWeaponInput)
            {
                switchRightWeaponInput = false;
                player.playerEquipmentManager.SwitchRightHand();
            }
        }

        private void HandleSwitchLeftWeaponInput()
        {
            if (switchLeftWeaponInput)
            {
                switchLeftWeaponInput = false;
                player.playerEquipmentManager.SwitchLeftHand();
            }
        }
        #endregion

        #region Attack
        private void HandleLightAttackInput()
        {
            if (lightAttack)
            {
                lightAttack = false;

                player.playerNetworkManager.SetCharacterAcionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightWeapon.lightAttackAction, player.playerInventoryManager.currentRightWeapon);
            }
        }

        private void HandleHeavyAttackInput()
        {
            if (heavyAttack)
            {
                heavyAttack = false;

                player.playerNetworkManager.SetCharacterAcionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightWeapon.HeavyAttackAction, player.playerInventoryManager.currentRightWeapon);
            }
        }

        private void HandleHoldAttackInput()
        {
            if (player.isPerformingAcion)
            {
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerNetworkManager.isHoldAttack.Value = holdAttack;
                }
            }
        }
        #endregion

        #region Lock On
        private void HandleLockOnInput()
        {
            if (player.playerNetworkManager.isLockOn.Value)
            {
                if (player.playerCombatManager.currentTarget == null)
                    return;

                if (player.playerCombatManager.currentTarget.isDead.Value)
                {
                    player.playerNetworkManager.isLockOn.Value = false;
                }

                if (lockOnCoroutine != null)
                    StopCoroutine(lockOnCoroutine);

                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }

            if (lockOnInput && player.playerNetworkManager.isLockOn.Value)
            {
                lockOnInput = false;
                PlayerCamera.instance.ClearLockOnTarget();   
                player.playerNetworkManager.isLockOn.Value = false;
                return;
            }

            if (lockOnInput && !player.playerNetworkManager.isLockOn.Value)
            {
                lockOnInput = false;

                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.nearestLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockOn.Value = true;
                }
            }
        }

        private void HandleLockOnSwithTargetInput()
        {
            if (lockOnLeftInput)
            {
                lockOnLeftInput = false;

                if (player.playerNetworkManager.isLockOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.instance.leftLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                    }
                }
            }

            if (lockOnRightInput)
            {
                lockOnRightInput = false;

                if (player.playerNetworkManager.isLockOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.instance.rightLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                    }
                }
            }
        }
        #endregion
    }
}