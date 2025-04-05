using UnityEngine;
using System.IO;
using System;
using System.Linq.Expressions;

namespace SA
{
    public class SaveFileDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string saveFileName = "";

        // 새로운 저장 파일을 만들기 전, 이미 슬롯 중 하나에 캐릭터가 존재하는지 확인해야 함
        public bool CheckToSeeIfFileExists()
        {
            if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 저장 파일을 삭제하는 메소드
        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }

        // 새로운 캐릭터 저장 파일을 만드는 메소드
        public void CreateNewCharacterSaveFile(CharacterSaveData data)
        {
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // 파일이 쓰일 디렉토리가 존재하지 않으면 생성
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log($"CREATE SAVE FILE, AT SAVE PATH: {savePath}");

                // C# 게임 데이터 객체를 json으로 직렬화
                string dataToStore = JsonUtility.ToJson(data, true);

                // 파일을 시스템에 쓰기
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"ERROR TO SAVE DATA: {savePath} : {ex.Message}");
            }
        }

        // 저장 파일을 불러오는 메소드
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;

            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if (File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {
                    Debug.Log($"ERROR TO LOAD DATA: {loadPath} : {ex.Message}");
                }

            }

            return characterData;
        }
    }
}