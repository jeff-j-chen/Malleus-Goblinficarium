using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    static string path = Application.persistentDataPath + "/save.data";
    public static void SaveData(Scripts scripts) {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);
        Data data = new Data(scripts);
        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }

    public static Data LoadData() { 
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);
            Data data = formatter.Deserialize(fileStream) as Data;
            fileStream.Close();
            return data;
        }
        else { 
            Debug.LogError($"savefile not found in {path}");
            return null;
        }
    }
}
