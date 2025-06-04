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
            if (player.characterAnimationManager.applyRootMotion)
            {
                Vector3 velocity = player.anim.deltaPosition;
                player.characterController.Move(velocity);
                player.transform.rotation *= player.anim.deltaRotation;
            }
        }

        public override void EnableCanDoCombo()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerCombatManager.canComboWithMainHandWeapon = true;
            }
            else
            {

            }
        }

        public override void DisableCanDoCombo()
        {
            player.playerCombatManager.canComboWithMainHandWeapon = false;
        }
    }
}