using UnityEngine;

namespace SA
{
    [System.Serializable]
    // ��� ���� ���Ͽ� ���� �� �����͸� �����ϱ� ���ϱ� ������, �� ��ũ��Ʈ�� MonoBehaviour�� �ƴϰ� ��� ����ȭ �����մϴ�.
    public class CharacterSaveData
    {
        [Header("Scene Index")]
        public int sceneIndex =1;

        [Header("Character Name")]
        public string characterName = "Character";

        [Header("Time Played")]
        public float timePlayed;

        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        public float xRotation;
        public float yRotation;
        public float zRotation;

        [Header("Resource")]
        public int currentHealth;
        public float currentStamia;

        [Header("Stats")]
        public int vitality;
        public int endurance;
    }
}