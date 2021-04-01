using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using System.IO;

public class Statistics : MonoBehaviour {
    private string persistentPath = "persistentSave.txt";
    private string[] weaponNames = new string[] { "dagger", "flail", "hatchet", "mace", "maul", "montante", "rapier", "scimitar", "spear", "sword" };
    private WaitForSeconds tenthSecond = new WaitForSeconds(0.1f);
    private WaitForSeconds oneSecond = new WaitForSeconds(1f);
    private string baseText = "hold [space] to delete all data - this action is irrecoverable";
    private SoundManager soundManager;
    [SerializeField] private PersistentData persistentData;
    [SerializeField] private TextMeshProUGUI leftWhite;
    [SerializeField] private TextMeshProUGUI leftGray;
    [SerializeField] private TextMeshProUGUI favWeapon;
    [SerializeField] private TextMeshProUGUI rightWhite;
    [SerializeField] private TextMeshProUGUI rightGray;
    [SerializeField] private TextMeshProUGUI bottomText;
    private void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        persistentData = LoadPersistentData();
        ShowStatistics();
    }

    private void Update() { 
        if (Input.GetKeyDown(KeyCode.Space)) { 
            StartCoroutine(DataClearCountdown());
        }
    }

    private void ShowStatistics() { 
        leftWhite.text = $"\n{persistentData.gamesPlayed}\n\n\n\n\n{persistentData.attacksParried}\n{persistentData.woundsReceived}\n{persistentData.woundsInflicted}";
        leftGray.text = $"\n\n{persistentData.highestLevel}-{persistentData.highestSub}\n{persistentData.successfulRuns}\n{persistentData.deaths}\n\n\n\n\n{persistentData.woundsInflictedArr[0]}\n{persistentData.woundsInflictedArr[1]}\n{persistentData.woundsInflictedArr[2]}\n{persistentData.woundsInflictedArr[3]}\n{persistentData.woundsInflictedArr[4]}\n{persistentData.woundsInflictedArr[5]}\n{persistentData.woundsInflictedArr[6]}\n{persistentData.woundsInflictedArr[7]}";
        favWeapon.text = weaponNames[Array.IndexOf(persistentData.weaponUses, persistentData.weaponUses.Max())];
        rightWhite.text = $"\n\n\n\n\n\n{persistentData.enemiesSlain}\n{persistentData.staminaUsed}";
        rightGray.text = $"\n\n\n\n\n\n\n\n{persistentData.armorBroken}\n{persistentData.weaponsSwapped}\n{persistentData.scrollsRead}\n{persistentData.potionsQuaffed}\n{persistentData.foodEaten}\n{persistentData.shurikensThrown}\n{persistentData.itemsTraded}\n{persistentData.diceRerolled}\n{persistentData.diceDiscarded}";
    }

    /// <summary>
    /// Coroutine for the player to reset their stats with.
    /// </summary>
    private IEnumerator DataClearCountdown() {
        float time = 5f; 
        // total time htye have to hold down the bottom
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
            bottomText.text = "[done]";
            soundManager.PlayClip("click1");
            persistentData = new PersistentData();
            File.WriteAllText(persistentPath, JsonUtility.ToJson(persistentData));
            ShowStatistics();
            for (int i = 0; i < 5; i++) { yield return oneSecond; }
            // some tactile feedback, clearing the stats.
            bottomText.text = baseText;
        }
        else { bottomText.text = baseText; }
    }

    public PersistentData LoadPersistentData() { 
        if (File.Exists(persistentPath)) { return JsonUtility.FromJson<PersistentData>(File.ReadAllText(persistentPath)); }
        else { 
            Debug.Log($"no statistics found, so just created one!");
            File.WriteAllText(persistentPath, JsonUtility.ToJson(new PersistentData()));
            return JsonUtility.FromJson<PersistentData>(File.ReadAllText(persistentPath));
        }
    }
}
