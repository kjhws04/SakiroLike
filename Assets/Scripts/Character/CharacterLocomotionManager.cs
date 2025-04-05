using UnityEngine;

namespace SA
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -20;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundCheckSphereRadius = 1f;  
        [SerializeField] protected Vector3 yVelocity;
        [SerializeField] protected float groundedYVelocity = -20;
        [SerializeField] protected float fallStartYVelocity = -5;
        protected bool fallingVelocityHAsBeenSet = false;
        [SerializeField] protected float inAirTimer = 0;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (character.isGrounded)
            {
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityHAsBeenSet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHAsBeenSet)
                {
                    fallingVelocityHAsBeenSet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer += Time.deltaTime;
                character.anim.SetFloat("InAirTimer", inAirTimer);

                yVelocity.y += gravityForce * Time.deltaTime;
            }
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
        }

        protected void OnDrawGizmosSelected()
        {
            //Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }
    }
}