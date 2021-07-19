using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
public class Statistics : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI leftWhite;
    [SerializeField] private TextMeshProUGUI leftGray;
    [SerializeField] private TextMeshProUGUI favWeapon;
    [SerializeField] private TextMeshProUGUI rightWhite;
    [SerializeField] private TextMeshProUGUI rightGray;
    [SerializeField] private TextMeshProUGUI bottomText;
    private readonly string[] weaponNames = 
        { "dagger", "flail", "hatchet", "mace", "maul", "montante", "rapier", "scimitar", "spear", "sword" };
    private readonly WaitForSeconds tenthSecond = new(0.1f);
    private readonly WaitForSeconds oneSecond = new(1f);
    private readonly string baseText = "hold [space] to delete all data - this action is irrecoverable";
    private SoundManager soundManager;
    
    private void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        ShowStatistics();
    }

    private void Update() { 
        if (Input.GetKeyDown(KeyCode.Space)) { 
            StartCoroutine(DataClearCountdown());
        }
    }

    private void ShowStatistics() { 
        leftWhite.text = $"\n{Save.persistent.gamesPlayed}\n\n\n\n\n{Save.persistent.attacksParried}\n{Save.persistent.woundsReceived}\n{Save.persistent.woundsInflicted}";
        leftGray.text = $"\n\n{Save.persistent.highestLevel}-{Save.persistent.highestSub}\n{Save.persistent.successfulRuns}\n{Save.persistent.deaths}\n\n\n\n\n{Save.persistent.woundsInflictedArr[0]}\n{Save.persistent.woundsInflictedArr[1]}\n{Save.persistent.woundsInflictedArr[2]}\n{Save.persistent.woundsInflictedArr[3]}\n{Save.persistent.woundsInflictedArr[4]}\n{Save.persistent.woundsInflictedArr[5]}\n{Save.persistent.woundsInflictedArr[6]}\n{Save.persistent.woundsInflictedArr[7]}";
        favWeapon.text = weaponNames[Array.IndexOf(Save.persistent.weaponUses, Save.persistent.weaponUses.Max())];
        rightWhite.text = $"\n\n\n\n\n\n{Save.persistent.enemiesSlain}\n{Save.persistent.staminaUsed}";
        rightGray.text = $"\n\n\n\n\n\n\n\n{Save.persistent.armorBroken}\n{Save.persistent.weaponsSwapped}\n{Save.persistent.scrollsRead}\n{Save.persistent.potionsQuaffed}\n{Save.persistent.foodEaten}\n{Save.persistent.shurikensThrown}\n{Save.persistent.itemsTraded}\n{Save.persistent.diceRerolled}\n{Save.persistent.diceDiscarded}";
    }

    /// <summary>
    /// Coroutine for the player to reset their stats with.
    /// </summary>
    private IEnumerator DataClearCountdown() {
        float time = 5f; 
        // total time the reset button must be held down
        for (int i = 0; i < 50; i++) { 
            // 50 times, decrease by 0.1s
            if (i % 10 == 0) {
                bottomText.text = $"[{Mathf.Round(time)}]";
                soundManager.PlayClip("click0");
                // whole number, so display it
            }
            if (!Input.GetKey(KeyCode.Space)) { break; }
            // if they released spacebar, stop the countdown
            yield return tenthSecond;
            time -= 0.1f;
        }
        yield return tenthSecond;
        if (time <= 0.1f) { 
            // player held it all the way through
            Save.persistent = new PersistentData();
            Save.SavePersistent();
            bottomText.text = "[done]";
            soundManager.PlayClip("click1");
            ShowStatistics();
            for (int i = 0; i < 5; i++) { yield return oneSecond; }
            // some tactile feedback, clearing the stats.
            bottomText.text = baseText;
        }
        else { bottomText.text = baseText; }
    }
}
