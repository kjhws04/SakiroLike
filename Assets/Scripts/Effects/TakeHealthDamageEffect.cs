using UnityEngine;
using UnityEngine.Rendering;

namespace SA
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Health Damage")]
    public class TakeHealthDamageEffect : InstantCharacterEffect
    {
        [Header("Character Health Damage")]
        public CharacterManager characterCausingDamage;

        [Header("Damage Options")]
        public float physicalDamage = 0;
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("Final Damage")]
        private int finalDamageDealt = 0;

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("Sound FX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX;

        [Header("Direction Damage Taken From")]
        public float angleHitFrom;
        public Vector3 contactPoint;

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            if (character.isDead.Value)
            {
                return;
            }

            // [무적]을 확인 (구르기등)

            // 데미지를 계산
            CalculateDamage(character);
            // 데미지가 어느 방향에서 왔는지 확인
            // 데미지 애니메이션을 재생
            // 빌드 업(독, 출혈 등)을 확인
            // 데미지 사운드 FX를 재생
            // 데미지 VFX(피)를 재생합

            // 캐릭터가 A.I인 경우 데미지를 입히는 캐릭터가 존재하는지 확인
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            if (characterCausingDamage == null)
            {
                // 데미지룰 확 인하고 기본 데미지(물리, 마법, 화염, 번개, 신성)를  설정
            }

            // 캐릭터의 평소 방어를 확인하고 데미지에서 뺌

            // 캐릭터의 방어력을 확인하고 데미지에서 백분율로 뺌

            // 모든 데미지 유형을 합산하고 최종 데미지를 적용

            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }
            
            //Debug.Log($"Final Damage : {finalDamageDealt}");
            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
        }
    }
}