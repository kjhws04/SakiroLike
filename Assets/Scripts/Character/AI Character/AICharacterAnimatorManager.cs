using UnityEngine;

namespace SA
{
    public class AICharacterAnimatorManager : CharacterAnimationManager
    {
        AICharacterManager aiCharacter;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
        }

        private void OnAnimatorMove()
        {
            // host
            if (aiCharacter.IsOwner)
            {
                if (!aiCharacter.isGrounded)
                    return;

                Vector3 velocity = aiCharacter.anim.deltaPosition;
                aiCharacter.characterController.Move(velocity);
                aiCharacter.transform.rotation *= aiCharacter.anim.deltaRotation;
            }
            // client
            else
            {
                if (!aiCharacter.isGrounded)
                    return;

                Vector3 velocity = aiCharacter.anim.deltaPosition;

                aiCharacter.characterController.Move(velocity);
                aiCharacter.transform.position = Vector3.SmoothDamp(transform.position,
                    aiCharacter.characterNetworkManager.networkPosition.Value, 
                    ref aiCharacter.characterNetworkManager.networkPositionVelocity,
                    aiCharacter.characterNetworkManager.networkPositionSmoothTime);
                aiCharacter.transform.rotation *= aiCharacter.anim.deltaRotation;
            }
        }
    }
}