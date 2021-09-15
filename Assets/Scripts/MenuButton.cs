using System;
using UnityEngine;
public class MenuButton : MonoBehaviour {
    private float transitionMultiplier;
    private SoundManager soundManager;
    private Arrow arrow;
    private Scripts scripts;

    private void Start() {
        Save.LoadGame();
        Save.LoadPersistent();
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
    public void ButtonPress(string buttonName) {
        soundManager.PlayClip("click0");
        // play sound clip
        switch (buttonName) {
            case "Continu":
                // continu because the button is actuall continu + e, for some reason contiue has the e slightly off
                if (Save.game.enemyNum is 0 or 1 or 2) { 
                    if (scripts.music.audioSource.clip.name != "LaBossa") { scripts.music.FadeVolume("LaBossa"); }
                }
                else if (Save.game.resumeSub == 4) { 
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
                Save.game = new GameData();
                Save.persistent.gamesPlayed++;
                Save.SaveGame();
                Save.SavePersistent();
                Initiate.Fade("Game", Color.black, transitionMultiplier);
                break;
            case "Tutorial": 
                if (scripts.music.audioSource.clip.name != "Through") { scripts.music.FadeVolume("Through"); }
                Initiate.Fade("Tutorial", Color.black, transitionMultiplier);
                break;
            default:
                Initiate.Fade(buttonName, Color.black, transitionMultiplier);
                break;
            // go to the associated level
        }
    }

}
