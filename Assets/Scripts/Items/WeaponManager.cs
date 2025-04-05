using UnityEngine;

namespace SA
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] MeleeWeaponDamageCollider meleeDamageCol;

        private void Awake()
        {
            meleeDamageCol = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        public void SetWeaponDamage(CharacterManager character, WeaponItem weapon)
        {
            meleeDamageCol.characterHitDamage = character;
            meleeDamageCol.physicalDamage = weapon.physicalDamage;
            meleeDamageCol.magicDamage = weapon.magicDamage;
            meleeDamageCol.fireDamage = weapon.fireDamage;
            meleeDamageCol.lightningDamage = weapon.lightningDamage;
            meleeDamageCol.holyDamage = weapon.holyDamage;
        }
    }
}