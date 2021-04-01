using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    string persistentPath = "persistentSave.txt";
    string gamePath = "gameSave.txt";
    private float transitionMultiplier;
    private SoundManager soundManager;
    private Arrow arrow;
    private Scripts scripts;
    void Start() {
        scripts = FindObjectOfType<Scripts>();
        transitionMultiplier = FindObjectOfType<BackToMenu>().transitionMultiplier;
        soundManager = FindObjectOfType<SoundManager>();
        arrow = FindObjectOfType<Arrow>();
        arrow.MoveToButtonPos(1);
    }

    public void OnMouseEnter() {
        arrow.MoveToButtonPos(Array.IndexOf(arrow.menuButtons, gameObject));
        // when the cursor moves over a menu button, make the arrow go there
    }

    public void OnMouseDown() {
        // when clicked, call necessary function
        ButtonPress(name);
    }

    /// <summary>
    /// Handle when one of the menu buttons is pressed.
    /// </summary>
    /// <param name="buttonName"></param>
    public void ButtonPress(string buttonName) {
        soundManager.PlayClip("click0");
        // play sound clip
        switch (buttonName) {
            case "Continue":
                GameData gameData = LoadGameData();
                if (gameData.enemyNum == 0 || gameData.enemyNum == 1 || gameData.enemyNum == 2) { 
                    if (scripts.music.audioSource.clip.name != "LaBossa") { scripts.music.FadeVolume("LaBossa"); }
                }
                else if (gameData.resumeSub == 4) { 
                    if (scripts.music.audioSource.clip.name != "Smoke") { scripts.music.FadeVolume("Smoke"); }
                }
                else { 
                    if (scripts.music.audioSource.clip.name != "Through") { scripts.music.FadeVolume("Through"); }
                }
                // fade music in based on what the player is currently on, but only if the same music is not already playing
                Initiate.Fade("Game", Color.black, transitionMultiplier);
                break;
            case "New Game":
                if (scripts.music.audioSource.clip.name != "Through") { scripts.music.FadeVolume("Through"); }
                File.WriteAllText("gameSave.txt", JsonUtility.ToJson(new GameData()));
                PersistentData persistentData = LoadPersistentData();
                persistentData.gamesPlayed++;
                Initiate.Fade("Game", Color.black, transitionMultiplier);
                break;
            default:
                Initiate.Fade(buttonName, Color.black, transitionMultiplier);
                break;
            // go to the associated level
        }
    }

    public PersistentData LoadPersistentData() { 
        if (File.Exists(persistentPath)) { return JsonUtility.FromJson<PersistentData>(File.ReadAllText(persistentPath)); }
        else { 
            Debug.LogError($"no statistics found, so just created one!");
            File.WriteAllText(persistentPath, JsonUtility.ToJson(new PersistentData()));
            return JsonUtility.FromJson<PersistentData>(File.ReadAllText(persistentPath));
        }
    }

    public GameData LoadGameData() { 
        if (File.Exists(gamePath)) { return JsonUtility.FromJson<GameData>(File.ReadAllText(gamePath)); }
        else { 
            Debug.Log($"no savefile found, so just created one!");
            File.WriteAllText(gamePath, JsonUtility.ToJson(new GameData()));
            return JsonUtility.FromJson<GameData>(File.ReadAllText(gamePath));
        }
    }
}
