using SA;
using UnityEngine;

namespace SA
{
    public class AIDuweokshiniCombatManager : AICharacterCombatManager
    {
        [Header("Changgui Damage Collider")]
        [SerializeField] MonsterDamageCollider rightHandDamageCollider;
        [SerializeField] MonsterDamageCollider leftHandDamageCollider;

        //[Header("Damage")]
        //[SerializeField] float baseDamage = 25;
        //[SerializeField] float attack01DamageModifier = 1.0f;
        //[SerializeField] float attack02DamageModifier = 2.0f;

        public void SetAttack01Damage()
        {
            //rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
            //leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }
    }
}