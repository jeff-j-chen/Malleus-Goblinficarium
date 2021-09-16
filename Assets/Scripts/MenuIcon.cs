using System.Collections;
using UnityEngine;
public class MenuIcon : MonoBehaviour {
    [SerializeField] private GameObject debug;
    [SerializeField] private GameObject hints;
    [SerializeField] private GameObject sound;
    [SerializeField] private GameObject music;
    [SerializeField] private GameObject buttons;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource sfxPlayer;
    public Color gray;
    private SpriteRenderer debugSR;
    private SpriteRenderer soundsSR;
    private SpriteRenderer hintsSR;
    private SpriteRenderer musicSR;
    private SpriteRenderer buttonsSR;
    private Scripts scripts;
    
    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        debugSR = debug.GetComponent<SpriteRenderer>();
        hintsSR = hints.GetComponent<SpriteRenderer>();
        soundsSR = sound.GetComponent<SpriteRenderer>();
        musicSR = music.GetComponent<SpriteRenderer>();
        buttonsSR = buttons.GetComponent<SpriteRenderer>();
        ColorUtility.TryParseHtmlString("#404040", out gray);
        // get the necessary components and colors
        // assign var gray to the html string parsed
        PlayerPrefSetter(scripts.DEBUG_KEY, debugSR, false);
        PlayerPrefSetter(scripts.HINTS_KEY, hintsSR, false);
        PlayerPrefSetter(scripts.SOUNDS_KEY, soundsSR, false);
        PlayerPrefSetter(scripts.MUSIC_KEY, musicSR, false);
        PlayerPrefSetter(scripts.BUTTONS_KEY, buttonsSR, false);
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            PlayerPrefs.SetString(scripts.BUTTONS_KEY, "on");
        }
        // set the default preferences
        StartCoroutine(FindMusicLate());
    }

    private IEnumerator FindMusicLate() { 
        yield return scripts.delays[0.25f];
        musicPlayer = FindObjectOfType<Music>().GetComponent<AudioSource>();
    }

    private void Update() {
        if (Input.GetKeyDown("d")) { PlayerPrefSetter(scripts.DEBUG_KEY, debugSR); }
        else if (Input.GetKeyDown("h")) { PlayerPrefSetter(scripts.HINTS_KEY, hintsSR); }
        else if (Input.GetKeyDown("s")) { PlayerPrefSetter(scripts.SOUNDS_KEY, soundsSR); }
        else if (Input.GetKeyDown("m")) { PlayerPrefSetter(scripts.MUSIC_KEY, musicSR); }
        else if (Input.GetKeyDown("b")) { PlayerPrefSetter(scripts.BUTTONS_KEY, buttonsSR); }
    }
    // toggle player preferences based on the keys pressed

    /// <summary>
    /// Set the player preference (setting) for the given key.
    /// </summary>
    public void PlayerPrefSetter(string key, SpriteRenderer spriteRenderer, bool isSwap = true) {
        if (PlayerPrefs.GetString(key) == "on") {
            // on
            if (isSwap) {
                PlayerPrefs.SetString(key, "off");
                TurnOff(key, spriteRenderer);
                // so turn off
            }
            else { TurnOn(key, spriteRenderer); }
            // set the correct sprite
        }
        else {
            // off
            if (isSwap) {
                PlayerPrefs.SetString(key, "on");
                TurnOn(key, spriteRenderer);
                // so turn on
            }
            else { TurnOff(key, spriteRenderer); }
            // set the correct sprite
        }
    }

    /// <summary>
    /// Turn on the player preference with the associated key.
    /// </summary>
    private void TurnOn(string key, SpriteRenderer spriteRenderer) {
        spriteRenderer.color = Color.white;
        // make the icon white
        if (key == scripts.SOUNDS_KEY) { sfxPlayer.volume = 1f; }
        else if (key == scripts.MUSIC_KEY) { musicPlayer.volume = 0.5f; }
    }

    /// <summary>
    /// Turn on the player preference with the associated key.
    /// </summary>
    private void TurnOff(string key, SpriteRenderer spriteRenderer) {
        spriteRenderer.color = gray;
        // make the icon gray
        if (key == scripts.SOUNDS_KEY) { sfxPlayer.volume = 0f; }
        else if (key == scripts.MUSIC_KEY) { musicPlayer.volume = 0f; }
    }
}
