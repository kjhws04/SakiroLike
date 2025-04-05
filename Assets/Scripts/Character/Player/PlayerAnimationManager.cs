using UnityEngine;

namespace SA
{
    public class PlayerAnimationManager : CharacterAnimationManager
    {
        PlayerManager player;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        private void OnAnimatorMove()
        {
            if (player.applyRootMotion)
            {
                Vector3 velocity = player.anim.deltaPosition;
                player.characterController.Move(velocity);
                player.transform.rotation *= player.anim.deltaRotation;
            }
        }
    }
}