using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Profiling;
using UnityEngine;

public static class EnemyAI {
    private static readonly string[] Stats = { "green", "blue", "red", "white" };
    private static readonly string[] Targets = { "chest", "guts", "knee", "hip", "head", "hand", "armpits", "neck" };
    private static readonly int[] PreferredTargetSearchOrder = { 7, 6, 4, 5, 3, 2, 1, 0 };
    private const int AdvancedPlanProfileLogInterval = 10;
    private const double AdvancedPlanSlowLogThresholdMs = 8d;
    private const int MaxLikelyPlayerReplyDice = 5;
    private static readonly string[][] YellowSearchOrders = {
        new[] { "green", "blue", "red", "white" },
        new[] { "blue", "green", "red", "white" },
        new[] { "red", "green", "blue", "white" },
        new[] { "white", "green", "blue", "red" },
    };
    private static readonly ProfilerMarker BuildAdvancedPlanProfiler = new("EnemyAI.BuildAdvancedPlan");
    private static readonly Dictionary<string, int> StatIndexByName = new() {
        { "green", 0 },
        { "blue", 1 },
        { "red", 2 },
        { "white", 3 },
    };
    private static readonly Dictionary<string, int> DefaultDieRanks = new() {
        { "yellow6", 0 }, { "red6", 1 }, { "white6", 2 }, { "yellow5", 3 }, { "red5", 4 }, { "white5", 5 },
        { "yellow4", 6 }, { "red4", 7 }, { "white4", 8 }, { "yellow3", 9 }, { "red3", 10 }, { "white3", 11 },
        { "green6", 12 }, { "yellow2", 13 }, { "red2", 14 }, { "white2", 15 }, { "yellow1", 16 }, { "red1", 17 },
        { "white1", 18 }, { "green5", 19 }, { "green4", 20 }, { "blue6", 21 }, { "green3", 22 }, { "blue5", 23 },
        { "blue4", 24 }, { "green2", 25 }, { "blue3", 26 }, { "green1", 27 }, { "blue2", 28 }, { "blue1", 29 },
    };

    private sealed class Plan {
        public int TargetIndex;
        public Dictionary<string, int> Stamina = NewStatDictionary();
        public Dictionary<Dice, string> YellowAssignments = new();
    }

    private sealed class AdvancedPlanEvaluation {
        public bool EnemyKills;
        public bool EnemyDamagesPlayer;
        public bool EnemyAvoidsKill;
        public bool EnemyAvoidsDamage;
        public bool BreaksPlayerKill;
        public bool BreaksPlayerDamage;
        public bool BreaksPlayerProtection;
        public bool StripsPlayerStamina;
        public bool BreaksPlayerSpeed;
        public bool RemovesPlayerRed;
        public bool RemovesPlayerWhite;
        public bool RemovesPlayerBestDie;
        public bool BreaksPlayerTarget;
        public bool UsesChestOnHighValuePlayerDice;
        public bool UsesChestAsLastDitchGamble;
        public bool UsesAimStaminaForNonFatalTrade;
        public bool EnemyActsFirst;
        public int SpentStamina;
        public int RedOverspend;
        public int BlueOverspend;
        public int GreenOverspend;
        public int WhiteOverspend;
        public int ResourceOverspend;
        public int TargetIndex;

        public int TotalOverspend => RedOverspend + BlueOverspend + GreenOverspend + WhiteOverspend;
    }

    private sealed class DraftChoiceEvaluation {
        public AdvancedPlanEvaluation BestPlan;
        public bool CompletesKillBreakpoint;
        public bool CompletesHitBreakpoint;
        public bool CompletesOrderBreakpoint;
        public bool CompletesArmpitsBreakpoint;
        public bool CompletesHeadBreakpoint;
        public bool CompletesDefenseBreakpoint;
        public bool DeniesPlayerKill;
        public bool DeniesPlayerDamage;
        public bool DeniesPlayerGoFirst;
        public bool DeniesPlayerTarget;
        public string DieType;
        public bool IsYellow;
        public bool LosesValueToHatchet;
        public int DieValue;
        public float FallbackScore;
        public float ProgressScore;
        public float PlayerDenialScore;
    }

    private sealed class LiveDiscardEvaluation {
        public bool BreaksKill;
        public bool BreaksDamage;
        public bool BreaksGoFirst;
        public bool BreaksTarget;
        public bool RestoresDefense;
        public bool IsYellow;
        public int DieValue;
    }

    private sealed class DraftPreviewContext {
        public PlannerSnapshot BaseSnapshot;
        public List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> PlayerYellowReassignmentOptions = new();
        public Dictionary<YellowAssignmentStateKey, PlannerSnapshot> PlayerSnapshotCache = new();
        public Dictionary<Dice, List<(Dictionary<string, int> totals, Dictionary<string, int> counts)>> ReplyStatesByExcludedDie = new();
        public Dictionary<DraftPreviewCacheKey, AdvancedPlanEvaluation> PreviewEvaluationCache = new();
    }

    private sealed class SimAttachedDie {
        public string Stat;
        public int Value;
        public bool IsRerolled;
        public bool IsYellow;

        public SimAttachedDie Clone() {
            return (SimAttachedDie)MemberwiseClone();
        }
    }

    private sealed class PlannerSnapshot {
        public int PlayerAim;
        public int PlayerSpd;
        public int PlayerAtt;
        public int PlayerDef;
        public bool PlayerGuardSelected;
        public int PlayerPendingGuardParryBonus;
        public int EnemyBaseAim;
        public int EnemyBaseSpd;
        public int EnemyBaseAtt;
        public int EnemyBaseDef;
        public int PlayerTargetIndex;
        public int PlayerWoundCount;
        public int EnemyWoundCount;
        public int PlayerAddedGreen;
        public int PlayerAddedBlue;
        public int PlayerAddedRed;
        public int PlayerAddedWhite;
        public int PlayerGreenDiceCount;
        public int PlayerBlueDiceCount;
        public int PlayerRedDiceCount;
        public int PlayerWhiteDiceCount;
        public int EnemyBaseGreenDiceCount;
        public int EnemyBaseBlueDiceCount;
        public int EnemyBaseRedDiceCount;
        public int EnemyBaseWhiteDiceCount;
        public int PlayerGreenDiceSum;
        public int PlayerBlueDiceSum;
        public int PlayerRedDiceSum;
        public int PlayerWhiteDiceSum;
        public int EnemyBaseGreenDiceSum;
        public int EnemyBaseBlueDiceSum;
        public int EnemyBaseRedDiceSum;
        public int EnemyBaseWhiteDiceSum;
        public int EnemyAttachedDiceCount;
        public bool PlayerHasArmor;
        public bool PlayerHasDodgy;
        public bool PlayerCanBecomeDodgy;
        public bool PlayerHasMaul;
        public int PlayerCrystalShardCopies;
        public int PlayerCrystalShardLossPerShatter;
        public int PlayerBulwarkImmediateParryBonus;
        public int PlayerInevitableImmediateBonus;
        public int PlayerRiposteImmediateBonus;
        public int PlayervindictiveImmediateBonus;
        public int PlayerTridentImmediateBonus;
        public int PlayerScimitarDiscardCount;
        public bool PlayerHasGlassSword;
        public bool PlayerGlassSwordShattered;
        public int PlayerGlassSwordAimDeltaOnShatter;
        public int PlayerGlassSwordSpdDeltaOnShatter;
        public int PlayerGlassSwordAttDeltaOnShatter;
        public int PlayerGlassSwordDefDeltaOnShatter;
        public bool EnemyIsLich;
        public bool PlayerSpeedLockedHigh;
        public bool EnemySpeedLockedHigh;
        public List<SimAttachedDie> PlayerAttachedDice = new();
        public List<SimAttachedDie> EnemyAttachedDice = new();

        public PlannerSnapshot Clone() {
            PlannerSnapshot clone = (PlannerSnapshot)MemberwiseClone();
            clone.PlayerAttachedDice = PlayerAttachedDice.Select(die => die.Clone()).ToList();
            clone.EnemyAttachedDice = EnemyAttachedDice.Select(die => die.Clone()).ToList();
            return clone;
        }
    }

    private readonly struct YellowAssignmentStateKey : IEquatable<YellowAssignmentStateKey> {
        private readonly int greenTotal;
        private readonly int blueTotal;
        private readonly int redTotal;
        private readonly int whiteTotal;
        private readonly int greenCount;
        private readonly int blueCount;
        private readonly int redCount;
        private readonly int whiteCount;

        public YellowAssignmentStateKey(Dictionary<string, int> yellowTotals, Dictionary<string, int> yellowCounts) {
            greenTotal = yellowTotals["green"];
            blueTotal = yellowTotals["blue"];
            redTotal = yellowTotals["red"];
            whiteTotal = yellowTotals["white"];
            greenCount = yellowCounts["green"];
            blueCount = yellowCounts["blue"];
            redCount = yellowCounts["red"];
            whiteCount = yellowCounts["white"];
        }

        public bool Equals(YellowAssignmentStateKey other) {
            return greenTotal == other.greenTotal
                && blueTotal == other.blueTotal
                && redTotal == other.redTotal
                && whiteTotal == other.whiteTotal
                && greenCount == other.greenCount
                && blueCount == other.blueCount
                && redCount == other.redCount
                && whiteCount == other.whiteCount;
        }

        public override bool Equals(object obj) {
            return obj is YellowAssignmentStateKey other && Equals(other);
        }

        public override int GetHashCode() {
            HashCode hash = new();
            hash.Add(greenTotal);
            hash.Add(blueTotal);
            hash.Add(redTotal);
            hash.Add(whiteTotal);
            hash.Add(greenCount);
            hash.Add(blueCount);
            hash.Add(redCount);
            hash.Add(whiteCount);
            return hash.ToHashCode();
        }
    }

    private readonly struct DraftPreviewCacheKey : IEquatable<DraftPreviewCacheKey> {
        private readonly YellowAssignmentStateKey playerState;
        private readonly YellowAssignmentStateKey previewState;

        public DraftPreviewCacheKey(
            Dictionary<string, int> playerTotals,
            Dictionary<string, int> playerCounts,
            Dictionary<string, int> previewTotals,
            Dictionary<string, int> previewCounts
        ) {
            playerState = new YellowAssignmentStateKey(playerTotals, playerCounts);
            previewState = new YellowAssignmentStateKey(previewTotals, previewCounts);
        }

        public bool Equals(DraftPreviewCacheKey other) {
            return playerState.Equals(other.playerState)
                && previewState.Equals(other.previewState);
        }

        public override bool Equals(object obj) {
            return obj is DraftPreviewCacheKey other && Equals(other);
        }

        public override int GetHashCode() {
            HashCode hash = new();
            hash.Add(playerState);
            hash.Add(previewState);
            return hash.ToHashCode();
        }
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public static string LastAdvancedPlanProfileSummary { get; private set; } = "advanced plan profiler idle";
    private static int advancedPlanProfileRuns;
    private static double advancedPlanProfileTotalMs;
    private static double advancedPlanProfileMaxMs;
    private static int advancedPlanCacheHits;
    private static int advancedPlanCacheMisses;
#endif

    private static int suppressTargetEvaluationDepth;
    private static int cachedAdvancedPlanKey;
    private static Plan cachedAdvancedPlan;
    private static bool hasCachedAdvancedPlan;

    /// <summary>
    /// clear the single-entry advanced-plan cache
    /// </summary>
    public static void InvalidateCachedPlan() {
        hasCachedAdvancedPlan = false;
        cachedAdvancedPlan = null;
        cachedAdvancedPlanKey = 0;
    }

    private sealed class SimState {
        public int PlayerAim;
        public int PlayerSpd;
        public int PlayerAtt;
        public int PlayerDef;
        public bool PlayerGuardSelected;
        public int PlayerPendingGuardParryBonus;
        public int EnemyAim;
        public int EnemySpd;
        public int EnemyAtt;
        public int EnemyDef;
        public int EnemyTargetIndex;
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
        public bool PlayerCanBecomeDodgy;
        public bool PlayerHasMaul;
        public int PlayerCrystalShardCopies;
        public int PlayerCrystalShardLossPerShatter;
        public int PlayerBulwarkImmediateParryBonus;
        public int PlayerInevitableImmediateBonus;
        public int PlayerRiposteImmediateBonus;
        public int PlayervindictiveImmediateBonus;
        public int PlayerTridentImmediateBonus;
        public int PlayerScimitarDiscardCount;
        public bool PlayerHasGlassSword;
        public bool PlayerGlassSwordShattered;
        public int PlayerGlassSwordAimDeltaOnShatter;
        public int PlayerGlassSwordSpdDeltaOnShatter;
        public int PlayerGlassSwordAttDeltaOnShatter;
        public int PlayerGlassSwordDefDeltaOnShatter;
        public bool EnemyIsLich;
        public bool PlayerSpeedLockedHigh;
        public bool EnemySpeedLockedHigh;
        public bool PlayerImmuneToWounds;
        public float Bonus;
        public List<SimAttachedDie> PlayerAttachedDice = new();
        public List<SimAttachedDie> EnemyAttachedDice = new();

        public SimState Clone() {
            SimState clone = (SimState)MemberwiseClone();
            clone.PlayerAttachedDice = PlayerAttachedDice.Select(die => die.Clone()).ToList();
            clone.EnemyAttachedDice = EnemyAttachedDice.Select(die => die.Clone()).ToList();
            return clone;
        }
    }

    /// <summary>
    /// choose the best available die for the enemy
    /// </summary>
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

    /// <summary>
    /// build and apply the enemy's current live plan
    /// </summary>
    public static void ApplyLivePlan(Scripts s) {
        if (!CanPlan(s)) { return; }

        Plan plan = BuildPlan(s);
        suppressTargetEvaluationDepth++;
        try {
            ApplyPlan(s, plan);
        }
        finally {
            suppressTargetEvaluationDepth = Mathf.Max(0, suppressTargetEvaluationDepth - 1);
        }
    }

    /// <summary>
    /// build the nightmare plan at attack time and reveal it step-by-step
    /// </summary>
    public static IEnumerator AnimateAndApplyNightmarePlan(Scripts s) {
        if (!CanPlan(s)) { yield break; }

        PlannerSnapshot snapshot = BuildPlannerSnapshot(s);
        Plan currentPlan = CaptureCurrentEnemyPlanState(s);
        Plan plan = BuildPlan(s);
        Dictionary<string, int> startingStamina = CopyStatDictionary(s.statSummoner.addedEnemyStamina);
        int startingTargetIndex = Mathf.Clamp(s.enemy.targetIndex, 0, Targets.Length - 1);
        AdvancedPlanEvaluation currentEvaluation = EvaluatePlanOutcome(s, snapshot, currentPlan);
        AdvancedPlanEvaluation plannedEvaluation = EvaluatePlanOutcome(s, snapshot, plan);
        NormalizeNightmarePlanForReveal(s, snapshot, currentPlan, plan, currentEvaluation, ref plannedEvaluation, startingStamina, startingTargetIndex);

        bool playerDamagesEnemyBeforePlan = currentEvaluation != null && !currentEvaluation.EnemyAvoidsDamage;
        bool playerDamagesEnemyAfterPlan = plannedEvaluation != null && !plannedEvaluation.EnemyAvoidsDamage;
        bool enemyDamagesPlayerBeforePlan = currentEvaluation != null && currentEvaluation.EnemyDamagesPlayer;
        bool enemyDamagesPlayerAfterPlan = plannedEvaluation != null && plannedEvaluation.EnemyDamagesPlayer;
        bool targetChanged = plan.TargetIndex != startingTargetIndex;
        bool preventedPlayerHit = playerDamagesEnemyBeforePlan && !playerDamagesEnemyAfterPlan;
        bool createdEnemyHit = !enemyDamagesPlayerBeforePlan && enemyDamagesPlayerAfterPlan;
        bool changedTargetWhileStillHitting = enemyDamagesPlayerBeforePlan && enemyDamagesPlayerAfterPlan && targetChanged;

        int staminaSteps = Stats.Sum(stat => Mathf.Max(0, plan.Stamina[stat] - startingStamina[stat]));
        List<Dice> movedYellowDice = GetEnemyYellowDice(s)
            .Where(yellowDie => plan.YellowAssignments.TryGetValue(yellowDie, out string targetStat)
                && GetCurrentEnemyYellowAssignment(yellowDie) != targetStat)
            .ToList();
        int targetSteps = Mathf.Abs(plan.TargetIndex - startingTargetIndex);
        if (!enemyDamagesPlayerAfterPlan) {
            targetSteps = 0;
        }

        bool playSoundForSteps = preventedPlayerHit || createdEnemyHit || changedTargetWhileStillHitting;
        bool playSoundForTargetSteps = createdEnemyHit || changedTargetWhileStillHitting;

        int totalSteps = staminaSteps + movedYellowDice.Count + targetSteps;
        int stepsCompleted = 0;

        bool ShouldPauseAfterStep() {
            stepsCompleted++;
            return stepsCompleted < totalSteps;
        }

        foreach (string stat in Stats) {
            int current = startingStamina[stat];
            int target = plan.Stamina[stat];
            for (int i = current; i < target; i++) {
                yield return RunNightmareAnimationStep(
                    s,
                    () => ApplySingleEnemyStaminaStep(s, stat),
                    ShouldPauseAfterStep(),
                    playSoundForSteps);
            }
        }

        foreach (Dice yellowDie in movedYellowDice) {
            string targetStat = plan.YellowAssignments[yellowDie];
            yield return RunNightmareAnimationStep(
                s,
                () => MoveEnemyYellowDieToStat(s, yellowDie, targetStat),
                ShouldPauseAfterStep(),
                playSoundForSteps);
        }

        int targetDirection = Math.Sign(plan.TargetIndex - startingTargetIndex);
        while (s.enemy.targetIndex != plan.TargetIndex) {
            yield return RunNightmareAnimationStep(
                s,
                () => AdvanceEnemyTargetStep(s, targetDirection),
                ShouldPauseAfterStep(),
                playSoundForTargetSteps);
        }
        if (totalSteps > 0) {
            yield return s.delays[0.4f];
        }
        ApplyPlan(s, plan, saveGame: false);
    }

    /// <summary>
    /// get the best current target index for the enemy
    /// </summary>
    public static int GetBestTargetIndex(Scripts s) {
        if (!CanTarget(s)) { return 0; }
        if (suppressTargetEvaluationDepth > 0) { return Mathf.Clamp(s.enemy.targetIndex, 0, Targets.Length - 1); }

        if (DifficultyHelper.UsesAdvancedEnemyAI(Save.persistent.gameDifficulty) && HasAnyDiceInPlay(s) && s.diceSummoner.CountUnattachedDice() == 0) {
            return BuildPlan(s).TargetIndex;
        }

        return GetDefaultTargetIndex(s, s.statSummoner.SumOfStat("green", "enemy"));
    }

    /// <summary>
    /// choose which player die should be discarded by a head wound
    /// </summary>
    public static Dice GetBestPlayerDieToDiscard(Scripts s, List<Dice> playerDice) {
        if (playerDice == null || playerDice.Count == 0) { return null; }
        if (!DifficultyHelper.UsesAdvancedEnemyAI(Save.persistent.gameDifficulty)) {
            return playerDice
                .OrderBy(dice => GetDefaultRank(dice))
                .FirstOrDefault();
        }

        Dice bestDie = null;
        LiveDiscardEvaluation bestEvaluation = null;
        foreach (Dice dice in playerDice) {
            LiveDiscardEvaluation evaluation = EvaluateLiveDiscardChoice(s, dice);
            if (IsBetterLiveDiscardChoice(evaluation, bestEvaluation)) {
                bestEvaluation = evaluation;
                bestDie = dice;
            }
        }

        return bestDie ?? playerDice[0];
    }

    /// <summary>
    /// check whether the enemy can legally build a plan right now
    /// </summary>
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
                && HasAnyDiceInPlay(s)
            && s.diceSummoner.CountUnattachedDice() == 0;
    }

    /// <summary>
    /// check whether the enemy can legally retarget right now
    /// </summary>
    private static bool CanTarget(Scripts s) {
        return s != null
            && s.enemy != null
            && s.player != null
            && s.turnManager != null
            && s.enemy.enemyName.text is not "Merchant" and not "Blacksmith" and not "Tombstone";
    }

    /// <summary>
    /// check whether the current round has any live dice in play yet
    /// </summary>
    private static bool HasAnyDiceInPlay(Scripts s) {
        return s != null && s.diceSummoner != null && s.diceSummoner.existingDice.Count > 0;
    }

    /// <summary>
    /// dispatch to the correct planner for the current difficulty
    /// </summary>
    private static Plan BuildPlan(Scripts s) {
        string difficulty = DifficultyHelper.Normalize(Save.persistent.gameDifficulty);
        if (DifficultyHelper.IsEasy(difficulty)) { return BuildEasyPlan(s); }
        if (DifficultyHelper.IsNormal(difficulty)) { return BuildNormalPlan(s); }

        int cacheKey = CreateAdvancedPlanCacheKey(s);
        if (hasCachedAdvancedPlan && cacheKey == cachedAdvancedPlanKey) {
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
            advancedPlanCacheHits++;
    #endif
            return cachedAdvancedPlan;
        }

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        advancedPlanCacheMisses++;
    #endif

        Plan plan = BuildAdvancedPlan(s);
        cachedAdvancedPlanKey = cacheKey;
        cachedAdvancedPlan = plan;
        hasCachedAdvancedPlan = true;
        return plan;
    }

    /// <summary>
    /// build the easy-mode threshold plan
    /// </summary>
    private static Plan BuildEasyPlan(Scripts s) {
        return BuildNormalPlan(s);
    }

