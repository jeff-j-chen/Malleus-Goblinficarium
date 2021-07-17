using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class Item : MonoBehaviour {
    [SerializeField] public Scripts scripts;
    [SerializeField] public string itemName;
    [SerializeField] public string itemType;
    [SerializeField] public string modifier;
    [SerializeField] public Dictionary<string, int> weaponStats = new();
    private bool preventPlayingFX = true;

    void Awake() {
        scripts = FindObjectOfType<Scripts>();
    }

    private IEnumerator AllowFX() {
        yield return new WaitForSeconds(0.45f);
        preventPlayingFX = false;
    }

    void Start() {
        gameObject.name = itemName;
        // set the object's name to its itemname, so it can be identified in the editor
        if (itemName == "torch" && scripts.player != null) {
            // if the item is a torch
            if (Random.Range(0, 2) == 0) {
                // 1/2 chance
                if (scripts.levelManager.sub != 4) { modifier = $"{scripts.levelManager.level + 1}-{scripts.levelManager.sub}"; }
                else { modifier = $"{scripts.levelManager.level + 1}-1"; }
                // set fade time to be default
            }
            else {
                // 1/2 chance
                if (scripts.levelManager.sub + 1 == 4 || scripts.levelManager.sub + 1 == 5) { modifier = $"{scripts.levelManager.level + 1}-2"; }
                else { modifier = $"{scripts.levelManager.level + 1}-{scripts.levelManager.sub + 1}"; }
                // set fade time to be slightly longer
            }
        }
        StartCoroutine(AllowFX());
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            // left click
            if (scripts.itemManager.highlightedItem == gameObject) { Use(); }
            // if clicking over highlighted weapon, use it
            else {
                Select();
                // otherwise select it
                if (scripts.player != null) {
                    // in game
                    if (scripts.player.inventory.Contains(gameObject)) { scripts.itemManager.curList = scripts.player.inventory; }
                    // selection occured in inventory, so assign the curlist variable as such
                    else { scripts.itemManager.curList = scripts.itemManager.floorItems; }
                    // selection was on floor so         "  "               
                }
                else {
                    // in character select 
                    scripts.itemManager.curList = scripts.itemManager.floorItems;
                }
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            // print("right clicked, does player have kapala? " + scripts.itemManager.PlayerHas("kapala"));
            // right click
            if (scripts.itemManager.highlightedItem == gameObject) {
                // if highlighted
                if (itemType != "weapon" && !scripts.itemManager.floorItems.Contains(gameObject)) {
                    // if the item is not weapon and not on the floor
                    if (scripts.levelManager.sub == 4 || scripts.enemy.isDead || scripts.enemy.enemyName.text == "Tombstone" || scripts.itemManager.PlayerHas("kapala") || (scripts.tutorial != null && modifier == "nothing" && scripts.tutorial.curIndex == 2 && !scripts.tutorial.isAnimating)) {
                        // only allow dropping of items if player is trading, enemy is dead, on a tombstone, are offering to kapala, or are dropping scroll in tutorial
                        Remove(true);
                    }
                }
            }
            // could combine these but it's beyond hideous and unreadable
        }
    }

    /// <summary>
    /// Hide an item, preventing it from being seen.
    /// </summary>
    public void Hide() { transform.localScale = new Vector2(0, 0); }

    /// <summary>
    /// Show an item.
    /// </summary>
    public void UnHide() { transform.localScale = new Vector2(1, 1); }

    /// <summary>
    /// Select an item.
    /// </summary>
    public void Select(bool playAudio = true) {
        if (itemType == "weapon") {
            // if the item is a weapon
            if (scripts.itemManager.descriptionDict[itemName.Split(' ')[1]] == "") { scripts.itemManager.itemDesc.text = itemName; }
            // if no description, just display the itemname
            else { scripts.itemManager.itemDesc.text = $"{itemName}\n- {scripts.itemManager.descriptionDict[itemName.Split(' ')[1]]}"; }
            // if description, then display it
            if (scripts.itemManager.floorItems.Contains(gameObject) && scripts.player != null) {
                // if item on the floor and not in character select
                scripts.enemy.stats = weaponStats;
                scripts.statSummoner.SummonStats();
                scripts.statSummoner.SetDebugInformationFor("enemy");
                scripts.turnManager.blackBox.transform.position = scripts.turnManager.offScreen;
                // display the stats for the player to see
            }
            else {
                // if item is in inventory
                if (scripts.enemy != null) {
                    // and not in character select
                    if (scripts.enemy.isDead) { scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen; }
                    // hide the weapon stats if enemy is dead and not clicking on an enemy
                }
            }
        }
        else {
            if (scripts.levelManager == null || !scripts.levelManager.lockActions) {
                // only allow weapons to be selected when locked
                if (scripts.levelManager != null && scripts.enemy.isDead || scripts.levelManager != null && scripts.enemy.enemyName.text == "Tombstone") { scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen; }
                // hide the weapon stats if enemy is dead and not clicking on an enemy
                switch (itemName) {
                    case "potion":
                        switch (modifier) {
                            case "accuracy": scripts.itemManager.itemDesc.text = "potion of accuracy\n+3 accuracy"; break;
                            case "speed": scripts.itemManager.itemDesc.text = "potion of speed\n+3 speed"; break;
                            case "strength": scripts.itemManager.itemDesc.text = "potion of strength\n+3 damage"; break;
                            case "defense": scripts.itemManager.itemDesc.text = "potion of parry\n+3 parry"; break;
                            case "might": scripts.itemManager.itemDesc.text = "potion of might\ngain a yellow die"; break;
                            case "life": scripts.itemManager.itemDesc.text = "potion of life\nheal all wounds"; break;
                            case "nothing": scripts.itemManager.itemDesc.text = "potion of nothing\ndoes nothing"; break;
                            // default: print("invalid potion modifier"); break;
                        }
                        break;
                    case "scroll":
                        switch (modifier) {
                            case "fury": scripts.itemManager.itemDesc.text = "scroll of fury\nall picked dice turn yellow"; break;
                            case "haste": scripts.itemManager.itemDesc.text = "scroll of haste\npick 3 dice, enemy gets the rest"; break;
                            case "dodge": scripts.itemManager.itemDesc.text = "scroll of dodge\nif you strike first, ignore all damage"; break;
                            case "leech": scripts.itemManager.itemDesc.text = "scroll of leech\ncure the same wound as inflicted"; break;
                            case "courage": scripts.itemManager.itemDesc.text = "scroll of courage\nkeep 1 of your die till next round"; break;
                            case "challenge": scripts.itemManager.itemDesc.text = "scroll of challenge\n???"; break;
                            case "nothing": scripts.itemManager.itemDesc.text = "scroll of nothing\ndoes nothing"; break;
                            // default: print("invalid scroll modifier"); break;
                        }
                        break;
                    case "necklet":
                        switch (modifier) {
                            case "arcane": scripts.itemManager.itemDesc.text = "arcane necklet\nall necklets are more effective"; break;
                            case "nothing": scripts.itemManager.itemDesc.text = "necklet of nothing\ndoes nothing"; break;
                            case "victory": scripts.itemManager.itemDesc.text = "necklet of victory\nthe victory is in your hands!.."; break;
                            default: scripts.itemManager.itemDesc.text = $"necklet of {modifier}\n+{scripts.itemManager.neckletCounter["arcane"]} {scripts.itemManager.statArr1[Array.IndexOf(scripts.itemManager.neckletTypes, modifier)]}"; break;
                        }
                        break;
                    case "cheese":
                    case "steak":
                        if (scripts.player == null || scripts.player.charNum == 0) { scripts.itemManager.itemDesc.text = $"{itemName}\n+{int.Parse(scripts.itemManager.descriptionDict[itemName]) + 2} stamina"; }
                        else { scripts.itemManager.itemDesc.text = $"{itemName}\n+{scripts.itemManager.descriptionDict[itemName]} stamina"; }
                        break;
                    case "moldy cheese":
                        scripts.itemManager.itemDesc.text = "moldy cheese\n+0 stamina"; break;
                    case "rotten steak":
                        scripts.itemManager.itemDesc.text = "rotten steak\n+0 stamina"; break;
                    case "arrow":
                        if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) {
                            scripts.itemManager.itemDesc.text = "leave dungeon";
                        }
                        else { scripts.itemManager.itemDesc.text = scripts.itemManager.descriptionDict[itemName]; }
                        break;
                    case "retry":
                        scripts.itemManager.itemDesc.text = scripts.itemManager.descriptionDict[itemName]; break;
                    default:
                        scripts.itemManager.itemDesc.text = $"{itemName}\n{scripts.itemManager.descriptionDict[itemName]}"; break;
                }
                // set the proper item descriptions based on its name
            }
        }
        if (scripts.levelManager == null || itemType == "weapon" ||
            scripts.levelManager != null && !scripts.levelManager.lockActions) {
            // only allow weapons to be used when unlocked
            scripts.itemManager.highlight.transform.position = transform.position;
            // move the highlight to the selected item
            GameObject curItem = gameObject;
            scripts.itemManager.highlightedItem = curItem;
            // update the highlighted item variable
            // update the col variable
            scripts.itemManager.col = scripts.itemManager.curList.IndexOf(curItem);
            if (playAudio && !preventPlayingFX) { scripts.soundManager.PlayClip("click0"); }
            // dont play this if we just came from menu scene
            // play sound clip
        }
    }

    /// <summary>
    /// use an item, picking it up if it is on the floor.
    /// </summary>
    public void Use() {
        if (scripts.levelManager != null && !scripts.levelManager.lockActions) {
            if (scripts.tutorial == null || scripts.tutorial != null && !scripts.tutorial.preventAttack) { 
                // only allow item usage in certain conditions
                if (scripts.itemManager.floorItems.Contains(gameObject)) {
                    // if item is on the floor
                    if (itemType == "arrow") {
                        if (scripts.tutorial == null) { 
                            // if the item is arrow (next level indicator)
                            scripts.levelManager.NextLevel();
                            scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                            return;
                            // go to the next level and end the level
                        }
                        Initiate.Fade("Menu", Color.black, 2.5f);
                    }
                    else {
                        // not an arrow
                        if (scripts.levelManager.sub == 4) {
                            // if on the trader level
                            if (scripts.itemManager.numItemsDroppedForTrade > 0) {
                                // if player has dropped items for trading
                                scripts.itemManager.numItemsDroppedForTrade--;
                                Save.game.numItemsDroppedForTrade = scripts.itemManager.numItemsDroppedForTrade;
                                Save.persistent.itemsTraded++;
                                // decrement counter
                                scripts.itemManager.MoveToInventory(scripts.itemManager.floorItems.IndexOf(gameObject));
                                // move the selected item into the player's inventory
                            }
                            else { scripts.turnManager.SetStatusText("drop an item to trade"); }
                            // player has not dropped items, so give a reminder
                        }
                        else { scripts.itemManager.MoveToInventory(scripts.itemManager.floorItems.IndexOf(gameObject)); }
                        // not trader, so just pick up the item
                    }
                }
                else {
                    if (itemType == "retry") {
                        Save.persistent.gamesPlayed++;
                        Save.SavePersistent();
                        scripts.levelManager.lockActions = true;
                        Initiate.Fade("Game", Color.black, scripts.backToMenu.transitionMultiplier);
                        scripts.soundManager.PlayClip("next");
                        scripts.music.FadeVolume("Through");
                        // reload scene
                    }
                    else if (!scripts.turnManager.isMoving && scripts.player.inventory.Contains(gameObject)) {
                        // in player's inventory and not moving, MUST HAVE CHECK FOR INVENTORY HERE BECAUSE OTHERWISE IT BREAKS
                        if (itemType == "weapon") {
                            // if player is trying to use weapon
                            if (!scripts.turnManager.isMoving && !scripts.player.isDead) {
                                // if conditions allow for attack
                                if (scripts.enemy.isDead) { scripts.turnManager.SetStatusText("he's dead"); }
                                else if (scripts.levelManager.sub == 4 || scripts.enemy.enemyName.text == "Tombstone") { scripts.turnManager.SetStatusText("mind your manners"); }
                                // send reminders accordingly
                                else if (!scripts.enemy.isDead) { scripts.player.UseWeapon(); }
                                // attack if enemy is not dead or tombstone
                                // else { print("error!"); }
                            }
                        }
                        else if (itemType == "common") { UseCommon(); }
                        else if (itemType == "rare") { UseRare(); }
                        // not item, so use corresponding item type
                    }
                }
            }
        }
        if (itemType != "retry") { scripts.itemManager.SaveInventoryItems(); }
        Save.SavePersistent();
        // Save the items as long as we didn't use the retry button
    }

    /// <summary>
    /// Use a common item.
    /// </summary>
    private void UseCommon() {
        if (scripts.enemy.isDead) {
            if (itemName == "steak" || itemName == "cheese" || itemName == "retry" || itemName == "arrow") {
                // only allow those 4 items to be used if the enemy is dead, otherwise its a waste
                StartCoroutine(UseCommonCoro());
            }
        }
        else {
            StartCoroutine(UseCommonCoro());
        }
    }

    /// <summary>
    /// Do not call this coroutine, use UseCommon() instead.
    /// </summary>
    private IEnumerator UseCommonCoro() {
        if (!scripts.levelManager.lockActions) {
            // don't use items when locked
            switch (itemName) {
                case "steak" when scripts.enemy.enemyName.text != "Tombstone":
                    // eating steak
                    Save.persistent.foodEaten++;
                    scripts.soundManager.PlayClip("eat");
                    if (scripts.player.charNum == 0) { scripts.turnManager.ChangeStaminaOf("player", 7); }
                    else { scripts.turnManager.ChangeStaminaOf("player", 5); }
                    // change stamina based on the character
                    scripts.turnManager.SetStatusText("you swallow steak");
                    // status text
                    Remove();
                    // remove from player inventory
                    break;
                case "rotten steak" when scripts.enemy.enemyName.text != "Tombstone":
                    Save.persistent.foodEaten++;
                    // don't change stamina for rotten foods
                    scripts.soundManager.PlayClip("eat");
                    scripts.turnManager.SetStatusText("you swallow rotten steak");
                    Remove();
                    break;
                case "cheese" when scripts.enemy.enemyName.text != "Tombstone":
                    Save.persistent.foodEaten++;
                    scripts.soundManager.PlayClip("eat");
                    if (scripts.player.charNum == 0) { scripts.turnManager.ChangeStaminaOf("player", 5); }
                    else { scripts.turnManager.ChangeStaminaOf("player", 3); }
                    scripts.turnManager.SetStatusText("you swallow cheese");
                    Remove();
                    break;
                case "moldy cheese" when scripts.enemy.enemyName.text != "Tombstone":
                    Save.persistent.foodEaten++;
                    scripts.soundManager.PlayClip("eat");
                    scripts.turnManager.SetStatusText("you swallow moldy cheese");
                    Remove();
                    break;
                case "scroll" when scripts.levelManager.sub != 4:
                    Save.persistent.scrollsRead++;
                    switch (modifier) {
                        case "fury":
                            if (scripts.player.isFurious) { scripts.turnManager.SetStatusText("you are already furious"); }
                            // prevent player from accidentally using two scrolls
                            else {
                                scripts.soundManager.PlayClip("fwoosh");
                                scripts.player.SetPlayerStatusEffect("fury", true);
                                // turn on fury
                                scripts.turnManager.SetStatusText("you read scroll of fury... you feel furious");
                                scripts.diceSummoner.MakeAllAttachedYellow();
                                Remove();
                                // consume the scroll
                            }
                            break;
                        case "dodge":
                            if (scripts.player.isDodgy) { scripts.turnManager.SetStatusText("you are already dodgy"); }
                            // prevent player from accidentally using two scrolls
                            else {
                                scripts.soundManager.PlayClip("fwoosh");
                                scripts.player.SetPlayerStatusEffect("dodge", true);
                                // turn on dodge
                                scripts.turnManager.SetStatusText("you read scroll of dodge... you feel dodgy");
                                Remove();
                                // consume and notify player
                            }
                            break;
                        case "haste":
                            if ((from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached == false select a).ToList().Count == 0) {
                                scripts.turnManager.SetStatusText("all dice have been chosen");
                                // prevent player from wasting scroll
                            }
                            else {
                                scripts.soundManager.PlayClip("fwoosh");
                                if (scripts.player.isHasty) { scripts.turnManager.SetStatusText("you are already winged"); }
                                // prevent player from accidentally using two scrolls
                                else {
                                    scripts.player.SetPlayerStatusEffect("haste", true);
                                    // turn on haste
                                    scripts.turnManager.SetStatusText("you read scroll of haste... you feel winged");
                                    Remove();
                                    // consume and notify player
                                }
                            }
                            break;
                        case "leech":
                            if (scripts.player.isBloodthirsty) { scripts.turnManager.SetStatusText("you are already bloodthirsty"); }
                            // prevent player from accidentally using two scrolls
                            else {
                                scripts.soundManager.PlayClip("fwoosh");
                                scripts.player.SetPlayerStatusEffect("leech", true);
                                // turn on leech
                                scripts.turnManager.SetStatusText("you read scroll of leech... you feel bloodthirsty");
                                Remove();
                                // consume and notify player
                            }
                            break;
                        case "courage":
                            if (scripts.player.isCourageous) { scripts.turnManager.SetStatusText("you are already courageous"); }
                            // prevent player from accidentally using two scrolls
                            else {
                                scripts.soundManager.PlayClip("fwoosh");
                                scripts.player.SetPlayerStatusEffect("courage", true);
                                // turn on courage
                                scripts.turnManager.SetStatusText("you read scroll of courage... you feel courageous");
                                Remove();
                                // consume and notify player
                            }
                            break;
                        case "challenge" when scripts.levelManager.level == 4:
                            // dont let scroll of challenge be used on devil!
                            scripts.soundManager.PlayClip("shuriken");
                            scripts.turnManager.SetStatusText("the scroll crumbles to dust");
                            scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                            Remove();
                            break;
                        case "challenge":
                            scripts.levelManager.NextLevel(true);
                            // load lich level
                            scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                            Remove();
                            break;
                        case "nothing":
                            scripts.soundManager.PlayClip("fwoosh");
                            scripts.turnManager.SetStatusText("you read scroll of nothing... nothing happens");
                            Remove();
                            break;
                    }
                    break;
                case "potion" when scripts.levelManager.sub != 4:
                    // don't let potions be used at merchant
                    Save.persistent.potionsQuaffed++;
                    scripts.soundManager.PlayClip("gulp");
                    scripts.turnManager.SetStatusText($"you quaff potion of {modifier}");
                    // notify player
                    switch (modifier) {
                        case "accuracy":
                            scripts.player.potionStats["green"] += 3;
                            scripts.statSummoner.ShiftDiceAccordingly("green", 3);
                            Save.game.potionAcc = scripts.player.potionStats["green"];
                            break;
                        case "speed":
                            scripts.player.potionStats["blue"] += 3;
                            scripts.statSummoner.ShiftDiceAccordingly("blue", 3);
                            Save.game.potionSpd = scripts.player.potionStats["blue"];
                            break;
                        case "strength":
                            scripts.player.potionStats["red"] += 3;
                            scripts.statSummoner.ShiftDiceAccordingly("red", 3);
                            Save.game.potionDef = scripts.player.potionStats["red"];
                            break;
                        case "defense":
                            scripts.player.potionStats["white"] += 3;
                            scripts.statSummoner.ShiftDiceAccordingly("white", 3);
                            Save.game.potionDmg = scripts.player.potionStats["white"];
                            break;
                        case "might":
                            scripts.diceSummoner.GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", isFromMight:true);
                            break;
                        case "life":
                            scripts.player.woundList.Clear();
                            scripts.turnManager.DisplayWounds();
                            // heal and display the wounds
                            // injuredtextchange does not want to work for some reason
                            yield return scripts.delays[0.25f];
                            scripts.soundManager.PlayClip("blip1");
                            break;
                        case "nothing": break;
                        // default: print("invalid potion modifier detected"); break;
                    }
                    if (scripts.tutorial == null) { Save.SaveGame(); }
                    scripts.statSummoner.SummonStats();
                    scripts.statSummoner.SetDebugInformationFor("player");
                    Remove();
                    scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                    break;
                case "shuriken" when scripts.levelManager.sub != 4:
                    Save.persistent.shurikensThrown++;
                    scripts.soundManager.PlayClip("shuriken");
                    // play sound clip
                    scripts.itemManager.discardableDieCounter++;print("incremented from shuriken!");
                    // increment counter
                    Save.game.discardableDieCounter = scripts.itemManager.discardableDieCounter;
                    if (scripts.tutorial == null) { Save.SaveGame(); }
                    Remove();
                    scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                    break;
                case "skeleton key" when scripts.levelManager.sub != 4:
                    if (scripts.levelManager.level == 4 && scripts.levelManager.sub == 1) {
                        // can't use skeleton key on the devil
                        scripts.soundManager.PlayClip("shuriken");
                        scripts.turnManager.SetStatusText("the key crumbles to dust");
                    }
                    else {
                        // spawning a normal enemy
                        scripts.levelManager.NextLevel();
                        // load next level
                        scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                    }
                    Remove();
                    scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                    break;
            }
        }
    }

    /// <summary>
    /// Use a rare item. 
    /// </summary>
    private void UseRare() {
        // these are pretty self explanatory
        if (!scripts.levelManager.lockActions && scripts.levelManager.sub != 4 && scripts.enemy.enemyName.text != "Tombstone" && !scripts.enemy.isDead) {
            switch (itemName) {
                case "helm of might":
                    if (!scripts.itemManager.usedHelm) {
                        if (scripts.player.stamina >= 3) {
                            scripts.soundManager.PlayClip("fwoosh");
                            // need 3 stamina
                            scripts.itemManager.usedHelm = true;
                            Save.game.usedHelm = true;
                            if (scripts.tutorial == null) { Save.SaveGame(); }
                            // set variable
                            scripts.turnManager.SetStatusText("you feel mighty");
                            // notify player
                            scripts.turnManager.ChangeStaminaOf("player", -3);
                            // use stamina
                            scripts.diceSummoner.GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", isFromMight:true);
                            // add yellow die to red stat
                        }
                        else { scripts.turnManager.SetStatusText("not enough stamina"); }
                        // notify player
                    }
                    else { scripts.turnManager.SetStatusText("helm can help you no further"); }
                    // notfiy player
                    break;
                // these are pretty self explanatory
                case "kapala":
                    scripts.turnManager.SetStatusText("offer an item to become furious");
                    break;
                case "boots of dodge":
                    if (!scripts.itemManager.usedBoots) {
                        if (scripts.player.stamina >= 1) {
                            scripts.soundManager.PlayClip("fwoosh");
                            scripts.turnManager.SetStatusText("you feel dodgy");
                            scripts.itemManager.usedBoots = true;
                            Save.game.usedBoots = true;
                            if (scripts.tutorial == null) { Save.SaveGame(); }
                            scripts.turnManager.ChangeStaminaOf("player", -1);
                            scripts.player.SetPlayerStatusEffect("dodge", true);
                        }
                        else { scripts.turnManager.SetStatusText("not enough stamina"); }
                    }
                    else { scripts.turnManager.SetStatusText("boots can help you no further"); }
                    break;
                case "ankh":
                    if (!scripts.itemManager.usedAnkh) {
                        scripts.soundManager.PlayClip("click0");
                        scripts.itemManager.usedAnkh = true;
                        Save.game.usedAnkh = true;
                        if (scripts.tutorial == null) { Save.SaveGame(); }
                        foreach (string key in scripts.itemManager.statArr) {
                            scripts.turnManager.ChangeStaminaOf("player", scripts.statSummoner.addedPlayerStamina[key]);
                            scripts.statSummoner.addedPlayerStamina[key] = 0;
                            // refund stamina
                        }
                        scripts.statSummoner.ResetDiceAndStamina();
                        scripts.diceSummoner.SummonDice(false, true);
                        scripts.statSummoner.SummonStats();
                    }
                    else { scripts.turnManager.SetStatusText("ankh glows with red light"); }
                    break;
                // default: print("rare item with invalid name found!"); break;
            }
        }
    }

    /// <summary>
    /// Remove an item from the player's inventory.
    /// </summary>
    public void Remove(bool drop = false, bool selectNew = true, bool armorFade = false, bool torchFade = false, bool dontSave = false) {
        if (drop && !scripts.itemManager.floorItems.Contains(gameObject) && !scripts.turnManager.isMoving) {
            if (scripts.tutorial != null && modifier == "nothing" && scripts.tutorial.curIndex == 2 && !scripts.tutorial.isAnimating) { scripts.tutorial.Increment(); }
            // if dropping the item when allowed
            if (scripts.itemManager.PlayerHas("kapala") && scripts.levelManager.sub != 4) {
                // play add checks so that this doesn't happen multiple times a round / when dropping items after enemy has dead
                if (itemType != "weapon") {
                    // if the item is not the player's weapon
                    scripts.player.SetPlayerStatusEffect("fury", true);
                    // turn on fury
                    scripts.turnManager.SetStatusText("deity accepts your offering... you feel furious");
                    // notify player
                    scripts.soundManager.PlayClip("fwoosh");
                    // play sound clip
                    scripts.diceSummoner.MakeAllAttachedYellow();
                }
            }
            else {
                if (scripts.levelManager.sub == 4) { scripts.itemManager.numItemsDroppedForTrade++; }
                Save.game.numItemsDroppedForTrade = scripts.itemManager.numItemsDroppedForTrade;
                if (scripts.tutorial == null) { Save.SaveGame(); }
                // if trader level increment the number of items dropped for trading
                if (itemType == "weapon") {
                    scripts.turnManager.SetStatusText($"you drop {scripts.itemManager.descriptionDict[itemName.Split(' ')[1]]}");
                }
                else if (itemName == "necklet") {
                    if (modifier == "arcane") { scripts.turnManager.SetStatusText("you drop arcane necklet"); }
                    else { scripts.turnManager.SetStatusText($"you drop {itemName} of {modifier}"); }
                }
                else if (itemName == "potion" || itemName == "scroll") { scripts.turnManager.SetStatusText($"you drop {itemName} of {modifier}"); }
                else { scripts.turnManager.SetStatusText($"you drop {itemName}"); }
                // notify player that item has been dropped
            }
        }
        int index = scripts.itemManager.curList.IndexOf(gameObject);
        // get the index of the object from the current selected list
        if (scripts.player.inventory[index].GetComponent<Item>().itemName == "necklet")
        {
            // if removing a necklet
            if (scripts.player.inventory[index].GetComponent<Item>().modifier != "arcane")
            {
                // not an arcane necklet
                string t = scripts.itemManager.statArr[Array.IndexOf(scripts.itemManager.neckletTypes, scripts.player.inventory[index].GetComponent<Item>().modifier)];
                // get a string refernce for the necklet's stat
                scripts.itemManager.neckletCounter[t]--;
                // decrement counter
                scripts.itemManager.neckletStats[t] = scripts.itemManager.neckletCounter["arcane"] * scripts.itemManager.neckletCounter[t];
                // adjust stats added from that necklet type
            }
            else
            {
                // is an arcane necklet
                scripts.itemManager.neckletCounter["arcane"]--;
                // decrement counter
                foreach (string stat in scripts.itemManager.statArr)
                {
                    // for every stat
                    scripts.itemManager.neckletStats[stat] = scripts.itemManager.neckletCounter["arcane"] * scripts.itemManager.neckletCounter[stat];
                    // adjust stats based on new number of arcane necklets
                }
            }
            scripts.statSummoner.SummonStats();
            scripts.statSummoner.SetDebugInformationFor("player");
            // update stuff
        }
        if (armorFade) {
            StartCoroutine(FadeArmor(index, selectNew));
        }
        else if (torchFade) {
            StartCoroutine(FadeTorch(index, selectNew));
        }
        // fading armor or torch, so do something special
        else {
            Destroy(gameObject);
            // destroy the object
            ShiftItems(index, selectNew);
        }
        if (!dontSave) { scripts.itemManager.SaveInventoryItems(); }
    }

    /// <summary>
    /// Shift the items in the current list over, starting from a given index.
    /// </summary>
    private void ShiftItems(int index, bool selectNew) {
        scripts.itemManager.curList.RemoveAt(index);
        // remove the item from the list
        for (int i = index; i < scripts.player.inventory.Count; i++) {
            // for each item in the inventory after the index of the previous one
            scripts.player.inventory[i].transform.position = new Vector2(scripts.player.inventory[i].transform.position.x - 1f, 3.16f);
            // shift over each item
        }
        if (selectNew) { scripts.itemManager.Select(scripts.itemManager.curList, index, playAudio: false); }
        // select the next item over if needed
    }

    /// <summary>
    /// Coroutine to break player's armor when it is hit.
    /// </summary>
    private IEnumerator FadeArmor(int index, bool selectNew) {
        yield return scripts.delays[0.05f];
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color temp = sr.color;
        temp.a = 1;
        sr.color = temp;
        for (int i = 0; i < 5; i++)
        {
            yield return scripts.delays[0.033f];
            temp.a -= 1f / 5f;
            sr.color = temp;
        }
        Destroy(gameObject);
        ShiftItems(index, selectNew);
    }

    /// <summary>
    /// Coroutine to fade out a torch when its time has come.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="selectNew"></param>
    /// <returns></returns>
    private IEnumerator FadeTorch(int index, bool selectNew) {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color temp = sr.color;
        temp.a = 1;
        sr.color = temp;
        for (int i = 0; i < 10; i++) {
            yield return scripts.delays[0.033f];
            temp.a -= 1f / 10f;
            sr.color = temp;
        }
        for (int i = 0; i < 10; i++) {
            yield return scripts.delays[0.033f];
            temp.a += 1f / 10f;
            sr.color = temp;
        }
        for (int i = 0; i < 8; i++) {
            yield return scripts.delays[0.033f];
            temp.a -= 1f / 8f;
            sr.color = temp;
        }
        for (int i = 0; i < 8; i++) {
            yield return scripts.delays[0.033f];
            temp.a += 1f / 8f;
            sr.color = temp;
        }
        for (int i = 0; i < 6; i++) {
            yield return scripts.delays[0.033f];
            temp.a -= 1f / 6f;
            sr.color = temp;
        }
        // flash in and out, faster and faster
        Destroy(gameObject);
        ShiftItems(index, selectNew);
        // destroy it and shift over
    }
}
