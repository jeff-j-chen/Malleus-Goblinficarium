using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Tutorial : MonoBehaviour {
    [SerializeField] private Sprite blackBox;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] public int curIndex = 0;
    private Scripts scripts;
    public bool preventAttack = true;
    public bool isAnimating;
    private Coroutine mainScroll;
    private Coroutine statScroll;
    private List<string> tutorialTextList = new() {
        "Welcome to MALLEUS GOBLINIFICARIUM,\na realistic dice-based combat simulator!\n\nYes, a realistic game can contain goblins and scrolls.\n[click] to continue", 
        // ^ 0
        "You can navigate your inventory using\nleft and right [arrow_keys]\nor [left_mouse]\n\n[click] to continue", 
        // ^ 1
        "Press [enter] or [left_mouse] to use an item,\n[shift_enter] or [right_mouse] to drop it.\nYou can use or drop selected items only.\n\n[drop the scroll] to continue",
        // ^ 2
        "\n\n\n\n[click] to continue", 
        // ^ 3
        "\n\n\n\n[click] to continue", 
        // ^ 4
        "Accuracy (green) allows you to choose\ndifferent body parts (each applying some\nspecial debuff in damaged) as a target.\n\n[click] to continue", 
        // ^ 5
        "You can use up and down\n[arrow_keys] or [mouse_wheel]\nto adjust your aim.\n\n[click] to continue", 
        // ^ 6
        "As you invest into your accuracy,\nyou will have more options\nto choose from.\n\n[click] to continue", 
        // ^ 7
        "If your damage (red) is higher than\nthe enemy's parry (white), you will wound\nthe body part you are targeting.\n\n[click] to continue", 
        // ^ 8
        "Respectively, if your parry (white) isn\nhigher than the enemy's damage (red), his\nattack will deal no harm.\n\n[click] to continue", 
        // ^ 9
        "Speed (blue) defines who will strike first.\nIt matters because all debuffs are\napplied instantly.\n\n[click] to continue", 
        // ^ 10
        "Speed also defines who will start\nthe draft, a process of picking dice,\none by one.\n\n[click] to continue",
        // ^ 11
        "The die you pick increases\nthe corresponding combat stat.\nYour speed is higher, so choose one!\n\n[click + drag die] to continue", 
        // ^ 12
        "You can notice that your enemy has\ntaken a die as well.\nThis is how draft works.\n\n[pick 2 more dice] to continue",
        // ^ 13
        "Now your damage (7) is higher than\nenemy's parry (3), so you'll inflict a\nwound. You are also safe from his blow.\n\n[click] to continue", 
        // ^ 14
        "You have the same speed (3)\nas your enemy,which still means you'll\nstrike first.\n\n[click] to continue",
        // ^ 15
        "You can probably start the fight now\nand land a successful hit. Enemy will\ndie once he has 3 wounds.\n\n[click] to continue", 
        // ^ 16
        "But it would be wise to use your\nfirst-strike advantage and kill him in\none blow, before he can retaliate!\n\n[click] to continue", 
        // ^ 17
        "The yellow icon above the aim list\nindicates your stamina, a resource\nused to increase your weapon stats.\n\n[click] to continue", 
        // ^ 18
        "\nUse \"-\" and \"+\" to the left\nfrom your weapon stats to throw\n3 stamina into your accuracy (green)",
        // ^ 19
        "\nAim to the face, since it's\nthe only wound that is instantly lethal.\nScroll to the bottom of the aim list\nusing [arrow_keys] or [mouse_wheel]",
        // ^ 20
        "\n\nNow use your weapon\n(click the \"sword\" icon) to start the\nfight, and watch him die...",
        // ^ 21
        "Take the loot now. use [ctrl]\n or [left_mouse] to access his\ninventory.\n\n[take steak] to continue",
        // ^ 22
        "To the right you can see the stats of\nthe weapon you are about to take.\nYou can't carry more than one weapon.\n\n[click] to continue",
        // ^ 23
        "Finally, use the arrow in the\nenemy's inventory to proceed.\n\nThose are the very basics of\nMalleus Goblinificarium. \nYou'll learn more as you play more!"
        // ^ 24
    };

    private void Start() {
        scripts = FindObjectOfType<Scripts>();
        GetComponent<SpriteRenderer>().sprite = blackBox;
        mainScroll = StartCoroutine(TextAnimation(0));
    }

    private void OnMouseDown() {
        scripts.soundManager.PlayClip("click0");
        if (curIndex + 1 < tutorialTextList.Count) {
            // if within the bounds of the tutorial
            if (isAnimating) { 
                try { StopCoroutine(mainScroll); } catch { }
                try { StopCoroutine(statScroll); } catch { }
                isAnimating = false;
                // if animating stop animating (skip the text scroll)
                if (curIndex == 3) { 
                    GetComponent<SpriteRenderer>().sprite = null;
                    statText.text = "< These are your weapon stats."; 
                }
                else if (curIndex == 4) { statText.text = "< Accuracy\n< Speed\n< Damage\n< Parry"; }
                // some tutorial steps have other text to display
                tutorialText.text = tutorialTextList[curIndex];
                // show the full text
            }
            else {
                // animation has already finished playing
                if (curIndex is not (2 or 12 or 13 or 19 or 20 or 21 or 22)) { Increment(); }
                // some tutorial steps cannot be skipped through clicking, a specific player action needs tobe taken
            }
            if (curIndex == 12 && scripts.diceSummoner.existingDice.Count == 0) { scripts.diceSummoner.SummonDice(true, false);  }
            else if (curIndex == 21) { preventAttack = false; }
            else if (curIndex != 3 && curIndex != 4) { statText.text = ""; }
            // specific tutorials have specific necessary extra things to introduce or clear from the previous step
        }
    }

    public void Increment() { 
        curIndex++;
        try { StopCoroutine(mainScroll); } catch {}
        try { StopCoroutine(statScroll); } catch {}
        statText.text = "";
        tutorialText.text = "";
        // clear everything 
        if (curIndex == 3) { 
            GetComponent<SpriteRenderer>().sprite = null;
            statScroll = StartCoroutine(TextAnimation("< These are your weapon stats.")); 
        }
        else if (curIndex == 4) { statScroll = StartCoroutine(TextAnimation("< Accuracy\n< Speed\n< Damage\n< Parry")); }
        // some tutorial steps have extra text pointing to the stats
        mainScroll = StartCoroutine(TextAnimation(curIndex));
        // initialize the animation scroll of the text
    }

    private IEnumerator TextAnimation(int index) {
        isAnimating = true;
        for (int i = 0; i < tutorialTextList[index].Length; i++) { 
            tutorialText.text += tutorialTextList[index][i];
            yield return scripts.delays[0.02f];
        }
        isAnimating = false;
        // self explanatory, just add the text step by step for a specific tutorial index
        if (curIndex == 21) { preventAttack = false; }
        // enable the player to attack after the text is done scrolling
    }

    private IEnumerator TextAnimation(string str) {
        isAnimating = true;
        foreach (char c in str) {
            statText.text += c;
            yield return scripts.delays[0.02f];
        }
        isAnimating = false;
        // same as above, but instead with a given text
    }
}
