using Unity.Hierarchy;
using UnityEngine;

namespace SA
{
    public class MeleeWeaponDamageCollider : DamageColider
    {
        [Header("Attack Character")]
        public CharacterManager characterCausingDamage;

        [Header("Weapon Attack Modifiers")]
        public float light_Attack_01_Modifier;
        public float light_Attack_02_Modifier;
        public float heavy_Attack_01_Modifier;
        public float heavy_Attack_02_Modifier;
        public float hold_Attack_01_Modifier;   
        public float hold_Attack_02_Modifier;

        protected override void Awake()
        {
            base.Awake();

            if (damageCol == null)
                damageCol = GetComponent<Collider>();

            damageCol.enabled = false;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                // 자해 방지
                if (damageTarget == characterCausingDamage)
                    return;

                contackPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                // check if we can damage this target based friendly fire

                // check if target is Blocking

                // check if target is invulnerable

                // damage
                DamageTarget(damageTarget);
            }
        }

        protected override void DamageTarget(CharacterManager damageTarget)
        {
            // if we don't want to damage the same target more than onve in a single attack
            // so we add the, to a list that checks before applying damage
            if (charactersDamaged.Contains(damageTarget))
                return;

            charactersDamaged.Add(damageTarget);

            TakeHealthDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealthDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contackPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.LightAttack02:
                    ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack01:
                    ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack02:
                    ApplyAttackDamageModifiers(heavy_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.HoldAttack01:
                    ApplyAttackDamageModifiers(hold_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.HoldAttack02:
                    ApplyAttackDamageModifiers(hold_Attack_02_Modifier, damageEffect);
                    break;
            }

            // damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

            if (characterCausingDamage.IsOwner)
            {
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    characterCausingDamage.NetworkObjectId,
                    damageEffect.physicalDamage,
                    damageEffect.magicDamage,
                    damageEffect.fireDamage,
                    damageEffect.lightningDamage,
                    damageEffect.holyDamage,
                    damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x,
                    damageEffect.contactPoint.y,
                    damageEffect.contactPoint.z);
            }
        }

        private void ApplyAttackDamageModifiers(float modifierm, TakeHealthDamageEffect damage)
        {
            damage.physicalDamage *= modifierm;
            damage.magicDamage *= modifierm;
            damage.fireDamage *= modifierm;
            damage.lightningDamage *= modifierm;
            damage.holyDamage *= modifierm;
        }
    }
}