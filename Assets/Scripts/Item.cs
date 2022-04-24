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
    public Dictionary<string, int> weaponStats = new();
    private bool preventPlayingFX = true;

    void Awake() {
        scripts = FindObjectOfType<Scripts>();
    }

    private IEnumerator AllowFX() {
        yield return new WaitForSeconds(0.45f);
        preventPlayingFX = false;
    }

    private void Start() {
        gameObject.name = itemName;
        // set the object's name to its itemname, so it can be identified in the editor
        if (itemName == "torch" && scripts.player != null) {
            // if the item is a torch
            modifier = $"{scripts.levelManager.level + 1}-{Mathf.Clamp(scripts.levelManager.sub-1, 1, 3)}";
            // set the fade time of the torch to be the next level but with previous sub (clamping to prevent bugs)
        }
        StartCoroutine(AllowFX());
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            // left click
            if (scripts.itemManager.highlightedItem == gameObject) { Use(); }
            // if clicking over highlighted weapon, use it
            else {
                Select();
                // otherwise select it
                if (scripts.player != null) {
                    // in game
                    scripts.itemManager.curList = scripts.player.inventory.Contains(gameObject) 
                        ? scripts.player.inventory 
                        : scripts.itemManager.floorItems;
                    // depending on where the selection occurred, assign the curlist variable to be there      
                }
                else {
                    // in character select 
                    scripts.itemManager.curList = scripts.itemManager.floorItems;
                }
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            DropItem();
        }
    }

    public void DropItem() { 
        // right click
        if (scripts.itemManager.highlightedItem == gameObject) {
            // if highlighted
            if (itemType != "weapon" && !scripts.itemManager.floorItems.Contains(gameObject)) {
                // if the item is not weapon and not on the floor
                if (scripts.levelManager.sub == 4 || Save.game.enemyIsDead || scripts.enemy.enemyName.text == "Tombstone" || scripts.itemManager.PlayerHas("kapala") || (scripts.tutorial != null && modifier == "nothing" && scripts.tutorial.curIndex == 2 && !scripts.tutorial.isAnimating)) {
                    // only allow dropping of items if player is trading, enemy is dead, on a tombstone, are offering to kapala, or are dropping scroll in tutorial
                    Remove(true);
                    // scripts.soundManager.PlayClip("")
                }
            }
        }
        // could combine these but it's beyond hideous and unreadable
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
            string descSearch;
            if (modifier == "legendary") { descSearch = itemName; }
            else { descSearch = itemName.Split(' ')[1]; }   
            // depending on whether the item is legendary or not, load the correct item
            scripts.itemManager.itemDesc.text = scripts.itemManager.descriptionDict[descSearch] == "" 
                ? itemName 
                : $"{itemName}\n- {scripts.itemManager.descriptionDict[descSearch]}";
            // if description, then display it, else just display it normally
            if (scripts.itemManager.floorItems.Contains(gameObject) && scripts.player != null) {
                // if item on the floor and not in character select
                scripts.enemy.stats = weaponStats;
                scripts.statSummoner.SummonStats();
                scripts.statSummoner.SetDebugInformationFor("enemy");
                scripts.turnManager.blackBox.transform.localPosition = scripts.turnManager.offScreen;
                // display the stats for the player to see
            }
            else {
                // if item is in inventory
                if (scripts.enemy != null) {
                    // and not in character select
                    if (Save.game.enemyIsDead) { scripts.turnManager.blackBox.transform.localPosition = scripts.turnManager.onScreen; }
                    // hide the weapon stats if enemy is dead and not clicking on an enemy
                }
            }
        }
        else {
            if (scripts.levelManager == null || !scripts.levelManager.lockActions) {
                // only allow weapons to be selected when locked
                if (scripts.levelManager != null && Save.game.enemyIsDead || scripts.levelManager != null && scripts.enemy.enemyName.text == "Tombstone") { scripts.turnManager.blackBox.transform.localPosition = scripts.turnManager.onScreen; }
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
                        scripts.itemManager.itemDesc.text = modifier switch {
                            "arcane" => "arcane necklet\nall necklets are more effective",
                            "nothing" => "necklet of nothing\ndoes nothing",
                            "victory" => "necklet of victory\nthe victory is in your hands!..",
                            _ => $"necklet of {modifier}\n+{scripts.itemManager.neckletCounter["arcane"]} {scripts.itemManager.statArr1[Array.IndexOf(scripts.itemManager.neckletTypes, modifier)]}"
                        };
                        break;
                    case "cheese":
                    case "steak":
                        if (scripts.player == null || Save.game.curCharNum == 0) { scripts.itemManager.itemDesc.text = $"{itemName}\n+{int.Parse(scripts.itemManager.descriptionDict[itemName]) + 2} stamina"; }
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
                    if (itemName == "arrow") {
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
                            if (Save.game.numItemsDroppedForTrade > 0) {
                                // if player has dropped items for trading
                                Save.game.numItemsDroppedForTrade--;
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
                    if (itemName == "retry") {
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
                                if (Save.game.enemyIsDead) { scripts.turnManager.SetStatusText("he's dead"); }
                                else if (scripts.levelManager.sub == 4 || scripts.enemy.enemyName.text == "Tombstone") { scripts.turnManager.SetStatusText("mind your manners"); }
                                // send reminders accordingly
                                else if (!Save.game.enemyIsDead) { scripts.player.UseWeapon(); }
                                // attack if enemy is not dead or tombstone
                                // else { print("error!"); }
                            }
                        }
                        else if (itemType == "common") { UseCommon(); }
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
        if (Save.game.enemyIsDead || scripts.enemy.enemyName.text is "Tombstone" or "Merchant") {
            if (itemName is "steak" or "cheese" or "retry" or "arrow" || itemName == "potion" && modifier == "life") {
                // only these specific items can be used at a merchant/tombstone, otherwise it doesn't make sense or is a waste
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
                case "steak":
                    // eating steak
                    Save.persistent.foodEaten++;
                    scripts.soundManager.PlayClip("eat");
                    scripts.turnManager.ChangeStaminaOf("player", Save.game.curCharNum == 0 ? 7 : 5);
                    // change stamina based on the character
                    scripts.turnManager.SetStatusText("you swallow steak");
                    // status text
                    Remove();
                    // remove from player inventory
                    break;
                case "rotten steak":
                    Save.persistent.foodEaten++;
                    // don't change stamina for rotten foods
                    scripts.soundManager.PlayClip("eat");
                    scripts.turnManager.SetStatusText("you swallow rotten steak");
                    Remove();
                    break;
                case "cheese":
                    Save.persistent.foodEaten++;
                    scripts.soundManager.PlayClip("eat");
                    scripts.turnManager.ChangeStaminaOf("player", Save.game.curCharNum == 0 ? 5 : 3);
                    scripts.turnManager.SetStatusText("you swallow cheese");
                    Remove();
                    break;
                case "moldy cheese":
                    Save.persistent.foodEaten++;
                    scripts.soundManager.PlayClip("eat");
                    scripts.turnManager.SetStatusText("you swallow moldy cheese");
                    Remove();
                    break;
                case "scroll":
                    Save.persistent.scrollsRead++;
                    switch (modifier) {
                        case "fury":
                            if (Save.game.isFurious) { scripts.turnManager.SetStatusText("you are already furious"); }
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
                            if (Save.game.isDodgy) { scripts.turnManager.SetStatusText("you are already dodgy"); }
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
                                if (Save.game.isHasty) { scripts.turnManager.SetStatusText("you are already winged"); }
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
                            if (Save.game.isBloodthirsty) { scripts.turnManager.SetStatusText("you are already bloodthirsty"); }
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
                            if (Save.game.isCourageous) { scripts.turnManager.SetStatusText("you are already courageous"); }
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
                case "potion":
                    Save.persistent.potionsQuaffed++;
                    scripts.soundManager.PlayClip("gulp");
                    scripts.turnManager.SetStatusText($"you quaff potion of {modifier}");
                    // notify player
                    switch (modifier) {
                        case "accuracy":
                            scripts.statSummoner.ShiftDiceAccordingly("green", 
                                scripts.statSummoner.RawSumOfStat("green", "player") == -1 ? 1 : 3);
                            scripts.player.potionStats["green"] += 3;
                            Save.game.potionAcc = scripts.player.potionStats["green"];
                            break;
                        case "speed":
                            scripts.statSummoner.ShiftDiceAccordingly("blue", 
                                scripts.statSummoner.RawSumOfStat("blue", "player") == -1 ? 1 : 3);
                            scripts.player.potionStats["blue"] += 3;
                            Save.game.potionSpd = scripts.player.potionStats["blue"];
                            break;
                        case "strength":
                            scripts.statSummoner.ShiftDiceAccordingly("red", 
                                scripts.statSummoner.RawSumOfStat("red", "player") == -1 ? 1 : 3);
                            scripts.player.potionStats["red"] += 3;
                            Save.game.potionDef = scripts.player.potionStats["red"];
                            break;
                        case "defense":
                            scripts.statSummoner.ShiftDiceAccordingly("white", 
                                scripts.statSummoner.RawSumOfStat("white", "player") == -1 ? 1 : 3);
                            scripts.player.potionStats["white"] += 3;
                            Save.game.potionDmg = scripts.player.potionStats["white"];
                            break;
                        // for regular potions, shift the stats by varying amounts depending if the sum without die is -1 or not (or knee injury), making sure the dice stay in the correct position
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
                case "shuriken":
                    Save.persistent.shurikensThrown++;
                    scripts.soundManager.PlayClip("shuriken");
                    // play sound clip
                    Save.game.discardableDieCounter++;
                    // increment counter
                    if (scripts.tutorial == null) { Save.SaveGame(); }
                    Remove();
                    scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                    break;
                case "skeleton key":
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
                case "helm of might":
                    if (!Save.game.usedHelm) {
                        if (scripts.player.stamina >= 3) {
                            scripts.soundManager.PlayClip("fwoosh");
                            // need 3 stamina
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
                    // notify player
                    break;
                case "kapala":
                    scripts.turnManager.SetStatusText("offer an item to become furious");
                    break;
                case "boots of dodge":
                    if (!Save.game.usedBoots) {
                        if (scripts.player.stamina >= 1) {
                            scripts.soundManager.PlayClip("fwoosh");
                            scripts.turnManager.SetStatusText("you feel dodgy");
                            Save.game.usedBoots = true;
                            if (scripts.tutorial == null) { Save.SaveGame(); }
                            scripts.turnManager.ChangeStaminaOf("player", -1);
                            scripts.player.SetPlayerStatusEffect("dodge", true);
                        }
                        else { scripts.turnManager.SetStatusText("not enough stamina"); }
                    }
                    else { scripts.turnManager.SetStatusText("you are already dodgy"); }
                    break;
                case "ankh":
                    if (!Save.game.usedAnkh) {
                        scripts.soundManager.PlayClip("click0");
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
                        scripts.turnManager.DetermineMove(true);
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
                if (itemType != "weapon" && scripts.enemy.enemyName.text != "Tombstone" && !Save.game.enemyIsDead) {
                    // if the item is not the player's weapon and the enemy is not the tombstone
                    bool wasntAlreadyFurious = scripts.player.SetPlayerStatusEffect("fury", true);
                    if (wasntAlreadyFurious) {
                        // turn on fury
                        scripts.turnManager.SetStatusText("deity accepts your offering... you feel furious");
                        // notify player
                        scripts.soundManager.PlayClip("fwoosh");
                        // play sound clip
                        scripts.diceSummoner.MakeAllAttachedYellow();
                    }
                }
            }
            else {
                if (scripts.levelManager.sub == 4) { Save.game.numItemsDroppedForTrade++; }
                if (scripts.tutorial == null) { Save.SaveGame(); }
                // if trader level increment the number of items dropped for trading
                if (itemType == "weapon") {
                    scripts.turnManager.SetStatusText($"you drop {scripts.itemManager.descriptionDict[itemName.Split(' ')[1]]}");
                }
                else if (itemName == "necklet") {
                    scripts.turnManager.SetStatusText(modifier == "arcane" 
                                                          ? "you drop arcane necklet" 
                                                          : $"you drop {itemName} of {modifier}");
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
        if (selectNew) { scripts.itemManager.Select(scripts.itemManager.curList, index, playAudio: true); }
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
        scripts.itemManager.SaveInventoryItems();
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
