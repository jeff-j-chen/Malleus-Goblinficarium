using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
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
    public int sub { get; private set; } = 1;
    private Dictionary<string, float[]> levelStats = new Dictionary<string, float[]>()
    {
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

    void Start()
    {
        scripts = FindObjectOfType<Scripts>();
        boxSR = levelBox.GetComponent<SpriteRenderer>();
        temp = boxSR.color;
        temp.a = 0f;
        boxSR.color = temp;
        levelBox.transform.position = offScreen;
        loadingCircle.transform.position = offScreen;
        // StartCoroutine(NextLevel());
    }

    public float[] GenStats(string lichOrDevilOrNormal = "normal")
    {
        if (lichOrDevilOrNormal != "normal")
        {
            if (lichOrDevilOrNormal == "lich") { return new float[] { 1f, 1f, 1f, 1f }; }
            else if (lichOrDevilOrNormal == "devil") { return new float[] { 2f, 2f, 2f, 2f }; }
            else { print("invalid enemy to attempt to spawn");return balanced;}
        }
        else
        {
            float[] stats = levelStats[level.ToString() + sub.ToString()];
            float[] totalStats = new float[4];
            float[] baseStats = null;
            if (level == 1) { baseStats = GenBaseStats(stats, balanced); }
            else if (level == 2) { baseStats = GenBaseStats(stats, damage); }
            else if (level == 3) { baseStats = GenBaseStats(stats, fast); }
            
            for (int i = 0; i < 4; i++)
            {
                totalStats[i] = Mathf.Round((stats[i] + baseStats[i] + UnityEngine.Random.Range(0f, stats[4]))/10f);
            }
            return totalStats;
        }
    }

    private float[] GenBaseStats(float[] stats, float[] normal)
    {
        float sum = stats[5] + stats[6] + stats[7] + stats[8] + stats[9];
        if (sum != 10f) { print("not 10f"); return balanced; }
        else
        {
            int rand = UnityEngine.Random.Range(1, 11);

            float[] chances = new float[] { stats[5], stats[5] + stats[6], stats[5] + stats[6] + stats[7], stats[5] + stats[6] + stats[7] + stats[8], sum };
            if (rand >= 0f && rand < chances[0])               { return balanced; }
            else if (rand >= chances[0] && rand < chances[1])  { return fast; }
            else if (rand >= chances[1] && rand < chances[2])  { return damage; }
            else if (rand >= chances[2] && rand < chances[3])  { return defense; }
            else if (rand >= chances[3] && rand < chances[4])  { return mix; }
            else { return normal; }
        }
    }
    // dice of slain heroes rattle around his neck
    // just use devil's head for his icon
    // name is devil
    // wound list is [cloaked]
    // his cloak shatters
    // glass breaking sound

    private void SummonTrader()
    {
        scripts.enemy.SpawnNewEnemy(99); // spawn the trader here
        scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen;
        scripts.itemManager.SpawnTraderItems();
    }

    public IEnumerator NextLevel(bool isLich=false)
    {
        Color temp = boxSR.color;
        temp.a = 0f;
        for (int i = 0; i < 15; i++)
        {
            yield return scripts.delays[0.033f];
            temp.a += 1f/15f;
            boxSR.color = temp;
        }
        loadingCircle.transform.position = onScreen;
        scripts.itemManager.HideItems();
        if (!isLich)
        {
            sub++;
            if (sub == 4) 
            { 
                SummonTrader();
                levelText.text = "level " + level + "-3+"; 
            }
            else if (sub > 4) { sub = 1; level++; levelText.text = "level " + level + "-" + sub; }
            else { levelText.text = "level " + level + "-" + sub; }
            if (level == 3 && sub == 4) { scripts.enemy.SpawnNewEnemy(0); }
            else { scripts.enemy.SpawnNewEnemy(UnityEngine.Random.Range(3, 7)); }
        }
        else 
        { 
            levelText.text = "???"; 
            scripts.enemy.SpawnNewEnemy(2);
        }
        scripts.itemManager.Select(scripts.player.inventory, 0);
        yield return scripts.delays[1.5f];
        scripts.statSummoner.SummonStats();
        scripts.statSummoner.SetDebugInformationFor("enemy");
        scripts.itemManager.AttemptFadeTorches();
        levelText.text = "";
        loadingCircle.transform.position = offScreen;
        levelBox.transform.position = offScreen;
        scripts.itemManager.numItemsDroppedForTrade = 0;
        if (sub != 4)
        {
            scripts.diceSummoner.SummonDice(false);
            scripts.turnManager.blackBox.transform.position = scripts.turnManager.offScreen;
        }
        for (int i = 0; i < 15; i++)
        {
            yield return scripts.delays[0.033f];
            temp.a -= 1f/15f;
            boxSR.color = temp;
        }
        scripts.itemManager.DestroyItems();
        scripts.turnManager.DetermineMove(false);
        StopCoroutine(NextLevel());
    }
}
