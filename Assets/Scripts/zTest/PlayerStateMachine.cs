using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SA
{
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("Move Setting")]
        public float moveSpeed = 5f;
        public float acceleration = 3f;
        public float deceleration = 4f;
        public float currentSpeed = 0f;

        public float jumpForce = 5f;
        public float rollSpeedMultiplier = 2f;
        public float rollDuration = 0.5f;

        private Vector2 moveInput;
        private Vector3 moveDirection;
        private PlayerState currentState;

        public PlayerIdleState idleState;
        public PlayerMoveState moveState;
        public PlayerJumpState jumpState;
        public PlayerRollState rollState;
        public Animator anim;

        private void Awake()
        {
            idleState = new PlayerIdleState(this);
            moveState = new PlayerMoveState(this);
            jumpState = new PlayerJumpState(this);
            rollState = new PlayerRollState(this);
            anim = GetComponent<Animator>();

            currentState = idleState;
        }

        private void Start() { currentState.EnterState(); }
        private void Update() { currentState.UpdateState(); }

        private void ChangeState(PlayerState newState)
        {
            currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }

        #region Move Setting
        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            if (moveInput.magnitude > 0) { ChangeState(moveState); }
            else {ChangeState(idleState);}
        }

        public void MoveCharacter()
        {
            moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

            if (moveInput.magnitude > 0) { currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.deltaTime); }
            else { currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime); }

            transform.Translate(moveDirection * currentSpeed * Time.deltaTime);

            anim.SetFloat("x", moveInput.x);
            anim.SetFloat("y", moveInput.y);
        }
        #endregion

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ChangeState(jumpState);
            }
        }
    }
}