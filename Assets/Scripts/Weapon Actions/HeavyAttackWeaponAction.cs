using UnityEngine;

namespace SA
{
    [CreateAssetMenu(fileName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponAction : WeaponItemAction
    {
        [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAcion, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAcion, weaponPerformingAction);
            if (!playerPerformingAcion.IsOwner)
                return;

            if (playerPerformingAcion.playerNetworkManager.currentStamina.Value <= 0)
                return;

            if (!playerPerformingAcion.isGrounded)
                return;

            PerformHeavyAttack(playerPerformingAcion, weaponPerformingAction);
        }

        private void PerformHeavyAttack(PlayerManager playerPerformingAcion, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAcion.playerNetworkManager.isUsingRightHand.Value)
            {
                playerPerformingAcion.playerAnimationManager.PlayTargetAttackAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
            }
            if (playerPerformingAcion.playerNetworkManager.isUsingLeftHand.Value)
            {

            }
        }
    }
}