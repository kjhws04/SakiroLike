using SA;
using UnityEngine;

namespace SA
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete Later")]
        [SerializeField] InstantCharacterEffect effectToTest;
        [SerializeField] TakeStaminaDamageEffect effectToTest2;
        [SerializeField] bool processEffect = false;

        protected void Update()
        {
            if (processEffect)
            {
                processEffect = false;

                // 인스턴스화 할 때 원본이 영향을 받지 않음
                // TakeStaminaDamageEffect effect = Instantiate(effectToTest) as TakeStaminaDamageEffect;
                // effect.staminaDamage = 2;
                // ProcessInstantEffect(effect);

                // 인스턴스화하지 않을 때 원본이 변경
                // effectToTest2.staminaDamage = 2;
                // ProcessInstantEffect(effectToTest);

                InstantCharacterEffect effect = Instantiate(effectToTest);
                ProcessInstantEffect(effect);
            }
        }
    }
}