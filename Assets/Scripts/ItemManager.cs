using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
public class ItemManager : MonoBehaviour {
    private const int MinTorchCombatLifetime = 3;
    private const int MaxTorchCombatLifetime = 5;
    private static readonly string[] CanonicalWeaponNames = {
        "dagger", "flail", "hatchet", "mace", "maul", "montante", "rapier", "scimitar", "spear", "sword", "katar", "buckler", "ham", "gladius", "glass sword", "stave", "gauntlets", "glaive", "claymore", "crossbow", "trident", "warhammer"
    };
    private static readonly string[] CanonicalNeckletTypes = { "solidity", "rapidity", "strength", "defense", "arcane", "nothing", "victory" };
    private static readonly string[] CanonicalCharmTypes = { "unbroken", "relentless", "aether", "ruthless", "arcane", "riposte", "bulwark", "vindictive", "inevitable", "nothing" };
    private static readonly string[] CanonicalTarotTypes = { "abyss", "verdant", "inferno", "glacier", "dawn", "leviathan", "viper", "dragon", "wyvern", "phoenix", "arcane", "nothing" };
    private static readonly Dictionary<string, string> RuinedCommonItemNames = new() {
        { "helm of might", "rusted helm" },
        { "boots of dodge", "ruined boots" },
        { "ankh", "shattered ankh" },
        { "kapala", "defiled kapala" },
        { "steak", "rotten steak" },
        { "cheese", "moldy cheese" },
        { "bloodletter's curse", "rotting mask" },
        { "rabadon's deathcap", "ruined cap" },
        { "cursed mask", "shattered mask" },
        { "goggles", "broken goggles" },
        { "salt shaker", "empty shaker" },
        { "cornucopia", "broken horn" },
        { "thief's armband", "torn armband" },
        { "sacrificial chalice", "desecrated chalice" },
        { "unstable spellbook", "ravaged book" },
    };
    private static readonly Dictionary<string, string> LegacyCommonItemKeyAliases = new() {
        { "holy water", "holy_water" },
        { "witch hand", "witch_hand" },
        { "skeleton key", "skeleton_key" },
        { "crystal shard", "crystal_shard" },
        { "salt shaker", "salt_shaker" },
        { "cursed mask", "cursed_mask" },
        { "cursed dice", "cursed_dice" },
        { "sacrificial chalice", "sacrificial_chalice" },
        { "unstable spellbook", "unstable_spellbook" },
        { "lucky dice", "lucky_dice" },
        { "boots of dodge", "boots_of_dodge" },
        { "helm of might", "helm_of_might" },
        { "amulet of resurrection", "amulet_of_resurrection" },
        { "broken amulet", "broken_amulet" },
        { "thief's armband", "thiefs_armband" },
        { "thiefs armband", "thiefs_armband" },
        { "thiefs_armband", "thiefs_armband" },
        { "bloodletter's curse", "bloodletters_curse" },
        { "bloodletters", "bloodletters_curse" },
        { "rabadon's deathcap", "rabadons_deathcap" },
        { "rabadons deathcap", "rabadons_deathcap" },
        { "rabadons", "rabadons_deathcap" },
        { "rotten steak", "rotten_steak" },
        { "moldy cheese", "moldy_cheese" },
        { "rusted helm", "rusted_helm" },
        { "ruined boots", "ruined_boots" },
        { "defiled kapala", "defiled_kapala" },
        { "shattered ankh", "shattered_ankh" },
        { "rotting mask", "rotting_mask" },
        { "ruined cap", "ruined_cap" },
        { "shattered mask", "shattered_mask" },
        { "broken goggles", "broken_goggles" },
        { "empty shaker", "empty_shaker" },
        { "broken horn", "broken_horn" },
        { "torn armband", "torn_armband" },
        { "desecrated chalice", "desecrated_chalice" },
        { "ravaged book", "ravaged_book" },
        { "rotten ham", "rotten_ham" },
    };
    private static readonly HashSet<string> WeaponNamePrefixes = new() {
        "common", "legendary", "forged", "accurate", "brisk", "blunt", "heavy", "nimble", "precise", "ruthless",
        "stable", "sharp", "harsh", "quick", "rotten", "rusty", "shattered"
    };
    private static readonly string[] forgeStats = { "accuracy", "speed", "damage", "parry" };

    private sealed class WeaponRollData {
        public string BaseName;
        public string Modifier;
        public Dictionary<string, int> Stats;
        public Sprite Sprite;
    }

