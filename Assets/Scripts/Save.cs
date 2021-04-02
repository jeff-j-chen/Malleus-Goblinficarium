using UnityEngine;
using System;
using System.IO;

public static class Save {
    private static string gamePath = "gameSave.txt";
    private static string persistentPath = "persistentSave.txt";
    public static string tutorialJson = "{\"newGame\":false,\"curCharNum\":0,\"floorItemNames\":[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"],\"floorItemTypes\":[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"],\"floorItemMods\":[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"],\"resumeItemNames\":[\"sword\",\"scroll\",\"torch\",\"\",\"\",\"\",\"\",\"\",\"\"],\"resumeItemTypes\":[\"weapon\",\"common\",\"common\",\"\",\"\",\"\",\"\",\"\",\"\"],\"resumeItemMods\":[\"harsh\",\"nothing\",\"2-1\",\"\",\"\",\"\",\"\",\"\",\"\"],\"resumeLevel\":1,\"resumeSub\":1,\"resumeAcc\":2,\"resumeSpd\":2,\"resumeDmg\":1,\"resumeDef\":2,\"floorAcc\":0,\"floorSpd\":0,\"floorDmg\":0,\"floorDef\":0,\"potionAcc\":0,\"potionSpd\":0,\"potionDmg\":0,\"potionDef\":0,\"playerStamina\":3,\"enemyStamina\":1,\"diceNumbers\":[2,1,1,1,6,2],\"diceTypes\":[\"green\",\"blue\",\"blue\",\"blue\",\"red\",\"white\"],\"dicePlayerOrEnemy\":[\"none\",\"none\",\"none\",\"none\",\"none\",\"none\"],\"diceAttachedToStat\":[\"\",\"\",\"\",\"\",\"\",\"\"],\"diceRerolled\":[false,false,false,false,false,false],\"playerWounds\":[],\"enemyWounds\":[],\"enemyNum\":6,\"usedMace\":false,\"usedAnkh\":false,\"usedHelm\":false,\"usedBoots\":false,\"isFurious\":false,\"isDodgy\":false,\"isHasty\":false,\"isBloodthirsty\":false,\"isCourageous\":false,\"expendedStamina\":0,\"numItemsDroppedForTrade\":0,\"discardableDieCounter\":0,\"enemyIsDead\":false,\"enemyAcc\":1,\"enemySpd\":1,\"enemyDmg\":1,\"enemyDef\":1}";
    public static GameData game;
    public static PersistentData persistent;

    public static void LoadTutorial() { 
        game = JsonUtility.FromJson<GameData>(tutorialJson);
    }

    public static void SaveGame() { 
        File.WriteAllText(gamePath, JsonUtility.ToJson(game));
    }

    public static void LoadGame() { 
        if (File.Exists(gamePath)) { 
            game = JsonUtility.FromJson<GameData>(File.ReadAllText(gamePath));
            if (game is null) { 
                // little extra redundancy incase something went really wrong   
                Debug.LogError("something went really wrong!");
                game = new GameData();
                SaveGame();
            }
            // Debug.Log($"just saved the game! newgame is {game.newGame}");
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
        if (File.Exists(persistentPath)) { 
            persistent = JsonUtility.FromJson<PersistentData>(File.ReadAllText(persistentPath)); 
            if (persistent is null) { 
                persistent = new PersistentData();
                SavePersistent();
            }
        }
        else { 
            Debug.Log($"no statistics found, so just created one!");
            persistent = new PersistentData();
            SavePersistent();
        }
    }
}
