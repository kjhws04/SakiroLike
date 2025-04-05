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

        [Header("Character Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("Character Acion Input")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput = false;
        [SerializeField] bool jumpInput = false;
        [SerializeField] bool RB_Input = false;

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

                // 단발성 입력
                playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
                playerControls.PlayerActions.Dodge.performed += ctx => dodgeInput = true;
                playerControls.PlayerActions.Jump.performed += ctx => jumpInput = true;
                playerControls.PlayerActions.RB.performed += ctx => RB_Input = true;

                // 지속성 입력 
                playerControls.PlayerActions.Sprint.performed += ctx => sprintInput = true;
                playerControls.PlayerActions.Sprint.canceled += ctx => sprintInput = false;
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
            HandleCharacterMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleRBInput();
        }

        #region Move
        private void HandleCharacterMovementInput()
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

            player.playerAnimationManager.UpdateanimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
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

        private void HandleRBInput()
        {
            if (RB_Input)
            {
                RB_Input = false;

                player.playerNetworkManager.SetCharacterAcionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightWeapon.rbAction, player.playerInventoryManager.currentRightWeapon);
            }
        }
        #endregion
    }
}