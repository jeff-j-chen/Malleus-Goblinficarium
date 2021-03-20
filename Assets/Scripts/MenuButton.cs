using System;
using System.IO;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    string persistentPath = "persistentSave.txt";
    private float transitionMultiplier;
    private SoundManager soundManager;
    private Arrow arrow;
    void Start() {
        transitionMultiplier = FindObjectOfType<BackToMenu>().transitionMultiplier;
        soundManager = FindObjectOfType<SoundManager>();
        arrow = FindObjectOfType<Arrow>();
        
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
        soundManager.PlayClip("click");
        // play sound clip
        switch (buttonName) {
            case "Continue":
                // do something special here
                Initiate.Fade("Game", Color.black, transitionMultiplier);
                break;
            case "New Game":
                GameData data = new GameData();
                File.WriteAllText("save.txt", JsonUtility.ToJson(data));
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
}