    /// <summary>
    /// build the normal-mode threshold plan
    /// </summary>
    private static Plan BuildNormalPlan(Scripts s) {
        Plan plan = CreateBaselinePlan(s);
        bool canUseStamina = (!s.enemy.woundList.Contains("hip") && !s.itemManager.EnemyHasTemporaryHipInjury()) || s.enemy.enemyName.text == "Lich";
        int remainingStamina = canUseStamina
            ? s.enemy.stamina + s.statSummoner.addedEnemyStamina.Values.Sum()
            : 0;
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int enemyAtt = GetEnemyStatWithPlan(s, plan, "red");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int enemySpd = GetEnemyStatWithPlan(s, plan, "blue");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemyDef = GetEnemyStatWithPlan(s, plan, "white");
        int naturalAim = s.enemy.stats["green"] + GetFixedEnemyDiceSum(s, "green") + GetAssignedYellowSum(plan, "green");
        int bulwarkBonus = s.itemManager.GetEffectiveCharmCount("bulwark");

        plan.TargetIndex = GetDefaultTargetIndex(s, Mathf.Min(naturalAim, 6));

        int playerDefAgainstCurrentOrder = playerDef + (enemySpd > playerSpd ? bulwarkBonus : 0);
        if (enemyAtt <= playerDefAgainstCurrentOrder && enemyAtt + remainingStamina > playerDefAgainstCurrentOrder) {
            int spend = playerDefAgainstCurrentOrder - enemyAtt + 1;
            plan.Stamina["red"] += spend;
            remainingStamina -= spend;
            enemyAtt += spend;
        }

        int playerDefIfEnemyGoesFirst = playerDef + bulwarkBonus;
        if (enemyAtt > playerDefIfEnemyGoesFirst && playerSpd >= enemySpd && playerAtt > enemyDef && enemySpd + remainingStamina > playerSpd) {
            int spend = playerSpd - enemySpd + 1;
            plan.Stamina["blue"] += spend;
            remainingStamina -= spend;
        }

        enemyDef = GetEnemyStatWithPlan(s, plan, "white");
        if (playerAtt > enemyDef && s.statSummoner.SumOfStat("green", "player") >= 0 && enemyDef + remainingStamina >= playerAtt) {
            int spend = playerAtt - enemyDef;
            plan.Stamina["white"] += spend;
        }

        if (naturalAim >= 7) {
            plan.TargetIndex = GetDefaultTargetIndex(s, naturalAim);
        }

        return plan;
    }

    /// <summary>
    /// build the hard and nightmare search plan
    /// </summary>
    private static Plan BuildAdvancedPlan(Scripts s) {
        using (BuildAdvancedPlanProfiler.Auto()) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif
            PlannerSnapshot snapshot = BuildPlannerSnapshot(s);
            List<Dice> yellowDice = GetEnemyYellowDice(s).ToList();
            string[] yellowAssignments = new string[yellowDice.Count];
            string[] bestYellowAssignments = new string[yellowDice.Count];
            Dictionary<string, int> yellowTotals = NewStatDictionary();
            Dictionary<string, int> yellowCounts = NewStatDictionary();
            Dictionary<string, int> staminaPlan = NewStatDictionary();
            Dictionary<string, int> bestStaminaPlan = NewStatDictionary();
            AdvancedPlanEvaluation bestEvaluation = null;
            bool canUseStamina = (!s.enemy.woundList.Contains("hip") && !s.itemManager.EnemyHasTemporaryHipInjury()) || snapshot.EnemyIsLich;
            int totalAvailableStamina = s.enemy.stamina + s.statSummoner.addedEnemyStamina.Values.Sum();
            int yellowLeavesVisited = 0;
            int candidatesEvaluated = 0;
            int futileCandidatesSkipped = 0;
            int bestTargetIndex = GetDefaultTargetIndex(s, snapshot.EnemyBaseAim);
            HashSet<YellowAssignmentStateKey> visitedYellowStates = new();

            if (TryBuildZeroResourceAdvancedPlan(s, snapshot, yellowDice, out Plan zeroResourcePlan, out int zeroResourceCandidates)) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                RecordAdvancedPlanProfile(stopwatch, 1, zeroResourceCandidates, 0);
#endif
                return zeroResourcePlan;
            }

            bool TryCandidate(int targetIndex) {
                candidatesEvaluated++;
                AdvancedPlanEvaluation evaluation = EvaluateAdvancedPlanCandidate(s, snapshot, targetIndex, yellowTotals, yellowCounts, staminaPlan);
                if (IsTrulyFutileAdvancedEvaluation(evaluation)) {
                    futileCandidatesSkipped++;
                    return false;
                }
                if (!IsBetterAdvancedEvaluation(evaluation, bestEvaluation)) { return false; }

                foreach (string stat in Stats) {
                    bestStaminaPlan[stat] = staminaPlan[stat];
                }
                for (int i = 0; i < yellowDice.Count; i++) {
                    bestYellowAssignments[i] = yellowAssignments[i];
                }

                bestTargetIndex = targetIndex;
                bestEvaluation = evaluation;
                return IsPerfectAdvancedEvaluation(evaluation);
            }

            bool SearchCompressedStaminaSpace() {
                YellowAssignmentStateKey yellowState = new(yellowTotals, yellowCounts);
                if (!visitedYellowStates.Add(yellowState)) { return false; }

                yellowLeavesVisited++;
                int baseAim = snapshot.EnemyBaseAim + yellowTotals["green"];
                int baseSpd = snapshot.EnemyBaseSpd + yellowTotals["blue"];
                int baseAtt = snapshot.EnemyBaseAtt + yellowTotals["red"];
                int baseDef = snapshot.EnemyBaseDef + yellowTotals["white"];
                List<int> blueOptions = BuildSpeedSpendOptions(snapshot, canUseStamina, totalAvailableStamina, baseSpd, baseAtt);

                foreach (int blueSpend in blueOptions) {
                    int remainingAfterBlue = totalAvailableStamina - blueSpend;
                    List<int> whiteOptions = canUseStamina
                        ? BuildDefenseSpendOptions(s, snapshot, yellowTotals, yellowCounts, remainingAfterBlue, baseDef, blueSpend)
                        : BuildSpendOptions(0, 0);

                    foreach (int whiteSpend in whiteOptions) {
                        int remainingAfterWhite = remainingAfterBlue - whiteSpend;
                        int postPlayerRedSpend = canUseStamina
                            ? GetPostPlayerWoundAttackSpendNeeded(s, snapshot, yellowTotals, yellowCounts, blueSpend, whiteSpend)
                            : 0;
                        List<int> redOptions = canUseStamina
                            ? BuildSpendOptions(remainingAfterWhite, 0, GetExactAttackSpendNeeded(GetProjectedPlayerDefenseForEnemyAttack(snapshot, baseSpd + blueSpend), baseAtt), postPlayerRedSpend)
                            : BuildSpendOptions(0, 0);

                        foreach (int redSpend in redOptions) {
                            int remainingAfterRed = remainingAfterWhite - redSpend;
                            int maxTarget = Mathf.Clamp(baseAim + remainingAfterRed, 0, 7);

                            staminaPlan["blue"] = blueSpend;
                            staminaPlan["white"] = whiteSpend;
                            staminaPlan["red"] = redSpend;

                            foreach (int targetIndex in GetTargetSearchOrder(maxTarget)) {
                                int greenSpend = canUseStamina ? GetExactAimSpendNeeded(targetIndex, baseAim) : 0;
                                if (greenSpend > remainingAfterRed) { continue; }

                                staminaPlan["green"] = greenSpend;
                                if (TryCandidate(targetIndex)) { return true; }
                            }
                        }
                    }
                }

                return false;
            }

            bool SearchYellow(int index) {
                if (index >= yellowDice.Count) {
                    return SearchCompressedStaminaSpace();
                }

                string curStat = string.IsNullOrEmpty(yellowDice[index].statAddedTo) ? "red" : yellowDice[index].statAddedTo;
                foreach (string stat in GetYellowSearchOrder(curStat)) {
                    yellowAssignments[index] = stat;
                    yellowTotals[stat] += yellowDice[index].diceNum;
                    yellowCounts[stat]++;
                    if (SearchYellow(index + 1)) { return true; }
                    yellowTotals[stat] -= yellowDice[index].diceNum;
                    yellowCounts[stat]--;
                }

                return false;
            }

