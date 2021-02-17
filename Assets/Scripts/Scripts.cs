using System;
using System.Collections.Generic;
using UnityEngine;

public class Scripts : MonoBehaviour {
    public Data data;
    public Dice dice;
    public Arrow arrow;
    public Enemy enemy;
    public Music music;
    public Colors colors;
    public Player player;
    public MenuIcon menuIcon;
    public BackToMenu backToMenu;
    public MenuButton menuButton;
    public TurnManager turnManager;
    public ItemManager itemManager;
    public DiceSummoner diceSummoner;
    public SoundManager soundManager;
    public LevelManager levelManager;
    public StatSummoner statSummoner;
    public TombstoneData tombstoneData;
    public CharacterSelector characterSelector;
    public HighlightCalculator highlightCalculator;
    private readonly float[] delayArr = { 0.0001f, 0.001f, 0.005f, 0.01f, 0.0125f, 0.02f, 0.025f, 0.033f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.4f, 0.45f, 0.5f, 0.55f, 0.6f, 0.65f, 0.75f, 0.8f, 1f, 1.5f, 1.55f, 2f, 2.5f, 3f };
    public Dictionary<float, WaitForSeconds> delays = new Dictionary<float, WaitForSeconds>();

    private void Start() {
        SaveSystem.SaveData(this, false);
        data = SaveSystem.LoadData();
        dice = FindObjectOfType<Dice>();
        arrow = FindObjectOfType<Arrow>();
        enemy = FindObjectOfType<Enemy>();
        music = FindObjectOfType<Music>();
        colors = FindObjectOfType<Colors>();
        player = FindObjectOfType<Player>();
        menuIcon = FindObjectOfType<MenuIcon>();
        backToMenu = FindObjectOfType<BackToMenu>();
        menuButton = FindObjectOfType<MenuButton>();
        turnManager = FindObjectOfType<TurnManager>();
        itemManager = FindObjectOfType<ItemManager>();
        diceSummoner = FindObjectOfType<DiceSummoner>();
        soundManager = FindObjectOfType<SoundManager>();
        levelManager = FindObjectOfType<LevelManager>();
        statSummoner = FindObjectOfType<StatSummoner>();
        tombstoneData = FindObjectOfType<TombstoneData>();
        characterSelector = FindObjectOfType<CharacterSelector>();
        highlightCalculator = FindObjectOfType<HighlightCalculator>();
        foreach (float delay in delayArr) {
            delays.Add(delay, new WaitForSeconds(delay));
        }
        print($"data loaded, easyMode: {data.easyMode}");
    }
}