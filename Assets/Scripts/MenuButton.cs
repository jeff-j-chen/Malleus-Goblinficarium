using System;
using TMPro;
using UnityEngine;
using TMPro;
public class MenuButton : MonoBehaviour {
    private float transitionMultiplier;
    private Scripts scripts;
    private TextMeshProUGUI text;

    private void Start() {
        Save.LoadGame();
        Save.LoadPersistent();
        scripts = FindObjectOfType<Scripts>();
        text = GetComponent<TextMeshProUGUI>();
        transitionMultiplier = FindObjectOfType<BackToMenu>().transitionMultiplier;
        if (scripts.arrow != null) {
            scripts.arrow = FindObjectOfType<Arrow>();
            scripts.arrow.MoveToButtonPos(1);
        }
    }

    public void OnMouseEnter() {
        if (scripts.arrow != null) { scripts.arrow.MoveToButtonPos(Array.IndexOf(scripts.arrow.menuButtons, gameObject)); }
        // when the cursor moves over a menu button, make the arrow go there
        if (PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on") { text.color = Colors.hovered; }
    }

    public void OnMouseDown() {
        // when clicked, call necessary function
        ButtonPress(name);
    }

    /// <summary>
    /// Handle when one of the menu buttons is pressed.
    /// </summary>
    public void ButtonPress(string buttonName) {
        scripts.soundManager.PlayClip("click0");
        if (PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on") { text.color = Colors.clicked; }
        // play sound clip
        switch (buttonName) {

            case "Continue":
                // continu because the button is actually continu + e, for some reason continue has the e slightly off
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

    private void OnMouseExit() {
        if (PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on") { text.color = Color.white; }
    }

    private void OnMouseUp() {
        if (PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on") { text.color = Color.white; }
    }

}
