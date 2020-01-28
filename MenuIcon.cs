using UnityEngine;
using TMPro;

public class MenuIcon : MonoBehaviour
{
    private const string DEBUG_KEY = "debug";
    private const string HINTS_KEY = "hints";
    private const string SOUNDS_KEY = "sounds";
    private const string MUSIC_KEY = "music";
    [SerializeField] GameObject debug;
    [SerializeField] GameObject hints;
    [SerializeField] GameObject sounds;
    [SerializeField] GameObject music;
    SpriteRenderer debugSR;
    SpriteRenderer soundsSR;
    SpriteRenderer hintsSR;
    SpriteRenderer musicSR;
    public Color gray;
    AudioSource musicPlayer;
    void Start()
    {
        musicPlayer = FindObjectOfType<Music>().GetComponent<AudioSource>();
        musicPlayer.ignoreListenerVolume = true;
        debugSR = debug.GetComponent<SpriteRenderer>();
        hintsSR = hints.GetComponent<SpriteRenderer>();
        soundsSR = sounds.GetComponent<SpriteRenderer>();
        musicSR = music.GetComponent<SpriteRenderer>();
        ColorUtility.TryParseHtmlString("#404040", out gray);
        // assign var gray to the html string parsed
        PlayerPrefSetter(DEBUG_KEY, debugSR, false);
        PlayerPrefSetter(HINTS_KEY, hintsSR, false);
        PlayerPrefSetter(SOUNDS_KEY, soundsSR, false);
        PlayerPrefSetter(MUSIC_KEY, musicSR, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("d"))
        {
            PlayerPrefSetter(DEBUG_KEY, debugSR);
        }
        else if (Input.GetKeyDown("h"))
        {
            PlayerPrefSetter(HINTS_KEY, hintsSR);
        }
        else if (Input.GetKeyDown("s"))
        {
            PlayerPrefSetter(SOUNDS_KEY, soundsSR);
        }
        else if (Input.GetKeyDown("m"))
        {
            PlayerPrefSetter(MUSIC_KEY, musicSR);
        }
    }

    public void PlayerPrefSetter(string key, SpriteRenderer spriteRenderer, bool isSwap = true)
    {
        if (PlayerPrefs.GetString(key) == "on")
        {
            if (isSwap)
            {
                PlayerPrefs.SetString(key, "off");
                TurnOff(key, spriteRenderer);
            }
            else
            {
                TurnOn(key, spriteRenderer);
            }
        }
        else
        {
            if (isSwap)
            {
                PlayerPrefs.SetString(key, "on");
                TurnOn(key, spriteRenderer);
            }
            else
            {
                TurnOff(key, spriteRenderer);
            }
        }
    }

    private void TurnOn(string key, SpriteRenderer spriteRenderer)
    {
        AssignColor(spriteRenderer, Color.white);
        switch (key)
        {
            case DEBUG_KEY:
                print("debug on");
                break;
            case HINTS_KEY:
                print("hints on");
                break;
            case SOUNDS_KEY:
                AudioListener.volume = 1f;
                break;
            case MUSIC_KEY:
                musicPlayer.volume = 1f;
                break;
        }
    }

    private void TurnOff(string key, SpriteRenderer spriteRenderer)
    {
        AssignColor(spriteRenderer, gray);
        switch (key)
        {
            case DEBUG_KEY:
                break;
            case HINTS_KEY:
                print("hints off");
                break;
            case SOUNDS_KEY:
                AudioListener.volume = 0f;
                break;
            case MUSIC_KEY:
                musicPlayer.volume = 0f;
                break;
        }
    }

    private void AssignColor(SpriteRenderer debugSR, Color color)
    {
        debugSR.color = color;
    }
}
