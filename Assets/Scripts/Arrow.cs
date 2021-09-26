using System.Collections;
using UnityEngine;
public class Arrow : MonoBehaviour {
    [SerializeField] public GameObject[] menuButtons;
    public readonly float xOffset = -4f;
    public readonly float yOffset = -0.04f;
    // how far off the arrow should be on the x and y axes
    private int currentIndex = 1;
    // the current index of the menu item the arrow is selecting
    private Scripts scripts;
    // necessary for all files
    private bool preventPlayingFX = true;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        // find scripts
        // hide/show the continue button if there is a game or not
        MoveToButtonPos(currentIndex);
        // immediately move to the correct button position
        StartCoroutine(AllowFx());
        transform.localPosition = PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on" ? new Vector2(1000f, 0) : new Vector2(menuButtons[0].transform.position.x + xOffset, menuButtons[0].transform.position.y + yOffset);
    }

    private IEnumerator AllowFx() { 
        yield return new WaitForSeconds(0.1f);
        preventPlayingFX = false;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            // if player pressed down
            if (currentIndex + 1 < menuButtons.Length) {
                // if can move the selector (arrow) down
                currentIndex++;
                MoveToButtonPos(currentIndex);
                // increment current index and select the menu item
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            // if player pressed up
            if (currentIndex - 1 >= 0 && Save.game.newGame == false || currentIndex - 1 >= 1 && Save.game.newGame) {
                // if can move the selector (arrow) up
                currentIndex--;
                MoveToButtonPos(currentIndex);
                // decrement current index and select the menu item
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            // if the player presses return or the numpad enter
            scripts.menuButton.ButtonPress(menuButtons[currentIndex].name);
            // call the function for button press
        }
    }

    /// <summary>
    /// Move the selection arrow to a menu item to a given index.
    /// </summary>
    public void MoveToButtonPos(int index) {
        scripts = FindObjectOfType<Scripts>();
        // function used to move the arrow to the desired button position
        if (!(index == 0 && Save.game.newGame)) {
            // as long as we are not trying to select continue when new game is true (previous Save wiped)
            currentIndex = index;
            if (PlayerPrefs.GetString(scripts.BUTTONS_KEY) == "on") {
                // offset the arrow all the way off the screen 
                transform.localPosition = new Vector2(menuButtons[index].transform.position.x + 1000f, menuButtons[index].transform.position.y + yOffset);
            }
            else {
                transform.localPosition = new Vector2(menuButtons[index].transform.position.x + xOffset, menuButtons[index].transform.position.y + yOffset);
            }
            // move the arrow to the menu icon at the index, with offset
            if (!preventPlayingFX) { scripts.soundManager.PlayClip("click0"); }
            // play sound clip
        }
    }

    
}