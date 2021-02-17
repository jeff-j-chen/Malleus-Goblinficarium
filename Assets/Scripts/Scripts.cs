using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Scripts : MonoBehaviour {
    string path = "save.txt";
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
        data = LoadData();
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
    public void NormalSaveData() {
        if (characterSelector != null) {
            // curCharNum = oldData.curCharNum;
            data.newCharNum = characterSelector.selectionNum;
            data.easyMode = characterSelector.easy;
        }
        if (player != null) { 
            data.curCharNum = player.charNum;
            data.newCharNum = player.charNum;
            // easyMode = oldData.easyMode;
            data.resumeLevel = levelManager.level;
            data.resumeSub = levelManager.sub;
            data.resumeStamina = player.stamina;
            for (int i = 0; i < player.inventory.Count; i++) {
                // for every item after the weapon
                Item item = player.inventory[i].GetComponent<Item>();
                // create a temporary store for the item
                if (item.itemName == "torch") {} // don't do anything for the torch, it just disappears
                else if (item.itemType == "weapon") {
                    data.tsWeaponAcc = data.resumeAcc = item.weaponStats["green"];
                    data.tsWeaponSpd = data.resumeSpd = item.weaponStats["blue"];
                    data.tsWeaponDmg = data.resumeDmg = item.weaponStats["red"];
                    data.tsWeaponDef = data.resumeDef = item.weaponStats["white"];
                    data.tsItemNames[0] = data.resumeItemNames[0] = player.inventory[0].GetComponent<Item>().itemName.Split(' ')[1];
                    data.tsItemTypes[0] = data.resumeItemTypes[0] = "weapon";
                    data.tsItemMods[0]  = data.resumeItemMods[0]  = player.inventory[0].GetComponent<Item>().modifier.Split(' ')[0];
                }
                else {
                    data.tsItemNames[i] = data.resumeItemNames[i] = player.inventory[i].GetComponent<Item>().itemName;
                    data.tsItemTypes[i] = data.resumeItemTypes[i] = player.inventory[i].GetComponent<Item>().itemType;
                    data.tsItemMods[i]  = data.resumeItemMods[i]  = player.inventory[i].GetComponent<Item>().modifier;
                }
            }
            if (levelManager.sub == 4) { 
                Debug.Log("saving merchant wares!");
                bool arrowFound = false;
                for (int i = 0; i < 9; i++) {
                    if (!arrowFound) { 
                        Item item = itemManager.floorItems[i].GetComponent<Item>();
                        Debug.Log($"saved {itemManager.floorItems[i].GetComponent<Item>().itemName}");
                        data.merchantItemNames[i] = itemManager.floorItems[i].GetComponent<Item>().itemName;
                        data.merchantItemTypes[i] = itemManager.floorItems[i].GetComponent<Item>().itemType;
                        data.merchantItemMods[i] = itemManager.floorItems[i].GetComponent<Item>().modifier;
                        if (data.merchantItemNames[i] == "arrow") { arrowFound = true;Debug.Log("found an arrow!"); }
                    }
                    else {
                        Debug.Log($"clearing {i}");
                        data.merchantItemNames[i] = "";
                        data.merchantItemTypes[i] = "";
                        data.merchantItemMods[i] = "";
                    }
                }
            }
        }
        File.WriteAllText(path, JsonUtility.ToJson(data));
    }

    public void ClearMerchantWaresAndSave() { 
        data.merchantItemNames = new string[9];
        data.merchantItemTypes = new string[9];
        data.merchantItemMods  = new string[9];
        NormalSaveData();
    }

    public void ResetTSLevelAndSave() {
        data.tsLevel = -1;
        data.tsSub = -1;
        NormalSaveData();
    }

    public void SetTSAndSave() { 
        if (levelManager.level == 4 && levelManager.sub == 1) { 
            data.tsLevel = 3;
            data.tsSub = 3;
            // game will crash if we go to 4-1*, easy solution here
        }
        else { 
            if (levelManager.sub == 4) { data.tsSub = 3; }
            else { data.tsSub = levelManager.sub; }
            data.tsLevel = levelManager.level;
        }
        data.resumeAcc = data.resumeSpd = data.resumeDmg = data.resumeDef = 0;
        data.resumeItemNames = new string[9];
        data.resumeItemTypes = new string[9];
        data.resumeItemMods  = new string[9];
        NormalSaveData();
    }

    public Data LoadData() { 
        if (File.Exists(path)) { return JsonUtility.FromJson<Data>(File.ReadAllText(path)); }
        else { 
            Debug.LogError($"no savefile found, so just created one!");
            File.WriteAllText(path, JsonUtility.ToJson(new Data()));
            return JsonUtility.FromJson<Data>(File.ReadAllText(path));
        }
    }
}