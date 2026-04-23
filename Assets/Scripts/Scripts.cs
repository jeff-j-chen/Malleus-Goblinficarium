using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Scripts : MonoBehaviour {
    [SerializeField] public Animator terrain;
    [SerializeField] private GameObject border;
    public Dice dice;
    public Arrow arrow;
    public Enemy enemy;
    public Music music;
    public Player player;
    public MenuIcon menuIcon;
    public Tutorial tutorial;
    public BackToMenu backToMenu;
    public MenuButton menuButton;
    public Statistics statistics;
    public TurnManager turnManager;
    public ItemManager itemManager;
    public DiceSummoner diceSummoner;
    public SoundManager soundManager;
    public LevelManager levelManager;
    public StatSummoner statSummoner;
    public TombstoneData tombstoneData;
    public MobileResizer mobileResizer;
    public CharacterSelector characterSelector;
    public HighlightCalculator highlightCalculator;
    private readonly float[] delayArr = { 0.0001f, 0.001f, 0.005f, 0.01f, 0.0125f, 0.015f, 0.02f, 0.025f, 0.03f, 0.033f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 0.55f, 0.6f, 0.65f, 0.75f, 0.8f, 1f, 1.15f, 1.25f, 1.4f, 1.5f, 1.55f, 2f, 2.5f, 3f };
    // array of delays to initiate waitforseconds with, this saves on memory
    public Dictionary<float, WaitForSeconds> delays = new();
    public readonly string DEBUG_KEY = "debug";
    public readonly string HINTS_KEY = "hints";
    public readonly string SOUNDS_KEY = "sounds";
    public readonly string MUSIC_KEY = "music";
    public readonly string BUTTONS_KEY = "button";
    public readonly string RESOLUTION_KEY = "resolution";
    public bool mobileMode; // only for use in-game, don't use this for menu screen!
    private readonly Vector2Int[] availableResolutions = {
        new(800, 600),
        new(1200, 900),
        new(1600, 1200),
    };

    private void Start() {
        tutorial = FindFirstObjectByType<Tutorial>();
        if (tutorial == null) { Save.LoadGame(); }
        else { Save.LoadTutorial(); }
        Save.LoadPersistent();
        ApplySavedResolution();
        EnemyAI.InvalidateCachedPlan();
        mobileMode = PlayerPrefs.GetString(BUTTONS_KEY) == "on";
        dice = FindFirstObjectByType<Dice>();
        arrow = FindFirstObjectByType<Arrow>();
        enemy = FindFirstObjectByType<Enemy>();
        player = FindFirstObjectByType<Player>();
        menuIcon = FindFirstObjectByType<MenuIcon>();
        backToMenu = FindFirstObjectByType<BackToMenu>();
        menuButton = FindFirstObjectByType<MenuButton>();
        statistics = FindFirstObjectByType<Statistics>();
        turnManager = FindFirstObjectByType<TurnManager>();
        itemManager = FindFirstObjectByType<ItemManager>();
        diceSummoner = FindFirstObjectByType<DiceSummoner>();
        soundManager = FindFirstObjectByType<SoundManager>();
        levelManager = FindFirstObjectByType<LevelManager>();
        statSummoner = FindFirstObjectByType<StatSummoner>();
        tombstoneData = FindFirstObjectByType<TombstoneData>();
        mobileResizer = FindFirstObjectByType<MobileResizer>();
        characterSelector = FindFirstObjectByType<CharacterSelector>();
        highlightCalculator = FindFirstObjectByType<HighlightCalculator>();
        foreach (float delay in delayArr) {
            delays.Add(delay, new WaitForSeconds(delay));
        }
        if (border != null) { border.SetActive(!mobileMode); }
        StartCoroutine(SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay() { 
        // set newgame to false after a delay so that stuff can load in if its true
        yield return delays[0.25f];
        if (player != null) { Save.game.newGame = false; }
        
        if (tutorial == null) { Save.SaveGame(); }
        music = FindFirstObjectByType<Music>();
        // also get the music here, because we need it to set up the singleton pattern first
    }

    public void OnApplicationQuit() { 
        if (player != null) { 
            if (tutorial == null) { Save.SaveGame(); } 
            Save.SavePersistent(); 
        }
    }

    public void ApplySavedResolution() {
        if (!PlayerPrefs.HasKey(RESOLUTION_KEY)) {
            PlayerPrefs.SetInt(RESOLUTION_KEY, 0);
        }
        ApplyResolutionIndex(GetResolutionIndex());
    }

    public void CycleResolution() {
        int nextIndex = (GetResolutionIndex() + 1) % availableResolutions.Length;
        PlayerPrefs.SetInt(RESOLUTION_KEY, nextIndex);
        ApplyResolutionIndex(nextIndex);
        PlayerPrefs.Save();
    }

    public int GetResolutionIndex() {
        return Mathf.Clamp(PlayerPrefs.GetInt(RESOLUTION_KEY, 0), 0, availableResolutions.Length - 1);
    }

    private void ApplyResolutionIndex(int index) {
        print($"setting resolution to index {index}");
        Vector2Int resolution = availableResolutions[Mathf.Clamp(index, 0, availableResolutions.Length - 1)];
        Screen.SetResolution(resolution.x, resolution.y, false);
    }
}