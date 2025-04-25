using UnityEngine;

namespace SA
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // 즉시 효과 처리 (데미지 입기, 치유 등)
        // 시간 효과 처리 (독, 화상, 빌드업)
        // 정적 효과 처리 (버프, 디버프)

        CharacterManager character;

        [Header("VFX")]
        [SerializeField] GameObject bloodSplatterVFX;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }   

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }

        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            // 모델에 수동으로 혈흔 효과를 배치한 경우, 해당 버전을 재생합니다.
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            // 수동으로 혈흔 효과를 배치한 경우, 해당 버전을 재생
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }
    }
}
