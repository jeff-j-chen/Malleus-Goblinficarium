using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
public class StatSummoner : MonoBehaviour {
    private enum PlayerStatShape {
        Square,
        Circle,
        Diamond,
    }

    [SerializeField] private GameObject plus;
    [SerializeField] private GameObject minus;
    [SerializeField] private GameObject square;
    [SerializeField] private GameObject negSquare;
    [SerializeField] private GameObject circle;
    [SerializeField] private GameObject diamond;
    [SerializeField] private TextMeshProUGUI playerDebug;
    [SerializeField] private TextMeshProUGUI enemyDebug;
    private readonly float xCoord = -10.5f;
    private readonly float desktopDiceOffset = 1.0f;
    private readonly float mobileDiceOffset = 1.0f*1.5f;
    public float diceOffset;
    private readonly float desktopXOffset = 0.6f;
    private readonly float desktopHighlightOffset = 0.8333f;
    private readonly float mobileXOffset = 0.6f;
    private readonly float mobileHighlightOffset = 0.8333f*1.25f;
    public float xOffset;
    public float highlightOffset;
    private readonly float desktopButtonXCoord = -11.7f;
    private readonly float desktopButtonXOffset = -0.6f;
    private readonly float mobileButtonXCoord = -11.7f;
    private readonly float mobileButtonXOffset = -1f;
    private readonly Vector3 desktopButtonScale = new Vector3(1f, 1f, 1f);
    private readonly Vector3 mobileButtonScale = new Vector3(1.5f, 1.5f, 1f);
    private Vector3 buttonScale;
    private float buttonXCoord;
    private float buttonXOffset;
    private readonly float[] desktopYCoords = { 8.77f, 7.77f, 6.77f, 5.77f };
    private readonly float[] mobileYCoords = { 0.5f+8.77f, 0.5f+7.77f-0.5f*1, 0.5f+6.77f-0.5f*2, 0.5f+5.77f-0.5f*3 };
    public float[] yCoords;
    private readonly Vector2 baseDebugPos = new(-1.667f, 7.333f);
    private List<GameObject> existingStatSquares = new();
    public readonly Dictionary<string, List<Dice>> addedPlayerDice = new() {
        { "green", new List<Dice>() },
        { "blue", new List<Dice>() },
        { "red", new List<Dice>() },
        { "white", new List<Dice>() },
    };
    public readonly Dictionary<string, List<Dice>> addedEnemyDice = new() {
        { "green", new List<Dice>() },
        { "blue", new List<Dice>() },
        { "red", new List<Dice>() },
        { "white", new List<Dice>() },
    };
    public Dictionary<string, int> addedPlayerStamina = new() {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };

