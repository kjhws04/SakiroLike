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

                // �ν��Ͻ�ȭ �� �� ������ ������ ���� ����
                // TakeStaminaDamageEffect effect = Instantiate(effectToTest) as TakeStaminaDamageEffect;
                // effect.staminaDamage = 2;
                // ProcessInstantEffect(effect);

                // �ν��Ͻ�ȭ���� ���� �� ������ ����
                // effectToTest2.staminaDamage = 2;
                // ProcessInstantEffect(effectToTest);

                InstantCharacterEffect effect = Instantiate(effectToTest);
                ProcessInstantEffect(effect);
            }
        }
    }
}