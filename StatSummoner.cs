using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatSummoner : MonoBehaviour
{
    [SerializeField] private GameObject plus;
    [SerializeField] private GameObject minus;
    [SerializeField] private GameObject square;
    [SerializeField] private GameObject negSquare;
    [SerializeField] private GameObject circle;
    [SerializeField] private TextMeshProUGUI playerDebug;
    [SerializeField] private TextMeshProUGUI enemyDebug;

    public readonly float xCoord = -10.5f;
    public readonly float xOffset = 0.65f;
    public readonly float highlightOffset = 0.85f;
    public readonly float diceOffset = 1f;
    private readonly float buttonXCoord = -11.7f;
    private readonly float buttonXOffset = -0.6f;
    public readonly float[] yCoords = new float[] { 8.77f, 7.77f, 6.77f, 5.77f };

    private List<GameObject> existingStatSquares = new List<GameObject>();

    public Dictionary<string, List<Dice>> addedPlayerDice = new Dictionary<string, List<Dice>>()
    {
        { "green", new List<Dice>() },
        { "blue", new List<Dice>() },
        { "red", new List<Dice>() },
        { "white", new List<Dice>() },
    };

    public Dictionary<string, List<Dice>> addedEnemyDice = new Dictionary<string, List<Dice>>()
    {
        { "green", new List<Dice>() },
        { "blue", new List<Dice>() },
        { "red", new List<Dice>() },
        { "white", new List<Dice>() },
    };

    public Dictionary<string, int> addedPlayerStamina = new Dictionary<string, int>()
    {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };

    public Dictionary<string, int> addedEnemyStamina = new Dictionary<string, int>()
    {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };

    private Scripts scripts;

    private void Start()
    {
        scripts = FindObjectOfType<Scripts>();
        if (PlayerPrefs.GetString("debug") == "on")
        {
            playerDebug.color = scripts.colors.disabled;
            enemyDebug.color = scripts.colors.disabled;
        }
        else
        {
            playerDebug.color = Color.black;
            enemyDebug.color = Color.black;
        }
        SummonStaminaButtons();
    }

    public void SummonStats()
    {
        foreach (GameObject stat in existingStatSquares)
        {
            Destroy(stat);
        }
        existingStatSquares = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GenerateForStat(i, scripts.colors.colorNameArr[i]);
        }
        SetDebugInformationFor("player");
        SetDebugInformationFor("enemy");
    }

    public int SumOfStat(string stat, string playerOrEnemy)
    {
        if (stat != "green" && stat != "blue" && stat != "red" && stat != "white")
        {
            Debug.LogError("Invalid stat to get the sum of");
            return 0;
        }
        else
        {
            if (playerOrEnemy == "player")
            {
                if (scripts.enemy.woundList.Contains("knee") && stat == "blue")
                {
                    return 99;
                }
                int sum = scripts.player.stats[stat] + scripts.player.potionStats[stat] + addedPlayerStamina[stat] + scripts.itemManager.neckletStats[stat];
                foreach (Dice dice in addedPlayerDice[stat])
                {
                    if (dice != null)
                    {
                        sum += dice.GetComponent<Dice>().diceNum;
                    }
                }
                return sum;
            }
            else if (playerOrEnemy == "enemy")
            {
                if (scripts.player.woundList.Contains("knee") && stat == "blue")
                {
                    return 99;
                }
                int sum = scripts.enemy.stats[stat] + addedEnemyStamina[stat];
                foreach (Dice dice in addedEnemyDice[stat])
                {
                    if (dice != null)
                    {
                        sum += dice.GetComponent<Dice>().diceNum;
                    }
                }
                return sum;
            }
            else
            {
                Debug.LogError("Can only get the stats of a player or an enemy");
                return 0;
            }
        }
    }

    private void SummonStaminaButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject spawnedPlusButton = SpawnButton(plus, new Vector2(buttonXCoord, yCoords[i] - 0.01f));
            // - 0.01 is necessary beacuse otherwise it has a weird visual glitch
            GameObject spawnedMinusButton = SpawnButton(minus, new Vector2(buttonXCoord + buttonXOffset, yCoords[i]));
            string colorName = scripts.colors.colorNameArr[i];
            spawnedPlusButton.GetComponent<StaminaButton>().stat = colorName;
            spawnedMinusButton.GetComponent<StaminaButton>().stat = colorName;
        }
    }

    private void GenerateForStat(int i, string colorName)
    {
        Color statColor = scripts.colors.colorArr[Array.IndexOf(scripts.colors.colorNameArr, colorName)];
        if (scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName] > 0)
        {
            int k0;
            for (k0 = 0; k0 < scripts.player.stats[colorName] + scripts.player.potionStats[colorName]; k0++)
            {
                SpawnGeneratedShape(i, statColor, k0, xCoord, xOffset, true, true);
            }
            for (int k1 = 0; k1 < scripts.itemManager.neckletStats[colorName]; k1++)
            {
                SpawnGeneratedShape(i, statColor, k0 + k1, xCoord, xOffset, true, false);
            }
        }
        else 
        {
            for (int k = 0; k < -(scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName]); k++)
            {
                SpawnGeneratedShape(i, statColor, k, xCoord, xOffset, false);
            }
        }
        if (addedPlayerStamina[colorName] > 0 && scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName] > 0)
        {
            if (scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] > 0)
            {
                for (int j = scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName]; j < scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName]; j++)
                {
                    SpawnGeneratedShape(i, scripts.colors.yellow, j, xCoord, xOffset, true);
                }
            }
            else
            {
                for (int j = 0; j < scripts.player.stats[colorName] + scripts.itemManager.neckletStats[colorName] + scripts.player.potionStats[colorName] + addedPlayerStamina[colorName]; j++)
                {
                    SpawnGeneratedShape(i, scripts.colors.yellow, j, xCoord, xOffset, true);
                }
            }
        }
        if (scripts.enemy.stats[colorName] + addedEnemyStamina[colorName] > 0)
        {
            for (int l = 0; l < scripts.enemy.stats[colorName]; l++)
            {
                SpawnGeneratedShape(i, statColor, l, -xCoord + 1, -xOffset, true);
            }
        }
        else
        {
            for (int l = 0; l < -(scripts.enemy.stats[colorName] + addedEnemyStamina[colorName]); l++)
            {
                SpawnGeneratedShape(i, statColor, l, -xCoord + 1, -xOffset, false);
            }
        }
        if (addedEnemyStamina[colorName] > 0 && scripts.enemy.stats[colorName] + addedEnemyStamina[colorName] > 0)
        {
            if (scripts.enemy.stats[colorName] > 0)
            {
                for (int n = scripts.enemy.stats[colorName]; n < scripts.enemy.stats[colorName] + addedEnemyStamina[colorName]; n++)
                {
                    SpawnGeneratedShape(i, scripts.colors.yellow, n, -xCoord + 1, -xOffset, true);
                }
            }
            else
            {
                for (int n = 0; n < scripts.enemy.stats[colorName] + addedEnemyStamina[colorName]; n++)
                {
                    SpawnGeneratedShape(i, scripts.colors.yellow, n, -xCoord + 1, -xOffset, true);
                }
            }
        }
    }

    private void SpawnGeneratedShape(int i, Color statColor, int k, float coord, float offset, bool isPositive, bool isSquare=true)
    {
        Vector3 instantationPos = new Vector2(coord + (k * offset), yCoords[i]);
        GameObject spawnedShape = GenShape(statColor, instantationPos, isPositive, isSquare);
        existingStatSquares.Add(spawnedShape);
    }

    private GameObject GenShape(Color statColor, Vector3 instantationPos, bool isPositive, bool isSquare)
    {
        GameObject spawnedShape = null;
        if (isPositive)
        { 
            if (isSquare) { spawnedShape = Instantiate(square, instantationPos, Quaternion.identity);  }
            else { spawnedShape = Instantiate(circle, new Vector2(instantationPos.x, instantationPos.y), Quaternion.identity);  }
        }
        else 
        { 
            if (instantationPos.x <= 0) { spawnedShape = Instantiate(negSquare, instantationPos, Quaternion.identity);  }
            else 
            { 
                spawnedShape = Instantiate(negSquare, instantationPos, Quaternion.identity);  
                spawnedShape.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        spawnedShape.GetComponent<SpriteRenderer>().color = statColor;
        spawnedShape.transform.parent = transform;
        return spawnedShape;
    }

    private GameObject SpawnButton(GameObject buttonType, Vector3 instantationPos)
    {
        GameObject spawnedButton = Instantiate(buttonType, instantationPos, Quaternion.identity);
        spawnedButton.transform.parent = transform;
        return spawnedButton;
    }

    public void ResetDiceAndStamina()
    {
        foreach (GameObject dice in scripts.diceSummoner.existingDice)
        {
            StartCoroutine(dice.GetComponent<Dice>().FadeOut());
        }
        foreach (string key in addedPlayerDice.Keys)
        { 
            addedPlayerDice[key].Clear();
            addedEnemyDice[key].Clear();
            addedPlayerStamina[key] = 0;
            addedEnemyStamina[key] = 0;
        }
        SetDebugInformationFor("player");
        SetDebugInformationFor("enemy");
    }

    public void AddDiceToPlayer(string addTo, Dice dice)
    {
        addedPlayerDice[addTo].Add(dice);
    }

    public void AddDiceToEnemy(string addTo, Dice dice)
    {
        addedEnemyDice[addTo].Add(dice);
    }

    public float OutermostPlayerX(string statType, string optionalDiceOffsetStatToMultiplyBy = null)
    {
        if (optionalDiceOffsetStatToMultiplyBy == null) { optionalDiceOffsetStatToMultiplyBy = statType; };
        return xCoord + ((Mathf.Abs(scripts.player.stats[statType] + scripts.player.potionStats[statType] + scripts.itemManager.neckletStats[statType] + addedPlayerStamina[statType]) - 1) * xOffset + highlightOffset + diceOffset * scripts.statSummoner.addedPlayerDice[optionalDiceOffsetStatToMultiplyBy].Count);
    }

    public float OutermostEnemyX(string statType, string optionalDiceOffsetStatToMultiplyBy = null)
    {
        if (optionalDiceOffsetStatToMultiplyBy == null) { optionalDiceOffsetStatToMultiplyBy = statType; };
        return -xCoord + 1 + ((Mathf.Abs(scripts.enemy.stats[statType]) - 1) * -xOffset)  - scripts.statSummoner.highlightOffset - scripts.statSummoner.diceOffset * (scripts.statSummoner.addedEnemyDice[statType].Count - 1);
    }

    public void SetDebugInformationFor(string playerOrEnemy)
    {
        if (playerOrEnemy == "player")
        {
            playerDebug.text = "(" + SumOfStat("green", "player") + ")\n(" + SumOfStat("blue", "player") + ")\n(" + SumOfStat("red", "player") + ")\n(" + SumOfStat("white", "player") + ")";
        }
        else if (playerOrEnemy == "enemy")
        {
            enemyDebug.text = "(" + SumOfStat("green", "enemy") + ")\n(" + SumOfStat("blue", "enemy") + ")\n(" + SumOfStat("red", "enemy") + ")\n(" + SumOfStat("white", "enemy") + ")";
        }
        else { Debug.Log("error"); }
    }
}