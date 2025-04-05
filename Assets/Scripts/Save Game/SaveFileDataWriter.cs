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

        // ���ο� ���� ������ ����� ��, �̹� ���� �� �ϳ��� ĳ���Ͱ� �����ϴ��� Ȯ���ؾ� ��
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

        // ���� ������ �����ϴ� �޼ҵ�
        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }

        // ���ο� ĳ���� ���� ������ ����� �޼ҵ�
        public void CreateNewCharacterSaveFile(CharacterSaveData data)
        {
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // ������ ���� ���丮�� �������� ������ ����
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log($"CREATE SAVE FILE, AT SAVE PATH: {savePath}");

                // C# ���� ������ ��ü�� json���� ����ȭ
                string dataToStore = JsonUtility.ToJson(data, true);

                // ������ �ý��ۿ� ����
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

        // ���� ������ �ҷ����� �޼ҵ�
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