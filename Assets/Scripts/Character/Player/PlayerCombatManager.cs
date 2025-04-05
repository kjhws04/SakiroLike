using UnityEngine;

namespace SA
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;

        public WeaponItem currentWeaponItem;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAcion)
        {
            // �ൿ�� ����
            weaponAction.AttemptToPerformAction(player, weaponPerformingAcion);

            // �������� �ൿ�� ���������� �˸�. ����� ���������� ����.
        }
    }
}