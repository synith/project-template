using System.IO;
using UnityEngine;

namespace Slayground.Common
{
    public class SaveSystem
    {
        public void Save<T>(T data, string filename)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, filename), json);
        }

        public T Load<T>(string filename)
        {
            string filePath = Path.Combine(Application.persistentDataPath, filename);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(json);
            }

            return default(T);
        }
    }
}