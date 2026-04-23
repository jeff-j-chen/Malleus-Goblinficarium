using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
public class Enemy : MonoBehaviour {
    public const int MerchantEnemyNum = 7;
    public const int TombstoneEnemyNum = 8;
    public const int BlacksmithEnemyNum = 9;
    [SerializeField] public RuntimeAnimatorController[] controllers;
    [SerializeField] public RuntimeAnimatorController lichDeathController;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Sprite tombstoneIcon;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private GameObject iconGameobject;
    [SerializeField] public TextMeshProUGUI enemyName;
    [SerializeField] public TextMeshProUGUI woundGUIElement;
    [SerializeField] public TextMeshProUGUI staminaCounter;
    [SerializeField] public TextMeshProUGUI target;
    public List<string> woundList = new();
    public Dictionary<string, int> stats;
    private readonly string[] enemyArr = { "Cloaked", "Devil", "Lich", "Skeleton", "Kobold", "Gog", "Goblin", "Merchant", "Tombstone", "Blacksmith" };
    private readonly string[] valueArr = { "yellow6", "red6", "white6", "yellow5", "red5", "white5", "yellow4", "red4", "white4", "yellow3", "red3", "white3", "green6", "yellow2", "red2", "white2", "yellow1", "red1", "white1", "green5", "green4", "blue6", "green3", "blue5", "blue4", "green2", "blue3", "green1", "blue2", "blue1" };
    public int stamina = 1;
    public int targetIndex = 0;
    private readonly Vector2 basePosition = new(1.9f, -1.866667f);
    private readonly Vector2 iconPosition = new(6.16667f, 3.333333f);
    private readonly Dictionary<string, Vector2> deathPositions = new() {
        {"Devil", new Vector2(2.24f, -2.25f)},
        {"Lich", new Vector2(1.9f, -1.865334f)},
        {"Skeleton", new Vector2(2.1686665f, -2.117f)},
        {"Kobold", new Vector2(2.03233367f, -2.52f)},
        {"Gog", new Vector2(2.0360003f, -2.52f)},
        {"Goblin", new Vector2(2.1f, -2.52f)},
    };
    private readonly Dictionary<string, Vector2> offsetPositions = new() {
        {"Devil", new Vector2(1.9f, -1.33334f)},
        {"Tombstone", new Vector2(1.9f, -2.118f)},
    };
    private readonly Dictionary<string, int> givenStamina = new() {
        {"11", 1},
        {"12", 1},
        {"13", 2},
        {"21", 2},
        {"22", 2},
        {"23", 3},
        {"31", 3},
        {"32", 3},
        {"33", 4},
        {"41", 5},
    };
    private readonly Dictionary<string, int> givenStaminaHard = new() {
        {"11", 2},
        {"12", 2},
        {"13", 3},
        {"21", 3},
        {"22", 3},
        {"23", 4},
        {"31", 4},
        {"32", 4},
        {"33", 5},
        {"41", 6},
    };
    private Scripts s;
    public int spawnNum;
    private readonly List<Dice> availableDice = new();
    private readonly List<int> diceValuations = new();
    public readonly int lichStamina = 5;

    private int GetDisplayIndex(int enemyNum) {
        return enemyNum == BlacksmithEnemyNum ? MerchantEnemyNum : enemyNum;
    }

    private bool IsVendor(int enemyNum) {
        return enemyNum == MerchantEnemyNum || enemyNum == BlacksmithEnemyNum;
    }

    private int GetSpawnStamina(int enemyNum) {

        bool useHardStamina = DifficultyHelper.IsHard(Save.persistent.gameDifficulty)
            || DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty);
        Dictionary<string, int> staminaTable = useHardStamina ? givenStaminaHard : givenStamina;

        if (enemyArr[enemyNum] == "Devil" || enemyArr[enemyNum] == "Cloaked") {
            int devilBonus = s.levelManager.level - 4;
            if (devilBonus < 0) { devilBonus = 0; }
            return staminaTable["41"] + Mathf.FloorToInt(devilBonus * 1.5f);
        }
        if (enemyArr[enemyNum] == "Tombstone" || IsVendor(enemyNum)) { return 0; }
        if (enemyArr[enemyNum] == "Lich") { return lichStamina; }

