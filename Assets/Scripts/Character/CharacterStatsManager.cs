using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering;

namespace SA
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Stamina Regen")]
        [SerializeField] float staminaRegenAmount = 1;
        private float staminaRegenTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenDelay = 2;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
        }

        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            health = vitality * 15;

            return Mathf.RoundToInt(health);
        }

        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public virtual void RegenerateStamina()
        {
            if (!character.IsOwner)
                return;

            if (character.characterNetworkManager.isSprinting.Value)
                return;

            if (character.isPerformingAcion)
                return;

            staminaRegenTimer += Time.deltaTime;

            if (staminaRegenTimer >= staminaRegenDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1f)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            if (currentStaminaAmount < previousStaminaAmount)
                staminaRegenTimer = 0;
        }
    }
}