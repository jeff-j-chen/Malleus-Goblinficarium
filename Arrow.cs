using UnityEngine;
public class Arrow : MonoBehaviou {
    [SerializeField] public GameObject[] menuButtons;
    // serialized field of menubuttons 
    private float xOffset = -3.2f;
    private float yOffset = -0.04f;
    // how far off the arrow should be on the x and y axes
    private int currentIndex = 1;
    // the current index of the menu item the arrow is selecting
    private Scripts scripts;
    // necessary for all files

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        // find scripts
        MoveToButtonPos(currentIndex);
        // immediately move to the correct button position
        
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
            if (currentIndex - 1 >= 0) {
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
            scripts.soundManager.PlayClip("click");
            // play the click sound
        }
    }

    /// <summary>
    /// Move the selection arrow to a menu item.
    /// </summary>
    /// <param name="index">The index of the menu item to move the selection arrow to.</param>
    public void MoveToButtonPos(int index) {
        // function used to move the arrow to the desired button position
        currentIndex = index;
        transform.position = new Vector2(menuButtons[index].transform.position.x + xOffset, menuButtons[index].transform.position.y + yOffset);
        // move the arrow to the menu icon at the index, with offset
        scripts.soundManager.PlayClip("click");
        // play sound clip
    }

    
}