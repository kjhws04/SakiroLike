using UnityEngine;

namespace SA
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // ��� ȿ�� ó�� (������ �Ա�, ġ�� ��)
        // �ð� ȿ�� ó�� (��, ȭ��, �����)
        // ���� ȿ�� ó�� (����, �����)

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
            // �𵨿� �������� ���� ȿ���� ��ġ�� ���, �ش� ������ ����մϴ�.
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            // �������� ���� ȿ���� ��ġ�� ���, �ش� ������ ���
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }
    }
}