    public Dictionary<string, int> addedEnemyStamina = new() {
        { "green", 0 },
        { "blue", 0 },
        { "red", 0 },
        { "white", 0 },
    };
    private Scripts s;

    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        if (PlayerPrefs.GetString("debug") == "on") {
            playerDebug.color = Colors.disabled;
            enemyDebug.color = Colors.disabled;
        }
        else {
            playerDebug.color = Color.black;
            enemyDebug.color = Color.black;
        }
        // set the debug color if needed
        if (s.mobileMode) {
            diceOffset = mobileDiceOffset;
            xOffset = mobileXOffset;
            highlightOffset = mobileHighlightOffset;
            buttonXCoord = mobileButtonXCoord;
            buttonXOffset = mobileButtonXOffset;
            yCoords = mobileYCoords;
            buttonScale = mobileButtonScale;
        }
        else {
            diceOffset = desktopDiceOffset;
            xOffset = desktopXOffset;
            highlightOffset = desktopHighlightOffset;
            buttonXCoord = desktopButtonXCoord;
            buttonXOffset = desktopButtonXOffset;
            yCoords = desktopYCoords;
            buttonScale = desktopButtonScale;
        }
        SummonStaminaButtons();
        // create the stamina buttons
    }

    private void Update() {
        if (s == null || s.player == null || s.enemy == null || s.turnManager == null) { return; }
        if (PlayerPrefs.GetString(s.DEBUG_KEY) != "on") { return; }
        if (Input.GetKeyDown(KeyCode.S)) {
            Debug.Log(BuildDebugSnapshot());
        }
    }

    /// <summary>
    /// Summon the stats squares for the player.
    /// </summary>
    public void SummonStats() {
        foreach (GameObject stat in existingStatSquares) {
            Destroy(stat);
            // destroy all existing stats
        }
        existingStatSquares.Clear();
        // clear the array
        for (int i = 0; i < 4; i++) {
            GenerateForStat(i, Colors.colorNameArr[i]);
            // generate stat squares for every stat
        }
        SetDebugInformationFor("player");
        SetDebugInformationFor("enemy");
        // set the debug information for both
    }

    /// <summary>
    /// Gets the sum of the specified stat from the player or enemy. 
    /// </summary>
    public int SumOfStat(string stat, string playerOrEnemy) {
        if (stat != "green" && stat != "blue" && stat != "red" && stat != "white") {
            // make sure they are getting a valid stat
            Debug.LogError("Invalid stat to get the sum of");
            return 0;
        }
        if (playerOrEnemy == "player") {
            // get for player
            int sum = s.player.stats[stat] + s.player.potionStats[stat] + addedPlayerStamina[stat] + s.itemManager.neckletStats[stat] + s.itemManager.charmPassiveStats[stat] + s.itemManager.charmActiveBonus[stat] + s.itemManager.GetSacrificialChaliceAppliedBonus() + GetEncounterWeaponStatBonus(stat) + s.itemManager.GetLuckyDiceRoundStatBonus(stat);
            // get the sum of base stats + potion + stamina + necklet + charm
            foreach (Dice dice in addedPlayerDice[stat]) {
                // add to the sum all the added die
                if (dice != null) { sum += dice.GetComponent<Dice>().diceNum; }
            }
            return sum;
            // return the end
        }
        if (playerOrEnemy == "enemy") {
            // get for enemy, similar process to getting from player
            int sum = s.enemy.stats[stat] + addedEnemyStamina[stat];
            foreach (Dice dice in addedEnemyDice[stat]) {
                if (dice != null) { sum += dice.GetComponent<Dice>().diceNum; }
            }
            return sum;
        }
        Debug.LogError("Can only get the stats of a player or an enemy");
        return 0;
    }

    /// <summary>
    /// Gets the sum of the specified stat from the player or enemy, disregarding wounds and dice.
    /// </summary>
    public int RawSumOfStat(string stat, string playerOrEnemy) {
        if (stat != "green" && stat != "blue" && stat != "red" && stat != "white") {
            // make sure they are getting a valid stat
            Debug.LogError("Invalid stat to get the sum of");
            return 0;
        }
        if (playerOrEnemy == "player") {
              return GetPlayerDisplayedStatTotal(stat);
        }
        if (playerOrEnemy == "enemy") {
            return s.enemy.stats[stat] + addedEnemyStamina[stat];
        }
        Debug.LogError("Can only get the stats of a player or an enemy");
        return 0;
    }

    private int GetPlayerStatTotalWithoutAddedStamina(string stat) {
        return s.player.stats[stat]
            + s.player.potionStats[stat]
            + s.itemManager.neckletStats[stat]
            + s.itemManager.charmPassiveStats[stat]
            + s.itemManager.charmActiveBonus[stat]
            + s.itemManager.GetSacrificialChaliceAppliedBonus()
            + GetEncounterWeaponStatBonus(stat)
            + s.itemManager.GetLuckyDiceRoundStatBonus(stat);
    }

    private int GetPlayerDisplayedStatTotal(string stat) {
        return GetPlayerStatTotalWithoutAddedStamina(stat) + addedPlayerStamina[stat];
    }

    /// <summary>
    /// Instantiate the player's stamina buttons.
    /// </summary>
    private void SummonStaminaButtons() {
        for (int i = 0; i < 4; i++) {
            // 1 for every stat
            GameObject spawnedPlusButton = SpawnButton(plus, new Vector2(buttonXCoord, yCoords[i] - 0.01f));
            // - 0.01 is necessary because otherwise it has a weird visual glitch
            GameObject spawnedMinusButton = SpawnButton(minus, new Vector2(buttonXCoord + buttonXOffset, yCoords[i]));
            // create the buttons
            spawnedPlusButton.GetComponent<StaminaButton>().stat = Colors.colorNameArr[i];
            spawnedMinusButton.GetComponent<StaminaButton>().stat = Colors.colorNameArr[i];
            // assign stats to the buttons
            spawnedPlusButton.transform.localScale = buttonScale;
            spawnedMinusButton.transform.localScale = buttonScale;
        }
    }

    /// <summary>
    /// Generate the stat squares for the given stat.
    /// </summary>
    private void GenerateForStat(int i, string colorName) {
        // could use Colors.colorNameArr[i] instead of colorName but that takes up way more space and its much more confusing
        Color statColor = Colors.colorArr[Array.IndexOf(Colors.colorNameArr, colorName)];
        int encounterWeaponBonus = GetEncounterWeaponStatBonus(colorName);
        int encounterWeaponDiamondBonus = s.itemManager.GetCurrentPlayerWeaponDiamondBonus(colorName);
        int playerSquareStats = s.player.stats[colorName] + s.player.potionStats[colorName] + encounterWeaponBonus - encounterWeaponDiamondBonus;
        int playerCircleStats = s.itemManager.neckletStats[colorName];
        int playerDiamondStats = s.itemManager.charmPassiveStats[colorName] + s.itemManager.charmActiveBonus[colorName] + s.itemManager.GetSacrificialChaliceAppliedBonus() + encounterWeaponDiamondBonus + s.itemManager.GetLuckyDiceRoundStatBonus(colorName);
        int playerNonStaminaTotal = GetPlayerStatTotalWithoutAddedStamina(colorName);
        int playerTotal = GetPlayerDisplayedStatTotal(colorName);
        // get the color of the given colorname
        if (playerNonStaminaTotal > 0) {
            // if player's stats are greater than 0
            int visibleSquareStats = Mathf.Min(Mathf.Max(0, playerSquareStats), playerNonStaminaTotal);
            int remainingAfterSquares = playerNonStaminaTotal - visibleSquareStats;
            int visibleCircleStats = Mathf.Min(Mathf.Max(0, playerCircleStats), remainingAfterSquares);
            int remainingAfterCircles = remainingAfterSquares - visibleCircleStats;
            int visibleDiamondStats = Mathf.Min(Mathf.Max(0, playerDiamondStats), remainingAfterCircles);

            int k0;
            for (k0 = 0; k0 < visibleSquareStats; k0++) {
                SpawnGeneratedShape(i, statColor, k0, xCoord, xOffset, true, PlayerStatShape.Square);
            }
            // summon the positive stat squares at the proper place
            for (int k1 = 0; k1 < visibleCircleStats && k0 < playerNonStaminaTotal; k1++, k0++) {
                SpawnGeneratedShape(i, statColor, k0, xCoord, xOffset, true, PlayerStatShape.Circle);
            }
            for (int k2 = 0; k2 < visibleDiamondStats && k0 < playerNonStaminaTotal; k2++, k0++) {
                SpawnGeneratedShape(i, statColor, k0, xCoord, xOffset, true, PlayerStatShape.Diamond);
            }
        }
        else {
            // stats are less than 0
            for (int k = 0; k < Mathf.Max(0, -playerTotal); k++) {
                SpawnGeneratedShape(i, statColor, k, xCoord, xOffset, false);
            }
            // create negative stat squares
        }
        if (addedPlayerStamina[colorName] > 0 && playerTotal > 0) {
            // if player stamina is greater than 0 and total stats) are greater than 0
            if (playerNonStaminaTotal > 0) {
                // if player's total stats (without stamina) are greater than 0
                for (int j = playerNonStaminaTotal; j < playerTotal; j++) {
                    GameObject addedStaminaSquare = SpawnGeneratedShape(i, Colors.yellow, j, xCoord, xOffset, true);
                    Vector3 position = addedStaminaSquare.transform.position;
                    position = new Vector2(position.x - 0.01f, position.y);
                    addedStaminaSquare.transform.position = position;
                    // move it over a tiny bit
                    addedStaminaSquare.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    // make sure the sorting order is higher than that of other squares
                    // THIS IS THE EASY TO FIX A VISUAL GLITCH INVOLVING NECKLETS AND NEGATIVES, IT WORKS SO DON'T TOUCH IT
                }
                // make yellow squares in the correct places
            }
            else {
                // player's total stats w/o stamina are less than 0
                for (int j = 0; j < playerTotal; j++) {
                    SpawnGeneratedShape(i, Colors.yellow, j, xCoord, xOffset, true);
                }
                // make yellow squares in the correct place
            }
        }
        if (s.enemy.stats[colorName] + addedEnemyStamina[colorName] > 0) {
            for (int l = 0; l < s.enemy.stats[colorName]; l++) {
                SpawnGeneratedShape(i, statColor, l, -xCoord + 1, -xOffset, true);
            }
        }
        else {
            for (int l = 0; l < Mathf.Max(0, -(s.enemy.stats[colorName] + addedEnemyStamina[colorName])); l++) {
                SpawnGeneratedShape(i, statColor, l, -xCoord + 1, -xOffset, false);
            }
        }
        if (addedEnemyStamina[colorName] > 0 && s.enemy.stats[colorName] + addedEnemyStamina[colorName] > 0) {
            if (s.enemy.stats[colorName] > 0) {
                for (int n = s.enemy.stats[colorName]; n < s.enemy.stats[colorName] + addedEnemyStamina[colorName]; n++) {
                    SpawnGeneratedShape(i, Colors.yellow, n, -xCoord + 1, -xOffset, true);
                }
            }
            else {
                for (int n = 0; n < s.enemy.stats[colorName] + addedEnemyStamina[colorName]; n++) {
                    SpawnGeneratedShape(i, Colors.yellow, n, -xCoord + 1, -xOffset, true);
                }
            }
        }
        // enemy stat spawning is super similar to player
    }

    /// <summary>
    /// Spawn a generated shape with given information.
    /// </summary>
    /// <param name="i">The index (0-3) of the stat (green, blue...)</param>
    /// <param name="statColor">The color of which to give to the shape.</param>
    /// <param name="k">The number of stat squares generated for the stat (so offsets can be applied).</param>
    /// <param name="coord">The base x-coordinate at which to create the stats.</param>
    /// <param name="offset">The offset of which to apply between each square.</param>
    /// <param name="isPositive">true to make a positive square, false to make a negative square.</param>
    /// <param name="shapeType">the positive shape to create.</param>
    private GameObject SpawnGeneratedShape(int i, Color statColor, int k, float coord, float offset, bool isPositive, PlayerStatShape shapeType=PlayerStatShape.Square) {
        Vector3 instantiationsPos = new Vector2(coord + (k * offset), yCoords[i]);
        // set where the shape will be created
        GameObject spawnedShape;
        if (isPositive) {
            GameObject prefab = shapeType switch {
                PlayerStatShape.Circle => circle,
                PlayerStatShape.Diamond => diamond,
                _ => square,
            };
            spawnedShape = Instantiate(prefab, instantiationsPos, Quaternion.identity);
        }
        else { 
            if (instantiationsPos.x <= 0) { spawnedShape = Instantiate(negSquare, instantiationsPos, Quaternion.identity);  }
            else { 
                spawnedShape = Instantiate(negSquare, instantiationsPos, Quaternion.identity);  
                spawnedShape.GetComponent<SpriteRenderer>().flipX = true;
            }
            // depending on stat type and position, set the correct sprite.
        }
        spawnedShape.GetComponent<SpriteRenderer>().color = statColor;
        // give the die the correct player
        spawnedShape.transform.parent = transform;
        // move to correct position
        existingStatSquares.Add(spawnedShape);
        // add it to the array
        return spawnedShape;
    }

    /// <summary>
    /// Make a stamina +/- button.
    /// </summary>
    private GameObject SpawnButton(GameObject buttonType, Vector3 instantiationPos) {
        GameObject spawnedButton = Instantiate(buttonType, instantiationPos, Quaternion.identity);
        // create a button
        spawnedButton.transform.parent = transform;
        // child the button to this stat summoner
        return spawnedButton;
        // return the created button
    }

    /// <summary>
    /// Remove all attached die and stamina.
    /// </summary>
    public void ResetDiceAndStamina(bool refundEnemyPlannedStamina = false) {
        int refundedEnemyStamina = addedEnemyStamina.Values.Sum();
        bool livingLich = s != null
            && s.enemy != null
            && s.enemy.enemyName.text == "Lich"
            && !Save.game.enemyIsDead;

        if (refundEnemyPlannedStamina && livingLich) {
            s.enemy.stamina = s.enemy.lichStamina;
            s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
        }
        else if (refundEnemyPlannedStamina && refundedEnemyStamina > 0) {
            s.enemy.stamina += refundedEnemyStamina;
            s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
        }
        foreach (GameObject dice in s.diceSummoner.existingDice) {
            // fade out every die
            StartCoroutine(dice.GetComponent<Dice>().FadeOut());
        }
        foreach (string key in addedPlayerDice.Keys) { 
            // clear die and stamina from each stat
            addedPlayerDice[key].Clear();
            addedEnemyDice[key].Clear();
            addedPlayerStamina[key] = 0;
            addedEnemyStamina[key] = 0;
        }
        s.highlightCalculator.diceTakenByPlayer = 0;
        Save.game.playerStamina = s.player.stamina;
        Save.game.enemyStamina = s.enemy.stamina;
        s.player.staminaCounter.text = s.player.stamina.ToString();
        s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
        SetDebugInformationFor("player");
        SetDebugInformationFor("enemy");
        // set the debug information
    }

    /// <summary>
    /// Add a die to player's stat.
    /// </summary>
    public void AddDiceToPlayer(string addTo, Dice dice) {
        if (dice != null && !addedPlayerDice[addTo].Contains(dice)) {
            addedPlayerDice[addTo].Add(dice);
        }
    }

    /// <summary>
    /// Add a die to enemy's stat.
    /// </summary>
    public void AddDiceToEnemy(string addTo, Dice dice) {
        if (dice != null && !addedEnemyDice[addTo].Contains(dice)) {
            addedEnemyDice[addTo].Add(dice);
        }
    }

    /// <summary>
    /// Reposition all attached dice for the specified side.
    /// </summary>
    public void RepositionAllDice(string playerOrEnemy) {
        Dictionary<string, List<Dice>> diceDictionary = playerOrEnemy == "player"
            ? addedPlayerDice
            : addedEnemyDice;

        foreach (string stat in diceDictionary.Keys) {
            RepositionDice(playerOrEnemy, stat);
        }
    }

    /// <summary>
    /// Reposition all attached dice for a given stat on the specified side.
    /// </summary>
    public void RepositionDice(string playerOrEnemy, string stat) {
        Dictionary<string, List<Dice>> diceDictionary = playerOrEnemy == "player"
            ? addedPlayerDice
            : addedEnemyDice;

        if (!diceDictionary.TryGetValue(stat, out List<Dice> diceList)) { return; }

        CompactDiceList(diceList);

        int count = diceList.Count;
        float y = yCoords[Array.IndexOf(Colors.colorNameArr, stat)] - 0.01f;

        for (int i = 0; i < count; i++) {
            float x = playerOrEnemy == "player"
                ? OutermostPlayerX(stat) - (count * diceOffset) + (i * diceOffset)
                : OutermostEnemyX(stat) + ((count - 1 - i) * diceOffset);

            Vector2 position = new(x, y);
            diceList[i].transform.position = position;
            diceList[i].instantiationPos = position;
        }
    }

    /// <summary>
    /// Remove null and duplicate dice references while preserving order.
    /// </summary>
    private static void CompactDiceList(List<Dice> diceList) {
        List<Dice> compacted = diceList
            .Where(dice => dice != null)
            .Distinct()
            .ToList();

        if (compacted.Count == diceList.Count) { return; }

        diceList.Clear();
        diceList.AddRange(compacted);
    }

    /// <summary>
    /// Return the outermost player's x coordinate to add dice onto.
    /// </summary>
    public float OutermostPlayerX(string statType, string optionalDiceOffsetStatToMultiplyBy = null) {
        optionalDiceOffsetStatToMultiplyBy ??= statType;
        // not setting the optional variable will just default it to the base stat type
            return xCoord + ((Mathf.Abs(GetPlayerDisplayedStatTotal(statType)) - 1) * xOffset + highlightOffset + diceOffset * addedPlayerDice[optionalDiceOffsetStatToMultiplyBy].Count);
        // sum everything to get the offset
    }

    private int GetEncounterWeaponStatBonus(string statType) {
        if (s == null || s.itemManager == null) { return 0; }
        return s.itemManager.GetCurrentPlayerWeaponStatBonus(statType);
    }

    /// <summary>
    /// Get the outermost enemy's x coordinate to add dice onto.
    /// </summary>
    public float OutermostEnemyX(string statType) {
        int totalEnemyStat = s.enemy.stats[statType] + addedEnemyStamina[statType];
        return -xCoord + 1 + ((Mathf.Abs(totalEnemyStat) - 1) * -xOffset) - highlightOffset - diceOffset * (addedEnemyDice[statType].Count - 1);
        // similar to outermostplayerx
    }

    /// <summary>
    /// Set the debug information for player or enemy.
    /// </summary>
    public void SetDebugInformationFor(string playerOrEnemy) {
        if (s.tutorial == null) {
            if (playerOrEnemy == "player") {
                float furthest = (new[] { OutermostPlayerX("green"), OutermostPlayerX("blue"), OutermostPlayerX("red"), OutermostPlayerX("white") }).Max();
                playerDebug.transform.position = furthest >= -3.8 ? new Vector2(furthest + 1.333f, baseDebugPos.y) : new Vector2(baseDebugPos.x, baseDebugPos.y);
                // if the outermost position to too far, start moving the debug for plaer over
                playerDebug.text = "("+SumOfStat("green", "player")+")\n("+GetDebugBlueStat("player")+")\n("+SumOfStat("red", "player")+")\n("+SumOfStat("white", "player")+")";
            }
            else if (playerOrEnemy == "enemy") {
                enemyDebug.text = "("+SumOfStat("green", "enemy")+")\n("+GetDebugBlueStat("enemy")+")\n("+SumOfStat("red", "enemy")+")\n("+SumOfStat("white", "enemy")+")";
            }
            // ends up looking like
            // (2)
            // (2)
            // (1)
            // (2)
            // for example
            else { Debug.Log("error"); }
        }
    }

    private int GetDebugBlueStat(string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            return (s.enemy.woundList.Contains("knee") && s.enemy.enemyName.text != "Lich" || s.itemManager.PlayerHasWeapon("gauntlets")) ? 99 : SumOfStat("blue", "player");
        }

        if (playerOrEnemy == "enemy") {
            return s.player.woundList.Contains("knee")
                ? 99
                : SumOfStat("blue", "enemy");
        }

        Debug.LogError("Can only get debug blue stat of a player or an enemy");
        return 0;
    }

    /// <summary>
    /// Update debug text and queue a deferred enemy replan for real combat-state changes.
    /// </summary>
    public void SetCombatDebugInformationFor(string playerOrEnemy) {
        SetDebugInformationFor(playerOrEnemy);
        if (s == null || s.turnManager == null) { return; }
        s.turnManager.RefreshEnemyPlanIfNeeded();
    }

    /// <summary>
    /// Shift the dice of a given stat by a given amount.
    /// </summary>    
    public void ShiftDiceAccordingly(string stat, int shiftAmount) {
        int diceCount = addedPlayerDice[stat].Count;
        float currentX = xCoord + ((Mathf.Abs(GetPlayerDisplayedStatTotal(stat)) - 1) * xOffset + highlightOffset + diceOffset * diceCount);
        float nextX = xCoord + ((Mathf.Abs(GetPlayerDisplayedStatTotal(stat) + shiftAmount) - 1) * xOffset + highlightOffset + diceOffset * diceCount);
        float deltaX = nextX - currentX;

        foreach (Dice dice in addedPlayerDice[stat]) {
            // for every die in the specified stat
            Vector3 position = dice.transform.position;
            position = new Vector2(position.x + deltaX, position.y);
            dice.transform.position = position;
            // shift the die by the specified amount
            dice.instantiationPos = position;
            // update the instantiation position
        }
        SetDebugInformationFor("player");
    }

    private string BuildDebugSnapshot() {
        StringBuilder builder = new();
        builder.AppendLine("PLAYER: " + BuildStatSummary("player"));
        builder.AppendLine();
        builder.AppendLine(BuildDetailedStatLine("green", "player", "accuracy"));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(BuildDetailedStatLine("blue", "player", "speed"));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(BuildDetailedStatLine("red", "player", "damage"));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(BuildDetailedStatLine("white", "player", "parry"));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"stamina: {s.player.stamina} remaining");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"wounds: {BuildWoundsLine(s.player.woundList)}");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"targeting: {CleanTargetText(s.player.target.text)}");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"available dice: {BuildAvailableDiceLine()}");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"enemy: {s.enemy.enemyName.text}");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine("ENEMY: " + BuildStatSummary("enemy"));
        builder.AppendLine();
        builder.AppendLine(BuildDetailedStatLine("green", "enemy", "accuracy"));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(BuildDetailedStatLine("blue", "enemy", "speed"));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(BuildDetailedStatLine("red", "enemy", "damage"));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(BuildDetailedStatLine("white", "enemy", "parry"));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"stamina: {s.enemy.stamina} remaining");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"wounds: {BuildWoundsLine(s.enemy.woundList)}");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"targeting: {CleanTargetText(s.enemy.target.text)}");
        return builder.ToString();
    }

    private string BuildStatSummary(string playerOrEnemy) {
        return $"{SumOfStat("green", playerOrEnemy)} / {SumOfStat("blue", playerOrEnemy)} / {SumOfStat("red", playerOrEnemy)} / {SumOfStat("white", playerOrEnemy)}";
    }

    private string BuildDetailedStatLine(string stat, string playerOrEnemy, string label) {
        return $"{label}: {SumOfStat(stat, playerOrEnemy)} = {BuildStatBreakdown(stat, playerOrEnemy)}";
    }

    private string BuildStatBreakdown(string stat, string playerOrEnemy) {
        if (stat == "blue" && SumOfStat(stat, playerOrEnemy) == 99) {
            return "(always higher)";
        }

        List<string> parts = new();
        parts.Add(GetBaseStatWithoutAddedStamina(stat, playerOrEnemy).ToString());

        foreach (Dice die in GetAttachedDice(stat, playerOrEnemy)) {
            if (die != null) { parts.Add(FormatDie(die)); }
        }

        int addedStamina = GetAddedStamina(stat, playerOrEnemy);
        if (addedStamina > 0) { parts.Add($"{addedStamina} s"); }

        return "(" + string.Join(" + ", parts) + ")";
    }

    private int GetBaseStatWithoutAddedStamina(string stat, string playerOrEnemy) {
        if (playerOrEnemy == "player") {
            return GetPlayerStatTotalWithoutAddedStamina(stat);
        }

        return s.enemy.stats[stat];
    }

    private int GetAddedStamina(string stat, string playerOrEnemy) {
        return playerOrEnemy == "player"
            ? addedPlayerStamina[stat]
            : addedEnemyStamina[stat];
    }

    private IEnumerable<Dice> GetAttachedDice(string stat, string playerOrEnemy) {
        return playerOrEnemy == "player"
            ? addedPlayerDice[stat].Where(die => die != null)
            : addedEnemyDice[stat].Where(die => die != null);
    }

    private string BuildWoundsLine(IEnumerable<string> wounds) {
        List<string> woundList = wounds.Where(wound => !string.IsNullOrWhiteSpace(wound)).ToList();
        return woundList.Count == 0 ? "none" : string.Join(", ", woundList);
    }

    private string BuildAvailableDiceLine() {
        List<string> dice = s.diceSummoner.existingDice
            .Where(dieObject => dieObject != null)
            .Select(dieObject => dieObject.GetComponent<Dice>())
            .Where(die => die != null && !die.isAttached)
            .Select(FormatDie)
            .ToList();

        return dice.Count == 0 ? "none" : string.Join(", ", dice);
    }

    private string CleanTargetText(string targetText) {
        return string.IsNullOrWhiteSpace(targetText) ? "none" : targetText.TrimStart('*');
    }

    private string FormatDie(Dice die) {
        return ColorAbbreviation(die.diceType) + die.diceNum;
    }

    private string ColorAbbreviation(string colorName) {
        return colorName switch {
            "green" => "g",
            "blue" => "b",
            "red" => "r",
            "white" => "w",
            "yellow" => "y",
            _ => colorName.Length > 0 ? colorName.Substring(0, 1).ToLower() : "?"
        };
    }
}