        string staminaKey = $"{s.levelManager.level}{s.levelManager.sub}";
        if (staminaTable.ContainsKey(staminaKey)) {
            return staminaTable[staminaKey];
        }

        string fallbackKey = $"3{s.levelManager.sub}";
        int fallbackStamina = staminaTable["33"];
        if (staminaTable.ContainsKey(fallbackKey)) {
            fallbackStamina = staminaTable[fallbackKey];
        }

        int levelBonus = s.levelManager.level - 3;
        if (levelBonus < 0) { levelBonus = 0; }
        return fallbackStamina + levelBonus;
    }

    private bool HasSavedFloorItems() {
        return Save.game.floorItemNames != null
            && Save.game.floorItemNames.Any(itemName => !string.IsNullOrEmpty(itemName));
    }

    private void SyncWoundDerivedEnemyState() {
        if (woundList == null) { woundList = new List<string>(); }

        bool canUseWoundDerivedAbilities = enemyName != null
            && enemyName.text is not "Merchant" and not "Blacksmith" and not "Tombstone" and not "Lich"
            && !Save.game.enemyIsDead;

        Save.game.discardableDieCounter = canUseWoundDerivedAbilities && woundList.Contains("head")
            ? Mathf.Clamp(Save.game.discardableDieCounter, 0, 1)
            : 0;
    }

    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        s.turnManager.RefreshBlackBoxVisibility();
        // make sure to show the enemy's stats at the start
        if (s.levelManager.level == Save.persistent.tsLevel && s.levelManager.sub == Save.persistent.tsSub) {
            // on the tombstone level
            SpawnNewEnemy(TombstoneEnemyNum, Save.game.newGame);
            // spawn th tombstone
            s.itemManager.lootText.text = "loot:";
            // indicate that the player can loot
            s.tombstoneData.SpawnSavedTSItems(true);
            // spawn the Saved tombstone items
            s.turnManager.RefreshBlackBoxVisibility();
            // hide the stats (don't fight tombstones)
        }
        else if (s.levelManager.ShouldForceBlacksmithSpawn()) {
            bool spawnNewBlacksmith = Save.game.newGame || Save.game.enemyNum != BlacksmithEnemyNum;
            SpawnNewEnemy(BlacksmithEnemyNum, spawnNewBlacksmith);
            s.itemManager.lootText.text = "goods:";
            if (spawnNewBlacksmith) { s.itemManager.SpawnBlacksmithItems(true); }
            else if (HasSavedFloorItems()) { s.tombstoneData.SpawnSavedMerchantItems(true); }
            else { s.itemManager.SpawnBlacksmithItems(true); }
            s.turnManager.RefreshBlackBoxVisibility();
        }
        else if (s.levelManager.sub == 4) {
            // on a merchant level
            SpawnNewEnemy(MerchantEnemyNum, Save.game.newGame);
            // spawn the merchant
            s.itemManager.lootText.text = "goods:";
            // indicate that the player should trade
            if (HasSavedFloorItems()) { s.tombstoneData.SpawnSavedMerchantItems(true); }
            else { s.itemManager.SpawnMerchantItems(); }
            // spawn the Saved merchant items
            s.turnManager.RefreshBlackBoxVisibility();
            // hide the stats (don't fight merchants)
        }
        else { 
            // else some fightable enemy
            if (Save.game.newGame) { SpawnNewEnemy(Random.Range(3, 7), true); }
            // if in a new game, spawn a generic enemy
            else { 
                // else resuming game from teh Savefile
                SpawnNewEnemy(Save.game.enemyNum, false); 
                // spawn the same enemy back in
                if (Save.game.enemyIsDead) { 
                    // if resuming after the enemy has been killed
                    s.itemManager.lootText.text = "loot:";
                    if (s.levelManager.level == 4 || Save.game.enemyNum == 2) { s.tombstoneData.SpawnSavedMerchantItems(true); }
                    else { 
                        if (Save.game.floorItemTypes[0] == "weapon") { s.tombstoneData.SpawnSavedFloorItems(true);  }
                        else { s.tombstoneData.SpawnSavedMerchantItems(true); }
                    }
                    // spawn in the enemy's loot again for the player
                    // devil and lich dont have weapons, so make sure it doesnt bug
                    GetComponent<SpriteRenderer>().sprite = GetDeathSprite();
                    SetEnemyPositionAfterDeath();
                    // show the enemy as dead
                }
                else { s.itemManager.lootText.text = ""; }
                // enemy is not dead, so nothing special 
            }
        }
        // spawn an enemy at the start of the round
        iconGameobject.transform.position = iconPosition;
        // set the position of the icon, enemy's is set in spawnnewenemy()
    }

    /// <summary>
    /// Make the enemy target the best wound that it can.
    /// </summary>
    public void TargetBest() {
        targetIndex = EnemyAI.GetBestTargetIndex(s);
        s.turnManager.SetTargetOf("enemy");
    }

    /// <summary>
    /// Make the enemy pick the dice it deems to be most valuable. 
    /// </summary>
    public void ChooseBestDie() {
        EnemyAI.ChooseBestDie(s);
    }

    /// <summary>
    /// Spawn an enemy based on its number on in the array.
    /// </summary>
    public void SpawnNewEnemy(int enemyNum, bool isNewEnemy) {
        EnemyAI.InvalidateCachedPlan();
        if (isNewEnemy) {
            // creating new enemy
            Save.game.enemyIsDead = false;
            Save.game.enemyBleedsOutNextRound = false;
            // make sure enemy is not dead
            float[] temp;
            // array to hold stats
            if (enemyNum == 2) { temp = s.levelManager.GenStats("lich"); }
            else if (enemyNum == 0) { temp = s.levelManager.GenStats("devil"); }
            else { temp = s.levelManager.GenStats("normal"); }
            stats = new Dictionary<string, int> {
                { "green", (int)temp[0] },
                { "blue", (int)temp[1] },
                { "red", (int)temp[2] },
                { "white", (int)temp[3] },
            };
            Save.game.enemyAcc = stats["green"];
            Save.game.enemySpd = stats["blue"];
            Save.game.enemyDmg = stats["red"];
            Save.game.enemyDef = stats["white"];
            // set stats of the enemy
            spawnNum = enemyNum;
            int displayIndex = GetDisplayIndex(enemyNum);
            iconGameobject.GetComponent<SpriteRenderer>().sprite = icons[displayIndex];
            // set the sprite for the icon
            GetComponent<Animator>().enabled = true;
            // enable the animator (which is disabled from enemies dying)
            try {GetComponent<Animator>().runtimeAnimatorController = controllers[displayIndex]; } 
            catch { 
                GetComponent<Animator>().runtimeAnimatorController = null; 
                GetComponent<SpriteRenderer>().sprite = icons[displayIndex];
            }
            // try set the controller (none for tombstone), must use runtimeanimationcontroller here
            if (enemyArr[enemyNum] == "Devil" || enemyArr[enemyNum] == "Cloaked") {
                // devil needs to be in a different spot
                transform.position = offsetPositions["Devil"];
            }
            else if (enemyArr[enemyNum] == "Tombstone") {
                // tombstone also needs to be different
                transform.position = offsetPositions["Tombstone"];
                iconGameobject.GetComponent<SpriteRenderer>().sprite = tombstoneIcon;
            }
            else {
                // set normal position
                transform.position = basePosition;
            }
            enemyName.text = enemyArr[enemyNum] == "Cloaked" ? "Devil" : enemyArr[enemyNum];
            // set the name, when spawning the cloaked just set it to be "Devil"
            stamina = GetSpawnStamina(enemyNum);
            // assign stamina based on level, sub, and difficulty
            woundList = new List<string>();
            Save.game.enemyWounds = woundList;
            Save.game.enemyStamina = stamina;
            Save.game.enemyTargetIndex = targetIndex;
            SyncWoundDerivedEnemyState();
            Save.SaveGame();
        }
        else { 
            // spawning old enemy
            stats = new Dictionary<string, int> {
                { "green", Save.game.enemyAcc },
                { "blue", Save.game.enemySpd },
                { "red", Save.game.enemyDmg },
                { "white", Save.game.enemyDef },
            };
            spawnNum = enemyNum;
            // enemy inherits its stats from the Save
            try { s.turnManager.DisplayWounds(); } catch {}
            // try displaying wounds
            int displayIndex = GetDisplayIndex(enemyNum);
            iconGameobject.GetComponent<SpriteRenderer>().sprite = icons[displayIndex];
            // set its sprite
            GetComponent<Animator>().enabled = !Save.game.enemyIsDead;
            // enable/disable the animator, depending on if the enemy is dead or not
            try { GetComponent<Animator>().runtimeAnimatorController = controllers[displayIndex]; } 
            catch { 
                GetComponent<Animator>().runtimeAnimatorController = null; 
                GetComponent<SpriteRenderer>().sprite = icons[displayIndex];
            }
            // try setting the controller, if it doesnt work (tombstone) then set it to null
            if (enemyArr[enemyNum] == "Devil" || enemyArr[enemyNum] == "Cloaked") {
                transform.position = offsetPositions["Devil"];
            }
            else if (enemyArr[enemyNum] == "Tombstone") {
                transform.position = offsetPositions["Tombstone"];
                iconGameobject.GetComponent<SpriteRenderer>().sprite = tombstoneIcon;
            }
            else { transform.position = basePosition; }
            // devil and tombstone have special positions
            enemyName.text = enemyArr[enemyNum] == "Cloaked" ? "Devil" : enemyArr[enemyNum];
            // devil should only be called 'cloaked', not devil
            if (enemyArr[enemyNum] == "Tombstone" || IsVendor(enemyNum)) { stamina = 0; }
            else if (enemyArr[enemyNum] == "Lich") { stamina = s.enemy.lichStamina; }
            else { stamina = Save.game.enemyStamina; }
            woundList = Save.game.enemyWounds != null ? new List<string>(Save.game.enemyWounds) : new List<string>();
            Save.game.enemyWounds = woundList;
            targetIndex = Mathf.Clamp(Save.game.enemyTargetIndex, 0, 7);
            SyncWoundDerivedEnemyState();
            // set stamina, show wounds and name
        }
        staminaCounter.text = stamina.ToString();
        // show the amount of stamina the enemy has
        try { s.turnManager.SetTargetOf("enemy"); } catch {} 
        try { s.turnManager.DisplayWounds(); } catch {}
        Save.game.enemyNum = enemyNum;
        if (s.tutorial == null) { Save.SaveGame(); }
    }

    /// <summary>
    /// Discard the player's most valuable dice from them.
    /// </summary>
    public void DiscardBestPlayerDie() {
        StartCoroutine(DiscardBestPlayerDieCoro());
    }

    /// <summary>
    /// Do not call this coroutine, use DiscardBestPlayerDie() instead.
    /// </summary>
    private IEnumerator DiscardBestPlayerDieCoro() {
        yield return s.delays[0.25f];
        List<Dice> curAvailableDice = new();
        foreach (GameObject curDice in s.diceSummoner.existingDice) {
            Dice diceScript = curDice.GetComponent<Dice>();
            if (curDice.GetComponent<Dice>().isOnPlayerOrEnemy == "player") {
                curAvailableDice.Add(diceScript);
            }
        }
        Dice chosenDie = EnemyAI.GetBestPlayerDieToDiscard(s, curAvailableDice);
        if (chosenDie != null) { chosenDie.DiscardFromPlayer(); }
    }

    /// <summary>
    /// Gets the death sprite of the enemy.
    /// </summary>
    public Sprite GetDeathSprite() {
        return deathSprites[Array.IndexOf(enemyArr, enemyName.text)];
    }

    /// <summary>
    /// After death, move the enemy's corpse to the correct position on the ground.
    /// </summary>
    public void SetEnemyPositionAfterDeath() {
        transform.position = deathPositions[enemyName.text];
    }
}