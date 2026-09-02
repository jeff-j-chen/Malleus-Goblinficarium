using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the title-screen selection arrow for keyboard navigation.
/// </summary>
public class Arrow : MonoBehaviour {
    [SerializeField] public GameObject[] menuButtons; // ordered menu targets the arrow can point at
    public readonly float xOffset = -4f; // horizontal offset that places the arrow left of the selected button
    public readonly float yOffset = -0.04f; // slight vertical offset so the arrow visually aligns with the button art
    private int currentIndex = 1; // current selected menu index, defaulting to new game when continue may be unavailable
    private Scripts s; // central project references and shared systems
    private bool preventPlayingFX = true; // blocks startup audio while the arrow snaps into place

    /// <summary>
    /// Initializes the arrow position and respects the button-hints accessibility toggle.
    /// </summary>
    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        MoveToButtonPos(currentIndex);
        StartCoroutine(AllowFx());
        // keep the logical selection on the first button, but visually hide the arrow if button prompts are enabled
        transform.localPosition = PlayerPrefs.GetString(s.BUTTONS_KEY) == "on" ? new Vector2(1000f, 0) : new Vector2(menuButtons[0].transform.position.x + xOffset, menuButtons[0].transform.position.y + yOffset);
    }

    /// <summary>
    /// Delays sound playback so the initial snap does not click.
    /// </summary>
    private IEnumerator AllowFx() { 
        yield return new WaitForSeconds(0.1f);
        preventPlayingFX = false;
    }

    /// <summary>
    /// Reads keyboard menu navigation and forwards the selected button on confirm.
    /// </summary>
    private void Update() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentIndex + 1 < menuButtons.Length) {
                currentIndex++;
                MoveToButtonPos(currentIndex);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            bool canMoveUp = (!Save.game.newGame && currentIndex - 1 >= 0)
                || (Save.game.newGame && currentIndex - 1 >= 1);
            // when no save exists, index 0 is the hidden continue button and must be skipped
            if (canMoveUp) {
                currentIndex--;
                MoveToButtonPos(currentIndex);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            s.menuButton.ButtonPress(menuButtons[currentIndex].name);
        }
    }

    /// <summary>
    /// Moves the selection arrow to the requested menu button.
    /// </summary>
    /// <param name="index">target button index in `menuButtons`</param>
    public void MoveToButtonPos(int index) {
        s = FindFirstObjectByType<Scripts>();
        if (!(index == 0 && Save.game.newGame)) {
            currentIndex = index;
            if (PlayerPrefs.GetString(s.BUTTONS_KEY) == "on") {
                // button icons replace the arrow in this mode, so park the arrow far off-screen
                transform.localPosition = new Vector2(menuButtons[index].transform.position.x + 1000f, menuButtons[index].transform.position.y + yOffset);
            }
            else {
                transform.localPosition = new Vector2(menuButtons[index].transform.position.x + xOffset, menuButtons[index].transform.position.y + yOffset);
            }
            if (!preventPlayingFX) { s.soundManager.PlayClip("click0"); }
        }
    }
}