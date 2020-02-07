using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour {
    [SerializeField] GameObject levelBox;
    [SerializeField] GameObject loadingCircle;
    Vector3 onScreen = new Vector2(0.0502f, -1.533f);
    Vector3 offScreen = new Vector2(-20f, 15f);
    [SerializeField] TextMeshProUGUI levelText;
    SpriteRenderer boxSR;
    Color temp;
    Scripts scripts;
    private float[] balanced = new float[] { 0f, 10f, 10f, 0f };
    private float[] fast =     new float[] { 0f, 20f, 10f, 0f };
    private float[] damage =   new float[] { 10f, 3f, 23f, 3f };
    private float[] defense =  new float[] { 2f, 10f, 2f, 23f };
    private float[] mix =      new float[] { 2f, -10f, 18f, 18f };
    public int level { get; private set; } = 1;
    public int sub { get; private set; } = 3;
    private Dictionary<string, float[]> levelStats = new Dictionary<string, float[]>() {
        // add on the stats and iterate (add) through with random variance, divide, then round to get final stats
        //                    aim, spd, atk, def, var,   bal/fas/dmg/def/mix
        { "11", new float[] { 10f, 10f, 10f, 10f, 0f,    7f, 1f, 1f, 1f, 0f } },
        { "12", new float[] { 10f, 10f, 10f, 10f, 0f,    4f, 2f, 2f, 1f, 1f } },
        { "13", new float[] { 10f, 10f, 10f, 10f, 0.75f, 3f, 2f, 2f, 2f, 1f } },
        { "21", new float[] { 10f, 10f, 10f, 10f, 1f,    3f, 2f, 2f, 2f, 1f} },
        { "22", new float[] { 10f, 15f, 10f, 10f, 1f,    2f, 2f, 2f, 2f, 2f } },
        { "23", new float[] { 10f, 10f, 11f, 11f, 1.25f, 2f, 2f, 2f, 2f, 2f } },
        { "31", new float[] { 10f, 10f, 12f, 12f, 1.25f, 1f, 3f, 2f, 2f, 2f } },
        { "32", new float[] { 12f, 15f, 14f, 14f, 1.5f,  0f, 2f, 3f, 2f, 3f } },
        { "33", new float[] { 15f, 15f, 15f, 15f, 3f,    2f, 2f, 2f, 2f, 2f  } }
        // something to make it more probable that genstats will gen more difficult enemies later
    };

    [SerializeField] public bool lockActions = false;

    void Start()
    {
        scripts = FindObjectOfType<Scripts>();
        boxSR = levelBox.GetComponent<SpriteRenderer>();
        // get the spriterenderer for the box that covers the screen when the next level is being loaded
        temp = boxSR.color;
        temp.a = 0f;
        boxSR.color = temp;
        // make it transparent
        levelBox.transform.position = offScreen;
        loadingCircle.transform.position = offScreen;
        // make the black box and the loading circle go off the screen
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
            else { print("invalid enemy to attempt to spawn");return balanced;}
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
        if (sum != 10f) { print("not 10f"); return balanced; }
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
    // dice of slain heroes rattle around his neck
    // just use devil's head for his icon
    // name is devil
    // wound list is [cloaked]
    // his cloak shatters
    // glass breaking sound

    /// <summary>
    /// Summon a trader.
    /// </summary>
    private void SummonTrader() {
        scripts.enemy.SpawnNewEnemy(7); // spawn the trader here
        // create the trader enemy
        scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen;
        // hide the trader's stats (which are given to enemies by default)
    }

    /// <summary>
    /// Makes the game go to the next level.
    /// </summary>
    /// <param name="isLich">true to spawn the lich, false (default) otherwise</param>
    public IEnumerator NextLevel(bool isLich=false) {
        if (!lockActions) {
            lockActions = true;
            Color temp = boxSR.color;
            temp.a = 0f;
            // hide the level loading box
            scripts.soundManager.PlayClip("next");
            // play sound clip
            for (int i = 0; i < 15; i++) {
                yield return scripts.delays[0.033f];
                temp.a += 1f/15f;
                boxSR.color = temp;
            }
            // fade in the box
            loadingCircle.transform.position = onScreen;
            // make the loading thing go on screen
            scripts.itemManager.HideItems();
            // hide the items (can't destroy them here or the game breaks for some reason)
            if (!isLich) {
                // if spawning a normal enemy
                sub++;
                // increment the sub counter
                if (sub == 4) { 
                    SummonTrader();
                    // summon trader if necessary
                    levelText.text = "level " + level + "-3+"; 
                    // set the correct level loading text
                }
                else if (sub > 4) { sub = 1; level++; levelText.text = "level " + level + "-" + sub; }
                // going on to the next level (as opposed to next sub, so make sure to set the variables up correctly)
                else { levelText.text = "level " + level + "-" + sub; }
                // only going to the next sub, so notify the player accordingly
                if (level == 3 && sub == 4) { scripts.enemy.SpawnNewEnemy(0); }
                // spawn the devil if on the correct level

                // add something here to make it really glitchy (like how it is in the actual game)

                else { 
                    if (sub != 4) {
                        scripts.enemy.SpawnNewEnemy(UnityEngine.Random.Range(3, 7)); 
                    }
                }
                // otherwise just spawn a random enemy
            }
            else { 
                levelText.text = "???"; 
                // going to the lich level, so notify player
                scripts.enemy.SpawnNewEnemy(2);
            }
            yield return scripts.delays[1.5f];
            // wait 1.5s
            scripts.statSummoner.SummonStats();
            scripts.statSummoner.SetDebugInformationFor("enemy");
            // summon the stats and update the debug information
            levelText.text = "";
            loadingCircle.transform.position = offScreen;
            levelBox.transform.position = offScreen;
            // clear the loading text and move the box offscreen
            scripts.itemManager.numItemsDroppedForTrade = 0;
            // clear the number of items player has dropped
            if (sub != 4) {
                // if not going to the trader level
                scripts.diceSummoner.SummonDice(false);
                scripts.turnManager.blackBox.transform.position = scripts.turnManager.offScreen;
                // summon die and make sure the enemy's stats can be seen
            }
            else{ scripts.itemManager.SpawnTraderItems(); }
            // can spawn the items here because we have a deletion queue rather than just deleting all
            for (int i = 0; i < 15; i++) {
                yield return scripts.delays[0.033f];
                temp.a -= 1f/15f;
                boxSR.color = temp;
            }
            // fade the level box back out
            scripts.itemManager.AttemptFadeTorches();
            // try to remove torches
            scripts.itemManager.DestroyItems();
            // only now do we destroy the items
            // spawn the items so the player can interact with them, after the items are destroyed
            scripts.turnManager.DetermineMove(false);
            // determine who moves
            lockActions = false;
            StopCoroutine(NextLevel());
            // stop this coroutine
        }
    }
}
