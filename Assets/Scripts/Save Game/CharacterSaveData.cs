using UnityEngine;

namespace SA
{
    [System.Serializable]
    // 모든 저장 파일에 대한 이 데이터를 참조하길 원하기 때문에, 이 스크립트는 MonoBehaviour가 아니고 대신 직렬화 가능합니다.
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