    [SerializeField] public TextMeshProUGUI lootText;
    [SerializeField] public TextMeshProUGUI itemDesc;
    [SerializeField] private GameObject item;
    [SerializeField] public GameObject highlight;
    [SerializeField] public GameObject highlightedItem;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] public Sprite[] weaponSprites;
    [SerializeField] private Sprite[] otherSprites;
    [SerializeField] public List<GameObject> floorItems;
    public List<GameObject> deletionQueue = new();
    public string[] statArr = { "green", "blue", "red", "white" };
    public string[] statArr1 = { "accuracy", "speed", "damage", "parry" };
    private readonly Dictionary<string, Sprite> spriteLookup = new();
    private readonly Dictionary<string, Sprite> weaponSpriteByBaseName = new();
    private readonly List<string> weaponBaseNames = new();
    private readonly Dictionary<string, int> inventoryItemCounts = new();
    private readonly Dictionary<string, GameObject> firstInventoryItemByName = new();
    private readonly Dictionary<string, int> charmCountsByModifier = new();
    private readonly Dictionary<string, int> tarotCountsByModifier = new();
    private bool luckyDiceRoundStatsInitialized;
    private int tridentImmediateAttackBonus;
    private Item cachedEquippedWeapon;
    private string cachedEquippedWeaponBaseName = "";
    private bool cachedEquippedWeaponIsLegendary;
    private bool inventoryCacheDirty = true;
    private int pendingShatteredCrystalShards;
    private readonly Dictionary<string, Dictionary<string, int>> weaponStatDict = new() {
        { "dagger",   new Dictionary<string, int> { 
            { "green", 3 }, { "blue", 5 }, { "red", 1 }, { "white", 1 } } },
        { "flail",    new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 3 }, { "red", 3 }, { "white", 1 } } },
        { "hatchet",  new Dictionary<string, int> { 
            { "green", 2 }, { "blue", 3 }, { "red", 3 }, { "white", 2 } } },
        { "mace",     new Dictionary<string, int> { 
            { "green", 2 }, { "blue", 4 }, { "red", 3 }, { "white", 1 } } },
        { "maul",     new Dictionary<string, int> { 
            { "green",-1 }, { "blue",-1 }, { "red", 4 }, { "white", 2 } } },
        { "montante", new Dictionary<string, int> { 
            { "green", 2 }, { "blue", 2 }, { "red", 4 }, { "white", 3 } } },
        { "rapier",   new Dictionary<string, int> { 
            { "green", 5 }, { "blue", 3 }, { "red",-1 }, { "white", 2 } } },
        { "scimitar", new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 3 }, { "red", 2 }, { "white", 3 } } },
        { "spear",    new Dictionary<string, int> { 
            { "green", 3 }, { "blue",-1 }, { "red", 4 }, { "white", 2 } } },
        { "sword",    new Dictionary<string, int> { 
            { "green", 2 }, { "blue", 3 }, { "red", 2 }, { "white", 3 } } },
        { "katar",    new Dictionary<string, int> { 
            { "green", 3 }, { "blue", 3 }, { "red", 5 }, { "white", -1 } } },
        { "buckler",    new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 2 }, { "red", 0 }, { "white", 6 } } },
        { "ham",    new Dictionary<string, int> { 
            { "green", 2 }, { "blue", 2 }, { "red", 2 }, { "white", 2 } } },
        { "gladius",    new Dictionary<string, int> {
            { "green", 2 }, { "blue", 4 }, { "red", 3 }, { "white", 1 } } },
        { "glass sword", new Dictionary<string, int> {
            { "green", 0 }, { "blue", 3 }, { "red", 6 }, { "white", 0 } } },
        { "stave", new Dictionary<string, int> {
            { "green", 2 }, { "blue", 2 }, { "red", 0 }, { "white", 2 } } },
        { "gauntlets", new Dictionary<string, int> {
            { "green", 2 }, { "blue", -1 }, { "red", 1 }, { "white", -1 } } },
        { "glaive", new Dictionary<string, int> {
            { "green", 2 }, { "blue", 0 }, { "red", 4 }, { "white", 1 } } },
        { "claymore", new Dictionary<string, int> {
            { "green", 1 }, { "blue", 1 }, { "red", 3 }, { "white", 1 } } },
        { "crossbow", new Dictionary<string, int> {
            { "green", -1 }, { "blue", -1 }, { "red", -1 }, { "white", -1 } } },
        { "trident", new Dictionary<string, int> {
            { "green", 1 }, { "blue", 3 }, { "red", 3 }, { "white", 1 } } },
        { "warhammer", new Dictionary<string, int> {
            { "green", 1 }, { "blue", -1 }, { "red", 4 }, { "white", 1 } } },
 
    };  
    private readonly Dictionary<string, Dictionary<string, int>> legendaryStatDict = new() {
        { "dagger",   new Dictionary<string, int> { 
            { "green", 4 }, { "blue", 7 }, { "red", 1 }, { "white", 1 } } }, 
            // +1g +2b
        { "flail",    new Dictionary<string, int> { 
            { "green", 2 }, { "blue", 3 }, { "red", 4 }, { "white", 1 } } }, 
            // +1g +1r
        { "hatchet",  new Dictionary<string, int> { 
            { "green", 3 }, { "blue", 3 }, { "red", 4 }, { "white", 2 } } }, 
            // +1g +1r
        { "mace",     new Dictionary<string, int> { 
            { "green", 2 }, { "blue", 4 }, { "red", 4 }, { "white", 1 } } }, 
            // +1g +1r
        { "maul",     new Dictionary<string, int> { 
            { "green", 0 }, { "blue", -1 }, { "red", 6 }, { "white", 1 } } }, 
            // +1g +2r -1w
        { "montante", new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 1 }, { "red", 6 }, { "white", 6 } } }, 
            // -1g -1b +2r +3w
        { "rapier",   new Dictionary<string, int> { 
            { "green", 8 }, { "blue", 1 }, { "red", 1 }, { "white", 2 } } }, 
            // +3g +1r
        { "scimitar", new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 3 }, { "red", 2 }, { "white", 5 } } }, 
            // +2w 
        { "spear",    new Dictionary<string, int> { 
            { "green", 4 }, { "blue", -1 }, { "red", 5 }, { "white", 1 } } }, 
            // +1g +1r
        { "sword",    new Dictionary<string, int> { 
            { "green", 3 }, { "blue", 4 }, { "red", 3 }, { "white", 4 } } }, 
            // +1g +1b +1r +1w
        { "katar",    new Dictionary<string, int> { 
            { "green", 3 }, { "blue", 3 }, { "red", 7 }, { "white", -1 } } },
            // +2r
        { "buckler",    new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 2 }, { "red", 0 }, { "white", 9 } } },
            // +3w
        { "ham",    new Dictionary<string, int> {
            { "green", 2 }, { "blue", 3 }, { "red", 3 }, { "white", 3 } } },
            // +1g +1b +1r +1w
        { "gladius",    new Dictionary<string, int> {
            { "green", 3 }, { "blue", 5 }, { "red", 4 }, { "white", 2 } } },
            // +1g +1b +1r +1w
        { "glass sword", new Dictionary<string, int> {
            { "green", 0 }, { "blue", 3 }, { "red", 8 }, { "white", 0 } } },
            // legendary: +2r
        { "stave", new Dictionary<string, int> {
            { "green", 3 }, { "blue", 3 }, { "red", 0 }, { "white", 3 } } },
            // +1g +1b +1w
        { "gauntlets", new Dictionary<string, int> {
            { "green", 3 }, { "blue", -1 }, { "red", 3 }, { "white", -1 } } },
            // +1g +2r
        { "glaive", new Dictionary<string, int> {
            { "green", 2 }, { "blue", 0 }, { "red", 6 }, { "white", 1 } } },
            // +2r
        { "claymore", new Dictionary<string, int> {
            { "green", 1 }, { "blue", 2 }, { "red", 3 }, { "white", 1 } } },
            // scaling handled dynamically
        { "crossbow", new Dictionary<string, int> {
            { "green", 1 }, { "blue", -1 }, { "red", 0 }, { "white", -1 } } },
            // +2g 1r
        { "trident", new Dictionary<string, int> {
            { "green", 2 }, { "blue", 3 }, { "red", 4 }, { "white", 1 } } },
            // +1g +1r
        { "warhammer", new Dictionary<string, int> {
            { "green", 2 }, { "blue", -1 }, { "red", 5 }, { "white", 1 } } },
            // +1g +1r
    };  
    private readonly Dictionary<string, int> itemDropDict = new() {
        { "potion",                 16 },
        { "scroll",                 16 },
        { "shuriken",               16 },
        { "gem",                    16 },
        { "necklet",                12 },
        { "tarot",                  12 },
        { "charm",                  12 },
        { "tincture",               12 },
        { "torch",                  12 },
        { "cheese",                 12 },
        { "steak",                  8 },
        { "mirror",                 8 },
        { "witch_hand",             8 },
        { "skeleton_key",           4 },
        { "crystal_shard",          4 },
        { "lucky_dice",             4 },
        { "goggles",                4 },
        { "campfire",               2 },
        { "kapala",                 2 },
        { "boots_of_dodge",         2 },
        { "helm_of_might",          2 },
        { "sacrificial_chalice",    2 },
        { "unstable_spellbook",     2 },
        { "thiefs_armband",         2 },
        { "bloodletters_curse",     2 },
        { "salt_shaker",            2 },
        { "cornucopia",             2 },
        { "rabadons_deathcap",      2 },
        { "cursed_mask",            2 },
        { "cursed_dice",            2 },
        { "holy_water",             1 },
        { "amulet_of_resurrection", 1 },
        { "ankh",                   1 },
        { "armor",                  1 },
    };
    [SerializeField] private List<string> itemDropTable;

    // canonical order for the almanac pages; indices must stay stable
    public static readonly string[] AlmanacWeaponOrder = {
        "dagger",       "legendary dagger",
        "flail",        "legendary flail",
        "hatchet",      "legendary hatchet",
        "mace",         "legendary mace",
        "maul",         "legendary maul",
        "montante",     "legendary montante",
        "rapier",       "legendary rapier",
        "scimitar",     "legendary scimitar",
        "spear",        "legendary spear",
        "sword",        "legendary sword",
        "katar",        "legendary katar",
        "buckler",      "legendary buckler",
        "ham",          "legendary ham",
        "gladius",      "legendary gladius",
        "glass sword",  "legendary glass sword",
        "stave",        "legendary stave",
        "gauntlets",    "legendary gauntlets",
        "glaive",       "legendary glaive",
        "claymore",     "legendary claymore",
        "crossbow",     "legendary crossbow",
        "trident",      "legendary trident",
        "warhammer",    "legendary warhammer",
    };
    public static readonly string[] AlmanacItemOrder = {
        // consumables
        "steak", "cheese", "torch", "shuriken", "mirror",
        // scrolls
        "scroll of fury", "scroll of haste", "scroll of dodge", "scroll of leech",
        "scroll of courage", "scroll of destruction", "scroll of fortification", "scroll of duality", "scroll of challenge",
        "scroll of chest", "scroll of guts", "scroll of knee", "scroll of hip", "scroll of hand", "scroll of armpits", "scroll of nothing",
        // potions
        "potion of accuracy", "potion of speed", "potion of strength", "potion of defense",
        "potion of might", "potion of life", "potion of chaos", "potion of pandemonium",
        "potion of rage", "potion of alacrity", "potion of force", "potion of lethality", "potion of resilience", "potion of nothing",
        // necklets
        "necklet of solidity", "necklet of rapidity", "necklet of strength", "necklet of defense",
        "arcane necklet", "necklet of nothing", "necklet of victory",
        // charms
        "charm of the unbroken", "charm of the relentless", "charm of the aether", "charm of the ruthless",
        "charm of the arcane", "charm of the riposte",
        "charm of the bulwark", "charm of the vindictive", "charm of the inevitable", "charm of the nothing",
        // tarots
        "tarot of the abyss", "tarot of the verdant", "tarot of the inferno",
        "tarot of the glacier", "tarot of the dawn", "tarot of the leviathan", "tarot of the viper",
        "tarot of the dragon", "tarot of the wyvern", "tarot of the phoenix", "tarot of the arcane", "tarot of the nothing",
        // passive / utility items
        "crystal shard", "sacrificial chalice", "unstable spellbook", "thief's armband", "lucky dice", "bloodletter's curse", "salt shaker", "cornucopia",
        "holy water", "rabadon's deathcap", "goggles", "cursed mask", "cursed dice",
        "emerald gem", "ruby gem", "sapphire gem", "topaz gem", "citrine gem",
        // equipment
        "armor", "ankh", "skeleton key", "witch hand", "campfire", "tincture",
        "helm of might", "boots of dodge", "kapala", "amulet of resurrection", "phylactery",
    };

    private readonly Dictionary<string, Dictionary<string, int>> modifierStatDict = new() {
        { "accurate0", new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "accurate1", new Dictionary<string, int> { 
            { "green", 2 }, { "blue",-1 }, { "red", 0 }, { "white", 0 } } },
        { "brisk0",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 1 }, { "red",-1 }, { "white", 0 } } },
        { "brisk1",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 1 }, { "red", 0 }, { "white",-1 } } },
        { "blunt0",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 1 } } },
        { "blunt1",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 0 }, { "red",-1 }, { "white", 1 } } },
        { "common0",   new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "heavy0",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue",-1 }, { "red", 1 }, { "white", 0 } } },
        { "heavy1",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue",-1 }, { "red", 0 }, { "white", 1 } } },
        { "nimble0",   new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 1 }, { "red", 0 }, { "white",-1 } } },
        { "nimble1",   new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 1 }, { "red",-1 }, { "white", 1 } } },
        { "precise0",  new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 0 }, { "red",-1 }, { "white", 0 } } },
        { "precise1",  new Dictionary<string, int> { 
            { "green", 1 }, { "blue",-1 }, { "red", 0 }, { "white", 0 } } },
        { "ruthless0", new Dictionary<string, int> { 
            { "green", 1 }, { "blue",-1 }, { "red",-1 }, { "white", 1 } } },
        { "ruthless1", new Dictionary<string, int> { 
            { "green", 1 }, { "blue",-1 }, { "red",-1 }, { "white", 0 } } },
        { "stable0",   new Dictionary<string, int> { 
            { "green",-1 }, { "blue", 0 }, { "red", 0 }, { "white", 1 } } },
        { "stable1",   new Dictionary<string, int> { 
            { "green", 0 }, { "blue",-1 }, { "red", 0 }, { "white", 1 } } },
        { "sharp0",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 0 }, { "red", 1 }, { "white", 0 } } },
        { "sharp1",    new Dictionary<string, int> { 
            { "green", 1 }, { "blue",-1 }, { "red", 1 }, { "white", 0 } } },
        { "harsh0",    new Dictionary<string, int> { 
            { "green", 1 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } } },
        { "quick0",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 1 }, { "red", 0 }, { "white", 0 } } },
        { "lame0",    new Dictionary<string, int> { 
            { "green", 0 }, { "blue", 0 }, { "red", -1 }, { "white", -1 } } },
    };

    private readonly Dictionary<string, int> modifierDropDict = new() {
        { "common0",   15 },
        { "legendary0", 5 },
        { "accurate0", 5 },
        { "accurate1", 5 },
        { "brisk0",    5 },
        { "brisk1",    5 },
        { "blunt0",    5 },
        { "blunt1",    5 },
        { "heavy0",    5 },
        { "heavy1",    5 },
        { "nimble0",   5 },
        { "nimble1",   5 },
        { "precise0",  5 },
        { "precise1",  5 },
        { "ruthless0", 5 },
        { "ruthless1", 5 },
        { "stable0",   5 },
        { "stable1",   5 },
        { "sharp0",    5 },
        { "sharp1",    5 },
        { "harsh0",    5 },
        { "quick0",    5 },
        { "lame0",    5 },
    };

    private List<string> modifierDropTable = new();
    
    public readonly Dictionary<string, string> descriptionDict = new() {
        {"armor",              "protects from one hit"}, 
        {"arrow",              "proceed to the next level"}, 
        {"ankh",               "force new draft"}, 
        {"amulet of resurrection", ""},
        {"boots of dodge",     "pay 1 stamina to become dodgy"}, 
        {"rusted helm",        ""}, 
        {"rotting mask",       ""},
        {"ruined cap",         ""},
        {"shattered mask",     ""},
        {"broken goggles",     ""},
        {"empty shaker",       ""},
        {"broken horn",        ""},
        {"torn armband",       ""},
        {"desecrated chalice", ""},
        {"ravaged book",       ""},
        {"cheese",             "3"}, 
        {"dagger",             "green dice buff damage"}, 
        {"legendary dagger",   "+1 stamina regen "},
        {"defiled kapala",     ""}, 
        {"flail",              "start with red die"}, 
        {"legendary flail",    "start with two red die"}, 
        {"forge",              ""},
        {"hatchet",            "enemy can't use yellow die"},  
        {"legendary hatchet",  "start with yellow die"},  
        {"helm of might",      "pay 3 stamina to gain a yellow die"}, 
        {"kapala",             "offer an item to become furious"}, 
        {"mace",               "reroll all dice still to be picked"}, 
        {"legendary mace",     "reroll any number of enemy's dice"}, 
        {"maul",               "any wound is deadly"}, 
        {"legendary maul",     "any wound is deadly"}, 
        {"montante",           ""}, 
        {"legendary montante", ""}, 
        {"necklet of solidity", "+1 accuracy"},
        {"necklet of rapidity", "+1 speed"},
        {"necklet of strength", "+1 damage"},
        {"necklet of defense",  "+1 parry"},
        {"arcane necklet",      "all necklets are more effective"},
        {"necklet of nothing",  "does nothing"},
        {"necklet of victory",  "the victory is in your hands!.."},
        {"necklet",            ""}, 
        {"phylactery",         "gain \"leech\" buff once wounded"},
        {"potion of accuracy", "gain a green die"},
        {"potion of speed",    "gain a blue die"},
        {"potion of strength", "gain a red die"},
        {"potion of defense",  "gain a white die"},
        {"potion of might",    "gain a yellow die"},
        {"potion of life",     "heal all wounds"},
        {"potion of chaos",    "gain 3 random die"},
        {"potion of pandemonium", "gain 3 random 6 die"},
        {"potion of rage",     "gain 2 stamina per red die attached"},
        {"potion of alacrity", "gain 2 stamina per blue die attached"},
        {"potion of force",    "gain 2 stamina per white die attached"},
        {"potion of lethality", "gain 2 stamina per green die attached"},
        {"potion of resilience", "gain 2 stamina per yellow die attached"},
        {"potion of nothing",  "does nothing"},
        {"potion",             ""}, 
        {"rapier",             "gain 3 stamina upon kill"}, 
        {"legendary rapier",   "gain 5 stamina upon kill"}, 
        {"retry",              "retry?"}, 
        {"ruined boots",       ""}, 
        {"scimitar",           "discard enemy's die upon parry"},  
        {"legendary scimitar", "discard two of enemy's die upon parry"},  
        {"scroll of fury",      "all picked dice turn yellow"},
        {"scroll of haste",     "pick 3 dice, enemy gets the rest"},
        {"scroll of dodge",     "if you strike first, ignore all damage"},
        {"scroll of leech",     "cure the same wound as inflicted"},
        {"scroll of courage",   "keep 1 of your die till next round"},
        {"scroll of destruction", "your attack is equal to accuracy this turn"},
        {"scroll of fortification", "your parry is equal to speed this turn"},
        {"scroll of duality",   "your attack is equal to parry this turn"},
        {"scroll of challenge", "???"},
        {"scroll of chest",     "reroll any number of enemy's dice"},
        {"scroll of guts",      "all enemy's dice suffer a penalty of -1"},
        {"scroll of knee",      "your speed is higher than enemy's"},
        {"scroll of hip",       "enemy can't use stamina"},
        {"scroll of hand",      "enemy can't use white dice"},
        {"scroll of armpits",   "enemy can't use red dice"},
        {"scroll of nothing",   "does nothing"},
        {"scroll",             ""}, 
        {"shuriken",           "discard enemy's die"}, 
        {"skeleton key",       "escape the fight"}, 
        {"shattered ankh",     ""}, 
        {"spear",              "always choose first die"}, 
        {"legendary spear",    "always go first"}, 
        {"steak",              "5"}, 
        {"sword",              ""}, 
        {"legendary sword",    "find more loot"}, 
        {"torch",              "find more loot"}, 
        {"katar",              "first wound reduces speed by 1"}, 
        {"legendary katar",    "first wound reduces speed by 2"}, 
        {"buckler",            "guard gains +2 parry"},
        {"legendary buckler",  "guard gains +2 parry"},
        {"ham",                "gain 2 stamina next level"},
        {"legendary ham",      "gain 4 stamina next level"},
        {"rotten ham",         "gain 0 stamina next level"},
        {"gladius",            "start combat with +3 attack"},
        {"legendary gladius",  "start combat with +5 attack"},
        {"campfire",           "recover when safe"},
        {"tincture",           "heal a non-lethal wound"},
        {"mirror",             "copy any die"},
        {"witch hand",         "curse your enemy"},
        {"glass sword",        "shatters if wounded"},
        {"legendary glass sword", "shatters if wounded"},
        {"shattered glass sword", ""},
        {"stave",              "charms, necklets, and tarots are more effective"},
        {"legendary stave",    "charms, necklets, and tarots are more effective"},
        {"gauntlets",          "always go first"},
        {"legendary gauntlets", "always go first"},
        {"glaive",             "two wounds are deadly"},
        {"legendary glaive",   "two wounds are deadly"},
        {"claymore",           "stronger with stamina"},
        {"legendary claymore", "stronger with stamina"},
        {"crossbow",           "ignore enemy parry"},
        {"legendary crossbow", "ignore enemy parry"},
        {"trident",            "attack first for +1 attack"},
        {"legendary trident",  "attack first for +1 attack"},
        {"warhammer",          "wound to stun"},
        {"legendary warhammer", "wound to stun"},
        {"unstable spellbook",     "pay 2 stamina to transmute a die"},
        {"lucky dice",             "randomizes your stats"},
        {"bloodletter's curse",    "find more loot when wounded"},
        {"crystal shard",          "+2 attack\nshatters if wounded"},
        {"rabadon's deathcap",     "restore 1 stamina per turn if less than 3 stamina"},
        {"goggles",                "start combat with +3 accuracy and +2 attack"},
        {"cursed mask",            "+2 stamina regen per wound"},
        {"cursed dice",            "start with yellow dice while wounded"},
        {"holy water",             "+10 stamina or trade for 2 items"},
        {"salt shaker",            "food tastes better"},
        {"cornucopia",             "+1 stamina per item taken"},
        {"emerald gem",            "turn an attached dice green"},
        {"ruby gem",               "turn an attached dice red"},
        {"sapphire gem",           "turn an attached dice blue"},
        {"topaz gem",              "turn an attached dice white"},
        {"citrine gem",            "turn an attached dice yellow"},
        // charms
        {"charm of the unbroken",     "parry to gain +1 parry"},
        {"charm of the relentless",   "wound to gain +1 attack"},
        {"charm of the aether",       "attack first to gain +1 speed"},
        {"charm of the ruthless",     "attack neck for +1 accuracy"},
        {"charm of the riposte",      "parry to gain +1 attack"},
        {"charm of the vindictive",   "gain +2 attack when wounded"},
        {"charm of the bulwark",      "attack second to gain +1 parry"},
        {"charm of the inevitable",   "attack second to gain +1 attack"},
        {"charm of the arcane",       "all charms are more effective"},
        {"charm of the nothing",      "does nothing"},
        // tarot
        {"tarot of the abyss",     "blue die are more effective"},
        {"tarot of the verdant",   "green die are more effective"},
        {"tarot of the inferno",   "red die are more effective"},
        {"tarot of the glacier",   "white die are more effective"},
        {"tarot of the dawn",      "yellow die are more effective"},
        {"tarot of the leviathan", "enemy blue die are less effective"},
        {"tarot of the viper",     "enemy green die are less effective"},
        {"tarot of the dragon",    "enemy red die are less effective"},
        {"tarot of the wyvern",    "enemy white die are less effective"},
        {"tarot of the phoenix",   "enemy yellow die are less effective"},
        {"tarot of the arcane",    "all tarots are more effective"},
        {"tarot of the nothing",   "does nothing"},
        {"sacrificial chalice",    "it thirsts...  "},
        {"thief's armband",        "take what's yours"},
    };
    public string[] neckletTypes = CanonicalNeckletTypes.ToArray();
    public readonly Dictionary<string, int> neckletStats = new() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    public readonly Dictionary<string, int> neckletCounter = new() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 }, { "arcane", 1 } };
    // start with 1 arcane necklet so we don't have to have +1's everywhere
    public string[] charmTypes = CanonicalCharmTypes.ToArray();
    public string[] tarotTypes = CanonicalTarotTypes.ToArray();
    public readonly Dictionary<string, int> luckyDiceRoundStats = new() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    // always-on passive bonuses from inventory items that share the charm stat pipeline
    public readonly Dictionary<string, int> charmPassiveStats = new() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    // stat bonus active this round (earned last round; cleared at round clear)
    public Dictionary<string, int> charmActiveBonus = new() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    // stat bonus pending (earned this round; activates next round; cleared at round clear)
    public Dictionary<string, int> charmPendingBonus = new() { { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 } };
    private readonly Dictionary<string, int> charmActiveProcCounts = new() {
        { "unbroken", 0 }, { "relentless", 0 }, { "aether", 0 }, { "ruthless", 0 },
        { "arcane", 0 }, { "riposte", 0 }, { "bulwark", 0 }, { "vindictive", 0 }, { "inevitable", 0 }, { "nothing", 0 }
    };
    private readonly Dictionary<string, int> charmPendingProcCounts = new() {
        { "unbroken", 0 }, { "relentless", 0 }, { "aether", 0 }, { "ruthless", 0 },
        { "arcane", 0 }, { "riposte", 0 }, { "bulwark", 0 }, { "vindictive", 0 }, { "inevitable", 0 }, { "nothing", 0 }
    };
    private readonly string[] scrollTypes = { "fury", "haste", "dodge", "leech", "courage", "destruction", "fortification", "duality", "challenge", "chest", "guts", "knee", "hip", "hand", "armpits", "nothing" };
    private readonly string[] potionTypes = { "accuracy", "speed", "strength", "defense", "might", "life", "chaos", "pandemonium", "rage", "alacrity", "force", "lethality", "resilience", "nothing" };
    private readonly string[] gemTypes = { "emerald", "ruby", "sapphire", "topaz", "citrine" };
    private Sprite[] allSprites;
    public float itemSpacing = 1f;
    public float itemY = -5.3f;
    public float itemX = -3.75f;
    private Scripts s;
    public int col = 0;
    public List<GameObject> curList;
    public bool isCharSelect = false;
    public bool isAlmanac = false;

    private void Awake() {
        s = FindFirstObjectByType<Scripts>();
        BuildSpriteCatalogs();
        neckletTypes = NormalizeSerializedStringArray(neckletTypes, CanonicalNeckletTypes);
        charmTypes = NormalizeSerializedStringArray(charmTypes, CanonicalCharmTypes);
        tarotTypes = NormalizeSerializedStringArray(tarotTypes, CanonicalTarotTypes);
    }

    private static string[] NormalizeSerializedStringArray(string[] currentValues, string[] canonicalValues) {
        if (currentValues == null || currentValues.Length != canonicalValues.Length) {
            return canonicalValues.ToArray();
        }

        for (int i = 0; i < canonicalValues.Length; i++) {
            if (currentValues[i] != canonicalValues[i]) {
                return canonicalValues.ToArray();
            }
        }

        return currentValues;
    }

    public static string NormalizeWeaponSaveName(string weaponName) {
        if (string.IsNullOrWhiteSpace(weaponName)) { return ""; }

        string trimmedName = weaponName.Trim();
        if (System.Array.IndexOf(CanonicalWeaponNames, trimmedName) >= 0) { return trimmedName; }

        string baseName = GetWeaponBaseName(trimmedName);
        return System.Array.IndexOf(CanonicalWeaponNames, baseName) >= 0 ? baseName : trimmedName;
    }

    public static int IndexOfWeaponName(string weaponName) {
        return System.Array.IndexOf(CanonicalWeaponNames, NormalizeWeaponSaveName(weaponName));
    }

    public static string NormalizeCommonItemKey(string itemName) {
        if (string.IsNullOrWhiteSpace(itemName)) { return ""; }

        string key = itemName.Trim().ToLowerInvariant().Replace('-', '_');
        if (LegacyCommonItemKeyAliases.TryGetValue(key, out string aliasedKey)) {
            return aliasedKey;
        }

        key = key.Replace("'", "");
        key = key.Replace(' ', '_');
        while (key.Contains("__")) {
            key = key.Replace("__", "_");
        }

        return LegacyCommonItemKeyAliases.TryGetValue(key, out aliasedKey)
            ? aliasedKey
            : key;
    }

    private void BuildSpriteCatalogs() {
        spriteLookup.Clear();
        weaponSpriteByBaseName.Clear();
        weaponBaseNames.Clear();

        List<Sprite> sprites = new();
        RegisterSprites(itemSprites, sprites);
        RegisterSprites(weaponSprites, sprites);
        RegisterSprites(otherSprites, sprites);
        allSprites = sprites.ToArray();

        if (weaponSprites != null) {
            foreach (Sprite sprite in weaponSprites) {
                if (sprite == null) { continue; }

                string baseName = GetWeaponNameFromSpriteName(sprite.name);
                if (string.IsNullOrEmpty(baseName) || baseName == "shattered glass sword") { continue; }
                if (weaponSpriteByBaseName.ContainsKey(baseName)) { continue; }

                weaponSpriteByBaseName[baseName] = sprite;
                weaponBaseNames.Add(baseName);
            }
        }

        ValidateWeaponCatalog();
    }

    private void RegisterSprites(Sprite[] sprites, List<Sprite> allSpriteList) {
        if (sprites == null) { return; }

        foreach (Sprite sprite in sprites) {
            if (sprite == null) { continue; }

            allSpriteList.Add(sprite);
            spriteLookup.TryAdd(sprite.name, sprite);
        }
    }

    private void ValidateWeaponCatalog() {
        foreach (string weaponName in weaponStatDict.Keys) {
            if (!weaponSpriteByBaseName.ContainsKey(weaponName)) {
                Debug.LogWarning($"Missing weapon sprite for '{weaponName}'");
            }

            if (!legendaryStatDict.ContainsKey(weaponName)) {
                Debug.LogWarning($"Missing legendary weapon stats for '{weaponName}'");
            }
        }

        foreach (string weaponName in weaponSpriteByBaseName.Keys) {
            if (!weaponStatDict.ContainsKey(weaponName)) {
                Debug.LogWarning($"Weapon sprite '{weaponName}' does not have base stats");
            }
        }

        if (!spriteLookup.ContainsKey("glass_sword_shattered")) {
            Debug.LogWarning("Missing shattered glass sword sprite alias");
        }
    }

    private static string GetWeaponNameFromSpriteName(string spriteName) {
        return spriteName switch {
            "glass_sword" => "glass sword",
            "glass_sword_shattered" => "shattered glass sword",
            _ => spriteName.Replace('_', ' ')
        };
    }

    private static string BuildWeaponFullName(string weaponBaseName, string modifier) {
        if (string.IsNullOrWhiteSpace(weaponBaseName)) { return ""; }
        if (modifier == "shattered" && weaponBaseName == "glass sword") { return "shattered glass sword"; }
        if (string.IsNullOrWhiteSpace(modifier)) { return weaponBaseName; }
        return $"{modifier} {weaponBaseName}";
    }

    public void InvalidateInventoryCache() {
        inventoryCacheDirty = true;
    }

    private void EnsureInventoryCache() {
        if (!inventoryCacheDirty) { return; }

        inventoryItemCounts.Clear();
        firstInventoryItemByName.Clear();
        charmCountsByModifier.Clear();
        tarotCountsByModifier.Clear();
        cachedEquippedWeapon = null;
        cachedEquippedWeaponBaseName = "";
        cachedEquippedWeaponIsLegendary = false;

        if (s == null || s.player == null || s.player.inventory == null) {
            inventoryCacheDirty = true;
            return;
        }

        for (int i = 0; i < s.player.inventory.Count; i++) {
            GameObject inventoryObject = s.player.inventory[i];
            if (inventoryObject == null) { continue; }

            Item itemScript = inventoryObject.GetComponent<Item>();
            if (itemScript == null) { continue; }

            if (!string.IsNullOrEmpty(itemScript.itemName)) {
                string normalizedItemName = itemScript.itemType == "weapon"
                    ? itemScript.itemName
                    : NormalizeCommonItemKey(itemScript.itemName);
                if (itemScript.itemType != "weapon") {
                    itemScript.itemName = normalizedItemName;
                    itemScript.gameObject.name = normalizedItemName;
                }

                inventoryItemCounts[normalizedItemName] = GetDictionaryCount(inventoryItemCounts, normalizedItemName) + 1;
                firstInventoryItemByName.TryAdd(normalizedItemName, inventoryObject);
            }

            if (itemScript.itemName == "charm" && !string.IsNullOrEmpty(itemScript.modifier)) {
                charmCountsByModifier[itemScript.modifier] = GetDictionaryCount(charmCountsByModifier, itemScript.modifier) + 1;
            }
            else if (itemScript.itemName == "tarot" && !string.IsNullOrEmpty(itemScript.modifier)) {
                tarotCountsByModifier[itemScript.modifier] = GetDictionaryCount(tarotCountsByModifier, itemScript.modifier) + 1;
            }

            if (i == 0 && itemScript.itemType == "weapon") {
                cachedEquippedWeapon = itemScript;
                cachedEquippedWeaponBaseName = NormalizeWeaponSaveName(itemScript.itemName);
                cachedEquippedWeaponIsLegendary = itemScript.modifier == "legendary";
            }
        }

        inventoryCacheDirty = false;
    }

    private static int GetDictionaryCount(Dictionary<string, int> dictionary, string key) {
        return dictionary.TryGetValue(key, out int count) ? count : 0;
    }

    private int GetInventoryItemCount(string itemName) {
        EnsureInventoryCache();
        if (string.IsNullOrEmpty(itemName)) { return 0; }

        string normalizedName = NormalizeCommonItemKey(itemName);
        return GetDictionaryCount(inventoryItemCounts, normalizedName);
    }

    public bool IsMerchantEncounter() {
        return s != null && s.enemy != null && s.enemy.enemyName.text == "Merchant";
    }

    public bool IsBlacksmithEncounter() {
        return s != null && s.enemy != null && s.enemy.enemyName.text == "Blacksmith";
    }

    public bool IsVendorEncounter() {
        return IsMerchantEncounter() || IsBlacksmithEncounter();
    }

    public bool IsFightableEncounter() {
        return s != null
            && s.enemy != null
            && !Save.game.enemyIsDead
            && s.enemy.enemyName.text is not "Merchant" and not "Blacksmith" and not "Tombstone";
    }

    public bool ShouldPreviewWeaponOnRight(GameObject selectedItem) {
        if (selectedItem == null || s == null || s.player == null || IsFightableEncounter()) {
            return false;
        }

        Item selectedItemScript = selectedItem.GetComponent<Item>();
        if (selectedItemScript == null || selectedItemScript.itemType != "weapon") {
            return false;
        }

        return s.player.inventory == null
            || s.player.inventory.Count == 0
            || s.player.inventory[0] != selectedItem;
    }

    public bool IsShowingForeignWeaponPreview(GameObject selectedItem = null) {
        return ShouldPreviewWeaponOnRight(selectedItem ?? highlightedItem);
    }

    public void RestoreCurrentEnemyStatsForDisplay() {
        if (s == null || s.enemy == null || Save.game == null) { return; }

        s.enemy.stats = new Dictionary<string, int> {
            { "green", Save.game.enemyAcc },
            { "blue", Save.game.enemySpd },
            { "red", Save.game.enemyDmg },
            { "white", Save.game.enemyDef },
        };

        if (s.statSummoner == null) { return; }

        s.statSummoner.SummonStats();
        s.statSummoner.SetDebugInformationFor("enemy");
    }

    public int GetHamLevelStartBonus() {
        if (!PlayerHasWeapon("ham")) { return 0; }
        Item equippedWeapon = GetEquippedWeapon();
        int saltShakerBonus = PlayerHas("salt_shaker") ? 1 : 0;
        if (equippedWeapon != null && equippedWeapon.modifier == "rotten") { return saltShakerBonus; }
        return (PlayerHasLegendary() ? 4 : 2) + saltShakerBonus;
    }

    public bool PlayerAlwaysChoosesFirstDraftDie() {
        return EnemyHasForcedKneeInitiativeLoss() || PlayerHasWarhammerStunActive() || PlayerHasForcedDraftInitiativeOverride();
    }

    public bool PlayerAlwaysChoosesLastDraftDie() {
        return s != null
            && s.player != null
            && s.player.woundList.Contains("knee")
            && !PlayerAlwaysChoosesFirstDraftDie();
    }

    public bool PlayerAlwaysActsFirst() {
        return EnemyHasForcedKneeInitiativeLoss() || PlayerHasWarhammerStunActive() || PlayerHasForcedActionInitiativeOverride();
    }

    public bool PlayerAlwaysActsLast() {
        return s != null
            && s.player != null
            && s.player.woundList.Contains("knee")
            && !PlayerAlwaysActsFirst();
    }

    public bool PlayerHasForcedDraftInitiativeOverride() {
        return PlayerHasWeapon("gauntlets") || PlayerHasWeapon("spear");
    }

    public bool PlayerHasForcedActionInitiativeOverride() {
        return PlayerHasWeapon("gauntlets") || PlayerHasLegendarySpear();
    }

    public bool PlayerHasLegendarySpear() {
        return PlayerHasWeapon("spear") && PlayerHasLegendary();
    }

    public bool EnemyHasForcedKneeInitiativeLoss() {
        return s != null
            && s.enemy != null
            && s.enemy.enemyName.text != "Lich"
            && (s.enemy.woundList.Contains("knee") || EnemyHasTemporaryKneeInjury());
    }

    public bool EnemyHasTemporaryChestInjury() {
        return Save.game != null && Save.game.enemyScrollChestActive;
    }

    public bool EnemyHasTemporaryGutsInjury() {
        return Save.game != null && Save.game.enemyScrollGutsActive;
    }

    public bool EnemyHasTemporaryKneeInjury() {
        return Save.game != null && Save.game.enemyScrollKneeActive;
    }

    public bool EnemyHasTemporaryHipInjury() {
        return Save.game != null && Save.game.enemyScrollHipActive;
    }

    public bool EnemyHasTemporaryHandInjury() {
        return Save.game != null && Save.game.enemyScrollHandActive;
    }

    public bool EnemyHasTemporaryArmpitsInjury() {
        return Save.game != null && Save.game.enemyScrollArmpitsActive;
    }

    public void ApplyTemporaryEnemyWitchHandCurse(int reductionPerStat) {
        if (Save.game == null || s == null || s.enemy == null || reductionPerStat <= 0) { return; }

        int greenPenalty = Mathf.Min(reductionPerStat, s.enemy.stats["green"]);
        int bluePenalty = Mathf.Min(reductionPerStat, s.enemy.stats["blue"]);
        int redPenalty = Mathf.Min(reductionPerStat, s.enemy.stats["red"]);
        int whitePenalty = Mathf.Min(reductionPerStat, s.enemy.stats["white"]);

        s.enemy.stats["green"] -= greenPenalty;
        s.enemy.stats["blue"] -= bluePenalty;
        s.enemy.stats["red"] -= redPenalty;
        s.enemy.stats["white"] -= whitePenalty;

        Save.game.enemyWitchHandPenaltyGreen += greenPenalty;
        Save.game.enemyWitchHandPenaltyBlue += bluePenalty;
        Save.game.enemyWitchHandPenaltyRed += redPenalty;
        Save.game.enemyWitchHandPenaltyWhite += whitePenalty;

        Save.game.enemyAcc = s.enemy.stats["green"];
        Save.game.enemySpd = s.enemy.stats["blue"];
        Save.game.enemyDmg = s.enemy.stats["red"];
        Save.game.enemyDef = s.enemy.stats["white"];
    }

    public void ClearTemporaryEnemyWitchHandCurse() {
        if (Save.game == null) { return; }

        bool hasPenalty = Save.game.enemyWitchHandPenaltyGreen > 0
            || Save.game.enemyWitchHandPenaltyBlue > 0
            || Save.game.enemyWitchHandPenaltyRed > 0
            || Save.game.enemyWitchHandPenaltyWhite > 0;
        if (!hasPenalty) { return; }

        if (s != null && s.enemy != null && IsFightableEncounter()) {
            s.enemy.stats["green"] += Save.game.enemyWitchHandPenaltyGreen;
            s.enemy.stats["blue"] = Save.game.enemyHasKatarSpeedPenalty
                ? Mathf.Max(0, Save.game.enemyKatarBaseSpeedAfterPenalty)
                : s.enemy.stats["blue"] + Save.game.enemyWitchHandPenaltyBlue;
            s.enemy.stats["red"] += Save.game.enemyWitchHandPenaltyRed;
            s.enemy.stats["white"] += Save.game.enemyWitchHandPenaltyWhite;
            Save.game.enemyAcc = s.enemy.stats["green"];
            Save.game.enemySpd = s.enemy.stats["blue"];
            Save.game.enemyDmg = s.enemy.stats["red"];
            Save.game.enemyDef = s.enemy.stats["white"];
        }

        Save.game.enemyWitchHandPenaltyGreen = 0;
        Save.game.enemyWitchHandPenaltyBlue = 0;
        Save.game.enemyWitchHandPenaltyRed = 0;
        Save.game.enemyWitchHandPenaltyWhite = 0;
    }

    public void ClearTemporaryEnemyInjuryScrollEffects() {
        if (Save.game == null) { return; }

        Save.game.enemyScrollChestActive = false;
        Save.game.enemyScrollGutsActive = false;
        Save.game.enemyScrollKneeActive = false;
        Save.game.enemyScrollHipActive = false;
        Save.game.enemyScrollHandActive = false;
        Save.game.enemyScrollArmpitsActive = false;
        ClearTemporaryEnemyWitchHandCurse();
    }

    public bool PlayerHasWarhammerStunActive() {
        return Save.game != null && Save.game.warhammerStunActive;
    }

    public void QueueWarhammerStunForNextTurn() {
        if (Save.game == null) { return; }

        Save.game.warhammerStunNextTurn = true;
    }

    public void AdvanceWarhammerStunTurnState() {
        if (Save.game == null) { return; }

        Save.game.warhammerStunActive = Save.game.warhammerStunNextTurn;
        Save.game.warhammerStunNextTurn = false;
    }

    public void ClearWarhammerStunState() {
        if (Save.game == null) { return; }

        Save.game.warhammerStunActive = false;
        Save.game.warhammerStunNextTurn = false;
    }

    public int GetGladiusOpeningAttackBonus() {
        if (!Save.game.isFirstCombatRoundOfEncounter || !IsFightableEncounter() || !PlayerHasWeapon("gladius")) { return 0; }
        return PlayerHasLegendary() ? 5 : 3;
    }

    public int GetGogglesOpeningAccuracyBonus() {
        if (!Save.game.isFirstCombatRoundOfEncounter || !IsFightableEncounter()) { return 0; }

        int gogglesCount = GetPlayerItemCount("goggles");
        return gogglesCount * 3;
    }

    public int GetGogglesOpeningAttackBonus() {
        if (!Save.game.isFirstCombatRoundOfEncounter || !IsFightableEncounter()) { return 0; }

        int gogglesCount = GetPlayerItemCount("goggles");
        return gogglesCount * 2;
    }

    public void ClearRoundAttackWeaponBonuses(bool refreshCombatUI = false) {
        if (tridentImmediateAttackBonus == 0) { return; }

        tridentImmediateAttackBonus = 0;
        if (refreshCombatUI && s != null && s.statSummoner != null) {
            RefreshPlayerCombatStatsAndDice(recalculateTarget:false);
        }
    }

    public void ActivatePlayerActsFirstWeaponBonuses(bool refreshCombatUI = true) {
        int nextTridentBonus = IsFightableEncounter() && PlayerHasWeapon("trident") ? 1 : 0;
        if (tridentImmediateAttackBonus == nextTridentBonus) { return; }

        tridentImmediateAttackBonus = nextTridentBonus;
        if (refreshCombatUI && s != null && s.statSummoner != null) {
            RefreshPlayerCombatStatsAndDice(recalculateTarget:false);
        }
    }

    public int GetCurrentPlayerWeaponStatBonus(string stat) {
        int bonus = stat switch {
            "green" => GetGogglesOpeningAccuracyBonus(),
            "red" => GetGladiusOpeningAttackBonus() + tridentImmediateAttackBonus + GetGogglesOpeningAttackBonus(),
            _ => 0,
        };
        Item equippedWeapon = GetEquippedWeapon();
        if (equippedWeapon == null || GetWeaponBaseName(equippedWeapon.itemName) != "claymore") {
            return bonus;
        }

        int claymoreBonus = GetClaymoreBonusForWeapon(equippedWeapon);
        if (stat == "red") {
            bonus += claymoreBonus;
        }
        else if (stat == "blue") {
            bonus += Mathf.FloorToInt(claymoreBonus / 2f);
        }

        return bonus;
    }

    public int GetCurrentPlayerWeaponDiamondBonus(string stat) {
        return stat switch {
            "green" => GetGogglesOpeningAccuracyBonus(),
            "red" => GetGladiusOpeningAttackBonus() + tridentImmediateAttackBonus + GetGogglesOpeningAttackBonus(),
            _ => 0,
        };
    }

    public int GetClaymoreBonusForWeapon(Item weapon, int? staminaOverride = null) {
        if (weapon == null || GetWeaponBaseName(weapon.itemName) != "claymore") { return 0; }

        int stamina = staminaOverride ?? (s?.player != null ? s.player.stamina : Save.game.playerStamina);
        int breakpoint = weapon.modifier == "legendary" ? 3 : 4;
        return breakpoint <= 0 ? 0 : Mathf.Max(0, stamina / breakpoint);
    }

    public Dictionary<string, int> GetWeaponStatsForPreview(Item weapon, int? staminaOverride = null) {
        if (weapon == null) {
            return new Dictionary<string, int> {
                { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 }
            };
        }

        Dictionary<string, int> previewStats = weapon.weaponStats.ToDictionary(entry => entry.Key, entry => entry.Value);
        int claymoreBonus = GetClaymoreBonusForWeapon(weapon, staminaOverride);
        if (claymoreBonus > 0) {
            previewStats["blue"] += Mathf.FloorToInt(claymoreBonus / 2);
            previewStats["red"] += claymoreBonus;
        }
        return previewStats;
    }

    public int GetFoodStaminaAmount(string itemName, bool includeCharacterBonus = true) {
        itemName = NormalizeCommonItemKey(itemName);
        int staminaAmount = itemName switch {
            "steak" => 5,
            "cheese" => 3,
            "rotten_steak" => 0,
            "moldy_cheese" => 0,
            _ => 0,
        };

        if (includeCharacterBonus && Save.game.curCharNum == 0 && itemName is "steak" or "cheese") {
            staminaAmount += 2;
        }

        if (PlayerHas("salt_shaker")) {
            staminaAmount += itemName switch {
                "steak" or "cheese" => 2,
                "rotten_steak" or "moldy_cheese" => 1,
                _ => 0,
            };
        }

        return staminaAmount;
    }

    public int GetHolyWaterStaminaAmount() {
        int staminaAmount = 10;
        if (Save.game.curCharNum == 0) {
            staminaAmount += 2;
        }
        if (PlayerHas("salt_shaker")) {
            staminaAmount += 2;
        }
        return staminaAmount;
    }

    public int GetCursedMaskRegenAmount() {
        int cursedMaskCount = GetPlayerItemCount("cursed_mask");
        if (cursedMaskCount <= 0 || s?.player == null) { return 0; }

        int woundCount = Mathf.Min(2, s.player.woundList.Count);
        return woundCount <= 0 ? 0 : cursedMaskCount * (woundCount >= 2 ? 4 : 2);
    }

    public int GetCursedDiceSpawnCount() {
        int cursedDiceCount = GetPlayerItemCount("cursed_dice");
        if (cursedDiceCount <= 0 || s?.player == null) { return 0; }

        int woundCount = Mathf.Min(2, s.player.woundList.Count);
        return cursedDiceCount * woundCount;
    }

    public bool ShouldSpawnCursedDiceAtDraftStart() {
        return IsFightableEncounter() && GetCursedDiceSpawnCount() > 0;
    }

    private static float ParseSacrificialChaliceCharge(string modifier) {
        if (string.IsNullOrWhiteSpace(modifier)) { return 0f; }
        return float.TryParse(modifier, out float parsed)
            ? Mathf.Clamp(parsed, 0f, 3f)
            : 0f;
    }

    private static string FormatSacrificialChaliceCharge(float charge) {
        float rounded = Mathf.Clamp(Mathf.Round(charge * 2f) / 2f, 0f, 3f);
        return Mathf.Approximately(rounded % 1f, 0.5f)
            ? $"{Mathf.FloorToInt(rounded)}.5"
            : Mathf.RoundToInt(rounded).ToString();
    }

    private static float SnapSacrificialChaliceChargeStep(float charge) {
        return Mathf.Clamp(Mathf.Round(charge * 2f) / 2f, 0f, 3f);
    }

    private static float SumSacrificialChaliceCharges(IEnumerable<Item> chalices) {
        float sum = 0f;
        foreach (Item chalice in chalices) {
            sum += ParseSacrificialChaliceCharge(chalice.modifier);
        }
        return sum;
    }

    private void SyncSacrificialChaliceSaveTotalFromInventory(List<Item> chalices) {
        Save.game.sacrificialChaliceCharge = chalices.Count == 0 ? 0f : SumSacrificialChaliceCharges(chalices);
    }

    private List<Item> GetSacrificialChaliceItemsInInventory() {
        if (s?.player?.inventory == null) { return new List<Item>(); }

        return s.player.inventory
            .Where(inventoryItem => inventoryItem != null)
            .Select(inventoryItem => inventoryItem.GetComponent<Item>())
            .Where(itemScript => itemScript != null && itemScript.itemName == "sacrificial_chalice")
            .ToList();
    }

    private void EnsureSacrificialChaliceModifierData() {
        List<Item> chalices = GetSacrificialChaliceItemsInInventory();
        if (chalices.Count == 0) {
            Save.game.sacrificialChaliceCharge = 0f;
            return;
        }

        bool anyHasCharge = false;
        foreach (Item chalice in chalices) {
            if (string.IsNullOrWhiteSpace(chalice.modifier)) {
                chalice.modifier = "0";
                continue;
            }
            anyHasCharge |= ParseSacrificialChaliceCharge(chalice.modifier) > 0f;
        }

        if (!anyHasCharge && Save.game.sacrificialChaliceCharge > 0f) {
            // Old saves stored one combined total; split evenly so each chalice shows the same
            // per-copy charge (e.g.2 chalices + 2 combined → +1 on each, not +2 on one).
            float legacyCombined = Mathf.Clamp(Save.game.sacrificialChaliceCharge, 0f, 3f * chalices.Count);
            float perChalice = SnapSacrificialChaliceChargeStep(legacyCombined / chalices.Count);
            foreach (Item chalice in chalices) {
                chalice.modifier = FormatSacrificialChaliceCharge(perChalice);
            }
        }

        // Multiple copies always advance together; resync if modifiers drifted apart.
        if (chalices.Count > 1) {
            float a0 = ParseSacrificialChaliceCharge(chalices[0].modifier);
            bool allMatch = true;
            for (int i = 1; i < chalices.Count; i++) {
                if (!Mathf.Approximately(a0, ParseSacrificialChaliceCharge(chalices[i].modifier))) {
                    allMatch = false;
                    break;
                }
            }
            if (!allMatch) {
                float average = SumSacrificialChaliceCharges(chalices) / chalices.Count;
                float snapped = SnapSacrificialChaliceChargeStep(average);
                foreach (Item chalice in chalices) {
                    chalice.modifier = FormatSacrificialChaliceCharge(snapped);
                }
            }
        }

        SyncSacrificialChaliceSaveTotalFromInventory(chalices);
    }

    public float GetSacrificialChaliceCombinedCharge() {
        EnsureSacrificialChaliceModifierData();
        return SumSacrificialChaliceCharges(GetSacrificialChaliceItemsInInventory());
    }

    public int GetSacrificialChaliceAppliedBonus() {
        if (!PlayerHas("sacrificial_chalice")) { return 0; }
        // Whole stat bonus is floor(total charge across chalices): two at 1.5 → +3, not +1+1.
        float combined = GetSacrificialChaliceCombinedCharge();
        return Mathf.FloorToInt(combined + 0.0001f);
    }

    public bool TryAdvanceSacrificialChalice(bool refreshCombatUI = true) {
        if (!PlayerHas("sacrificial_chalice")) { return false; }

        EnsureSacrificialChaliceModifierData();
        List<Item> chalices = GetSacrificialChaliceItemsInInventory();
        float previousCombinedCharge = SumSacrificialChaliceCharges(chalices);
        int previousAppliedBonus = Mathf.FloorToInt(previousCombinedCharge + 0.0001f);
        bool advancedAnyChalice = false;

        foreach (Item chalice in chalices) {
            float previousCharge = ParseSacrificialChaliceCharge(chalice.modifier);
            float nextCharge = previousCharge <= 0f ? 1f : Mathf.Min(3f, previousCharge + 0.5f);
            if (Mathf.Approximately(previousCharge, nextCharge)) { continue; }

            chalice.modifier = FormatSacrificialChaliceCharge(nextCharge);
            advancedAnyChalice = true;
        }

        if (!advancedAnyChalice) { return false; }

        float nextCombinedCharge = SumSacrificialChaliceCharges(chalices);
        Save.game.sacrificialChaliceCharge = nextCombinedCharge;

        if (refreshCombatUI) {
            if (Mathf.FloorToInt(nextCombinedCharge) != previousAppliedBonus) {
                RefreshPlayerCombatStatsAndDice();
            }
            RefreshHighlightedItemDescription();
        }

        SaveInventoryItems();
        return true;
    }

    public string GetSacrificialChaliceDescription(string chaliceModifier = null) {
        float charge = chaliceModifier == null
            ? 0f
            : ParseSacrificialChaliceCharge(chaliceModifier);

        if (charge >= 3f) {
            return "sacrificial chalice\nit's full...  +3";
        }

        return $"sacrificial chalice\nit thirsts...  +{FormatHalfStepValue(charge)}";
    }

    public int GetVendorTakeAllowance() {
        return IsMerchantEncounter() && PlayerHas("thiefs_armband") ? 2 : 0;
    }

    public bool TryConsumeMerchantStealAllowance() {
        if (!IsMerchantEncounter() || Save.game == null || Save.game.merchantStealAllowanceRemaining <= 0) {
            return false;
        }

        Save.game.merchantStealAllowanceRemaining--;
        return true;
    }

    public string GetAttachedStatForDieType(string diceType, string owner) {
        if (owner == "player") {
            if (diceType == "yellow" || diceType == "green" && PlayerHasWeapon("dagger") || diceType == "white" && Save.game.curCharNum == 3) {
                return "red";
            }

            return diceType;
        }

        return diceType == "yellow" ? "red" : diceType;
    }

    public void RefreshHighlightedItemDescription() {
        if (highlightedItem == null) { return; }

        Item highlightedItemScript = highlightedItem.GetComponent<Item>();
        if (highlightedItemScript == null) { return; }

        itemDesc.text = GetDisplayTextForItem(highlightedItemScript);
    }

    public void TransmuteAttachedDie(Dice dice) {
        if (dice == null || !dice.isAttached || dice.isOnPlayerOrEnemy == "none") { return; }
        s.soundManager.PlayClip("zap");

        string owner = dice.isOnPlayerOrEnemy;
        Dictionary<string, List<Dice>> diceDictionary = owner == "player"
            ? s.statSummoner.addedPlayerDice
            : s.statSummoner.addedEnemyDice;

        if (!string.IsNullOrEmpty(dice.statAddedTo) && diceDictionary.TryGetValue(dice.statAddedTo, out List<Dice> currentDiceList)) {
            currentDiceList.Remove(dice);
        }

        string[] randomTypes = owner == "player"
            ? new[] { "green", "blue", "red", "white", "yellow" }
            : new[] { "green", "blue", "red", "white" };
        string newType = randomTypes[Random.Range(0, randomTypes.Length)];
        int newValue = Random.Range(1, 7);
        string newStat = GetAttachedStatForDieType(newType, owner);

        dice.tarotUpgradeApplied = false;
        dice.SetDieType(newType);
        dice.SetDiceValue(newValue);
        dice.statAddedTo = newStat;

        if (owner == "player") {
            if (newType == "yellow") {
                dice.moveable = true;
            }
            s.statSummoner.AddDiceToPlayer(newStat, dice);
            ApplyPlayerDieTransmuteWoundEffects(dice);
        }
        else {
            s.statSummoner.AddDiceToEnemy(newStat, dice);
            ApplyEnemyDieTransmuteWoundEffects(dice);
        }

        s.statSummoner.SummonStats();
        s.statSummoner.RepositionAllDice("player");
        s.statSummoner.RepositionAllDice("enemy");
        s.statSummoner.SetCombatDebugInformationFor("player");
        s.statSummoner.SetCombatDebugInformationFor("enemy");
        s.turnManager.RecalculateMaxFor("player");
        s.turnManager.RecalculateMaxFor("enemy");
        s.turnManager.RecalculateEnemyCombatIntent();
        s.diceSummoner.SaveDiceValues();
    }

    public void TransmuteDie(Dice dice) {
        if (dice == null) { return; }
        
        // attached dice go through the full transmute logic
        if (dice.isAttached && dice.isOnPlayerOrEnemy != "none") {
            TransmuteAttachedDie(dice);
            return;
        }
        
        // unattached dice (e.g. during draft) just get mutated in place
        s.soundManager.PlayClip("zap");
        
        string[] randomTypes = { "green", "blue", "red", "white", "yellow" };
        string newType = randomTypes[Random.Range(0, randomTypes.Length)];
        int newValue = Random.Range(1, 7);
        
        dice.tarotUpgradeApplied = false;
        dice.moveable = true;
        dice.isAttached = false;
        dice.isOnPlayerOrEnemy = "none";
        dice.statAddedTo = string.Empty;
        dice.SetDieType(newType);
        dice.SetDiceValue(newValue);
        dice.instantiationPos = dice.transform.position;
        s.diceSummoner.SaveDiceValues(0.05f);
    }

    public void TransformAttachedPlayerDieToColor(Dice dice, string newType) {
        if (dice == null || !dice.isAttached || dice.isOnPlayerOrEnemy != "player") { return; }
        if (newType != "green" && newType != "blue" && newType != "red" && newType != "white" && newType != "yellow") { return; }

        if (!string.IsNullOrEmpty(dice.statAddedTo) && s.statSummoner.addedPlayerDice.TryGetValue(dice.statAddedTo, out List<Dice> currentDiceList)) {
            currentDiceList.Remove(dice);
        }

        string previousAttachedStat = dice.statAddedTo;
        dice.tarotUpgradeApplied = false;
        dice.SetDieType(newType);
        dice.statAddedTo = newType == "yellow" && !string.IsNullOrEmpty(previousAttachedStat)
            ? previousAttachedStat
            : GetAttachedStatForDieType(newType, "player");
        if (newType == "yellow") {
            dice.moveable = true;
        }
        s.statSummoner.AddDiceToPlayer(dice.statAddedTo, dice);
        ApplyPlayerDieTransmuteWoundEffects(dice);
        TryUpgradeTakenDieWithTarot(dice, 0f);

        s.statSummoner.SummonStats();
        s.statSummoner.RepositionAllDice("player");
        s.statSummoner.SetCombatDebugInformationFor("player");
        s.turnManager.RecalculateMaxFor("player");
        s.turnManager.RecalculateEnemyCombatIntent();
        s.diceSummoner.SaveDiceValues();
    }

    public void RemoveCursedDiceIfPlayerHealedBeforeAttacking() {
        if (s?.player == null || s.diceSummoner == null || s.turnManager == null) { return; }
        if (s.player.woundList.Count > 0 || s.turnManager.playerHasAttackedThisRound) { return; }

        bool removedAny = false;
        foreach (GameObject dieObject in s.diceSummoner.existingDice.ToList()) {
            Dice die = dieObject == null ? null : dieObject.GetComponent<Dice>();
            if (die == null || !die.spawnedByCursedDice) { continue; }
            die.spawnedByCursedDice = false;
            die.FadeOut();
            removedAny = true;
        }

        if (removedAny) {
            s.turnManager.RecalculateEnemyCombatIntent();
            s.diceSummoner.SaveDiceValues();
        }
    }

    private void ApplyPlayerDieTransmuteWoundEffects(Dice dice) {
        if (dice == null) { return; }

        if (s.player.woundList.Contains("guts")) { StartCoroutine(dice.DecreaseDiceValue(false)); }
        if (dice.diceType == "red" && s.player.woundList.Contains("armpits")) { StartCoroutine(dice.FadeOut()); }
        else if (dice.diceType == "white" && s.player.woundList.Contains("hand")) { StartCoroutine(dice.FadeOut()); }
        else if (dice.diceType == "white" && Save.game.curCharNum == 2) { dice.SetToOne(); }
    }

    private void ApplyEnemyDieTransmuteWoundEffects(Dice dice) {
        if (dice == null) { return; }

        TryWeakenEnemyTakenDieWithTarot(dice, 0f);

        if (s.enemy.enemyName.text == "Lich") { return; }

        if (dice.diceType == "red" && s.enemy.woundList.Contains("armpits")) { StartCoroutine(dice.FadeOut()); }
        else if (dice.diceType == "white" && s.enemy.woundList.Contains("hand")) { StartCoroutine(dice.FadeOut()); }
    }

    private static string FormatHalfStepValue(float value) {
        float rounded = Mathf.Clamp(Mathf.Round(value * 2f) / 2f, 0f, 3f);
        return Mathf.Approximately(rounded % 1f, 0.5f)
            ? $"{Mathf.FloorToInt(rounded)}.5"
            : Mathf.RoundToInt(rounded).ToString();
    }

    public void LoadLuckyDiceRoundStatsFromSave() {
        luckyDiceRoundStats["green"] = Save.game.luckyStatGreen;
        luckyDiceRoundStats["blue"] = Save.game.luckyStatBlue;
        luckyDiceRoundStats["red"] = Save.game.luckyStatRed;
        luckyDiceRoundStats["white"] = Save.game.luckyStatWhite;
        ClampLuckyDiceRoundStatsToMinimums();
        luckyDiceRoundStatsInitialized = Save.game.hasLuckyDiceRoundStats
            || luckyDiceRoundStats.Values.Any(value => value != 0);
    }

    public void SyncLuckyDiceRoundStatsToSave() {
        if (!luckyDiceRoundStatsInitialized
            && Save.game != null
            && (Save.game.hasLuckyDiceRoundStats
            || Save.game.luckyStatGreen != 0
            || Save.game.luckyStatBlue != 0
            || Save.game.luckyStatRed != 0
            || Save.game.luckyStatWhite != 0)) {
            return;
        }

        SyncLuckyDiceRoundStatsToSave(luckyDiceRoundStatsInitialized);
    }

    private void SyncLuckyDiceRoundStatsToSave(bool hasRoundStats) {
        ClampLuckyDiceRoundStatsToMinimums();
        Save.game.luckyStatGreen = luckyDiceRoundStats["green"];
        Save.game.luckyStatBlue = luckyDiceRoundStats["blue"];
        Save.game.luckyStatRed = luckyDiceRoundStats["red"];
        Save.game.luckyStatWhite = luckyDiceRoundStats["white"];
        Save.game.hasLuckyDiceRoundStats = hasRoundStats;
    }

    private int GetPlayerStatTotalWithoutLuckyDice(string stat) {
        return s.player.stats[stat]
            + s.player.potionStats[stat]
            + neckletStats[stat]
            + charmPassiveStats[stat]
            + charmActiveBonus[stat]
            + GetSacrificialChaliceAppliedBonus()
            + GetCurrentPlayerWeaponStatBonus(stat);
    }

    private int GetMinimumLuckyDiceRoundBonus(string stat) {
        if (s?.player == null || string.IsNullOrWhiteSpace(stat) || !luckyDiceRoundStats.ContainsKey(stat)) {
            return int.MinValue;
        }

        return -1 - GetPlayerStatTotalWithoutLuckyDice(stat);
    }

    private void ClampLuckyDiceRoundStatsToMinimums() {
        if (s?.player == null) { return; }

        foreach (string stat in statArr) {
            int minimumBonus = GetMinimumLuckyDiceRoundBonus(stat);
            if (luckyDiceRoundStats[stat] < minimumBonus) {
                luckyDiceRoundStats[stat] = minimumBonus;
            }
        }
    }

    private bool HasSavedLuckyDiceRoundStats() {
        return luckyDiceRoundStatsInitialized
            || (Save.game != null && (Save.game.hasLuckyDiceRoundStats
            || Save.game.luckyStatGreen != 0
            || Save.game.luckyStatBlue != 0
            || Save.game.luckyStatRed != 0
            || Save.game.luckyStatWhite != 0));
    }

    private bool IsLuckyDiceInventoryStatePendingRestore() {
        return s?.player?.inventory != null
            && !Save.game.newGame
            && s.player.inventory.Count == 0
            && Save.game.resumeItemNames != null
            && Save.game.resumeItemNames.Any(itemName => !string.IsNullOrWhiteSpace(itemName));
    }

    private void RefreshLuckyDiceRoundStatsForCurrentState(bool rerollLuckyDice, bool refreshCombatUI = true) {
        if (IsLuckyDiceInventoryStatePendingRestore()) {
            if (HasSavedLuckyDiceRoundStats()) {
                LoadLuckyDiceRoundStatsFromSave();
            }
            return;
        }

        int luckyDiceCount = GetInventoryItemCount("lucky_dice");
        bool shouldApplyLuckyDice = luckyDiceCount > 0 && ShouldApplyLuckyDiceRoundStats();

        if (!shouldApplyLuckyDice) {
            ClearLuckyDiceRoundStats();
            if (refreshCombatUI) {
                RefreshPlayerCombatStatsAndDice();
            }
            return;
        }

        if (rerollLuckyDice || !HasSavedLuckyDiceRoundStats()) {
            RollLuckyDiceRoundStats(refreshCombatUI);
            return;
        }

        LoadLuckyDiceRoundStatsFromSave();
        SyncLuckyDiceRoundStatsToSave(true);

        if (refreshCombatUI) {
            RefreshPlayerCombatStatsAndDice();
        }
    }

    public void ClearLuckyDiceRoundStats() {
        luckyDiceRoundStats["green"] = 0;
        luckyDiceRoundStats["blue"] = 0;
        luckyDiceRoundStats["red"] = 0;
        luckyDiceRoundStats["white"] = 0;
        luckyDiceRoundStatsInitialized = false;
        SyncLuckyDiceRoundStatsToSave(false);
    }

    public int GetLuckyDiceRoundStatBonus(string stat) {
        if (!luckyDiceRoundStats.TryGetValue(stat, out int value)) { return 0; }

        int minimumBonus = GetMinimumLuckyDiceRoundBonus(stat);
        if (minimumBonus != int.MinValue && value < minimumBonus) {
            value = minimumBonus;
            luckyDiceRoundStats[stat] = value;
        }

        return value;
    }

    private bool ShouldApplyLuckyDiceRoundStats() {
        return IsFightableEncounter();
    }

    public void RollLuckyDiceRoundStats(bool refreshCombatUI = true) {
        luckyDiceRoundStats["green"] = 0;
        luckyDiceRoundStats["blue"] = 0;
        luckyDiceRoundStats["red"] = 0;
        luckyDiceRoundStats["white"] = 0;
        int luckyDiceCount = GetInventoryItemCount("lucky_dice");

        if (luckyDiceCount <= 0 || !ShouldApplyLuckyDiceRoundStats()) {
            luckyDiceRoundStatsInitialized = false;
            SyncLuckyDiceRoundStatsToSave(false);
            if (refreshCombatUI) {
                RefreshPlayerCombatStatsAndDice();
            }
            return;
        }

        for (int i = 0; i < luckyDiceCount; i++) {
            Dictionary<string, int> rollDeltas = new();
            List<string> positiveStats = new();

            foreach (string stat in statArr) {
                int delta = Random.value < 0.5f ? -1 : 1;
                rollDeltas[stat] = delta;
                if (delta > 0) {
                    positiveStats.Add(stat);
                }
            }

            if (positiveStats.Count > 0) {
                string boostedStat = positiveStats[Random.Range(0, positiveStats.Count)];
                rollDeltas[boostedStat] += 1;
            }
            else {
                string boostedStat = statArr[Random.Range(0, statArr.Length)];
                rollDeltas[boostedStat] = 2;
            }

            foreach (string stat in statArr) {
                luckyDiceRoundStats[stat] += rollDeltas[stat];
            }

            ClampLuckyDiceRoundStatsToMinimums();
        }

        ClampLuckyDiceRoundStatsToMinimums();
    luckyDiceRoundStatsInitialized = true;
        SyncLuckyDiceRoundStatsToSave(true);
        
        if (refreshCombatUI) {
            RefreshPlayerCombatStatsAndDice();
        }
    }

    public void RefreshPassiveInventoryEffects(bool updateUI = true) {
        EnsureInventoryCache();
        RecalculateNeckletEffects();
        UpdateCharmPassiveStats();
        if (charmActiveProcCounts.Values.Any(value => value > 0) || charmPendingProcCounts.Values.Any(value => value > 0)) {
            RebuildCharmTriggeredBonuses();
        }

        if (!updateUI || s == null || s.statSummoner == null) { return; }

        RefreshPlayerCombatStatsAndDice();
    }

    public void RefreshPlayerCombatStatsAndDice(bool recalculateTarget = true) {
        if (s == null || s.statSummoner == null) { return; }

        s.statSummoner.SummonStats();
        s.statSummoner.RepositionAllDice("player");
        s.statSummoner.SetCombatDebugInformationFor("player");
        if (recalculateTarget && s.turnManager != null) {
            s.turnManager.RecalculateMaxFor("player");
        }
    }

    public void RefreshEnemyCombatStatsAndDice(bool recalculateTarget = true) {
        if (s == null || s.statSummoner == null) { return; }

        s.statSummoner.SummonStats();
        s.statSummoner.RepositionAllDice("enemy");
        s.statSummoner.SetCombatDebugInformationFor("enemy");
        if (recalculateTarget && s.turnManager != null) {
            s.turnManager.RecalculateMaxFor("enemy");
        }
    }

    private void RecalculateNeckletEffects() {
        foreach (string stat in statArr) {
            neckletStats[stat] = 0;
            neckletCounter[stat] = 0;
        }

        int effectiveArcaneCount = 1 + (PlayerHasWeapon("stave") ? 1 : 0);
        if (s?.player?.inventory != null) {
            foreach (GameObject inventoryItem in s.player.inventory) {
                if (inventoryItem == null) { continue; }

                Item itemScript = inventoryItem.GetComponent<Item>();
                if (itemScript == null || itemScript.itemName != "necklet") { continue; }

                switch (itemScript.modifier) {
                    case "solidity": neckletCounter["green"]++; break;
                    case "rapidity": neckletCounter["blue"]++; break;
                    case "strength": neckletCounter["red"]++; break;
                    case "defense": neckletCounter["white"]++; break;
                    case "arcane": effectiveArcaneCount++; break;
                }
            }
        }

        neckletCounter["arcane"] = effectiveArcaneCount;
        foreach (string stat in statArr) {
            neckletStats[stat] = effectiveArcaneCount * neckletCounter[stat];
        }
    }

    public string RollTorchFadeModifier() {
        return $"rooms:{Random.Range(MinTorchCombatLifetime, MaxTorchCombatLifetime + 1)}";
    }

    public void BeginNewEncounterWeaponState(bool rerollLuckyDice = false) {
        InvalidateInventoryCache();
        ClearRoundAttackWeaponBonuses(refreshCombatUI:false);
        Save.game.enemyHasKatarSpeedPenalty = false;
        Save.game.enemyKatarSpeedPenaltyAmount = 0;
        Save.game.enemyKatarBaseSpeedAfterPenalty = 0;
        if (Save.game.enemyWounds == null || Save.game.enemyWounds.Count == 0 || Save.game.enemyIsDead) {
            ClearWarhammerStunState();
        }
        bool isFightable = IsFightableEncounter();
        Save.game.isFirstCombatRoundOfEncounter = isFightable;
        Save.game.pendingMirrorCopy = false;
        Save.game.pendingSpellbookTransmute = false;
        RefreshLuckyDiceRoundStatsForCurrentState(rerollLuckyDice, refreshCombatUI:true);
    }

    public void EndEncounterWeaponState(bool rerollLuckyDice = false) {
        InvalidateInventoryCache();
        ClearRoundAttackWeaponBonuses(refreshCombatUI:false);
        Save.game.isFirstCombatRoundOfEncounter = false;
        Save.game.pendingMirrorCopy = false;
        Save.game.pendingSpellbookTransmute = false;
        RefreshLuckyDiceRoundStatsForCurrentState(rerollLuckyDice, refreshCombatUI:true);
    }

    public void RefreshEncounterWeaponState() {
        Save.game.pendingMirrorCopy = Save.game.pendingMirrorCopy && IsFightableEncounter();
        Save.game.pendingSpellbookTransmute = Save.game.pendingSpellbookTransmute && IsFightableEncounter();
        if (!IsFightableEncounter()) {
            ClearRoundAttackWeaponBonuses(refreshCombatUI:false);
            ClearWarhammerStunState();
            Save.game.isFirstCombatRoundOfEncounter = false;
        }
        RefreshLuckyDiceRoundStatsForCurrentState(rerollLuckyDice:false, refreshCombatUI:true);
    }

    public bool TryApplyKatarFirstWoundEffect(bool enemyHadNoWoundsBeforeHit) {
        if (!enemyHadNoWoundsBeforeHit) { return false; }
        if (Save.game.enemyHasKatarSpeedPenalty || !PlayerHasWeapon("katar") || !IsFightableEncounter()) { return false; }
        if (s?.enemy == null || s.enemy.spawnNum == 0) { return false; }

        int katarPenalty = PlayerHasLegendary() ? 2 : 1;
        int baseEnemySpeed = s.enemy.stats["blue"] + Save.game.enemyWitchHandPenaltyBlue;
        int reducedBaseEnemySpeed = Mathf.Max(0, baseEnemySpeed - katarPenalty);
        int adjustedEnemySpeed = Mathf.Max(0, reducedBaseEnemySpeed - Save.game.enemyWitchHandPenaltyBlue);

        Save.game.enemyHasKatarSpeedPenalty = true;
        Save.game.enemyKatarSpeedPenaltyAmount = katarPenalty;
        Save.game.enemyKatarBaseSpeedAfterPenalty = reducedBaseEnemySpeed;
        s.enemy.stats["blue"] = adjustedEnemySpeed;
        Save.game.enemySpd = s.enemy.stats["blue"];
        if (s.tutorial == null) { Save.SaveGame(); }
        RefreshEnemyCombatStatsAndDice();
        return true;
    }

    public bool TryApplyKatarFirstWoundEffect() {
        return TryApplyKatarFirstWoundEffect(s?.enemy != null && s.enemy.woundList != null && s.enemy.woundList.Count == 0);
    }

    public bool IsUpgradeArrow(Item curItem) {
        return curItem != null && curItem.itemName == "forge";
    }

    public void UpdateVendorUIForSelection(GameObject selectedItem = null) {
        if (s?.enemy == null || !IsVendorEncounter()) { return; }

        if (IsBlacksmithEncounter() && selectedItem != null && floorItems.Contains(selectedItem)) {
            Item curItem = selectedItem.GetComponent<Item>();
            if (curItem != null) {
                if (curItem.itemType == "weapon") {
                    s.enemy.target.text = "exchange";
                    s.enemy.woundGUIElement.text = "[no wounds]";
                    return;
                }

                if (IsUpgradeArrow(curItem)) {
                    s.enemy.target.text = "forge";
                    s.enemy.woundGUIElement.text = "[no wounds]";
                    return;
                }
            }
        }

        s.enemy.target.text = "bargain";
        s.enemy.woundGUIElement.text = "[no wounds]";
    }

    private void Start() {
        s = FindFirstObjectByType<Scripts>();
        InvalidateInventoryCache();
        EnsureDropTablesBuilt();
        // assemble weighted drop tables for items and weapon modifiers
        if (isCharSelect) {
            // in character selection screen
            curList = floorItems;
            // assign the curlist variable for item selection navigation
            CreateWeaponWithStats("sword", "harsh", 2, 2, 1, 2);
            MoveItemToDisplay();
            CreateItem("steak");
            MoveItemToDisplay();
            CreateItem("torch");
            MoveItemToDisplay();
            // create the items
        }
        else if (isAlmanac) {
            // almanac scene - items are populated by AlmanacController
            lootText.text = "";
            curList = floorItems;
        }
        else {
            // in game
            lootText.text = "";
            curList = s.player.inventory;
            // assign the curlist variable for item selection navigation
            // need to implement a check if continuing or new game
        }
        // assign variables based on the Save, preventing cheating
        // StringBuilder builder = new();
        // builder.AppendLine("[startup debug loot printer] 50 random weapons");
        // for (int i = 0; i < 50; i++) {
        //     builder.AppendLine($"{i + 1}. {DebugRollRandomWeaponName()}");
        // }

        // builder.AppendLine("[startup debug loot printer] 100 random items");
        // for (int i = 0; i < 100; i++) {
        //     builder.AppendLine($"{i + 1}. {DebugRollRandomItemName()}");
        // }
        // Debug.Log(builder.ToString());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
            ChangeItemList();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !isCharSelect && !isAlmanac) {
            // if pressing left and not in a menu scene
            SelectLeft();
            // move the selection left
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !isCharSelect && !isAlmanac) {
            // if pressing right and not in a menu scene
            SelectRight();
            // move the selection to the right 
        }
        else if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) {
            // if pressing return or enter, depending on whether shift was pressed or not, drop or use the selected item
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                DropCurrentItem();
            }
            else { 
                UseCurrentItem();
            }
        }
    }
    
    public void ChangeItemList() {
        // if pressing one of the ctrl keys
        if (isCharSelect || isAlmanac) { return; }
        // if not in char select scene
        if (curList == s.player.inventory) {
            Select(floorItems, 0);
            return;
        }

        if (curList == floorItems) {
            Select(s.player.inventory, 0);
        }
        // swap the curList (used for selection) to the other
        // else { print("invalid list to select from"); }
        // something is wrong here
    }

    // kept in individual functions so that they may be called by buttons
    public void SelectLeft() { Select(curList, col - 1, false); }
    public void SelectRight() { Select(curList, col + 1, false); }
    public void UseCurrentItem() { 
        if (s.turnManager == null || s.turnManager.isMoving || isCharSelect || isAlmanac) { return; }
        // if not moving and in game
        highlightedItem.GetComponent<Item>().Use();
        // use the item
    }
    public void DropCurrentItem() { 
        highlightedItem.GetComponent<Item>().DropItem();
    }

    public static string GetCharmModifierFromDisplayName(string displayName) {
        return displayName switch {
            "charm of the unbroken" => "unbroken",
            "charm of the vindictive" => "vindictive",
            _ when displayName.StartsWith("charm of the ") => displayName.Substring("charm of the ".Length),
            _ => displayName,
        };
    }

    public static string GetCanonicalItemDisplayName(string itemName, string modifier) {
        string normalizedItemName = NormalizeCommonItemKey(itemName);
        return normalizedItemName switch {
            "scroll" => $"scroll of {modifier}",
            "potion" => $"potion of {modifier}",
            "gem" => $"{modifier} gem",
            "necklet" => modifier == "arcane" ? "arcane necklet" : $"necklet of {modifier}",
            "charm" => $"charm of the {modifier}",
            "tarot" => $"tarot of the {modifier}",
            "rabadons_deathcap" => "rabadon's deathcap",
            "bloodletters_curse" => "bloodletter's curse",
            "thiefs_armband" => "thief's armband",
            _ => normalizedItemName.Replace('_', ' '),
        };
    }

    private bool TryParseAlmanacEntry(string entryName, out string itemName, out string itemType, out string modifier) {
        itemName = entryName;
        itemType = "common";
        modifier = "";

        if (entryName.StartsWith("legendary ") || IsKnownWeaponName(entryName)) {
            itemType = "weapon";
            modifier = entryName.StartsWith("legendary ") ? "legendary" : "common";
            return true;
        }

        if (entryName.StartsWith("scroll of ")) {
            itemName = "scroll";
            modifier = entryName.Substring("scroll of ".Length);
            return true;
        }

        if (entryName.StartsWith("potion of ")) {
            itemName = "potion";
            modifier = entryName.Substring("potion of ".Length);
            return true;
        }

        if (entryName.EndsWith(" gem")) {
            itemName = "gem";
            modifier = entryName.Substring(0, entryName.Length - " gem".Length);
            return true;
        }

        if (entryName == "arcane necklet") {
            itemName = "necklet";
            modifier = "arcane";
            return true;
        }

        if (entryName.StartsWith("necklet of ")) {
            itemName = "necklet";
            modifier = entryName.Substring("necklet of ".Length);
            return true;
        }

        if (entryName.StartsWith("charm of the ")) {
            itemName = "charm";
            modifier = GetCharmModifierFromDisplayName(entryName);
            return true;
        }

        if (entryName.StartsWith("tarot of the ")) {
            itemName = "tarot";
            modifier = entryName.Substring("tarot of the ".Length);
            return true;
        }

        return true;
    }

    public string GetDisplayTextForItem(Item item, bool useDynamicValues = true) {
        return item == null ? "" : GetDisplayTextForItem(item.itemName, item.itemType, item.modifier, useDynamicValues);
    }

    public string GetDisplayTextForEntry(string entryName) {
        if (!TryParseAlmanacEntry(entryName, out string itemName, out string itemType, out string modifier)) {
            return entryName;
        }

        return GetDisplayTextForItem(itemName, itemType, modifier, useDynamicValues:false, displayNameOverride:entryName);
    }

    public string GetDisplayTextForItem(string itemName, string itemType, string modifier, bool useDynamicValues = true, string displayNameOverride = null) {
        itemName = itemType == "weapon" ? itemName : NormalizeCommonItemKey(itemName);
        if (itemName is "???" or "hint") { return "???\nnot yet discovered"; }

        if (itemType == "weapon") {
            string displayName = displayNameOverride ?? itemName;
            string descriptionKey = descriptionDict.ContainsKey(displayName)
                ? displayName
                : GetWeaponBaseName(displayName);
            if (!descriptionDict.TryGetValue(descriptionKey, out string weaponDescription) || string.IsNullOrEmpty(weaponDescription)) {
                return displayName;
            }

            return $"{displayName}\n- {weaponDescription}";
        }

        string displayNameForItem = displayNameOverride ?? GetCanonicalItemDisplayName(itemName, modifier);
        switch (itemName) {
            case "forge":
                return $"forge\n+1 {modifier}";
            case "charm": {
                return modifier switch {
                    "unbroken" => $"{displayNameForItem}\nparry to gain +1 parry",
                    "relentless" => $"{displayNameForItem}\nwound to gain +1 attack",
                    "aether" => $"{displayNameForItem}\nattack first to gain +1 speed",
                    "ruthless" => $"{displayNameForItem}\nattack neck for +1 accuracy",
                    "riposte" => $"{displayNameForItem}\nparry to gain +1 attack",
                    "bulwark" => $"{displayNameForItem}\nattack second to gain +1 parry",
                    "vindictive" => $"{displayNameForItem}\ngain +2 attack when wounded",
                    "inevitable" => $"{displayNameForItem}\nattack second to gain +1 attack",
                    _ => descriptionDict.TryGetValue(displayNameForItem, out string charmDescription) && !string.IsNullOrEmpty(charmDescription)
                        ? $"{displayNameForItem}\n{charmDescription}"
                        : displayNameForItem,
                };
            }
            case "cheese":
            case "steak": {
                int staminaRestored = useDynamicValues
                    ? GetFoodStaminaAmount(itemName)
                    : int.Parse(descriptionDict[itemName]);
                return $"{displayNameForItem}\n+{staminaRestored} stamina";
            }
            case "sacrificial_chalice":
                return useDynamicValues ? GetSacrificialChaliceDescription(modifier) : "sacrificial chalice\nit thirsts...  +0";
            case "moldy_cheese":
            case "rotten_steak":
                return $"{displayNameForItem}\n+{GetFoodStaminaAmount(itemName, includeCharacterBonus:false)} stamina";
            case "holy_water": {
                int staminaRestored = useDynamicValues ? GetHolyWaterStaminaAmount() : 10;
                int tradeValue = 2;
                return $"holy water\n+{staminaRestored} stamina\nor trade for {tradeValue} items";
            }
            case "arrow":
                return useDynamicValues
                    && s?.levelManager != null
                    && s.levelManager.level == 4
                    && s.levelManager.sub == 1
                    && (Save.persistent == null || !Save.persistent.endlessModeEnabled)
                    ? "leave dungeon"
                    : descriptionDict[itemName];
            case "retry":
                return descriptionDict[itemName];
            case "amulet_of_resurrection":
            case "broken_amulet":
                return displayNameForItem;
        }

        if (!descriptionDict.TryGetValue(displayNameForItem, out string description) || string.IsNullOrEmpty(description)) {
            return displayNameForItem;
        }

        return $"{displayNameForItem}\n{description}";
    }

    /// <summary>
    /// Give the player their starting items, based on chosen class.
    /// </summary>
    public void GiveStarterItems() {
        if (Save.game.newGame) {
            ClearLuckyDiceRoundStats();
            bool isNightmare = DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty);
            bool isEasy = DifficultyHelper.IsEasy(Save.persistent.gameDifficulty);
            switch (Save.game.curCharNum) {
                // new game, so give the base weapons
                case 0: {
                    if (isNightmare) {
                        CreateWeaponWithStats("sword", "rusty", 2, 1, 1, 2);
                    }
                    else { 
                        CreateWeaponWithStats("sword", "harsh", 3, 3, 2, 3);
                    }
                    // CreateWeaponWithStats("maul", "administrative", 10, 10, 10, 10);
                    MoveToInventory(0, true, false, false);
                    CreateItem(isNightmare ? GetNightmareStarterTradeItemName(0) : "steak");
                    MoveToInventory(0, true, false, false);
                    // CreateItem("scroll", "leech");
                    // MoveToInventory(0, true, false, false);
                    // CreateItem("bloodletters");
                    // MoveToInventory(0, true, false, false);
                    // CreateItem("rabadons");
                    // MoveToInventory(0, true, false, false);
                    // CreateItem("potion", "force");
                    // MoveToInventory(0, true, false, false);
                    // CreateItem("potion", "rage");
                    // MoveToInventory(0, true, false, false);
                    // CreateItem("potion", "strength");
                    // MoveToInventory(0, true, false, false);
                    // CreateItem("potion", "alacrity");
                    // MoveToInventory(0, true, false, false);
                    if (isEasy) {
                        CreateItem("torch");
                        MoveToInventory(0, true, false, false);
                    }
                    break;
                }
                case 1: {
                    if (isNightmare) {
                        CreateWeaponWithStats("maul", "rusty", -1, -1, 1, -1);
                    }
                    else { 
                        CreateWeaponWithStats("maul", "common", -1, -1, 4, 1);
                    }
                    MoveToInventory(0, true, false, false);
                    CreateItem(isNightmare ? GetNightmareStarterTradeItemName(1) : "armor");
                    MoveToInventory(0, true, false, false);
                    if (isEasy) {
                        CreateItem("helm_of_might");
                        MoveToInventory(0, true, false, false);
                    }
                    break;
                }
                case 2: {
                    if (isNightmare) {
                        CreateWeaponWithStats("dagger", "rusty", 2, 3, 0, 0);
                    }
                    else { 
                        CreateWeaponWithStats("dagger", "common", 3, 6, 0, 0);
                    }
                    MoveToInventory(0, true, false, false);
                    CreateItem(isNightmare ? GetNightmareStarterTradeItemName(2) : "boots_of_dodge");
                    MoveToInventory(0, true, false, false);
                    if (isEasy) {
                        CreateItem("ankh");
                        MoveToInventory(0, true, false, false);
                    }
                    break;
                }
                case 3: {
                    if (isNightmare) {
                        CreateWeaponWithStats("mace", "rusty", 2, 2, 1, 0);
                    }
                    else { 
                        CreateWeaponWithStats("mace", "ruthless", 3, 3, 2, 1);
                    }
                    MoveToInventory(0, true, false, false);
                    CreateItem(isNightmare ? GetNightmareStarterTradeItemName(3) : "cheese");
                    MoveToInventory(0, true, false, false);
                    if (isEasy) {
                        CreateItem("kapala");
                        MoveToInventory(0, true, false, false);
                    }
                    break;
                }
            }
        }
        else { 
            // continuing previously existing game
            RestoreSavedInventoryDirectly();
            LoadLuckyDiceRoundStatsFromSave();
            RestoreCharmStateFromSave();
            RefreshPassiveInventoryEffects();
            SyncCharmStateToSave();
        }
        Select(curList, 0, playAudio:false);
        // select the first item
        SaveInventoryItems();
    }

    /// <summary>
    /// Select an item from the given list at the given index. 
    /// </summary>
    public void Select(List<GameObject> itemList, int c, bool forceDifferentSelection=true, bool playAudio=true) {
        if (c <= itemList.Count - 1 && c >= 0) {
            // if the column is within the bounds of the list
            itemList[c].GetComponent<Item>().Select(playAudio);
            // select the object
            curList = itemList;
            col = c;
            // update the variables used for selection 
            return;
        }

        // column is invalid for the list
        if (!forceDifferentSelection) {
            highlightedItem.GetComponent<Item>().Select();
            // not forcing a different selection, so select the item regardless
            return;
        }

        if (itemList.Count > 1) {
            SelectFallbackItem(itemList);
            return;
        }

        SelectDefaultSingleItem();
        // select the weapon and update the variables used for selection.
    }

    /// <summary>
    /// Get the sprite of an item given its name.
    /// </summary>
    public Sprite GetItemSprite(string itemName) {
        string normalizedItemName = NormalizeItemSpriteName(itemName);
        if (!string.IsNullOrEmpty(normalizedItemName) && spriteLookup.TryGetValue(normalizedItemName, out Sprite sprite)) {
            return sprite;
        }

        Debug.LogWarning($"Missing sprite for item '{itemName}', using fallback");
        if (spriteLookup.TryGetValue("retry", out Sprite retrySprite)) { return retrySprite; }

        foreach (Sprite fallbackSprite in allSprites) {
            if (fallbackSprite != null) { return fallbackSprite; }
        }

        return null;
    }

    private Sprite GetRandomCharmSprite() {
        string[] charmSpriteNames = { "charm0", "charm1", "charm2", "charm3", "charm4", "charm5" };
        return GetItemSprite(charmSpriteNames[Random.Range(0, charmSpriteNames.Length)]);
    }

    private Item CreateItemInstance(Vector2 position, string itemName, string itemType, Sprite sprite, string modifier = "", Dictionary<string, int> weaponStats = null, System.Action<Item> postProcess = null) {
        GameObject instantiatedItem = Instantiate(item, position, Quaternion.identity);
        instantiatedItem.transform.parent = gameObject.transform;

        SpriteRenderer spriteRenderer = instantiatedItem.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.sprite = sprite;
        }

        Item itemScript = instantiatedItem.GetComponent<Item>();
        itemScript.itemName = itemType == "weapon" ? itemName : NormalizeCommonItemKey(itemName);
        itemScript.itemType = itemType;
        itemScript.modifier = string.IsNullOrEmpty(modifier) ? "" : modifier;
        if (itemScript.itemName == "sacrificial_chalice" && string.IsNullOrWhiteSpace(itemScript.modifier)) {
            itemScript.modifier = "0";
        }
        itemScript.weaponStats = weaponStats != null
            ? new Dictionary<string, int>(weaponStats)
            : new Dictionary<string, int>();
        itemScript.gameObject.name = itemName;
        postProcess?.Invoke(itemScript);
        if (spriteRenderer != null && itemScript.itemName == "charm") {
            spriteRenderer.sprite = GetRandomCharmSprite();
        }
        return itemScript;
    }

    private GameObject CreateFloorItem(string itemName, string itemType, Sprite sprite, string modifier = "", int negativeOffset = 0, Dictionary<string, int> weaponStats = null, System.Action<Item> postProcess = null) {
        Vector2 position = new(itemX + (floorItems.Count - negativeOffset) * itemSpacing, itemY);
        Item itemScript = CreateItemInstance(position, itemName, itemType, sprite, modifier, weaponStats, postProcess);
        floorItems.Add(itemScript.gameObject);
        return itemScript.gameObject;
    }

    private GameObject CreateInventoryItem(string itemName, string itemType, Sprite sprite, string modifier = "", Dictionary<string, int> weaponStats = null) {
        Vector2 position = new(itemX + itemSpacing * s.player.inventory.Count, 3.16f);
        Item itemScript = CreateItemInstance(position, itemName, itemType, sprite, modifier, weaponStats);
        s.player.inventory.Add(itemScript.gameObject);
        InvalidateInventoryCache();
        return itemScript.gameObject;
    }

    /// <summary>
    /// Create an item with specified type.
    /// </summary>
    private void CreateRandomItem(int negativeOffset = 0) {
        string itemKey = itemDropTable[Random.Range(0, itemDropTable.Count)];
        string createdItemName = GetCanonicalCreatedItemName(itemKey);
        CreateFloorItem(createdItemName, "common", GetItemSprite(itemKey), negativeOffset:negativeOffset, postProcess:SetItemStatsImmediately);
    }

    /// <summary>
    /// Create an item with the specified name and type.
    /// </summary>
    public GameObject CreateItem(string itemName, int negativeOffset=0) {
        string modifier = "";
        NormalizeLegacyCommonItem(ref itemName, ref modifier);
        string createdItemName = GetCanonicalCreatedItemName(itemName);
        return CreateFloorItem(createdItemName, "common", GetItemSprite(itemName), negativeOffset:negativeOffset, postProcess:SetItemStatsImmediately);
    }

    /// <summary>
    /// Create an item with the specified name, type, and modifier.
    /// </summary>
    public GameObject CreateItem(string itemName, string modifier, int negativeOffset=0) {
        NormalizeLegacyCommonItem(ref itemName, ref modifier);
        string createdItemName = GetCanonicalCreatedItemName(itemName);
        return CreateFloorItem(createdItemName, "common", GetItemSprite(itemName), modifier, negativeOffset);
    }

    // ^ i love overloading functions!

    /// <summary>
    /// Instantly assign necessary attributes of items (like their modifier).
    /// </summary>
    private void SetItemStatsImmediately(Item itemScript) {
        // this needs to be done here rather than in Item.Start() or Awake() because the timing will be off and errors will be thrown
        if (itemScript.itemName == "necklet") {
            int rand = Random.Range(0, 5);
            itemScript.modifier = neckletTypes[rand];
        }
        else if (itemScript.itemName == "scroll") {
            itemScript.modifier = scrollTypes[Random.Range(0, scrollTypes.Length)];
        }
        else if (itemScript.itemName == "potion") {
            itemScript.modifier = potionTypes[Random.Range(0, potionTypes.Length)];
        }
        else if (itemScript.itemName == "charm") {
            itemScript.modifier = charmTypes[Random.Range(0, charmTypes.Length)];
        }
        else if (itemScript.itemName == "tarot") {
            itemScript.modifier = tarotTypes[Random.Range(0, tarotTypes.Length)];
        }
        else if (itemScript.itemName == "gem") {
            itemScript.modifier = gemTypes[Random.Range(0, gemTypes.Length)];
        }
        // assign a modifier for a necklet, scroll, potion, charm, or tarot
    }

    private void ApplyNightmareRandomWeaponPenalty(Dictionary<string, int> rolledStats) {
        if (!DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) { return; }

        List<string> nonNegativeStats = statArr
            .Where(stat => rolledStats.TryGetValue(stat, out int value) && value >= 0)
            .ToList();
        if (nonNegativeStats.Count == 0) { return; }

        string guaranteedPenaltyStat = nonNegativeStats[Random.Range(0, nonNegativeStats.Count)];
        rolledStats[guaranteedPenaltyStat] = Mathf.Max(-1, rolledStats[guaranteedPenaltyStat] - 1);

        foreach (string stat in statArr) {
            if (stat == guaranteedPenaltyStat) { continue; }

            rolledStats[stat] = Mathf.Max(-1, rolledStats[stat] - Random.Range(0, 2));
        }
    }

    private void EnsureDropTablesBuilt() {
        if (itemDropTable == null) {
            itemDropTable = new List<string>();
        }
        if (modifierDropTable == null) {
            modifierDropTable = new List<string>();
        }

        if (itemDropTable.Count == 0) {
            foreach (KeyValuePair<string, int> entry in itemDropDict) {
                for (int i = 0; i < entry.Value; i++) {
                    itemDropTable.Add(entry.Key);
                }
            }
        }

        if (modifierDropTable.Count == 0) {
            foreach (KeyValuePair<string, int> entry in modifierDropDict) {
                for (int i = 0; i < entry.Value; i++) {
                    modifierDropTable.Add(entry.Key);
                }
            }
        }
    }

    public string DebugRollRandomWeaponName() {
        EnsureDropTablesBuilt();
        WeaponRollData weaponRoll = RollRandomWeaponData();
        return BuildWeaponFullName(weaponRoll.BaseName, weaponRoll.Modifier);
    }

    public string DebugRollRandomItemName() {
        EnsureDropTablesBuilt();

        string itemKey = itemDropTable[Random.Range(0, itemDropTable.Count)];
        string itemName = GetCanonicalCreatedItemName(itemKey);
        string modifier = "";

        switch (itemName) {
            case "necklet":
                modifier = neckletTypes[Random.Range(0, 5)];
                break;
            case "scroll":
                modifier = scrollTypes[Random.Range(0, scrollTypes.Length)];
                break;
            case "potion":
                modifier = potionTypes[Random.Range(0, potionTypes.Length)];
                break;
            case "charm":
                modifier = charmTypes[Random.Range(0, charmTypes.Length)];
                break;
            case "tarot":
                modifier = tarotTypes[Random.Range(0, tarotTypes.Length)];
                break;
        }

        return string.IsNullOrEmpty(modifier)
            ? itemName
            : GetCanonicalItemDisplayName(itemName, modifier);
    }

    private WeaponRollData RollRandomWeaponData(string forcedWeaponName = null) {
        string weaponBaseName = NormalizeWeaponSaveName(forcedWeaponName);
        if (string.IsNullOrEmpty(weaponBaseName) || !weaponSpriteByBaseName.ContainsKey(weaponBaseName)) {
            weaponBaseName = weaponBaseNames[Random.Range(0, weaponBaseNames.Count)];
        }

        string modifierRoll = modifierDropTable[Random.Range(0, modifierDropTable.Count)];
        string modifier = modifierRoll.Substring(0, modifierRoll.Length - 1);
        Dictionary<string, int> rolledStats = new() {
            { "green", 0 }, { "blue", 0 }, { "red", 0 }, { "white", 0 }
        };

        foreach (string key in statArr) {
            int value = weaponStatDict[weaponBaseName][key];
            if (modifierRoll == "legendary0") {
                value = legendaryStatDict[weaponBaseName][key];
            }
            else {
                value += modifierStatDict[modifierRoll][key];
            }
            rolledStats[key] = Mathf.Max(-1, value);
        }

        ApplyNightmareRandomWeaponPenalty(rolledStats);

        return new WeaponRollData {
            BaseName = weaponBaseName,
            Modifier = modifier,
            Stats = rolledStats,
            Sprite = GetWeaponSprite(weaponBaseName)
        };
    }

    /// <summary>
    /// Create a weapon with randomized modifier and stats. Use to generate a weapon when the player slays the enemy.
    /// </summary>
    private GameObject CreateRandomWeapon(int negativeOffset = 0, string forcedWeaponName = null) {
        WeaponRollData weaponRoll = RollRandomWeaponData(forcedWeaponName);
        GameObject instantiatedItem = CreateFloorItem(
            BuildWeaponFullName(weaponRoll.BaseName, weaponRoll.Modifier),
            "weapon",
            weaponRoll.Sprite,
            weaponRoll.Modifier,
            negativeOffset,
            weaponRoll.Stats
        );
        if (s.tutorial == null) { Save.SaveGame(); }
        return instantiatedItem;
    }

    /// <summary>
    /// Create a weapon with specified name, modifier, and stats.
    /// </summary>
    public GameObject CreateWeaponWithStats(string weaponName, string modifier, int aim, int spd, int atk, int def) {
        weaponName = NormalizeWeaponSaveName(weaponName);
        Sprite sprite = weaponName == "glass sword" && modifier == "shattered"
            ? GetItemSprite("glass_sword_shattered")
            : GetWeaponSprite(weaponName);
        Dictionary<string, int> baseWeapon = new() {
            { "green", aim >= 0 ? aim : -1 },
            { "blue", spd >= 0 ? spd : -1 },
            { "red", atk >= 0 ? atk : -1 },
            { "white", def >= 0 ? def : -1 }
        };
        return CreateFloorItem(BuildWeaponFullName(weaponName, modifier), "weapon", sprite, modifier, 0, baseWeapon);
    }

    public GameObject CreateSmithUpgradeArrow(string stat, int negativeOffset = 0) {
        return CreateFloorItem("forge", "common", GetItemSprite("forge"), stat, negativeOffset);
    }

    public GameObject CreateSavedFloorItem(string itemName, string itemType, string modifier, int aim, int spd, int atk, int def) {
        if (itemName == null || itemName == "") { return null; }

        if (itemType == "weapon") {
            return CreateWeaponWithStats(NormalizeWeaponSaveName(itemName), modifier, aim, spd, atk, def);
        }

        if (itemName == "forge") {
            return CreateSmithUpgradeArrow(modifier);
        }

        return CreateItem(itemName, modifier);
    }

    public void RemoveFloorItemAt(int index, bool saveData = true) {
        Destroy(floorItems[index]);
        floorItems.RemoveAt(index);
        for (int i = index; i < floorItems.Count; i++) {
            floorItems[i].transform.position = new Vector2(itemX + i * itemSpacing, itemY);
        }
        if (floorItems.Count > 0) { Select(floorItems, Mathf.Clamp(index, 0, floorItems.Count - 1), playAudio:false); }
        else { Select(s.player.inventory, 0, playAudio:false); }
        if (saveData) { SaveFloorItems(); }
    }

    public void ForgePlayerWeapon(string statName) {
        Item weapon = s.player.inventory[0].GetComponent<Item>();
        string statColor = statName switch {
            "accuracy" => "green",
            "speed" => "blue",
            "damage" => "red",
            _ => "white"
        };

        weapon.weaponStats[statColor]++;
        if (weapon.modifier != "legendary") {
            weapon.modifier = "forged";
            weapon.itemName = BuildWeaponFullName(GetWeaponBaseName(weapon.itemName), weapon.modifier);
            weapon.gameObject.name = weapon.itemName;
        }

        InvalidateInventoryCache();
        s.player.stats = weapon.weaponStats;
        Save.game.resumeAcc = s.player.stats["green"];
        Save.game.resumeSpd = s.player.stats["blue"];
        Save.game.resumeDmg = s.player.stats["red"];
        Save.game.resumeDef = s.player.stats["white"];
        Save.game.glassSwordShattered = false;
        // forge repairs/upgrades the sword — reset shatter state
        s.statSummoner.SummonStats();
        s.statSummoner.SetCombatDebugInformationFor("player");
        SaveInventoryItems();
    }

    /// <summary>
    /// Move the item at index 0 from the floor to the display in the character selection screen.
    /// </summary>
    private void MoveItemToDisplay() {
        if (!isCharSelect) { return; }
        // print("This function should only be used in character select!"); 

        for (int i = 0; i < floorItems.Count; i++) { 
            // for every item on the floor
            floorItems[i].transform.position = new Vector2(-4.572f + itemSpacing * i, 6.612f);
            // change its position to be in the display area
        }
    }

    /// <summary>
    /// Move the floor item at the specified index into the player's inventory.
    /// </summary>
    public void MoveToInventory(int index, string actionVerbOverride) {
        MoveToInventory(index, starter:false, playAudio:true, saveData:true, actionVerbOverride:actionVerbOverride);
    }

    public void MoveToInventory(int index, bool starter=false, bool playAudio=true, bool saveData=true) {
        MoveToInventory(index, starter, playAudio, saveData, null);
    }

    public void MoveToInventory(int index, bool starter, bool playAudio, bool saveData, string actionVerbOverride) {
        if (floorItems[index] == null) {
            Destroy(floorItems[index]);
            floorItems.RemoveAt(index);
            // something went wrong here, so destroy it
            if (saveData) {
                SaveInventoryItems();
                SaveFloorItems();
            }
            return;
        }

        Item floorItem = floorItems[index].GetComponent<Item>();
        if (s.player.inventory.Count >= 9 && floorItem.itemType != "weapon") {
            s.turnManager.SetStatusText("you can't carry any more");
            if (saveData) {
                SaveInventoryItems();
                SaveFloorItems();
            }
            return;
        }

        // selection after pickup already plays the cursor sound once
        if (floorItem.itemType == "weapon") {
            MoveWeaponToInventory(index, starter);
        }
        else {
            MoveCommonItemToInventory(index, starter, playAudio, actionVerbOverride);
        }

        if (saveData) { 
            SaveInventoryItems();
            if (s.levelManager.level == Save.persistent.tsLevel && s.levelManager.sub == Save.persistent.tsSub) { SaveFloorItems(); }
            else { SaveFloorItems(); }
            // only Save items if necessary
        }
    }

    /// <summary>
    /// A coroutine to set the debug and summon stats after a delay (0.1s).
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateUIAfterDelay() {
        yield return s.delays[0.1f];
        s.statSummoner.SetCombatDebugInformationFor("player");
        s.statSummoner.SummonStats();
    }

    private void SelectFallbackItem(List<GameObject> itemList) {
        try {
            // if there is more than 1 item
            itemList[col - 1].GetComponent<Item>().Select();
            // select the next item over
            curList = itemList;
            col--;
            // update the variables used for selection
        }
        catch {
            itemList[0].GetComponent<Item>().Select();
            // select item 0
            curList = itemList;
            col = 0;
            // update variables
        }
    }

    private void SelectDefaultSingleItem() {
        if (s.player != null) {
            s.player.inventory[0].GetComponent<Item>().Select();
            curList = s.player.inventory;
        }
        else {
            floorItems[0].GetComponent<Item>().Select();
            curList = floorItems;
        }

        col = 0;
    }

    private void MoveWeaponToInventory(int index, bool starter) {
        // if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty) && !starter) {
        //     s.turnManager.SetStatusText($"you may only wield {GetEquippedWeapon()?.itemName}");
        //     // prevent player from swapping weapons if they are in hard mode
        //     return;
        // }

        if (!starter) { Save.persistent.weaponsSwapped++; }
        if (!starter) { Save.persistent.itemsFound++; }
        MarkItemDiscovered(floorItems[index].GetComponent<Item>());
        // if the item being moved is a weapon 
        GameObject pickedWeapon = floorItems[index];
        pickedWeapon.transform.position = new Vector2(itemX, 3.16f);
        // move the item to the weapon slot
        s.player.stats = pickedWeapon.GetComponent<Item>().weaponStats;
        // set the player's stats to be equal to that of the weapon
        Save.game.resumeAcc = s.player.stats["green"];
        Save.game.resumeSpd = s.player.stats["blue"];
        Save.game.resumeDmg = s.player.stats["red"];
        Save.game.resumeDef = s.player.stats["white"];
        Save.game.glassSwordShattered = false;
        // new weapon — clear any prior glass sword shatter state

        if (starter) {
            s.player.inventory.Add(pickedWeapon);
            // item is a starter, so just add it to the player's inventory
            floorItems.RemoveAt(index);
            foreach(GameObject curItem in floorItems) {
                curItem.transform.position = new Vector2(curItem.transform.position.x - 1f, itemY);
                // for every item, shift it over now that an item has been removed
            }
            InvalidateInventoryCache();
            RefreshPassiveInventoryEffects();
            Select(s.player.inventory, 0);
            return;
        }

        if (IsBlacksmithEncounter()) {
            s.turnManager.SetStatusText("you exchange weapons");
        }
        else {
            string pickupText = "you take " + GetWeaponBaseName(pickedWeapon.GetComponent<Item>().itemName);
            s.turnManager.SetStatusText(pickupText);
        }
        // notify the player
        GameObject oldWeapon = s.player.inventory[0];
        oldWeapon.transform.position = new Vector2(itemX + index * itemSpacing, itemY);
        oldWeapon.transform.rotation = Quaternion.identity;
        s.player.inventory[0] = pickedWeapon;
        floorItems[index] = oldWeapon;
        // add the new weapon to the player's inventory
        InvalidateInventoryCache();
        RefreshPassiveInventoryEffects();
        // update debug, because player just took a new weapon
        s.turnManager.RefreshBlackBoxVisibility();
        SelectPostWeaponSwap(index);
    }

    private void SelectPostWeaponSwap(int previousFloorIndex) {
        if (IsBlacksmithEncounter()) {
            Select(s.player.inventory, 0);
            return;
        }

        int nextFloorIndex = Mathf.Clamp(previousFloorIndex + 1, 0, floorItems.Count - 1);
        Select(floorItems, nextFloorIndex);
    }

    private void MoveCommonItemToInventory(int index, bool starter, bool playAudio, string actionVerbOverride = null) {
        Item tempItem = floorItems[index].GetComponent<Item>();
        SetPickupStatusText(tempItem, starter, actionVerbOverride);

        floorItems[index].transform.position = new Vector2(itemX + itemSpacing * s.player.inventory.Count, 3.16f);
        // add the item to the proper location
        if (!starter) { Save.persistent.itemsFound++; }
        MarkItemDiscovered(floorItems[index].GetComponent<Item>());
        s.player.inventory.Add(floorItems[index]);
        // add the item to the player's inventory
        floorItems.RemoveAt(index);
        // and remove it from the floor
        InvalidateInventoryCache();
        RefreshPassiveInventoryEffects();
        ApplyCornucopiaPickupStamina(starter, tempItem);
        for (int i = index; i < floorItems.Count; i++) {
            // for every item after where the removed item was
            floorItems[i].transform.position = new Vector2(floorItems[i].transform.position.x - 1f, itemY);
            // shift it over to the proper location
        }
        if (playAudio) { Select(curList, index); }
        else { Select(curList, index, playAudio:false); }
        // attempt to select the next item of where it was
    }

    private IEnumerable<string> GetRandomForgeStats(int forgeSlotCount) {
        List<string> availableForgeStats = forgeStats.ToList();
        for (int i = 0; i < forgeSlotCount && availableForgeStats.Count > 0; i++) {
            int rand = Random.Range(0, availableForgeStats.Count);
            yield return availableForgeStats[rand];
            availableForgeStats.RemoveAt(rand);
        }
    }

    private string GetWeightedTraderHealingItemName() {
        int roll = Random.Range(0, 10);
        if (roll < 5) { return "tincture"; }
        if (roll < 7) { return "scroll"; }
        if (roll < 9) { return "campfire"; }
        return "potion";
    }

    private GameObject CreateGuaranteedTraderHealingItem(int negativeOffset = 0) {
        string itemName = GetWeightedTraderHealingItemName();
        return itemName switch {
            "scroll" => CreateItem("scroll", "leech", negativeOffset),
            "potion" => CreateItem("potion", "life", negativeOffset),
            _ => CreateItem(itemName, negativeOffset),
        };
    }

    private void RemoveTraderNothingModifier(Item traderItem) {
        if (traderItem == null || traderItem.modifier != "nothing") { return; }

        if (traderItem.itemName == "scroll") {
            traderItem.modifier = scrollTypes[Random.Range(0, scrollTypes.Length - 1)];
        }
        else if (traderItem.itemName == "potion") {
            traderItem.modifier = potionTypes[Random.Range(0, potionTypes.Length - 1)];
        }
    }

    private GameObject CreateTraderRandomItem(int negativeOffset = 0) {
        GameObject createdItem = CreateRandomItemForTrader(negativeOffset);
        RemoveTraderNothingModifier(createdItem.GetComponent<Item>());
        return createdItem;
    }

    private GameObject CreateRandomItemForTrader(int negativeOffset = 0) {
        string itemKey = itemDropTable[Random.Range(0, itemDropTable.Count)];
        string createdItemName = GetCanonicalCreatedItemName(itemKey);
        return CreateFloorItem(createdItemName, "common", GetItemSprite(itemKey), negativeOffset:negativeOffset, postProcess:SetItemStatsImmediately);
    }

    private void SetPickupStatusText(Item tempItem, bool starter, string actionVerbOverride = null) {
        if (starter) { return; }

        string actionText = GetActionTextForItem(tempItem, string.IsNullOrEmpty(actionVerbOverride) ? "you take" : actionVerbOverride);
        int cornucopiaBonus = GetCornucopiaPickupStaminaBonus(starter, tempItem);
        s.turnManager.SetStatusText(cornucopiaBonus > 0 ? $"{actionText} and gain +{cornucopiaBonus} stamina" : actionText);

        if (s.tutorial != null) { s.tutorial.Increment(); }
    }

    private int GetCornucopiaPickupStaminaBonus(bool starter, Item pickedItem = null) {
        if (starter || pickedItem?.itemName == "arrow") { return 0; }

        int cornucopiaCount = GetPlayerItemCount("cornucopia");
        if (pickedItem != null && pickedItem.itemName == "cornucopia") {
            cornucopiaCount++;
        }

        return cornucopiaCount;
    }

    private void ApplyCornucopiaPickupStamina(bool starter, Item pickedItem = null) {
        int staminaBonus = GetCornucopiaPickupStaminaBonus(starter, pickedItem);
        if (staminaBonus <= 0 || s?.player == null) { return; }

        s.player.stamina += staminaBonus;
    }

    /// <summary>
    /// Get the base common-item drop count for the current combat floor.
    /// </summary>
    private int GetBaseDropCountForCurrentStage() {
        return $"{s.levelManager.level}-{s.levelManager.sub}" switch {
            // 1-1: 1 item guaranteed
            "1-1" => 1,
            // 1-2: 75% 1, 25% 2
            "1-2" => Random.Range(0, 4) == 0 ? 2 : 1,
            // 1-3: 66% 1, 33% 2
            "1-3" => Random.Range(0, 3) == 0 ? 2 : 1,
            // 2-1: 50% 1, 50% 2
            "2-1" => Random.Range(0, 2) == 0 ? 1 : 2,
            // 2-2: 37.5% 1, 50% 2, 12.5% 3
            "2-2" => GetTwoTwoDropCount(),
            // 2-3: 75% 2, 20% 3, 5% 4
            "2-3" => GetTwoThreeDropCount(),
            // 3-1: 50% 2, 45% 3, 5% 4
            "3-1" => GetThreeOneDropCount(),
            // 3-2: 25% 2, 62.5% 3, 12.5% 4
            "3-2" => GetThreeTwoDropCount(),
            // 3-3: 25% 2, 50% 3, 25% 4
            "3-3" => GetThreeThreeDropCount(),
            _ => GetThreeThreeDropCount()+1,
        };
    }

    /// <summary>
    /// Each torch adds a separate 0 or 1 item roll.
    /// </summary>
    private int GetTorchBonusDropCount(int torchCount) {
        int bonusCount = 0;
        for (int i = 0; i < torchCount; i++) {
            bonusCount += Random.Range(0, 3);
        }
        return bonusCount;
    }

    private int GetBloodlettersCurseEffectiveTorchCount() {
        int curseCount = GetInventoryItemCount("bloodletters_curse");
        if (curseCount <= 0 || s == null || s.player == null || s.player.woundList == null) { return 0; }

        int torchEquivalent = s.player.woundList.Count switch {
            <= 0 => 0,
            1 => 2,
            _ => 3,
        };
        return curseCount * torchEquivalent;
    }

    // 2-2: 37.5% 1, 50% 2, 12.5% 3
    private int GetTwoTwoDropCount() {
        int roll = Random.Range(0, 8);
        if (roll == 7) { return 3; }
        return roll < 3 ? 1 : 2;
    }

    // 2-3: 75% 2, 20% 3, 5% 4
    private int GetTwoThreeDropCount() {
        int roll = Random.Range(0, 20);
        if (roll == 19) { return 4; }
        return roll < 15 ? 2 : 3;
    }

    // 3-1: 50% 2, 45% 3, 5% 4
    private int GetThreeOneDropCount() {
        int roll = Random.Range(0, 20);
        if (roll == 19) { return 4; }
        return roll < 10 ? 2 : 3;
    }

    // 3-2: 25% 2, 62.5% 3, 12.5% 4
    private int GetThreeTwoDropCount() {
        int roll = Random.Range(0, 8);
        if (roll == 7) { return 4; }
        return roll < 2 ? 2 : 3;
    }

    // 3-3: 25% 2, 50% 3, 25% 4
    private int GetThreeThreeDropCount() {
        int roll = Random.Range(0, 4);
        if (roll == 0) { return 2; }
        if (roll == 3) { return 4; }
        return 3;
    }

    /// <summary>
    /// Spawn the items after the enemy has died. 
    /// </summary>
    public void SpawnItems() {
        lootText.text = "loot:";
        // set the text 
        if (s.tutorial == null) { 
            if (s.enemy.enemyName.text == "Lich") {
                CreateRandomWeapon();
                CreateItem("phylactery");
                CreateItem("tincture");
                // defeated lich, so give phylactery & tincture
            }
            else if (s.enemy.enemyName.text == "Devil") {
                CreateItem("necklet", "victory");
                // defeated devil, so give necklet of victory
            }
            else {
                // normal enemy
                int torchCount = GetInventoryItemCount("torch");
                torchCount += GetBloodlettersCurseEffectiveTorchCount();
                if (PlayerHasWeapon("sword") && PlayerHasLegendary()) { torchCount++; }
                // count the number of torches, legendary sword helps find loot
                int spawnCount = 1 + GetBaseDropCountForCurrentStage() + GetTorchBonusDropCount(torchCount);
                spawnCount = Mathf.Clamp(spawnCount, 2, 6);
                if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) {
                    // 50% chance to deduct a random number of items
                    if (Random.Range(0, 2) == 0) {
                        spawnCount -= Random.Range(1, s.levelManager.level);
                    }
                    spawnCount = Mathf.Clamp(spawnCount, 2, 5);
                }
                CreateRandomWeapon();
                // create a random weapon at index 0
                for (int i = 0; i < spawnCount; i++) {
                // for (int i = 0; i < 6; i++) {
                    CreateRandomItem();
                    // create a random number of items based on the spawn count
                }
            }
        }
        else { 
            CreateItem("steak");
            CreateWeaponWithStats("sword", "lame", 1, 1, 1, 1);
        }
        CreateItem("arrow");
        // create an arrow to move to the next level
        SaveFloorItems();
    }

    /// <summary>
    /// Spawn the items for which the player can trade with.
    /// </summary>
    public void SpawnMerchantItems() {
        int tempOffset = deletionQueue.Count;
        // get the count now so we can spawn items without fear of it changing
        lootText.text = "goods:";
        // set the test
        CreateGuaranteedTraderHealingItem(tempOffset);
        // 5 for easy, 4 normal, 3 for hard & nightmare
        int randomItemCount = DifficultyHelper.IsEasy(Save.persistent.gameDifficulty) ? 5 : DifficultyHelper.IsNormal(Save.persistent.gameDifficulty) ? 4 : 3;
        for (int i = 0; i < randomItemCount; i++) { CreateTraderRandomItem(tempOffset); }
        CreateItem("arrow", tempOffset);
        // create the next level arrow
        Save.game.numItemsDroppedForTrade = GetVendorTakeAllowance();
        Save.game.merchantStealAllowanceRemaining = GetVendorTakeAllowance();
        SaveFloorItems();
    }

    public void SpawnBlacksmithItems(bool delayForPlayerSetup = false) {
        if (delayForPlayerSetup) {
            StartCoroutine(SpawnBlacksmithItemsAfterPlayerSetup());
            return;
        }

        SpawnBlacksmithItemsImmediate();
    }

    private IEnumerator SpawnBlacksmithItemsAfterPlayerSetup() {
        yield return null;

        while (s == null || s.player == null || s.player.inventory.Count == 0) {
            yield return null;
        }

        yield return s.delays[0.05f];
        SpawnBlacksmithItemsImmediate();
    }

    private void SpawnBlacksmithItemsImmediate() {
        lootText.text = "goods:";
        if (s != null && s.turnManager != null) {
            s.turnManager.RefreshBlackBoxVisibility();
        }
        List<string> availableWeapons = new(weaponBaseNames);
        for (int i = 0; i < 2; i++) {
            int rand = Random.Range(0, availableWeapons.Count);
            CreateRandomWeapon(forcedWeaponName: availableWeapons[rand]);
            availableWeapons.RemoveAt(rand);
        }
        int forgeSlotCount = 0;
        // forge slot count: easy/normal: 75% 3, 25%4. hard: 50% 2, 50% 3. nightmare: 75% 2, 25% 3.
        if (DifficultyHelper.IsEasy(Save.persistent.gameDifficulty) || DifficultyHelper.IsNormal(Save.persistent.gameDifficulty)) {
            forgeSlotCount = Random.Range(0, 4) == 0 ? 4 : 3;
        }
        else if (DifficultyHelper.IsHard(Save.persistent.gameDifficulty)) {
            forgeSlotCount = Random.Range(0, 2) + 2;
        }
        else if (DifficultyHelper.IsNightmare(Save.persistent.gameDifficulty)) {
            forgeSlotCount = Random.Range(0, 4) == 0 ? 3 : 2;
        }
        foreach (string stat in GetRandomForgeStats(forgeSlotCount)) {
            CreateSmithUpgradeArrow(stat);
        }
        CreateItem("arrow");
        Save.game.numItemsDroppedForTrade = GetVendorTakeAllowance();
        Save.game.merchantStealAllowanceRemaining = 0;
        SaveFloorItems();
        UpdateVendorUIForSelection();
    }
    
    /// <summary>
    /// Destroy floor items when going on to the next level or restarting.
    /// </summary>
    public void DestroyItems() {
        lootText.text = "";
        Select(s.player.inventory, 0, playAudio:false);
        foreach (GameObject toBeRemoved in floorItems) {
            Destroy(toBeRemoved);
            // destroy every floor item
        }
        floorItems.Clear(); 
        Save.game.blacksmithHasForged = false;
        // clear the array
    }

    /// <summary>
    /// Returns true if the player has an item of the given name.
    /// </summary>
    public bool PlayerHas(string itemName) { return GetInventoryItemCount(itemName) > 0; }

    public int GetPlayerItemCount(string itemName) { return GetInventoryItemCount(itemName); }

    public static string GetRuinedCommonItemName(string itemName) {
        if (string.IsNullOrWhiteSpace(itemName)) { return itemName; }

        string normalizedItemName = NormalizeCommonItemKey(itemName).Replace('_', ' ');
        return RuinedCommonItemNames.TryGetValue(normalizedItemName, out string ruinedItemName)
            ? NormalizeCommonItemKey(ruinedItemName)
            : NormalizeCommonItemKey(normalizedItemName);
    }

    public static string GetNightmareStarterTradeItemName(int characterNum) {
        return characterNum switch {
            0 => "rotten_steak",
            1 => "rusted_helm",
            2 => "ruined_boots",
            3 => "moldy_cheese",
            _ => ""
        };
    }

    // returns the weapon base name; handles multi-word names like "glass sword" correctly
    // e.g. "common glass sword" → "glass sword", "legendary sword" → "sword"
    public static string GetWeaponBaseName(string fullItemName) {
        if (string.IsNullOrWhiteSpace(fullItemName)) { return ""; }

        int spaceIndex = fullItemName.IndexOf(' ');
        if (spaceIndex < 0) { return fullItemName; }

        string prefix = fullItemName.Substring(0, spaceIndex);
        return WeaponNamePrefixes.Contains(prefix)
            ? fullItemName.Substring(spaceIndex + 1)
            : fullItemName;
    }

    /// <summary>
    /// Returns true if the player has a weapon of given type.
    /// </summary>
    public bool PlayerHasWeapon(string weaponName) {
        EnsureInventoryCache();
        return cachedEquippedWeapon != null && cachedEquippedWeaponBaseName == weaponName;
    }

    public bool PlayerHasCharm(string modifier) {
        EnsureInventoryCache();
        return GetDictionaryCount(charmCountsByModifier, modifier) > 0;
    }

    public int GetCharmCount(string modifier) {
        EnsureInventoryCache();
        return GetDictionaryCount(charmCountsByModifier, modifier);
    }

    public bool PlayerHasTarot(string modifier) {
        EnsureInventoryCache();
        return GetDictionaryCount(tarotCountsByModifier, modifier) > 0;
    }

    public int GetTarotCount(string modifier) {
        EnsureInventoryCache();
        return GetDictionaryCount(tarotCountsByModifier, modifier);
    }

    public int GetCharmArcaneEffectiveness() {
        EnsureInventoryCache();
        return 1 + GetCharmCount("arcane") + (PlayerHasWeapon("stave") ? 1 : 0);
    }

    public int GetCharmEffectiveness(string modifier) {
        return CharmUsesArcaneScaling(modifier) ? GetCharmArcaneEffectiveness() : 1;
    }

    public int GetEffectiveCharmCount(string modifier) {
        return GetCharmCount(modifier) * GetCharmEffectiveness(modifier);
    }

    public int GetTarotArcaneEffectiveness() {
        EnsureInventoryCache();
        return 1 + GetTarotCount("arcane") + (PlayerHasWeapon("stave") ? 1 : 0);
    }

    public int GetTarotEffectiveness(string modifier) {
        return TarotUsesArcaneScaling(modifier) ? GetTarotArcaneEffectiveness() : 1;
    }

    public int GetTarotBonusForDieType(string diceType) {
        string tarotModifier = diceType switch {
            "blue" => "abyss",
            "green" => "verdant",
            "red" => "inferno",
            "white" => "glacier",
            "yellow" => "dawn",
            _ => ""
        };

        return string.IsNullOrEmpty(tarotModifier)
            ? 0
            : GetTarotCount(tarotModifier) * GetTarotEffectiveness(tarotModifier);
    }

    public int GetEnemyTarotPenaltyForDieType(string diceType) {
        string tarotModifier = diceType switch {
            "blue" => "leviathan",
            "green" => "viper",
            "red" => "dragon",
            "white" => "wyvern",
            "yellow" => "phoenix",
            _ => ""
        };

        return string.IsNullOrEmpty(tarotModifier)
            ? 0
            : GetTarotCount(tarotModifier) * GetTarotEffectiveness(tarotModifier);
    }

    private static bool CharmUsesArcaneScaling(string modifier) {
        return modifier is not "" and not "arcane" and not "nothing";
    }

    private static bool TarotUsesArcaneScaling(string modifier) {
        return modifier is not "" and not "arcane" and not "nothing";
    }

    /// <summary>
    /// Queue any tarot bonus for a newly attached player die.
    /// </summary>
    public void TryUpgradeTakenDieWithTarot(Dice dice, float delay = 0.05f) {
        if (dice != null && dice.diceType == "white" && Save.game.curCharNum == 2) { return; }

        int tarotBonus = GetTarotBonusForDieType(dice == null ? "" : dice.diceType);
        if (dice == null || tarotBonus <= 0 || dice.tarotUpgradeApplied) { return; }

        s?.turnManager?.BeginEnemyPlanRefreshBatch();
        StartCoroutine(UpgradeTakenDieWithTarotAfterDelay(dice, tarotBonus, delay));
    }

    public void TryWeakenEnemyTakenDieWithTarot(Dice dice, float delay = 0.05f) {
        int tarotPenalty = GetEnemyTarotPenaltyForDieType(dice == null ? "" : dice.diceType);
        if (dice == null || tarotPenalty <= 0 || !dice.isAttached || dice.isOnPlayerOrEnemy != "enemy") { return; }

        s?.turnManager?.BeginEnemyPlanRefreshBatch();
        StartCoroutine(WeakenEnemyTakenDieWithTarotAfterDelay(dice, tarotPenalty, delay));
    }

    /// <summary>
    /// Apply the tarot bonus after any other die-settling effects have finished.
    /// </summary>
    private IEnumerator UpgradeTakenDieWithTarotAfterDelay(Dice dice, int tarotBonus, float delay) {
        try {
            if (delay > 0f) { yield return new WaitForSeconds(delay); }

            if (dice == null || !dice.isAttached || dice.isOnPlayerOrEnemy != "player") {
                yield break;
            }

            dice.tarotUpgradeApplied = true;
            if (dice.diceNum >= 6) {
                s.diceSummoner.SaveDiceValues();
                yield break;
            }

            for (int i = 0; i < tarotBonus; i++) {
                if (dice == null || !dice.isAttached || dice.isOnPlayerOrEnemy != "player" || dice.diceNum >= 6) {
                    yield break;
                }

                yield return StartCoroutine(dice.IncreaseDiceValue(false));
            }
        }
        finally {
            s?.turnManager?.EndEnemyPlanRefreshBatch(true);
        }
    }

    private IEnumerator WeakenEnemyTakenDieWithTarotAfterDelay(Dice dice, int tarotPenalty, float delay) {
        try {
            if (delay > 0f) { yield return new WaitForSeconds(delay); }

            for (int i = 0; i < tarotPenalty; i++) {
                if (dice == null || !dice.isAttached || dice.isOnPlayerOrEnemy != "enemy") {
                    yield break;
                }

                if (dice.diceNum <= 1) {
                    yield return StartCoroutine(dice.FadeOut());
                    yield break;
                }

                yield return StartCoroutine(dice.DecreaseDiceValue(false));
            }
        }
        finally {
            s?.turnManager?.EndEnemyPlanRefreshBatch(true);
        }
    }

    // recalculate always-on charm stat bonuses from inventory
    public void UpdateCharmPassiveStats() {
        int crystalShardCount = GetInventoryItemCount("crystal_shard");
        pendingShatteredCrystalShards = Mathf.Clamp(pendingShatteredCrystalShards, 0, crystalShardCount);
        int activeCrystalShardCount = Mathf.Max(0, crystalShardCount - pendingShatteredCrystalShards);
        charmPassiveStats["green"] = 0;
        charmPassiveStats["blue"] = 0;
        charmPassiveStats["red"] = activeCrystalShardCount * 2;
        charmPassiveStats["white"] = 0;
    }

    public void RegisterCrystalShardShatter(int amount) {
        if (amount <= 0) { return; }

        pendingShatteredCrystalShards += amount;
        RefreshPassiveInventoryEffects();
    }

    private void ClearCharmProcCounts(Dictionary<string, int> procCounts) {
        foreach (string charmType in charmTypes) {
            procCounts[charmType] = 0;
        }
    }

    private bool TryGetCharmTriggeredStat(string modifier, out string stat, out int amountPerTrigger) {
        stat = "";
        amountPerTrigger = 0;
        switch (modifier) {
            case "unbroken":
                stat = "white";
                amountPerTrigger = 1;
                return true;
            case "relentless":
                stat = "red";
                amountPerTrigger = 1;
                return true;
            case "aether":
                stat = "blue";
                amountPerTrigger = 1;
                return true;
            case "ruthless":
                stat = "green";
                amountPerTrigger = 1;
                return true;
            case "riposte":
                stat = "red";
                amountPerTrigger = 1;
                return true;
            case "bulwark":
                stat = "white";
                amountPerTrigger = 1;
                return true;
            case "vindictive":
                stat = "red";
                amountPerTrigger = 2;
                return true;
            case "inevitable":
                stat = "red";
                amountPerTrigger = 1;
                return true;
            default:
                return false;
        }
    }

    private void RebuildCharmTriggeredBonuses() {
        foreach (string stat in statArr) {
            charmActiveBonus[stat] = 0;
            charmPendingBonus[stat] = 0;
        }

        foreach (string charmType in charmTypes) {
            if (TryGetCharmTriggeredStat(charmType, out string stat, out int amountPerTrigger)) {
                int activeCount = charmActiveProcCounts.TryGetValue(charmType, out int ac) ? ac : 0;
                int pendingCount = charmPendingProcCounts.TryGetValue(charmType, out int pc) ? pc : 0;
                int effectiveAmountPerTrigger = amountPerTrigger * GetCharmEffectiveness(charmType);
                charmActiveBonus[stat] += activeCount * effectiveAmountPerTrigger;
                charmPendingBonus[stat] += pendingCount * effectiveAmountPerTrigger;
            }
        }
    }

    private bool RestoreCharmProcCounts(int[] savedProcCounts, Dictionary<string, int> runtimeProcCounts) {
        if (savedProcCounts == null || savedProcCounts.Length != charmTypes.Length) { return false; }

        bool hasAnySavedProc = false;
        for (int i = 0; i < charmTypes.Length; i++) {
            int procCount = Mathf.Max(0, savedProcCounts[i]);
            if (runtimeProcCounts.ContainsKey(charmTypes[i])) {
                runtimeProcCounts[charmTypes[i]] = procCount;
            }
            hasAnySavedProc |= procCount > 0;
        }

        return hasAnySavedProc;
    }

    private void SaveCharmProcCounts(int[] savedProcCounts, Dictionary<string, int> runtimeProcCounts) {
        for (int i = 0; i < charmTypes.Length; i++) {
            savedProcCounts[i] = runtimeProcCounts.TryGetValue(charmTypes[i], out int count) ? count : 0;
        }
    }

    public void QueueCharmTrigger(string modifier, int triggerCount = 1) {
        if (triggerCount <= 0 || !charmPendingProcCounts.ContainsKey(modifier)) { return; }
        if (!TryGetCharmTriggeredStat(modifier, out string stat, out int amountPerTrigger)) { return; }

        charmPendingProcCounts[modifier] += triggerCount;
        charmPendingBonus[stat] += triggerCount * amountPerTrigger * GetCharmEffectiveness(modifier);
        SyncCharmStateToSave();
        PersistTriggeredCharmState();
    }

    public void ActivateCharmTriggerImmediately(string modifier, int triggerCount = 1, bool refreshCombatUI = true) {
        if (triggerCount <= 0 || !charmActiveProcCounts.ContainsKey(modifier)) { return; }
        if (!TryGetCharmTriggeredStat(modifier, out string stat, out int amountPerTrigger)) { return; }

        charmActiveProcCounts[modifier] += triggerCount;
        charmActiveBonus[stat] += triggerCount * amountPerTrigger * GetCharmEffectiveness(modifier);
        SyncCharmStateToSave();
        if (refreshCombatUI) {
            RefreshPlayerCombatStatsAndDice();
        }
        PersistTriggeredCharmState();
    }

    public void RotatePendingCharmBonusesToActive() {
        foreach (string charmType in charmTypes) {
            if (charmActiveProcCounts.ContainsKey(charmType) && charmPendingProcCounts.ContainsKey(charmType)) {
                charmActiveProcCounts[charmType] = charmPendingProcCounts[charmType];
                charmPendingProcCounts[charmType] = 0;
            }
        }

        RebuildCharmTriggeredBonuses();
        UpdateCharmPassiveStats();
        SyncCharmStateToSave();
        PersistTriggeredCharmState();
    }

    public void ClearTriggeredCharmBonuses() {
        ClearCharmProcCounts(charmActiveProcCounts);
        ClearCharmProcCounts(charmPendingProcCounts);
        RebuildCharmTriggeredBonuses();
        UpdateCharmPassiveStats();
        SyncCharmStateToSave();
        PersistTriggeredCharmState();
    }

    // restore charm active bonuses from save
    public void RestoreCharmStateFromSave() {
        ClearCharmProcCounts(charmActiveProcCounts);
        ClearCharmProcCounts(charmPendingProcCounts);
        foreach (string stat in statArr) {
            charmActiveBonus[stat] = 0;
            charmPendingBonus[stat] = 0;
        }

        bool hasSavedProcCounts = RestoreCharmProcCounts(Save.game.charmActiveProcCounts, charmActiveProcCounts)
            | RestoreCharmProcCounts(Save.game.charmPendingProcCounts, charmPendingProcCounts);
        bool hasSavedScalarBonuses = Save.game.charmActiveBonusGreen != 0
            || Save.game.charmActiveBonusBlue != 0
            || Save.game.charmActiveBonusRed != 0
            || Save.game.charmActiveBonusWhite != 0
            || Save.game.charmPendingBonusGreen != 0
            || Save.game.charmPendingBonusBlue != 0
            || Save.game.charmPendingBonusRed != 0
            || Save.game.charmPendingBonusWhite != 0;

        if (hasSavedProcCounts) {
            RebuildCharmTriggeredBonuses();
        }
        else if (hasSavedScalarBonuses) {
            charmActiveBonus["green"] = Save.game.charmActiveBonusGreen;
            charmActiveBonus["blue"]  = Save.game.charmActiveBonusBlue;
            charmActiveBonus["red"]   = Save.game.charmActiveBonusRed;
            charmActiveBonus["white"] = Save.game.charmActiveBonusWhite;
            charmPendingBonus["green"] = Save.game.charmPendingBonusGreen;
            charmPendingBonus["blue"]  = Save.game.charmPendingBonusBlue;
            charmPendingBonus["red"]   = Save.game.charmPendingBonusRed;
            charmPendingBonus["white"] = Save.game.charmPendingBonusWhite;
        }

        UpdateCharmPassiveStats();
        SyncCharmStateToSave();
    }

    public void SyncCharmStateToSave() {
        bool runtimeHasTriggeredCharmState = charmActiveProcCounts.Values.Any(value => value > 0)
            || charmPendingProcCounts.Values.Any(value => value > 0)
            || charmActiveBonus.Values.Any(value => value != 0)
            || charmPendingBonus.Values.Any(value => value != 0);
        bool savedHasTriggeredCharmState = (Save.game.charmActiveProcCounts != null && Save.game.charmActiveProcCounts.Any(value => value > 0))
            || (Save.game.charmPendingProcCounts != null && Save.game.charmPendingProcCounts.Any(value => value > 0))
            || Save.game.charmActiveBonusGreen != 0
            || Save.game.charmActiveBonusBlue != 0
            || Save.game.charmActiveBonusRed != 0
            || Save.game.charmActiveBonusWhite != 0
            || Save.game.charmPendingBonusGreen != 0
            || Save.game.charmPendingBonusBlue != 0
            || Save.game.charmPendingBonusRed != 0
            || Save.game.charmPendingBonusWhite != 0;

        if (s == null) {
            s = FindFirstObjectByType<Scripts>();
        }

        bool inventoryNotReady = s == null || s.player == null || s.player.inventory == null || s.player.inventory.Count == 0;
        if (inventoryNotReady && !runtimeHasTriggeredCharmState && savedHasTriggeredCharmState) {
            return;
        }

        Save.game.charmActiveProcCounts ??= new int[charmTypes.Length];
        Save.game.charmPendingProcCounts ??= new int[charmTypes.Length];
        if (Save.game.charmActiveProcCounts.Length != charmTypes.Length) { Save.game.charmActiveProcCounts = new int[charmTypes.Length]; }
        if (Save.game.charmPendingProcCounts.Length != charmTypes.Length) { Save.game.charmPendingProcCounts = new int[charmTypes.Length]; }

        SaveCharmProcCounts(Save.game.charmActiveProcCounts, charmActiveProcCounts);
        SaveCharmProcCounts(Save.game.charmPendingProcCounts, charmPendingProcCounts);
        Save.game.charmActiveBonusGreen = charmActiveBonus["green"];
        Save.game.charmActiveBonusBlue  = charmActiveBonus["blue"];
        Save.game.charmActiveBonusRed   = charmActiveBonus["red"];
        Save.game.charmActiveBonusWhite = charmActiveBonus["white"];
        Save.game.charmPendingBonusGreen = charmPendingBonus["green"];
        Save.game.charmPendingBonusBlue  = charmPendingBonus["blue"];
        Save.game.charmPendingBonusRed   = charmPendingBonus["red"];
        Save.game.charmPendingBonusWhite = charmPendingBonus["white"];
    }

    private void PersistTriggeredCharmState() {
        if (Save.game == null) { return; }

        if (s == null) {
            s = FindFirstObjectByType<Scripts>();
        }

        if (s != null && s.tutorial != null) { return; }

        Save.SaveGame();
    }

    public bool PlayerHasLegendary() {
        EnsureInventoryCache();
        return cachedEquippedWeapon != null && cachedEquippedWeaponIsLegendary;
    }

    /// <summary>
    /// Gets the first item in the player's inventory with given name.
    /// </summary>
    public GameObject GetPlayerItem(string itemName) {
        EnsureInventoryCache();
        string normalizedItemName = NormalizeCommonItemKey(itemName);
        return firstInventoryItemByName.TryGetValue(normalizedItemName, out GameObject itemObject) ? itemObject : null;
    }

    /// <summary>
    /// Fade all torches that have ended their lifespan.
    /// </summary>
    public void AttemptFadeTorches(bool advanceCounter) {
        bool updatedTorchState = false;
        List<GameObject> torchesToFade = new();

        foreach (GameObject curItem in s.player.inventory) {
            Item torch = curItem.GetComponent<Item>();
            if (torch == null || torch.itemName != "torch") { continue; }

            if (TryParseTorchCombatCounter(torch.modifier, out int remainingRooms)) {
                if (!advanceCounter) { continue; }

                remainingRooms = Mathf.Max(0, remainingRooms - 1);
                if (remainingRooms <= 0) {
                    torchesToFade.Add(curItem);
                }
                else {
                    torch.modifier = $"rooms:{remainingRooms}";
                    updatedTorchState = true;
                }
                continue;
            }

            continue;
        }

        if (updatedTorchState) {
            SaveInventoryItems();
        }

        foreach (GameObject torchToFade in torchesToFade) {
            s.turnManager.SetStatusText("your torch runs out");
            torchToFade.GetComponent<Item>().Remove(torchFade:true);
        }
    }

    private static bool TryParseTorchCombatCounter(string modifier, out int remainingRooms) {
        remainingRooms = 0;
        if (string.IsNullOrWhiteSpace(modifier)) { return false; }

        string value = modifier.StartsWith("rooms:") ? modifier.Substring("rooms:".Length) : modifier;
        return !modifier.Contains('-') && int.TryParse(value, out remainingRooms);
    }

    /// <summary>
    /// Save all inventory items onto the player's local Savefile.
    /// </summary>
    public void SaveInventoryItems(bool forceSave = false) {
        if (s.levelManager != null && (forceSave || !s.player.isDead)) { 
            EnsureInventoryCache();
            Item equippedWeapon = GetEquippedWeapon();
            if (equippedWeapon == null) {
                Debug.LogWarning("Skipping inventory save because there is no equipped weapon to serialize");
                return;
            }

            SyncCharmStateToSave();

            Save.game.resumeItemNames = new string[9];
            Save.game.resumeItemTypes = new string[9];
            Save.game.resumeItemMods = new string[9];
            // clear the data before placing in new
            Save.game.resumeItemNames[0] = GetWeaponBaseName(equippedWeapon.itemName);
            Save.game.resumeItemTypes[0] = equippedWeapon.itemType;
            Save.game.resumeItemMods[0] = equippedWeapon.modifier;
            Save.game.resumeAcc = equippedWeapon.weaponStats["green"];
            Save.game.resumeSpd = equippedWeapon.weaponStats["blue"];
            Save.game.resumeDmg = equippedWeapon.weaponStats["red"];
            Save.game.resumeDef = equippedWeapon.weaponStats["white"];
            // add the player's weapon first
            int saveIndex = 1;
            for (int i = 1; i < s.player.inventory.Count; i++) {
                Item curItem = s.player.inventory[i].GetComponent<Item>();
                Save.game.resumeItemNames[saveIndex] = NormalizeCommonItemKey(curItem.itemName);
                Save.game.resumeItemTypes[saveIndex] = curItem.itemType;
                Save.game.resumeItemMods[saveIndex] = curItem.modifier;
                saveIndex++;
                if (saveIndex >= Save.game.resumeItemNames.Length) { break; }
                // add all the remaining items
            }
            if (s.tutorial == null) { Save.SaveGame(); }
            // Save to file

        }
    }

    public void KeepOnlyWeaponAndBrokenAmulet() {
        if (s == null) { s = FindFirstObjectByType<Scripts>(); }
        if (s == null || s.player == null || s.player.inventory == null || s.player.inventory.Count == 0) { return; }

        for (int i = s.player.inventory.Count - 1; i >= 1; i--) {
            GameObject inventoryItem = s.player.inventory[i];
            if (inventoryItem == null) {
                s.player.inventory.RemoveAt(i);
                continue;
            }

            Item itemScript = inventoryItem.GetComponent<Item>();
            if (itemScript != null && itemScript.itemName == "broken_amulet") { continue; }

            Destroy(inventoryItem);
            s.player.inventory.RemoveAt(i);
        }

        for (int i = 0; i < s.player.inventory.Count; i++) {
            s.player.inventory[i].transform.position = new Vector2(itemX + itemSpacing * i, 3.16f);
        }

        foreach (string stat in statArr) {
            charmPassiveStats[stat] = 0;
        }
        InvalidateInventoryCache();
        ClearTriggeredCharmBonuses();
        RefreshPassiveInventoryEffects(updateUI:false);
        SyncCharmStateToSave();

        curList = s.player.inventory;
        Select(s.player.inventory, 0, playAudio:false);
        s.statSummoner.SummonStats();
        s.statSummoner.SetCombatDebugInformationFor("player");
    }

    // mark an item as discovered in persistent save (called when item enters player inventory)
    public void MarkItemDiscovered(Item item) {
        if (Save.persistent == null || item == null) { return; }
        string fullName = GetAlmanacFullName(item);
        if (string.IsNullOrEmpty(fullName)) { return; }

        int wi = System.Array.IndexOf(AlmanacWeaponOrder, fullName);
        if (wi >= 0) {
            Save.persistent.discoveredWeapons[wi] = true;
            Save.persistent.discoveredWeaponCounts[wi]++;
            Save.SavePersistent();
            return;
        }
        int ii = System.Array.IndexOf(AlmanacItemOrder, fullName);
        if (ii >= 0) {
            Save.persistent.discoveredItems[ii] = true;
            Save.persistent.discoveredItemCounts[ii]++;
            Save.SavePersistent();
        }
    }

    public void MarkItemUsed(Item item) {
        if (item == null) { return; }
        MarkItemUsed(GetTrackedItemUseName(item));
    }

    public void MarkItemUsed(string fullName) {
        if (Save.persistent == null || string.IsNullOrWhiteSpace(fullName)) { return; }

        Save.persistent.Normalize();
        int itemIndex = System.Array.IndexOf(AlmanacItemOrder, fullName);
        if (itemIndex < 0) { return; }

        Save.persistent.itemUses[itemIndex]++;
    }

    // resolve the canonical almanac name for an item (used for discovery tracking)
    private static string GetAlmanacFullName(Item item) {
        if (item.itemType == "weapon") {
            string baseName = GetWeaponBaseName(item.itemName);
            return item.modifier == "legendary" ? $"legendary {baseName}" : baseName;
        }
        return GetCanonicalItemDisplayName(item.itemName, item.modifier);
    }

    private static string GetTrackedItemUseName(Item item) {
        if (item == null) { return ""; }

        return item.itemName switch {
            "rotten_steak" => "steak",
            "moldy_cheese" => "cheese",
            "broken_amulet" => "amulet of resurrection",
            _ => GetAlmanacFullName(item),
        };
    }

    public Dictionary<string, int> GetDefaultWeaponStatsForEntry(string entryName) {
        bool isLegendary = entryName.StartsWith("legendary ");
        string baseName = isLegendary ? entryName.Substring(10) : entryName;
        Dictionary<string, int> source = null;

        if (isLegendary) {
            legendaryStatDict.TryGetValue(baseName, out source);
        }
        else {
            weaponStatDict.TryGetValue(baseName, out source);
        }

        return source == null
            ? new Dictionary<string, int>()
            : source.ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    // create a display item for the almanac at a specific world position
    // caller must add the returned object to floorItems if needed
    public GameObject CreateAlmanacItem(string entryName, bool known, bool isWeaponPage, Vector2 pos) {
        Sprite sprite;
        string displayName;
        string itemType;

        if (!known) {
            sprite     = GetItemSprite("hint");
            displayName = "hint";
            itemType    = "common";
        }
        else if (isWeaponPage) {
            bool   isLegendary = entryName.StartsWith("legendary ");
            string baseName    = isLegendary ? entryName.Substring(10) : entryName;
            sprite      = GetWeaponSprite(baseName);
            displayName = entryName;
            itemType    = "weapon";
        }
        else {
            sprite      = GetItemSprite(GetAlmanacSpriteName(entryName));
            displayName = entryName;
            itemType    = "common";
        }

        Item itemScript = CreateItemInstance(pos, displayName, itemType, sprite);
        if (known && isWeaponPage) {
            itemScript.modifier = entryName.StartsWith("legendary ") ? "legendary" : "common";
            itemScript.weaponStats = GetDefaultWeaponStatsForEntry(entryName);
        }
        return itemScript.gameObject;
    }

    // resolve the sprite lookup name for a page-2 almanac entry
    private static string GetAlmanacSpriteName(string entry) {
        if (entry.StartsWith("scroll of")) { return "scroll"; }
        if (entry.StartsWith("potion of")) { return "potion"; }
        if (entry.Contains("necklet"))     { return "necklet"; }
        if (entry.StartsWith("charm of"))  { return "charm"; }
        if (entry.StartsWith("tarot of"))  { return "tarot"; }
        if (entry.EndsWith(" gem"))        { return "gem"; }
        return entry.Replace(" ", "_");
    }

    private bool IsKnownWeaponName(string weaponName) {
        string normalizedWeaponName = NormalizeWeaponSaveName(weaponName);
        return !string.IsNullOrEmpty(normalizedWeaponName)
            && (weaponSpriteByBaseName.ContainsKey(normalizedWeaponName) || weaponStatDict.ContainsKey(normalizedWeaponName));
    }

    private bool HasItemSprite(string itemName) {
        string normalizedItemName = NormalizeItemSpriteName(itemName);
        return !string.IsNullOrEmpty(normalizedItemName) && spriteLookup.ContainsKey(normalizedItemName);
    }

    private Sprite GetWeaponSprite(string weaponName) {
        string normalizedWeaponName = NormalizeWeaponSaveName(weaponName);
        if (!string.IsNullOrEmpty(normalizedWeaponName)
            && weaponSpriteByBaseName.TryGetValue(normalizedWeaponName, out Sprite sprite)) {
            return sprite;
        }

        return GetItemSprite(normalizedWeaponName);
    }

    private string NormalizeItemSpriteName(string itemName) {
        string normalizedItemName = NormalizeCommonItemKey(itemName);
        return normalizedItemName switch {
            "broken_amulet" => "amulet_of_resurrection",
            "broken_helm" => "helm_of_might",
            "rusted_helm" => "helm_of_might",
            "bloodletters_curse" => "bloodletters_curse",
            "rotting_mask" => "bloodletters_curse",
            "rabadons_deathcap" => "rabadons_deathcap",
            "ruined_cap" => "rabadons_deathcap",
            "shattered_mask" => "cursed_mask",
            "broken_goggles" => "goggles",
            "empty_shaker" => "salt_shaker",
            "broken_horn" => "cornucopia",
            "torn_armband" => "thiefs_armband",
            "desecrated_chalice" => "sacrificial_chalice",
            "ravaged_book" => "unstable_spellbook",
            "rotten_steak" => "steak",
            "moldy_cheese" => "cheese",
            "ruined_boots" => "boots_of_dodge",
            "shattered_ankh" => "ankh",
            "defiled_kapala" => "kapala",
            "rotten_ham" => "ham",
            "emerald_gem" => "gem",
            "ruby_gem" => "gem",
            "sapphire_gem" => "gem",
            "topaz_gem" => "gem",
            "citrine_gem" => "gem",
            "shattered_glass_sword" => "glass_sword_shattered",
            "glass_sword_shattered" => "glass_sword_shattered",
            _ => normalizedItemName
        };
    }

    private static string GetCanonicalCreatedItemName(string itemName) {
        return NormalizeCommonItemKey(itemName);
    }

    private static void NormalizeLegacyCommonItem(ref string itemName, ref string modifier) {
        itemName = GetCanonicalCreatedItemName(itemName);
    }

    private string GetSafeResumeWeaponName() {
        string normalizedSavedWeaponName = NormalizeWeaponSaveName(Save.game.resumeItemNames[0]);
        if (IsKnownWeaponName(normalizedSavedWeaponName)) { return normalizedSavedWeaponName; }

        int fallbackIndex = Mathf.Clamp(Save.game.curCharNum, 0, 3);
        return fallbackIndex switch {
            0 => "sword",
            1 => "maul",
            2 => "dagger",
            _ => "mace"
        };
    }

    private Item GetEquippedWeapon() {
        EnsureInventoryCache();
        return cachedEquippedWeapon;
    }

    private void RestoreSavedInventoryDirectly() {
        string resumeWeaponName = GetSafeResumeWeaponName();
        bool hasSavedWeapon = IsKnownWeaponName(Save.game.resumeItemNames[0]);
        string resumeWeaponModifier = string.IsNullOrEmpty(Save.game.resumeItemMods[0]) ? "common" : Save.game.resumeItemMods[0];
        int resumeAcc = hasSavedWeapon ? Save.game.resumeAcc : weaponStatDict[resumeWeaponName]["green"];
        int resumeSpd = hasSavedWeapon ? Save.game.resumeSpd : weaponStatDict[resumeWeaponName]["blue"];
        int resumeDmg = hasSavedWeapon ? Save.game.resumeDmg : weaponStatDict[resumeWeaponName]["red"];
        int resumeDef = hasSavedWeapon ? Save.game.resumeDef : weaponStatDict[resumeWeaponName]["white"];

        CreateWeaponInInventory(resumeWeaponName, resumeWeaponModifier, resumeAcc, resumeSpd, resumeDmg, resumeDef);

        for (int i = 1; i < 9; i++) {
            if (string.IsNullOrEmpty(Save.game.resumeItemNames[i])) { break; }

            string savedItemName = NormalizeCommonItemKey(Save.game.resumeItemNames[i]);
            if (!HasItemSprite(savedItemName)) { continue; }

            CreateCommonItemInInventory(savedItemName, Save.game.resumeItemMods[i]);
        }

        InvalidateInventoryCache();
    }

    private void CreateWeaponInInventory(string weaponName, string modifier, int aim, int spd, int atk, int def) {
        weaponName = NormalizeWeaponSaveName(weaponName);
        Sprite sprite = weaponName == "glass sword" && modifier == "shattered"
            ? GetItemSprite("glass_sword_shattered")
            : GetWeaponSprite(weaponName);
        Dictionary<string, int> weaponStats = new() {
            { "green", aim >= 0 ? aim : -1 },
            { "blue", spd >= 0 ? spd : -1 },
            { "red", atk >= 0 ? atk : -1 },
            { "white", def >= 0 ? def : -1 }
        };

        Item itemScript = CreateItemInstance(new Vector2(itemX, 3.16f), BuildWeaponFullName(weaponName, modifier), "weapon", sprite, modifier, weaponStats);
        s.player.inventory.Add(itemScript.gameObject);
        InvalidateInventoryCache();
        s.player.stats = itemScript.weaponStats;
        Save.game.resumeAcc = s.player.stats["green"];
        Save.game.resumeSpd = s.player.stats["blue"];
        Save.game.resumeDmg = s.player.stats["red"];
        Save.game.resumeDef = s.player.stats["white"];
    }

    private void CreateCommonItemInInventory(string itemName, string modifier) {
        NormalizeLegacyCommonItem(ref itemName, ref modifier);
        CreateInventoryItem(itemName, "common", GetItemSprite(itemName), modifier);
    }

    public string GetActionTextForItem(Item item, string verb) {
        if (item == null) { return verb; }

        string displayName = GetCanonicalItemDisplayName(item.itemName, item.modifier);

        if (item.itemName == "necklet") {
            return item.modifier == "arcane"
                ? $"{verb} arcane necklet"
                : $"{verb} {item.itemName} of {item.modifier}";
        }

        if (item.itemName == "charm") {
            return $"{verb} charm of the {item.modifier}";
        }

        if (item.itemName == "tarot") {
            return $"{verb} tarot of the {item.modifier}";
        }

        if (item.itemName == "potion" || item.itemName == "scroll") {
            return $"{verb} {item.itemName} of {item.modifier}";
        }

        return $"{verb} {displayName}";
    }

    /// <summary>
    /// Save all floor items onto the player's local Savefile.
    /// </summary>
    public void SaveFloorItems() { 
        Save.game.floorItemNames = new string[9];
        Save.game.floorItemTypes = new string[9];
        Save.game.floorItemMods = new string[9];
        Save.game.floorItemAccs = new int[9];
        Save.game.floorItemSpds = new int[9];
        Save.game.floorItemDmgs = new int[9];
        Save.game.floorItemDefs = new int[9];
        // clear the data before placing in new 
        for (int i = 0; i < floorItems.Count; i++) {
            Item curItem = floorItems[i].GetComponent<Item>();
            if (curItem.itemType == "weapon") {
                Save.game.floorItemNames[i] = GetWeaponBaseName(curItem.itemName);
                Save.game.floorItemAccs[i] = curItem.weaponStats["green"];
                Save.game.floorItemSpds[i] = curItem.weaponStats["blue"];
                Save.game.floorItemDmgs[i] = curItem.weaponStats["red"];
                Save.game.floorItemDefs[i] = curItem.weaponStats["white"];
            }
            else { Save.game.floorItemNames[i] = NormalizeCommonItemKey(curItem.itemName); }

            Save.game.floorItemTypes[i] = curItem.itemType;
            Save.game.floorItemMods[i] = curItem.modifier;
        }
        if (IsVendorEncounter()) {
            StoreCurrentFloorItemsAsLastTrader(s.levelManager.level, s.levelManager.sub, Save.game.enemyNum);
        }
        if (s.tutorial == null) { Save.SaveGame(); }
    }

    public void StoreCurrentFloorItemsAsLastTrader(int traderLevel, int traderSub, int traderEnemyNum) {
        Save.game.lastTraderItemNames = new string[9];
        Save.game.lastTraderItemTypes = new string[9];
        Save.game.lastTraderItemMods = new string[9];
        Save.game.lastTraderItemAccs = new int[9];
        Save.game.lastTraderItemSpds = new int[9];
        Save.game.lastTraderItemDmgs = new int[9];
        Save.game.lastTraderItemDefs = new int[9];
        Save.game.lastTraderLevel = traderLevel;
        Save.game.lastTraderSub = traderSub;
        Save.game.lastTraderEnemyNum = traderEnemyNum;

        for (int i = 0; i < floorItems.Count; i++) {
            Item curItem = floorItems[i].GetComponent<Item>();
            if (curItem.itemType == "weapon") {
                Save.game.lastTraderItemNames[i] = GetWeaponBaseName(curItem.itemName);
                Save.game.lastTraderItemAccs[i] = curItem.weaponStats["green"];
                Save.game.lastTraderItemSpds[i] = curItem.weaponStats["blue"];
                Save.game.lastTraderItemDmgs[i] = curItem.weaponStats["red"];
                Save.game.lastTraderItemDefs[i] = curItem.weaponStats["white"];
            }
            else {
                Save.game.lastTraderItemNames[i] = NormalizeCommonItemKey(curItem.itemName);
            }

            Save.game.lastTraderItemTypes[i] = curItem.itemType;
            Save.game.lastTraderItemMods[i] = curItem.modifier;
        }
    }

    // two separate methods so that we dont have to do any fancy checks, just pull whichever we need as long as we Save it properly


}
