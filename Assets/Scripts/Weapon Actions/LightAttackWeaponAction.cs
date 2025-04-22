using UnityEngine;

namespace SA
{
    [CreateAssetMenu(fileName ="Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponAction : WeaponItemAction
    {
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAcion, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAcion, weaponPerformingAction);
            if (!playerPerformingAcion.IsOwner)
                return;
 
            if (playerPerformingAcion.playerNetworkManager.currentStamina.Value <= 0)
                return;

            if (!playerPerformingAcion.isGrounded)
                return;

            PerformLightAttack(playerPerformingAcion, weaponPerformingAction);
        }

        private void PerformLightAttack(PlayerManager playerPerformingAcion, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAcion.playerNetworkManager.isUsingRightHand.Value)
            {
                playerPerformingAcion.playerAnimationManager.PlayTargetAttackAnimation(AttackType.LightAttack01, light_Attack_01, true);
            }
            if (playerPerformingAcion.playerNetworkManager.isUsingLeftHand.Value)
            {

            }
        }
    }
}