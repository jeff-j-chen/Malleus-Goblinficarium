using System.Linq;
using System.IO;
using UnityEngine;
public static class Save {
    // private static string gamePath = "gameSave.txt";
    private static readonly string GamePath = Application.persistentDataPath + "gameSave.txt";
    private static readonly string TutorialGamePath = Application.persistentDataPath + "tutorialGameSave.txt";
    // private static string persistentPath = "persistentSave.txt";
    private static readonly string PersistentPath = Application.persistentDataPath + "persistentSave.txt";
    
    private static readonly string tutorialJson = "{\"newGame\":false,\"curCharNum\":0,\"floorItemNames\":[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"],\"floorItemTypes\":[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"],\"floorItemMods\":[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"],\"resumeItemNames\":[\"sword\",\"scroll\",\"torch\",\"\",\"\",\"\",\"\",\"\",\"\"],\"resumeItemTypes\":[\"weapon\",\"common\",\"common\",\"\",\"\",\"\",\"\",\"\",\"\"],\"resumeItemMods\":[\"harsh\",\"nothing\",\"2-1\",\"\",\"\",\"\",\"\",\"\",\"\"],\"resumeLevel\":1,\"resumeSub\":1,\"resumeAcc\":2,\"resumeSpd\":2,\"resumeDmg\":1,\"resumeDef\":2,\"floorAcc\":0,\"floorSpd\":0,\"floorDmg\":0,\"floorDef\":0,\"potionAcc\":0,\"potionSpd\":0,\"potionDmg\":0,\"potionDef\":0,\"playerStamina\":3,\"enemyStamina\":1,\"diceNumbers\":[2,1,1,1,6,2],\"diceTypes\":[\"green\",\"blue\",\"blue\",\"blue\",\"red\",\"white\"],\"dicePlayerOrEnemy\":[\"none\",\"none\",\"none\",\"none\",\"none\",\"none\"],\"diceAttachedToStat\":[\"\",\"\",\"\",\"\",\"\",\"\"],\"diceRerolled\":[false,false,false,false,false,false],\"playerWounds\":[],\"enemyWounds\":[],\"playerBleedsOutNextRound\":false,\"enemyBleedsOutNextRound\":false,\"enemyNum\":6,\"usedMace\":false,\"usedAnkh\":false,\"usedHelm\":false,\"usedBoots\":false,\"isFurious\":false,\"isDodgy\":false,\"isHasty\":false,\"isBloodthirsty\":false,\"isCourageous\":false,\"expendedStamina\":0,\"numItemsDroppedForTrade\":0,\"discardableDieCounter\":0,\"enemyIsDead\":false,\"enemyAcc\":1,\"enemySpd\":1,\"enemyDmg\":1,\"enemyDef\":1}";
    public static GameData game;
    public static PersistentData persistent;
    private static bool usingTutorialSave;

    public static void LoadTutorial() { 
        usingTutorialSave = true;
        if (File.Exists(TutorialGamePath)) {
            game = JsonUtility.FromJson<GameData>(File.ReadAllText(TutorialGamePath));
        }
        else {
            game = JsonUtility.FromJson<GameData>(tutorialJson);
            File.WriteAllText(TutorialGamePath, JsonUtility.ToJson(game));
        }
        game.Normalize();
    }

    public static void SaveGame() { 
        Scripts s = Object.FindFirstObjectByType<Scripts>();
        SyncCombatStateForSave(s);
        File.WriteAllText(usingTutorialSave ? TutorialGamePath : GamePath, JsonUtility.ToJson(game));
    }

    private static void SyncCombatStateForSave(Scripts s) {
        if (game == null || s == null) { return; }

        if (s.itemManager != null) {
            s.itemManager.SyncCharmStateToSave();
            s.itemManager.SyncLuckyDiceRoundStatsToSave();
        }

        if (s.enemy == null) { return; }

        int investedEnemyStamina = s.statSummoner != null
            ? s.statSummoner.addedEnemyStamina.Values.Sum()
            : 0;
        game.enemyStamina = s.enemy.stamina + investedEnemyStamina;
        game.enemyTargetIndex = s.enemy.targetIndex;
    }

    public static void LoadGame() { 
        usingTutorialSave = false;
        if (File.Exists(GamePath)) { 
            game = JsonUtility.FromJson<GameData>(File.ReadAllText(GamePath));
            if (game == null) { 
                // little extra redundancy incase something went really wrong   
                Debug.LogError("something went really wrong!");
                game = new GameData();
                SaveGame();
            }
            game.Normalize();
            // Debug.Log($"just saved the game! newgame is {game.newGame}");
        }
        else { 
            Debug.Log("no save found, so just created one!");
            game = new GameData();
            SaveGame();
        }
    }

    public static void SavePersistent() { 
        persistent ??= new PersistentData();
        persistent.Normalize();
        File.WriteAllText(PersistentPath, JsonUtility.ToJson(persistent));
    }

    public static void LoadPersistent() { 
        if (File.Exists(PersistentPath)) { 
            persistent = JsonUtility.FromJson<PersistentData>(File.ReadAllText(PersistentPath)); 
            if (persistent == null) { 
                persistent = new PersistentData();
                SavePersistent();
            }
            else {
                string migratedDifficulty = DifficultyHelper.Migrate(persistent.gameDifficulty, persistent.difficultyVersion);
                persistent.gameDifficulty = migratedDifficulty;
                persistent.difficultyVersion = DifficultyHelper.CurrentDifficultyVersion;
                persistent.Normalize();
                SavePersistent();
            }
        }
        else { 
            Debug.Log("no statistics found, so just created one!");
            persistent = new PersistentData();
            persistent.Normalize();
            SavePersistent();
        }
    }
}
