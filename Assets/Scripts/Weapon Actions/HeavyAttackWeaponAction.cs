using UnityEngine;

namespace SA
{
    [CreateAssetMenu(fileName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponAction : WeaponItemAction
    {
        [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01";
        [SerializeField] string heavy_Attack_02 = "Main_Heavy_Attack_02";

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
            if (playerPerformingAcion.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAcion.isPerformingAcion)
            {
                playerPerformingAcion.playerCombatManager.canComboWithMainHandWeapon = false;

                if (playerPerformingAcion.characterCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
                {
                    playerPerformingAcion.playerAnimationManager.PlayTargetAttackAnimation(AttackType.HeavyAttack02, heavy_Attack_02, true);
                }
                else
                {
                    playerPerformingAcion.playerAnimationManager.PlayTargetAttackAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
                }
            }
            else if (!playerPerformingAcion.isPerformingAcion)
            {
                playerPerformingAcion.playerAnimationManager.PlayTargetAttackAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
            }
        }
    }
}