using UnityEngine;
using System;
using System.IO;
public static class Save {
    private static string gamePath = "gameSave.txt";
    private static string persistentPath = "persistentSave.txt";
    public static GameData game;
    public static PersistentData persistent;
    public static void SaveGame() { 
        File.WriteAllText(gamePath, JsonUtility.ToJson(game));
    }

    public static void LoadGame() { 
        if (File.Exists(gamePath)) { 
            game = JsonUtility.FromJson<GameData>(File.ReadAllText(gamePath));
        }
        else { 
            Debug.Log($"no save found, so just created one!");
            game = new GameData();
            SaveGame();
        }
    }

    public static void SavePersistent() { 
        File.WriteAllText(persistentPath, JsonUtility.ToJson(persistent));
    }

    public static void LoadPersistent() { 
        if (File.Exists(persistentPath)) { persistent = JsonUtility.FromJson<PersistentData>(File.ReadAllText(persistentPath)); }
        else { 
            Debug.Log($"no statistics found, so just created one!");
            persistent = new PersistentData();
            SavePersistent();
        }
    }
}
