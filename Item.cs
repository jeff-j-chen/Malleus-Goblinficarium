using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public Scripts scripts;
    public string itemName;
    public string itemType;
    public string modifier;
    public Dictionary<string, int> weaponStats = new Dictionary<string, int>();

    void Awake() {
        scripts = FindObjectOfType<Scripts>();
    }

    void Start() {
        if (itemName == "torch")  {
            // if the item is a torch
            if (UnityEngine.Random.Range(0, 2) == 0)  { 
                // 1/2 chance
                if (scripts.levelManager.sub != 4) { modifier = $"{scripts.levelManager.level + 1}-{scripts.levelManager.sub}";  }
                else { modifier = $"{scripts.levelManager.level + 1}-1";  }
                // set fade time
            }
            else  {
                // 1/2 chance
                if (scripts.levelManager.sub + 1 == 4 || scripts.levelManager.sub + 1 == 5) { modifier = $"{scripts.levelManager.level + 1}-2"; }
                else { modifier = $"{scripts.levelManager.level + 1}-{scripts.levelManager.sub + 1}"; }
                // set fade time to be slightly longer
            }
        }
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            // left click
            if (scripts.itemManager.highlightedItem == gameObject) { Use(); }
            // if clicking over highlighted weapon, use it
            else  { 
                Select();
                // otherwise select it
                if (scripts.player.inventory.Contains(gameObject)) { scripts.itemManager.curList = scripts.player.inventory; }
                // selection occured in inventory, so assign the curlist variable as such
                else { scripts.itemManager.curList = scripts.itemManager.floorItems; }
                // selection was on floor so         "  "               
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            // right click
            if (scripts.itemManager.highlightedItem == gameObject)  { 
                // if highlighted
                if (itemType != "weapon" &&  !scripts.itemManager.floorItems.Contains(gameObject)) { Remove(true);  }
                // if the item is not weapon and not on the floor, drop the item
            }
        }
    }
    
    /// <summary>
    /// Select an item.
    /// </summary>
    public void Select(bool playAudio=true)  {
        if (itemType == "weapon") {
            // if the item is a weapon    
            if (scripts.itemManager.descriptionDict[itemName.Split(' ')[1]] == "") { scripts.itemManager.itemDesc.text = itemName; }
            // if no description, just display the itemname
            else { scripts.itemManager.itemDesc.text = $"{itemName}\n- {scripts.itemManager.descriptionDict[itemName.Split(' ')[1]]}"; }
            // if description, then display it
            if (scripts.itemManager.floorItems.Contains(gameObject)) {
                // if item on the floor
                scripts.enemy.stats = weaponStats;
                scripts.statSummoner.SummonStats();
                scripts.statSummoner.SetDebugInformationFor("enemy");
                scripts.turnManager.blackBox.transform.position = scripts.turnManager.offScreen;
                // display the stats for the player to see
            }
            else {
                // if item is in inventory
                if (scripts.enemy.isDead) { scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen; }
                // hide the weapon stats if enemy is dead and not clicking on an enemy
            }
        }
        else {
            if (!scripts.levelManager.lockActions) {
                // only allow weapons to be selected when locked
                if (scripts.enemy.isDead) { scripts.turnManager.blackBox.transform.position = scripts.turnManager.onScreen; }
                // hide the weapon stats if enemy is dead and not clicking on an enemy
                if (itemName == "potion")  {
                    if (modifier == "accuracy") {  scripts.itemManager.itemDesc.text = "potion of accuracy\n+3 accuracy"; }
                    else if (modifier == "speed") {  scripts.itemManager.itemDesc.text = "potion of speed\n+3 speed"; }
                    else if (modifier == "strength") {  scripts.itemManager.itemDesc.text = "potion of strength\n+3 damage"; }
                    else if (modifier == "defense") {  scripts.itemManager.itemDesc.text = "potion of parry\n+3 parry"; }
                    else if (modifier == "might") {  scripts.itemManager.itemDesc.text = "potion of might\ngain a yellow die"; }
                    else if (modifier == "life") {  scripts.itemManager.itemDesc.text = "potion of life\nheal all wounds"; }
                    else if (modifier == "nothing") {  scripts.itemManager.itemDesc.text = "potion of nothing\ndoes nothing"; }
                    else { print("invalid potion modifier detected"); }
                }
                else if (itemName == "scroll")  { 
                    if (modifier == "fury") { scripts.itemManager.itemDesc.text = "scroll of fury\nall picked dice turn yellow"; }
                    else if (modifier == "haste") { scripts.itemManager.itemDesc.text = "scroll of haste\npick 3 dice, enemy gets the rest"; }
                    else if (modifier == "dodge") { scripts.itemManager.itemDesc.text = "scroll of dodge\nif you strike first, ignore all damage"; }
                    else if (modifier == "leech") { scripts.itemManager.itemDesc.text = "scroll of leech\ncure the same wound as inflicted"; }
                    else if (modifier == "courage") { scripts.itemManager.itemDesc.text = "scroll of courage\nkeep 1 of your die till next round"; }
                    else if (modifier == "challenge") { scripts.itemManager.itemDesc.text = "scroll of challenge\n???"; }
                    else if (modifier == "nothing") { scripts.itemManager.itemDesc.text = "scroll of nothing\ndoes nothing"; }
                }
                else if (itemName == "necklet") {
                    int t = scripts.itemManager.neckletCounter["arcane"];
                    if (modifier == "arcane") { scripts.itemManager.itemDesc.text = $"arcane necklet\nall necklets are more effective"; }
                    else if (modifier == "nothing") { scripts.itemManager.itemDesc.text = $"necklet of nothing\ndoes nothing"; }
                    else if (modifier == "victory") { scripts.itemManager.itemDesc.text = $"necklet of victory"; }
                    else { scripts.itemManager.itemDesc.text = $"necklet of {modifier}\n+{t} {scripts.itemManager.statArr1[Array.IndexOf(scripts.itemManager.neckletTypes, modifier)]}"; } 
                }
                else if (itemName == "cheese" || itemName == "steak") {
                    if (scripts.player.charNum == 0) { scripts.itemManager.itemDesc.text = $"{itemName}\n+{int.Parse(scripts.itemManager.descriptionDict[itemName]) + 2} stamina"; }
                    else { scripts.itemManager.itemDesc.text = $"{itemName}\n+{scripts.itemManager.descriptionDict[itemName]} stamina"; }
                }
                else if (itemName == "moldy cheese") {
                    scripts.itemManager.itemDesc.text = "moldy cheese\n+0 stamina";
                }
                else if (itemName == "rotten steak") {
                    scripts.itemManager.itemDesc.text = "rotten steak\n+0 stamina";
                }
                else if (itemName == "arrow" || itemName == "retry") { scripts.itemManager.itemDesc.text = scripts.itemManager.descriptionDict[itemName]; }
                else { scripts.itemManager.itemDesc.text = $"{itemName}\n{scripts.itemManager.descriptionDict[itemName]}"; }
                // set the proper item descriptions for all items
            }
        }
        if (!scripts.levelManager.lockActions || itemType == "weapon") {
            // only allow weapons to be used when locked
            scripts.itemManager.highlight.transform.position = transform.position;
            // move the highlight to the selected item
            scripts.itemManager.highlightedItem = gameObject;
            // update the highlighted item variable
            scripts.itemManager.col = scripts.itemManager.curList.IndexOf(gameObject);
            // update the col variable
            if (playAudio) { scripts.soundManager.PlayClip("click"); }
            // play sound clip
        }
    }

    /// <summary>
    /// Pick up or use an item.
    /// </summary>
    public void Use() {
        if (!scripts.levelManager.lockActions) {
            if (scripts.itemManager.floorItems.Contains(gameObject)) {
                // if item is on the floor
                if (itemType == "arrow")  { 
                    // if the item is arrow (next level indicator)
                    scripts.levelManager.NextLevel();
                    scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                    return;
                    // go to the next level and end the level
                }
                else  { 
                    // not an arrow
                    if (scripts.levelManager.sub == 4)  { 
                        // if on the trader level
                        if (scripts.itemManager.numItemsDroppedForTrade > 0)  { 
                            // if player has dropped items for trading
                            scripts.itemManager.numItemsDroppedForTrade--;
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
                    scripts.levelManager.lockActions = true;
                    Initiate.Fade("Game", Color.black, scripts.backToMenu.transitionMultiplier);
                    // reload scene
                    scripts.soundManager.PlayClip("next");
                    // play sound clip
                }
                else if (!scripts.turnManager.isMoving && scripts.player.inventory.Contains(gameObject)) {
                    // in player's inventory and not moving, MUST HAVE CHECK FOR INVENTORY HERE BECAUSE OTHERWISE IT BREAKS
                    if (itemType == "weapon")  { 
                        // if player is trying to use weapon
                        if (!scripts.turnManager.isMoving && !scripts.player.isDead) {
                            // if conditions allow for attack
                            if (scripts.enemy.isDead) { scripts.turnManager.SetStatusText("he's dead"); }
                            else if (scripts.levelManager.sub == 4 || scripts.enemy.enemyName.text == "Tombstone") { scripts.turnManager.SetStatusText("mind your manners"); }
                            // send reminders accordingly
                            else if (!scripts.enemy.isDead) { scripts.player.UseWeapon(); }
                            // attack if enemy is not dead or tombstone
                            else { print("error!"); }
                        }
                    }
                    else if (itemType == "common") { UseCommon(); }
                    else if (itemType == "rare") { UseRare(); }
                    // not item, so use corresponding item type
                }
            }
        }
    }

    /// <summary>
    /// Use a common item. 
    /// </summary>
    private void UseCommon() {
        if (!scripts.levelManager.lockActions) {
            // don't use items when locked
            if (itemName == "steak")  {
                scripts.soundManager.PlayClip("eat");
                // play sound clip
                if (scripts.player.charNum == 0)  { scripts.turnManager.ChangeStaminaOf("player", 7); }
                else { scripts.turnManager.ChangeStaminaOf("player", 5); }
                // change stamina based on the character
                scripts.turnManager.SetStatusText("you swallow steak");
                // status text
                Remove();
                // remove from player inventory
            }
            else if (itemName == "rotten steak") {
                // don't change stamina for rotten foods
                scripts.soundManager.PlayClip("eat");
                scripts.turnManager.SetStatusText("you swallow rotten steak");
                Remove();
            }
            else if (itemName == "cheese")  {
                scripts.soundManager.PlayClip("eat");
                if (scripts.player.charNum == 0)  { scripts.turnManager.ChangeStaminaOf("player", 5); }
                else { scripts.turnManager.ChangeStaminaOf("player", 3); }
                scripts.turnManager.SetStatusText("you swallow cheese");
                Remove();
            }
            else if (itemName == "moldy cheese") {
                scripts.soundManager.PlayClip("eat");
                scripts.turnManager.SetStatusText("you swallow moldy cheese");
                Remove();
            }
            else if (itemName == "scroll" && scripts.levelManager.sub != 4) {
                // don't let scrolls be used at trader
                if (modifier != "challenge") {
                    scripts.soundManager.PlayClip("fwoosh");
                    // play sound clip (not for challenge)
                }
                if (modifier == "fury") {
                    if (scripts.player.isFurious) { scripts.turnManager.SetStatusText("you are already furious"); }
                    // prevent player from accidentally using two scrolls
                    else
                    {
                        scripts.player.SetPlayerStatusEffect("fury", true);
                        // turn on fury
                        scripts.turnManager.SetStatusText("you read scroll of fury... you feel furious");
                        MakeAllAttachedYellow();
                        Remove();
                        // consume the scroll
                    }
                }
                else if (modifier == "dodge") {
                    if (scripts.player.isDodgy) { scripts.turnManager.SetStatusText("you are already dodgy"); }
                    // prevent player from accidentally using two scrolls
                    else {
                        scripts.player.SetPlayerStatusEffect("dodge", true);
                        // turn on dodge
                        scripts.turnManager.SetStatusText("you read scroll of dodge... you feel dodgy");
                        Remove();
                        // consume and notify player
                    }
                }
                else if (modifier == "haste") {
                    if ((from a in scripts.diceSummoner.existingDice where a.GetComponent<Dice>().isAttached == false select a).ToList().Count == 0) {
                        scripts.turnManager.SetStatusText("all dice have been chosen");
                        // prevent player from wasting scroll
                    }
                    else {
                        if (scripts.player.isHasty) { scripts.turnManager.SetStatusText("you are already winged"); }
                        // prevent player from accidentally using two scrolls
                        else
                        {
                            scripts.player.SetPlayerStatusEffect("haste", true);
                            // turn on haste
                            scripts.turnManager.SetStatusText("you read scroll of haste... you feel winged");
                            Remove();
                            // consume and notify player
                        }
                    }
                    
                }
                else if (modifier == "leech") {
                    if (scripts.player.isBloodthirsty) { scripts.turnManager.SetStatusText("you are already bloodthirsty"); }
                    // prevent player from accidentally using two scrolls
                    else {
                        scripts.player.SetPlayerStatusEffect("leech", true);
                        // turn on leech
                        scripts.turnManager.SetStatusText("you read scroll of leech... you feel bloodthirsty");
                        Remove();
                        // consume and notify player
                    }
                }
                else if (modifier == "courage")  {
                    if (scripts.player.isCourageous) { scripts.turnManager.SetStatusText("you are already courageous"); }
                    // prevent player from accidentally using two scrolls
                    else {
                        scripts.player.SetPlayerStatusEffect("courage", true);
                        // turn on courage
                        scripts.turnManager.SetStatusText("you read scroll of courage... you feel courageous");
                        Remove();
                        // consume and notify player
                    }
                }
                else if (modifier == "challenge") {
                    scripts.levelManager.NextLevel(true);
                    // load lich level
                    scripts.itemManager.Select(scripts.player.inventory, 0, true, false);
                    Remove();
                }
                else if (modifier == "nothing") {
                    scripts.turnManager.SetStatusText("you read scroll of nothing... nothing happens");
                    Remove();
                    // consume and notfiy player
                }
            }
            else if (itemName == "potion" && scripts.levelManager.sub != 4) {
                // don't let potions be used at lich
                scripts.soundManager.PlayClip("gulp");
                scripts.turnManager.SetStatusText($"you quaff potion of {modifier}");
                // notify player
                if (modifier == "accuracy") {
                    scripts.player.potionStats["green"] += 3;
                    ShiftDiceAccordingly("green", 3);
                }
                else if (modifier == "speed") {
                    scripts.player.potionStats["blue"] += 3;
                    ShiftDiceAccordingly("blue", 3);
                }
                else if (modifier == "strength") {
                    scripts.player.potionStats["red"] += 3;
                    ShiftDiceAccordingly("red", 3);
                }
                else if (modifier == "defense") {
                    scripts.player.potionStats["white"] += 3;
                    ShiftDiceAccordingly("white", 3);
                }
                // increase stat temporarily via scripts.player.potionStats if it is a stat potion
                else if (modifier == "might") {
                    scripts.diceSummoner.GenerateSingleDie(UnityEngine.Random.Range(1, 7), "yellow", "player", "red");
                    // create a yellow die and add it to player's attack
                }
                else if (modifier == "life") {
                    scripts.player.woundList.Clear();
                    scripts.turnManager.DisplayWounds();
                    // heal and display the wounds
                    // injuredtextchange does not want to work for some reason
                    // StartCoroutine(scripts.turnManager.InjuredTextChange(scripts.player.woundGUIElement));
                }
                else if (modifier == "nothing") {}
                else { print("invalid potion modifier detected"); }
                scripts.statSummoner.SummonStats();
                scripts.statSummoner.SetDebugInformationFor("player");
                Remove();
            }
            else if (itemName == "shuriken" && scripts.levelManager.sub != 4) {
                scripts.soundManager.PlayClip("shuriken");
                // play sound clip
                scripts.itemManager.discardableDieCounter++;
                // increment counter
                Remove();
            }
            else if (itemName == "skeleton key" && scripts.levelManager.sub != 4) { 
                // don't allow usage on trader levels
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
            }
        }
    }

    /// <summary>
    /// Use a rare item. 
    /// </summary>
    private void UseRare() {
        // these are pretty self explanatory
        if (!scripts.levelManager.lockActions) {
            if (itemName == "helm of might" && scripts.levelManager.sub != 4) {
                if (!scripts.itemManager.usedHelm) {
                    scripts.soundManager.PlayClip("fwoosh");
                    if (scripts.player.stamina >= 3) {
                        // need 3 stamina
                        scripts.itemManager.usedHelm = true;
                        // set variable
                        scripts.turnManager.SetStatusText("you feel mighty");
                        // notify player
                        scripts.turnManager.ChangeStaminaOf("player", -3);
                        // use stamina
                        scripts.diceSummoner.GenerateSingleDie(UnityEngine.Random.Range(1, 7), "yellow", "player", "red");
                        // add yellow die to red stat
                    }
                    else { scripts.turnManager.SetStatusText("not enough stamina"); }
                    // notify player
                }
                else { scripts.turnManager.SetStatusText("helm can help you no further"); }
                // notfiy player
            }
            // these are pretty self explanatory
            else if (itemName == "kapala" && scripts.levelManager.sub != 4) { scripts.turnManager.SetStatusText("offer an item to become furious"); }
            else if (itemName == "boots of dodge" && scripts.levelManager.sub != 4) {
                if (!scripts.itemManager.usedBoots) {
                    scripts.soundManager.PlayClip("fwoosh");
                    if (scripts.player.stamina >= 1) {
                        scripts.turnManager.SetStatusText("you feel dodgy");
                        scripts.itemManager.usedBoots = true;
                        scripts.turnManager.ChangeStaminaOf("player", -1);
                        scripts.player.SetPlayerStatusEffect("dodge", true);
                    }
                    else { scripts.turnManager.SetStatusText("not enough stamina"); }
                }
                else { scripts.turnManager.SetStatusText("boots can help you no further"); }
            }
            else if (itemName == "ankh" && scripts.levelManager.sub != 4) {
                if (!scripts.itemManager.usedAnkh) {
                    scripts.soundManager.PlayClip("click");
                    scripts.itemManager.usedAnkh = true;
                    foreach (string key in scripts.itemManager.statArr) {
                        scripts.turnManager.ChangeStaminaOf("player", scripts.statSummoner.addedPlayerStamina[key]);
                        scripts.statSummoner.addedPlayerStamina[key] = 0;
                        // refund stamina
                    }
                    scripts.statSummoner.ResetDiceAndStamina();
                    scripts.diceSummoner.SummonDice(false);
                    scripts.statSummoner.SummonStats();
                }
                else { scripts.turnManager.SetStatusText("ankh glows with red light"); }
            }
        }
    }

    /// <summary>
    /// Shift all the die of a stat.
    /// </summary>
    /// <param name="stat">The stat of which die to shift.</param>
    /// <param name="shiftAmount">The amount of stat squares to shift each die by. </param>
    private void ShiftDiceAccordingly(string stat, int shiftAmount) {
        foreach (Dice dice in scripts.statSummoner.addedPlayerDice[stat]) {
            // for every die in the specified stat
            dice.transform.position = new Vector2(dice.transform.position.x + scripts.statSummoner.xOffset * shiftAmount, dice.transform.position.y);
            // shift the die by the specified amount
            dice.instantiationPos = dice.transform.position;
            // update the instantiation position
        }
        scripts.statSummoner.SetDebugInformationFor("player");
    }

    /// <summary>
    /// Make all die attached to the player into yellow.
    /// </summary>
    private void MakeAllAttachedYellow()
    {
        // notfiy player
        foreach (GameObject dice in scripts.diceSummoner.existingDice)
        {
            // for every die
            if (dice.GetComponent<Dice>().isAttached && dice.GetComponent<Dice>().isOnPlayerOrEnemy == "player")
            {
                // if the die is attached to the player
                dice.GetComponent<Dice>().GetComponent<SpriteRenderer>().color = Color.black;
                dice.GetComponent<Dice>().transform.GetChild(0).GetComponent<SpriteRenderer>().color = scripts.colors.yellow;
                dice.GetComponent<Dice>().diceType = scripts.colors.colorNameArr[4];
                // make the die yellow
                dice.GetComponent<Dice>().moveable = true;
                // allow for moving the die around
            }
        }
    }
    public void Remove(bool drop=false, bool selectNew=true) {
        if (drop) { 
            if (!scripts.itemManager.floorItems.Contains(gameObject)) {
                if (!scripts.turnManager.isMoving) {
                    // if the item is being dropped
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
                            MakeAllAttachedYellow();
                        }
                    }
                    else {
                        if (scripts.levelManager.sub == 4) { scripts.itemManager.numItemsDroppedForTrade++; }
                        // if trader level increment the number of items dropped for trading
                        if (itemType == "weapon") { 
                            scripts.turnManager.SetStatusText($"you drop {scripts.itemManager.descriptionDict[itemName.Split(' ')[1]]}"); 
                        }
                        if (itemName == "necklet")  { 
                            if (modifier == "arcane") { scripts.turnManager.SetStatusText($"you drop arcane necklet"); }
                            else { scripts.turnManager.SetStatusText($"you drop {itemName} of {modifier}"); }
                        }
                        else if (itemName == "potion" || itemName == "scroll") { scripts.turnManager.SetStatusText($"you drop {itemName} of {modifier}"); }
                        else { scripts.turnManager.SetStatusText($"you drop {itemName}"); }
                        // notify player that item has been dropped
                    }
                }
            }
        }
        int index = scripts.itemManager.curList.IndexOf(gameObject);
        // get the index of the object from the current selected list
        if (scripts.player.inventory[index].GetComponent<Item>().itemName == "necklet") {
            // if removing a necklet
            if (scripts.player.inventory[index].GetComponent<Item>().modifier != "arcane") {
                // not an arcane necklet
                string t = scripts.itemManager.statArr[Array.IndexOf(scripts.itemManager.neckletTypes, scripts.player.inventory[index].GetComponent<Item>().modifier)];
                // get a string refernce for the necklet's stat
                scripts.itemManager.neckletCounter[t]--;
                // decrement counter
                scripts.itemManager.neckletStats[t] = scripts.itemManager.neckletCounter["arcane"] * scripts.itemManager.neckletCounter[t];
                // adjust stats added from that necklet type
            }
            else {
                // is an arcane necklet
                scripts.itemManager.neckletCounter["arcane"]--;
                // decrement counter
                foreach (string stat in scripts.itemManager.statArr)  { 
                    // for every stat
                    scripts.itemManager.neckletStats[stat] = scripts.itemManager.neckletCounter["arcane"] * scripts.itemManager.neckletCounter[stat]; 
                    // adjust stats based on new number of arcane necklets
                }
            }
            scripts.statSummoner.SummonStats();
            scripts.statSummoner.SetDebugInformationFor("player");
            // update stuff
        }
        scripts.itemManager.curList.RemoveAt(index);
        // remove the item from the list
        for (int i = index; i < scripts.player.inventory.Count; i++)  {
            // for each item in the inventory after the index of the previous one
            scripts.player.inventory[i].transform.position = new Vector2(scripts.player.inventory[i].transform.position.x - 1f, 3.16f);
            // shift over each item
        }
        if (selectNew) { scripts.itemManager.Select(scripts.itemManager.curList, index); }
        // select the next item over if needed
        Destroy(gameObject);
        // destroy the object
    }
}
