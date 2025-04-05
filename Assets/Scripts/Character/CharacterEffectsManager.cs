using UnityEngine;

namespace SA
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // 즉시 효과 처리 (데미지 입기, 치유 등)
        // 시간 효과 처리 (독, 화상, 빌드업)
        // 정적 효과 처리 (버프, 디버프)

        CharacterManager character;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }   

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }
    }
}
