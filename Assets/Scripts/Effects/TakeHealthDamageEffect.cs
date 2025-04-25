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

            // [����]�� Ȯ�� (�������)

            // �������� ���
            CalculateDamage(character);
            // �������� ��� ���⿡�� �Դ��� Ȯ��
            PlayDirectionalBasedDamageAnimation(character);
            // ������ �ִϸ��̼��� ���
            // ���� ��(��, ���� ��)�� Ȯ��
            // ������ ���� FX�� ���
            PlayDamageSFX(character);
            // ������ VFX(��)�� �����
            PlayDamageVFX(character);

            // ĳ���Ͱ� A.I�� ��� �������� ������ ĳ���Ͱ� �����ϴ��� Ȯ��
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            if (characterCausingDamage == null)
            {
                // �������� Ȯ ���ϰ� �⺻ ������(����, ����, ȭ��, ����, �ż�)��  ����
            }

            // ĳ������ ��� �� Ȯ���ϰ� ���������� ��

            // ĳ������ ������ Ȯ���ϰ� ���������� ������� ��

            // ��� ������ ������ �ջ��ϰ� ���� �������� ����

            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }
            
            //Debug.Log($"Final Damage : {finalDamageDealt}");
            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
        }

        private void PlayDamageVFX(CharacterManager character)
        {
            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        }
        
        private void PlayDamageSFX(CharacterManager character)
        {
            AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

            character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
        }

        private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            // todo : calculate if poise is broken
            poiseIsBroken = true;

            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimationManager.GetRandomAnimationFromList(character.characterAnimationManager.forward_Medium_Damage);
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                damageAnimation = character.characterAnimationManager.GetRandomAnimationFromList(character.characterAnimationManager.forward_Medium_Damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimationManager.GetRandomAnimationFromList(character.characterAnimationManager.backward_Medium_Damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimationManager.GetRandomAnimationFromList(character.characterAnimationManager.left_Medium_Damage);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimationManager.GetRandomAnimationFromList(character.characterAnimationManager.right_Medium_Damage);
            }

            // if poise os broken, play a staggering damage animation
            if (poiseIsBroken)
            {
                character.characterAnimationManager.lastDamageAnimationPlayed = damageAnimation;
                character.characterAnimationManager.PlayTargetActionAnimation(damageAnimation, true);
            }
        }
    }
}