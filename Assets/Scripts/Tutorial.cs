using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Tutorial : MonoBehaviour {
    [SerializeField] private Sprite blackBox;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] public int curIndex = 0;
    private List<string> tutorialTextList = new List<string>() {
        "Welcome to MALLEUS GOBLINIFICARIUM,\na realistic dice-based combat simulator!\n\nYes, a realistic game can contain goblins and scrolls.\n[click] to continue",
        "You can navigate your inventory using\nleft and right [arrow_keys]\nor [left_mouse]\n\n[click to continue]",
        "Press [enter] or [left_mouse] to use an item,\n[shift_enter] or [right_mouse] to drop it.\nYou can use or drop selected items only.\n\n[drop the scroll] to continue",
        "\n\n\n\n[click] to continue",
        "\n\n\n\n[click] to continue",
        "Accuracy (green) allows you to choose\ndifferent body parts (each applying some\nspecial debuff in damaged) as a target.\n\n[click] to continue",
        "You can use up and down\n[arrow_keys] or [mouse_wheel]\nto adjust your aim.\n\n[click] to continue",
        "As you invest into your accuracy,\nyou will have more option\nto choose from.\n\n[click] to continue",
        "If your damage (red) is higher than\nthe enemy's parry (white), you will wound\nthe body part you are targeting.\n\n[click] to continue",
        "Respectively, if your parry (white) isn\nhigher than the enemy's damage (red), his\nattack will deal no harm.\n\n[click] to continue",
        "Speed (blue) defines who will strike first.\nIt matters because all debuffs are\napplied instantly.\n\n[click] to continue",
        "Speed also defines who will start\nthe draft, a process of picking dice,\none by one.\n\n[click] to continue",
        "The die you pick increases\nthe corresponding combat stat.\nYour speed is higher, so choose one!\n\n[click + drag die] to continue",
        "You can notice that your enemy has\ntaken a die as well.\nThis is how draft works.\n\n[pick 2 more dice] to continue",
        "Now your damage (7) is higher than\nenemy's parry (3), so you'll inflict a\nwound. You are also safe from his blow.\n\n[click] to continue",
        "You have the same speed (3)\nas your enemy,\nwhich still means you'll strike first.\n\n[click] to continue",
        "You can probably start the fight now\nand land a successful hit. Enemy will\ndie once he has 3 wounds.\n\n[click] to continue",
        "But it would be wise to use your\nfirst-strike advantage and kill him in\none blow, before he can retaliate!\n\n[click] to continue",
        "The yellow icon above the aim list\nindicatesyour stamina, a resource\nused to increase your weapon stats.\n\n[click] to continue",
        "\nUse \"-\" and \"+\" to the left\nfrom your weapon stats to throw\n3 stamina into your accuracy (green)",
        "\nAim to the face, since it's\nthe only wound that is instalty lethal.\nScroll to the bottom of the aim list\nusing [arrow_keys] or [mouse_wheel]",
        "\n\nNow use your weapon\n(click the \"sword\" icon) to start the\nfight, and watch him die...",
        "Take the loot now. use [ctrl]\n or [left_mouse] to access his\ninventory.\n\n[take steak] to continue",
        "To the right you can see the stats of\nthe weapon you are about to take.\nYou can't carry more than one weapon.\n\n[click] to continue",
        "\nFinally, use [space] or that arrow\n in the enemy's inventory to proceed."
    };
    Scripts scripts;
    // dice types: 2g, 1b, 1b, 1b, 3r, 2d
    // bad choice
    void Start() {
        scripts = FindObjectOfType<Scripts>();
        GetComponent<SpriteRenderer>().sprite = blackBox;
        tutorialText.text = tutorialTextList[0];
    }

    void OnMouseDown() {
        scripts.soundManager.PlayClip("click0");
        curIndex++;
        if (curIndex >= tutorialTextList.Count) {}
        else { 
            if (curIndex == 3) { 
                GetComponent<SpriteRenderer>().sprite = null;
                statText.text = "< These are your weapon stats."; 
            }
            else if (curIndex == 4) { statText.text = "< Accuracy\n< Speed\n< Damage\n< Parry"; }
            else { statText.text = ""; }
            tutorialText.text = tutorialTextList[curIndex]; 
        }
    }
}
