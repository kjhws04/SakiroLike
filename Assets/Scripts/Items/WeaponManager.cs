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
        }
    }
}