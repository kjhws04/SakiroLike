using UnityEngine;

namespace SA
{
    public class WeaponManager : MonoBehaviour
    {
        public MeleeWeaponDamageCollider meleeDamageCol;

        private void Awake()
        {
            meleeDamageCol = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        public void SetWeaponDamage(CharacterManager character, WeaponItem weapon)
        {
            meleeDamageCol.characterCausingDamage = character;
            meleeDamageCol.physicalDamage = weapon.physicalDamage;
            meleeDamageCol.magicDamage = weapon.magicDamage;
            meleeDamageCol.fireDamage = weapon.fireDamage;
            meleeDamageCol.lightningDamage = weapon.lightningDamage;
            meleeDamageCol.holyDamage = weapon.holyDamage;

            meleeDamageCol.light_Attack_01_Modifier = weapon.light_Attack_01_Modifier;
            meleeDamageCol.light_Attack_02_Modifier = weapon.light_Attack_02_Modifier;
            meleeDamageCol.light_Attack_03_Modifier = weapon.light_Attack_03_Modifier;
            meleeDamageCol.light_Attack_04_Modifier = weapon.light_Attack_04_Modifier;
            meleeDamageCol.light_Attack_05_Modifier = weapon.light_Attack_05_Modifier;

            meleeDamageCol.heavy_Attack_01_Modifier = weapon.heavy_Attack_01_Modifier;
            meleeDamageCol.heavy_Attack_02_Modifier = weapon.heavy_Attack_02_Modifier;

            meleeDamageCol.hold_Attack_01_Modifier = weapon.hold_Attack_01_Modifier;
            meleeDamageCol.hold_Attack_02_Modifier = weapon.hold_Attack_02_Modifier;
        }
    }
}