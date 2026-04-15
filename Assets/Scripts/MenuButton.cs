using System;
using TMPro;
using UnityEngine;

public class MenuButton : MonoBehaviour {
    private float transitionMultiplier;
    private Scripts s;
    private TextMeshProUGUI text;

    private void Start() {
        Save.LoadGame();
        Save.LoadPersistent();
        s = FindObjectOfType<Scripts>();
        text = GetComponent<TextMeshProUGUI>();
        transitionMultiplier = FindObjectOfType<BackToMenu>().transitionMultiplier;
        if (s.arrow != null) {
            s.arrow = FindObjectOfType<Arrow>();
            s.arrow.MoveToButtonPos(1);
        }
    }

    public void OnMouseEnter() {
        if (s.arrow != null) { s.arrow.MoveToButtonPos(Array.IndexOf(s.arrow.menuButtons, gameObject)); }
        // when the cursor moves over a menu button, make the arrow go there
        if (PlayerPrefs.GetString(s.BUTTONS_KEY) == "on") { text.color = Colors.hovered; }
    }

    public void OnMouseDown() {
        // when clicked, call necessary function
        ButtonPress(name);
    }

    /// <summary>
    /// Handle when one of the menu buttons is pressed.
    /// </summary>
    public void ButtonPress(string buttonName) {
        s.soundManager.PlayClip("click0");
        if (PlayerPrefs.GetString(s.BUTTONS_KEY) == "on") { text.color = Colors.clicked; }
        // play sound clip
        switch (buttonName) {

            case "Continue":
                // continu because the button is actually continu + e, for some reason continue has the e slightly off
                if (Save.game.enemyNum is 0 or 1 or 2) {
                    if (s.music.audioSource.clip.name != "LaBossa") { s.music.FadeVolume("LaBossa"); }
                }
                else if (Save.game.resumeSub == 4) {
                    if (s.music.audioSource.clip.name != "Smoke") { s.music.FadeVolume("Smoke"); }
                }
                else {
                    if (s.music.audioSource.clip.name != "Through") { s.music.FadeVolume("Through"); }
                }
                // fade music in based on what the player is currently on, but only if the same music is not already playing
                Initiate.Fade("Game", Color.black, transitionMultiplier);
                break;
            case "New Game":
                if (s.music.audioSource.clip.name != "Through") { s.music.FadeVolume("Through"); }
                Save.game = new GameData();
                Save.persistent.gamesPlayed++;
                Save.SaveGame();
                Save.SavePersistent();
                Initiate.Fade("Game", Color.black, transitionMultiplier);
                break;
            case "Tutorial":
                if (s.music.audioSource.clip.name != "Through") { s.music.FadeVolume("Through"); }
                Initiate.Fade("Tutorial", Color.black, transitionMultiplier);
                break;
            default:
                Initiate.Fade(buttonName, Color.black, transitionMultiplier);
                break;
            // go to the associated level
        }
    }

    private void OnMouseExit() {
        if (PlayerPrefs.GetString(s.BUTTONS_KEY) == "on") { text.color = Color.white; }
    }

    private void OnMouseUp() {
        if (PlayerPrefs.GetString(s.BUTTONS_KEY) == "on") { text.color = Color.white; }
    }

}
