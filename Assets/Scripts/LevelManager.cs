using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelManager : MonoBehaviour {
    [SerializeField] private GameObject levelBox;
    [SerializeField] private GameObject loadingCircle;
    Vector3 onScreen = new Vector2(0.0502f, -1.533f);
    Vector3 offScreen = new Vector2(-20f, 15f);
    [SerializeField] private TextMeshProUGUI levelTransText;
    [SerializeField] private TextMeshProUGUI levelText;
    SpriteRenderer boxSR;
    Color temp;
    Scripts scripts;
    private float[] balanced = new float[] { 0f, 10f, 10f, 0f };
    private float[] fast =     new float[] { 0f, 20f, 10f, 0f };
    private float[] damage =   new float[] { 10f, 3f, 23f, 3f };
    private float[] defense =  new float[] { 2f, 10f, 2f, 23f };
    private float[] mix =      new float[] { 2f, -10f, 18f, 18f };
    [SerializeField] public int level;
    [SerializeField] public int sub;
    private Dictionary<string, float[]> levelStats = new Dictionary<string, float[]>() {
        // add on the stats and iterate (add) through with random variance, divide, then round to get final stats
        //                    aim, spd, atk, def, var,   bal/fas/dmg/def/mix
        { "11", new float[] { 10f, 10f, 10f, 10f, 0f,    10f, 0f, 0f, 0f, 0f } },
        { "12", new float[] { 10f, 10f, 10f, 10f, 0f,    4f, 2f, 2f, 1f, 1f  } },
        { "13", new float[] { 10f, 10f, 10f, 10f, 0.75f, 3f, 2f, 2f, 2f, 1f  } },
        { "21", new float[] { 10f, 10f, 10f, 10f, 1f,    3f, 2f, 2f, 2f, 1f  } },
        { "22", new float[] { 10f, 15f, 10f, 10f, 1f,    2f, 2f, 2f, 2f, 2f  } },
        { "23", new float[] { 10f, 10f, 11f, 11f, 1.25f, 2f, 2f, 2f, 2f, 2f  } },
        { "31", new float[] { 10f, 10f, 12f, 12f, 1.25f, 1f, 3f, 2f, 2f, 2f  } },
        { "32", new float[] { 12f, 15f, 14f, 14f, 1.5f,  0f, 2f, 3f, 2f, 3f  } },
        { "33", new float[] { 15f, 15f, 15f, 15f, 3f,    2f, 2f, 2f, 2f, 2f  } }
        // something to make it more probable that genstats will gen more difficult enemies later
    };
    [SerializeField] public bool lockActions = false;
    [SerializeField] private string characters = "";
    [SerializeField] private string thinCharacters = "";
    private Coroutine transGlitchCoro;
    private Coroutine debugGlitchCoro;

    void Start() {
        scripts = FindObjectOfType<Scripts>();
        if (PlayerPrefs.GetString("debug") == "on") { levelText.gameObject.SetActive(true); }
        else { levelText.gameObject.SetActive(false); }
        level = scripts.gameData.resumeLevel;
        sub = scripts.gameData.resumeSub;
        boxSR = levelBox.GetComponent<SpriteRenderer>();
        // get the spriterenderer for the box that covers the screen when the next level is being loaded
        temp = boxSR.color;
        temp.a = 0f;
        boxSR.color = temp;
        // make it transparent
        levelBox.transform.position = offScreen;
        loadingCircle.transform.position = offScreen;
        // make the black box and the loading circle go off the screen
        lockActions = false;
        // make sure actions aren't locked
        if (sub == 4) { levelText.text = $"(level {level}-3+)"; }
        else if (sub == scripts.persistentData.tsSub && level == scripts.persistentData.tsLevel && !(sub == 1 && level == 1))
        { levelText.text = $"(level {level}-{sub}*)"; }
        else if (level == 4 && sub == 1) { StartCoroutine(GlitchyDebugText()); }
        else if (scripts.enemy.enemyName.text == "Lich") { levelText.text = $"(level ???)"; }
        else { levelText.text = $"(level {level}-{sub})"; }
    }

    /// <summary>
    /// Generate the stats for an enemy, which is either the lich, the devil, or just a normal enemy.
    /// </summary>
    /// <param name="lichOrDevilOrNormal">Which type of enemy to generate.</param>
    /// <returns>A float array of the stats that were generated.</returns>
    public float[] GenStats(string lichOrDevilOrNormal = "normal") {
        if (lichOrDevilOrNormal != "normal") {
            if (lichOrDevilOrNormal == "lich") { return new float[] { 1f, 1f, 1f, 1f }; }
            // lich has 1/1/1/1
            else if (lichOrDevilOrNormal == "devil") { return new float[] { 2f, 2f, 2f, 2f }; }
            // devil has 2/2/2/2
            else { 
                // print("invalid enemy to attempt to spawn");
                return balanced;
            }
            // notify me of error and return a basic one
        }
        else {
            float[] stats;
            if (sub == 4) { return new float[] { 0f, 0f, 0f, 0f }; }
            // given key is not present in the dictionary for sub-4s, instantly return blank
            else { stats = levelStats[level.ToString() + sub.ToString()]; }
            // based on the level and sub, get the stats from the level
            float[] totalStats = new float[4];
            float[] baseStats = null;
            // create empty arrays of floats to store the stats in
            if (level == 1) { baseStats = GenBaseStats(stats, balanced); }
            else if (level == 2) { baseStats = GenBaseStats(stats, damage); }
            else if (level == 3) { baseStats = GenBaseStats(stats, fast); }
            // generate the stats for the enemy based on level 
            for (int i = 0; i < 4; i++) {
                // for every stat, the set the stats to be the combination of level stats (from dictionary), the base stats (from function), and a slight amount of RNG 
                totalStats[i] = Mathf.Round((stats[i] + baseStats[i] + UnityEngine.Random.Range(0f, stats[4]))/10f);
            }
            return totalStats;
            // return the total stats
        }
    }

    /// <summary>
    /// Generate the base stats of an enemy on the given chances.
    /// </summary>
    /// <param name="stats">The stat (float[]) which contains the chances for the different stat presets.</param>
    /// <param name="normal">The default stat to go back to if something goes wrong.</param>
    /// <returns>A float array of the generated stats.</returns>
    private float[] GenBaseStats(float[] stats, float[] normal) {
        float sum = stats[5] + stats[6] + stats[7] + stats[8] + stats[9];
        // get the sum of the present chances
        if (sum != 10f) { 
            // print("not 10f"); 
            return balanced; 
        }
        // something went wrong while setting up the dictionary, so notify me and return (so compiler is happy)
        else {
            int rand = UnityEngine.Random.Range(1, 11);
            // get a random number from 1-10
            float[] chances = new float[] { stats[5], stats[5] + stats[6], stats[5] + stats[6] + stats[7], stats[5] + stats[6] + stats[7] + stats[8], 10f };
            // create a float array of the different chances
            if (rand >= 0f && rand < chances[0])               { return balanced; }
            else if (rand >= chances[0] && rand < chances[1])  { return fast; }
            else if (rand >= chances[1] && rand < chances[2])  { return damage; }
            else if (rand >= chances[2] && rand < chances[3])  { return defense; }
            else if (rand >= chances[3] && rand < chances[4])  { return mix; }
            else { return normal; }
            // return a stat preset based on the chances and the random number
        }
    }

    /// <summary>
    /// Go to the next level, or the lich. Call this instead of the coroutine (more stable).
    /// </summary>
    /// <param name="isLich">true to go to the lich, false otherwise.</param>
    public void NextLevel(bool isLich = false) {
        StartCoroutine(NextLevelCoroutine(isLich));
    }

    public IEnumerator NextLevelCoroutine(bool isLich=false) {
        if (!lockActions) {
            lockActions = true;
            Color temp = boxSR.color;
            temp.a = 0f;
            boxSR.color = temp;
            // hide the level loading box
            scripts = FindObjectOfType<Scripts>();
            // reget scripts
            scripts.soundManager.PlayClip("next");
            // play sound clip
            if (level == 4 && sub == 1) {
                if (debugGlitchCoro != null) { StopCoroutine(debugGlitchCoro); }
                // going to next level after having defeated devil
                if (scripts.player.charNum != 3) { 
                    // give player the next character, as long as they aren't on the last one\
                    scripts.persistentData.unlockedChars[scripts.player.charNum + 1] = true;
                    scripts.persistentData.successfulRuns++;
                    scripts.SavePersistentData();
                }
                print("notify the player they unlocked a new character to play here!");
                scripts.gameData = new GameData();
                scripts.SaveGameData();
                // for some reason file.delete doesn't want to work here
                Initiate.Fade("Credits", Color.black, 2.5f);
                // load credits scene
            }
            scripts.turnManager.ClearVariablesAfterRound();
            // remove variables before going to next level
            foreach (GameObject dice in scripts.diceSummoner.existingDice) {
                StartCoroutine(dice.GetComponent<Dice>().FadeOut(false, true));
            }
            // fade out all die (die are only faded out upon kill normally)
            // yield return scripts.delays[1.5f]; // uncomment for tombstone tests
            if (isLich || level == 3 && sub == 4) { 
                scripts.music.FadeVolume("LaBossa");
                // if spawning lich or devil, fade to boss music
            }
            else if (sub == 3) { 
                scripts.music.FadeVolume("Smoke");
                // if spawning merchant, fade to smoke
            }
            else { scripts.music.FadeVolume(); }
            string toSpawn = "";
            for (int i = 0; i < 15; i++) {
                yield return scripts.delays[0.033f];
                temp.a += 1f/15f;
                boxSR.color = temp;
            }
            // fade in the box
            loadingCircle.transform.position = onScreen;
            // make the loading thing go on screen
            scripts.gameData.floorItemNames = new string[9];
            scripts.gameData.floorItemTypes = new string[9];
            scripts.gameData.floorItemMods = new string[9];
            scripts.gameData.floorAcc = 0;
            scripts.gameData.floorSpd = 0;
            scripts.gameData.floorDmg = 0;
            scripts.gameData.floorDef = 0;
            // clear merchant and floor items when going to the next level
            if (!isLich) {
                // if spawning a normal enemy
                sub++;
                // increment the sub counter
                if (sub > 4) { sub = 1; level++; }
                // increment level and reset sub if we passed sub 4
                if (sub > scripts.persistentData.highestSub && level >= scripts.persistentData.highestLevel) { 
                    scripts.persistentData.highestSub = sub;
                    scripts.persistentData.highestLevel = level;
                }
                if (scripts.enemy.enemyName.text == "Tombstone") {
                    scripts.persistentData.tsLevel = -1;
                    scripts.persistentData.tsSub = -1;
                    scripts.persistentData.tsWeaponAcc = -1;
                    scripts.persistentData.tsWeaponSpd = -1;
                    scripts.persistentData.tsWeaponDmg = -1;
                    scripts.persistentData.tsWeaponDef = -1;
                    scripts.persistentData.tsItemNames = new string[9];
                    scripts.persistentData.tsItemNames = new string[9];
                    scripts.persistentData.tsItemNames = new string[9];
                    sub--;
                    // make tombstone inaccessible
                }
                // going on to the next level (as opposed to next sub, so make sure to set the variables up correctly)
                if (sub == scripts.persistentData.tsSub && level == scripts.persistentData.tsLevel && !(sub == 1 && level == 1)) {
                    // spawn tombstone if we are on the correct level and not on 1-1
                    toSpawn = "tombstone";
                    // level matches which level to add to
                    levelTransText.text = $"level {level}-{sub}*";
                    levelText.text = $"(level {level}-{sub}*)";
                    // decrement sub (because we went up 1 level but aren't going to fight anything)
                    scripts.enemy.SpawnNewEnemy(8, true);
                    // spawn the tombstone
                }
                else if (sub == 4) { 
                    toSpawn = "trader";
                    scripts.enemy.SpawnNewEnemy(7, true);
                    // create the trader enemy
                    scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen;
                    // summon trader if necessary
                    levelTransText.text = $"level {level}-3+";
                    levelText.text = $"(level {level}-3+)";
                    // set the correct level loading text
                }
                else if (level == 4 && sub == 1) { 
                    toSpawn = "devil";
                    // spawn the devil if on the correct level
                    transGlitchCoro = StartCoroutine(GlitchyLevelText());
                    scripts.enemy.SpawnNewEnemy(0, true); 
                    debugGlitchCoro = StartCoroutine(GlitchyDebugText());
                }
                else { 
                    toSpawn = "normal";
                    scripts.enemy.SpawnNewEnemy(UnityEngine.Random.Range(3, 7), true); 
                    levelTransText.text = $"level {level}-{sub}";
                    levelText.text = $"(level {level}-{sub})";
                }
                // normal level, so notify the player accordingly and spawn basic enemy
            }
            else { 
                toSpawn = "lich";
                levelTransText.text = "level ???";
                levelText.text = "(level ???)";
                // going to the lich level, so notify player
                scripts.enemy.SpawnNewEnemy(2, true);
            }
            // always spawn new enemy if going to next level
            scripts.itemManager.DestroyItems();
            // remove all items from the floor
            // scripts.player.GetComponent<Animation>().Rewind();
            scripts.enemy.GetComponent<Animator>().Rebind();
            scripts.enemy.GetComponent<Animator>().Update(0f);
            scripts.player.GetComponent<Animator>().Rebind();
            scripts.player.GetComponent<Animator>().Update(0f);
            yield return scripts.delays[1.5f];
            if (transGlitchCoro != null) { StopCoroutine(transGlitchCoro); }
            // wait 1.5s
            scripts.statSummoner.SummonStats();
            scripts.statSummoner.SetDebugInformationFor("enemy");
            // summon the stats and update the debug information
            levelTransText.text = "";
            loadingCircle.transform.position = offScreen;
            levelBox.transform.position = offScreen;
            // clear the loading text and move the box offscreen
            scripts.itemManager.numItemsDroppedForTrade = 0;
            // clear the number of items player has dropped
            scripts.gameData.numItemsDroppedForTrade = scripts.itemManager.numItemsDroppedForTrade;
            scripts.SaveGameData();
            if (toSpawn == "tombstone") { 
                // going to tombstone, spawn spawn items
                scripts.itemManager.lootText.text = "loot:";
                scripts.tombstoneData.SpawnSavedTSItems();
            }
            else if (toSpawn == "trader") {
                // going to trader, so spawn trader items
                scripts.itemManager.SpawnTraderItems();
            }
            else {
                // not going to the trader or tombstone
                scripts.diceSummoner.SummonDice(false, true);
                scripts.turnManager.blackBox.transform.position = scripts.turnManager.offScreen;
                // summon die and make sure the enemy's stats can be seen
            }
            // can spawn the items here because we have a deletion queue rather than just deleting all
            scripts.player.targetIndex = 0;
            scripts.turnManager.SetTargetOf("player");
            // make it so that the player auto targets chest, rather than their previous attack
            for (int i = 0; i < 15; i++) {
                yield return scripts.delays[0.033f];
                temp.a -= 1f/15f;
                boxSR.color = temp;
            }
            if (toSpawn == "tombstone") {
                scripts.turnManager.SetStatusText("you come across a humble tombstone");
            }
            else if (toSpawn == "lich") {
                scripts.turnManager.SetStatusText("impervious, he seems to be immune to wound effects");
            }
            else if (toSpawn == "devil") {
               scripts.turnManager.SetStatusText("dice of slain heroes rattle around his neck");
            }
            // fade the level box back out
            scripts.itemManager.AttemptFadeTorches();
            // try to remove torches
            // spawn the items so the player can interact with them, after the items are destroyed
            scripts.turnManager.DetermineMove(false);
            // determine who moves
            lockActions = false;
            scripts.gameData.resumeSub = scripts.levelManager.sub;
            scripts.gameData.resumeLevel = scripts.levelManager.level;
        }
        scripts.SavePersistentData();
        scripts.SaveGameData();
    }

    private char r() { 
        // return a random character
        return characters[UnityEngine.Random.Range(0, characters.Length)];
    }
    private char t() { 
        // return a random thin character
        return thinCharacters[UnityEngine.Random.Range(0, thinCharacters.Length)];
    }

    private IEnumerator GlitchyLevelText() {
        for (int i = 0; i < 12; i++) {
            levelTransText.text = $"level {r()}-{r()}";
            yield return scripts.delays[0.033f];
        }
        for (int i = 0; i < 12; i++) {
            levelTransText.text = $"lev{r()} {r()}{r()}{r()}";
            yield return scripts.delays[0.033f];
        }
        for (int i = 0; i < 12; i++) {
            levelTransText.text = $"{r()}{r()}{r()}{r()} {r()}{r()}{r()}";
            yield return scripts.delays[0.05f];
        }
    }

    private IEnumerator GlitchyDebugText() {
        while (true){ 
            levelText.text = $"level {t()}-{t()}";
            yield return scripts.delays[0.05f];
        }
    }
}
