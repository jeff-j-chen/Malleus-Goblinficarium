using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyAI {
    private static readonly string[] Stats = { "green", "blue", "red", "white" };
    private static readonly string[] Targets = { "chest", "guts", "knee", "hip", "head", "hand", "armpits", "face" };
    private static readonly Dictionary<string, int> DefaultDieRanks = new() {
        { "yellow6", 0 }, { "red6", 1 }, { "white6", 2 }, { "yellow5", 3 }, { "red5", 4 }, { "white5", 5 },
        { "yellow4", 6 }, { "red4", 7 }, { "white4", 8 }, { "yellow3", 9 }, { "red3", 10 }, { "white3", 11 },
        { "green6", 12 }, { "yellow2", 13 }, { "red2", 14 }, { "white2", 15 }, { "yellow1", 16 }, { "red1", 17 },
        { "white1", 18 }, { "green5", 19 }, { "green4", 20 }, { "blue6", 21 }, { "green3", 22 }, { "blue5", 23 },
        { "blue4", 24 }, { "green2", 25 }, { "blue3", 26 }, { "green1", 27 }, { "blue2", 28 }, { "blue1", 29 },
    };

    private sealed class Plan {
        public int TargetIndex;
        public float Score = float.NegativeInfinity;
        public Dictionary<string, int> Stamina = NewStatDictionary();
        public Dictionary<Dice, string> YellowAssignments = new();
    }

    private sealed class SimState {
        public int PlayerAim;
        public int PlayerSpd;
        public int PlayerAtt;
        public int PlayerDef;
        public int EnemyAim;
        public int EnemySpd;
        public int EnemyAtt;
        public int EnemyDef;
        public int PlayerWoundCount;
        public int EnemyWoundCount;
        public int PlayerAddedGreen;
        public int PlayerAddedBlue;
        public int PlayerAddedRed;
        public int PlayerAddedWhite;
        public int EnemyAddedGreen;
        public int EnemyAddedBlue;
        public int EnemyAddedRed;
        public int EnemyAddedWhite;
        public int PlayerGreenDiceCount;
        public int PlayerBlueDiceCount;
        public int PlayerRedDiceCount;
        public int PlayerWhiteDiceCount;
        public int EnemyGreenDiceCount;
        public int EnemyBlueDiceCount;
        public int EnemyRedDiceCount;
        public int EnemyWhiteDiceCount;
        public int PlayerGreenDiceSum;
        public int PlayerBlueDiceSum;
        public int PlayerRedDiceSum;
        public int PlayerWhiteDiceSum;
        public int EnemyGreenDiceSum;
        public int EnemyBlueDiceSum;
        public int EnemyRedDiceSum;
        public int EnemyWhiteDiceSum;
        public bool PlayerHasArmor;
        public bool PlayerHasDodgy;
        public bool PlayerHasMaul;
        public bool EnemyIsLich;
        public bool PlayerSpeedLockedHigh;
        public bool EnemySpeedLockedHigh;
        public bool PlayerImmuneToWounds;
        public float Bonus;

        public SimState Clone() {
            return (SimState)MemberwiseClone();
        }
    }

    public static void ChooseBestDie(Scripts s) {
        List<Dice> availableDice = s.diceSummoner.existingDice
            .Select(diceObject => diceObject.GetComponent<Dice>())
            .Where(dice => !dice.isAttached)
            .ToList();

        if (availableDice.Count == 0) { return; }

        Dice chosenDie = DifficultyHelper.UsesAdvancedEnemyAI(Save.persistent.gameDifficulty)
            ? ChooseAdvancedDraftDie(s, availableDice)
            : ChooseDefaultDraftDie(availableDice);

        if (chosenDie == null) { return; }

        AttachChosenDie(s, chosenDie);
    }

    public static void ApplyLivePlan(Scripts s) {
        if (!CanPlan(s)) { return; }

        Plan plan = BuildPlan(s);
        ApplyPlan(s, plan);
    }

    public static int GetBestTargetIndex(Scripts s) {
        if (!CanTarget(s)) { return 0; }

        if (DifficultyHelper.UsesAdvancedEnemyAI(Save.persistent.gameDifficulty) && s.diceSummoner.CountUnattachedDice() == 0) {
            return BuildPlan(s).TargetIndex;
        }

        return GetDefaultTargetIndex(s, s.statSummoner.SumOfStat("green", "enemy"));
    }

    public static Dice GetBestPlayerDieToDiscard(Scripts s, List<Dice> playerDice) {
        if (playerDice == null || playerDice.Count == 0) { return null; }
        if (!DifficultyHelper.UsesAdvancedEnemyAI(Save.persistent.gameDifficulty)) {
            return playerDice
                .OrderBy(dice => GetDefaultRank(dice))
                .FirstOrDefault();
        }

        Dice bestDie = null;
        float bestScore = float.NegativeInfinity;
        foreach (Dice dice in playerDice) {
            float score = GetDiscardScore(s, dice);
            if (score > bestScore) {
                bestScore = score;
                bestDie = dice;
            }
        }

        return bestDie ?? playerDice[0];
    }

    private static bool CanPlan(Scripts s) {
        return s != null
            && s.enemy != null
            && s.player != null
            && s.diceSummoner != null
            && s.statSummoner != null
            && s.turnManager != null
            && !s.player.isDead
            && !Save.game.enemyIsDead
            && s.enemy.enemyName.text is not "Merchant" and not "Blacksmith" and not "Tombstone"
            && s.diceSummoner.CountUnattachedDice() == 0;
    }

    private static bool CanTarget(Scripts s) {
        return s != null
            && s.enemy != null
            && s.player != null
            && s.turnManager != null
            && s.enemy.enemyName.text is not "Merchant" and not "Blacksmith" and not "Tombstone";
    }

    private static Plan BuildPlan(Scripts s) {
        string difficulty = DifficultyHelper.Normalize(Save.persistent.gameDifficulty);
        if (DifficultyHelper.IsEasy(difficulty)) { return BuildEasyPlan(s); }
        if (DifficultyHelper.IsNormal(difficulty)) { return BuildNormalPlan(s); }
        return BuildAdvancedPlan(s);
    }

    private static Plan BuildEasyPlan(Scripts s) {
        Plan plan = CreateBaselinePlan(s);
        int remainingStamina = s.enemy.stamina;
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int enemyAtt = GetEnemyStatWithPlan(s, plan, "red");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemyDef = GetEnemyStatWithPlan(s, plan, "white");

        if (enemyAtt <= playerDef) {
            int needed = playerDef - enemyAtt + 1;
            int spend = Mathf.Min(needed, remainingStamina);
            plan.Stamina["red"] += spend;
            remainingStamina -= spend;
        }

        enemyDef = GetEnemyStatWithPlan(s, plan, "white");
        if (remainingStamina > 0 && playerAtt > enemyDef && s.statSummoner.SumOfStat("green", "player") >= 0) {
            int needed = playerAtt - enemyDef;
            int spend = Mathf.Min(needed, remainingStamina);
            plan.Stamina["white"] += spend;
        }

        return plan;
    }

    private static Plan BuildNormalPlan(Scripts s) {
        Plan plan = CreateBaselinePlan(s);
        int remainingStamina = s.enemy.stamina;
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int enemyAtt = GetEnemyStatWithPlan(s, plan, "red");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int enemySpd = GetEnemyStatWithPlan(s, plan, "blue");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemyDef = GetEnemyStatWithPlan(s, plan, "white");

        if (enemyAtt <= playerDef && enemyAtt + remainingStamina > playerDef) {
            int spend = playerDef - enemyAtt + 1;
            plan.Stamina["red"] += spend;
            remainingStamina -= spend;
            enemyAtt += spend;
        }

        if (enemyAtt > playerDef && playerSpd >= enemySpd && playerAtt > enemyDef && enemySpd + remainingStamina > playerSpd) {
            int spend = playerSpd - enemySpd + 1;
            plan.Stamina["blue"] += spend;
            remainingStamina -= spend;
        }

        enemyDef = GetEnemyStatWithPlan(s, plan, "white");
        if (playerAtt > enemyDef && s.statSummoner.SumOfStat("green", "player") >= 0 && enemyDef + remainingStamina >= playerAtt) {
            int spend = playerAtt - enemyDef;
            plan.Stamina["white"] += spend;
        }

        return plan;
    }

    private static Plan BuildAdvancedPlan(Scripts s) {
        List<Dice> yellowDice = GetEnemyYellowDice(s).ToList();
        string[] yellowAssignments = new string[yellowDice.Count];
        Dictionary<string, int> yellowTotals = NewStatDictionary();
        Dictionary<string, int> yellowCounts = NewStatDictionary();
        Plan bestPlan = null;
        bool canUseStamina = !s.enemy.woundList.Contains("hip") || s.enemy.enemyName.text == "Lich";

        void SearchYellow(int index) {
            if (index >= yellowDice.Count) {
                SearchStamina(NewStatDictionary(), 0, s.enemy.stamina);
                return;
            }

            foreach (string stat in Stats) {
                yellowAssignments[index] = stat;
                yellowTotals[stat] += yellowDice[index].diceNum;
                yellowCounts[stat]++;
                SearchYellow(index + 1);
                yellowTotals[stat] -= yellowDice[index].diceNum;
                yellowCounts[stat]--;
            }
        }

        void SearchStamina(Dictionary<string, int> staminaPlan, int statIndex, int remaining) {
            if (!canUseStamina) {
                for (int i = 0; i < Stats.Length; i++) { staminaPlan[Stats[i]] = 0; }
                EvaluateAllTargets(staminaPlan);
                return;
            }

            if (statIndex >= Stats.Length - 1) {
                staminaPlan[Stats[statIndex]] = remaining;
                EvaluateAllTargets(staminaPlan);
                return;
            }

            for (int spend = 0; spend <= remaining; spend++) {
                staminaPlan[Stats[statIndex]] = spend;
                SearchStamina(staminaPlan, statIndex + 1, remaining - spend);
            }
        }

        void EvaluateAllTargets(Dictionary<string, int> staminaPlan) {
            int maxAim = s.enemy.stats["green"]
                + GetFixedEnemyDiceSum(s, "green")
                + yellowTotals["green"]
                + staminaPlan["green"];
            int maxTarget = Mathf.Clamp(maxAim, 0, 7);

            for (int targetIndex = 0; targetIndex <= maxTarget; targetIndex++) {
                float score = EvaluateAdvancedState(s, targetIndex, yellowTotals, yellowCounts, staminaPlan);
                if (bestPlan != null && score <= bestPlan.Score) { continue; }

                Plan candidate = CreateBaselinePlan(s);
                candidate.TargetIndex = targetIndex;
                candidate.Score = score;
                foreach (string stat in Stats) {
                    candidate.Stamina[stat] = staminaPlan[stat];
                }
                for (int i = 0; i < yellowDice.Count; i++) {
                    candidate.YellowAssignments[yellowDice[i]] = yellowAssignments[i];
                }
                bestPlan = candidate;
            }
        }

        SearchYellow(0);
        return bestPlan ?? CreateBaselinePlan(s);
    }

    private static float EvaluateAdvancedState(
        Scripts s,
        int targetIndex,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        Dictionary<string, int> staminaPlan
    ) {
        SimState state = CreateSimulationState(s, yellowTotals, yellowCounts, staminaPlan);
        string playerTarget = Targets[Mathf.Clamp(s.player.targetIndex, 0, Targets.Length - 1)];
        string enemyTarget = Targets[targetIndex];
        int spentStamina = staminaPlan.Values.Sum();
        bool enemyActsFirst = state.EnemySpeedLockedHigh || (!state.PlayerSpeedLockedHigh && state.EnemySpd > state.PlayerSpd);
        bool playerActsFirst = state.PlayerSpeedLockedHigh || (!state.EnemySpeedLockedHigh && state.PlayerSpd >= state.EnemySpd);
        bool playerCanHit = state.PlayerAim >= 0 && state.PlayerAtt > state.EnemyDef;
        bool enemyCanHit = state.EnemyAim >= 0 && state.EnemyAtt > state.PlayerDef;
        bool playerKills = PlayerWouldKillEnemy(s, state, playerTarget, playerCanHit);
        bool enemyKills = EnemyWouldKillPlayer(state, enemyTarget, enemyCanHit);
        float score = state.Bonus;

        if (enemyActsFirst) {
            if (enemyCanHit) {
                score += 1200f;
                if (enemyKills && !state.PlayerHasArmor && !state.PlayerHasDodgy) { score += 100000f; }

                SimState afterEnemyHit = state.Clone();
                ApplyWoundToPlayer(afterEnemyHit, enemyTarget, s);
                bool playerStillHits = !afterEnemyHit.PlayerImmuneToWounds && afterEnemyHit.PlayerAim >= 0 && afterEnemyHit.PlayerAtt > afterEnemyHit.EnemyDef;
                bool playerStillKills = playerStillHits && PlayerWouldKillEnemy(s, afterEnemyHit, playerTarget, true);
                if (!playerStillHits) { score += 2500f; }
                if (!playerStillKills) { score += 1800f; }
                if (state.PlayerHasArmor) { score -= 800f; }
                if (state.PlayerHasDodgy) { score -= 1000f; }
                score += GetTargetUtility(s, enemyTarget, afterEnemyHit, onPlayer:true);
            }

            if (playerCanHit) {
                score -= playerKills ? 90000f : 1400f;
                score -= GetPlayerThreatUtility(playerTarget, state);
            }
        }
        else if (playerActsFirst) {
            if (playerCanHit) {
                SimState afterPlayerHit = state.Clone();
                if (!afterPlayerHit.EnemyIsLich) { ApplyWoundToEnemy(afterPlayerHit, playerTarget, s); }
                if (playerKills) {
                    score -= 95000f;
                }
                else {
                    score -= 2500f;
                    bool enemyStillHits = afterPlayerHit.EnemyAim >= 0 && afterPlayerHit.EnemyAtt > afterPlayerHit.PlayerDef;
                    bool enemyStillKills = enemyStillHits && EnemyWouldKillPlayer(afterPlayerHit, enemyTarget, true);
                    if (!enemyStillHits) { score -= 1200f; }
                    if (!enemyStillKills && enemyKills) { score -= 900f; }
                }
            }

            if (enemyCanHit) {
                score += 600f;
                score += GetTargetUtility(s, enemyTarget, state, onPlayer:true);
                if (enemyKills && !state.PlayerHasArmor && !state.PlayerHasDodgy) { score += 2200f; }
            }
        }

        if (!enemyCanHit) { score -= 900f; }
        if (enemyTarget == "face" && state.PlayerHasArmor) { score -= 600f; }
        if (targetIndex < 7 && s.player.woundList.Contains(enemyTarget)) { score -= 350f; }
        score -= spentStamina * 35f;
        score -= staminaPlan["white"] * 4f;
        score += state.EnemyAtt * 3f + state.EnemySpd * 2f + state.EnemyAim * 1.5f + state.EnemyDef;
        score -= state.PlayerAtt * 1.2f;
        return score;
    }

    private static Plan CreateBaselinePlan(Scripts s) {
        Plan plan = new() {
            TargetIndex = GetDefaultTargetIndex(s, s.statSummoner.SumOfStat("green", "enemy"))
        };

        foreach (Dice yellowDie in GetEnemyYellowDice(s)) {
            plan.YellowAssignments[yellowDie] = yellowDie.statAddedTo == string.Empty ? "red" : yellowDie.statAddedTo;
        }

        return plan;
    }

    private static SimState CreateSimulationState(
        Scripts s,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        Dictionary<string, int> staminaPlan
    ) {
        SimState state = new() {
            PlayerAim = s.statSummoner.SumOfStat("green", "player"),
            PlayerSpd = s.statSummoner.SumOfStat("blue", "player"),
            PlayerAtt = s.statSummoner.SumOfStat("red", "player"),
            PlayerDef = s.statSummoner.SumOfStat("white", "player"),
            EnemyAim = s.enemy.stats["green"] + GetFixedEnemyDiceSum(s, "green") + yellowTotals["green"] + staminaPlan["green"],
            EnemySpd = s.enemy.stats["blue"] + GetFixedEnemyDiceSum(s, "blue") + yellowTotals["blue"] + staminaPlan["blue"],
            EnemyAtt = s.enemy.stats["red"] + GetFixedEnemyDiceSum(s, "red") + yellowTotals["red"] + staminaPlan["red"],
            EnemyDef = s.enemy.stats["white"] + GetFixedEnemyDiceSum(s, "white") + yellowTotals["white"] + staminaPlan["white"],
            PlayerWoundCount = s.player.woundList.Count,
            EnemyWoundCount = s.enemy.woundList.Count,
            PlayerAddedGreen = s.statSummoner.addedPlayerStamina["green"],
            PlayerAddedBlue = s.statSummoner.addedPlayerStamina["blue"],
            PlayerAddedRed = s.statSummoner.addedPlayerStamina["red"],
            PlayerAddedWhite = s.statSummoner.addedPlayerStamina["white"],
            EnemyAddedGreen = staminaPlan["green"],
            EnemyAddedBlue = staminaPlan["blue"],
            EnemyAddedRed = staminaPlan["red"],
            EnemyAddedWhite = staminaPlan["white"],
            PlayerGreenDiceCount = GetDiceCount(s.statSummoner.addedPlayerDice["green"]),
            PlayerBlueDiceCount = GetDiceCount(s.statSummoner.addedPlayerDice["blue"]),
            PlayerRedDiceCount = GetDiceCount(s.statSummoner.addedPlayerDice["red"]),
            PlayerWhiteDiceCount = GetDiceCount(s.statSummoner.addedPlayerDice["white"]),
            PlayerGreenDiceSum = GetDiceSum(s.statSummoner.addedPlayerDice["green"]),
            PlayerBlueDiceSum = GetDiceSum(s.statSummoner.addedPlayerDice["blue"]),
            PlayerRedDiceSum = GetDiceSum(s.statSummoner.addedPlayerDice["red"]),
            PlayerWhiteDiceSum = GetDiceSum(s.statSummoner.addedPlayerDice["white"]),
            EnemyGreenDiceCount = GetFixedEnemyDiceCount(s, "green") + yellowCounts["green"],
            EnemyBlueDiceCount = GetFixedEnemyDiceCount(s, "blue") + yellowCounts["blue"],
            EnemyRedDiceCount = GetFixedEnemyDiceCount(s, "red") + yellowCounts["red"],
            EnemyWhiteDiceCount = GetFixedEnemyDiceCount(s, "white") + yellowCounts["white"],
            EnemyGreenDiceSum = GetFixedEnemyDiceSum(s, "green") + yellowTotals["green"],
            EnemyBlueDiceSum = GetFixedEnemyDiceSum(s, "blue") + yellowTotals["blue"],
            EnemyRedDiceSum = GetFixedEnemyDiceSum(s, "red") + yellowTotals["red"],
            EnemyWhiteDiceSum = GetFixedEnemyDiceSum(s, "white") + yellowTotals["white"],
            PlayerHasArmor = s.itemManager.PlayerHas("armor"),
            PlayerHasDodgy = Save.game.isDodgy,
            PlayerHasMaul = s.itemManager.PlayerHasWeapon("maul"),
            EnemyIsLich = s.enemy.enemyName.text == "Lich",
            PlayerSpeedLockedHigh = s.enemy.woundList.Contains("knee") || (s.itemManager.PlayerHasWeapon("spear") && s.itemManager.PlayerHasLegendary()),
            EnemySpeedLockedHigh = s.player.woundList.Contains("knee"),
        };

        if (state.PlayerHasArmor) { state.Bonus -= 150f; }
        if (s.player.targetIndex == 6 && state.PlayerRedDiceSum > 0) { state.Bonus -= state.PlayerRedDiceSum * 12f; }
        if (s.player.targetIndex == 4 && s.statSummoner.addedEnemyDice.Sum(pair => pair.Value.Count) > 0) { state.Bonus -= 180f; }
        return state;
    }

    private static bool PlayerWouldKillEnemy(Scripts s, SimState state, string playerTarget, bool playerCanHit) {
        if (!playerCanHit) { return false; }
        if (state.PlayerHasMaul) { return true; }
        if (playerTarget == "face" && state.PlayerAim >= 7 && s.enemy.enemyName.text != "Lich") { return true; }
        return !s.enemy.woundList.Contains(playerTarget) && state.EnemyWoundCount >= 2;
    }

    private static bool EnemyWouldKillPlayer(SimState state, string enemyTarget, bool enemyCanHit) {
        if (!enemyCanHit) { return false; }
        if (enemyTarget == "face" && state.EnemyAim >= 7) { return true; }
        return state.PlayerWoundCount >= 2;
    }

    private static void ApplyWoundToPlayer(SimState state, string target, Scripts s) {
        switch (target) {
            case "guts":
                state.PlayerAim -= state.PlayerGreenDiceCount;
                state.PlayerSpd -= state.PlayerBlueDiceCount;
                state.PlayerAtt -= state.PlayerRedDiceCount;
                state.PlayerDef -= state.PlayerWhiteDiceCount;
                break;
            case "knee":
                state.EnemySpeedLockedHigh = true;
                break;
            case "hip":
                state.PlayerAim -= state.PlayerAddedGreen;
                state.PlayerSpd -= state.PlayerAddedBlue;
                state.PlayerAtt -= state.PlayerAddedRed;
                state.PlayerDef -= state.PlayerAddedWhite;
                state.PlayerAddedGreen = 0;
                state.PlayerAddedBlue = 0;
                state.PlayerAddedRed = 0;
                state.PlayerAddedWhite = 0;
                break;
            case "head":
                ApplyBestPlayerDiscard(state, s);
                break;
            case "hand":
                state.PlayerDef -= state.PlayerWhiteDiceSum;
                state.PlayerWhiteDiceCount = 0;
                state.PlayerWhiteDiceSum = 0;
                break;
            case "armpits":
                state.PlayerAtt -= state.PlayerRedDiceSum;
                state.PlayerRedDiceCount = 0;
                state.PlayerRedDiceSum = 0;
                break;
            case "chest":
                state.Bonus -= 650f;
                break;
            case "face":
                break;
        }
    }

    private static void ApplyWoundToEnemy(SimState state, string target, Scripts s) {
        switch (target) {
            case "guts":
                state.EnemyAim -= state.EnemyGreenDiceCount;
                state.EnemySpd -= state.EnemyBlueDiceCount;
                state.EnemyAtt -= state.EnemyRedDiceCount;
                state.EnemyDef -= state.EnemyWhiteDiceCount;
                break;
            case "knee":
                state.PlayerSpeedLockedHigh = true;
                break;
            case "hip":
                state.EnemyAim -= state.EnemyAddedGreen;
                state.EnemySpd -= state.EnemyAddedBlue;
                state.EnemyAtt -= state.EnemyAddedRed;
                state.EnemyDef -= state.EnemyAddedWhite;
                state.EnemyAddedGreen = 0;
                state.EnemyAddedBlue = 0;
                state.EnemyAddedRed = 0;
                state.EnemyAddedWhite = 0;
                break;
            case "head":
                ApplyBestEnemyDiscard(state);
                break;
            case "hand":
                state.EnemyDef -= state.EnemyWhiteDiceSum;
                state.EnemyWhiteDiceCount = 0;
                state.EnemyWhiteDiceSum = 0;
                break;
            case "armpits":
                state.EnemyAtt -= state.EnemyRedDiceSum;
                state.EnemyRedDiceCount = 0;
                state.EnemyRedDiceSum = 0;
                break;
            case "chest":
                state.Bonus -= 900f;
                break;
            case "face":
                break;
        }
    }

    private static void ApplyBestPlayerDiscard(SimState state, Scripts s) {
        List<(string stat, int value, float weight)> options = new() {
            ("red", state.PlayerRedDiceSum > 0 ? GetLargestPlayerDie(s, "red") : 0, 6f),
            ("white", state.PlayerWhiteDiceSum > 0 ? GetLargestPlayerDie(s, "white") : 0, 3f),
            ("blue", state.PlayerBlueDiceSum > 0 ? GetLargestPlayerDie(s, "blue") : 0, 4f),
            ("green", state.PlayerGreenDiceSum > 0 ? GetLargestPlayerDie(s, "green") : 0, 4.5f),
        };
        (string stat, int value, float weight) best = options.OrderByDescending(option => option.value * option.weight).First();
        RemoveValueFromPlayerState(state, best.stat, best.value);
    }

    private static void ApplyBestEnemyDiscard(SimState state) {
        List<(string stat, int value, float weight)> options = new() {
            ("red", state.EnemyRedDiceSum > 0 ? Mathf.CeilToInt((float)state.EnemyRedDiceSum / Mathf.Max(1, state.EnemyRedDiceCount)) : 0, 6f),
            ("white", state.EnemyWhiteDiceSum > 0 ? Mathf.CeilToInt((float)state.EnemyWhiteDiceSum / Mathf.Max(1, state.EnemyWhiteDiceCount)) : 0, 4f),
            ("blue", state.EnemyBlueDiceSum > 0 ? Mathf.CeilToInt((float)state.EnemyBlueDiceSum / Mathf.Max(1, state.EnemyBlueDiceCount)) : 0, 5f),
            ("green", state.EnemyGreenDiceSum > 0 ? Mathf.CeilToInt((float)state.EnemyGreenDiceSum / Mathf.Max(1, state.EnemyGreenDiceCount)) : 0, 4.5f),
        };
        (string stat, int value, float weight) best = options.OrderByDescending(option => option.value * option.weight).First();
        RemoveValueFromEnemyState(state, best.stat, best.value);
    }

    private static void RemoveValueFromPlayerState(SimState state, string stat, int value) {
        if (value <= 0) { return; }
        switch (stat) {
            case "green": state.PlayerAim -= value; state.PlayerGreenDiceSum -= value; state.PlayerGreenDiceCount = Mathf.Max(0, state.PlayerGreenDiceCount - 1); break;
            case "blue": state.PlayerSpd -= value; state.PlayerBlueDiceSum -= value; state.PlayerBlueDiceCount = Mathf.Max(0, state.PlayerBlueDiceCount - 1); break;
            case "red": state.PlayerAtt -= value; state.PlayerRedDiceSum -= value; state.PlayerRedDiceCount = Mathf.Max(0, state.PlayerRedDiceCount - 1); break;
            case "white": state.PlayerDef -= value; state.PlayerWhiteDiceSum -= value; state.PlayerWhiteDiceCount = Mathf.Max(0, state.PlayerWhiteDiceCount - 1); break;
        }
    }

    private static void RemoveValueFromEnemyState(SimState state, string stat, int value) {
        if (value <= 0) { return; }
        switch (stat) {
            case "green": state.EnemyAim -= value; state.EnemyGreenDiceSum -= value; state.EnemyGreenDiceCount = Mathf.Max(0, state.EnemyGreenDiceCount - 1); break;
            case "blue": state.EnemySpd -= value; state.EnemyBlueDiceSum -= value; state.EnemyBlueDiceCount = Mathf.Max(0, state.EnemyBlueDiceCount - 1); break;
            case "red": state.EnemyAtt -= value; state.EnemyRedDiceSum -= value; state.EnemyRedDiceCount = Mathf.Max(0, state.EnemyRedDiceCount - 1); break;
            case "white": state.EnemyDef -= value; state.EnemyWhiteDiceSum -= value; state.EnemyWhiteDiceCount = Mathf.Max(0, state.EnemyWhiteDiceCount - 1); break;
        }
    }

    private static float GetTargetUtility(Scripts s, string target, SimState state, bool onPlayer) {
        return target switch {
            "guts" => onPlayer ? 800f + state.PlayerRedDiceCount * 160f : -600f,
            "knee" => onPlayer ? 1100f : -700f,
            "hip" => onPlayer ? 950f + (s.player.stamina * 20f) : -900f,
            "head" => onPlayer ? 1000f : -950f,
            "hand" => onPlayer ? 650f + state.PlayerWhiteDiceSum * 30f : -700f,
            "armpits" => onPlayer ? 1400f + state.PlayerRedDiceSum * 35f : -850f,
            "chest" => onPlayer ? -300f : -800f,
            "face" => onPlayer ? 100000f : -100000f,
            _ => 0f,
        };
    }

    private static float GetPlayerThreatUtility(string playerTarget, SimState state) {
        return playerTarget switch {
            "guts" => 900f + state.EnemyRedDiceCount * 130f,
            "knee" => 1150f,
            "hip" => 1200f + (state.EnemyAddedGreen + state.EnemyAddedBlue + state.EnemyAddedRed + state.EnemyAddedWhite) * 50f,
            "head" => 1000f,
            "hand" => 700f + state.EnemyWhiteDiceSum * 28f,
            "armpits" => 1500f + state.EnemyRedDiceSum * 32f,
            "chest" => 700f,
            "face" => 100000f,
            _ => 0f,
        };
    }

    private static void ApplyPlan(Scripts s, Plan plan) {
        if (plan == null) { return; }

        int refunded = s.statSummoner.addedEnemyStamina.Values.Sum();
        foreach (string stat in Stats) {
            s.statSummoner.addedEnemyStamina[stat] = 0;
        }
        s.enemy.stamina += refunded;

        foreach (string stat in Stats) {
            List<Dice> toRemove = s.statSummoner.addedEnemyDice[stat]
                .Where(dice => dice != null && dice.diceType == "yellow")
                .ToList();
            foreach (Dice yellowDie in toRemove) {
                s.statSummoner.addedEnemyDice[stat].Remove(yellowDie);
            }
        }

        foreach (Dice yellowDie in GetEnemyYellowDice(s)) {
            string targetStat = plan.YellowAssignments.TryGetValue(yellowDie, out string assignedStat)
                ? assignedStat
                : (yellowDie.statAddedTo == string.Empty ? "red" : yellowDie.statAddedTo);
            yellowDie.statAddedTo = targetStat;
            if (!s.statSummoner.addedEnemyDice[targetStat].Contains(yellowDie)) {
                s.statSummoner.addedEnemyDice[targetStat].Add(yellowDie);
            }
        }

        foreach (string stat in Stats) {
            s.statSummoner.addedEnemyStamina[stat] = plan.Stamina[stat];
        }
        s.enemy.stamina = Mathf.Max(0, s.enemy.stamina - plan.Stamina.Values.Sum());
        s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
        s.enemy.targetIndex = plan.TargetIndex;
        s.statSummoner.SummonStats();
        RepositionEnemyDice(s);
        s.turnManager.SetTargetOf("enemy");
        Save.game.enemyStamina = s.enemy.stamina + s.statSummoner.addedEnemyStamina.Values.Sum();
        if (s.tutorial == null) { Save.SaveGame(); }
    }

    private static void RepositionEnemyDice(Scripts s) {
        s.statSummoner.RepositionAllDice("enemy");
    }

    private static Dice ChooseAdvancedDraftDie(Scripts s, List<Dice> availableDice) {
        Dice bestDie = null;
        float bestScore = float.NegativeInfinity;
        foreach (Dice dice in availableDice) {
            float score = EvaluateDraftChoice(s, dice);
            if (score > bestScore) {
                bestScore = score;
                bestDie = dice;
            }
        }
        return bestDie ?? availableDice[0];
    }

    private static Dice ChooseDefaultDraftDie(List<Dice> availableDice) {
        return availableDice
            .OrderBy(dice => GetDefaultRank(dice))
            .FirstOrDefault();
    }

    private static float EvaluateDraftChoice(Scripts s, Dice dice) {
        float denyScore = GetPlayerDieDesireScore(s, dice);
        float baseScore = dice.diceType switch {
            "yellow" => 130f + dice.diceNum * 22f,
            "red" => 100f + dice.diceNum * 16f,
            "blue" => 85f + dice.diceNum * 14f,
            "green" => 80f + dice.diceNum * 13f,
            "white" => 72f + dice.diceNum * 12f,
            _ => dice.diceNum * 10f,
        };

        float bestOutcome = float.NegativeInfinity;
        foreach (string stat in GetDraftAssignmentOptions(s, dice)) {
            Dictionary<string, int> yellowTotals = NewStatDictionary();
            Dictionary<string, int> yellowCounts = NewStatDictionary();
            if (dice.diceType == "yellow") {
                yellowTotals[stat] = dice.diceNum;
                yellowCounts[stat] = 1;
            }

            Dictionary<string, int> staminaPlan = NewStatDictionary();
            if (s.enemy.stamina > 0) { staminaPlan[stat] = 0; }
            int targetIndex = GetDraftPreviewTarget(s, dice, stat, yellowTotals, staminaPlan);
            float outcome = EvaluateAdvancedState(s, targetIndex, yellowTotals, yellowCounts, staminaPlan);
            if (dice.diceType != "yellow") {
                outcome += stat switch {
                    "red" => dice.diceNum * 12f,
                    "blue" => dice.diceNum * 10f,
                    "green" => dice.diceNum * 9f,
                    "white" => dice.diceNum * 8f,
                    _ => 0f,
                };
            }
            bestOutcome = Mathf.Max(bestOutcome, outcome);
        }

        return bestOutcome + baseScore + denyScore;
    }

    private static IEnumerable<string> GetDraftAssignmentOptions(Scripts s, Dice dice) {
        if (dice.diceType != "yellow") {
            if (dice.diceType == "green" && s.itemManager.PlayerHasWeapon("dagger")) { return new[] { "green" }; }
            if (dice.diceType == "white" && Save.game.curCharNum == 3) { return new[] { "white" }; }
            return new[] { dice.diceType };
        }

        return Stats;
    }

    private static int GetDraftPreviewTarget(
        Scripts s,
        Dice dice,
        string stat,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> staminaPlan
    ) {
        int previewAim = s.enemy.stats["green"] + GetFixedEnemyDiceSum(s, "green") + yellowTotals["green"] + staminaPlan["green"];
        if (dice.diceType != "yellow" && stat == "green") { previewAim += dice.diceNum; }
        return GetDefaultTargetIndex(s, previewAim);
    }

    private static float GetPlayerDieDesireScore(Scripts s, Dice dice) {
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int playerAim = s.statSummoner.SumOfStat("green", "player");
        float score = dice.diceNum * 7f;

        if (dice.diceType == "yellow") { score += 120f; }
        if (dice.diceType == "red" || dice.diceType == "green" && s.itemManager.PlayerHasWeapon("dagger") || dice.diceType == "white" && Save.game.curCharNum == 3) {
            score += Mathf.Max(0, enemyDef + 1 - playerAtt) * 20f;
        }
        if (dice.diceType == "blue") {
            score += Mathf.Max(0, enemySpd + 1 - playerSpd) * 18f;
        }
        if (dice.diceType == "green") {
            score += Mathf.Max(0, s.player.targetIndex - playerAim) * 22f;
        }
        if (dice.diceType == "white") {
            score += Mathf.Max(0, s.statSummoner.SumOfStat("red", "enemy") - s.statSummoner.SumOfStat("white", "player")) * 14f;
        }
        if (s.itemManager.PlayerHas("armor") && dice.diceType == "blue") {
            score -= 25f;
        }

        return score;
    }

    private static void AttachChosenDie(Scripts s, Dice chosenDie) {
        if (s.enemy.woundList.Contains("guts") && s.enemy.enemyName.text != "Lich") {
            s.enemy.StartCoroutine(chosenDie.DecreaseDiceValue(false));
        }

        chosenDie.isAttached = true;
        chosenDie.moveable = false;
        chosenDie.isOnPlayerOrEnemy = "enemy";

        string attachStat = "red";
        if (chosenDie.diceType != "yellow") {
            attachStat = chosenDie.diceType;
        }

        chosenDie.statAddedTo = attachStat;
        s.statSummoner.AddDiceToEnemy(attachStat, chosenDie);
        RepositionEnemyDice(s);

        if (chosenDie.diceType == "red" && s.enemy.woundList.Contains("armpits") || chosenDie.diceType == "white" && s.enemy.woundList.Contains("hand")) {
            if (s.enemy.enemyName.text != "Lich") { s.enemy.StartCoroutine(chosenDie.FadeOut(false)); }
        }
        else if (chosenDie.diceType == "yellow" && s.itemManager.PlayerHasWeapon("hatchet")) {
            s.enemy.StartCoroutine(chosenDie.FadeOut(false));
        }

        s.enemy.targetIndex = GetBestTargetIndex(s);
        s.turnManager.SetTargetOf("enemy");
        s.statSummoner.SetDebugInformationFor("enemy");
    }

    private static string ChooseBestYellowStatForDraft(Scripts s, Dice yellowDie) {
        string bestStat = "red";
        float bestScore = float.NegativeInfinity;
        foreach (string stat in Stats) {
            Dictionary<string, int> yellowTotals = NewStatDictionary();
            Dictionary<string, int> yellowCounts = NewStatDictionary();
            Dictionary<string, int> staminaPlan = NewStatDictionary();
            yellowTotals[stat] = yellowDie.diceNum;
            yellowCounts[stat] = 1;
            int targetIndex = GetDraftPreviewTarget(s, yellowDie, stat, yellowTotals, staminaPlan);
            float score = EvaluateAdvancedState(s, targetIndex, yellowTotals, yellowCounts, staminaPlan);
            if (score > bestScore) {
                bestScore = score;
                bestStat = stat;
            }
        }
        return bestStat;
    }

    private static int GetDefaultTargetIndex(Scripts s, int aim) {
        int maxTarget = Mathf.Clamp(aim, 0, 7);
        for (int i = maxTarget; i >= 0; i--) {
            if (!s.player.woundList.Contains(Targets[i])) {
                return i;
            }
        }
        return maxTarget >= 7 ? 7 : 0;
    }

    private static int GetEnemyStatWithPlan(Scripts s, Plan plan, string stat) {
        return s.enemy.stats[stat]
            + GetFixedEnemyDiceSum(s, stat)
            + GetAssignedYellowSum(plan, stat)
            + plan.Stamina[stat];
    }

    private static int GetAssignedYellowSum(Plan plan, string stat) {
        return plan.YellowAssignments
            .Where(pair => pair.Key != null && pair.Value == stat)
            .Sum(pair => pair.Key.diceNum);
    }

    private static IEnumerable<Dice> GetEnemyYellowDice(Scripts s) {
        return s.statSummoner.addedEnemyDice
            .SelectMany(pair => pair.Value)
            .Where(dice => dice != null && dice.diceType == "yellow")
            .Distinct();
    }

    private static int GetFixedEnemyDiceSum(Scripts s, string stat) {
        return s.statSummoner.addedEnemyDice[stat]
            .Where(dice => dice != null && dice.diceType != "yellow")
            .Sum(dice => dice.diceNum);
    }

    private static int GetFixedEnemyDiceCount(Scripts s, string stat) {
        return s.statSummoner.addedEnemyDice[stat]
            .Count(dice => dice != null && dice.diceType != "yellow");
    }

    private static int GetDiceCount(IEnumerable<Dice> diceList) {
        return diceList.Count(dice => dice != null);
    }

    private static int GetDiceSum(IEnumerable<Dice> diceList) {
        return diceList.Where(dice => dice != null).Sum(dice => dice.diceNum);
    }

    private static int GetLargestPlayerDie(Scripts s, string stat) {
        return s.statSummoner.addedPlayerDice[stat]
            .Where(dice => dice != null)
            .Select(dice => dice.diceNum)
            .DefaultIfEmpty(0)
            .Max();
    }

    private static float GetDiscardScore(Scripts s, Dice dice) {
        float score = dice.diceNum * 10f;
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");

        if (dice.statAddedTo == "red") {
            score += 80f;
            score += Mathf.Max(0, playerAtt - enemyDef) * 22f;
        }
        if (dice.statAddedTo == "blue") {
            score += 45f;
            score += Mathf.Max(0, playerSpd - enemySpd) * 18f;
        }
        if (dice.statAddedTo == "green") {
            score += 55f;
            score += Mathf.Max(0, s.player.targetIndex - s.statSummoner.SumOfStat("green", "player") + 1) * 25f;
        }
        if (dice.statAddedTo == "white") {
            score += 30f;
            score += Mathf.Max(0, s.statSummoner.SumOfStat("red", "enemy") - s.statSummoner.SumOfStat("white", "player")) * 14f;
        }
        if (dice.diceType == "yellow") { score += 50f; }
        return score;
    }

    private static int GetDefaultRank(Dice dice) {
        return DefaultDieRanks.TryGetValue(dice.diceType + dice.diceNum, out int rank) ? rank : int.MaxValue;
    }

    private static Dictionary<string, int> NewStatDictionary() {
        return new Dictionary<string, int> {
            { "green", 0 },
            { "blue", 0 },
            { "red", 0 },
            { "white", 0 },
        };
    }
}
