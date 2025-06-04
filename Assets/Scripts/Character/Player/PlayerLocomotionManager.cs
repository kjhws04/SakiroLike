using SA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        [Header("Movement Setting")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float sprintSpeed = 7;
        [SerializeField] float rotationSpeed = 15;
        [SerializeField] int sprintStaminaCost = 2;

        [Header("Jump")]
        [SerializeField] float jumpStmainaCost = 25;
        [SerializeField] float jumpHeight = 5;
        [SerializeField] float jumpForwardSpeed = 5;
        [SerializeField] float fallSpeed = 2;
        [SerializeField] float sprintJumpDistance = 1;
        [SerializeField] float runningJumpDistance = 0.5f;
        [SerializeField] float walkingJumpDistance = 0.25f;
        private Vector3 jumpDirection;

        [Header("Dodge")]
        private Vector3 rollDirection;
        [SerializeField] float dodgeStmainaCost = 25;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (player.IsOwner)
            {
                player.characterNetworkManager.animatorVerticalMovement.Value = verticalMovement;
                player.characterNetworkManager.animatorHorizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.animatorMoveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovement = player.playerNetworkManager.animatorVerticalMovement.Value;
                horizontalMovement = player.playerNetworkManager.animatorHorizontalMovement.Value;
                moveAmount = player.playerNetworkManager.animatorMoveAmount.Value;

                if (!player.playerNetworkManager.isLockOn.Value || player.playerNetworkManager.isSprinting.Value)
                {
                    player.playerAnimationManager.UpdateanimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
                }
                else
                {
                    player.playerAnimationManager.UpdateanimatorMovementParameters(horizontalMovement, verticalMovement, player.playerNetworkManager.isSprinting.Value);
                }
            }
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            HandleJumpingMovement();
            HandleFreeFallMovement();
        }

        #region Movement
        private void HandleGroundedMovement()
        {
            if (!player.characterLocomotionManager.canRotate)
                return;

            GetMovementValue();
            MovemetDirectionCheck();
            MovementForceCheck();
        }

        /// <summary>
        /// HandleGroundedMovement 함수에서 호출
        /// </summary>
        private void GetMovementValue()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
        }
        private void MovemetDirectionCheck()
        {
            moveDirection = SA.PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection += SA.PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;
        }
        private void MovementForceCheck()
        {
            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
            }
            else
            {
                if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (PlayerInputManager.instance.moveAmount <= 0.5f)
                {
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }
        }

        public void HandleSprinting()
        {
            if (player.isPerformingAcion)
                player.playerNetworkManager.isSprinting.Value = false;

            if (player.playerNetworkManager.currentStamina.Value <= 0)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }

            // run 상태일 때, sprint 가능
            if (moveAmount >= 0.5)
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -= sprintStaminaCost * Time.deltaTime;
            }
        }
        #endregion

        #region Rotation
        private void HandleRotation()
        {
            if (player.isDead.Value)
                return;

            if (!player.characterLocomotionManager.canRotate)
                return;

            if (player.playerNetworkManager.isLockOn.Value)
            {
                if (player.playerNetworkManager.isSprinting.Value || player.playerLocomotionManager.isRolling)
                {
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                    targetDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                        targetDirection = transform.forward;

                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = finalRotation;
                }
                else
                {
                    if (player.playerCombatManager.currentTarget == null)
                        return;

                    Vector3 targetDirection;
                    targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                    targetDirection.y = 0;
                    targetDirection.Normalize();
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = finalRotation;
                }
            }
            else
            {
                RotationDirectionCheck();
                PerformRotation();
            }
        }

        /// <summary>
        /// HandleRotation 함수에서 호출
        /// </summary>
        private void RotationDirectionCheck()
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = SA.PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection += SA.PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }
        }
        private void PerformRotation()
        {
            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
        #endregion

        #region Actions
        // Handle 
        private void HandleJumpingMovement()
        {
            if (player.characterNetworkManager.isJumping.Value)
            {
                player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
            }
        }

        private void HandleFreeFallMovement()
        {
            if (!player.characterLocomotionManager.isGrounded)
            {
                Vector3 freeFallDirection;

                freeFallDirection = SA.PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                freeFallDirection += SA.PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
                freeFallDirection.y = 0;

                player.characterController.Move(freeFallDirection * fallSpeed * Time.deltaTime);
            }
        }
        
        // Perform
        public void AttemptToPerformDodge()
        {
            if (player.isPerformingAcion)
                return;

            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;

            DisideRollOrBackStep();

            player.playerNetworkManager.currentStamina.Value -= dodgeStmainaCost;
        }

        /// <summary>
        /// AttemptToPerformDodge 함수에서 호출
        /// </summary>
        private void DisideRollOrBackStep()
        {

            if (PlayerInputManager.instance.moveAmount > 0) // 구르기
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                player.playerAnimationManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);
                player.playerLocomotionManager.isRolling = true;
            }
            else //백스탭
            {
                player.playerAnimationManager.PlayTargetActionAnimation("Back_Step_01", true, true);
            }
        }

        public void AttemptToPerformJump()
        {
            if (player.isPerformingAcion)
                return;

            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;

            if (player.characterNetworkManager.isJumping.Value)
                return;

            if (!player.characterLocomotionManager.isGrounded)
                return;

            JumpAnimAndBoolAndCostCheck();
            JumpDirectionAndForceCheck();
        }

        /// <summary>
        /// AttemptToPerformJump 함수에서 호출
        /// </summary>
        private void JumpAnimAndBoolAndCostCheck()
        {
            player.playerAnimationManager.PlayTargetActionAnimation("Main_Jump_Start_01", false);
            player.characterNetworkManager.isJumping.Value = true;
            player.playerNetworkManager.currentStamina.Value -= jumpStmainaCost;

        }
        private void JumpDirectionAndForceCheck()
        {
            jumpDirection = SA.PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            jumpDirection += SA.PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            jumpDirection.y = 0;

            if (jumpDirection != Vector3.zero)
            {
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= sprintJumpDistance;
                }
                else if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    jumpDirection *= runningJumpDistance;
                }
                else if (PlayerInputManager.instance.moveAmount <= 0.5f)
                {
                    jumpDirection *= walkingJumpDistance;
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// core_main_jump_01_start 애니메이션에서 Event로 호출
        /// </summary>
        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }
        #endregion

    }
}