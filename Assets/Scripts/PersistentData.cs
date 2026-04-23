using System;

[Serializable]
public class PersistentData {
    public const int WeaponStatCount = 22;
    public const int DifficultyRunCount = 4;
    private static readonly int AlmanacWeaponCount = ItemManager.AlmanacWeaponOrder.Length;
    private static readonly int AlmanacItemCount = ItemManager.AlmanacItemOrder.Length;
    public int difficultyVersion;
    public int tsLevel;
    public int tsSub;
    public int tsWeaponAcc;
    public int tsWeaponSpd;
    public int tsWeaponDmg;
    public int tsWeaponDef;
    public string[] tsItemNames;
    public string[] tsItemTypes;
    public string[] tsItemMods;
    public int newCharNum;
    public string gameDifficulty;
    public bool endlessModeEnabled;
    public bool[] unlockedChars;
    public int gamesPlayed;
    public int highestLevel;
    public int highestSub;
    public int successfulRuns;
    public int[] successfulRunsByDifficulty;
    public int deaths;
    public int attacksParried;
    public int woundsReceived;
    public int woundsInflicted;
    public int[] woundsInflictedArr;
    public int[] weaponUses;
    public int enemiesSlain;
    public int staminaUsed;
    public int armorBroken;
    public int weaponsSwapped;
    public int turnsTaken;
    public int itemsFound;
    public int scrollsRead;
    public int potionsQuaffed;
    public int foodEaten;
    public int shurikensThrown;
    public int itemsTraded;
    public int itemsStolen;
    public int diceRerolled;
    public int diceDiscarded;
    public int[] itemUses;
    // almanac: one bool per canonical almanac entry (see ItemManager.AlmanacWeaponOrder / AlmanacItemOrder)
    public bool[] discoveredWeapons;
    public bool[] discoveredItems;
    public int[] discoveredWeaponCounts;
    public int[] discoveredItemCounts;
    public PersistentData() {
        difficultyVersion = DifficultyHelper.CurrentDifficultyVersion;
        tsLevel = -1;
        tsSub = -1;
        tsWeaponAcc = 0;
        tsWeaponSpd = 0;
        tsWeaponDmg = 0;
        tsWeaponDef = 0;
        tsItemNames = new string[9];
        tsItemTypes = new string[9];
        tsItemMods = new string[9];
        newCharNum = 0;
        gameDifficulty = DifficultyHelper.Normal;
        endlessModeEnabled = false;
        unlockedChars = new bool[4] { true, false, false, false };
        gamesPlayed = 0;
        highestLevel = 1;
        highestSub = 1;
        successfulRuns = 0;
        successfulRunsByDifficulty = new int[DifficultyRunCount];
        deaths = 0;
        attacksParried = 0;
        woundsReceived = 0;
        woundsInflicted = 0;
        woundsInflictedArr = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        weaponUses = new int[WeaponStatCount];
        weaponUses[9] = 1;
        enemiesSlain = 0;
        staminaUsed = 0;
        armorBroken = 0;
        weaponsSwapped = 0;
        turnsTaken = 0;
        itemsFound = 0;
        scrollsRead = 0;
        potionsQuaffed = 0;
        foodEaten = 0;
        shurikensThrown = 0;
        itemsTraded = 0;
        itemsStolen = 0;
        diceRerolled = 0;
        diceDiscarded = 0;
        itemUses = new int[AlmanacItemCount];
        discoveredWeapons = new bool[AlmanacWeaponCount];
        discoveredItems   = new bool[AlmanacItemCount];
        discoveredWeaponCounts = new int[AlmanacWeaponCount];
        discoveredItemCounts   = new int[AlmanacItemCount];
    }

