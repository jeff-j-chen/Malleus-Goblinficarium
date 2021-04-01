using UnityEngine;
using System.Collections; 

public class Arrow : MonoBehaviour {
    [SerializeField] public GameObject[] menuButtons;
    // serialized field of menubuttons 
    private Vector2[] buttonPositions = new Vector2[] { 
        new Vector2(0.75f, 1.5f),
        new Vector2(0.75f, 0.5f),
        new Vector2(0.75f, -0.5f),
        new Vector2(0.75f, -1.5f),
        new Vector2(0.75f, -2.5f),
        new Vector2(0.75f, -3.5f),
    };

    private float xOffset = -3.2f;
    private float yOffset = -0.04f;
    // how far off the arrow should be on the x and y axes
    private int currentIndex = 1;
    // the current index of the menu item the arrow is selecting
    private Scripts scripts;
    // necessary for all files
    private bool preventPlayingFX = true;

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        // find scripts
        Save.LoadGame();
        if (Save.game.newGame == true) { 
            menuButtons[0].SetActive(false); 
            for (int i = 1; i < menuButtons.Length; i++) { 
                menuButtons[i].transform.position = buttonPositions[i-1];
            }
        }
        else { 
            menuButtons[0].SetActive(true); 
            for (int i = 0; i < menuButtons.Length; i++) { 
                menuButtons[i].transform.position = buttonPositions[i];
            }
        }
        // hide/show the continue button if there is a game or not
        MoveToButtonPos(currentIndex);
        // immediately move to the correct button position
        StartCoroutine(allowFX());
    }

    private IEnumerator allowFX() { 
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
            if (currentIndex - 1 >= 0 && Save.game.newGame == false || currentIndex - 1 >= 1 && Save.game.newGame == true) {
                // if can move the selector (arrow) up
                currentIndex--;
                MoveToButtonPos(currentIndex);
                // decrement current index and select the menu item
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            // if the player presses return or the numpad enter
            scripts.menuButton.ButtonPress(menuButtons[currentIndex].name);
            // call the function for buttonpress
        }
    }

    /// <summary>
    /// Move the selection arrow to a menu item to a given index.
    /// </summary>
    public void MoveToButtonPos(int index) {
        // function used to move the arrow to the desired button position
        if (!(index == 0 && Save.game.newGame == true)) {
            // as long as we are not trying to select continue when newgame is true (previous Save wiped)
            currentIndex = index;
            transform.position = new Vector2(menuButtons[index].transform.position.x + xOffset, menuButtons[index].transform.position.y + yOffset);
            // move the arrow to the menu icon at the index, with offset
            if (!preventPlayingFX) { scripts.soundManager.PlayClip("click0"); }
            // play sound clip
        }
    }

    
}