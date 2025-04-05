using UnityEngine;

namespace SA
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // ��� ȿ�� ó�� (������ �Ա�, ġ�� ��)
        // �ð� ȿ�� ó�� (��, ȭ��, �����)
        // ���� ȿ�� ó�� (����, �����)

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
