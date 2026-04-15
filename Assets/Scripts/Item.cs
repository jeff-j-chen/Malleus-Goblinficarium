using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class Item : MonoBehaviour {
    [SerializeField] public Scripts s;
    [SerializeField] public string itemName;
    [SerializeField] public string itemType;
    [SerializeField] public string modifier;
    public Dictionary<string, int> weaponStats = new();
    private bool preventPlayingFX = true;

    void Awake() {
        s = FindObjectOfType<Scripts>();
    }

    private IEnumerator AllowFX() {
        yield return new WaitForSeconds(0.45f);
        preventPlayingFX = false;
    }

    private void Start() {
        gameObject.name = itemName;
        // set the object's name to its itemname, so it can be identified in the editor
        if (itemName == "torch" && s.player != null) {
            // if the item is a torch
            modifier = $"{s.levelManager.level + 1}-{Mathf.Clamp(s.levelManager.sub-1, 1, 3)}";
            // set the fade time of the torch to be the next level but with previous sub (clamping to prevent bugs)
        }
        StartCoroutine(AllowFX());
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) { DropItem(); }
        if (!Input.GetMouseButtonDown(0)) return;
        // left click
        if (s.itemManager.highlightedItem == gameObject) { Use(); return; }
        Select();
        // in character select player is null - default to floor items
        s.itemManager.curList = s.player != null && s.player.inventory.Contains(gameObject)
            ? s.player.inventory
            : s.itemManager.floorItems;
    }

    public void DropItem() {
        if (s.itemManager.highlightedItem != gameObject) return;
        if (itemType == "weapon" || s.itemManager.floorItems.Contains(gameObject)) return;
        // only drop if: trading, enemy dead, tombstone, kapala offering, or tutorial scroll
        bool canDrop = s.itemManager.IsVendorEncounter() || Save.game.enemyIsDead
            || s.enemy.enemyName.text == "Tombstone" || s.itemManager.PlayerHas("kapala")
            || (s.tutorial != null && modifier == "nothing" && s.tutorial.curIndex == 2 && !s.tutorial.isAnimating);
        if (!canDrop) return;
        if (s.itemManager.IsBlacksmithEncounter() && (Save.game.blacksmithHasForged || Save.game.numItemsDroppedForTrade > 0)) {
            s.turnManager.SetStatusText("he's tired");
            return;
        }
        Remove(true);
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
            s.itemManager.itemDesc.text = s.itemManager.descriptionDict[descSearch] == "" 
                ? itemName 
                : $"{itemName}\n- {s.itemManager.descriptionDict[descSearch]}";
            // if description, then display it, else just display it normally
            if (s.itemManager.floorItems.Contains(gameObject) && s.player != null) {
                // if item on the floor and not in character select
                s.enemy.stats = weaponStats;
                s.statSummoner.SummonStats();
                s.statSummoner.SetDebugInformationFor("enemy");
                s.turnManager.blackBox.transform.localPosition = s.turnManager.offScreen;
                // display the stats for the player to see
            }
            else {
                // if item is in inventory
                if (s.enemy != null) {
                    // and not in character select
                    if (Save.game.enemyIsDead) { s.turnManager.blackBox.transform.localPosition = s.turnManager.onScreen; }
                    // hide the weapon stats if enemy is dead and not clicking on an enemy
                }
            }
        }
        else {
            if (s.levelManager == null || !s.levelManager.lockActions) {
                // only allow weapons to be selected when locked
                if (s.levelManager != null &&
                    (Save.game.enemyIsDead || s.enemy.enemyName.text == "Tombstone" || s.itemManager.IsVendorEncounter())) {
                    s.turnManager.blackBox.transform.localPosition = s.turnManager.onScreen;
                }
                // hide the weapon stats if enemy is dead and not clicking on an enemy
                switch (itemName) {
                    case "potion":
                        switch (modifier) {
                            case "accuracy": s.itemManager.itemDesc.text = "potion of accuracy\n+3 accuracy"; break;
                            case "speed": s.itemManager.itemDesc.text = "potion of speed\n+3 speed"; break;
                            case "strength": s.itemManager.itemDesc.text = "potion of strength\n+3 damage"; break;
                            case "defense": s.itemManager.itemDesc.text = "potion of parry\n+3 parry"; break;
                            case "might": s.itemManager.itemDesc.text = "potion of might\ngain a yellow die"; break;
                            case "life": s.itemManager.itemDesc.text = "potion of life\nheal all wounds"; break;
                            case "nothing": s.itemManager.itemDesc.text = "potion of nothing\ndoes nothing"; break;
                            // default: print("invalid potion modifier"); break;
                        }
                        break;
                    case "scroll":
                        switch (modifier) {
                            case "fury": s.itemManager.itemDesc.text = "scroll of fury\nall picked dice turn yellow"; break;
                            case "haste": s.itemManager.itemDesc.text = "scroll of haste\npick 3 dice, enemy gets the rest"; break;
                            case "dodge": s.itemManager.itemDesc.text = "scroll of dodge\nif you strike first, ignore all damage"; break;
                            case "leech": s.itemManager.itemDesc.text = "scroll of leech\ncure the same wound as inflicted"; break;
                            case "courage": s.itemManager.itemDesc.text = "scroll of courage\nkeep 1 of your die till next round"; break;
                            case "challenge": s.itemManager.itemDesc.text = "scroll of challenge\n???"; break;
                            case "nothing": s.itemManager.itemDesc.text = "scroll of nothing\ndoes nothing"; break;
                            // default: print("invalid scroll modifier"); break;
                        }
                        break;
                    case "necklet":
                        s.itemManager.itemDesc.text = modifier switch {
                            "arcane" => "arcane necklet\nall necklets are more effective",
                            "nothing" => "necklet of nothing\ndoes nothing",
                            "victory" => "necklet of victory\nthe victory is in your hands!..",
                            _ => $"necklet of {modifier}\n+{s.itemManager.neckletCounter["arcane"]} {s.itemManager.statArr1[Array.IndexOf(s.itemManager.neckletTypes, modifier)]}"
                        };
                        break;
                    case "cheese":
                    case "steak":
                        if (s.player == null || Save.game.curCharNum == 0) { s.itemManager.itemDesc.text = $"{itemName}\n+{int.Parse(s.itemManager.descriptionDict[itemName]) + 2} stamina"; }
                        else { s.itemManager.itemDesc.text = $"{itemName}\n+{s.itemManager.descriptionDict[itemName]} stamina"; }
                        break;
                    case "moldy cheese":
                        s.itemManager.itemDesc.text = "moldy cheese\n+0 stamina"; break;
                    case "rotten steak":
                        s.itemManager.itemDesc.text = "rotten steak\n+0 stamina"; break;
                    case "arrow":
                        if (s.levelManager.level == 4 && s.levelManager.sub == 1) {
                            s.itemManager.itemDesc.text = "leave dungeon";
                        }
                        else { s.itemManager.itemDesc.text = s.itemManager.descriptionDict[itemName]; }
                        break;
                    case "retry":
                        s.itemManager.itemDesc.text = s.itemManager.descriptionDict[itemName]; break;
                    default:
                        s.itemManager.itemDesc.text = $"{itemName}\n{s.itemManager.descriptionDict[itemName]}"; break;
                }
                if (itemType == "upgrade") {
                    s.itemManager.itemDesc.text = $"{itemName}\n{s.itemManager.GetForgeDescription(modifier)}";
                }
                // set the proper item descriptions based on its name
            }
        }
        if (s.levelManager == null || itemType == "weapon" ||
            s.levelManager != null && !s.levelManager.lockActions) {
            // only allow weapons to be used when unlocked
            s.itemManager.highlight.transform.position = transform.position;
            // move the highlight to the selected item
            GameObject curItem = gameObject;
            s.itemManager.highlightedItem = curItem;
            // update the highlighted item variable
            // update the col variable
            s.itemManager.col = s.itemManager.curList.IndexOf(curItem);
            if (playAudio && !preventPlayingFX) { s.soundManager.PlayClip("click0"); }
            // dont play this if we just came from menu scene
            // play sound clip
            if (s.itemManager.IsVendorEncounter()) {
                s.itemManager.UpdateVendorUIForSelection(curItem);
            }
        }
    }

    /// <summary>
    /// use an item, picking it up if it is on the floor.
    /// </summary>
    public void Use() {
        bool skipSaves = false;
        if (s.levelManager != null && !s.levelManager.lockActions
            && (s.tutorial == null || !s.tutorial.preventAttack)) {
            skipSaves = ExecuteUse();
        }
        // arrow advancing level skips saves; all other paths save normally
        if (!skipSaves && itemType != "retry") { s.itemManager.SaveInventoryItems(); }
        if (!skipSaves) { Save.SavePersistent(); }
    }

    // returns true only when transitioning levels via arrow (saves should be skipped)
    private bool ExecuteUse() {
        if (!s.itemManager.floorItems.Contains(gameObject)) { UseInventoryItem(); return false; }
        if (itemName != "arrow") { PickupFloorItem(); return false; }
        // arrow: end tutorial or advance level
        if (s.tutorial != null) { Initiate.Fade("Menu", Color.black, 2.5f); return false; }
        s.levelManager.NextLevel();
        s.itemManager.Select(s.player.inventory, 0, true, false);
        return true;
    }

    private void UseInventoryItem() {
        if (itemName == "retry") {
            Save.persistent.gamesPlayed++;
            Save.SavePersistent();
            s.levelManager.lockActions = true;
            Initiate.Fade("Game", Color.black, s.backToMenu.transitionMultiplier);
            s.soundManager.PlayClip("next");
            s.music.FadeVolume("Through");
            return;
        }
        // MUST HAVE CHECK FOR INVENTORY HERE BECAUSE OTHERWISE IT BREAKS
        if (s.turnManager.isMoving || !s.player.inventory.Contains(gameObject)) return;
        if (itemType == "weapon") { UseWeaponFromInventory(); return; }
        if (itemType == "common") { UseCommon(); }
    }

    private void UseWeaponFromInventory() {
        if (s.player.isDead) return;
        if (Save.game.enemyIsDead) { s.turnManager.SetStatusText("he's dead"); return; }
        if (s.itemManager.IsVendorEncounter() || s.enemy.enemyName.text == "Tombstone") {
            s.turnManager.SetStatusText("mind your manners");
            return;
        }
        s.player.UseWeapon();
    }

    private void PickupFloorItem() {
        int idx = s.itemManager.floorItems.IndexOf(gameObject);
        if (s.itemManager.IsMerchantEncounter()) {
            if (Save.game.numItemsDroppedForTrade <= 0) { s.turnManager.SetStatusText("drop an item to trade"); return; }
            Save.game.numItemsDroppedForTrade--;
            Save.persistent.itemsTraded++;
            s.itemManager.MoveToInventory(idx);
            return;
        }
        if (s.itemManager.IsBlacksmithEncounter()) { PickupFromBlacksmith(idx); return; }
        s.itemManager.MoveToInventory(idx);
    }

    private void PickupFromBlacksmith(int idx) {
        if (itemType == "weapon") { s.itemManager.MoveToInventory(idx); return; }
        if (itemType != "upgrade") { s.turnManager.SetStatusText("drop an item to trade"); return; }
        if (Save.game.blacksmithHasForged) { s.turnManager.SetStatusText("he's tired"); return; }
        if (Save.game.numItemsDroppedForTrade <= 0) { s.turnManager.SetStatusText("drop an item to trade"); return; }
        Save.game.numItemsDroppedForTrade--;
        Save.persistent.itemsTraded++;
        s.itemManager.ForgePlayerWeapon(modifier);
        s.turnManager.SetStatusText($"your {s.player.inventory[0].GetComponent<Item>().itemName.Split(' ')[1]} is forged");
        s.itemManager.RemoveFloorItemAt(idx);
        s.itemManager.UpdateVendorUIForSelection();
    }

    /// <summary>
    /// Use a common item.
    /// </summary>
    private void UseCommon() {
        if (Save.game.enemyIsDead || s.enemy.enemyName.text is "Tombstone" or "Merchant" or "Blacksmith") {
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
        if (!s.levelManager.lockActions) {
            // don't use items when locked
            switch (itemName) {
                case "steak":
                    // eating steak
                    Save.persistent.foodEaten++;
                    s.soundManager.PlayClip("eat");
                    s.turnManager.ChangeStaminaOf("player", Save.game.curCharNum == 0 ? 7 : 5);
                    // change stamina based on the character
                    s.turnManager.SetStatusText("you swallow steak");
                    // status text
                    Remove();
                    // remove from player inventory
                    break;
                case "rotten steak":
                    Save.persistent.foodEaten++;
                    // don't change stamina for rotten foods
                    s.soundManager.PlayClip("eat");
                    s.turnManager.SetStatusText("you swallow rotten steak");
                    Remove();
                    break;
                case "cheese":
                    Save.persistent.foodEaten++;
                    s.soundManager.PlayClip("eat");
                    s.turnManager.ChangeStaminaOf("player", Save.game.curCharNum == 0 ? 5 : 3);
                    s.turnManager.SetStatusText("you swallow cheese");
                    Remove();
                    break;
                case "moldy cheese":
                    Save.persistent.foodEaten++;
                    s.soundManager.PlayClip("eat");
                    s.turnManager.SetStatusText("you swallow moldy cheese");
                    Remove();
                    break;
                case "scroll":
                    Save.persistent.scrollsRead++;
                    switch (modifier) {
                        case "fury":
                            if (Save.game.isFurious) { s.turnManager.SetStatusText("you are already furious"); }
                            // prevent player from accidentally using two scrolls
                            else {
                                s.soundManager.PlayClip("fwoosh");
                                s.player.SetPlayerStatusEffect("fury", true);
                                // turn on fury
                                s.turnManager.SetStatusText("you read scroll of fury... you feel furious");
                                s.diceSummoner.MakeAllAttachedYellow();
                                Remove();
                                // consume the scroll
                            }
                            break;
                        case "dodge":
                            if (Save.game.isDodgy) { s.turnManager.SetStatusText("you are already dodgy"); }
                            // prevent player from accidentally using two scrolls
                            else {
                                s.soundManager.PlayClip("fwoosh");
                                s.player.SetPlayerStatusEffect("dodge", true);
                                // turn on dodge
                                s.turnManager.SetStatusText("you read scroll of dodge... you feel dodgy");
                                Remove();
                                // consume and notify player
                            }
                            break;
                        case "haste":
                            if ((from a in s.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached == false select a).ToList().Count == 0) {
                                s.turnManager.SetStatusText("all dice have been chosen");
                                // prevent player from wasting scroll
                            }
                            else {
                                s.soundManager.PlayClip("fwoosh");
                                if (Save.game.isHasty) { s.turnManager.SetStatusText("you are already winged"); }
                                // prevent player from accidentally using two scrolls
                                else {
                                    s.player.SetPlayerStatusEffect("haste", true);
                                    // turn on haste
                                    s.turnManager.SetStatusText("you read scroll of haste... you feel winged");
                                    Remove();
                                    // consume and notify player
                                }
                            }
                            break;
                        case "leech":
                            if (Save.game.isBloodthirsty) { s.turnManager.SetStatusText("you are already bloodthirsty"); }
                            // prevent player from accidentally using two scrolls
                            else {
                                s.soundManager.PlayClip("fwoosh");
                                s.player.SetPlayerStatusEffect("leech", true);
                                // turn on leech
                                s.turnManager.SetStatusText("you read scroll of leech... you feel bloodthirsty");
                                Remove();
                                // consume and notify player
                            }
                            break;
                        case "courage":
                            if (Save.game.isCourageous) { s.turnManager.SetStatusText("you are already courageous"); }
                            // prevent player from accidentally using two scrolls
                            else {
                                s.soundManager.PlayClip("fwoosh");
                                s.player.SetPlayerStatusEffect("courage", true);
                                // turn on courage
                                s.turnManager.SetStatusText("you read scroll of courage... you feel courageous");
                                Remove();
                                // consume and notify player
                            }
                            break;
                        case "challenge" when s.levelManager.level == 4:
                            // dont let scroll of challenge be used on devil!
                            s.soundManager.PlayClip("shuriken");
                            s.turnManager.SetStatusText("the scroll crumbles to dust");
                            s.itemManager.Select(s.player.inventory, 0, true, false);
                            Remove();
                            break;
                        case "challenge":
                            s.levelManager.NextLevel(true);
                            // load lich level
                            s.itemManager.Select(s.player.inventory, 0, true, false);
                            Remove();
                            break;
                        case "nothing":
                            s.soundManager.PlayClip("fwoosh");
                            s.turnManager.SetStatusText("you read scroll of nothing... nothing happens");
                            Remove();
                            break;
                    }
                    break;
                case "potion":
                    Save.persistent.potionsQuaffed++;
                    s.soundManager.PlayClip("gulp");
                    s.turnManager.SetStatusText($"you quaff potion of {modifier}");
                    // notify player
                    switch (modifier) {
                        case "accuracy":
                            s.statSummoner.ShiftDiceAccordingly("green", 
                                s.statSummoner.RawSumOfStat("green", "player") == -1 ? 1 : 3);
                            s.player.potionStats["green"] += 3;
                            Save.game.potionAcc = s.player.potionStats["green"];
                            break;
                        case "speed":
                            s.statSummoner.ShiftDiceAccordingly("blue", 
                                s.statSummoner.RawSumOfStat("blue", "player") == -1 ? 1 : 3);
                            s.player.potionStats["blue"] += 3;
                            Save.game.potionSpd = s.player.potionStats["blue"];
                            break;
                        case "strength":
                            s.statSummoner.ShiftDiceAccordingly("red", 
                                s.statSummoner.RawSumOfStat("red", "player") == -1 ? 1 : 3);
                            s.player.potionStats["red"] += 3;
                            Save.game.potionDef = s.player.potionStats["red"];
                            break;
                        case "defense":
                            s.statSummoner.ShiftDiceAccordingly("white", 
                                s.statSummoner.RawSumOfStat("white", "player") == -1 ? 1 : 3);
                            s.player.potionStats["white"] += 3;
                            Save.game.potionDmg = s.player.potionStats["white"];
                            break;
                        // for regular potions, shift the stats by varying amounts depending if the sum without die is -1 or not (or knee injury), making sure the dice stay in the correct position
                        case "might":
                            s.diceSummoner.GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", isFromMight:true);
                            break;
                        case "life":
                            s.player.woundList.Clear();
                            s.turnManager.DisplayWounds();
                            // heal and display the wounds
                            // injuredtextchange does not want to work for some reason
                            yield return s.delays[0.25f];
                            s.soundManager.PlayClip("blip1");
                            break;
                        case "nothing": break;
                        // default: print("invalid potion modifier detected"); break;
                    }
                    if (s.tutorial == null) { Save.SaveGame(); }
                    s.statSummoner.SummonStats();
                    s.statSummoner.SetDebugInformationFor("player");
                    Remove();
                    s.itemManager.Select(s.player.inventory, 0, true, false);
                    break;
                case "shuriken":
                    Save.persistent.shurikensThrown++;
                    s.soundManager.PlayClip("shuriken");
                    // play sound clip
                    Save.game.discardableDieCounter++;
                    // increment counter
                    if (s.tutorial == null) { Save.SaveGame(); }
                    Remove();
                    s.itemManager.Select(s.player.inventory, 0, true, false);
                    break;
                case "skeleton key":
                    if (s.levelManager.level == 4 && s.levelManager.sub == 1) {
                        // can't use skeleton key on the devil
                        s.soundManager.PlayClip("shuriken");
                        s.turnManager.SetStatusText("the key crumbles to dust");
                    }
                    else {
                        // spawning a normal enemy
                        s.levelManager.NextLevel();
                        // load next level
                        s.itemManager.Select(s.player.inventory, 0, true, false);
                    }
                    Remove();
                    s.itemManager.Select(s.player.inventory, 0, true, false);
                    break;
                case "helm of might":
                    if (!Save.game.usedHelm) {
                        if (s.player.stamina >= 3) {
                            s.soundManager.PlayClip("fwoosh");
                            // need 3 stamina
                            Save.game.usedHelm = true;
                            if (s.tutorial == null) { Save.SaveGame(); }
                            // set variable
                            s.turnManager.SetStatusText("you feel mighty");
                            // notify player
                            s.turnManager.ChangeStaminaOf("player", -3);
                            // use stamina
                            s.diceSummoner.GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", isFromMight:true);
                            // add yellow die to red stat
                        }
                        else { s.turnManager.SetStatusText("not enough stamina"); }
                        // notify player
                    }
                    else { s.turnManager.SetStatusText("helm can help you no further"); }
                    // notify player
                    break;
                case "kapala":
                    s.turnManager.SetStatusText("offer an item to become furious");
                    break;
                case "boots of dodge":
                    if (!Save.game.usedBoots) {
                        if (s.player.stamina >= 1) {
                            s.soundManager.PlayClip("fwoosh");
                            s.turnManager.SetStatusText("you feel dodgy");
                            Save.game.usedBoots = true;
                            if (s.tutorial == null) { Save.SaveGame(); }
                            s.turnManager.ChangeStaminaOf("player", -1);
                            s.player.SetPlayerStatusEffect("dodge", true);
                        }
                        else { s.turnManager.SetStatusText("not enough stamina"); }
                    }
                    else { s.turnManager.SetStatusText("you are already dodgy"); }
                    break;
                case "ankh":
                    if (!Save.game.usedAnkh) {
                        s.soundManager.PlayClip("click0");
                        Save.game.usedAnkh = true;
                        if (s.tutorial == null) { Save.SaveGame(); }
                        foreach (string key in s.itemManager.statArr) {
                            s.turnManager.ChangeStaminaOf("player", s.statSummoner.addedPlayerStamina[key]);
                            s.statSummoner.addedPlayerStamina[key] = 0;
                            // refund stamina
                        }
                        s.statSummoner.ResetDiceAndStamina();
                        s.diceSummoner.SummonDice(false, true);
                        s.statSummoner.SummonStats();
                        s.turnManager.DetermineMove(true);
                    }
                    else { s.turnManager.SetStatusText("ankh glows with red light"); }
                    break;
                // default: print("rare item with invalid name found!"); break;
            }
        }
    }

    /// <summary>
    /// Remove an item from the player's inventory.
    /// </summary>
    public void Remove(bool drop = false, bool selectNew = true, bool armorFade = false, bool torchFade = false, bool dontSave = false) {
        if (drop && !s.itemManager.floorItems.Contains(gameObject) && !s.turnManager.isMoving) {
            if (s.tutorial != null && modifier == "nothing" && s.tutorial.curIndex == 2 && !s.tutorial.isAnimating) {
                s.tutorial.Increment();
            }
            // all conditions must hold for the drop to trigger kapala fury
            bool offeringToKapala = s.itemManager.PlayerHas("kapala") && s.levelManager.sub != 4
                && itemType != "weapon" && s.enemy.enemyName.text != "Tombstone" && !Save.game.enemyIsDead;
            if (offeringToKapala) {
                bool wasntAlreadyFurious = s.player.SetPlayerStatusEffect("fury", true);
                if (wasntAlreadyFurious) {
                    s.turnManager.SetStatusText("deity accepts your offering... you feel furious");
                    s.soundManager.PlayClip("fwoosh");
                    s.diceSummoner.MakeAllAttachedYellow();
                }
            } else {
                if (s.itemManager.IsVendorEncounter()) { Save.game.numItemsDroppedForTrade++; }
                if (s.tutorial == null) { Save.SaveGame(); }
                if (itemName == "necklet") {
                    s.turnManager.SetStatusText(modifier == "arcane"
                        ? "you drop arcane necklet"
                        : $"you drop {itemName} of {modifier}");
                }
                else if (itemName == "potion" || itemName == "scroll") { s.turnManager.SetStatusText($"you drop {itemName} of {modifier}"); }
                else { s.turnManager.SetStatusText($"you drop {itemName}"); }
                if (s.itemManager.IsVendorEncounter()) { s.itemManager.UpdateVendorUIForSelection(); }
            }
        }
        int index = s.itemManager.curList.IndexOf(gameObject);
        Item removedItem = s.player.inventory[index].GetComponent<Item>();
        if (removedItem.itemName == "necklet") {
            if (removedItem.modifier == "arcane") {
                s.itemManager.neckletCounter["arcane"]--;
                foreach (string stat in s.itemManager.statArr) {
                    s.itemManager.neckletStats[stat] = s.itemManager.neckletCounter["arcane"] * s.itemManager.neckletCounter[stat];
                }
            } else {
                string t = s.itemManager.statArr[Array.IndexOf(s.itemManager.neckletTypes, removedItem.modifier)];
                s.itemManager.neckletCounter[t]--;
                s.itemManager.neckletStats[t] = s.itemManager.neckletCounter["arcane"] * s.itemManager.neckletCounter[t];
            }
            s.statSummoner.SummonStats();
            s.statSummoner.SetDebugInformationFor("player");
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
        if (!dontSave) { s.itemManager.SaveInventoryItems(); }
    }

    /// <summary>
    /// Shift the items in the current list over, starting from a given index.
    /// </summary>
    private void ShiftItems(int index, bool selectNew) {
        s.itemManager.curList.RemoveAt(index);
        // remove the item from the list
        for (int i = index; i < s.player.inventory.Count; i++) {
            // for each item in the inventory after the index of the previous one
            s.player.inventory[i].transform.position = new Vector2(s.player.inventory[i].transform.position.x - 1f, 3.16f);
            // shift over each item
        }
        if (selectNew) { s.itemManager.Select(s.itemManager.curList, index, playAudio: true); }
        // select the next item over if needed
    }

    /// <summary>
    /// Coroutine to break player's armor when it is hit.
    /// </summary>
    private IEnumerator FadeArmor(int index, bool selectNew) {
        yield return s.delays[0.05f];
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color temp = sr.color;
        temp.a = 1;
        sr.color = temp;
        for (int i = 0; i < 5; i++)
        {
            yield return s.delays[0.033f];
            temp.a -= 1f / 5f;
            sr.color = temp;
        }
        Destroy(gameObject);
        ShiftItems(index, selectNew);
        s.itemManager.SaveInventoryItems();
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
            yield return s.delays[0.033f];
            temp.a -= 1f / 10f;
            sr.color = temp;
        }
        for (int i = 0; i < 10; i++) {
            yield return s.delays[0.033f];
            temp.a += 1f / 10f;
            sr.color = temp;
        }
        for (int i = 0; i < 8; i++) {
            yield return s.delays[0.033f];
            temp.a -= 1f / 8f;
            sr.color = temp;
        }
        for (int i = 0; i < 8; i++) {
            yield return s.delays[0.033f];
            temp.a += 1f / 8f;
            sr.color = temp;
        }
        for (int i = 0; i < 6; i++) {
            yield return s.delays[0.033f];
            temp.a -= 1f / 6f;
            sr.color = temp;
        }
        // flash in and out, faster and faster
        Destroy(gameObject);
        ShiftItems(index, selectNew);
        // destroy it and shift over
    }
}
