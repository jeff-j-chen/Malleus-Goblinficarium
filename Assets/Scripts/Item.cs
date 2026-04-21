using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class Item : MonoBehaviour {
    private const int PotionStatBonus = 5;
    [SerializeField] public Scripts s;
    [SerializeField] public string itemName;
    [SerializeField] public string itemType;
    [SerializeField] public string modifier;
    public Dictionary<string, int> weaponStats = new();
    private bool preventPlayingFX = true;

    void Awake() {
        s = FindFirstObjectByType<Scripts>();
    }

    private IEnumerator AllowFX() {
        yield return new WaitForSeconds(0.45f);
        preventPlayingFX = false;
    }

    private void Start() {
        gameObject.name = itemName;
        // set the object's name to its itemname, so it can be identified in the editor
        if (itemName == "torch" && s.player != null && string.IsNullOrWhiteSpace(modifier)) {
            // if the item is a torch
            modifier = s.itemManager != null ? s.itemManager.RollTorchFadeModifier() : "rooms:4";
            // store remaining combat rooms until fade
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
        if (itemName == "hint") {
            // almanac placeholder — show discovery text then exit
            s.itemManager.highlight.transform.position = transform.position;
            s.itemManager.highlightedItem = gameObject;
            s.itemManager.itemDesc.text = "???\nnot yet discovered";
            return;
        }
        if (itemType == "weapon") {
            // if the item is a weapon
            s.itemManager.itemDesc.text = s.itemManager.GetDisplayTextForItem(this);
            if (s.itemManager.ShouldPreviewWeaponOnRight(gameObject)) {
                s.enemy.stats = s.itemManager.GetWeaponStatsForPreview(this);
                s.statSummoner.SummonStats();
                s.statSummoner.SetDebugInformationFor("enemy");
                s.turnManager.RefreshBlackBoxVisibility(gameObject);
            }
            else {
                if (!s.itemManager.IsFightableEncounter()) {
                    s.itemManager.RestoreCurrentEnemyStatsForDisplay();
                }
                if (s.enemy != null) { s.turnManager.RefreshBlackBoxVisibility(gameObject); }
            }
        }
        else {
            if (s.levelManager == null || !s.levelManager.lockActions) {
                // only allow weapons to be selected when locked
                // turnmanager might not exist in almanac
                if (!s.itemManager.IsFightableEncounter()) {
                    s.itemManager.RestoreCurrentEnemyStatsForDisplay();
                }
                if (s.turnManager != null) {
                    s.turnManager.RefreshBlackBoxVisibility(gameObject);
                }
                s.itemManager.itemDesc.text = s.itemManager.GetDisplayTextForItem(this);
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
            if (s.itemManager.TryConsumeMerchantStealAllowance()) {
                Save.persistent.itemsStolen++;
                s.itemManager.MoveToInventory(idx, "you steal");
            }
            else {
                Save.persistent.itemsTraded++;
                s.itemManager.MoveToInventory(idx);
            }
            return;
        }
        if (s.itemManager.IsBlacksmithEncounter()) { PickupFromBlacksmith(idx); return; }
        s.itemManager.MoveToInventory(idx);
    }

    private void PickupFromBlacksmith(int idx) {
        if (itemType == "weapon") { s.itemManager.MoveToInventory(idx); return; }
        if (itemName != "forge") { s.turnManager.SetStatusText("drop an item to trade"); return; }
        if (Save.game.numItemsDroppedForTrade <= 0) { s.turnManager.SetStatusText("drop an item to trade"); return; }
        Save.game.numItemsDroppedForTrade--;
        Save.persistent.itemsTraded++;
        s.soundManager.PlayClip("forge");
        s.itemManager.ForgePlayerWeapon(modifier);
        // s.turnManager.SetStatusText($"your {ItemManager.GetWeaponBaseName(s.player.inventory[0].GetComponent<Item>().itemName)} is forged");
        s.itemManager.RemoveFloorItemAt(idx);
        s.itemManager.UpdateVendorUIForSelection();
    }

    /// <summary>
    /// Use a common item.
    /// </summary>
    private void UseCommon() {
        if (!Save.game.enemyIsDead && s.enemy.enemyName.text is not "Tombstone" and not "Merchant" and not "Blacksmith") {
            StartCoroutine(UseCommonCoro());
            return;
        }

        if (itemName is "steak" or "cheese" or "retry" or "arrow" or "campfire" or "tincture"
            || itemName == "potion" && modifier == "life") {
            // only these specific items can be used at a merchant/tombstone, otherwise it doesn't make sense or is a waste
            StartCoroutine(UseCommonCoro());
        }
    }

    private bool IsInFightableCombat() {
        return !s.player.isDead && s.itemManager.IsFightableEncounter();
    }

    private void ConsumeCommonItemAndReselect() {
        Remove();
        s.itemManager.Select(s.player.inventory, 0, true, false);
    }

    private void UseCampfire() {
        if (s.player.woundList.Count == 0) {
            s.turnManager.SetStatusText("you're healthy");
            return;
        }

        if (!Save.game.enemyIsDead && !s.itemManager.IsVendorEncounter()) {
            s.turnManager.SetStatusText("you can't rest now");
            return;
        }

        s.soundManager.PlayClip("fwoosh");
        s.turnManager.SetStatusText("you feel rested");
        s.itemManager.MarkItemUsed(this);
        s.turnManager.ChangeStaminaOf("player", 1);
        s.turnManager.AnimatePlayerWoundRefresh("blip0", () => {
            s.player.woundList.Clear();
            Save.game.playerWounds = s.player.woundList;
            s.turnManager.SetBleedOutNextRound("player", false, saveGame:false);
            RefreshPlayerAfterHealingWounds();
        });
        ConsumeCommonItemAndReselect();
    }

    private void UseTincture() {
        string mostRecentWound = s.player.woundList.LastOrDefault();
        if (string.IsNullOrEmpty(mostRecentWound)) {
            s.turnManager.SetStatusText("you're healthy");
            return;
        }

        if (mostRecentWound == "neck") {
            s.turnManager.SetStatusText("it won't save you");
            return;
        }

        s.soundManager.PlayClip("gulp");
        s.turnManager.SetStatusText("you quaff tincture");
        s.itemManager.MarkItemUsed(this);
        s.turnManager.AnimatePlayerWoundRefresh("blip0", () => {
            s.player.woundList.Remove(mostRecentWound);
            Save.game.playerWounds = s.player.woundList;
            s.turnManager.SetBleedOutNextRound("player", s.player.woundList.Contains("neck"), saveGame:false);
            RefreshPlayerAfterHealingWounds();
        });
        ConsumeCommonItemAndReselect();
    }

    private void RefreshPlayerAfterHealingWounds() {
        if (s == null || s.itemManager == null) { return; }

        s.itemManager.RefreshPlayerCombatStatsAndDice();
    }

    private void UseMirror() {
        if (!IsInFightableCombat()) {
            s.turnManager.SetStatusText("nothing to copy");
            return;
        }

        if (s.diceSummoner.CountUnattachedDice() > 0) {
            s.turnManager.SetStatusText("draft your die");
            return;
        }

        s.soundManager.PlayClip("cloak");
        s.turnManager.SetStatusText("the mirror shatters");
        s.itemManager.MarkItemUsed(this);
        Save.game.pendingMirrorCopy = true;
        ConsumeCommonItemAndReselect();
    }

    private void UseUnstableSpellbook() {
        if (!IsInFightableCombat()) {
            s.turnManager.SetStatusText("nothing to transmute");
            return;
        }

        if (Save.game.pendingSpellbookTransmute) {
            s.turnManager.SetStatusText("choose a die");
            return;
        }

        if (Save.game.usedSpellbook) {
            s.turnManager.SetStatusText("it's too dangerous");
            return;
        }

        if (s.player.stamina < 3) {
            s.turnManager.SetStatusText("not enough stamina");
            return;
        }

        s.soundManager.PlayClip("fwoosh");
        s.turnManager.ChangeStaminaOf("player", -3);
        s.turnManager.SetStatusText("choose a die");
        s.itemManager.MarkItemUsed(this);
        Save.game.usedSpellbook = true;
        Save.game.pendingSpellbookTransmute = true;
        if (s.tutorial == null) { Save.SaveGame(); }
    }

    private void UseWitchHand() {
        if (!IsInFightableCombat()) {
            s.turnManager.SetStatusText("nothing to curse");
            return;
        }

        foreach (string stat in s.itemManager.statArr) {
            s.enemy.stats[stat] = Mathf.Max(0, s.enemy.stats[stat] - 1);
        }
        string randomStat = s.itemManager.statArr[Random.Range(0, s.itemManager.statArr.Length)];
        s.enemy.stats[randomStat] = Mathf.Max(0, s.enemy.stats[randomStat] - 1);
        Save.game.enemyAcc = s.enemy.stats["green"];
        Save.game.enemySpd = s.enemy.stats["blue"];
        Save.game.enemyDmg = s.enemy.stats["red"];
        Save.game.enemyDef = s.enemy.stats["white"];
        s.itemManager.MarkItemUsed(this);
        s.soundManager.PlayClip("scream");
        s.statSummoner.SummonStats();
        s.statSummoner.SetCombatDebugInformationFor("enemy");
        s.turnManager.RecalculateMaxFor("enemy");
        ConsumeCommonItemAndReselect();
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
                    s.itemManager.MarkItemUsed(this);
                    s.soundManager.PlayClip("eat");
                    s.turnManager.ChangeStaminaOf("player", s.itemManager.GetFoodStaminaAmount("steak"));
                    // change stamina based on the character
                    s.turnManager.SetStatusText("you swallow steak");
                    // status text
                    Remove();
                    // remove from player inventory
                    break;
                case "rotten steak":
                    Save.persistent.foodEaten++;
                    s.itemManager.MarkItemUsed(this);
                    s.soundManager.PlayClip("eat");
                    s.turnManager.ChangeStaminaOf("player", s.itemManager.GetFoodStaminaAmount("rotten steak", includeCharacterBonus:false));
                    s.turnManager.SetStatusText("you swallow rotten steak");
                    Remove();
                    break;
                case "cheese":
                    Save.persistent.foodEaten++;
                    s.itemManager.MarkItemUsed(this);
                    s.soundManager.PlayClip("eat");
                    s.turnManager.ChangeStaminaOf("player", s.itemManager.GetFoodStaminaAmount("cheese"));
                    s.turnManager.SetStatusText("you swallow cheese");
                    Remove();
                    break;
                case "moldy cheese":
                    Save.persistent.foodEaten++;
                    s.itemManager.MarkItemUsed(this);
                    s.soundManager.PlayClip("eat");
                    s.turnManager.ChangeStaminaOf("player", s.itemManager.GetFoodStaminaAmount("moldy cheese", includeCharacterBonus:false));
                    s.turnManager.SetStatusText("you swallow moldy cheese");
                    Remove();
                    break;
                case "scroll":
                    Save.persistent.scrollsRead++;
                    s.itemManager.MarkItemUsed(this);
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
                    if (modifier == "life" && s.player.woundList.Count == 0) {
                        s.turnManager.SetStatusText("you're healthy");
                        break;
                    }

                    Save.persistent.potionsQuaffed++;
                    s.itemManager.MarkItemUsed(this);
                    s.soundManager.PlayClip("gulp");
                    s.turnManager.SetStatusText($"you quaff potion of {modifier}");
                    // notify player
                    switch (modifier) {
                        case "accuracy":
                            s.player.potionStats["green"] += PotionStatBonus;
                            Save.game.potionAcc = s.player.potionStats["green"];
                            break;
                        case "speed":
                            s.player.potionStats["blue"] += PotionStatBonus;
                            Save.game.potionSpd = s.player.potionStats["blue"];
                            break;
                        case "strength":
                            s.player.potionStats["red"] += PotionStatBonus;
                            Save.game.potionDmg = s.player.potionStats["red"];
                            break;
                        case "defense":
                            s.player.potionStats["white"] += PotionStatBonus;
                            Save.game.potionDef = s.player.potionStats["white"];
                            break;
                        // for regular potions, shift the stats by varying amounts depending if the sum without die is -1 or not (or knee injury), making sure the dice stay in the correct position
                        case "might":
                            s.diceSummoner.GenerateSingleDie(Random.Range(1, 7), "yellow", "player", "red", isFromMight:true);
                            break;
                        case "life":
                            s.turnManager.AnimatePlayerWoundRefresh("blip0", () => {
                                s.player.woundList.Clear();
                                Save.game.playerWounds = s.player.woundList;
                                s.turnManager.SetBleedOutNextRound("player", false, saveGame:false);
                                RefreshPlayerAfterHealingWounds();
                            });
                            break;
                        case "nothing": break;
                        // default: print("invalid potion modifier detected"); break;
                    }
                    if (s.tutorial == null) { Save.SaveGame(); }
                    s.statSummoner.SummonStats();
                    s.statSummoner.RepositionAllDice("player");
                    s.statSummoner.SetCombatDebugInformationFor("player");
                    Remove();
                    s.itemManager.Select(s.player.inventory, 0, true, false);
                    break;
                case "campfire":
                    UseCampfire();
                    break;
                case "tincture":
                    UseTincture();
                    break;
                case "shuriken":
                    Save.persistent.shurikensThrown++;
                    s.itemManager.MarkItemUsed(this);
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
                        ConsumeCommonItemAndReselect();
                    }
                    else {
                        s.itemManager.MarkItemUsed(this);
                        // spawning a normal enemy
                        s.levelManager.ClearSavedEnemyCombatStateForEscape();
                        if (s.statSummoner != null) {
                            s.statSummoner.ResetDiceAndStamina();
                        }
                        s.levelManager.NextLevel();
                        Remove(selectNew:false);
                    }
                    break;
                case "mirror":
                    UseMirror();
                    break;
                case "unstable spellbook":
                    UseUnstableSpellbook();
                    break;
                case "witch hand":
                    UseWitchHand();
                    break;
                case "helm of might":
                    if (!Save.game.usedHelm) {
                        if (s.player.stamina >= 3) {
                            s.soundManager.PlayClip("fwoosh");
                            s.itemManager.MarkItemUsed(this);
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
                            s.itemManager.MarkItemUsed(this);
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
                        bool enemyDraftsFirstAfterAnkh = s.turnManager.EnemyShouldDraftFirstForFreshDraft();
                        s.soundManager.PlayClip("click0");
                        s.itemManager.MarkItemUsed(this);
                        Save.game.usedAnkh = true;
                        if (s.tutorial == null) { Save.SaveGame(); }
                        foreach (string key in s.itemManager.statArr) {
                            s.turnManager.ChangeStaminaOf("player", s.statSummoner.addedPlayerStamina[key]);
                            s.statSummoner.addedPlayerStamina[key] = 0;
                            // refund stamina
                        }
                        s.statSummoner.ResetDiceAndStamina(refundEnemyPlannedStamina:true);
                        s.diceSummoner.SummonDice(false, true);
                        s.statSummoner.SummonStats();
                        s.turnManager.DetermineMove(true, true, enemyDraftsFirstAfterAnkh);
                    }
                    else { s.turnManager.SetStatusText("ankh glows with red light"); }
                    break;
                // default: print("rare item with invalid name found!"); break;
            }
        }

        yield break;
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
            bool offeringToKapala = s.itemManager.PlayerHas("kapala") && !s.itemManager.IsVendorEncounter() && s.levelManager.sub != 4
                && itemType != "weapon" && s.enemy.enemyName.text != "Tombstone" && !Save.game.enemyIsDead;
            if (offeringToKapala) {
                bool wasntAlreadyFurious = s.player.SetPlayerStatusEffect("fury", true);
                if (wasntAlreadyFurious) {
                    s.turnManager.SetStatusText("deity accepts your offering... you feel furious");
                    s.soundManager.PlayClip("fwoosh");
                    s.diceSummoner.MakeAllAttachedYellow();
                }
            }
            else {
                if (s.itemManager.IsVendorEncounter()) { Save.game.numItemsDroppedForTrade++; }
                if (s.tutorial == null) { Save.SaveGame(); }
                s.turnManager.SetStatusText(s.itemManager.GetActionTextForItem(this, s.itemManager.IsVendorEncounter() ? "you offer" : "you drop"));
                if (s.itemManager.IsVendorEncounter()) { s.itemManager.UpdateVendorUIForSelection(); }
            }
        }
        List<GameObject> owningList = GetOwningItemList();
        if (owningList == null) {
            Destroy(gameObject);
            return;
        }

        bool skipFade = dontSave;
        if (armorFade && !skipFade) {
            StartCoroutine(FadeArmor(owningList, selectNew));
        }
        else if (torchFade && !skipFade) {
            StartCoroutine(FadeTorch(owningList, selectNew));
        }
        // fading armor or torch, so do something special
        else {
            Destroy(gameObject);
            // destroy the object
            ShiftItems(owningList, selectNew);
        }
        if (!dontSave) { s.itemManager.SaveInventoryItems(); }
    }

    private List<GameObject> GetOwningItemList() {
        if (s?.player?.inventory != null && s.player.inventory.Contains(gameObject)) {
            return s.player.inventory;
        }

        if (s?.itemManager?.floorItems != null && s.itemManager.floorItems.Contains(gameObject)) {
            return s.itemManager.floorItems;
        }

        if (s?.itemManager?.curList != null && s.itemManager.curList.Contains(gameObject)) {
            return s.itemManager.curList;
        }

        return null;
    }

    /// <summary>
    /// Shift the items in the current list over, starting from a given index.
    /// </summary>
    private void ShiftItems(List<GameObject> itemList, bool selectNew) {
        if (itemList == null) { return; }

        int index = itemList.IndexOf(gameObject);
        if (index < 0 || index >= itemList.Count) { return; }

        bool removedFromCurrentList = itemList == s.itemManager.curList;
        itemList.RemoveAt(index);
        // remove the item from the list
        s.itemManager.InvalidateInventoryCache();
        s.itemManager.RefreshPassiveInventoryEffects();
        s.itemManager.SyncCharmStateToSave();
        if (itemList == s.player.inventory) {
            for (int i = index; i < itemList.Count; i++) {
                // for each item in the inventory after the index of the previous one
                itemList[i].transform.position = new Vector2(s.itemManager.itemX + s.itemManager.itemSpacing * i, 3.16f);
                // shift over each item
            }
        }
        else if (itemList == s.itemManager.floorItems) {
            for (int i = index; i < itemList.Count; i++) {
                itemList[i].transform.position = new Vector2(s.itemManager.itemX + s.itemManager.itemSpacing * i, s.itemManager.itemY);
            }
        }

        if (!selectNew || !removedFromCurrentList) { return; }

        if (itemList.Count > 0) {
            s.itemManager.Select(itemList, Mathf.Clamp(index, 0, itemList.Count - 1), playAudio: true);
        }
        else if (s?.player?.inventory != null && s.player.inventory.Count > 0) {
            s.itemManager.Select(s.player.inventory, 0, playAudio: true);
        }
        // select the next item over if needed
    }

    /// <summary>
    /// Coroutine to break player's armor when it is hit.
    /// </summary>
    private IEnumerator FadeArmor(List<GameObject> itemList, bool selectNew) {
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
        ShiftItems(itemList, selectNew);
        s.itemManager.SaveInventoryItems();
    }

    /// <summary>
    /// Coroutine to fade out a torch when its time has come.
    /// </summary>
    /// <param name="itemList"></param>
    /// <param name="selectNew"></param>
    /// <returns></returns>
    private IEnumerator FadeTorch(List<GameObject> itemList, bool selectNew) {
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
            temp.a -= 1f / 6f;
            sr.color = temp;
        }
        // flash in and out, faster and faster
        Destroy(gameObject);
        ShiftItems(itemList, selectNew);
        // destroy it and shift over
    }
}