            if (yellowDice.Count == 0) {
                SearchCompressedStaminaSpace();
            }
            else {
                SearchYellow(0);
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            RecordAdvancedPlanProfile(stopwatch, yellowLeavesVisited, candidatesEvaluated, futileCandidatesSkipped);
#endif

            if (bestEvaluation == null) { return CreateBaselinePlan(s); }

            Plan bestPlan = CreateBaselinePlan(s);
            bestPlan.TargetIndex = bestTargetIndex;
            foreach (string stat in Stats) {
                bestPlan.Stamina[stat] = bestStaminaPlan[stat];
            }
            for (int i = 0; i < yellowDice.Count; i++) {
                bestPlan.YellowAssignments[yellowDice[i]] = bestYellowAssignments[i];
            }

            return bestPlan;
        }
    }

    /// <summary>
    /// evaluate a candidate by building a fresh planner snapshot on demand
    /// </summary>
    private static AdvancedPlanEvaluation EvaluateAdvancedPlanCandidate(
        Scripts s,
        int targetIndex,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        Dictionary<string, int> staminaPlan
    ) {
        return EvaluateAdvancedPlanCandidate(s, BuildPlannerSnapshot(s), targetIndex, yellowTotals, yellowCounts, staminaPlan);
    }

    /// <summary>
    /// evaluate one hard-mode candidate against the current board snapshot
    /// </summary>
    private static AdvancedPlanEvaluation EvaluateAdvancedPlanCandidate(
        Scripts s,
        PlannerSnapshot snapshot,
        int targetIndex,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        Dictionary<string, int> staminaPlan
    ) {
        SimState state = CreateSimulationState(snapshot, yellowTotals, yellowCounts, staminaPlan);
        state.EnemyTargetIndex = targetIndex;
        AdvancedPlanEvaluation evaluation = new() {
            SpentStamina = staminaPlan.Values.Sum(),
            TargetIndex = targetIndex,
        };
        string playerTarget = GetPlayerTargetName(snapshot.PlayerTargetIndex, snapshot.PlayerGuardSelected);
        string enemyTarget = Targets[targetIndex];
        bool enemyActsFirst = state.EnemySpeedLockedHigh || (!state.PlayerSpeedLockedHigh && state.EnemySpd > state.PlayerSpd);
        int playerDefenseAgainstEnemy = GetEffectivePlayerDefenseForEnemyAttack(state, enemyActsFirst);
        bool playerCanHitBefore = !state.PlayerGuardSelected && state.PlayerAim >= 0 && state.PlayerAtt > state.EnemyDef;
        bool playerDamagesBefore = PlayerHitDamagesEnemy(s, state, playerTarget, playerCanHitBefore);
        bool playerKillsBefore = PlayerWouldKillEnemy(s, state, playerTarget, playerCanHitBefore);
        bool enemyCanHitBefore = state.EnemyAim >= 0 && state.EnemyAtt > playerDefenseAgainstEnemy;
        bool enemyHitConnectsBefore = EnemyHitConnects(state, enemyCanHitBefore, enemyActsFirst);
        bool enemyBreaksProtectionBefore = EnemyHitBreaksProtection(state, enemyHitConnectsBefore);
        bool enemyDamagesBefore = EnemyHitAppliesWound(s, state, enemyTarget, enemyCanHitBefore, enemyActsFirst);
        bool enemyKillsBefore = EnemyWouldKillPlayer(s, state, enemyTarget, enemyCanHitBefore) && enemyDamagesBefore;
        bool enemyWasParriedBefore = EnemyAttackTriggersParryResponses(state.EnemyAtt, playerDefenseAgainstEnemy);

        evaluation.EnemyActsFirst = enemyActsFirst;
        evaluation.RedOverspend = GetAttackOverspend(playerDefenseAgainstEnemy, state.EnemyAtt, staminaPlan["red"]);
        evaluation.BlueOverspend = GetSpeedOverspend(state.PlayerSpd, state.EnemySpd, staminaPlan["blue"], state.PlayerSpeedLockedHigh);
        evaluation.GreenOverspend = GetAimOverspend(targetIndex, state.EnemyAim, staminaPlan["green"]);
        evaluation.WhiteOverspend = GetDefenseOverspend(state.PlayerAim, state.PlayerAtt, state.EnemyDef, staminaPlan["white"]);
        evaluation.ResourceOverspend = GetAttackResourceOverspend(playerDefenseAgainstEnemy, state.EnemyAtt)
            + GetSpeedResourceOverspend(state.PlayerSpd, state.EnemySpd, state.PlayerSpeedLockedHigh)
            + GetAimResourceOverspend(targetIndex, state.EnemyAim)
            + GetDefenseResourceOverspend(state.PlayerAim, state.PlayerAtt, state.EnemyDef);
        evaluation.UsesChestOnHighValuePlayerDice = false;

        if (enemyActsFirst) {
            SimState afterEnemyHit = state.Clone();
            if (enemyBreaksProtectionBefore) {
                ConsumePlayerProtection(afterEnemyHit);
            }
            if (enemyDamagesBefore) {
                ApplyWoundToPlayer(afterEnemyHit, enemyTarget, s);
            }
            ApplyImmediatePlayerResponseAfterEnemyActsFirst(afterEnemyHit, s, enemyWasParriedBefore, enemyDamagesBefore);

            bool playerCanHitAfter = !afterEnemyHit.PlayerGuardSelected && afterEnemyHit.PlayerAim >= 0 && afterEnemyHit.PlayerAtt > afterEnemyHit.EnemyDef;
            bool playerDamagesAfter = PlayerHitDamagesEnemy(s, afterEnemyHit, playerTarget, playerCanHitAfter);
            bool playerKillsAfter = PlayerWouldKillEnemy(s, afterEnemyHit, playerTarget, playerCanHitAfter);
            bool chestRescueCanBreakDamage = enemyTarget == "chest"
                && enemyDamagesBefore
                && playerDamagesAfter
                && EnemyChestRescueCanBreakPlayerDamage(s, afterEnemyHit, playerTarget);

            evaluation.EnemyDamagesPlayer = enemyDamagesBefore;
            evaluation.EnemyKills = enemyKillsBefore;
            evaluation.EnemyAvoidsDamage = !playerDamagesAfter;
            evaluation.EnemyAvoidsKill = !playerKillsAfter;
            evaluation.BreaksPlayerKill = playerKillsBefore && !playerKillsAfter;
            evaluation.BreaksPlayerDamage = playerDamagesBefore && !playerDamagesAfter;
            evaluation.BreaksPlayerProtection = enemyBreaksProtectionBefore;
            // enemy acts first means player has no speed advantage; a knee lock adds
            // no gate value this round even if it permanently locks future rounds
            evaluation.BreaksPlayerSpeed = !enemyActsFirst && afterEnemyHit.EnemySpeedLockedHigh;
            evaluation.BreaksPlayerTarget = !state.PlayerGuardSelected
                && snapshot.PlayerTargetIndex >= 0
                && state.PlayerAim >= snapshot.PlayerTargetIndex
                && afterEnemyHit.PlayerAim < snapshot.PlayerTargetIndex;
            evaluation.StripsPlayerStamina = state.PlayerAddedGreen + state.PlayerAddedBlue + state.PlayerAddedRed + state.PlayerAddedWhite > 0
                && afterEnemyHit.PlayerAddedGreen + afterEnemyHit.PlayerAddedBlue + afterEnemyHit.PlayerAddedRed + afterEnemyHit.PlayerAddedWhite == 0;
            evaluation.RemovesPlayerRed = state.PlayerRedDiceSum > 0 && afterEnemyHit.PlayerRedDiceSum == 0;
            evaluation.RemovesPlayerWhite = state.PlayerWhiteDiceSum > 0 && afterEnemyHit.PlayerWhiteDiceSum == 0;
            evaluation.RemovesPlayerBestDie = targetIndex == 4 && afterEnemyHit.PlayerGreenDiceCount + afterEnemyHit.PlayerBlueDiceCount + afterEnemyHit.PlayerRedDiceCount + afterEnemyHit.PlayerWhiteDiceCount
                < state.PlayerGreenDiceCount + state.PlayerBlueDiceCount + state.PlayerRedDiceCount + state.PlayerWhiteDiceCount;
            evaluation.UsesChestOnHighValuePlayerDice = enemyTarget == "chest"
                && evaluation.EnemyDamagesPlayer
                && PlayerHasHighValueDice(state);
            evaluation.UsesChestAsLastDitchGamble = enemyTarget == "chest"
                && chestRescueCanBreakDamage
                && evaluation.EnemyDamagesPlayer
                && !evaluation.EnemyKills
                && !evaluation.EnemyAvoidsDamage
                && playerDamagesAfter
                && PlayerHasHighValueDice(state)
                && !evaluation.RemovesPlayerRed
                && !evaluation.RemovesPlayerWhite
                && !evaluation.StripsPlayerStamina
                && !evaluation.RemovesPlayerBestDie;
            evaluation.UsesAimStaminaForNonFatalTrade = staminaPlan["green"] > 0
                && targetIndex < 7
                && !evaluation.EnemyKills
                && evaluation.EnemyDamagesPlayer
                && !evaluation.EnemyAvoidsDamage;
            return evaluation;
        }

        SimState afterPlayerHit = state.Clone();
        if (playerDamagesBefore && !afterPlayerHit.EnemyIsLich) {
            ApplyWoundToEnemy(afterPlayerHit, playerTarget, s);
        }

        bool enemyCanHitAfterPlayer = !playerKillsBefore && afterPlayerHit.EnemyAim >= 0 && afterPlayerHit.EnemyAtt > afterPlayerHit.PlayerDef;
        bool enemyHitConnectsAfterPlayer = !playerKillsBefore && EnemyHitConnects(afterPlayerHit, enemyCanHitAfterPlayer, false);
        bool enemyBreaksProtectionAfterPlayer = !playerKillsBefore && EnemyHitBreaksProtection(afterPlayerHit, enemyHitConnectsAfterPlayer);
        bool enemyDamagesAfterPlayer = !playerKillsBefore && EnemyHitAppliesWound(s, afterPlayerHit, enemyTarget, enemyCanHitAfterPlayer, false);
        bool enemyKillsAfterPlayer = !playerKillsBefore && EnemyWouldKillPlayer(s, afterPlayerHit, enemyTarget, enemyCanHitAfterPlayer) && enemyDamagesAfterPlayer;

        evaluation.EnemyDamagesPlayer = enemyDamagesAfterPlayer;
        evaluation.EnemyKills = enemyKillsAfterPlayer;
        evaluation.EnemyAvoidsDamage = !playerDamagesBefore;
        evaluation.EnemyAvoidsKill = !playerKillsBefore;
        evaluation.BreaksPlayerKill = false;
        evaluation.BreaksPlayerDamage = false;
        evaluation.BreaksPlayerProtection = enemyBreaksProtectionAfterPlayer;
        evaluation.BreaksPlayerSpeed = false;
        evaluation.BreaksPlayerTarget = false;
        evaluation.StripsPlayerStamina = false;
        evaluation.RemovesPlayerRed = false;
        evaluation.RemovesPlayerWhite = false;
        evaluation.RemovesPlayerBestDie = false;
        evaluation.UsesChestOnHighValuePlayerDice = enemyTarget == "chest"
            && evaluation.EnemyDamagesPlayer
            && PlayerHasHighValueDice(state);
        evaluation.UsesChestAsLastDitchGamble = enemyTarget == "chest"
            && evaluation.EnemyDamagesPlayer
            && playerDamagesBefore
            && !evaluation.EnemyKills
            && !evaluation.EnemyAvoidsDamage
            && PlayerHasHighValueDice(state);
        evaluation.UsesAimStaminaForNonFatalTrade = staminaPlan["green"] > 0
            && targetIndex < 7
            && !evaluation.EnemyKills
            && evaluation.EnemyDamagesPlayer
            && !evaluation.EnemyAvoidsDamage;
        return evaluation;
    }

    /// <summary>
    /// compare two advanced evaluations using the lexicographic gate ordering
    /// </summary>
    private static bool IsBetterAdvancedEvaluation(AdvancedPlanEvaluation candidate, AdvancedPlanEvaluation current) {
        if (candidate == null) { return false; }
        if (current == null) { return true; }

        if (candidate.EnemyKills != current.EnemyKills) { return candidate.EnemyKills; }
        if (candidate.EnemyDamagesPlayer != current.EnemyDamagesPlayer) { return candidate.EnemyDamagesPlayer; }
        if (candidate.EnemyAvoidsKill != current.EnemyAvoidsKill) { return candidate.EnemyAvoidsKill; }
        if (candidate.EnemyAvoidsDamage != current.EnemyAvoidsDamage) { return candidate.EnemyAvoidsDamage; }
        if (candidate.BreaksPlayerKill != current.BreaksPlayerKill) { return candidate.BreaksPlayerKill; }
        if (candidate.BreaksPlayerDamage != current.BreaksPlayerDamage) { return candidate.BreaksPlayerDamage; }
        if (candidate.BreaksPlayerProtection != current.BreaksPlayerProtection) { return candidate.BreaksPlayerProtection; }
        if (candidate.StripsPlayerStamina != current.StripsPlayerStamina) { return candidate.StripsPlayerStamina; }
        if (candidate.BreaksPlayerSpeed != current.BreaksPlayerSpeed) { return candidate.BreaksPlayerSpeed; }
        if (candidate.RemovesPlayerRed != current.RemovesPlayerRed) { return candidate.RemovesPlayerRed; }
        if (candidate.RemovesPlayerBestDie != current.RemovesPlayerBestDie) { return candidate.RemovesPlayerBestDie; }
        if (candidate.RemovesPlayerWhite != current.RemovesPlayerWhite) { return candidate.RemovesPlayerWhite; }
        if (candidate.BreaksPlayerTarget != current.BreaksPlayerTarget) { return candidate.BreaksPlayerTarget; }
        if (candidate.UsesChestOnHighValuePlayerDice != current.UsesChestOnHighValuePlayerDice) { return candidate.UsesChestOnHighValuePlayerDice; }
        if (candidate.UsesChestAsLastDitchGamble != current.UsesChestAsLastDitchGamble) { return candidate.UsesChestAsLastDitchGamble; }
        if (candidate.EnemyActsFirst != current.EnemyActsFirst) { return candidate.EnemyActsFirst; }
        if (candidate.SpentStamina != current.SpentStamina) { return candidate.SpentStamina < current.SpentStamina; }
        if (candidate.TotalOverspend != current.TotalOverspend) { return candidate.TotalOverspend < current.TotalOverspend; }
        if (candidate.ResourceOverspend != current.ResourceOverspend) { return candidate.ResourceOverspend < current.ResourceOverspend; }
        if (candidate.TargetIndex != current.TargetIndex) {
            // chest is not a default preference; non-chest beats chest here
            // among non-chest targets, lower index is preferred
            bool candidateIsChest = candidate.TargetIndex == 0;
            bool currentIsChest = current.TargetIndex == 0;
            if (candidateIsChest != currentIsChest) { return currentIsChest; }
            return candidate.TargetIndex < current.TargetIndex;
        }
        return false;
    }

    /// <summary>
    /// detect the zero-cost perfect kill that can short-circuit the search
    /// </summary>
    private static bool IsPerfectAdvancedEvaluation(AdvancedPlanEvaluation evaluation) {
        return evaluation != null
            && evaluation.EnemyKills
            && evaluation.SpentStamina == 0
            && evaluation.TotalOverspend == 0;
    }

    /// <summary>
    /// reject stamina plans that spend resources without changing any relevant gate
    /// </summary>
    private static bool IsTrulyFutileAdvancedEvaluation(AdvancedPlanEvaluation evaluation) {
        if (evaluation == null || evaluation.SpentStamina <= 0) { return false; }
        if (evaluation.UsesAimStaminaForNonFatalTrade) { return true; }
        if (evaluation.EnemyKills || evaluation.EnemyDamagesPlayer || evaluation.EnemyAvoidsKill || evaluation.EnemyAvoidsDamage) { return false; }
        if (evaluation.BreaksPlayerKill || evaluation.BreaksPlayerDamage || evaluation.BreaksPlayerProtection || evaluation.StripsPlayerStamina || evaluation.BreaksPlayerSpeed) { return false; }
        if (evaluation.RemovesPlayerRed || evaluation.RemovesPlayerWhite || evaluation.RemovesPlayerBestDie || evaluation.BreaksPlayerTarget) { return false; }
        if (evaluation.UsesChestOnHighValuePlayerDice || evaluation.UsesChestAsLastDitchGamble) { return false; }
        return evaluation.TotalOverspend >= evaluation.SpentStamina;
    }

    private static float EvaluateAdvancedState(
        Scripts s,
        int targetIndex,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        Dictionary<string, int> staminaPlan
    ) {
        SimState state = CreateSimulationState(s, yellowTotals, yellowCounts, staminaPlan);
        state.EnemyTargetIndex = targetIndex;
        string playerTarget = GetPlayerTargetName(s.player.targetIndex, IsPlayerGuardSelected(s));
        string enemyTarget = Targets[targetIndex];
        int spentStamina = staminaPlan.Values.Sum();
        bool enemyActsFirst = state.EnemySpeedLockedHigh || (!state.PlayerSpeedLockedHigh && state.EnemySpd > state.PlayerSpd);
        bool playerActsFirst = state.PlayerSpeedLockedHigh || (!state.EnemySpeedLockedHigh && state.PlayerSpd >= state.EnemySpd);
        int playerDefenseAgainstEnemy = GetEffectivePlayerDefenseForEnemyAttack(state, enemyActsFirst);
        bool playerCanHit = !state.PlayerGuardSelected && state.PlayerAim >= 0 && state.PlayerAtt > state.EnemyDef;
        bool enemyCanHit = state.EnemyAim >= 0 && state.EnemyAtt > playerDefenseAgainstEnemy;
        bool playerKills = PlayerWouldKillEnemy(s, state, playerTarget, playerCanHit);
        bool enemyKills = EnemyWouldKillPlayer(s, state, enemyTarget, enemyCanHit);
        bool enemyHitConnects = EnemyHitConnects(state, enemyCanHit, enemyActsFirst);
        bool enemyBreaksProtection = EnemyHitBreaksProtection(state, enemyHitConnects);
        bool enemyHitApplies = EnemyHitAppliesWound(s, state, enemyTarget, enemyCanHit, enemyActsFirst);
        bool enemyWasParried = EnemyAttackTriggersParryResponses(state.EnemyAtt, playerDefenseAgainstEnemy);
        bool playerHitApplies = PlayerHitAppliesWound(s, playerTarget, playerCanHit);
        float score = state.Bonus;

        if (enemyActsFirst) {
            if (enemyCanHit) {
                score += 1200f;
                if (enemyKills && enemyHitApplies) { score += 100000f; }

                SimState afterEnemyHit = state.Clone();
                if (enemyBreaksProtection) {
                    ConsumePlayerProtection(afterEnemyHit);
                }
                if (enemyHitApplies) {
                    ApplyWoundToPlayer(afterEnemyHit, enemyTarget, s);
                    ApplyImmediatePlayerResponseAfterEnemyActsFirst(afterEnemyHit, s, enemyWasParried, true);
                    score += GetTargetUtility(s, enemyTarget, afterEnemyHit, onPlayer:true);
                }
                else if (enemyBreaksProtection) {
                    score += 220f;
                }
                else if (PlayerHasOneShotProtection(state)) {
                    score += 120f;
                }

                if (!enemyHitApplies) {
                    ApplyImmediatePlayerResponseAfterEnemyActsFirst(afterEnemyHit, s, enemyWasParried, false);
                }

                bool playerStillHits = afterEnemyHit.PlayerAim >= 0 && afterEnemyHit.PlayerAtt > afterEnemyHit.EnemyDef;
                bool playerStillKills = playerStillHits && PlayerWouldKillEnemy(s, afterEnemyHit, playerTarget, true);
                if (!playerStillHits) { score += 2500f; }
                if (!playerStillKills) { score += 1800f; }
                if (PlayerHasOneShotProtection(state) && !enemyBreaksProtection) { score -= 800f; }
            }

            if (playerCanHit) {
                score -= playerKills ? 90000f : 1400f;
                score -= GetPlayerThreatUtility(playerTarget, state);
            }
        }
        else if (playerActsFirst) {
            if (playerCanHit) {
                SimState afterPlayerHit = state.Clone();
                if (playerHitApplies && !afterPlayerHit.EnemyIsLich) { ApplyWoundToEnemy(afterPlayerHit, playerTarget, s); }
                if (playerKills) {
                    score -= 95000f;
                }
                else {
                    score -= 2500f;
                    bool enemyStillHits = afterPlayerHit.EnemyAim >= 0 && afterPlayerHit.EnemyAtt > afterPlayerHit.PlayerDef;
                    bool enemyStillKills = enemyStillHits && EnemyWouldKillPlayer(s, afterPlayerHit, enemyTarget, true);
                    if (!enemyStillHits) { score -= 1200f; }
                    if (!enemyStillKills && enemyKills) { score -= 900f; }
                }
            }

            if (enemyCanHit) {
                if (enemyHitApplies) {
                    SimState afterEnemyHit = state.Clone();
                    ApplyWoundToPlayer(afterEnemyHit, enemyTarget, s);
                    score += 600f;
                    score += GetTargetUtility(s, enemyTarget, afterEnemyHit, onPlayer:true);
                    if (enemyKills) { score += 2200f; }
                }
                else if (enemyBreaksProtection) {
                    score += 170f;
                }
                else if (PlayerHasOneShotProtection(state)) {
                    score += 80f;
                }
                else if (state.PlayerHasDodgy) {
                    score -= 700f;
                }
            }
        }

        score -= GetFutileStaminaPenalty(s, state, targetIndex, staminaPlan);
        if (!enemyCanHit) { score -= 900f; }
        if (enemyTarget == "neck" && PlayerHasOneShotProtection(state)) { score -= 600f; }
        if (targetIndex < 7 && s.player.woundList.Contains(enemyTarget)) { score -= 350f; }
        score -= spentStamina * 35f;
        score -= staminaPlan["white"] * 4f;
        score += state.EnemyAtt * 3f + state.EnemySpd * 2f + state.EnemyAim * 1.5f + state.EnemyDef;
        score -= state.PlayerAtt * 1.2f;
        return score;
    }

    /// <summary>
    /// build the default board-aligned plan before any optimization or search
    /// </summary>
    private static Plan CreateBaselinePlan(Scripts s) {
        Plan plan = new() {
            TargetIndex = GetDefaultTargetIndex(s, s.statSummoner.SumOfStat("green", "enemy"))
        };

        foreach (Dice yellowDie in GetEnemyYellowDice(s)) {
            plan.YellowAssignments[yellowDie] = yellowDie.statAddedTo == string.Empty ? "red" : yellowDie.statAddedTo;
        }

        return plan;
    }

    /// <summary>
    /// snapshot the current board once for repeated advanced-plan candidate evaluation
    /// </summary>
    private static PlannerSnapshot BuildPlannerSnapshot(Scripts s) {
        bool playerHasArmor = s.itemManager.PlayerHas("armor");
        bool playerHasDodgy = Save.game.isDodgy;
        bool playerCanBecomeDodgy = !playerHasDodgy
            && !Save.game.usedBoots
            && s.player.stamina >= 1
            && s.itemManager.PlayerHas("boots of dodge");
        bool playerHasMaul = s.itemManager.PlayerHasWeapon("maul");
        bool playerHasGlassSword = s.itemManager.PlayerHasWeapon("glass sword");
        bool playerHasLegendaryWeapon = s.itemManager.PlayerHasLegendary();
        bool enemyIsLich = s.enemy.enemyName.text == "Lich";
        bool playerSpeedLockedHigh = IsPlayerSpeedLockedHigh(s);
        bool enemySpeedLockedHigh = IsEnemySpeedLockedHigh(s);
        int playerGlassSwordAimDeltaOnShatter = 0;
        int playerGlassSwordSpdDeltaOnShatter = 0;
        int playerGlassSwordAttDeltaOnShatter = 0;
        int playerGlassSwordDefDeltaOnShatter = 0;

        if (playerHasGlassSword && !Save.game.glassSwordShattered) {
            playerGlassSwordAimDeltaOnShatter = 0 - s.player.stats["green"];
            playerGlassSwordSpdDeltaOnShatter = 1 - s.player.stats["blue"];
            playerGlassSwordAttDeltaOnShatter = 1 - s.player.stats["red"];
            playerGlassSwordDefDeltaOnShatter = 0 - s.player.stats["white"];
        }

        int enemyBaseGreenDiceSum = GetFixedEnemyDiceSum(s, "green");
        int enemyBaseBlueDiceSum = GetFixedEnemyDiceSum(s, "blue");
        int enemyBaseRedDiceSum = GetFixedEnemyDiceSum(s, "red");
        int enemyBaseWhiteDiceSum = GetFixedEnemyDiceSum(s, "white");
        int enemyBaseGreenDiceCount = GetFixedEnemyDiceCount(s, "green");
        int enemyBaseBlueDiceCount = GetFixedEnemyDiceCount(s, "blue");
        int enemyBaseRedDiceCount = GetFixedEnemyDiceCount(s, "red");
        int enemyBaseWhiteDiceCount = GetFixedEnemyDiceCount(s, "white");
        int playerAddedGreen = s.statSummoner.addedPlayerStamina["green"];
        int playerAddedBlue = s.statSummoner.addedPlayerStamina["blue"];
        int playerAddedRed = s.statSummoner.addedPlayerStamina["red"];
        int playerAddedWhite = s.statSummoner.addedPlayerStamina["white"];
        int playerGreenDiceCount = GetDiceCount(s.statSummoner.addedPlayerDice["green"]);
        int playerBlueDiceCount = GetDiceCount(s.statSummoner.addedPlayerDice["blue"]);
        int playerRedDiceCount = GetDiceCount(s.statSummoner.addedPlayerDice["red"]);
        int playerWhiteDiceCount = GetDiceCount(s.statSummoner.addedPlayerDice["white"]);
        int playerGreenDiceSum = GetDiceSum(s.statSummoner.addedPlayerDice["green"]);
        int playerBlueDiceSum = GetDiceSum(s.statSummoner.addedPlayerDice["blue"]);
        int playerRedDiceSum = GetDiceSum(s.statSummoner.addedPlayerDice["red"]);
        int playerWhiteDiceSum = GetDiceSum(s.statSummoner.addedPlayerDice["white"]);

        if (Save.game.isDestructive) {
            playerAddedRed = playerAddedGreen;
            playerRedDiceCount = playerGreenDiceCount;
            playerRedDiceSum = playerGreenDiceSum;
        }
        if (Save.game.isEmpowered) {
            playerAddedRed = playerAddedWhite;
            playerRedDiceCount = playerWhiteDiceCount;
            playerRedDiceSum = playerWhiteDiceSum;
        }
        if (Save.game.isFortified) {
            playerAddedWhite = playerAddedBlue;
            playerWhiteDiceCount = playerBlueDiceCount;
            playerWhiteDiceSum = playerBlueDiceSum;
        }

        return new PlannerSnapshot {
            PlayerAim = s.statSummoner.SumOfStat("green", "player"),
            PlayerSpd = s.statSummoner.SumOfStat("blue", "player"),
            PlayerAtt = s.statSummoner.SumOfStat("red", "player"),
            PlayerDef = s.statSummoner.SumOfStat("white", "player"),
            PlayerGuardSelected = IsPlayerGuardSelected(s),
            PlayerPendingGuardParryBonus = s.turnManager != null ? s.turnManager.GetPlayerGuardParryBonus(includePendingSelection: true) : 0,
            EnemyBaseAim = s.enemy.stats["green"] + enemyBaseGreenDiceSum,
            EnemyBaseSpd = s.enemy.stats["blue"] + enemyBaseBlueDiceSum,
            EnemyBaseAtt = s.enemy.stats["red"] + enemyBaseRedDiceSum,
            EnemyBaseDef = s.enemy.stats["white"] + enemyBaseWhiteDiceSum,
            PlayerTargetIndex = IsPlayerGuardSelected(s) ? s.player.targetIndex : GetPlayerDraftReferenceTargetIndex(s),
            PlayerWoundCount = s.player.woundList.Count,
            EnemyWoundCount = s.enemy.woundList.Count,
            PlayerAddedGreen = playerAddedGreen,
            PlayerAddedBlue = playerAddedBlue,
            PlayerAddedRed = playerAddedRed,
            PlayerAddedWhite = playerAddedWhite,
            PlayerGreenDiceCount = playerGreenDiceCount,
            PlayerBlueDiceCount = playerBlueDiceCount,
            PlayerRedDiceCount = playerRedDiceCount,
            PlayerWhiteDiceCount = playerWhiteDiceCount,
            PlayerGreenDiceSum = playerGreenDiceSum,
            PlayerBlueDiceSum = playerBlueDiceSum,
            PlayerRedDiceSum = playerRedDiceSum,
            PlayerWhiteDiceSum = playerWhiteDiceSum,
            EnemyBaseGreenDiceCount = enemyBaseGreenDiceCount,
            EnemyBaseBlueDiceCount = enemyBaseBlueDiceCount,
            EnemyBaseRedDiceCount = enemyBaseRedDiceCount,
            EnemyBaseWhiteDiceCount = enemyBaseWhiteDiceCount,
            EnemyBaseGreenDiceSum = enemyBaseGreenDiceSum,
            EnemyBaseBlueDiceSum = enemyBaseBlueDiceSum,
            EnemyBaseRedDiceSum = enemyBaseRedDiceSum,
            EnemyBaseWhiteDiceSum = enemyBaseWhiteDiceSum,
            EnemyAttachedDiceCount = s.statSummoner.addedEnemyDice.Sum(pair => pair.Value.Count),
            PlayerHasArmor = playerHasArmor,
            PlayerHasDodgy = playerHasDodgy,
            PlayerCanBecomeDodgy = playerCanBecomeDodgy,
            PlayerHasMaul = playerHasMaul,
            PlayerCrystalShardCopies = s.itemManager.GetPlayerItemCount("crystal shard"),
            PlayerCrystalShardLossPerShatter = 2,
            PlayerBulwarkImmediateParryBonus = GetEffectiveTriggeredPlayerCharmBonus(s, "bulwark"),
            PlayerInevitableImmediateBonus = GetEffectiveTriggeredPlayerCharmBonus(s, "inevitable"),
            PlayerRiposteImmediateBonus = GetEffectiveTriggeredPlayerCharmBonus(s, "riposte"),
            PlayervindictiveImmediateBonus = GetEffectiveTriggeredPlayerCharmBonus(s, "vindictive", 2),
            PlayerTridentImmediateBonus = s.itemManager.PlayerHasWeapon("trident") ? 1 : 0,
            PlayerScimitarDiscardCount = s.itemManager.PlayerHasWeapon("scimitar") ? (playerHasLegendaryWeapon ? 2 : 1) : 0,
            PlayerHasGlassSword = playerHasGlassSword,
            PlayerGlassSwordShattered = Save.game.glassSwordShattered,
            PlayerGlassSwordAimDeltaOnShatter = playerGlassSwordAimDeltaOnShatter,
            PlayerGlassSwordSpdDeltaOnShatter = playerGlassSwordSpdDeltaOnShatter,
            PlayerGlassSwordAttDeltaOnShatter = playerGlassSwordAttDeltaOnShatter,
            PlayerGlassSwordDefDeltaOnShatter = playerGlassSwordDefDeltaOnShatter,
            EnemyIsLich = enemyIsLich,
            PlayerSpeedLockedHigh = playerSpeedLockedHigh,
            EnemySpeedLockedHigh = enemySpeedLockedHigh,
            PlayerAttachedDice = GetPlayerAttachedDiceSnapshot(s),
            EnemyAttachedDice = GetEnemyAttachedDiceSnapshot(s),
        };
    }

    private static List<SimAttachedDie> GetPlayerAttachedDiceSnapshot(Scripts s) {
        List<SimAttachedDie> dice = new();
        if (s?.statSummoner?.addedPlayerDice == null) { return dice; }

        foreach (string stat in Stats) {
            foreach (Dice attachedDie in s.statSummoner.addedPlayerDice[stat]) {
                if (attachedDie == null || !attachedDie.isAttached || attachedDie.isOnPlayerOrEnemy != "player") { continue; }
                dice.Add(new SimAttachedDie {
                    Stat = stat,
                    Value = attachedDie.diceNum,
                    IsRerolled = attachedDie.isRerolled,
                    IsYellow = attachedDie.diceType == "yellow",
                });
            }
        }

        return dice;
    }

    private static List<SimAttachedDie> GetEnemyAttachedDiceSnapshot(Scripts s) {
        List<SimAttachedDie> dice = new();
        if (s?.statSummoner?.addedEnemyDice == null) { return dice; }

        foreach (string stat in Stats) {
            foreach (Dice attachedDie in s.statSummoner.addedEnemyDice[stat]) {
                if (attachedDie == null || !attachedDie.isAttached || attachedDie.isOnPlayerOrEnemy != "enemy" || attachedDie.diceType == "yellow") {
                    continue;
                }

                dice.Add(new SimAttachedDie {
                    Stat = stat,
                    Value = attachedDie.diceNum,
                    IsRerolled = attachedDie.isRerolled,
                    IsYellow = false,
                });
            }
        }

        return dice;
    }

    /// <summary>
    /// build a simulation state from the live board on demand
    /// </summary>
    private static SimState CreateSimulationState(
        Scripts s,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        Dictionary<string, int> staminaPlan
    ) {
        return CreateSimulationState(BuildPlannerSnapshot(s), yellowTotals, yellowCounts, staminaPlan);
    }

    /// <summary>
    /// build a simulation state from a cached planner snapshot
    /// </summary>
    private static SimState CreateSimulationState(
        PlannerSnapshot snapshot,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        Dictionary<string, int> staminaPlan
    ) {
        SimState state = new() {
            PlayerAim = snapshot.PlayerAim,
            PlayerSpd = snapshot.PlayerSpd,
            PlayerAtt = snapshot.PlayerAtt,
            PlayerDef = snapshot.PlayerDef,
            PlayerGuardSelected = snapshot.PlayerGuardSelected,
            PlayerPendingGuardParryBonus = snapshot.PlayerPendingGuardParryBonus,
            EnemyAim = snapshot.EnemyBaseAim + yellowTotals["green"] + staminaPlan["green"],
            EnemySpd = snapshot.EnemyBaseSpd + yellowTotals["blue"] + staminaPlan["blue"],
            EnemyAtt = snapshot.EnemyBaseAtt + yellowTotals["red"] + staminaPlan["red"],
            EnemyDef = snapshot.EnemyBaseDef + yellowTotals["white"] + staminaPlan["white"],
            PlayerWoundCount = snapshot.PlayerWoundCount,
            EnemyWoundCount = snapshot.EnemyWoundCount,
            PlayerAddedGreen = snapshot.PlayerAddedGreen,
            PlayerAddedBlue = snapshot.PlayerAddedBlue,
            PlayerAddedRed = snapshot.PlayerAddedRed,
            PlayerAddedWhite = snapshot.PlayerAddedWhite,
            EnemyAddedGreen = staminaPlan["green"],
            EnemyAddedBlue = staminaPlan["blue"],
            EnemyAddedRed = staminaPlan["red"],
            EnemyAddedWhite = staminaPlan["white"],
            PlayerGreenDiceCount = snapshot.PlayerGreenDiceCount,
            PlayerBlueDiceCount = snapshot.PlayerBlueDiceCount,
            PlayerRedDiceCount = snapshot.PlayerRedDiceCount,
            PlayerWhiteDiceCount = snapshot.PlayerWhiteDiceCount,
            PlayerGreenDiceSum = snapshot.PlayerGreenDiceSum,
            PlayerBlueDiceSum = snapshot.PlayerBlueDiceSum,
            PlayerRedDiceSum = snapshot.PlayerRedDiceSum,
            PlayerWhiteDiceSum = snapshot.PlayerWhiteDiceSum,
            EnemyGreenDiceCount = snapshot.EnemyBaseGreenDiceCount + yellowCounts["green"],
            EnemyBlueDiceCount = snapshot.EnemyBaseBlueDiceCount + yellowCounts["blue"],
            EnemyRedDiceCount = snapshot.EnemyBaseRedDiceCount + yellowCounts["red"],
            EnemyWhiteDiceCount = snapshot.EnemyBaseWhiteDiceCount + yellowCounts["white"],
            EnemyGreenDiceSum = snapshot.EnemyBaseGreenDiceSum + yellowTotals["green"],
            EnemyBlueDiceSum = snapshot.EnemyBaseBlueDiceSum + yellowTotals["blue"],
            EnemyRedDiceSum = snapshot.EnemyBaseRedDiceSum + yellowTotals["red"],
            EnemyWhiteDiceSum = snapshot.EnemyBaseWhiteDiceSum + yellowTotals["white"],
            PlayerHasArmor = snapshot.PlayerHasArmor,
            PlayerHasDodgy = snapshot.PlayerHasDodgy,
            PlayerCanBecomeDodgy = snapshot.PlayerCanBecomeDodgy,
            PlayerHasMaul = snapshot.PlayerHasMaul,
            PlayerCrystalShardCopies = snapshot.PlayerCrystalShardCopies,
            PlayerCrystalShardLossPerShatter = snapshot.PlayerCrystalShardLossPerShatter,
            PlayerBulwarkImmediateParryBonus = snapshot.PlayerBulwarkImmediateParryBonus,
            PlayerInevitableImmediateBonus = snapshot.PlayerInevitableImmediateBonus,
            PlayerRiposteImmediateBonus = snapshot.PlayerRiposteImmediateBonus,
            PlayervindictiveImmediateBonus = snapshot.PlayervindictiveImmediateBonus,
            PlayerTridentImmediateBonus = snapshot.PlayerTridentImmediateBonus,
            PlayerScimitarDiscardCount = snapshot.PlayerScimitarDiscardCount,
            PlayerHasGlassSword = snapshot.PlayerHasGlassSword,
            PlayerGlassSwordShattered = snapshot.PlayerGlassSwordShattered,
            PlayerGlassSwordAimDeltaOnShatter = snapshot.PlayerGlassSwordAimDeltaOnShatter,
            PlayerGlassSwordSpdDeltaOnShatter = snapshot.PlayerGlassSwordSpdDeltaOnShatter,
            PlayerGlassSwordAttDeltaOnShatter = snapshot.PlayerGlassSwordAttDeltaOnShatter,
            PlayerGlassSwordDefDeltaOnShatter = snapshot.PlayerGlassSwordDefDeltaOnShatter,
            EnemyIsLich = snapshot.EnemyIsLich,
            PlayerSpeedLockedHigh = snapshot.PlayerSpeedLockedHigh,
            EnemySpeedLockedHigh = snapshot.EnemySpeedLockedHigh,
            PlayerAttachedDice = snapshot.PlayerAttachedDice.Select(die => die.Clone()).ToList(),
            EnemyAttachedDice = snapshot.EnemyAttachedDice.Select(die => die.Clone()).ToList(),
        };

        AppendEstimatedEnemyYellowDice(state.EnemyAttachedDice, yellowTotals, yellowCounts);

        bool playerActsFirst = state.PlayerSpeedLockedHigh || (!state.EnemySpeedLockedHigh && state.PlayerSpd >= state.EnemySpd);
        if (!state.PlayerGuardSelected && playerActsFirst && state.PlayerTridentImmediateBonus > 0) {
            state.PlayerAtt += state.PlayerTridentImmediateBonus;
        }

        if (PlayerHasOneShotProtection(state)) { state.Bonus -= 150f; }
        if (!snapshot.PlayerGuardSelected && snapshot.PlayerTargetIndex == 6 && state.PlayerRedDiceSum > 0) { state.Bonus -= state.PlayerRedDiceSum * 12f; }
        if (!snapshot.PlayerGuardSelected && snapshot.PlayerTargetIndex == 4 && snapshot.EnemyAttachedDiceCount > 0) { state.Bonus -= 180f; }
        return state;
    }

    private static void AppendEstimatedEnemyYellowDice(List<SimAttachedDie> attachedDice, Dictionary<string, int> yellowTotals, Dictionary<string, int> yellowCounts) {
        if (attachedDice == null || yellowTotals == null || yellowCounts == null) { return; }

        foreach (string stat in Stats) {
            foreach (int value in BuildEstimatedDieValues(yellowTotals[stat], yellowCounts[stat])) {
                attachedDice.Add(new SimAttachedDie {
                    Stat = stat,
                    Value = value,
                    IsRerolled = false,
                    IsYellow = true,
                });
            }
        }
    }

    private static IEnumerable<int> BuildEstimatedDieValues(int total, int count) {
        if (total <= 0 || count <= 0) { yield break; }

        int remainingTotal = total;
        int remainingCount = count;
        while (remainingCount > 0) {
            int nextValue = Mathf.Clamp(remainingTotal - (remainingCount - 1), 1, 6);
            yield return nextValue;
            remainingTotal -= nextValue;
            remainingCount--;
        }
    }

    // planner shorthand: this models an immediate fatal line
    private static bool PlayerWouldKillEnemy(Scripts s, SimState state, string playerTarget, bool playerCanHit) {
        if (!playerCanHit) { return false; }
        if (string.IsNullOrEmpty(playerTarget) || playerTarget == "guard") { return false; }
        if (state.PlayerHasMaul) { return true; }
        return !s.enemy.woundList.Contains(playerTarget) && state.EnemyWoundCount >= 2;
    }

    private static bool PlayerHitDamagesEnemy(Scripts s, SimState state, string playerTarget, bool playerCanHit) {
        if (!playerCanHit) { return false; }
        if (string.IsNullOrEmpty(playerTarget) || playerTarget == "guard") { return false; }
        if (state.PlayerHasMaul) { return true; }
        return PlayerHitAppliesWound(s, playerTarget, true);
    }

    // planner shorthand: this models an immediate fatal line
    private static bool EnemyWouldKillPlayer(Scripts s, SimState state, string enemyTarget, bool enemyCanHit) {
        if (!enemyCanHit) { return false; }
        return !s.player.woundList.Contains(enemyTarget) && state.PlayerWoundCount >= 2;
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
                state.PlayerAttachedDice.RemoveAll(die => die.Stat == "white");
                break;
            case "armpits":
                state.PlayerAtt -= state.PlayerRedDiceSum;
                state.PlayerRedDiceCount = 0;
                state.PlayerRedDiceSum = 0;
                state.PlayerAttachedDice.RemoveAll(die => die.Stat == "red");
                break;
            case "chest":
                state.Bonus -= 650f;
                break;
            case "neck":
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
                ApplyBestEnemyDiscard(state, s);
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
            case "neck":
                break;
        }
    }

    private static bool PlayerHasOneShotProtection(SimState state) {
        return state.PlayerHasArmor;
    }

    private static bool PlayerHasOneShotProtection(Scripts s) {
        return s.itemManager.PlayerHas("armor");
    }

    private static int GetEffectivePlayerDefenseForEnemyAttack(SimState state, bool enemyActsFirst) {
        return state.PlayerDef
            + state.PlayerPendingGuardParryBonus
            + (enemyActsFirst ? state.PlayerBulwarkImmediateParryBonus : 0);
    }

    private static bool EnemyHitConnects(SimState state, bool enemyCanHit, bool enemyActsFirst) {
        if (!enemyCanHit) { return false; }
        if (!enemyActsFirst && PlayerCanBecomeDodgy(state)) { return false; }
        return true;
    }

    private static bool PlayerCanBecomeDodgy(SimState state) {
        return state != null && (state.PlayerHasDodgy || state.PlayerCanBecomeDodgy);
    }

    private static bool EnemyHitBreaksProtection(SimState state, bool enemyHitConnects) {
        return enemyHitConnects && PlayerHasOneShotProtection(state);
    }

    private static bool EnemyAttackTriggersParryResponses(int enemyAtt, int playerDefenseAgainstEnemy) {
        return enemyAtt >= 0 && enemyAtt <= playerDefenseAgainstEnemy;
    }

    private static void ConsumePlayerProtection(SimState state) {
        if (state.PlayerHasArmor) {
            state.PlayerHasArmor = false;
        }
    }

    private static void ApplyImmediatePlayerResponseAfterEnemyActsFirst(SimState state, Scripts s, bool enemyWasParried, bool enemyDamagedPlayer) {
        ApplyImmediatePlayerEnemyFirstAlwaysOnEffects(state);

        if (enemyDamagedPlayer) {
            ApplyImmediatePlayerWoundResponseEffects(state);
            return;
        }

        if (enemyWasParried) {
            ApplyImmediatePlayerParryResponseEffects(state, s);
        }
    }

    private static void ApplyImmediatePlayerEnemyFirstAlwaysOnEffects(SimState state) {
        if (state.PlayerInevitableImmediateBonus > 0) {
            state.PlayerAtt += state.PlayerInevitableImmediateBonus;
        }
    }

    private static void ApplyImmediatePlayerWoundResponseEffects(SimState state) {
        if (state.PlayervindictiveImmediateBonus > 0) {
            state.PlayerAtt += state.PlayervindictiveImmediateBonus;
        }

        bool crystalShardShatters = state.PlayerCrystalShardCopies > 0;
        if (crystalShardShatters) {
            state.PlayerCrystalShardCopies--;
            state.PlayerAtt -= state.PlayerCrystalShardLossPerShatter;
        }

        if (state.PlayerHasGlassSword && !state.PlayerGlassSwordShattered && !crystalShardShatters) {
            state.PlayerGlassSwordShattered = true;
            state.PlayerAim += state.PlayerGlassSwordAimDeltaOnShatter;
            state.PlayerSpd += state.PlayerGlassSwordSpdDeltaOnShatter;
            state.PlayerAtt += state.PlayerGlassSwordAttDeltaOnShatter;
            state.PlayerDef += state.PlayerGlassSwordDefDeltaOnShatter;
        }
    }

    private static void ApplyImmediatePlayerParryResponseEffects(SimState state, Scripts s) {
        if (state.PlayerRiposteImmediateBonus > 0) {
            state.PlayerAtt += state.PlayerRiposteImmediateBonus;
        }

        for (int i = 0; i < state.PlayerScimitarDiscardCount; i++) {
            ApplyBestEnemyDiscard(state, s);
        }
    }

    private static void ApplyBestPlayerDiscard(SimState state, Scripts s) {
        if (state?.PlayerAttachedDice == null || state.PlayerAttachedDice.Count == 0) { return; }

        string playerTarget = GetPlayerTargetName(s.player.targetIndex, IsPlayerGuardSelected(s));
        SimAttachedDie bestDie = null;
        LiveDiscardEvaluation bestEvaluation = null;
        foreach (SimAttachedDie die in state.PlayerAttachedDice) {
            LiveDiscardEvaluation evaluation = EvaluatePlayerDiscardChoice(s, state, playerTarget, die, enemyAlreadyActed:true);
            if (IsBetterLiveDiscardChoice(evaluation, bestEvaluation)) {
                bestEvaluation = evaluation;
                bestDie = die;
            }
        }

        if (bestDie != null) {
            RemoveDieFromPlayerState(state, bestDie);
        }
    }

    private static void ApplyBestEnemyDiscard(SimState state, Scripts s) {
        if (state?.EnemyAttachedDice == null || state.EnemyAttachedDice.Count == 0) { return; }

        string playerTarget = GetPlayerTargetName(s.player.targetIndex, IsPlayerGuardSelected(s));
        SimAttachedDie bestDie = null;
        float bestScore = float.NegativeInfinity;
        foreach (SimAttachedDie die in state.EnemyAttachedDice) {
            float score = GetEnemyDiscardImpactScore(s, state, playerTarget, die);
            if (score > bestScore) {
                bestScore = score;
                bestDie = die;
            }
        }

        if (bestDie != null) {
            RemoveDieFromEnemyState(state, bestDie);
        }
    }

    private static void RemoveValueFromPlayerState(SimState state, string stat, int value) {
        if (value <= 0) { return; }
        switch (stat) {
            case "green": state.PlayerAim -= value; state.PlayerGreenDiceSum -= value; state.PlayerGreenDiceCount = Mathf.Max(0, state.PlayerGreenDiceCount - 1); break;
            case "blue": state.PlayerSpd -= value; state.PlayerBlueDiceSum -= value; state.PlayerBlueDiceCount = Mathf.Max(0, state.PlayerBlueDiceCount - 1); break;
            case "red": state.PlayerAtt -= value; state.PlayerRedDiceSum -= value; state.PlayerRedDiceCount = Mathf.Max(0, state.PlayerRedDiceCount - 1); break;
            case "white": state.PlayerDef -= value; state.PlayerWhiteDiceSum -= value; state.PlayerWhiteDiceCount = Mathf.Max(0, state.PlayerWhiteDiceCount - 1); break;
        }

        int dieIndex = state.PlayerAttachedDice.FindIndex(die => die.Stat == stat && die.Value == value);
        if (dieIndex >= 0) {
            state.PlayerAttachedDice.RemoveAt(dieIndex);
        }
    }

    private static void RemoveDieFromPlayerState(SimState state, SimAttachedDie die) {
        if (state == null || die == null) { return; }
        RemoveValueFromPlayerState(state, die.Stat, die.Value);
    }

    private static bool EnemyChestRescueCanBreakPlayerDamage(Scripts s, SimState state, string playerTarget) {
        if (s == null || state == null || state.PlayerAttachedDice == null || state.PlayerAttachedDice.Count == 0) {
            return false;
        }

        SimState bestCaseState = state.Clone();
        bool rerolledAnyDie = false;
        foreach (SimAttachedDie attachedDie in bestCaseState.PlayerAttachedDice) {
            if (attachedDie == null || attachedDie.IsRerolled || attachedDie.Value < 3) { continue; }
            if (attachedDie.Stat != "green" && attachedDie.Stat != "red") { continue; }

            int delta = 1 - attachedDie.Value;
            if (delta == 0) {
                attachedDie.IsRerolled = true;
                rerolledAnyDie = true;
                continue;
            }

            if (attachedDie.Stat == "green") {
                bestCaseState.PlayerAim += delta;
                bestCaseState.PlayerGreenDiceSum += delta;
            }
            else {
                bestCaseState.PlayerAtt += delta;
                bestCaseState.PlayerRedDiceSum += delta;
            }

            attachedDie.Value = 1;
            attachedDie.IsRerolled = true;
            rerolledAnyDie = true;
        }

        if (!rerolledAnyDie) { return false; }

        bool playerCanHit = bestCaseState.PlayerAim >= 0 && bestCaseState.PlayerAtt > bestCaseState.EnemyDef;
        return !PlayerHitDamagesEnemy(s, bestCaseState, playerTarget, playerCanHit);
    }

    private static void RemoveValueFromEnemyState(SimState state, string stat, int value) {
        if (value <= 0) { return; }
        switch (stat) {
            case "green": state.EnemyAim -= value; state.EnemyGreenDiceSum -= value; state.EnemyGreenDiceCount = Mathf.Max(0, state.EnemyGreenDiceCount - 1); break;
            case "blue": state.EnemySpd -= value; state.EnemyBlueDiceSum -= value; state.EnemyBlueDiceCount = Mathf.Max(0, state.EnemyBlueDiceCount - 1); break;
            case "red": state.EnemyAtt -= value; state.EnemyRedDiceSum -= value; state.EnemyRedDiceCount = Mathf.Max(0, state.EnemyRedDiceCount - 1); break;
            case "white": state.EnemyDef -= value; state.EnemyWhiteDiceSum -= value; state.EnemyWhiteDiceCount = Mathf.Max(0, state.EnemyWhiteDiceCount - 1); break;
        }

        int dieIndex = state.EnemyAttachedDice.FindIndex(die => die.Stat == stat && die.Value == value);
        if (dieIndex >= 0) {
            state.EnemyAttachedDice.RemoveAt(dieIndex);
        }
    }

    private static void RemoveDieFromEnemyState(SimState state, SimAttachedDie die) {
        if (state == null || die == null) { return; }
        RemoveValueFromEnemyState(state, die.Stat, die.Value);
    }

    private static float GetTargetUtility(Scripts s, string target, SimState state, bool onPlayer) {
        return target switch {
            "guts" => onPlayer ? 800f + state.PlayerRedDiceCount * 160f : -600f,
            "knee" => onPlayer ? GetEnemyKneeTargetUtility(state) : -700f,
            "hip" => onPlayer ? 950f + (s.player.stamina * 20f) : -900f,
            "head" => onPlayer ? 1000f : -950f,
            "hand" => onPlayer ? 650f + state.PlayerWhiteDiceSum * 30f : -700f,
            "armpits" => onPlayer ? 1400f + state.PlayerRedDiceSum * 35f : -850f,
            "chest" => onPlayer ? -300f : -800f,
            "neck" => onPlayer ? 100000f : -100000f,
            _ => 0f,
        };
    }

    private static float GetEnemyKneeTargetUtility(SimState state) {
        if (state != null && state.PlayerScimitarDiscardCount > 0) {
            return 220f;
        }

        return 1100f;
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
            "neck" => 100000f,
            _ => 0f,
        };
    }

    private static void NormalizeNightmarePlanForReveal(
        Scripts s,
        PlannerSnapshot snapshot,
        Plan currentPlan,
        Plan plan,
        AdvancedPlanEvaluation currentEvaluation,
        ref AdvancedPlanEvaluation plannedEvaluation,
        Dictionary<string, int> startingStamina,
        int startingTargetIndex
    ) {
        if (s == null || snapshot == null || currentPlan == null || plan == null) { return; }

        if (plannedEvaluation != null && !plannedEvaluation.EnemyDamagesPlayer && plan.TargetIndex != startingTargetIndex) {
            plan.TargetIndex = startingTargetIndex;
            plannedEvaluation = EvaluatePlanOutcome(s, snapshot, plan);
        }

        bool changedStamina = Stats.Any(stat => plan.Stamina[stat] != startingStamina[stat]);
        bool changedTarget = plan.TargetIndex != startingTargetIndex;
        bool changedYellowAssignments = HasNightmareYellowRearrangement(currentPlan, plan);
        if (!changedYellowAssignments || changedStamina || changedTarget) { return; }
        if (!HasMatchingNightmareOutcome(currentEvaluation, plannedEvaluation)) { return; }

        foreach (KeyValuePair<Dice, string> assignment in currentPlan.YellowAssignments) {
            if (assignment.Key == null) { continue; }
            plan.YellowAssignments[assignment.Key] = assignment.Value;
        }

        plannedEvaluation = EvaluatePlanOutcome(s, snapshot, plan);
    }

    private static bool HasNightmareYellowRearrangement(Plan currentPlan, Plan plan) {
        if (currentPlan == null || plan == null) { return false; }

        foreach (KeyValuePair<Dice, string> assignment in plan.YellowAssignments) {
            if (assignment.Key == null) { continue; }
            string currentStat = currentPlan.YellowAssignments.TryGetValue(assignment.Key, out string existingStat)
                ? existingStat
                : GetCurrentEnemyYellowAssignment(assignment.Key);
            if (currentStat != assignment.Value) {
                return true;
            }
        }

        return false;
    }

    private static bool HasMatchingNightmareOutcome(AdvancedPlanEvaluation first, AdvancedPlanEvaluation second) {
        if (first == null || second == null) { return false; }

        return first.EnemyKills == second.EnemyKills
            && first.EnemyDamagesPlayer == second.EnemyDamagesPlayer
            && first.EnemyAvoidsKill == second.EnemyAvoidsKill
            && first.EnemyAvoidsDamage == second.EnemyAvoidsDamage;
    }

    private static void ApplyPlan(Scripts s, Plan plan, bool saveGame = true) {
        if (plan == null) { return; }

        List<Dice> yellowDice = GetEnemyYellowDice(s).ToList();

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

        foreach (Dice yellowDie in yellowDice) {
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
        s.turnManager.RecalculateMaxFor("enemy");
        Save.game.enemyStamina = s.enemy.stamina;
        Save.game.enemyTargetIndex = s.enemy.targetIndex;
        if (saveGame && s.tutorial == null) { Save.SaveGame(); }
    }

    private static IEnumerator RunNightmareAnimationStep(Scripts s, Action applyStep, bool waitAfterStep, bool playSound) {
        if (playSound) {
            s.soundManager.PlayClip("click0");
        }
        applyStep?.Invoke();
        if (waitAfterStep) {
            yield return s.delays[0.15f];
        }
        s.turnManager.RecalculateMaxFor("enemy");
    }

    private static void ApplySingleEnemyStaminaStep(Scripts s, string stat) {
        if (s == null || string.IsNullOrEmpty(stat) || !s.statSummoner.addedEnemyStamina.ContainsKey(stat) || s.enemy.stamina <= 0) {
            return;
        }

        s.statSummoner.addedEnemyStamina[stat] += 1;
        s.enemy.stamina = Mathf.Max(0, s.enemy.stamina - 1);
        s.enemy.staminaCounter.text = s.enemy.stamina.ToString();
        s.statSummoner.SummonStats();
        RepositionEnemyDice(s);
    }

    private static void MoveEnemyYellowDieToStat(Scripts s, Dice yellowDie, string targetStat) {
        if (yellowDie == null || s == null || string.IsNullOrEmpty(targetStat) || !s.statSummoner.addedEnemyDice.ContainsKey(targetStat)) {
            return;
        }

        string currentStat = GetCurrentEnemyYellowAssignment(yellowDie);
        if (currentStat == targetStat) { return; }

        if (s.statSummoner.addedEnemyDice.TryGetValue(currentStat, out List<Dice> currentDice)) {
            currentDice.Remove(yellowDie);
        }

        yellowDie.statAddedTo = targetStat;
        if (!s.statSummoner.addedEnemyDice[targetStat].Contains(yellowDie)) {
            s.statSummoner.addedEnemyDice[targetStat].Add(yellowDie);
        }

        s.statSummoner.SummonStats();
        RepositionEnemyDice(s);
    }

    private static void AdvanceEnemyTargetStep(Scripts s, int direction) {
        if (direction == 0) { return; }

        s.enemy.targetIndex = Mathf.Clamp(s.enemy.targetIndex + direction, 0, Targets.Length - 1);
        s.turnManager.RecalculateMaxFor("enemy");
    }

    private static Dictionary<string, int> CopyStatDictionary(Dictionary<string, int> source) {
        Dictionary<string, int> copy = NewStatDictionary();
        if (source == null) { return copy; }

        foreach (string stat in Stats) {
            if (source.TryGetValue(stat, out int value)) {
                copy[stat] = value;
            }
        }

        return copy;
    }

    private static string GetCurrentEnemyYellowAssignment(Dice yellowDie) {
        return yellowDie == null || string.IsNullOrEmpty(yellowDie.statAddedTo) ? "red" : yellowDie.statAddedTo;
    }

    private static void RepositionEnemyDice(Scripts s) {
        s.statSummoner.RepositionAllDice("enemy");
    }

    private static Dice ChooseAdvancedDraftDie(Scripts s, List<Dice> availableDice) {
        PlannerSnapshot snapshot = BuildPlannerSnapshot(s);
        DraftPreviewContext previewContext = new() {
            BaseSnapshot = snapshot,
            PlayerYellowReassignmentOptions = GetPlayerYellowReassignmentPreviewOptions(s)
        };

        Dice bestDie = null;
        DraftChoiceEvaluation bestEvaluation = null;
        foreach (Dice dice in availableDice) {
            DraftChoiceEvaluation evaluation = EvaluateAdvancedDraftChoice(s, snapshot, dice, availableDice, previewContext);
            if (IsBetterDraftChoice(evaluation, bestEvaluation)) {
                bestEvaluation = evaluation;
                bestDie = dice;
            }
        }

        if (bestDie == null) { return availableDice[0]; }

        return availableDice
            .Where(dice => dice != null && dice.diceType == bestDie.diceType)
            .OrderByDescending(dice => dice.diceNum)
            .FirstOrDefault() ?? bestDie;
    }

    private static Dice ChooseDefaultDraftDie(List<Dice> availableDice) {
        return availableDice
            .OrderBy(dice => GetDefaultRank(dice))
            .FirstOrDefault();
    }

    private static float EvaluateDraftChoice(Scripts s, Dice dice) {
        int effectiveEnemyValue = GetEffectiveEnemyDraftValue(s, dice);
        float denyScore = GetPlayerDieDesireScore(s, dice);
        bool initiativeLocked = IsDraftInitiativeLocked(s);
        float baseScore = dice.diceType switch {
            "yellow" => 130f + effectiveEnemyValue * 22f,
            "red" => 100f + effectiveEnemyValue * 16f,
            "blue" when initiativeLocked => effectiveEnemyValue * 6f,
            "blue" => 85f + effectiveEnemyValue * 14f,
            "green" => 80f + effectiveEnemyValue * 13f,
            "white" => 72f + effectiveEnemyValue * 12f,
            _ => effectiveEnemyValue * 10f,
        };

        float bestOutcome = float.NegativeInfinity;
        foreach (string stat in GetDraftAssignmentOptions(s, dice)) {
            Dictionary<string, int> yellowTotals = NewStatDictionary();
            Dictionary<string, int> yellowCounts = NewStatDictionary();
            if (effectiveEnemyValue > 0) {
                yellowTotals[stat] = effectiveEnemyValue;
                yellowCounts[stat] = 1;
            }

            Dictionary<string, int> staminaPlan = NewStatDictionary();

            int previewAim = s.enemy.stats["green"] + GetFixedEnemyDiceSum(s, "green") + yellowTotals["green"];
            int maxTarget = Mathf.Clamp(previewAim, 0, 7);
            for (int targetIndex = 0; targetIndex <= maxTarget; targetIndex++) {
                float outcome = EvaluateAdvancedState(s, targetIndex, yellowTotals, yellowCounts, staminaPlan);
                outcome += GetDraftBreakpointBonus(s, dice, stat, effectiveEnemyValue);
                outcome += GetDraftProgressBonus(s, stat, effectiveEnemyValue);
                outcome -= GetDraftOvercommitPenalty(s, dice, stat, effectiveEnemyValue);
                bestOutcome = Mathf.Max(bestOutcome, outcome);
            }
        }

        return bestOutcome + baseScore + denyScore;
    }

    private static DraftChoiceEvaluation EvaluateAdvancedDraftChoice(
        Scripts s,
        PlannerSnapshot snapshot,
        Dice dice,
        List<Dice> availableDice,
        DraftPreviewContext previewContext
    ) {
        int effectiveEnemyValue = GetEffectiveEnemyDraftValue(s, dice);
        DraftChoiceEvaluation evaluation = new() {
            DieType = dice.diceType,
            IsYellow = dice.diceType == "yellow",
            LosesValueToHatchet = dice.diceType == "yellow" && s.itemManager.PlayerHasWeapon("hatchet"),
            DieValue = dice.diceNum,
            FallbackScore = EvaluateDraftChoice(s, dice),
            PlayerDenialScore = GetPlayerDieDesireScore(s, dice),
        };

        foreach (string stat in GetDraftAssignmentOptions(s, dice)) {
            AdvancedPlanEvaluation preview = GetDraftPreviewEvaluation(s, snapshot, dice, stat, availableDice, previewContext);
            if (IsBetterAdvancedEvaluation(preview, evaluation.BestPlan)) {
                evaluation.BestPlan = preview;
            }

            UpdateDraftBreakpointFlags(s, evaluation, stat, effectiveEnemyValue);
            evaluation.ProgressScore = Mathf.Max(evaluation.ProgressScore, GetDraftProgressBonus(s, stat, effectiveEnemyValue));
        }

        evaluation.DeniesPlayerKill = DraftDieDeniesPlayerKill(s, dice);
        evaluation.DeniesPlayerDamage = DraftDieDeniesPlayerDamage(s, dice);
        evaluation.DeniesPlayerGoFirst = DraftDieDeniesPlayerGoFirst(s, dice);
        evaluation.DeniesPlayerTarget = DraftDieDeniesPlayerTarget(s, dice);
        if (effectiveEnemyValue <= 0 && !evaluation.DeniesPlayerKill && !evaluation.DeniesPlayerDamage && !evaluation.DeniesPlayerGoFirst && !evaluation.DeniesPlayerTarget) {
            evaluation.LosesValueToHatchet = true;
        }
        return evaluation;
    }

    private static AdvancedPlanEvaluation GetDraftPreviewEvaluation(
        Scripts s,
        PlannerSnapshot snapshot,
        Dice dice,
        string stat,
        List<Dice> availableDice,
        DraftPreviewContext previewContext
    ) {
        int effectiveEnemyValue = GetEffectiveEnemyDraftValue(s, dice);
        if (effectiveEnemyValue <= 0) {
            return GetWorstCaseDraftPreviewEvaluation(
                s,
                snapshot,
                dice,
                availableDice,
                NewStatDictionary(),
                NewStatDictionary(),
                previewContext);
        }

        Dictionary<string, int> previewTotals = NewStatDictionary();
        Dictionary<string, int> previewCounts = NewStatDictionary();
        previewTotals[stat] = effectiveEnemyValue;
        previewCounts[stat] = 1;

        return GetWorstCaseDraftPreviewEvaluation(s, snapshot, dice, availableDice, previewTotals, previewCounts, previewContext) ?? new AdvancedPlanEvaluation();
    }

    private static AdvancedPlanEvaluation GetWorstCaseDraftPreviewEvaluation(
        Scripts s,
        PlannerSnapshot snapshot,
        Dice chosenDie,
        List<Dice> availableDice,
        Dictionary<string, int> previewTotals,
        Dictionary<string, int> previewCounts,
        DraftPreviewContext previewContext
    ) {
        Dictionary<string, int> zeroTotals = NewStatDictionary();
        Dictionary<string, int> zeroCounts = NewStatDictionary();
        AdvancedPlanEvaluation baseline = GetCachedDraftPreviewEvaluation(
            s,
            snapshot,
            previewContext,
            zeroTotals,
            zeroCounts,
            previewTotals,
            previewCounts);
        AdvancedPlanEvaluation worstReply = baseline;
        foreach ((Dictionary<string, int> totals, Dictionary<string, int> counts) playerReplyState
            in GetPlayerReplyPreviewStates(s, availableDice, chosenDie, previewContext)) {
            AdvancedPlanEvaluation replyEvaluation = GetCachedDraftPreviewEvaluation(
                s,
                snapshot,
                previewContext,
                playerReplyState.totals,
                playerReplyState.counts,
                previewTotals,
                previewCounts);
            if (IsWorseAdvancedEvaluation(replyEvaluation, worstReply)) {
                worstReply = replyEvaluation;
            }
        }

        return worstReply;
    }

    private static AdvancedPlanEvaluation GetCachedDraftPreviewEvaluation(
        Scripts s,
        PlannerSnapshot baseSnapshot,
        DraftPreviewContext previewContext,
        Dictionary<string, int> playerTotals,
        Dictionary<string, int> playerCounts,
        Dictionary<string, int> previewTotals,
        Dictionary<string, int> previewCounts
    ) {
        if (previewContext == null) {
            PlannerSnapshot replySnapshot = CreateDraftPreviewSnapshot(baseSnapshot, playerTotals, playerCounts);
            return GetBestDraftPreviewEvaluation(s, replySnapshot, previewTotals, previewCounts) ?? new AdvancedPlanEvaluation();
        }

        DraftPreviewCacheKey cacheKey = new(playerTotals, playerCounts, previewTotals, previewCounts);
        if (previewContext.PreviewEvaluationCache.TryGetValue(cacheKey, out AdvancedPlanEvaluation cachedEvaluation)) {
            return cachedEvaluation;
        }

        YellowAssignmentStateKey playerStateKey = new(playerTotals, playerCounts);
        PlannerSnapshot snapshot;
        if (!previewContext.PlayerSnapshotCache.TryGetValue(playerStateKey, out snapshot)) {
            PlannerSnapshot snapshotSource = previewContext.BaseSnapshot ?? baseSnapshot;
            snapshot = CreateDraftPreviewSnapshot(snapshotSource, playerTotals, playerCounts);
            previewContext.PlayerSnapshotCache[playerStateKey] = snapshot;
        }

        AdvancedPlanEvaluation computedEvaluation = GetBestDraftPreviewEvaluation(s, snapshot, previewTotals, previewCounts) ?? new AdvancedPlanEvaluation();
        previewContext.PreviewEvaluationCache[cacheKey] = computedEvaluation;
        return computedEvaluation;
    }

    /// <summary>
    /// get the drafted die's real value after immediate enemy-side attach penalties
    /// </summary>
    private static int GetEffectiveEnemyDraftValue(Scripts s, Dice dice) {
        if (s == null || dice == null || s.enemy == null) { return 0; }
        if (dice.diceType == "blue" && IsDraftInitiativeLocked(s)) { return 0; }
        if (dice.diceType == "yellow" && s.itemManager.PlayerHasWeapon("hatchet")) { return 0; }

        int value = dice.diceNum;
        if (s.enemy.enemyName.text != "Lich") {
            if (dice.diceType == "red" && (s.enemy.woundList.Contains("armpits") || s.itemManager.EnemyHasTemporaryArmpitsInjury())) { return 0; }
            if (dice.diceType == "white" && (s.enemy.woundList.Contains("hand") || s.itemManager.EnemyHasTemporaryHandInjury())) { return 0; }
            if (s.enemy.woundList.Contains("guts") || s.itemManager.EnemyHasTemporaryGutsInjury()) {
                value = Mathf.Max(0, value - 1);
            }
        }

        value = Mathf.Max(0, value - s.itemManager.GetEnemyTarotPenaltyForDieType(dice.diceType));
        return value;
    }

    private static int GetEffectivePlayerDraftValue(Scripts s, Dice dice) {
        if (s == null || dice == null || s.player == null) { return 0; }
        if (dice.diceType == "blue" && IsDraftInitiativeLocked(s)) { return 0; }

        int value = dice.diceNum;
        if (s.player.woundList.Contains("guts")) {
            value = Mathf.Max(0, value - 1);
        }

        value = Mathf.Min(6, value + s.itemManager.GetTarotBonusForDieType(dice.diceType));

        if (dice.diceType == "red" && s.player.woundList.Contains("armpits")) { return 0; }
        if (dice.diceType == "white" && s.player.woundList.Contains("hand")) { return 0; }
        if (dice.diceType == "white" && Save.game.curCharNum == 2) { return Mathf.Min(value, 1); }
        return value;
    }

    private static void UpdateDraftBreakpointFlags(Scripts s, DraftChoiceEvaluation evaluation, string stat, int effectiveEnemyValue) {
        if (s == null || evaluation == null || effectiveEnemyValue <= 0) { return; }

        int enemyAim = s.statSummoner.SumOfStat("green", "enemy");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        bool initiativeLocked = IsDraftInitiativeLocked(s);

        int nextEnemyAim = enemyAim + (stat == "green" ? effectiveEnemyValue : 0);
        int nextEnemySpd = enemySpd + (stat == "blue" && !initiativeLocked ? effectiveEnemyValue : 0);
        int nextEnemyAtt = enemyAtt + (stat == "red" ? effectiveEnemyValue : 0);
        int nextEnemyDef = enemyDef + (stat == "white" ? effectiveEnemyValue : 0);

        bool playerSpeedLockedHigh = IsPlayerSpeedLockedHigh(s);
        bool enemySpeedLockedHigh = IsEnemySpeedLockedHigh(s);
        bool enemyActsFirstNow = enemySpeedLockedHigh || (!playerSpeedLockedHigh && enemySpd > playerSpd);
        bool enemyActsFirstAfterPick = enemySpeedLockedHigh || (!playerSpeedLockedHigh && nextEnemySpd > playerSpd);
        bool killEnabledNow = enemyAim >= 7 && enemyAtt > playerDef;
        bool killEnabledAfterPick = nextEnemyAim >= 7 && nextEnemyAtt > playerDef;

        evaluation.CompletesKillBreakpoint |= !killEnabledNow && killEnabledAfterPick;
        evaluation.CompletesHitBreakpoint |= enemyAtt <= playerDef && nextEnemyAtt > playerDef;
        evaluation.CompletesOrderBreakpoint |= !initiativeLocked && !enemyActsFirstNow && enemyActsFirstAfterPick;
        evaluation.CompletesArmpitsBreakpoint |= enemyAim < 6 && nextEnemyAim >= 6;
        evaluation.CompletesHeadBreakpoint |= enemyAim < 4 && nextEnemyAim >= 4;
        evaluation.CompletesDefenseBreakpoint |= enemyDef < playerAtt && nextEnemyDef >= playerAtt;
    }

    private static IEnumerable<string> GetPlayerDraftAssignmentOptions(Scripts s, Dice dice) {
        if (dice == null) { return Array.Empty<string>(); }
        if (dice.diceType == "yellow" || Save.game.isFurious) {
            return IsDraftInitiativeLocked(s)
                ? new[] { "green", "red", "white" }
                : Stats;
        }
        if (dice.diceType == "green" && s.itemManager.PlayerHasWeapon("dagger")) { return new[] { "red" }; }
        if (dice.diceType == "white" && Save.game.curCharNum == 3) { return new[] { "red" }; }
        return new[] { dice.diceType };
    }

    private static PlannerSnapshot CreateDraftPreviewSnapshot(
        PlannerSnapshot source,
        Dictionary<string, int> playerTotals,
        Dictionary<string, int> playerCounts
    ) {
        PlannerSnapshot preview = source.Clone();
        preview.PlayerAim += playerTotals["green"];
        preview.PlayerSpd += playerTotals["blue"];
        preview.PlayerAtt += playerTotals["red"];
        preview.PlayerDef += playerTotals["white"];
        preview.PlayerGreenDiceCount += playerCounts["green"];
        preview.PlayerBlueDiceCount += playerCounts["blue"];
        preview.PlayerRedDiceCount += playerCounts["red"];
        preview.PlayerWhiteDiceCount += playerCounts["white"];
        preview.PlayerGreenDiceSum += playerTotals["green"];
        preview.PlayerBlueDiceSum += playerTotals["blue"];
        preview.PlayerRedDiceSum += playerTotals["red"];
        preview.PlayerWhiteDiceSum += playerTotals["white"];
        return preview;
    }

    private static List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> GetPlayerYellowReassignmentPreviewOptions(Scripts s) {
        List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> results = new();
        HashSet<YellowAssignmentStateKey> visited = new();
        AddPreviewState(results, visited, NewStatDictionary(), NewStatDictionary());

        if (s?.statSummoner?.addedPlayerDice == null) {
            return results;
        }

        List<Dice> playerYellowDice = GetPlayerYellowDice(s).ToList();
        if (playerYellowDice.Count == 0) {
            return results;
        }

        Dictionary<string, int> currentTotals = NewStatDictionary();
        Dictionary<string, int> currentCounts = NewStatDictionary();
        foreach (Dice yellowDie in playerYellowDice) {
            string currentStat = string.IsNullOrEmpty(yellowDie.statAddedTo) ? "red" : yellowDie.statAddedTo;
            currentTotals[currentStat] += yellowDie.diceNum;
            currentCounts[currentStat] += 1;
        }

        if (IsPlayerGuardSelected(s)) {
            AddPreviewState(results, visited, currentTotals, currentCounts);

            Dictionary<string, int> guardTotals = NewStatDictionary();
            Dictionary<string, int> guardCounts = NewStatDictionary();
            foreach (Dice yellowDie in playerYellowDice) {
                guardTotals["white"] += yellowDie.diceNum;
                guardCounts["white"] += 1;
            }

            AddPreviewState(results, visited, guardTotals, guardCounts);
            return results;
        }

        int playerAim = s.statSummoner.SumOfStat("green", "player");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int nonYellowAim = playerAim - currentTotals["green"];
        int nonYellowSpd = playerSpd - currentTotals["blue"];
        int nonYellowAtt = playerAtt - currentTotals["red"];
        int nonYellowDef = playerDef - currentTotals["white"];
        int targetAim = GetPlayerDraftReferenceTargetIndex(s);
        bool playerSpeedLockedHigh = IsPlayerSpeedLockedHigh(s);
        bool enemySpeedLockedHigh = IsEnemySpeedLockedHigh(s);
        bool playerActsFirst = playerSpeedLockedHigh || (!enemySpeedLockedHigh && playerSpd >= enemySpd);
        bool enemyThreatens = enemyAtt > playerDef;
        bool playerCanHit = playerAim >= 0 && playerAtt > enemyDef;
        int neededGreenForTarget = Mathf.Max(0, targetAim - nonYellowAim);
        int neededGreenToHit = Mathf.Max(0, 0 - nonYellowAim);
        int neededGreen = Mathf.Max(neededGreenForTarget, neededGreenToHit);
        int neededRed = Mathf.Max(0, enemyDef + 1 - nonYellowAtt);
        int neededBlue = playerSpeedLockedHigh || enemySpeedLockedHigh
            ? 0
            : Mathf.Max(0, enemySpd - nonYellowSpd);
        int neededWhite = Mathf.Max(0, enemyAtt - nonYellowDef);

        AddPreviewState(
            results,
            visited,
            BuildPlayerYellowHeuristicDelta(
                playerYellowDice,
                currentTotals,
                currentCounts,
                CreateRequiredStatTotals(neededRed, 0, neededGreen, 0),
                new[] { "red", "green" },
                !playerActsFirst ? "blue" : (enemyThreatens ? "white" : "red")));

        if (!playerActsFirst) {
            AddPreviewState(
                results,
                visited,
                BuildPlayerYellowHeuristicDelta(
                    playerYellowDice,
                    currentTotals,
                    currentCounts,
                    CreateRequiredStatTotals(neededRed, neededBlue, neededGreen, 0),
                    new[] { "blue", "red", "green" },
                    playerCanHit ? "white" : "red"));
        }

        if (enemyThreatens || !playerCanHit) {
            AddPreviewState(
                results,
                visited,
                BuildPlayerYellowHeuristicDelta(
                    playerYellowDice,
                    currentTotals,
                    currentCounts,
                    CreateRequiredStatTotals(playerCanHit ? neededRed : 0, 0, playerCanHit ? neededGreen : 0, neededWhite),
                    new[] { "white", "red", "green" },
                    "white"));
        }

        if (neededGreen > 0) {
            AddPreviewState(
                results,
                visited,
                BuildPlayerYellowHeuristicDelta(
                    playerYellowDice,
                    currentTotals,
                    currentCounts,
                    CreateRequiredStatTotals(neededRed, 0, neededGreen, 0),
                    new[] { "green", "red" },
                    !playerActsFirst ? "blue" : (enemyThreatens ? "white" : "red")));
        }

        return results;
    }

    private static List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> GetPlayerReplyPreviewStates(
        Scripts s,
        List<Dice> availableDice,
        Dice excludedDie,
        DraftPreviewContext previewContext
    ) {
        if (previewContext == null) {
            return BuildPlayerReplyPreviewStates(s, availableDice, excludedDie, GetPlayerYellowReassignmentPreviewOptions(s));
        }

        if (previewContext.ReplyStatesByExcludedDie.TryGetValue(excludedDie, out List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> cachedStates)) {
            return cachedStates;
        }

        List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> replyStates = BuildPlayerReplyPreviewStates(
            s,
            availableDice,
            excludedDie,
            previewContext.PlayerYellowReassignmentOptions);
        previewContext.ReplyStatesByExcludedDie[excludedDie] = replyStates;
        return replyStates;
    }

    private static List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> BuildPlayerReplyPreviewStates(
        Scripts s,
        List<Dice> availableDice,
        Dice excludedDie,
        List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> yellowReassignments
    ) {
        List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> replyStates = new();
        HashSet<YellowAssignmentStateKey> visited = new();

        void AddReplyState(Dictionary<string, int> totals, Dictionary<string, int> counts) {
            YellowAssignmentStateKey stateKey = new(totals, counts);
            if (!visited.Add(stateKey)) { return; }
            replyStates.Add((totals, counts));
        }

        AddReplyState(NewStatDictionary(), NewStatDictionary());

        List<Dice> playerReplyDice = GetLikelyPlayerReplyDice(s, availableDice, excludedDie).ToList();

        foreach (Dice replyDie in playerReplyDice) {
            int effectivePlayerValue = GetEffectivePlayerDraftValue(s, replyDie);
            if (effectivePlayerValue <= 0) { continue; }

            IEnumerable<string> playerStats = GetHeuristicPlayerDraftAssignmentOptions(s, replyDie);

            foreach (string playerStat in playerStats) {
                foreach ((Dictionary<string, int> totals, Dictionary<string, int> counts) yellowReassignment in yellowReassignments) {
                    Dictionary<string, int> playerTotals = CopyStatDictionary(yellowReassignment.totals);
                    Dictionary<string, int> playerCounts = CopyStatDictionary(yellowReassignment.counts);

                    if (effectivePlayerValue > 0 && !string.IsNullOrEmpty(playerStat)) {
                        playerTotals[playerStat] += effectivePlayerValue;
                        playerCounts[playerStat] += 1;
                    }

                    AddReplyState(playerTotals, playerCounts);
                }
            }
        }

        return replyStates;
    }

    private static Dictionary<string, int> CreateRequiredStatTotals(int red, int blue, int green, int white) {
        Dictionary<string, int> required = NewStatDictionary();
        required["red"] = Mathf.Max(0, red);
        required["blue"] = Mathf.Max(0, blue);
        required["green"] = Mathf.Max(0, green);
        required["white"] = Mathf.Max(0, white);
        return required;
    }

    private static void AddPreviewState(
        List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> results,
        HashSet<YellowAssignmentStateKey> visited,
        Dictionary<string, int> totals,
        Dictionary<string, int> counts
    ) {
        YellowAssignmentStateKey stateKey = new(totals, counts);
        if (!visited.Add(stateKey)) { return; }
        results.Add((totals, counts));
    }

    private static void AddPreviewState(
        List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> results,
        HashSet<YellowAssignmentStateKey> visited,
        (Dictionary<string, int> totals, Dictionary<string, int> counts) state
    ) {
        AddPreviewState(results, visited, state.totals, state.counts);
    }

    private static (Dictionary<string, int> totals, Dictionary<string, int> counts) BuildPlayerYellowHeuristicDelta(
        List<Dice> playerYellowDice,
        Dictionary<string, int> currentTotals,
        Dictionary<string, int> currentCounts,
        Dictionary<string, int> requiredTotals,
        string[] needPriorityOrder,
        string fallbackStat
    ) {
        Dictionary<string, int> desiredTotals = NewStatDictionary();
        Dictionary<string, int> desiredCounts = NewStatDictionary();
        Dictionary<string, int> remaining = CopyStatDictionary(requiredTotals);
        string safeFallbackStat = StatIndexByName.ContainsKey(fallbackStat) ? fallbackStat : "red";

        foreach (Dice yellowDie in playerYellowDice.OrderByDescending(die => die.diceNum)) {
            string targetStat = null;
            foreach (string stat in needPriorityOrder) {
                if (StatIndexByName.ContainsKey(stat) && remaining[stat] > 0) {
                    targetStat = stat;
                    break;
                }
            }

            targetStat ??= safeFallbackStat;
            desiredTotals[targetStat] += yellowDie.diceNum;
            desiredCounts[targetStat] += 1;
            remaining[targetStat] = Mathf.Max(0, remaining[targetStat] - yellowDie.diceNum);
        }

        Dictionary<string, int> deltaTotals = NewStatDictionary();
        Dictionary<string, int> deltaCounts = NewStatDictionary();
        foreach (string stat in Stats) {
            deltaTotals[stat] = desiredTotals[stat] - currentTotals[stat];
            deltaCounts[stat] = desiredCounts[stat] - currentCounts[stat];
        }

        return (deltaTotals, deltaCounts);
    }

    private static IEnumerable<Dice> GetLikelyPlayerReplyDice(Scripts s, List<Dice> availableDice, Dice excludedDie) {
        List<Dice> remainingDice = availableDice == null
            ? new List<Dice>()
            : availableDice.Where(dice => dice != null && dice != excludedDie).ToList();
        if (remainingDice.Count == 0) {
            return Array.Empty<Dice>();
        }

        List<Dice> likelyDice = new();

        void AddLikelyDie(Dice dice) {
            if (dice != null && !likelyDice.Contains(dice)) {
                likelyDice.Add(dice);
            }
        }

        AddLikelyDie(remainingDice
            .OrderByDescending(dice => GetPlayerDieDesireScore(s, dice))
            .ThenByDescending(dice => dice.diceNum)
            .FirstOrDefault());

        foreach (string diceType in new[] { "yellow", "red", "blue", "green", "white" }) {
            AddLikelyDie(remainingDice
                .Where(dice => dice.diceType == diceType)
                .OrderByDescending(dice => GetPlayerDieDesireScore(s, dice))
                .ThenByDescending(dice => dice.diceNum)
                .FirstOrDefault());
        }

        return likelyDice.Take(MaxLikelyPlayerReplyDice);
    }

    private static IEnumerable<string> GetHeuristicPlayerDraftAssignmentOptions(Scripts s, Dice dice) {
        IEnumerable<string> fullOptions = GetPlayerDraftAssignmentOptions(s, dice);
        if (dice == null || (dice.diceType != "yellow" && !Save.game.isFurious)) {
            return fullOptions;
        }

        if (IsPlayerGuardSelected(s)) {
            return fullOptions.Where(stat => stat == "white").DefaultIfEmpty(fullOptions.First());
        }

        List<string> options = new();
        int playerAim = s.statSummoner.SumOfStat("green", "player");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        bool playerSpeedLockedHigh = IsPlayerSpeedLockedHigh(s);
        bool enemySpeedLockedHigh = IsEnemySpeedLockedHigh(s);
        bool playerActsFirst = playerSpeedLockedHigh || (!enemySpeedLockedHigh && playerSpd >= enemySpd);
        bool playerCanHit = playerAim >= 0 && playerAtt > enemyDef;
        bool enemyThreatens = enemyAtt > playerDef;

        if (!playerActsFirst) { options.Add("blue"); }
        if (playerAim < GetPlayerDraftReferenceTargetIndex(s) || playerAim < 0) { options.Add("green"); }
        if (!playerCanHit || playerAtt <= enemyDef) { options.Add("red"); }
        if (!playerCanHit || enemyThreatens) { options.Add("white"); }
        if (options.Count == 0) { options.Add("red"); }

        return options
            .Where(stat => fullOptions.Contains(stat))
            .Distinct()
            .Take(3)
            .DefaultIfEmpty(fullOptions.First());
    }

    private static bool IsWorseAdvancedEvaluation(AdvancedPlanEvaluation candidate, AdvancedPlanEvaluation current) {
        if (candidate == null) { return false; }
        if (current == null) { return true; }
        return IsBetterAdvancedEvaluation(current, candidate);
    }

    /// <summary>
    /// evaluate the best real post-pick advanced plan for a drafted die preview
    /// </summary>
    private static AdvancedPlanEvaluation GetBestDraftPreviewEvaluation(
        Scripts s,
        PlannerSnapshot snapshot,
        Dictionary<string, int> previewTotals,
        Dictionary<string, int> previewCounts
    ) {
        if (s == null || snapshot == null) { return null; }

        Dictionary<string, int> staminaPlan = NewStatDictionary();
        AdvancedPlanEvaluation best = null;
        int totalAvailableStamina = Mathf.Max(0, s.enemy.stamina);
        bool canUseStamina = (!s.enemy.woundList.Contains("hip") && !s.itemManager.EnemyHasTemporaryHipInjury()) || s.enemy.enemyName.text == "Lich";
        int baseAim = snapshot.EnemyBaseAim + previewTotals["green"];
        int baseSpd = snapshot.EnemyBaseSpd + previewTotals["blue"];
        int baseAtt = snapshot.EnemyBaseAtt + previewTotals["red"];
        int baseDef = snapshot.EnemyBaseDef + previewTotals["white"];
        List<int> blueOptions = BuildSpeedSpendOptions(snapshot, canUseStamina, totalAvailableStamina, baseSpd, baseAtt);

        foreach (int blueSpend in blueOptions) {
            int remainingAfterBlue = totalAvailableStamina - blueSpend;
            List<int> whiteOptions = canUseStamina
                ? BuildDefenseSpendOptions(s, snapshot, previewTotals, previewCounts, remainingAfterBlue, baseDef, blueSpend)
                : BuildSpendOptions(0, 0);

            foreach (int whiteSpend in whiteOptions) {
                int remainingAfterWhite = remainingAfterBlue - whiteSpend;
                int postPlayerRedSpend = canUseStamina
                    ? GetPostPlayerWoundAttackSpendNeeded(s, snapshot, previewTotals, previewCounts, blueSpend, whiteSpend)
                    : 0;
                List<int> redOptions = canUseStamina
                    ? BuildSpendOptions(remainingAfterWhite, 0, GetExactAttackSpendNeeded(GetProjectedPlayerDefenseForEnemyAttack(snapshot, baseSpd + blueSpend), baseAtt), postPlayerRedSpend)
                    : BuildSpendOptions(0, 0);

                foreach (int redSpend in redOptions) {
                    int remainingAfterRed = remainingAfterWhite - redSpend;
                    int maxTarget = Mathf.Clamp(baseAim + remainingAfterRed, 0, 7);

                    staminaPlan["blue"] = blueSpend;
                    staminaPlan["white"] = whiteSpend;
                    staminaPlan["red"] = redSpend;

                    foreach (int targetIndex in GetTargetSearchOrder(maxTarget)) {
                        int greenSpend = canUseStamina ? GetExactAimSpendNeeded(targetIndex, baseAim) : 0;
                        if (greenSpend > remainingAfterRed) { continue; }

                        staminaPlan["green"] = greenSpend;
                        AdvancedPlanEvaluation candidate = EvaluateAdvancedPlanCandidate(s, snapshot, targetIndex, previewTotals, previewCounts, staminaPlan);
                        if (IsTrulyFutileAdvancedEvaluation(candidate)) { continue; }
                        if (!IsBetterAdvancedEvaluation(candidate, best)) { continue; }

                        best = candidate;
                        if (IsPerfectAdvancedEvaluation(best)) {
                            return best;
                        }
                    }
                }
            }
        }

        if (best != null) { return best; }

        staminaPlan["green"] = 0;
        staminaPlan["blue"] = 0;
        staminaPlan["red"] = 0;
        staminaPlan["white"] = 0;
        return EvaluateAdvancedPlanCandidate(s, snapshot, Mathf.Clamp(s.enemy.targetIndex, 0, Targets.Length - 1), previewTotals, previewCounts, staminaPlan);
    }

    private static bool IsBetterDraftOutcomePreview(AdvancedPlanEvaluation candidate, AdvancedPlanEvaluation current) {
        if (candidate == null) { return false; }
        if (current == null) { return true; }
        if (candidate.EnemyKills != current.EnemyKills) { return candidate.EnemyKills; }
        if (candidate.EnemyDamagesPlayer != current.EnemyDamagesPlayer) { return candidate.EnemyDamagesPlayer; }
        if (candidate.EnemyAvoidsKill != current.EnemyAvoidsKill) { return candidate.EnemyAvoidsKill; }
        if (candidate.EnemyAvoidsDamage != current.EnemyAvoidsDamage) { return candidate.EnemyAvoidsDamage; }
        return false;
    }

    private static bool IsBetterDraftPlanPreview(AdvancedPlanEvaluation candidate, AdvancedPlanEvaluation current) {
        if (candidate == null) { return false; }
        if (current == null) { return true; }
        if (candidate.EnemyKills != current.EnemyKills) { return candidate.EnemyKills; }
        if (candidate.EnemyDamagesPlayer != current.EnemyDamagesPlayer) { return candidate.EnemyDamagesPlayer; }
        if (candidate.EnemyAvoidsKill != current.EnemyAvoidsKill) { return candidate.EnemyAvoidsKill; }
        if (candidate.EnemyAvoidsDamage != current.EnemyAvoidsDamage) { return candidate.EnemyAvoidsDamage; }
        if (candidate.BreaksPlayerKill != current.BreaksPlayerKill) { return candidate.BreaksPlayerKill; }
        if (candidate.BreaksPlayerDamage != current.BreaksPlayerDamage) { return candidate.BreaksPlayerDamage; }
        if (candidate.BreaksPlayerProtection != current.BreaksPlayerProtection) { return candidate.BreaksPlayerProtection; }
        if (candidate.StripsPlayerStamina != current.StripsPlayerStamina) { return candidate.StripsPlayerStamina; }
        if (candidate.BreaksPlayerSpeed != current.BreaksPlayerSpeed) { return candidate.BreaksPlayerSpeed; }
        if (candidate.RemovesPlayerRed != current.RemovesPlayerRed) { return candidate.RemovesPlayerRed; }
        if (candidate.RemovesPlayerBestDie != current.RemovesPlayerBestDie) { return candidate.RemovesPlayerBestDie; }
        if (candidate.RemovesPlayerWhite != current.RemovesPlayerWhite) { return candidate.RemovesPlayerWhite; }
        if (candidate.BreaksPlayerTarget != current.BreaksPlayerTarget) { return candidate.BreaksPlayerTarget; }
        if (candidate.UsesChestOnHighValuePlayerDice != current.UsesChestOnHighValuePlayerDice) { return candidate.UsesChestOnHighValuePlayerDice; }
        if (candidate.UsesChestAsLastDitchGamble != current.UsesChestAsLastDitchGamble) { return candidate.UsesChestAsLastDitchGamble; }
        if (candidate.EnemyActsFirst != current.EnemyActsFirst) { return candidate.EnemyActsFirst; }
        return false;
    }

    private static bool IsBetterDraftChoice(DraftChoiceEvaluation candidate, DraftChoiceEvaluation current) {
        if (candidate == null) { return false; }
        if (current == null) { return true; }
        if (IsBetterDraftPlanPreview(candidate.BestPlan, current.BestPlan)) { return true; }
        if (IsBetterDraftPlanPreview(current.BestPlan, candidate.BestPlan)) { return false; }
        if (IsBetterDraftOutcomePreview(candidate.BestPlan, current.BestPlan)) { return true; }
        if (IsBetterDraftOutcomePreview(current.BestPlan, candidate.BestPlan)) { return false; }
        if (candidate.DieType == current.DieType && candidate.DieValue != current.DieValue) {
            return candidate.DieValue > current.DieValue;
        }
        if (candidate.DeniesPlayerKill != current.DeniesPlayerKill) { return candidate.DeniesPlayerKill; }
        if (candidate.DeniesPlayerDamage != current.DeniesPlayerDamage) { return candidate.DeniesPlayerDamage; }
        int candidateSpentStamina = candidate.BestPlan?.SpentStamina ?? int.MaxValue;
        int currentSpentStamina = current.BestPlan?.SpentStamina ?? int.MaxValue;
        if (candidateSpentStamina != currentSpentStamina) { return candidateSpentStamina < currentSpentStamina; }
        if (candidate.CompletesKillBreakpoint != current.CompletesKillBreakpoint) { return candidate.CompletesKillBreakpoint; }
        if (candidate.CompletesHitBreakpoint != current.CompletesHitBreakpoint) { return candidate.CompletesHitBreakpoint; }
        if (candidate.CompletesDefenseBreakpoint != current.CompletesDefenseBreakpoint) { return candidate.CompletesDefenseBreakpoint; }
        if (candidate.CompletesOrderBreakpoint != current.CompletesOrderBreakpoint) { return candidate.CompletesOrderBreakpoint; }
        if (candidate.CompletesArmpitsBreakpoint != current.CompletesArmpitsBreakpoint) { return candidate.CompletesArmpitsBreakpoint; }
        if (candidate.CompletesHeadBreakpoint != current.CompletesHeadBreakpoint) { return candidate.CompletesHeadBreakpoint; }
        if (candidate.DeniesPlayerGoFirst != current.DeniesPlayerGoFirst) { return candidate.DeniesPlayerGoFirst; }
        if (candidate.DeniesPlayerTarget != current.DeniesPlayerTarget) { return candidate.DeniesPlayerTarget; }
        if (!Mathf.Approximately(candidate.ProgressScore, current.ProgressScore)) { return candidate.ProgressScore > current.ProgressScore; }
        if (candidate.LosesValueToHatchet != current.LosesValueToHatchet) { return !candidate.LosesValueToHatchet; }
        if (candidate.DieValue == current.DieValue && candidate.IsYellow != current.IsYellow) { return candidate.IsYellow; }
        if (candidate.DieValue != current.DieValue) { return candidate.DieValue > current.DieValue; }
        if (IsBetterAdvancedEvaluation(candidate.BestPlan, current.BestPlan)) { return true; }
        if (IsBetterAdvancedEvaluation(current.BestPlan, candidate.BestPlan)) { return false; }
        if (!Mathf.Approximately(candidate.PlayerDenialScore, current.PlayerDenialScore)) { return candidate.PlayerDenialScore > current.PlayerDenialScore; }
        if (!Mathf.Approximately(candidate.FallbackScore, current.FallbackScore)) { return candidate.FallbackScore > current.FallbackScore; }
        return false;
    }

    private static bool DraftDieDeniesPlayerKill(Scripts s, Dice dice) {
        if (IsPlayerGuardSelected(s)) { return false; }

        int playerAim = s.statSummoner.SumOfStat("green", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        string playerTarget = Targets[GetPlayerDraftReferenceTargetIndex(s)];
        bool currentKill = PlayerWouldKillEnemy(
            s,
            new SimState {
                PlayerAim = playerAim,
                PlayerAtt = playerAtt,
                EnemyDef = enemyDef,
                EnemyWoundCount = s.enemy.woundList.Count,
                PlayerHasMaul = s.itemManager.PlayerHasWeapon("maul"),
            },
            playerTarget,
            playerAim >= 0 && playerAtt > enemyDef
        );

        if (currentKill || effectivePlayerValue <= 0) { return false; }
        if (dice.diceType == "yellow") {
            return playerTarget == "neck" && playerAim < 7 && playerAim + effectivePlayerValue >= 7
                || playerAtt <= enemyDef && playerAtt + effectivePlayerValue > enemyDef
                || s.itemManager.PlayerHasWeapon("maul");
        }
        if (dice.diceType == "green") {
            return playerTarget == "neck" && playerAim < 7 && playerAim + effectivePlayerValue >= 7;
        }
        if (dice.diceType == "red") {
            return playerAtt <= enemyDef && playerAtt + effectivePlayerValue > enemyDef;
        }
        return false;
    }

    private static bool DraftDieDeniesPlayerDamage(Scripts s, Dice dice) {
        if (IsPlayerGuardSelected(s)) { return false; }

        int playerAim = s.statSummoner.SumOfStat("green", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        bool currentDamage = playerAim >= 0 && playerAtt > enemyDef;
        if (currentDamage || effectivePlayerValue <= 0) { return false; }
        if (dice.diceType == "yellow") {
            return playerAtt <= enemyDef && playerAtt + effectivePlayerValue > enemyDef
                || playerAim < 0 && playerAim + effectivePlayerValue >= 0;
        }
        if (dice.diceType == "red") {
            return playerAtt <= enemyDef && playerAtt + effectivePlayerValue > enemyDef;
        }
        if (dice.diceType == "green") {
            return playerAim < 0 && playerAim + effectivePlayerValue >= 0;
        }
        return false;
    }

    private static bool DraftDieDeniesPlayerGoFirst(Scripts s, Dice dice) {
        if (IsDraftInitiativeLocked(s)) { return false; }
        if (PlayerAlwaysActsFirst(s)) { return false; }
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        if (playerSpd >= enemySpd || effectivePlayerValue <= 0) { return false; }
        return (dice.diceType == "blue" || dice.diceType == "yellow") && playerSpd + effectivePlayerValue >= enemySpd;
    }

    private static bool DraftDieDeniesPlayerTarget(Scripts s, Dice dice) {
        if (IsPlayerGuardSelected(s)) { return false; }

        int playerAim = s.statSummoner.SumOfStat("green", "player");
        int neededAim = GetPlayerDraftReferenceTargetIndex(s);
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        if (playerAim >= neededAim || effectivePlayerValue <= 0) { return false; }
        return (dice.diceType == "green" || dice.diceType == "yellow") && playerAim + effectivePlayerValue >= neededAim;
    }

    private static IEnumerable<string> GetDraftAssignmentOptions(Scripts s, Dice dice) {
        if (dice.diceType != "yellow") {
            if (dice.diceType == "green" && s.itemManager.PlayerHasWeapon("dagger")) { return new[] { "green" }; }
            if (dice.diceType == "white" && Save.game.curCharNum == 3) { return new[] { "white" }; }
            return new[] { dice.diceType };
        }

        return IsDraftInitiativeLocked(s)
            ? new[] { "green", "red", "white" }
            : Stats;
    }

    private static int GetEffectiveTriggeredPlayerCharmBonus(Scripts s, string modifier, int amountPerTrigger = 1) {
        if (s == null || s.itemManager == null || string.IsNullOrEmpty(modifier)) { return 0; }
        return s.itemManager.GetEffectiveCharmCount(modifier) * amountPerTrigger;
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
        if (s != null && dice != null && dice.diceType == "blue" && IsDraftInitiativeLocked(s)) { return 0f; }

        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        if (IsPlayerGuardSelected(s)) {
            int playerDef = s.statSummoner.SumOfStat("white", "player");
            int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
            float guardScore = effectivePlayerValue * 7f;

            if (dice.diceType == "yellow") { guardScore += 70f; }
            if (dice.diceType == "white" || dice.diceType == "yellow") {
                guardScore += Mathf.Max(0, enemyAtt + 1 - playerDef) * 22f;
            }
            if (dice.diceType == "blue") {
                guardScore += 10f;
            }

            return guardScore;
        }

        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int playerAim = s.statSummoner.SumOfStat("green", "player");
        float score = effectivePlayerValue * 7f;

        if (dice.diceType == "yellow") { score += 120f; }
        if (dice.diceType == "red" || dice.diceType == "green" && s.itemManager.PlayerHasWeapon("dagger") || dice.diceType == "white" && Save.game.curCharNum == 3) {
            score += Mathf.Max(0, enemyDef + 1 - playerAtt) * 20f;
        }
        if (dice.diceType == "blue") {
            score += Mathf.Max(0, enemySpd + 1 - playerSpd) * 18f;
        }
        if (dice.diceType == "green") {
            score += Mathf.Max(0, GetPlayerDraftReferenceTargetIndex(s) - playerAim) * 22f;
        }
        if (dice.diceType == "white") {
            score += Mathf.Max(0, s.statSummoner.SumOfStat("red", "enemy") - s.statSummoner.SumOfStat("white", "player")) * 14f;
        }
        if (PlayerHasOneShotProtection(s) && dice.diceType == "blue") {
            score -= 25f;
        }

        return score;
    }

    private static void AttachChosenDie(Scripts s, Dice chosenDie) {
        if ((s.enemy.woundList.Contains("guts") || s.itemManager.EnemyHasTemporaryGutsInjury()) && s.enemy.enemyName.text != "Lich") {
            s.enemy.StartCoroutine(chosenDie.DecreaseDiceValue(false));
        }

        chosenDie.isAttached = true;
        chosenDie.moveable = false;
        chosenDie.isOnPlayerOrEnemy = "enemy";

        string attachStat = chosenDie.diceType == "yellow" && DifficultyHelper.UsesAdvancedEnemyAI(Save.persistent.gameDifficulty)
            ? ChooseBestYellowStatForDraft(s, chosenDie)
            : (chosenDie.diceType == "yellow" ? "red" : chosenDie.diceType);

        chosenDie.statAddedTo = attachStat;
        s.statSummoner.AddDiceToEnemy(attachStat, chosenDie);
        s.itemManager.TryWeakenEnemyTakenDieWithTarot(chosenDie, 0.05f);
        RepositionEnemyDice(s);

        if (chosenDie.diceType == "red" && (s.enemy.woundList.Contains("armpits") || s.itemManager.EnemyHasTemporaryArmpitsInjury())
            || chosenDie.diceType == "white" && (s.enemy.woundList.Contains("hand") || s.itemManager.EnemyHasTemporaryHandInjury())) {
            if (s.enemy.enemyName.text != "Lich") { s.enemy.StartCoroutine(chosenDie.FadeOut(false)); }
        }
        else if (chosenDie.diceType == "yellow" && s.itemManager.PlayerHasWeapon("hatchet")) {
            s.enemy.StartCoroutine(chosenDie.FadeOut(false));
        }

        s.enemy.targetIndex = GetBestTargetIndex(s);
        s.turnManager.SetTargetOf("enemy");
        s.statSummoner.SetDebugInformationFor("enemy");

        if (s.diceSummoner.CountUnattachedDice() == 0) {
            s.turnManager.RecalculateMaxFor("player");
            s.turnManager.RecalculateMaxFor("enemy");
            s.turnManager.RefreshEnemyPlanIfNeeded();
        }
    }

    private static string ChooseBestYellowStatForDraft(Scripts s, Dice yellowDie) {
        string bestStat = "red";
        AdvancedPlanEvaluation bestEvaluation = null;
        PlannerSnapshot snapshot = BuildPlannerSnapshot(s);
        List<Dice> remainingDice = s.diceSummoner.existingDice
            .Select(obj => obj.GetComponent<Dice>())
            .Where(dice => dice != null && !dice.isAttached)
            .ToList();
        foreach (string stat in Stats) {
            AdvancedPlanEvaluation evaluation = GetDraftPreviewEvaluation(s, snapshot, yellowDie, stat, remainingDice, null);
            if (IsBetterAdvancedEvaluation(evaluation, bestEvaluation)) {
                bestEvaluation = evaluation;
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

    private static bool PlanDamagesPlayer(Scripts s, Plan plan) {
        if (s == null || plan == null) { return false; }

        PlannerSnapshot snapshot = BuildPlannerSnapshot(s);
        AdvancedPlanEvaluation evaluation = EvaluatePlanOutcome(s, snapshot, plan);
        return evaluation != null && evaluation.EnemyDamagesPlayer;
    }

    private static Plan CaptureCurrentEnemyPlanState(Scripts s) {
        if (s == null) { return null; }

        Plan currentPlan = new() {
            TargetIndex = Mathf.Clamp(s.enemy.targetIndex, 0, Targets.Length - 1),
            Stamina = CopyStatDictionary(s.statSummoner.addedEnemyStamina),
        };

        foreach (Dice yellowDie in GetEnemyYellowDice(s)) {
            if (yellowDie == null) { continue; }
            currentPlan.YellowAssignments[yellowDie] = GetCurrentEnemyYellowAssignment(yellowDie);
        }

        return currentPlan;
    }

    private static AdvancedPlanEvaluation EvaluatePlanOutcome(Scripts s, PlannerSnapshot snapshot, Plan plan) {
        if (s == null || snapshot == null || plan == null) { return null; }

        Dictionary<string, int> yellowTotals = NewStatDictionary();
        Dictionary<string, int> yellowCounts = NewStatDictionary();

        foreach (KeyValuePair<Dice, string> assignment in plan.YellowAssignments) {
            if (assignment.Key == null || !StatIndexByName.ContainsKey(assignment.Value)) { continue; }
            yellowTotals[assignment.Value] += assignment.Key.diceNum;
            yellowCounts[assignment.Value] += 1;
        }

        return EvaluateAdvancedPlanCandidate(
            s,
            snapshot,
            Mathf.Clamp(plan.TargetIndex, 0, Targets.Length - 1),
            yellowTotals,
            yellowCounts,
            plan.Stamina);
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

    private static IEnumerable<Dice> GetPlayerYellowDice(Scripts s) {
        return s.statSummoner.addedPlayerDice
            .SelectMany(pair => pair.Value)
            .Where(dice => dice != null && dice.isAttached && dice.isOnPlayerOrEnemy == "player" && dice.diceType == "yellow")
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
        int playerAim = s.statSummoner.SumOfStat("green", "player");
        string stat = string.IsNullOrEmpty(dice.statAddedTo)
            ? (dice.diceType == "yellow" ? "red" : dice.diceType)
            : dice.statAddedTo;
        int afterAim = playerAim - (stat == "green" ? dice.diceNum : 0);
        int afterSpd = playerSpd - (stat == "blue" ? dice.diceNum : 0);
        int afterAtt = playerAtt - (stat == "red" ? dice.diceNum : 0);
        int afterDef = s.statSummoner.SumOfStat("white", "player") - (stat == "white" ? dice.diceNum : 0);
        bool beforeCanHit = playerAim >= 0 && playerAtt > enemyDef;
        bool afterCanHit = afterAim >= 0 && afterAtt > enemyDef;
        bool beforeKills = PlayerWouldKillEnemy(
            s,
            new SimState {
                PlayerAim = playerAim,
                PlayerAtt = playerAtt,
                EnemyDef = enemyDef,
                EnemyWoundCount = s.enemy.woundList.Count,
                PlayerHasMaul = s.itemManager.PlayerHasWeapon("maul"),
            },
            GetPlayerTargetName(s.player.targetIndex, IsPlayerGuardSelected(s)),
            beforeCanHit
        );
        bool afterKills = PlayerWouldKillEnemy(
            s,
            new SimState {
                PlayerAim = afterAim,
                PlayerAtt = afterAtt,
                EnemyDef = enemyDef,
                EnemyWoundCount = s.enemy.woundList.Count,
                PlayerHasMaul = s.itemManager.PlayerHasWeapon("maul"),
            },
            GetPlayerTargetName(s.player.targetIndex, IsPlayerGuardSelected(s)),
            afterCanHit
        );

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
            score += IsPlayerGuardSelected(s) ? 0f : Mathf.Max(0, s.player.targetIndex - s.statSummoner.SumOfStat("green", "player") + 1) * 25f;
        }
        if (dice.statAddedTo == "white") {
            score += 30f;
            score += Mathf.Max(0, s.statSummoner.SumOfStat("red", "enemy") - s.statSummoner.SumOfStat("white", "player")) * 14f;
        }
        if (dice.diceType == "yellow") { score += 50f; }
        if (beforeCanHit && !afterCanHit) { score += 500f; }
        if (playerSpd >= enemySpd && afterSpd < enemySpd) { score += 320f; }
        if (!IsPlayerGuardSelected(s) && playerAim >= s.player.targetIndex && afterAim < s.player.targetIndex) { score += 360f; }
        if (s.statSummoner.SumOfStat("red", "enemy") > s.statSummoner.SumOfStat("white", "player")
            && s.statSummoner.SumOfStat("red", "enemy") <= afterDef) {
            score += 220f;
        }
        if (beforeKills && !afterKills) { score += 800f; }
        return score;
    }

    private static LiveDiscardEvaluation EvaluateLiveDiscardChoice(Scripts s, Dice dice) {
        SimState state = BuildLiveDiscardState(s);
        string stat = string.IsNullOrEmpty(dice.statAddedTo)
            ? (dice.diceType == "yellow" ? "red" : dice.diceType)
            : dice.statAddedTo;
        SimAttachedDie attachedDie = new() {
            Stat = stat,
            Value = dice.diceNum,
            IsRerolled = dice.isRerolled,
            IsYellow = dice.diceType == "yellow",
        };
        string playerTarget = GetPlayerTargetName(s.player.targetIndex, IsPlayerGuardSelected(s));
        return EvaluatePlayerDiscardChoice(s, state, playerTarget, attachedDie, enemyAlreadyActed:true);
    }

    private static SimState BuildLiveDiscardState(Scripts s) {
        return new SimState {
            PlayerAim = s.statSummoner.SumOfStat("green", "player"),
            PlayerSpd = s.statSummoner.SumOfStat("blue", "player"),
            PlayerAtt = s.statSummoner.SumOfStat("red", "player"),
            PlayerDef = s.statSummoner.SumOfStat("white", "player"),
            PlayerGuardSelected = IsPlayerGuardSelected(s),
            PlayerPendingGuardParryBonus = s.turnManager != null ? s.turnManager.GetPlayerGuardParryBonus(includePendingSelection: true) : 0,
            EnemyAim = s.statSummoner.SumOfStat("green", "enemy"),
            EnemySpd = s.statSummoner.SumOfStat("blue", "enemy"),
            EnemyAtt = s.statSummoner.SumOfStat("red", "enemy"),
            EnemyDef = s.statSummoner.SumOfStat("white", "enemy"),
            EnemyWoundCount = s.enemy.woundList.Count,
            PlayerHasMaul = s.itemManager.PlayerHasWeapon("maul"),
            PlayerSpeedLockedHigh = IsPlayerSpeedLockedHigh(s),
            EnemySpeedLockedHigh = IsEnemySpeedLockedHigh(s),
        };
    }

    private static LiveDiscardEvaluation EvaluatePlayerDiscardChoice(Scripts s, SimState state, string playerTarget, SimAttachedDie die, bool enemyAlreadyActed) {
        int afterAim = state.PlayerAim - (die.Stat == "green" ? die.Value : 0);
        int afterSpd = state.PlayerSpd - (die.Stat == "blue" ? die.Value : 0);
        int afterAtt = state.PlayerAtt - (die.Stat == "red" ? die.Value : 0);
        int afterDef = state.PlayerDef - (die.Stat == "white" ? die.Value : 0);
        bool beforeCanHit = !state.PlayerGuardSelected && state.PlayerAim >= 0 && state.PlayerAtt > state.EnemyDef;
        bool afterCanHit = !state.PlayerGuardSelected && afterAim >= 0 && afterAtt > state.EnemyDef;
        bool beforeKills = PlayerWouldKillEnemy(s, state, playerTarget, beforeCanHit);

        SimState afterState = state.Clone();
        afterState.PlayerAim = afterAim;
        afterState.PlayerSpd = afterSpd;
        afterState.PlayerAtt = afterAtt;
        afterState.PlayerDef = afterDef;
        bool afterKills = PlayerWouldKillEnemy(s, afterState, playerTarget, afterCanHit);
        bool beforeActsFirst = !enemyAlreadyActed && PlayerActsFirstInState(state);
        bool afterActsFirst = !enemyAlreadyActed && PlayerActsFirstInState(afterState);

        return new LiveDiscardEvaluation {
            BreaksKill = beforeKills && !afterKills,
            BreaksDamage = beforeCanHit && !afterCanHit,
            BreaksGoFirst = beforeActsFirst && !afterActsFirst,
            BreaksTarget = !IsPlayerGuardSelected(s) && state.PlayerAim >= s.player.targetIndex && afterAim < s.player.targetIndex,
            RestoresDefense = state.EnemyAtt > state.PlayerDef && state.EnemyAtt <= afterDef,
            IsYellow = die.IsYellow,
            DieValue = GetDiscardTieBreakValue(die, state, enemyAlreadyActed),
        };
    }

    private static bool PlayerActsFirstInState(SimState state) {
        return state != null && (state.PlayerSpeedLockedHigh || (!state.EnemySpeedLockedHigh && state.PlayerSpd >= state.EnemySpd));
    }

    private static int GetDiscardTieBreakValue(SimAttachedDie die, SimState state, bool enemyAlreadyActed) {
        if (die == null) { return 0; }
        if (die.Stat == "blue" && (enemyAlreadyActed || state?.PlayerSpeedLockedHigh == true || state?.EnemySpeedLockedHigh == true)) {
            return 0;
        }

        return die.Value;
    }

    private static bool IsBetterLiveDiscardChoice(LiveDiscardEvaluation candidate, LiveDiscardEvaluation current) {
        if (candidate == null) { return false; }
        if (current == null) { return true; }
        if (candidate.BreaksKill != current.BreaksKill) { return candidate.BreaksKill; }
        if (candidate.BreaksDamage != current.BreaksDamage) { return candidate.BreaksDamage; }
        if (candidate.BreaksGoFirst != current.BreaksGoFirst) { return candidate.BreaksGoFirst; }
        if (candidate.BreaksTarget != current.BreaksTarget) { return candidate.BreaksTarget; }
        if (candidate.RestoresDefense != current.RestoresDefense) { return candidate.RestoresDefense; }
        if (candidate.IsYellow != current.IsYellow) { return candidate.IsYellow; }
        if (candidate.DieValue != current.DieValue) { return candidate.DieValue > current.DieValue; }
        return false;
    }

    private static bool EnemyHitAppliesWound(Scripts s, SimState state, string enemyTarget, bool enemyCanHit, bool enemyActsFirst) {
        if (!EnemyHitConnects(state, enemyCanHit, enemyActsFirst)) { return false; }
        if (PlayerHasOneShotProtection(state)) { return false; }
        return enemyTarget == "neck" || !s.player.woundList.Contains(enemyTarget);
    }

    private static bool PlayerHitAppliesWound(Scripts s, string playerTarget, bool playerCanHit) {
        if (!playerCanHit) { return false; }
        return playerTarget == "neck" || !s.enemy.woundList.Contains(playerTarget);
    }

    private static bool PlayerHasHighValueDice(SimState state) {
        return state.PlayerGreenDiceSum >= 5
            || state.PlayerBlueDiceSum >= 5
            || state.PlayerRedDiceSum >= 5
            || state.PlayerWhiteDiceSum >= 5
            || state.PlayerGreenDiceCount + state.PlayerBlueDiceCount + state.PlayerRedDiceCount + state.PlayerWhiteDiceCount >= 4;
    }

    private static IEnumerable<int> GetTargetSearchOrder(int maxTarget) {
        foreach (int targetIndex in PreferredTargetSearchOrder) {
            if (targetIndex <= maxTarget) { yield return targetIndex; }
        }
    }

    private static string[] GetYellowSearchOrder(string currentStat) {
        return currentStat switch {
            "green" => YellowSearchOrders[0],
            "blue" => YellowSearchOrders[1],
            "red" => YellowSearchOrders[2],
            "white" => YellowSearchOrders[3],
            _ => YellowSearchOrders[2],
        };
    }

    private static int CreateAdvancedPlanCacheKey(Scripts s) {
        HashCode hash = new();
        hash.Add(DifficultyHelper.Normalize(Save.persistent.gameDifficulty));
        hash.Add(s.player.targetIndex);
        hash.Add(s.enemy.targetIndex);
        hash.Add(s.enemy.enemyName.text);

        AddStatDictionaryToHash(ref hash, s.player.stats);
        AddStatDictionaryToHash(ref hash, s.enemy.stats);
        AddStaminaDictionaryToHash(ref hash, s.statSummoner.addedPlayerStamina);
        AddStaminaDictionaryToHash(ref hash, s.statSummoner.addedEnemyStamina);

        hash.Add(s.player.stamina);
        hash.Add(s.enemy.stamina);
        hash.Add(s.player.isDead);
        hash.Add(Save.game.enemyIsDead);
        hash.Add(Save.game.isDodgy);
        hash.Add(Save.game.isDestructive);
        hash.Add(Save.game.isFortified);
        hash.Add(Save.game.isEmpowered);
        hash.Add(s.itemManager.PlayerHas("armor"));
        hash.Add(s.itemManager.PlayerHas("boots of dodge"));
        hash.Add(s.itemManager.PlayerHas("goggles"));
        hash.Add(Save.game.usedBoots);
        hash.Add(s.itemManager.GetPlayerItemCount("crystal shard"));
        hash.Add(s.itemManager.GetCharmCount("riposte"));
        hash.Add(s.itemManager.GetCharmCount("bulwark"));
        hash.Add(s.itemManager.GetCharmCount("vindictive"));
        hash.Add(s.itemManager.GetCharmCount("inevitable"));
        hash.Add(s.itemManager.GetCharmCount("arcane"));
        hash.Add(s.itemManager.charmActiveBonus["green"]);
        hash.Add(s.itemManager.charmActiveBonus["blue"]);
        hash.Add(s.itemManager.charmActiveBonus["red"]);
        hash.Add(s.itemManager.charmActiveBonus["white"]);
        hash.Add(s.itemManager.GetTarotCount("abyss"));
        hash.Add(s.itemManager.GetTarotCount("verdant"));
        hash.Add(s.itemManager.GetTarotCount("inferno"));
        hash.Add(s.itemManager.GetTarotCount("glacier"));
        hash.Add(s.itemManager.GetTarotCount("dawn"));
        hash.Add(s.itemManager.GetTarotCount("leviathan"));
        hash.Add(s.itemManager.GetTarotCount("viper"));
        hash.Add(s.itemManager.GetTarotCount("dragon"));
        hash.Add(s.itemManager.GetTarotCount("wyvern"));
        hash.Add(s.itemManager.GetTarotCount("phoenix"));
        hash.Add(s.itemManager.GetTarotCount("arcane"));
        hash.Add(s.itemManager.GetLuckyDiceRoundStatBonus("green"));
        hash.Add(s.itemManager.GetLuckyDiceRoundStatBonus("blue"));
        hash.Add(s.itemManager.GetLuckyDiceRoundStatBonus("red"));
        hash.Add(s.itemManager.GetLuckyDiceRoundStatBonus("white"));
        hash.Add(s.itemManager.PlayerHasWeapon("maul"));
        hash.Add(s.itemManager.PlayerHasWeapon("gladius"));
        hash.Add(s.itemManager.PlayerHasWeapon("scimitar"));
        hash.Add(s.itemManager.PlayerHasWeapon("spear"));
        hash.Add(s.itemManager.PlayerHasWeapon("spear"));
        hash.Add(s.itemManager.PlayerHasWeapon("gauntlets"));
        hash.Add(s.itemManager.PlayerHasWeapon("stave"));
        hash.Add(s.itemManager.PlayerHasWeapon("glass sword"));
        hash.Add(s.itemManager.PlayerHasWeapon("trident"));
        hash.Add(s.itemManager.PlayerHasLegendary());
        hash.Add(Save.game.isFirstCombatRoundOfEncounter);
        hash.Add(Save.game.glassSwordShattered);

        AddWoundsToHash(ref hash, s.player.woundList);
        AddWoundsToHash(ref hash, s.enemy.woundList);
        AddDiceListsToHash(ref hash, s.statSummoner.addedPlayerDice);
        AddDiceListsToHash(ref hash, s.statSummoner.addedEnemyDice);
        return hash.ToHashCode();
    }

    private static void AddStatDictionaryToHash(ref HashCode hash, Dictionary<string, int> stats) {
        foreach (string stat in Stats) {
            hash.Add(stats.TryGetValue(stat, out int value) ? value : 0);
        }
    }

    private static void AddStaminaDictionaryToHash(ref HashCode hash, Dictionary<string, int> stamina) {
        foreach (string stat in Stats) {
            hash.Add(stamina[stat]);
        }
    }

    private static void AddWoundsToHash(ref HashCode hash, List<string> wounds) {
        foreach (string target in Targets) {
            hash.Add(wounds.Contains(target));
        }
    }

    private static void AddDiceListsToHash(ref HashCode hash, Dictionary<string, List<Dice>> diceByStat) {
        int[] counts = new int[4 * 5 * 6 * 2];
        foreach (string stat in Stats) {
            int statIndex = StatIndexByName[stat];
            foreach (Dice dice in diceByStat[stat]) {
                if (dice == null) { continue; }
                int typeIndex = GetDiceTypeIndex(dice.diceType);
                if (typeIndex < 0) { continue; }
                int neckIndex = Mathf.Clamp(dice.diceNum, 1, 6) - 1;
                int rerollIndex = dice.isRerolled ? 1 : 0;
                counts[(statIndex * 60) + (typeIndex * 12) + (neckIndex * 2) + rerollIndex]++;
            }
        }

        for (int i = 0; i < counts.Length; i++) {
            hash.Add(counts[i]);
        }
    }

    private static bool PlayerAlwaysActsFirst(Scripts s) {
        return s != null && s.itemManager != null && s.itemManager.PlayerAlwaysActsFirst();
    }

    private static bool PlayerCanBecomeDodgy(Scripts s) {
        return s != null
            && s.itemManager != null
            && (Save.game.isDodgy || (!Save.game.usedBoots && s.player.stamina >= 1 && s.itemManager.PlayerHas("boots of dodge")));
    }

    private static int GetPlayerScimitarDiscardCount(Scripts s) {
        if (s == null || s.itemManager == null || !s.itemManager.PlayerHasWeapon("scimitar")) { return 0; }
        return s.itemManager.PlayerHasLegendary() ? 2 : 1;
    }

    private static int GetDiceTypeIndex(string diceType) {
        return diceType switch {
            "green" => 0,
            "blue" => 1,
            "red" => 2,
            "white" => 3,
            "yellow" => 4,
            _ => -1,
        };
    }

    private static List<int> BuildSpendOptions(int remaining, params int[] candidateSpends) {
        List<int> options = new();
        foreach (int spend in candidateSpends) {
            if (spend < 0 || spend > remaining || options.Contains(spend)) { continue; }
            options.Add(spend);
        }

        if (options.Count == 0) { options.Add(0); }
        options.Sort();
        return options;
    }

    private static List<int> BuildSpeedSpendOptions(
        PlannerSnapshot snapshot,
        bool canUseStamina,
        int totalAvailableStamina,
        int baseSpd,
        int baseAtt
    ) {
        List<int> options = canUseStamina
            ? BuildSpendOptions(totalAvailableStamina, 0, GetExactSpeedSpendNeeded(snapshot.PlayerSpd, baseSpd, snapshot.PlayerSpeedLockedHigh, snapshot.EnemySpeedLockedHigh))
            : BuildSpendOptions(0, 0);

        if (options.Count <= 1) { return options; }

        return options
            .Where(blueSpend => blueSpend == 0 || baseAtt + Mathf.Max(0, totalAvailableStamina - blueSpend) > GetProjectedPlayerDefenseForEnemyAttack(snapshot, baseSpd + blueSpend))
            .ToList();
    }

    private static List<int> BuildDefenseSpendOptions(
        Scripts s,
        PlannerSnapshot snapshot,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        int remainingAfterBlue,
        int baseDef,
        int blueSpend
    ) {
        return BuildSpendOptions(
            remainingAfterBlue,
            0,
            GetExactDefenseSpendNeeded(snapshot.PlayerAim, snapshot.PlayerAtt, baseDef),
            GetEnemyFirstParryCounterattackDefenseSpendNeeded(s, snapshot, yellowTotals, yellowCounts, baseDef, blueSpend),
            GetEnemyFirstWoundCounterattackDefenseSpendNeeded(s, snapshot, yellowTotals, yellowCounts, baseDef, blueSpend));
    }

    private static int GetExactAttackSpendNeeded(int playerDef, int enemyAtt) {
        if (enemyAtt > playerDef) { return 0; }
        return playerDef - enemyAtt + 1;
    }

    private static int GetExactSpeedSpendNeeded(int playerSpd, int enemySpd, bool playerSpeedLockedHigh, bool enemySpeedLockedHigh) {
        if (playerSpeedLockedHigh || enemySpeedLockedHigh || enemySpd > playerSpd) { return 0; }
        return playerSpd - enemySpd + 1;
    }

    private static int GetExactAimSpendNeeded(int targetIndex, int enemyAim) {
        if (enemyAim >= targetIndex) { return 0; }
        return targetIndex - enemyAim;
    }

    private static int GetExactDefenseSpendNeeded(int playerAim, int playerAtt, int enemyDef) {
        if (playerAim < 0 || enemyDef >= playerAtt) { return 0; }
        return playerAtt - enemyDef;
    }

    private static int GetEnemyFirstParryCounterattackDefenseSpendNeeded(
        Scripts s,
        PlannerSnapshot snapshot,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        int enemyDef,
        int blueSpend
    ) {
        if (s == null || snapshot == null) { return 0; }

        Dictionary<string, int> previewStamina = NewStatDictionary();
        previewStamina["blue"] = blueSpend;

        SimState previewState = CreateSimulationState(snapshot, yellowTotals, yellowCounts, previewStamina);
        bool enemyActsFirst = previewState.EnemySpeedLockedHigh || (!previewState.PlayerSpeedLockedHigh && previewState.EnemySpd > previewState.PlayerSpd);
        if (!enemyActsFirst) {
            return GetExactDefenseSpendNeeded(snapshot.PlayerAim, snapshot.PlayerAtt, enemyDef);
        }

        SimState afterParry = previewState.Clone();
        ApplyImmediatePlayerResponseAfterEnemyActsFirst(afterParry, s, true, false);
        return GetExactDefenseSpendNeeded(afterParry.PlayerAim, afterParry.PlayerAtt, enemyDef);
    }

    private static int GetEnemyFirstWoundCounterattackDefenseSpendNeeded(
        Scripts s,
        PlannerSnapshot snapshot,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        int enemyDef,
        int blueSpend
    ) {
        if (s == null || snapshot == null) { return 0; }

        Dictionary<string, int> previewStamina = NewStatDictionary();
        previewStamina["blue"] = blueSpend;

        SimState previewState = CreateSimulationState(snapshot, yellowTotals, yellowCounts, previewStamina);
        bool enemyActsFirst = previewState.EnemySpeedLockedHigh || (!previewState.PlayerSpeedLockedHigh && previewState.EnemySpd > previewState.PlayerSpd);
        if (!enemyActsFirst) {
            return GetExactDefenseSpendNeeded(snapshot.PlayerAim, snapshot.PlayerAtt, enemyDef);
        }

        SimState afterWound = previewState.Clone();
        ApplyImmediatePlayerResponseAfterEnemyActsFirst(afterWound, s, false, true);
        return GetExactDefenseSpendNeeded(afterWound.PlayerAim, afterWound.PlayerAtt, enemyDef);
    }

    private static bool IsPlayerSpeedLockedHigh(Scripts s) {
        return s != null && s.itemManager != null && s.itemManager.PlayerAlwaysActsFirst();
    }

    private static bool IsEnemySpeedLockedHigh(Scripts s) {
        return s != null && s.itemManager != null && s.itemManager.PlayerAlwaysActsLast();
    }

    private static bool IsDraftInitiativeLocked(Scripts s) {
        return s != null && (IsPlayerSpeedLockedHigh(s) || IsEnemySpeedLockedHigh(s));
    }

    /// <summary>
    /// estimate the red spend needed after the player's declared wound is applied first
    /// </summary>
    private static int GetPostPlayerWoundAttackSpendNeeded(
        Scripts s,
        PlannerSnapshot snapshot,
        Dictionary<string, int> yellowTotals,
        Dictionary<string, int> yellowCounts,
        int blueSpend,
        int whiteSpend
    ) {
        Dictionary<string, int> previewStamina = NewStatDictionary();
        previewStamina["blue"] = blueSpend;
        previewStamina["white"] = whiteSpend;

        SimState previewState = CreateSimulationState(snapshot, yellowTotals, yellowCounts, previewStamina);
        bool enemyActsFirst = previewState.EnemySpeedLockedHigh || (!previewState.PlayerSpeedLockedHigh && previewState.EnemySpd > previewState.PlayerSpd);
        if (enemyActsFirst) {
            return GetExactAttackSpendNeeded(GetEffectivePlayerDefenseForEnemyAttack(previewState, true), previewState.EnemyAtt);
        }

        string playerTarget = GetPlayerTargetName(snapshot.PlayerTargetIndex, snapshot.PlayerGuardSelected);
        bool playerCanHit = !previewState.PlayerGuardSelected && previewState.PlayerAim >= 0 && previewState.PlayerAtt > previewState.EnemyDef;
        bool playerDamages = PlayerHitDamagesEnemy(s, previewState, playerTarget, playerCanHit);
        if (!playerDamages || previewState.EnemyIsLich) {
            return GetExactAttackSpendNeeded(GetEffectivePlayerDefenseForEnemyAttack(previewState, false), previewState.EnemyAtt);
        }

        SimState afterPlayerHit = previewState.Clone();
        ApplyWoundToEnemy(afterPlayerHit, playerTarget, s);
        return GetExactAttackSpendNeeded(GetEffectivePlayerDefenseForEnemyAttack(afterPlayerHit, false), afterPlayerHit.EnemyAtt);
    }

    private static bool IsPlayerGuardSelected(Scripts s) {
        return s != null
            && s.turnManager != null
            && s.diceSummoner != null
            && s.diceSummoner.CountUnattachedDice() == 0
            && s.turnManager.IsPlayerGuarding();
    }

    private static int GetPlayerDraftReferenceTargetIndex(Scripts s) {
        if (s?.turnManager == null) { return 0; }
        return s.turnManager.GetPlayerDraftReferenceTargetIndex();
    }

    private static string GetPlayerTargetName(int targetIndex, bool playerGuardSelected) {
        if (playerGuardSelected || targetIndex < 0) { return null; }
        return Targets[Mathf.Clamp(targetIndex, 0, Targets.Length - 1)];
    }

    private static int GetProjectedPlayerDefenseForEnemyAttack(PlannerSnapshot snapshot, int projectedEnemySpd) {
        if (snapshot == null) { return 0; }

        bool enemyActsFirst = snapshot.EnemySpeedLockedHigh || (!snapshot.PlayerSpeedLockedHigh && projectedEnemySpd > snapshot.PlayerSpd);
        return snapshot.PlayerDef
            + snapshot.PlayerPendingGuardParryBonus
            + (enemyActsFirst ? snapshot.PlayerBulwarkImmediateParryBonus : 0);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    /// <summary>
    /// update profiler stats and log only on slow or sampled runs
    /// </summary>
    private static void RecordAdvancedPlanProfile(Stopwatch stopwatch, int yellowLeavesVisited, int candidatesEvaluated, int futileCandidatesSkipped) {
        stopwatch.Stop();
        double elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        advancedPlanProfileRuns++;
        advancedPlanProfileTotalMs += elapsedMs;
        advancedPlanProfileMaxMs = Math.Max(advancedPlanProfileMaxMs, elapsedMs);
        double averageMs = advancedPlanProfileTotalMs / advancedPlanProfileRuns;
        LastAdvancedPlanProfileSummary
            = $"advanced plan last={elapsedMs:F3}ms avg={averageMs:F3}ms max={advancedPlanProfileMaxMs:F3}ms yellow={yellowLeavesVisited} candidates={candidatesEvaluated} futile={futileCandidatesSkipped} cache={advancedPlanCacheHits}/{advancedPlanCacheHits + advancedPlanCacheMisses}";
        if (elapsedMs >= AdvancedPlanSlowLogThresholdMs || advancedPlanProfileRuns % AdvancedPlanProfileLogInterval == 0) {
            UnityEngine.Debug.Log(LastAdvancedPlanProfileSummary);
        }
    }
#endif

    /// <summary>
    /// handle the cheapest advanced case where only target choice can vary
    /// </summary>
    private static bool TryBuildZeroResourceAdvancedPlan(
        Scripts s,
        PlannerSnapshot snapshot,
        List<Dice> yellowDice,
        out Plan plan,
        out int candidatesEvaluated
    ) {
        plan = null;
        candidatesEvaluated = 0;
        bool canUseStamina = (!s.enemy.woundList.Contains("hip") && !s.itemManager.EnemyHasTemporaryHipInjury()) || snapshot.EnemyIsLich;
        int totalAvailableStamina = s.enemy.stamina + s.statSummoner.addedEnemyStamina.Values.Sum();
        if (yellowDice.Count > 0 || (canUseStamina && totalAvailableStamina > 0)) { return false; }

        Dictionary<string, int> zeroStats = NewStatDictionary();
        AdvancedPlanEvaluation bestEvaluation = null;
        int bestTargetIndex = GetDefaultTargetIndex(s, snapshot.EnemyBaseAim);
        int maxTarget = Mathf.Clamp(snapshot.EnemyBaseAim, 0, 7);

        foreach (int targetIndex in GetTargetSearchOrder(maxTarget)) {
            candidatesEvaluated++;
            AdvancedPlanEvaluation evaluation = EvaluateAdvancedPlanCandidate(s, snapshot, targetIndex, zeroStats, zeroStats, zeroStats);
            if (IsBetterAdvancedEvaluation(evaluation, bestEvaluation)) {
                bestEvaluation = evaluation;
                bestTargetIndex = targetIndex;
                if (IsPerfectAdvancedEvaluation(evaluation)) { break; }
            }
        }

        plan = CreateBaselinePlan(s);
        plan.TargetIndex = bestTargetIndex;
        return true;
    }

    private static int GetAttackOverspend(int playerDef, int enemyAtt, int spentRed) {
        if (spentRed <= 0) { return 0; }
        int enemyAttBefore = enemyAtt - spentRed;
        if (enemyAttBefore > playerDef) { return spentRed; }
        int needed = playerDef - enemyAttBefore + 1;
        return Mathf.Max(0, spentRed - needed);
    }

    private static int GetSpeedOverspend(int playerSpd, int enemySpd, int spentBlue, bool playerSpeedLockedHigh) {
        if (spentBlue <= 0) { return 0; }
        if (playerSpeedLockedHigh) { return spentBlue; }
        int enemySpdBefore = enemySpd - spentBlue;
        if (enemySpdBefore > playerSpd) { return spentBlue; }
        int needed = playerSpd - enemySpdBefore + 1;
        return Mathf.Max(0, spentBlue - needed);
    }

    private static int GetAimOverspend(int targetIndex, int enemyAim, int spentGreen) {
        if (spentGreen <= 0) { return 0; }
        int enemyAimBefore = enemyAim - spentGreen;
        if (enemyAimBefore >= targetIndex) { return spentGreen; }
        int needed = Mathf.Max(0, targetIndex - enemyAimBefore);
        return Mathf.Max(0, spentGreen - needed);
    }

    private static int GetDefenseOverspend(int playerAim, int playerAtt, int enemyDef, int spentWhite) {
        if (spentWhite <= 0) { return 0; }
        if (playerAim < 0) { return spentWhite; }
        int enemyDefBefore = enemyDef - spentWhite;
        if (enemyDefBefore >= playerAtt) { return spentWhite; }
        int needed = playerAtt - enemyDefBefore;
        return Mathf.Max(0, spentWhite - needed);
    }

    private static int GetAttackResourceOverspend(int playerDef, int enemyAtt) {
        if (enemyAtt <= playerDef) { return 0; }
        return enemyAtt - (playerDef + 1);
    }

    private static int GetSpeedResourceOverspend(int playerSpd, int enemySpd, bool playerSpeedLockedHigh) {
        if (playerSpeedLockedHigh || enemySpd <= playerSpd) { return 0; }
        return enemySpd - (playerSpd + 1);
    }

    private static int GetAimResourceOverspend(int targetIndex, int enemyAim) {
        if (enemyAim < targetIndex) { return 0; }
        return enemyAim - targetIndex;
    }

    private static int GetDefenseResourceOverspend(int playerAim, int playerAtt, int enemyDef) {
        if (playerAim < 0 || enemyDef < playerAtt) { return 0; }
        return enemyDef - playerAtt;
    }

    private static float GetFutileStaminaPenalty(Scripts s, SimState state, int targetIndex, Dictionary<string, int> staminaPlan) {
        float penalty = 0f;
        int enemyAttBefore = state.EnemyAtt - staminaPlan["red"];
        int enemyDefBefore = state.EnemyDef - staminaPlan["white"];
        int enemySpdBefore = state.EnemySpd - staminaPlan["blue"];
        int enemyAimBefore = state.EnemyAim - staminaPlan["green"];
        bool enemyActsFirstAfterSpend = state.EnemySpeedLockedHigh || (!state.PlayerSpeedLockedHigh && state.EnemySpd > state.PlayerSpd);

        if (!enemyActsFirstAfterSpend && PlayerCanBecomeDodgy(state)) {
            penalty += staminaPlan["red"] * 140f;
            penalty += staminaPlan["green"] * 110f;
        }

        if (staminaPlan["red"] > 0) {
            if (enemyAttBefore > state.PlayerDef) {
                penalty += staminaPlan["red"] * 140f;
            }
            else {
                int needed = state.PlayerDef - enemyAttBefore + 1;
                if (enemyAttBefore + staminaPlan["red"] <= state.PlayerDef) {
                    penalty += staminaPlan["red"] * 100f;
                }
                else if (staminaPlan["red"] > needed) {
                    penalty += (staminaPlan["red"] - needed) * 90f;
                }
            }
        }

        if (staminaPlan["white"] > 0) {
            if (state.PlayerAim < 0 || enemyDefBefore >= state.PlayerAtt) {
                penalty += staminaPlan["white"] * 120f;
            }
            else {
                int needed = state.PlayerAtt - enemyDefBefore;
                if (enemyDefBefore + staminaPlan["white"] < state.PlayerAtt) {
                    penalty += staminaPlan["white"] * 85f;
                }
                else if (staminaPlan["white"] > needed) {
                    penalty += (staminaPlan["white"] - needed) * 70f;
                }
            }
        }

        if (staminaPlan["blue"] > 0) {
            bool playerAlwaysFirst = state.PlayerSpeedLockedHigh;
            if (playerAlwaysFirst || enemySpdBefore > state.PlayerSpd) {
                penalty += staminaPlan["blue"] * 120f;
            }
            else {
                int needed = state.PlayerSpd - enemySpdBefore + 1;
                if (enemySpdBefore + staminaPlan["blue"] <= state.PlayerSpd) {
                    penalty += staminaPlan["blue"] * 80f;
                }
                else if (staminaPlan["blue"] > needed) {
                    penalty += (staminaPlan["blue"] - needed) * 70f;
                }
            }
        }

        if (staminaPlan["green"] > 0) {
            if (enemyAimBefore >= targetIndex) {
                penalty += staminaPlan["green"] * 90f;
            }
            else {
                int needed = Mathf.Max(0, targetIndex - enemyAimBefore);
                if (enemyAimBefore + staminaPlan["green"] < targetIndex) {
                    penalty += staminaPlan["green"] * 85f;
                }
                else if (staminaPlan["green"] > needed) {
                    penalty += (staminaPlan["green"] - needed) * 70f;
                }
            }

            if (targetIndex == 7 && PlayerHasOneShotProtection(state) && enemyAimBefore < 7) {
                penalty += staminaPlan["green"] * 40f;
            }
        }

        return penalty;
    }

    private static float GetPlayerDiscardImpactScore(Scripts s, SimState state, string stat, int value, float weight) {
        if (value <= 0) { return float.NegativeInfinity; }

        int afterAim = state.PlayerAim - (stat == "green" ? value : 0);
        int afterSpd = state.PlayerSpd - (stat == "blue" ? value : 0);
        int afterAtt = state.PlayerAtt - (stat == "red" ? value : 0);
        int afterDef = state.PlayerDef - (stat == "white" ? value : 0);
        bool playerGuardSelected = IsPlayerGuardSelected(s);
        string playerTarget = GetPlayerTargetName(s.player.targetIndex, playerGuardSelected);
        bool beforeCanHit = !playerGuardSelected && state.PlayerAim >= 0 && state.PlayerAtt > state.EnemyDef;
        bool afterCanHit = !playerGuardSelected && afterAim >= 0 && afterAtt > state.EnemyDef;
        bool beforeKills = PlayerWouldKillEnemy(s, state, playerTarget, beforeCanHit);

        SimState afterState = state.Clone();
        afterState.PlayerAim = afterAim;
        afterState.PlayerSpd = afterSpd;
        afterState.PlayerAtt = afterAtt;
        afterState.PlayerDef = afterDef;

        bool afterKills = PlayerWouldKillEnemy(s, afterState, playerTarget, afterCanHit);
        float score = value * weight * 10f;

        if (beforeCanHit && !afterCanHit) { score += 500f; }
        if (state.PlayerSpd >= state.EnemySpd && afterSpd < state.EnemySpd) { score += 320f; }
        if (!playerGuardSelected && state.PlayerAim >= s.player.targetIndex && afterAim < s.player.targetIndex) { score += 360f; }
        if (state.EnemyAtt > state.PlayerDef && state.EnemyAtt <= afterDef) { score += 220f; }
        if (beforeKills && !afterKills) { score += 800f; }

        return score;
    }

    private static float GetEnemyDiscardImpactScore(Scripts s, SimState state, string playerTarget, SimAttachedDie die) {
        if (state == null || die == null || die.Value <= 0) { return float.NegativeInfinity; }

        int afterDef = state.EnemyDef - (die.Stat == "white" ? die.Value : 0);
        bool beforeCanHit = state.PlayerAim >= 0 && state.PlayerAtt > state.EnemyDef;
        bool afterCanHit = state.PlayerAim >= 0 && state.PlayerAtt > afterDef;
        bool beforeKills = PlayerWouldKillEnemy(s, state, playerTarget, beforeCanHit);

        SimState afterState = state.Clone();
        afterState.EnemyDef = afterDef;
        bool afterKills = PlayerWouldKillEnemy(s, afterState, playerTarget, afterCanHit);

        float score = die.Value * 10f;
        if (!beforeCanHit && afterCanHit) { score += 900f; }
        if (!beforeKills && afterKills) { score += 1400f; }
        if (die.Stat == "white") { score += 280f; }
        if (die.IsYellow) { score += 60f; }
        return score;
    }

    static float GetDraftBreakpointBonus(Scripts s, Dice dice, string stat, int effectiveEnemyValue) {
        int enemyAim = s.statSummoner.SumOfStat("green", "enemy");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        float score = 0f;
        if (effectiveEnemyValue <= 0) { return dice.diceType == "yellow" ? 60f : 0f; }

        switch (stat) {
            case "red":
                if (enemyAtt <= playerDef && enemyAtt + effectiveEnemyValue > playerDef) { score += 240f; }
                else if (enemyAtt > playerDef) { score -= 110f; }
                break;
            case "blue":
                if (PlayerAlwaysActsFirst(s)) {
                    score -= 180f;
                }
                else if (enemySpd <= playerSpd && enemySpd + effectiveEnemyValue > playerSpd) {
                    score += 220f;
                }
                else if (enemySpd > playerSpd) {
                    score -= 100f;
                }
                if (PlayerCanBecomeDodgy(s) && !PlayerAlwaysActsFirst(s) && enemySpd <= playerSpd && enemySpd + effectiveEnemyValue > playerSpd) {
                    score += 320f;
                }
                if (PlayerHasOneShotProtection(s)) { score -= 45f; }
                break;
            case "green":
                if (enemyAim < 6 && enemyAim + effectiveEnemyValue >= 6) { score += 220f; }
                else if (enemyAim < 4 && enemyAim + effectiveEnemyValue >= 4) { score += 180f; }
                else if (enemyAim < 3 && enemyAim + effectiveEnemyValue >= 3) { score += 140f; }
                else if (!IsPlayerGuardSelected(s) && enemyAim >= Mathf.Clamp(s.player.targetIndex, 0, 7)) { score -= 70f; }
                break;
            case "white":
                if (enemyDef < playerAtt && enemyDef + effectiveEnemyValue >= playerAtt) { score += 200f; }
                else if (enemyDef >= playerAtt) { score -= 90f; }
                break;
        }

        if (dice.diceType == "yellow") { score += 60f; }
        return score;
    }

    private static float GetDraftProgressBonus(Scripts s, string stat, int effectiveEnemyValue) {
        if (s == null || effectiveEnemyValue <= 0) { return 0f; }

        int enemyAim = s.statSummoner.SumOfStat("green", "enemy");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int futureStamina = ((!s.enemy.woundList.Contains("hip") && !s.itemManager.EnemyHasTemporaryHipInjury()) || s.enemy.enemyName.text == "Lich")
            ? Mathf.Max(0, s.enemy.stamina)
            : 0;
        float score = 0f;

        switch (stat) {
            case "red": {
                int currentGap = Mathf.Max(0, playerDef + 1 - enemyAtt);
                int nextGap = Mathf.Max(0, playerDef + 1 - (enemyAtt + effectiveEnemyValue + futureStamina));
                score += (currentGap - nextGap) * 32f;
                break;
            }
            case "white": {
                int currentGap = Mathf.Max(0, playerAtt - enemyDef);
                int nextGap = Mathf.Max(0, playerAtt - (enemyDef + effectiveEnemyValue + futureStamina));
                int gapReduction = currentGap - nextGap;
                score += gapReduction * 36f;
                if (enemyAtt <= playerDef) {
                    score += gapReduction * 14f;
                }
                break;
            }
            case "blue": {
                if (!PlayerAlwaysActsFirst(s)) {
                    int currentGap = Mathf.Max(0, playerSpd - enemySpd + 1);
                    int nextGap = Mathf.Max(0, playerSpd - (enemySpd + effectiveEnemyValue + futureStamina) + 1);
                    score += (currentGap - nextGap) * 24f;
                }
                if (PlayerCanBecomeDodgy(s) && !PlayerAlwaysActsFirst(s) && enemySpd <= playerSpd) {
                    score += effectiveEnemyValue * 28f;
                }
                break;
            }
            case "green": {
                int nextBreakpoint = enemyAim < 4 ? 4 : enemyAim < 6 ? 6 : enemyAim < 7 ? 7 : -1;
                if (nextBreakpoint > 0) {
                    int currentGap = Mathf.Max(0, nextBreakpoint - enemyAim);
                    int nextGap = Mathf.Max(0, nextBreakpoint - (enemyAim + effectiveEnemyValue + futureStamina));
                    score += (currentGap - nextGap) * 18f;
                }
                break;
            }
        }

        return score;
    }

    private static float GetDraftOvercommitPenalty(Scripts s, Dice dice, string stat, int effectiveEnemyValue) {
        int enemyAim = s.statSummoner.SumOfStat("green", "enemy");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        float penalty = 0f;
        if (effectiveEnemyValue <= 0) { return penalty; }

        if (stat == "red" && enemyAtt > playerDef) {
            penalty += 120f + effectiveEnemyValue * 10f;
        }
        if (stat == "white" && enemyDef >= playerAtt) {
            penalty += 100f;
        }
        if (stat == "blue" && (enemySpd > playerSpd || PlayerAlwaysActsFirst(s))) {
            penalty += 130f;
        }
        if ((stat == "red" || stat == "green") && PlayerCanBecomeDodgy(s) && !PlayerAlwaysActsFirst(s) && enemySpd <= playerSpd) {
            penalty += 220f;
        }
        if (stat == "green" && !IsPlayerGuardSelected(s) && enemyAim >= Mathf.Clamp(s.player.targetIndex, 0, 7)) {
            penalty += 80f;
        }

        return penalty;
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