    public void Normalize() {
        if (tsItemNames == null || tsItemNames.Length != 9) { tsItemNames = new string[9]; }
        if (tsItemTypes == null || tsItemTypes.Length != 9) { tsItemTypes = new string[9]; }
        if (tsItemMods == null || tsItemMods.Length != 9) { tsItemMods = new string[9]; }
        if (unlockedChars == null || unlockedChars.Length != 4) { unlockedChars = new bool[4] { true, false, false, false }; }
        if (woundsInflictedArr == null || woundsInflictedArr.Length != 8) { woundsInflictedArr = new[] { 0, 0, 0, 0, 0, 0, 0, 0 }; }

        if (successfulRunsByDifficulty == null || successfulRunsByDifficulty.Length != DifficultyRunCount) {
            int[] normalizedSuccessfulRuns = new int[DifficultyRunCount];
            bool hasExistingPerDifficultyData = false;
            if (successfulRunsByDifficulty != null) {
                int copyLength = Math.Min(successfulRunsByDifficulty.Length, normalizedSuccessfulRuns.Length);
                Array.Copy(successfulRunsByDifficulty, normalizedSuccessfulRuns, copyLength);
                for (int i = 0; i < copyLength; i++) {
                    if (successfulRunsByDifficulty[i] > 0) {
                        hasExistingPerDifficultyData = true;
                        break;
                    }
                }
            }

            if (!hasExistingPerDifficultyData && successfulRuns > 0) {
                normalizedSuccessfulRuns[GetDifficultyRunIndex(DifficultyHelper.Normal)] = successfulRuns;
            }

            successfulRunsByDifficulty = normalizedSuccessfulRuns;
        }

        successfulRuns = 0;
        for (int i = 0; i < successfulRunsByDifficulty.Length; i++) {
            successfulRuns += successfulRunsByDifficulty[i];
        }

        if (weaponUses == null || weaponUses.Length != WeaponStatCount) {
            int[] normalizedWeaponUses = new int[WeaponStatCount];
            if (weaponUses != null) {
                Array.Copy(weaponUses, normalizedWeaponUses, Math.Min(weaponUses.Length, normalizedWeaponUses.Length));
            }
            weaponUses = normalizedWeaponUses;
        }
        if (discoveredWeapons == null || discoveredWeapons.Length != AlmanacWeaponCount) {
            bool[] norm = new bool[AlmanacWeaponCount];
            if (discoveredWeapons != null) {
                Array.Copy(discoveredWeapons, norm, Math.Min(discoveredWeapons.Length, AlmanacWeaponCount));
            }
            discoveredWeapons = norm;
        }
        if (discoveredItems == null || discoveredItems.Length != AlmanacItemCount) {
            bool[] norm = new bool[AlmanacItemCount];
            if (discoveredItems != null) {
                Array.Copy(discoveredItems, norm, Math.Min(discoveredItems.Length, AlmanacItemCount));
            }
            discoveredItems = norm;
        }
        if (discoveredWeaponCounts == null || discoveredWeaponCounts.Length != AlmanacWeaponCount) {
            int[] norm = new int[AlmanacWeaponCount];
            if (discoveredWeaponCounts != null) {
                Array.Copy(discoveredWeaponCounts, norm, Math.Min(discoveredWeaponCounts.Length, AlmanacWeaponCount));
            }
            discoveredWeaponCounts = norm;
        }
        if (discoveredItemCounts == null || discoveredItemCounts.Length != AlmanacItemCount) {
            int[] norm = new int[AlmanacItemCount];
            if (discoveredItemCounts != null) {
                Array.Copy(discoveredItemCounts, norm, Math.Min(discoveredItemCounts.Length, AlmanacItemCount));
            }
            discoveredItemCounts = norm;
        }
        if (itemUses == null || itemUses.Length != AlmanacItemCount) {
            int[] norm = new int[AlmanacItemCount];
            if (itemUses != null) {
                Array.Copy(itemUses, norm, Math.Min(itemUses.Length, AlmanacItemCount));
            }
            itemUses = norm;
        }
    }

    public int GetSuccessfulRuns(string difficulty) {
        if (successfulRunsByDifficulty == null || successfulRunsByDifficulty.Length != DifficultyRunCount) {
            Normalize();
        }

        return successfulRunsByDifficulty[GetDifficultyRunIndex(difficulty)];
    }

    public void IncrementSuccessfulRuns(string difficulty) {
        if (successfulRunsByDifficulty == null || successfulRunsByDifficulty.Length != DifficultyRunCount) {
            Normalize();
        }

        successfulRunsByDifficulty[GetDifficultyRunIndex(difficulty)]++;
        successfulRuns++;
    }

    private static int GetDifficultyRunIndex(string difficulty) {
        string normalized = DifficultyHelper.Normalize(difficulty);
        int index = Array.IndexOf(DifficultyHelper.OrderedDifficulties, normalized);
        return index < 0 ? Array.IndexOf(DifficultyHelper.OrderedDifficulties, DifficultyHelper.Normal) : index;
    }
}
