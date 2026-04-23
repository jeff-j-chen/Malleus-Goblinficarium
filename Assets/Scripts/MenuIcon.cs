using System.Collections;
using UnityEngine;
using TMPro; 

public class MenuIcon : MonoBehaviour {
    [SerializeField] public GameObject resolution;
    [SerializeField] public GameObject debug;
    [SerializeField] public GameObject hints;
    [SerializeField] public GameObject sound;
    [SerializeField] public GameObject music;
    [SerializeField] public GameObject buttons;
    [SerializeField] public GameObject resolutionText;
    [SerializeField] public GameObject debugText;
    [SerializeField] public GameObject soundsText;
    [SerializeField] public GameObject hintsText;
    [SerializeField] public GameObject musicText;
    [SerializeField] public GameObject buttonsText;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource sfxPlayer;
    private SpriteRenderer resolutionSR;
    private SpriteRenderer debugSR;
    private SpriteRenderer soundsSR;
    private SpriteRenderer hintsSR;
    private SpriteRenderer musicSR;
    private SpriteRenderer buttonsSR;
    private Scripts s;
    public GameObject[] menuIconOrdering;
    public GameObject[] menuIconTextOrdering;
    
    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        resolutionSR = resolution.GetComponent<SpriteRenderer>();
        debugSR = debug.GetComponent<SpriteRenderer>();
        hintsSR = hints.GetComponent<SpriteRenderer>();
        soundsSR = sound.GetComponent<SpriteRenderer>();
        musicSR = music.GetComponent<SpriteRenderer>();
        buttonsSR = buttons.GetComponent<SpriteRenderer>();
        // get the necessary components and colors
        if(!PlayerPrefs.HasKey(s.RESOLUTION_KEY)) PlayerPrefs.SetInt(s.RESOLUTION_KEY, 0);
        if(!PlayerPrefs.HasKey(s.DEBUG_KEY)) PlayerPrefs.SetString(s.DEBUG_KEY, "on");
        if(!PlayerPrefs.HasKey(s.HINTS_KEY)) PlayerPrefs.SetString(s.HINTS_KEY, "on");
        if(!PlayerPrefs.HasKey(s.SOUNDS_KEY)) PlayerPrefs.SetString(s.SOUNDS_KEY, "on");
        if(!PlayerPrefs.HasKey(s.MUSIC_KEY)) PlayerPrefs.SetString(s.MUSIC_KEY, "on");
        if(!PlayerPrefs.HasKey(s.BUTTONS_KEY)) PlayerPrefs.SetString(s.BUTTONS_KEY, "off");
        // set defaults for settings if they dont already exist
        PlayerPrefSetter(s.DEBUG_KEY, debugSR, false);
        PlayerPrefSetter(s.HINTS_KEY, hintsSR, false);
        PlayerPrefSetter(s.SOUNDS_KEY, soundsSR, false);
        PlayerPrefSetter(s.MUSIC_KEY, musicSR, false);
        PlayerPrefSetter(s.BUTTONS_KEY, buttonsSR, false);
        // then set the icons
        if (SystemInfo.deviceType == DeviceType.Handheld || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            PlayerPrefs.SetString(s.BUTTONS_KEY, "on");
            PlayerPrefSetter(s.BUTTONS_KEY, buttonsSR, false);
            // mobile devices always have buttons
        }
        RefreshResolutionIconState();
        // set the default preferences
        StartCoroutine(FindMusicLate());
    }

    private IEnumerator FindMusicLate() { 
        yield return s.delays[0.25f];
        musicPlayer = FindFirstObjectByType<Music>().GetComponent<AudioSource>();
    }

    private void Update() {
        if (Input.GetKeyDown("r")) { CycleResolution(); }
        else if (Input.GetKeyDown("d")) { PlayerPrefSetter(s.DEBUG_KEY, debugSR); }
        else if (Input.GetKeyDown("h")) { PlayerPrefSetter(s.HINTS_KEY, hintsSR); }
        else if (Input.GetKeyDown("s")) { PlayerPrefSetter(s.SOUNDS_KEY, soundsSR); }
        else if (Input.GetKeyDown("m")) { PlayerPrefSetter(s.MUSIC_KEY, musicSR); }
        else if (Input.GetKeyDown("b")) { PlayerPrefSetter(s.BUTTONS_KEY, buttonsSR); }
    }
    // toggle player preferences based on the keys pressed

    /// <summary>
    /// Set the player preference (setting) for the given key.
    /// </summary>
    public void PlayerPrefSetter(string key, SpriteRenderer spriteRenderer, bool isSwap = true) {
        if (key == s.RESOLUTION_KEY) {
            CycleResolution();
            return;
        }
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

    public void CycleResolution() {
        if (PlayerPrefs.GetString(s.BUTTONS_KEY) == "on") {
            RefreshResolutionIconState();
            return;
        }
        s.CycleResolution();
        RefreshResolutionIconState();
    }

    /// <summary>
    /// Turn on the player preference with the associated key.
    /// </summary>
    private void TurnOn(string key, SpriteRenderer spriteRenderer) {
        spriteRenderer.color = Color.white;
        // make the icon white
        if (key == s.SOUNDS_KEY) { sfxPlayer.volume = 1f; }
        else if (key == s.MUSIC_KEY) { musicPlayer.volume = 0.5f; }
        else if (key == s.BUTTONS_KEY) { 
            s.mobileResizer.FlipMenuIconMode();
            s.arrow = FindFirstObjectByType<Arrow>();
            RefreshResolutionIconState();
        }
    }

    /// <summary>
    /// Turn on the player preference with the associated key.
    /// </summary>
    private void TurnOff(string key, SpriteRenderer spriteRenderer) {
        spriteRenderer.color = Colors.disabled;
        // make the icon gray
        if (key == s.SOUNDS_KEY) { sfxPlayer.volume = 0f; }
        else if (key == s.MUSIC_KEY) { musicPlayer.volume = 0f; }
        else if (key == s.BUTTONS_KEY) { 
            s.mobileResizer.FlipMenuIconMode();
            s.arrow = FindFirstObjectByType<Arrow>();
            RefreshResolutionIconState();
        }
    }

    private void RefreshResolutionIconState() {
        bool showResolution = PlayerPrefs.GetString(s.BUTTONS_KEY) != "on";
        resolution.SetActive(showResolution);
        resolutionText.SetActive(showResolution);
        if (showResolution) {
            resolutionSR.color = Color.white;
        }
    }
}
