using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class TurnManager : MonoBehaviour
{
    [SerializeField] public GameObject blackBox;
    public Vector3 onScreen = new Vector2(0.33f, 10f);
    public Vector3 offScreen = new Vector2(0.33f, 20f);
    private Coroutine coroutine = null;
    [SerializeField] public TextMeshProUGUI statusText;
    [SerializeField] public string[] targetArr = { "chest", "guts", "knee", "hip", "head", "hand", "armpits", "face" };
    [SerializeField] public string[] targetInfoArr = { "reroll any number of enemy's dice", "all enemy's dice suffer a penalty of -1", "your speed is always higher than enemy's", "enemy can't use stamina", "discard one of enemy's die", "enemy can't use white dice", "enemy can't use red dice", "instantaneous death" };
    private Scripts scripts;
    public string toMove;
    public bool isMoving = false;
    public bool actionsAvailable = false;
    public bool alterationDuringMove = false;
    public bool diceDiscarded = false;
    public bool scimitarParry = false;
    public GameObject dieSavedFromLastRound = null;
    public bool discardDieBecauseCourage = false;

    private void Start()
    {
        blackBox.transform.position = offScreen;
        scripts = FindObjectOfType<Scripts>();
        DisplayWounds();
        SetTargetOf("player");
        SetTargetOf("enemy");
        scripts.enemy.TargetBest();
        scripts.statSummoner.ResetDiceAndStamina();
        scripts.diceSummoner.SummonDice(true);
        scripts.statSummoner.SummonStats();
        DetermineMove(true);
    }

    public void DetermineMove(bool delay=false)
    {
        if (scripts.statSummoner.SumOfStat("blue", "player") < scripts.statSummoner.SumOfStat("blue", "enemy") && !(scripts.itemManager.PlayerHasWeapon("spear")))
        {
            StartCoroutine(EnemyMove(delay));
        }
    }

    public IEnumerator EnemyMove(bool delay, bool selectAllRemaining=false)
    {
        if (delay)
        {
            yield return scripts.delays[0.45f];
        }
        if (selectAllRemaining)
        {
            for (int i = 0; i < 3; i++)
            {
                scripts.enemy.ChooseBestDie();
            }
        }
        else { scripts.enemy.ChooseBestDie(); }
    }

    public void RecalculateMaxFor(string playerOrEnemy)
    {
        if (playerOrEnemy == "player")
        {
            SetAvailableTargetsOf(playerOrEnemy);
            if (scripts.player.targetIndex > scripts.player.availableTargets.Count)
            {
                scripts.player.targetIndex = scripts.player.availableTargets.Count;
            }
            SetTargetOf("player");
        }
        else if (playerOrEnemy == "enemy")
        {
            SetAvailableTargetsOf(playerOrEnemy);
            if (scripts.enemy.targetIndex > scripts.enemy.availableTargets.Count)
            {
                scripts.enemy.targetIndex = scripts.enemy.availableTargets.Count;
            }
            SetTargetOf("enemy");
        }
        else { Debug.LogError("Invalid string passed in to RecalculateMax() in TurnManager.cs"); }
    }

    public void DisplayWounds()
    {
        // add in something to fade in the wound text here
        if (scripts.player.woundList.Count > 0)
        {
            scripts.player.woundGUIElement.text = "";
            foreach (string wound in scripts.player.woundList)
            {
                scripts.player.woundGUIElement.text += ("*" + wound + "\n");
            }
        }
        else { scripts.player.woundGUIElement.text = "[no wounds]"; }
        if (scripts.enemy.woundList.Count > 0)
        {
            scripts.enemy.woundGUIElement.text = "";
            foreach (string wound in scripts.enemy.woundList)
            {
                scripts.enemy.woundGUIElement.text += ("*" + wound + "\n");
            }
        }
        else { scripts.enemy.woundGUIElement.text = "[no wounds]"; }
    }

    public IEnumerator InjuredTextChange(TextMeshProUGUI text)
    {
        yield return scripts.delays[0.55f];
        Color temp = text.color;
        temp.a = 0.5f;
        for (int i = 0; i < 20; i++)
        {
            yield return scripts.delays[0.01f];
            temp.a -= 0.025f;
            text.color = temp;
        }
        DisplayWounds();
        for (int i = 0; i < 40; i++)
        {
            yield return scripts.delays[0.005f];
            temp.a += 0.025f;
            text.color = temp;
        }
    }

    public void SetTargetOf(string playerOrEnemy)
    {
        if (playerOrEnemy == "player")
        {
            if (scripts.statSummoner.SumOfStat("green", "player") < 0) 
            {
                scripts.player.target.text = "none";
                scripts.player.targetInfo.text = "not enough accuracy to inflict any wound";
            }
            else
            {
                // if (scripts.statSummoner.SumOfStat("green", "player") == 0) { if (!scripts.player.availableTargets.Contains("chest")) { scripts.player.availableTargets.Add("chest"); } }
                if (scripts.enemy.woundList.Contains(targetArr[scripts.player.targetIndex])) { scripts.player.target.text = "*" + targetArr[scripts.player.targetIndex]; }
                else { scripts.player.target.text = targetArr[scripts.player.targetIndex]; }
                scripts.player.targetInfo.text = targetInfoArr[scripts.player.targetIndex];
            }
        }
        else if (playerOrEnemy == "enemy")
        {
            if (scripts.player.woundList.Contains("*")) { scripts.enemy.target.text = "*"+targetArr[scripts.enemy.targetIndex]; }
            else { scripts.enemy.target.text = targetArr[scripts.enemy.targetIndex]; }
        }
        else { Debug.LogError("Invalid string passed in to SetTarget() in TurnManager.cs"); }
    }

    public void SetAvailableTargetsOf(string playerOrEnemy)
    {
        int accuracy = scripts.statSummoner.SumOfStat("green", playerOrEnemy);
        if (accuracy > 7) { accuracy = 7; }
        if (playerOrEnemy == "player")
        {
            scripts.player.availableTargets = new List<string>();
            foreach (string targetingString in targetArr.Take(accuracy))
            {
                scripts.player.availableTargets.Add(targetingString);
            }
        }
        else if (playerOrEnemy == "enemy")
        {
            scripts.enemy.availableTargets = new List<string>();
            foreach (string targetingString in targetArr.Take(accuracy))
            {
                scripts.enemy.availableTargets.Add(targetingString);
            }
            
        }
        else { Debug.LogError("Invalid string passed in to SetAvailableTargetsOf() in TurnManager.cs"); }
    }

    public void ChangeStaminaOf(string playerOrEnemy, int amount)
    {
        if (playerOrEnemy == "player")
        {
            scripts.player.stamina += amount;
            scripts.player.staminaCounter.text = scripts.player.stamina.ToString();
            RecalculateMaxFor(playerOrEnemy);
        }
        else if (playerOrEnemy == "enemy")
        {
            scripts.enemy.stamina += amount;
            scripts.enemy.staminaCounter.text = scripts.enemy.stamina.ToString();
            RecalculateMaxFor(playerOrEnemy);
        }
        else { Debug.LogError("Invalid string passed in to ChangeStaminaAndUpdate() in TurnManager.cs"); }
    }

    public void RoundOne()
    {
        // targeting wounded body part
        scimitarParry = false;
        isMoving = true;
        List<Dice> availableDice = new List<Dice>();
        foreach (GameObject dice in scripts.diceSummoner.existingDice)
        {
            Dice diceScript = dice.GetComponent<Dice>();
            if (diceScript.isAttached == false)
            {
                availableDice.Add(diceScript);
            }
        }
        if (availableDice.Count == 0)
        {
            RunEnemyCalculations();
            if (scripts.player.woundList.Contains("head"))
            {
                scripts.enemy.DiscardBestPlayerDie();
            }
            InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
            if (playerSpd >= enemySpd)
            {
                if (!PlayerAttacks(playerAtt, enemyDef))
                {
                    StartCoroutine(RoundTwo("enemy"));
                }
                else
                {
                    SetTargetOf("player");
                    StartCoroutine(Kill("enemy"));
                    isMoving = false;
                }
            }
            else
            {
                scripts.player.isDodgy = false;
                scripts.player.SetPlayerStatusEffect("dodge", "off");
                if (!EnemyAttacks(enemyAtt, playerDef))
                {
                    StartCoroutine(RoundTwo("player"));
                }
                else
                {
                    StartCoroutine(Kill("player"));
                    isMoving = false;
                }
            }
        }
        else 
        { 
            isMoving = false;
            if (scripts.itemManager.PlayerHasWeapon("mace"))
            {
                foreach (Dice dice in from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached == false select a.GetComponent<Dice>())
                {
                    StartCoroutine(dice.RerollAnimation(false));
                }
            }
            else
            {
                SetStatusText("choose a die"); 
            }
        }
    }

    private IEnumerator RoundTwo(string toMove)
    {
        int playerAtt;
        int playerDef;
        int enemyAtt;
        int enemyDef;
        isMoving = true;
        yield return scripts.delays[2f];
        if (toMove == "player")
        {
            if (scimitarParry)
            {
                scripts.turnManager.SetStatusText("discard enemy's die");
                scripts.itemManager.discardableDieCounter++;
                actionsAvailable = true;
                for (float i = 2.5f; i > 0; i -= 0.1f)
                {
                    if (scripts.diceSummoner.breakOutOfScimitarParryLoop) { break; }
                    yield return scripts.delays[0.1f];
                }
                actionsAvailable = false;
                scimitarParry = false;
            }
            playerAtt = scripts.statSummoner.SumOfStat("red", "player");
            enemyDef = scripts.statSummoner.SumOfStat("white", "enemy");
            if (PlayerAttacks(playerAtt, enemyDef))
            {
                SetTargetOf("player");
                StartCoroutine(Kill("enemy"));
                isMoving = false;
            }
        }
        else if (toMove == "enemy")
        {
            if ((scripts.enemy.woundList.Contains("chest") && Rerollable() || scripts.enemy.woundList.Contains("head") && !diceDiscarded) && PlayerPrefs.GetString("hints") == "on")
            {
                if (scripts.enemy.woundList.Contains("head") && !diceDiscarded) { SetStatusText("note: you can discard enemy's die"); }
                else if (scripts.enemy.woundList.Contains("chest") && Rerollable()) { SetStatusText("note: you can reroll enemy's dice"); }
                actionsAvailable = true;
                for (float i = 2.5f; i > 0; i -= 0.1f)
                {
                    if (alterationDuringMove)
                    {
                        i += 0.75f;
                        alterationDuringMove = false;
                    }
                    yield return scripts.delays[0.1f];
                }
                actionsAvailable = false;
                diceDiscarded = false;
            }
            enemyAtt = scripts.statSummoner.SumOfStat("red", "enemy");
            playerDef = scripts.statSummoner.SumOfStat("white", "player");
            if (EnemyAttacks(enemyAtt, playerDef))
            {
                StartCoroutine(Kill("player"));
                isMoving = false;
            }
        }
        else { print("error passing into ienumerator attack"); }
        if (!scripts.player.isDead && !scripts.enemy.isDead)
        { 
            yield return scripts.delays[2f];
            if (scripts.player.isCourageous) 
            {
                actionsAvailable = true;
                discardDieBecauseCourage = true;
                scripts.turnManager.SetStatusText("discard all your dice, except one");
                for (int i = 0; i < 50; i++)
                {
                    yield return scripts.delays[0.1f];
                    if ((from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached && a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList().Count <= 1)
                    {
                        dieSavedFromLastRound = (from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList()[0];
                        try { } catch {}
                        break;
                    }
                }
                if (dieSavedFromLastRound == null && scripts.diceSummoner.existingDice.Count > 0)
                {
                    // player has not discarded enough dice, so choose a random one
                    dieSavedFromLastRound = (from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList()[UnityEngine.Random.Range(0, (from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isOnPlayerOrEnemy == "player" select a).ToList().Count)];
                }
                actionsAvailable = false;
                discardDieBecauseCourage = false;
                scripts.player.isCourageous = false;
                scripts.player.SetPlayerStatusEffect("courage", "off");
            }
            isMoving = false;
            scripts.statSummoner.ResetDiceAndStamina();
            scripts.diceSummoner.SummonDice(false);
            scripts.statSummoner.SummonStats();
            // keep the setdebugs!!!
            RecalculateMaxFor("player");
            RecalculateMaxFor("enemy");
            DetermineMove(true);
        }
        else
        {
            isMoving = false;
        }
        yield return scripts.delays[0.45f];
        scripts.player.isFurious = false;
        scripts.player.SetPlayerStatusEffect("fury", "off");
        scripts.player.isDodgy = false;
        scripts.player.SetPlayerStatusEffect("dodge", "off");
        scripts.player.isBloodthirsty = false;
        scripts.player.SetPlayerStatusEffect("leech", "off");
        scripts.highlightCalculator.diceTakenByPlayer = 0;
        scripts.itemManager.discardableDieCounter = 0;
        scripts.itemManager.usedAnkh = false;
        scripts.itemManager.usedBoots = false;
        scripts.itemManager.usedHelm = false;
        scripts.diceSummoner.breakOutOfScimitarParryLoop = false;
    }
    
    public void ClearPotionStats()
    {
        foreach (string key in scripts.itemManager.statArr)
        {
            scripts.player.potionStats[key] = 0;
        }
    }

    private IEnumerator Kill(string playerOrEnemy)
    {
        if (playerOrEnemy == "player") { scripts.player.isDead = true; }
        else if (playerOrEnemy == "enemy") { scripts.enemy.isDead = true; }
        yield return scripts.delays[0.55f];
        if (playerOrEnemy == "player")
        {
            scripts.turnManager.SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... you die");
            StartCoroutine(PlayDeathAnimation("player"));
            
        }
        else if (playerOrEnemy == "enemy")
        {
            scripts.turnManager.SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... he dies");
            StartCoroutine(PlayDeathAnimation("enemy"));
        }
        else { print("invalid string passed"); }
    }

    public IEnumerator PlayHitAnimation(string playerOrEnemy)
    {
        SpriteRenderer spriteRenderer = null;
        if (playerOrEnemy == "player")
        {
            spriteRenderer = scripts.player.GetComponent<SpriteRenderer>();
        }
        else if (playerOrEnemy == "enemy")
        {
            spriteRenderer = scripts.enemy.GetComponent<SpriteRenderer>();
        }
        else { print("bad"); }
        Color temp = Color.white;
        temp.a = 0.5f;
        for (int i = 0; i < 14; i++)
        {
            yield return scripts.delays[0.0125f];
            temp.a -= 1f / 28f;
            spriteRenderer.color = temp;
        }
        for (int i = 0; i < 28; i++)
        {
            yield return scripts.delays[0.0125f];
            temp.a += 1f / 28f;
            temp.a += 1f / 28f;
            spriteRenderer.color = temp;
        }
    }

    public IEnumerator PlayDeathAnimation(string playerOrEnemy)
    {
        SpriteRenderer spriteRenderer = null;
        if (playerOrEnemy == "player")
        {
            spriteRenderer = scripts.player.GetComponent<SpriteRenderer>();
        }
        else if (playerOrEnemy == "enemy")
        {
            spriteRenderer = scripts.enemy.GetComponent<SpriteRenderer>();
        }
        else { print("invalid string passed"); }
        Color temp = Color.white;
        temp.a = 0.5f;
        for (int i = 0; i < 14; i++)
        {
            yield return scripts.delays[0.0125f];
            temp.a -= 1f / 28f;
            spriteRenderer.color = temp;
        }
        if (playerOrEnemy == "player")
        {
            scripts.player.GetComponent<Animator>().enabled = false;
        }
        else if (playerOrEnemy == "enemy")
        {
            scripts.enemy.GetComponent<Animator>().enabled = false;
        }
        for (int i = 0; i < 28; i++)
        {
            yield return scripts.delays[0.0125f];
            temp.a += 1f / 28f;
            spriteRenderer.color = temp;
        }
        yield return scripts.delays[0.8f];
        scripts.soundManager.PlayClip("death");
        if (scripts.itemManager.PlayerHasWeapon("rapier"))
        {
            ChangeStaminaOf("player", 3);
        }
        if (playerOrEnemy == "player")
        {
            scripts.player.GetComponent<SpriteRenderer>().sprite = scripts.player.GetDeathSprite();
            scripts.player.SetPlayerPositionAfterDeath();
        }
        else if (playerOrEnemy == "enemy")
        {
            scripts.enemy.GetComponent<SpriteRenderer>().sprite = scripts.enemy.GetDeathSprite();
            scripts.enemy.SetEnemyPositionAfterDeath();
            scripts.itemManager.SpawnItems();
            blackBox.transform.position = onScreen;
        }
        else { print("invalid string passed"); }
        foreach (GameObject dice in scripts.diceSummoner.existingDice)
        {
            StartCoroutine(dice.GetComponent<Dice>().FadeOut(false, true));
        }
        scripts.statSummoner.ResetDiceAndStamina();
        foreach (GameObject item in scripts.player.inventory)
        {
            
        }
        ClearPotionStats();
        scripts.statSummoner.SummonStats();
        scripts.statSummoner.SetDebugInformationFor("player");
        RecalculateMaxFor("player");
        RecalculateMaxFor("enemy");
    }

    public IEnumerator StatusTextCoroutine(string text)
    {
        Color temp = statusText.color;
        temp.a = 0f;
        statusText.text = text;
        for (int i = 0; i < 10; i++)
        {
            yield return scripts.delays[0.033f];
            temp.a += 0.1f;
            statusText.color = temp;
        }
        yield return scripts.delays[1f];
        for (int i = 0; i < 10; i++)
        {
            yield return scripts.delays[0.033f];
            temp.a -= 0.1f;
            statusText.color = temp;
        }
        statusText.text = "";
    }

    public void SetStatusText(string text) 
    {
        try { StopCoroutine(coroutine); } catch {}
        coroutine = StartCoroutine(StatusTextCoroutine(text));
    }

    public IEnumerator DoStuffForAttack(string hitOrParry, string playerOrEnemy, bool showAnimation=true, bool armor=false)
    {
        yield return scripts.delays[0.55f];
        if (hitOrParry == "hit")
        {
            if (scripts.player.isDodgy && playerOrEnemy == "player")
            {
                // play whoosh
            }
            else
            {
                scripts.soundManager.PlayClip("hit");
                if (showAnimation)
                {
                    if (!(playerOrEnemy == "player" && armor)) 
                    { 
                        StartCoroutine(PlayHitAnimation(playerOrEnemy)); 
                    }
                }
            }
        }
        else if (hitOrParry == "parry")
        {
            scripts.soundManager.PlayClip("parry");
        }
        else { Debug.LogError("invalid string passed"); }
    }

    private bool EnemyAttacks(int enemyAtt, int playerDef)
    {
        bool armor = false;
        scripts.soundManager.PlayClip("swing");
        if (enemyAtt > playerDef)
        {
            if (scripts.player.isDodgy)
                {
                    SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... you dodge");
                }

            else
            {
                if (scripts.itemManager.PlayerHas("armour"))
                {
                    // play shatter
                    armor = true;
                    SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... your armour shatters");
                    scripts.itemManager.Select(scripts.player.inventory, (from a in scripts.player.inventory select a.GetComponent<SpriteRenderer>().sprite.name).ToList().IndexOf("armour"));
                    scripts.itemManager.GetPlayerItem("armour").GetComponent<Item>().Remove();
                    scripts.itemManager.Select(scripts.player.inventory, 0);
                }
                else
                {
                    if (scripts.enemy.target.text != "face")
                    {
                        if (scripts.enemy.target.text.Contains("*"))
                        {
                            SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you, damaging {scripts.enemy.target.text.Substring(1)}!");
                        }
                        else
                        {
                            if (scripts.player.woundList.Count != 2)
                            {
                                SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you, damaging {scripts.enemy.target.text}!");
                            }
                        }
                    }
                }
            }
            StartCoroutine(DoStuffForAttack("hit", "player", true, armor));
            if (!(scripts.player.woundList.Contains(scripts.enemy.target.text) || scripts.player.woundList.Contains(scripts.enemy.target.text.Substring(1)) || armor || scripts.player.isDodgy))
            {
                scripts.player.woundList.Add(scripts.enemy.target.text);
                StartCoroutine(InjuredTextChange(scripts.player.woundGUIElement));
                RecalculateMaxFor("player");
                return InstantlyApplyInjuries(scripts.enemy.target.text, "player");
            }
        }
        else
        {
            StartCoroutine(DoStuffForAttack("parry", "player"));
            if (enemyAtt < 0) { SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... the attack is to weak"); }
            else 
            { 
                scimitarParry = true;
                SetStatusText($"{scripts.enemy.enemyName.text.ToLower()} hits you... you parry"); 
            }
        }
        return false;
    }

    private bool PlayerAttacks(int playerAtt, int enemyDef)
    {
        scripts.soundManager.PlayClip("swing");
        if (playerAtt > enemyDef)
        {
            if (scripts.statSummoner.SumOfStat("green", "player") < 0)
            {
                SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... you miss");
                // play missed atk sound
            }
            else
            {
                if (scripts.player.target.text == "face" || (scripts.enemy.woundList.Count == 2 && !scripts.player.target.text.Contains("*")))
                {
                    StartCoroutine(DoStuffForAttack("hit", "enemy", false));
                }
                else
                {
                    StartCoroutine(DoStuffForAttack("hit", "enemy"));
                    if (scripts.player.target.text.Contains("*"))
                    {
                        if (scripts.itemManager.PlayerHasWeapon("maul")) {}
                        else
                        {
                            SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}, damaging {scripts.player.target.text.Substring(1)}!");
                        }
                    }
                    else
                    {
                        if (scripts.itemManager.PlayerHasWeapon("maul")) {}
                        else
                        {
                            SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}, damaging {scripts.player.target.text}!");
                        }
                    }
                }
                if (!scripts.player.target.text.Contains("*") && scripts.statSummoner.SumOfStat("green", "player") >= 0)
                {
                    scripts.enemy.woundList.Add(scripts.player.target.text);
                    if (scripts.player.isBloodthirsty)
                    {
                        try { scripts.player.woundList.Remove(scripts.player.target.text); } catch { print("no need to heal"); }
                        StartCoroutine(InjuredTextChange(scripts.player.woundGUIElement));
                        scripts.player.isBloodthirsty = false;
                        scripts.player.SetPlayerStatusEffect("leech", "off");
                    }
                    StartCoroutine(InjuredTextChange(scripts.enemy.woundGUIElement));
                    RecalculateMaxFor("enemy");
                    return InstantlyApplyInjuries(scripts.player.target.text, "enemy");
                }
                if (scripts.itemManager.PlayerHasWeapon("maul")) { return true; }
            }
        }
        else
        {
            StartCoroutine(DoStuffForAttack("parry", "enemy"));
            if (playerAtt < 0) { SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... the attack is too weak"); }
            else if (scripts.statSummoner.SumOfStat("green", "player") < 0)
            {
                SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... you miss");
                // play missed atk sound
            }
            else 
            { SetStatusText($"you hit {scripts.enemy.enemyName.text.ToLower()}... he parries"); }
        }
        return false;
    }

    private bool InstantlyApplyInjuries(string injury, string appliedTo)
    {
        if (appliedTo != "enemy" && appliedTo != "player") { print("invalid string passed into param. appliedTo in InstantlyApplyInjuries"); }
        if (scripts.itemManager.PlayerHasWeapon("maul") && appliedTo == "enemy") { return true; }
        if (injury == "guts")
        {
            if (appliedTo == "player")
            {
                foreach (string key in scripts.statSummoner.addedPlayerDice.Keys)
                {
                    foreach (Dice dice in scripts.statSummoner.addedPlayerDice[key])
                    {
                        StartCoroutine(dice.DecreaseDiceValue());
                    }
                }
                RecalculateMaxFor("player");
            }
            else if (appliedTo == "enemy")
            {
                foreach (string key in scripts.statSummoner.addedEnemyDice.Keys)
                {
                    foreach (Dice dice in scripts.statSummoner.addedEnemyDice[key])
                    {
                        StartCoroutine(dice.DecreaseDiceValue());
                    }
                }
                RecalculateMaxFor("enemy");
            }
        }
        else if (injury == "hip")
        {
            scripts.statSummoner.addedPlayerStamina = new Dictionary<string, int>()
            {
                { "green", 0 },
                { "blue", 0 },
                { "red", 0 },
                { "white", 0 },
            };
        }
        else if (injury == "hand")
        {
            if (appliedTo == "player")
            {
                StartCoroutine(RemoveDice("white", "player"));
            }
            else if (appliedTo == "enemy")
            {
                StartCoroutine(RemoveDice("white", "enemy"));
            }
        }
        else if (injury == "armpits")
        {
            if (appliedTo == "player")
            {
                StartCoroutine(RemoveDice("red", "player"));
            }
            else if (appliedTo == "enemy")
            {
                StartCoroutine(RemoveDice("red", "enemy"));
            }
        }
        else if (injury == "face")
        {
            return true;
        }
        scripts.statSummoner.SetDebugInformationFor("player");
        scripts.statSummoner.SetDebugInformationFor("enemy");
        if (appliedTo == "player" && scripts.player.woundList.Count == 3) { return true; }
        else if (appliedTo == "enemy" && scripts.enemy.woundList.Count == 3) { return true; }
        return false;
    }

    private IEnumerator RemoveDice(string diceType, string removeFrom)
    {
        yield return scripts.delays[1f];
        foreach (Dice dice in scripts.statSummoner.addedPlayerDice["red"])
        {
            if (dice.diceType == diceType)
            {
                scripts.diceSummoner.existingDice.Remove(dice.gameObject);
                Destroy(dice.gameObject);
            }
        }
        if (removeFrom == "player")
        {
            for (int k = 0; k < scripts.statSummoner.addedPlayerDice[diceType].Count; k++)
            {
                if (scripts.statSummoner.addedPlayerDice[diceType][k].diceType == diceType)
                {
                    int index = scripts.statSummoner.addedPlayerDice[scripts.statSummoner.addedPlayerDice[diceType][k].statAddedTo].IndexOf(scripts.statSummoner.addedPlayerDice[diceType][k]);     
                    if (index != -1 && index != scripts.statSummoner.addedPlayerDice[scripts.statSummoner.addedPlayerDice[diceType][k].statAddedTo].Count - 1)
                    {
                        scripts.statSummoner.addedPlayerDice[diceType].Remove(scripts.statSummoner.addedPlayerDice[diceType][k]);
                        List<Dice> diceList = scripts.statSummoner.addedPlayerDice[scripts.statSummoner.addedPlayerDice[diceType][k].statAddedTo];
                        for (int i = index; i < diceList.Count; i++)
                        {
                            diceList[i].transform.position = new Vector2(diceList[i].transform.position.x - scripts.statSummoner.diceOffset, diceList[i].transform.position.y);
                            diceList[i].GetComponent<Dice>().instantiationPos = diceList[i].transform.position;
                        }
                        if (diceList.Count > 0)
                        {
                            Vector3 lastDicePos = diceList[diceList.Count - 1].transform.position;
                            scripts.statSummoner.addedPlayerDice[diceType][k].instantiationPos = new Vector2(lastDicePos.x + scripts.statSummoner.diceOffset, lastDicePos.y);
                        }
                    }
                }
            }
        }
        else if (removeFrom == "enemy")
        {

        }
        else
        {
            Debug.LogError("error in passing in to RemoveDice");
        }
    }

    private void RunEnemyCalculations()
    {
        if (!scripts.enemy.woundList.Contains("hip"))
        {
            InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef);
            if (enemyAtt <= playerDef && enemyAtt + scripts.enemy.stamina > playerDef)
            {
                // if enemy can hit player
                // add stamina to hit player
                UseEnemyStaminaOn("red", (playerDef - enemyAtt) + 1);
                if (playerSpd >= enemySpd && playerAtt > enemyDef)
                {
                    // if player will attack first
                    if (enemySpd + scripts.enemy.stamina > playerSpd)
                    {
                        // if enemy can attack first
                        // add speed to go first
                        UseEnemyStaminaOn("blue", (playerSpd - enemySpd) + 1);
                    }
                }
                if (enemyAim < 7 && enemyAim + scripts.enemy.stamina > 6)
                {
                    // if enemy can target face
                    // add accuracy to target face
                    UseEnemyStaminaOn("green", 7 - enemyAim);
                    scripts.enemy.TargetBest();
                }
            }
            if (enemyDef < playerAtt && enemyDef + scripts.enemy.stamina >= playerAtt)
            {
                // if enemy will be hit and can defend
                // add stamina to defend
                UseEnemyStaminaOn("white", playerAtt - enemyDef);
                // TO DO: check for gut/hip/chest strike and take actions correspondingly
            }
            scripts.statSummoner.SetDebugInformationFor("enemy");
        }
    }

    private void UseEnemyStaminaOn(string stat, int amount)
    {
        if (scripts.enemy.stamina < amount) { print("too much stamina to use!"); }
        else 
        {
            scripts.statSummoner.addedEnemyStamina[stat] += amount;
            scripts.turnManager.ChangeStaminaOf("enemy", -amount);
            scripts.statSummoner.SummonStats();
            foreach (Dice dice in scripts.statSummoner.addedEnemyDice[stat])
            {
                dice.transform.position = new Vector2(dice.transform.position.x - scripts.statSummoner.xOffset * amount, dice.transform.position.y);
                dice.instantiationPos = dice.transform.position;
            }
        }
    }

    private void InitializeVariables(out int playerAim, out int enemyAim, out int playerSpd, out int enemySpd, out int playerAtt, out int enemyAtt, out int playerDef, out int enemyDef)
    {
        playerAim = scripts.statSummoner.SumOfStat("green", "player");
        enemyAim = scripts.statSummoner.SumOfStat("green", "enemy");
        playerSpd = scripts.statSummoner.SumOfStat("blue", "player");
        enemySpd = scripts.statSummoner.SumOfStat("blue", "enemy");
        playerAtt = scripts.statSummoner.SumOfStat("red", "player");
        enemyAtt = scripts.statSummoner.SumOfStat("red", "enemy");
        playerDef = scripts.statSummoner.SumOfStat("white", "player");
        enemyDef = scripts.statSummoner.SumOfStat("white", "enemy");
    }

    private bool Rerollable()
    {
        foreach (string key in scripts.statSummoner.addedPlayerDice.Keys)
        {
            foreach (Dice dice in scripts.statSummoner.addedPlayerDice[key])
            {
                if (!dice.isRerolled)
                {
                    return true;
                }
            }
        }
        return false;
    }
}