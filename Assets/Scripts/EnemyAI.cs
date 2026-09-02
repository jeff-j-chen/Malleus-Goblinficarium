using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Profiling;
using UnityEngine;

/// <summary>
/// Central enemy-planning system. It chooses draft dice, stamina spending, yellow die routing,
/// and target selection across all difficulties, with the hard/nightmare planner simulating both
/// sides to rank candidate plans lexicographically.
/// ENEMY_AI.md is the behavior spec; this file is the runtime implementation of that spec.
/// </summary>
public static class EnemyAI {
    private static readonly string[] Stats = { "green", "blue", "red", "white" }; // canonical enemy/player stat keys used throughout planning
    private static readonly string[] Targets = { "chest", "guts", "knee", "hip", "head", "hand", "armpits", "neck" }; // wound target names by target index
    private static readonly int[] PreferredTargetSearchOrder = { 7, 6, 4, 5, 3, 2, 1, 0 }; // fallback preference order when multiple targets are legal
    private const int AdvancedPlanProfileLogInterval = 10; // how often profiling summaries are refreshed in dev builds
    private const double AdvancedPlanSlowLogThresholdMs = 8d; // dev-build threshold for logging unusually slow advanced plans
    private const int MaxLikelyPlayerReplyDice = 5; // cap used when previewing likely player yellow-die reply states
    private static readonly string[][] YellowSearchOrders = {
        new[] { "green", "blue", "red", "white" },
        new[] { "blue", "green", "red", "white" },
        new[] { "red", "green", "blue", "white" },
        new[] { "white", "green", "blue", "red" },
    }; // preferred yellow re-assignment search order keyed by the die's current row
    private static readonly ProfilerMarker BuildAdvancedPlanProfiler = new("EnemyAI.BuildAdvancedPlan"); // profiler marker around hard/nightmare planning
    private static readonly Dictionary<string, int> StatIndexByName = new() {
        { "green", 0 },
        { "blue", 1 },
        { "red", 2 },
        { "white", 3 },
    }; // maps stat names to shared array indices used in snapshots and helpers
    private static readonly Dictionary<string, int> DefaultDieRanks = new() {
        { "yellow6", 0 }, { "red6", 1 }, { "white6", 2 }, { "yellow5", 3 }, { "red5", 4 }, { "white5", 5 },
        { "yellow4", 6 }, { "red4", 7 }, { "white4", 8 }, { "yellow3", 9 }, { "red3", 10 }, { "white3", 11 },
        { "green6", 12 }, { "yellow2", 13 }, { "red2", 14 }, { "white2", 15 }, { "yellow1", 16 }, { "red1", 17 },
        { "white1", 18 }, { "green5", 19 }, { "green4", 20 }, { "blue6", 21 }, { "green3", 22 }, { "blue5", 23 },
        { "blue4", 24 }, { "green2", 25 }, { "blue3", 26 }, { "green1", 27 }, { "blue2", 28 }, { "blue1", 29 },
    }; // legacy draft ranking used by easy/normal heuristics and fallback sorting

    /// <summary>
    /// Concrete plan the enemy can apply to the live board.
    /// </summary>
    private sealed class Plan {
        public int TargetIndex; // wound index the enemy wants to attack this turn
        public Dictionary<string, int> Stamina = NewStatDictionary(); // stamina spend per stat color
        public Dictionary<Dice, string> YellowAssignments = new(); // target stat row for each enemy yellow die
    }

    /// <summary>
    /// Lexicographic scoring payload for one hard/nightmare candidate plan.
    /// </summary>
    private sealed class AdvancedPlanEvaluation {
        public bool EnemyKills; // candidate kills the player outright
        public bool EnemyDamagesPlayer; // candidate lands a wound even if it does not kill
        public bool EnemyAvoidsKill; // candidate prevents the player from killing the enemy
        public bool EnemyAvoidsDamage; // candidate prevents the player from damaging the enemy
        public bool BreaksPlayerKill; // candidate specifically breaks an existing player kill line
        public bool BreaksPlayerDamage; // candidate specifically breaks an existing player damage line
        public bool BreaksPlayerProtection; // candidate strips dodge/parry/other one-shot protection
        public bool StripsPlayerStamina; // candidate empties the player's temporary stamina adds
        public bool BreaksPlayerSpeed; // candidate removes the player's go-first advantage
        public bool RemovesPlayerRed; // candidate eliminates the player's red dice pressure
        public bool RemovesPlayerWhite; // candidate eliminates the player's white dice defense
        public bool RemovesPlayerBestDie; // candidate removes the player's most valuable attached die state
        public bool BreaksPlayerTarget; // candidate knocks aim below the player's chosen target threshold
        public bool UsesChestOnHighValuePlayerDice; // chest was used to hit high-value player dice state
        public bool UsesChestAsLastDitchGamble; // chest is only being used as a desperate damage-preserving gamble
        public bool UsesAimStaminaForNonFatalTrade; // candidate spends green stamina for a weak non-lethal trade
        public bool EnemyActsFirst; // true if the resulting state gives the enemy turn order priority
        public int SpentStamina; // total stamina committed by the plan
        public int RedOverspend; // excess red spending beyond the needed breakpoint
        public int BlueOverspend; // excess blue spending beyond the needed breakpoint
        public int GreenOverspend; // excess green spending beyond the needed breakpoint
        public int WhiteOverspend; // excess white spending beyond the needed breakpoint
        public int ResourceOverspend; // combined breakpoint waste measured with alternate resource heuristics
        public int TargetIndex; // chosen target wound index for tie-breaking

        /// <summary>
        /// Returns summed overspend across all four stamina colors.
        /// </summary>
        public int TotalOverspend => RedOverspend + BlueOverspend + GreenOverspend + WhiteOverspend;
    }

    /// <summary>
    /// Evaluation payload for choosing one draft die before it is attached.
    /// </summary>
    private sealed class DraftChoiceEvaluation {
        public AdvancedPlanEvaluation BestPlan; // best resulting combat plan if this draft die is taken
        public bool CompletesKillBreakpoint; // die completes an immediate lethal breakpoint
        public bool CompletesHitBreakpoint; // die completes an immediate damage breakpoint
        public bool CompletesOrderBreakpoint; // die secures go-first order
        public bool CompletesArmpitsBreakpoint; // die enables the armpits wound line
        public bool CompletesHeadBreakpoint; // die enables the head wound line
        public bool CompletesDefenseBreakpoint; // die closes a defensive survival breakpoint
        public bool DeniesPlayerKill; // taking this die breaks the player's current kill line
        public bool DeniesPlayerDamage; // taking this die breaks the player's current damage line
        public bool DeniesPlayerDefense; // taking this die prevents a player defensive breakpoint
        public bool ReinforcesPlayerDamage; // leaving/taking this die may strengthen player damage lines
        public bool ReinforcesPlayerDefense; // leaving/taking this die may strengthen player defense lines
        public bool DeniesPlayerGoFirst; // die helps stop the player from moving first
        public bool DeniesPlayerTarget; // die helps stop the player from reaching a preferred target
        public string DieType; // color of the draft die under evaluation
        public bool IsYellow; // whether the draft die is yellow and therefore flexible
        public bool LosesValueToHatchet; // whether hatchet interactions reduce the die's effective value
        public int DieValue; // raw face value of the draft die
        public int EffectiveEnemyValue; // heuristic value to the enemy after synergies/constraints
        public int EffectivePlayerValue; // heuristic value if the player were to claim it instead
        public float FallbackScore; // tie-break score for otherwise equal draft choices
        public float ProgressScore; // score for how much the die advances the enemy plan
        public float PlayerDenialScore; // score for how much the die denies the player's plan
    }

    /// <summary>
    /// Evaluation payload for choosing which attached player die to discard live.
    /// </summary>
    private sealed class LiveDiscardEvaluation {
        public bool BreaksKill; // removing this die stops the player's kill line
        public bool BreaksDamage; // removing this die stops the player's damage line
        public bool BreaksGoFirst; // removing this die flips initiative back to the enemy
        public bool BreaksTarget; // removing this die drops player aim below its target threshold
        public bool RestoresDefense; // removing this die prevents the player from piercing defense
        public bool IsYellow; // yellow dice are flexible and usually more valuable to remove
        public int DieValue; // raw die face used for tie-breaking
    }

    /// <summary>
    /// Scratch context reused while previewing draft decisions against likely player replies.
    /// </summary>
    private sealed class DraftPreviewContext {
        public PlannerSnapshot BaseSnapshot; // base combat snapshot before simulating preview branches
        public List<(Dictionary<string, int> totals, Dictionary<string, int> counts)> PlayerYellowReassignmentOptions = new(); // likely player yellow assignment states
        public Dictionary<YellowAssignmentStateKey, PlannerSnapshot> PlayerSnapshotCache = new(); // cached snapshots keyed by player yellow state
        public Dictionary<Dice, List<(Dictionary<string, int> totals, Dictionary<string, int> counts)>> ReplyStatesByExcludedDie = new(); // reply options after excluding one contested draft die
        public Dictionary<DraftPreviewCacheKey, AdvancedPlanEvaluation> PreviewEvaluationCache = new(); // memoized preview evaluations for repeated branches
    }

    /// <summary>
    /// Lightweight simulated attached die used by planner snapshots.
    /// </summary>
    private sealed class SimAttachedDie {
        public string Stat; // attached stat row in the simulated board state
        public int Value; // current simulated face value
        public bool IsRerolled; // whether the simulated die has already spent its reroll opportunity
        public bool IsYellow; // whether the simulated die is yellow and therefore flexible

        /// <summary>
        /// Returns a shallow clone for snapshot copying.
        /// </summary>
        public SimAttachedDie Clone() {
            return (SimAttachedDie)MemberwiseClone();
        }
    }

    /// <summary>
    /// Immutable-ish board snapshot used as the base for hard/nightmare simulations.
    /// </summary>
    private sealed class PlannerSnapshot {
        public int PlayerAim; // current player aim total before simulated changes
        public int PlayerSpd; // current player speed total before simulated changes
        public int PlayerAtt; // current player attack total before simulated changes
        public int PlayerDef; // current player defense total before simulated changes
        public bool PlayerGuardSelected; // whether the player currently has guard selected as the target
        public int PlayerPendingGuardParryBonus; // pending extra parry from guard-related effects
        public int EnemyBaseAim; // enemy aim before simulated yellow/stamina changes
        public int EnemyBaseSpd; // enemy speed before simulated yellow/stamina changes
        public int EnemyBaseAtt; // enemy attack before simulated yellow/stamina changes
        public int EnemyBaseDef; // enemy defense before simulated yellow/stamina changes
        public int PlayerTargetIndex; // player target wound index at snapshot creation time
        public int PlayerWoundCount; // number of active player wounds
        public int EnemyWoundCount; // number of active enemy wounds
        public int PlayerAddedGreen; // player temporary green stamina additions in play
        public int PlayerAddedBlue; // player temporary blue stamina additions in play
        public int PlayerAddedRed; // player temporary red stamina additions in play
        public int PlayerAddedWhite; // player temporary white stamina additions in play
        public int PlayerGreenDiceCount; // count of player green-attached dice
        public int PlayerBlueDiceCount; // count of player blue-attached dice
        public int PlayerRedDiceCount; // count of player red-attached dice
        public int PlayerWhiteDiceCount; // count of player white-attached dice
        public int EnemyBaseGreenDiceCount; // count of enemy green-attached dice before yellow reassignment
        public int EnemyBaseBlueDiceCount; // count of enemy blue-attached dice before yellow reassignment
        public int EnemyBaseRedDiceCount; // count of enemy red-attached dice before yellow reassignment
        public int EnemyBaseWhiteDiceCount; // count of enemy white-attached dice before yellow reassignment
        public int PlayerGreenDiceSum; // sum of player green-attached die values
        public int PlayerBlueDiceSum; // sum of player blue-attached die values
        public int PlayerRedDiceSum; // sum of player red-attached die values
        public int PlayerWhiteDiceSum; // sum of player white-attached die values
        public int EnemyBaseGreenDiceSum; // sum of enemy green-attached die values before yellow reassignment
        public int EnemyBaseBlueDiceSum; // sum of enemy blue-attached die values before yellow reassignment
        public int EnemyBaseRedDiceSum; // sum of enemy red-attached die values before yellow reassignment
        public int EnemyBaseWhiteDiceSum; // sum of enemy white-attached die values before yellow reassignment
        public int EnemyAttachedDiceCount; // total enemy attached dice count at snapshot time
        public bool PlayerHasArmor; // whether armor is active in the simulated baseline
        public bool PlayerHasDodgy; // whether dodge protection is already active
        public bool PlayerCanBecomeDodgy; // whether the player can still gain dodge from current state
        public bool PlayerHasMaul; // whether maul-specific rules apply to simulated hits
        public int PlayerCrystalShardCopies; // number of crystal shard effects still active
        public int PlayerCrystalShardLossPerShatter; // stat loss per crystal shard shatter
        public int PlayerBulwarkImmediateParryBonus; // immediate parry bonus from bulwark
        public int PlayerInevitableImmediateBonus; // immediate bonus from inevitable-like effects
        public int PlayerRiposteImmediateBonus; // immediate bonus from riposte-like effects
        public int PlayervindictiveImmediateBonus; // immediate bonus from vindictive-like effects
        public int PlayerTridentImmediateBonus; // immediate bonus from trident-like effects
        public int PlayerScimitarDiscardCount; // remaining scimitar discard responses
        public bool PlayerHasGlassSword; // whether glass sword rules apply
        public bool PlayerGlassSwordShattered; // whether glass sword already shattered
        public int PlayerGlassSwordAimDeltaOnShatter; // aim loss applied when glass sword shatters
        public int PlayerGlassSwordSpdDeltaOnShatter; // speed loss applied when glass sword shatters
        public int PlayerGlassSwordAttDeltaOnShatter; // attack loss applied when glass sword shatters
        public int PlayerGlassSwordDefDeltaOnShatter; // defense loss applied when glass sword shatters
        public bool EnemyIsLich; // whether the current enemy follows lich-specific rules
        public bool PlayerSpeedLockedHigh; // whether player speed is forced to win ties
        public bool EnemySpeedLockedHigh; // whether enemy speed is forced to win ties
        public List<SimAttachedDie> PlayerAttachedDice = new(); // simulated player attached dice list
        public List<SimAttachedDie> EnemyAttachedDice = new(); // simulated enemy attached dice list

        /// <summary>
        /// Deep-copies the attached-die lists while cloning scalar snapshot values.
        /// </summary>
        public PlannerSnapshot Clone() {
            PlannerSnapshot clone = (PlannerSnapshot)MemberwiseClone();
            clone.PlayerAttachedDice = PlayerAttachedDice.Select(die => die.Clone()).ToList();
            clone.EnemyAttachedDice = EnemyAttachedDice.Select(die => die.Clone()).ToList();
            return clone;
        }
    }

    /// <summary>
    /// Hashable representation of one yellow-die assignment state.
    /// Stores both totals and die counts because a sum of 6 from one die behaves differently
    /// than a sum of 6 split across several dice during draft preview simulation.
    /// </summary>
    private readonly struct YellowAssignmentStateKey : IEquatable<YellowAssignmentStateKey> {
        private readonly int greenTotal; // sum of yellow value routed to green
        private readonly int blueTotal; // sum of yellow value routed to blue
        private readonly int redTotal; // sum of yellow value routed to red
        private readonly int whiteTotal; // sum of yellow value routed to white
        private readonly int greenCount; // number of yellow dice routed to green
        private readonly int blueCount; // number of yellow dice routed to blue
        private readonly int redCount; // number of yellow dice routed to red
        private readonly int whiteCount; // number of yellow dice routed to white

        /// <summary>
        /// Captures one yellow assignment state from totals/count dictionaries.
        /// </summary>
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

    /// <summary>
    /// Cache key for draft preview evaluation.
    /// Combines the likely player reply state with the enemy preview state because either side's
    /// yellow routing can change the resulting advanced-plan evaluation.
    /// </summary>
    private readonly struct DraftPreviewCacheKey : IEquatable<DraftPreviewCacheKey> {
        private readonly YellowAssignmentStateKey playerState; // simulated player reply routing state
        private readonly YellowAssignmentStateKey previewState; // simulated enemy post-pick routing state

        /// <summary>
        /// Captures both sides' yellow-assignment states for preview memoization.
        /// </summary>
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
    public static string LastAdvancedPlanProfileSummary { get; private set; } = "advanced plan profiler idle"; // latest dev-build summary text for debugging planner cost
    private static int advancedPlanProfileRuns; // number of profiled advanced-plan builds
    private static double advancedPlanProfileTotalMs; // accumulated build time for averaging
    private static double advancedPlanProfileMaxMs; // slowest recorded advanced-plan build
    private static int advancedPlanCacheHits; // advanced-plan cache hits in dev builds
    private static int advancedPlanCacheMisses; // advanced-plan cache misses in dev builds
#endif

    private static int suppressTargetEvaluationDepth; // guards against recursive retargeting while applying a plan
    private static int cachedAdvancedPlanKey; // signature for the single-entry advanced-plan cache
    private static Plan cachedAdvancedPlan; // last built advanced plan for the current board signature
    private static bool hasCachedAdvancedPlan; // whether the single-entry advanced-plan cache is populated

    /// <summary>
    /// Clears the single-entry advanced-plan cache.
    /// </summary>
    public static void InvalidateCachedPlan() {
        hasCachedAdvancedPlan = false;
        cachedAdvancedPlan = null;
        cachedAdvancedPlanKey = 0;
    }

    /// <summary>
    /// Fully mutable simulated combat state used while evaluating candidate plans.
    /// </summary>
    private sealed class SimState {
        public int PlayerAim; // mutable player aim after simulated wounds, discards, and triggers
        public int PlayerSpd; // mutable player speed after simulated wounds, discards, and triggers
        public int PlayerAtt; // mutable player attack after simulated wounds, discards, and triggers
        public int PlayerDef; // mutable player defense after simulated wounds, discards, and triggers
        public bool PlayerGuardSelected; // whether the player is guarding instead of attacking
        public int PlayerPendingGuardParryBonus; // guard/buckler parry bonus already committed for this resolution
        public int EnemyAim; // mutable enemy aim after yellow routing and stamina spend
        public int EnemySpd; // mutable enemy speed after yellow routing and stamina spend
        public int EnemyAtt; // mutable enemy attack after yellow routing and stamina spend
        public int EnemyDef; // mutable enemy defense after yellow routing and stamina spend
        public int EnemyTargetIndex; // simulated wound target index the enemy is attacking
        public int PlayerWoundCount; // active player wounds in this simulated branch
        public int EnemyWoundCount; // active enemy wounds in this simulated branch
        public int PlayerAddedGreen; // player temporary green stamina add still present in this branch
        public int PlayerAddedBlue; // player temporary blue stamina add still present in this branch
        public int PlayerAddedRed; // player temporary red stamina add still present in this branch
        public int PlayerAddedWhite; // player temporary white stamina add still present in this branch
        public int EnemyAddedGreen; // enemy temporary green stamina add committed by this plan
        public int EnemyAddedBlue; // enemy temporary blue stamina add committed by this plan
        public int EnemyAddedRed; // enemy temporary red stamina add committed by this plan
        public int EnemyAddedWhite; // enemy temporary white stamina add committed by this plan
        public int PlayerGreenDiceCount; // count of player green dice surviving in this branch
        public int PlayerBlueDiceCount; // count of player blue dice surviving in this branch
        public int PlayerRedDiceCount; // count of player red dice surviving in this branch
        public int PlayerWhiteDiceCount; // count of player white dice surviving in this branch
        public int EnemyGreenDiceCount; // count of enemy green dice surviving in this branch
        public int EnemyBlueDiceCount; // count of enemy blue dice surviving in this branch
        public int EnemyRedDiceCount; // count of enemy red dice surviving in this branch
        public int EnemyWhiteDiceCount; // count of enemy white dice surviving in this branch
        public int PlayerGreenDiceSum; // summed player green die value in this branch
        public int PlayerBlueDiceSum; // summed player blue die value in this branch
        public int PlayerRedDiceSum; // summed player red die value in this branch
        public int PlayerWhiteDiceSum; // summed player white die value in this branch
        public int EnemyGreenDiceSum; // summed enemy green die value in this branch
        public int EnemyBlueDiceSum; // summed enemy blue die value in this branch
        public int EnemyRedDiceSum; // summed enemy red die value in this branch
        public int EnemyWhiteDiceSum; // summed enemy white die value in this branch
        public bool PlayerHasArmor; // whether one-hit armor protection is still intact
        public bool PlayerHasDodgy; // whether dodgy is already active
        public bool PlayerCanBecomeDodgy; // whether boots can still convert into dodgy before an enemy-second hit
        public bool PlayerHasMaul; // whether any successful player hit should be treated as lethal
        public int PlayerCrystalShardCopies; // crystal shards still available to shatter on incoming wounds
        public int PlayerCrystalShardLossPerShatter; // stat loss each shattered crystal shard inflicts
        public int PlayerBulwarkImmediateParryBonus; // extra parry granted immediately if enemy attacks first
        public int PlayerInevitableImmediateBonus; // immediate player attack gained after enemy-first resolution
        public int PlayerRiposteImmediateBonus; // immediate player attack gained after a successful parry
        public int PlayervindictiveImmediateBonus; // immediate player attack gained after taking a wound
        public int PlayerTridentImmediateBonus; // immediate player attack granted if player acts first with trident
        public int PlayerScimitarDiscardCount; // number of enemy dice the player may discard after a parry
        public bool PlayerHasGlassSword; // whether the glass sword shatter transformation can still apply
        public bool PlayerGlassSwordShattered; // whether glass sword has already shattered in this branch
        public int PlayerGlassSwordAimDeltaOnShatter; // aim delta applied when glass sword shatters
        public int PlayerGlassSwordSpdDeltaOnShatter; // speed delta applied when glass sword shatters
        public int PlayerGlassSwordAttDeltaOnShatter; // attack delta applied when glass sword shatters
        public int PlayerGlassSwordDefDeltaOnShatter; // defense delta applied when glass sword shatters
        public bool EnemyIsLich; // whether enemy uses lich exceptions like no hip stamina lock
        public bool PlayerSpeedLockedHigh; // player always wins initiative in this branch
        public bool EnemySpeedLockedHigh; // enemy always wins initiative in this branch
        public bool PlayerImmuneToWounds; // reserved scratch flag for wound immunity branches
        public float Bonus; // soft heuristic bonus used only by non-lexicographic fallback scoring helpers
        public List<SimAttachedDie> PlayerAttachedDice = new(); // mutable player dice list for discard/wound simulation
        public List<SimAttachedDie> EnemyAttachedDice = new(); // mutable enemy dice list for discard/wound simulation

        /// <summary>
        /// Deep-copies the mutable simulated board state for branch evaluation.
        /// </summary>
        public SimState Clone() {
            SimState clone = (SimState)MemberwiseClone();
            clone.PlayerAttachedDice = PlayerAttachedDice.Select(die => die.Clone()).ToList();
            clone.EnemyAttachedDice = EnemyAttachedDice.Select(die => die.Clone()).ToList();
            return clone;
        }
    }

    /// <summary>
    /// Chooses the best currently unattached draft die for the enemy and attaches it.
    /// </summary>
    /// <param name="s">shared scene references and combat systems</param>
    public static void ChooseBestDie(Scripts s) {
        List<Dice> availableDice = s.diceSummoner.existingDice
            .Select(diceObject => diceObject.GetComponent<Dice>())
            .Where(dice => !dice.isAttached)
            .ToList();

        if (availableDice.Count == 0) { return; }

        // hard/nightmare use full preview simulation; easy/normal keep the legacy rank list
        Dice chosenDie = DifficultyHelper.UsesAdvancedEnemyAI(Save.persistent.gameDifficulty)
            ? ChooseAdvancedDraftDie(s, availableDice)
            : ChooseDefaultDraftDie(availableDice);

        if (chosenDie == null) { return; }

        AttachChosenDie(s, chosenDie);
    }

    /// <summary>
    /// Builds the enemy's current live plan and applies it directly to the board.
    /// </summary>
    /// <param name="s">shared scene references and combat systems</param>
    public static void ApplyLivePlan(Scripts s) {
        if (!CanPlan(s)) { return; }

        Plan plan = BuildPlan(s);
        // applying a plan mutates target/stamina/dice state; suppress recursive target evaluation during that window
        suppressTargetEvaluationDepth++;
        try {
            ApplyPlan(s, plan);
        }
        finally {
            suppressTargetEvaluationDepth = Mathf.Max(0, suppressTargetEvaluationDepth - 1);
        }
    }

    /// <summary>
    /// Builds the nightmare plan at attack time and animates each change before applying it.
    /// </summary>
    /// <param name="s">shared scene references and combat systems</param>
    public static IEnumerator AnimateAndApplyNightmarePlan(Scripts s) {
        if (!CanPlan(s)) { yield break; }

        // nightmare hides intent during draft, so capture both the visible current state and the hidden planned state
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

        // only play reveal clicks when the animation actually communicates a meaningful combat change
        bool playSoundForSteps = preventedPlayerHit || createdEnemyHit || changedTargetWhileStillHitting;
        bool playSoundForTargetSteps = createdEnemyHit || changedTargetWhileStillHitting;

        int totalSteps = staminaSteps + movedYellowDice.Count + targetSteps;
        int stepsCompleted = 0;

        bool ShouldPauseAfterStep() {
            stepsCompleted++;
            return stepsCompleted < totalSteps;
        }

        // reveal order is fixed by spec: stamina first, then yellow die moves, then target shifts
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
    /// Returns the best current target index for the enemy.
    /// </summary>
    /// <param name="s">shared scene references and combat systems</param>
    /// <returns>target index in `Targets`</returns>
    public static int GetBestTargetIndex(Scripts s) {
        if (!CanTarget(s)) { return 0; }
        if (suppressTargetEvaluationDepth > 0) { return Mathf.Clamp(s.enemy.targetIndex, 0, Targets.Length - 1); }

        // hard/nightmare only use the advanced planner once the draft is fully resolved
        if (DifficultyHelper.UsesAdvancedEnemyAI(Save.persistent.gameDifficulty) && HasAnyDiceInPlay(s) && s.diceSummoner.CountUnattachedDice() == 0) {
            return BuildPlan(s).TargetIndex;
        }

        return GetDefaultTargetIndex(s, s.statSummoner.SumOfStat("green", "enemy"));
    }

    /// <summary>
    /// Chooses which attached player die should be discarded by a head-wound style effect.
    /// </summary>
    /// <param name="s">shared scene references and combat systems</param>
    /// <param name="playerDice">candidate player-attached dice</param>
    /// <returns>best die to discard, or null if none exist</returns>
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
    /// Returns whether the enemy can legally build a combat plan right now.
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
    /// Returns whether the enemy is allowed to retarget right now.
    /// </summary>
    private static bool CanTarget(Scripts s) {
        return s != null
            && s.enemy != null
            && s.player != null
            && s.turnManager != null
            && s.enemy.enemyName.text is not "Merchant" and not "Blacksmith" and not "Tombstone";
    }

    /// <summary>
    /// Returns whether the current round has any live dice in play yet.
    /// </summary>
    private static bool HasAnyDiceInPlay(Scripts s) {
        return s != null && s.diceSummoner != null && s.diceSummoner.existingDice.Count > 0;
    }

    /// <summary>
    /// Dispatches to the correct planner for the current difficulty and cache state.
    /// </summary>
    private static Plan BuildPlan(Scripts s) {
        string difficulty = DifficultyHelper.Normalize(Save.persistent.gameDifficulty);
        if (DifficultyHelper.IsEasy(difficulty)) { return BuildEasyPlan(s); }
        if (DifficultyHelper.IsNormal(difficulty)) { return BuildNormalPlan(s); }

        // hard and nightmare share the same advanced brain; only visibility/reveal differs elsewhere
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
    /// Builds the easy-mode threshold plan.
    /// </summary>
    private static Plan BuildEasyPlan(Scripts s) {
        // easy is intentionally just normal planning with visible intent, so reuse the exact same thresholds
        return BuildNormalPlan(s);
    }

    /// <summary>
    /// Builds the normal-mode threshold plan using direct breakpoint checks.
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

        // normal mode never funds neck with stamina, so pre-neck targeting is clamped to 0..6 first
        plan.TargetIndex = GetDefaultTargetIndex(s, Mathf.Min(naturalAim, 6));

        int playerDefAgainstCurrentOrder = playerDef + (enemySpd > playerSpd ? bulwarkBonus : 0);
        if (enemyAtt <= playerDefAgainstCurrentOrder && enemyAtt + remainingStamina > playerDefAgainstCurrentOrder) {
            // threshold rule 1: spend only the exact red amount needed to start dealing damage
            int spend = playerDefAgainstCurrentOrder - enemyAtt + 1;
            plan.Stamina["red"] += spend;
            remainingStamina -= spend;
            enemyAtt += spend;
        }

        int playerDefIfEnemyGoesFirst = playerDef + bulwarkBonus;
        if (enemyAtt > playerDefIfEnemyGoesFirst && playerSpd >= enemySpd && playerAtt > enemyDef && enemySpd + remainingStamina > playerSpd) {
            // threshold rule 2: if going first protects the hit line, spend the exact blue amount to flip order
            int spend = playerSpd - enemySpd + 1;
            plan.Stamina["blue"] += spend;
            remainingStamina -= spend;
        }

        enemyDef = GetEnemyStatWithPlan(s, plan, "white");
        if (playerAtt > enemyDef && s.statSummoner.SumOfStat("green", "player") >= 0 && enemyDef + remainingStamina >= playerAtt) {
            // threshold rule 3: if survival is reachable, spend the exact white amount to survive the reply
            int spend = playerAtt - enemyDef;
            plan.Stamina["white"] += spend;
        }

        if (naturalAim >= 7) {
            // only natural aim unlocks neck in easy/normal
            plan.TargetIndex = GetDefaultTargetIndex(s, naturalAim);
        }

        return plan;
    }

    /// <summary>
    /// Builds the hard/nightmare search plan by simulating yellow routing and stamina spending.
    /// </summary>
    private static Plan BuildAdvancedPlan(Scripts s) {
        using (BuildAdvancedPlanProfiler.Auto()) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif
            // snapshot once, then search over yellow routing + compressed stamina breakpoints + legal targets
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

            // fast path: if there are no yellow branches and no spendable stamina, only target selection can vary
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
                    // permanent stamina is too valuable to waste on plans that change no meaningful gate
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

                // once yellow routing is fixed, only breakpoint-relevant stamina spends are searched
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
                                // first lexicographically perfect kill found here can terminate the whole search
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
                // yellow search order is biased toward the die's current row so harmless rearrangements are tried first
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
    /// Evaluates one advanced-plan candidate by building a fresh snapshot on demand.
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
    /// Evaluates one hard-mode candidate against an existing board snapshot.
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
        bool enemyDamagesBefore = EnemyHitDamagesPlayer(state, enemyCanHitBefore, enemyActsFirst);
        bool enemyAppliesNewWoundBefore = EnemyHitAppliesWound(s, state, enemyTarget, enemyCanHitBefore, enemyActsFirst);
        bool enemyKillsBefore = EnemyWouldKillPlayer(s, state, enemyTarget, enemyCanHitBefore) && enemyAppliesNewWoundBefore;
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
            // branch A from ENEMY_AI.md: enemy hits before the player can apply their wound/guard response
            SimState afterEnemyHit = state.Clone();
            if (enemyBreaksProtectionBefore) {
                ConsumePlayerProtection(afterEnemyHit);
            }
            if (enemyAppliesNewWoundBefore) {
                ApplyWoundToPlayer(afterEnemyHit, enemyTarget, s);
            }
            // immediate player counters like inevitable, vindictive, riposte, scimitar all live here
            ApplyImmediatePlayerResponseAfterEnemyActsFirst(afterEnemyHit, s, enemyWasParriedBefore, enemyDamagesBefore);

            bool playerCanHitAfter = !afterEnemyHit.PlayerGuardSelected && afterEnemyHit.PlayerAim >= 0 && afterEnemyHit.PlayerAtt > afterEnemyHit.EnemyDef;
            bool playerDamagesAfter = PlayerHitDamagesEnemy(s, afterEnemyHit, playerTarget, playerCanHitAfter);
            bool playerKillsAfter = PlayerWouldKillEnemy(s, afterEnemyHit, playerTarget, playerCanHitAfter);
            bool chestRescueCanBreakDamage = enemyTarget == "chest"
                && enemyAppliesNewWoundBefore
                && playerDamagesAfter
                && EnemyChestRescueCanBreakPlayerDamage(s, afterEnemyHit, playerTarget);

            evaluation.EnemyDamagesPlayer = enemyDamagesBefore;
            evaluation.EnemyKills = enemyKillsBefore;
            evaluation.EnemyAvoidsDamage = !playerDamagesAfter;
            evaluation.EnemyAvoidsKill = !playerKillsAfter;
            evaluation.BreaksPlayerKill = playerKillsBefore && !playerKillsAfter;
            evaluation.BreaksPlayerDamage = playerDamagesBefore && !playerDamagesAfter;
            evaluation.BreaksPlayerProtection = enemyBreaksProtectionBefore;
            // enemy acts first means the player already lost this round's order check,
            // so knee matters only if some future-round behavior later reads the locked state
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

        // branch B from ENEMY_AI.md: player attacks first, so enemy evaluation must survive the incoming wound before swinging
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
    /// Compares two advanced evaluations using the lexicographic gate ordering.
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
            bool guaranteedHitTradeoff = candidate.EnemyDamagesPlayer && current.EnemyDamagesPlayer;
            if (guaranteedHitTradeoff) {
                // when both tied plans still guarantee enemy damage and no higher gates differ,
                // prefer the highest legal wound target (neck > armpits > hand ... > chest)
                return candidate.TargetIndex > current.TargetIndex;
            }

            // otherwise keep the legacy cleanup ordering:
            // chest is worst, then lower non-chest target index wins
            bool candidateIsChest = candidate.TargetIndex == 0;
            bool currentIsChest = current.TargetIndex == 0;
            if (candidateIsChest != currentIsChest) { return currentIsChest; }
            return candidate.TargetIndex < current.TargetIndex;
        }
        return false;
    }

    /// <summary>
    /// Returns whether a candidate is a zero-cost perfect kill that can short-circuit the search.
    /// </summary>
    private static bool IsPerfectAdvancedEvaluation(AdvancedPlanEvaluation evaluation) {
        return evaluation != null
            && evaluation.EnemyKills
            && evaluation.SpentStamina == 0
            && evaluation.TotalOverspend == 0;
    }

    /// <summary>
    /// Rejects stamina plans that spend resources without changing any relevant evaluation gate.
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

    /// <summary>
    /// Legacy float-scored evaluator retained for draft cleanup heuristics and debugging.
    /// The real hard/nightmare planner prefers the lexicographic gates above this method.
    /// </summary>
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
        bool enemyDamagesPlayer = EnemyHitDamagesPlayer(state, enemyCanHit, enemyActsFirst);
        bool enemyHitApplies = EnemyHitAppliesWound(s, state, enemyTarget, enemyCanHit, enemyActsFirst);
        bool enemyWasParried = EnemyAttackTriggersParryResponses(state.EnemyAtt, playerDefenseAgainstEnemy);
        bool playerHitApplies = PlayerHitAppliesWound(s, playerTarget, playerCanHit);
        float score = state.Bonus;

        if (enemyActsFirst) {
            if (enemyCanHit) {
                // reward live hits heavily, then evaluate how much the post-hit reply is blunted
                score += 1200f;
                if (enemyKills && enemyHitApplies) { score += 100000f; }

                SimState afterEnemyHit = state.Clone();
                if (enemyBreaksProtection) {
                    ConsumePlayerProtection(afterEnemyHit);
                }
                if (enemyHitApplies) {
                    ApplyWoundToPlayer(afterEnemyHit, enemyTarget, s);
                    ApplyImmediatePlayerResponseAfterEnemyActsFirst(afterEnemyHit, s, enemyWasParried, enemyDamagesPlayer);
                    score += GetTargetUtility(s, enemyTarget, afterEnemyHit, onPlayer:true);
                }
                else if (enemyBreaksProtection) {
                    score += 220f;
                }
                else if (PlayerHasOneShotProtection(state)) {
                    score += 120f;
                }

                if (!enemyHitApplies) {
                    ApplyImmediatePlayerResponseAfterEnemyActsFirst(afterEnemyHit, s, enemyWasParried, enemyDamagesPlayer);
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

        // the farther a spend drifts away from a real breakpoint, the more this heuristic should distrust it
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

        // baseline yellow routing mirrors the live board before any optimization moves them elsewhere
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
            // snapshot the stat delta now so later wound simulation can apply the shatter without rereading live equipment
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
            // destructive/empowered/fortified rewrite one player row to mirror another before planning starts
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
            PlayerTridentImmediateBonus = s.itemManager.PlayerHasWeapon("trident") ? (s.itemManager.PlayerHasLegendary() ? 2 : 1) : 0,
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

        // copy only the information the planner actually reasons about, not whole live Dice components
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

                // enemy yellow dice are tracked separately through totals/counts because they can be rerouted during search
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

        // yellow dice are converted into estimated attached dice so discard/head logic can treat them like real dice later
        AppendEstimatedEnemyYellowDice(state.EnemyAttachedDice, yellowTotals, yellowCounts);

        bool playerActsFirst = state.PlayerSpeedLockedHigh || (!state.EnemySpeedLockedHigh && state.PlayerSpd >= state.EnemySpd);
        if (!state.PlayerGuardSelected && playerActsFirst && state.PlayerTridentImmediateBonus > 0) {
            state.PlayerAtt += state.PlayerTridentImmediateBonus;
        }

        // soft bonuses/penalties here only affect heuristic helpers, never the main lexicographic planner
        if (PlayerHasOneShotProtection(state)) { state.Bonus -= 150f; }
        if (!snapshot.PlayerGuardSelected && snapshot.PlayerTargetIndex == 6 && state.PlayerRedDiceSum > 0) { state.Bonus -= state.PlayerRedDiceSum * 12f; }
        if (!snapshot.PlayerGuardSelected && snapshot.PlayerTargetIndex == 4 && snapshot.EnemyAttachedDiceCount > 0) { state.Bonus -= 180f; }
        return state;
    }

    /// <summary>
    /// Adds synthetic yellow dice into the simulated enemy attached-die list.
    /// This lets discard/head/chest helpers reason about yellow dice without special-case logic later.
    /// </summary>
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

    /// <summary>
    /// Splits a known yellow total into a plausible multiset of die values.
    /// Exact faces are unknown in snapshot form, so this produces a bounded approximation.
    /// </summary>
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
        // planner fatal helpers intentionally treat "third unique wound" as the fatal bucket;
        // neck bleed-out is modeled elsewhere and does not count here as same-round lethal
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

    /// <summary>
    /// Applies one player wound to a simulated state using the same immediate consequences as combat.
    /// </summary>
    private static void ApplyWoundToPlayer(SimState state, string target, Scripts s) {
        switch (target) {
            case "guts":
                // guts reduces every attached die by one pip, which is equivalent here to subtracting die counts per stat
                state.PlayerAim -= state.PlayerGreenDiceCount;
                state.PlayerSpd -= state.PlayerBlueDiceCount;
                state.PlayerAtt -= state.PlayerRedDiceCount;
                state.PlayerDef -= state.PlayerWhiteDiceCount;
                break;
            case "knee":
                // a player knee wound means the enemy now wins order checks unless an override already says otherwise
                state.EnemySpeedLockedHigh = true;
                break;
            case "hip":
                // hip refunds and clears all added stamina on the wounded side immediately
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
                // head discards the player's best attached die according to the live discard comparator
                ApplyBestPlayerDiscard(state, s);
                break;
            case "hand":
                // hand removes all white dice from the wounded side
                state.PlayerDef -= state.PlayerWhiteDiceSum;
                state.PlayerWhiteDiceCount = 0;
                state.PlayerWhiteDiceSum = 0;
                state.PlayerAttachedDice.RemoveAll(die => die.Stat == "white");
                break;
            case "armpits":
                // armpits removes all red dice from the wounded side
                state.PlayerAtt -= state.PlayerRedDiceSum;
                state.PlayerRedDiceCount = 0;
                state.PlayerRedDiceSum = 0;
                state.PlayerAttachedDice.RemoveAll(die => die.Stat == "red");
                break;
            case "chest":
                // chest itself does not directly change stats here; it unlocks rescue rerolls, represented as a heuristic nudge
                state.Bonus -= 650f;
                break;
            case "neck":
                // neck bleed-out is tracked outside the immediate stat model
                break;
        }
    }

    /// <summary>
    /// Applies one enemy wound to a simulated state using the same immediate consequences as combat.
    /// </summary>
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
                // enemy chest lowers confidence in the branch because player reroll pressure gets unlocked
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
        // if the enemy attacks second and boots/dodgy can still activate, treat the hit as dodged entirely
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

    /// <summary>
    /// Applies player immediate-response effects after an enemy-first swing resolves.
    /// </summary>
    private static void ApplyImmediatePlayerResponseAfterEnemyActsFirst(SimState state, Scripts s, bool enemyWasParried, bool enemyDamagedPlayer) {
        ApplyImmediatePlayerEnemyFirstAlwaysOnEffects(state);

        if (enemyDamagedPlayer) {
            // wound-triggered responses like vindictive/crystal shard/glass sword happen before any counterattack check
            ApplyImmediatePlayerWoundResponseEffects(state);
            return;
        }

        if (enemyWasParried) {
            // parry-triggered responses like riposte/scimitar only happen when the attack failed on defense
            ApplyImmediatePlayerParryResponseEffects(state, s);
        }
    }

    private static void ApplyImmediatePlayerEnemyFirstAlwaysOnEffects(SimState state) {
        if (state.PlayerInevitableImmediateBonus > 0) {
            state.PlayerAtt += state.PlayerInevitableImmediateBonus;
        }
    }

    /// <summary>
    /// Applies wound-triggered player effects such as vindictive, crystal shard, and glass sword shatter.
    /// </summary>
    private static void ApplyImmediatePlayerWoundResponseEffects(SimState state) {
        if (state.PlayervindictiveImmediateBonus > 0) {
            state.PlayerAtt += state.PlayervindictiveImmediateBonus;
        }

        // crystal shards resolve before glass sword; if a shard absorbs the wound, glass sword does not also shatter
        bool crystalShardShatters = state.PlayerCrystalShardCopies > 0;
        if (crystalShardShatters) {
            int shatteredCopies = state.PlayerCrystalShardCopies;
            state.PlayerCrystalShardCopies = 0;
            state.PlayerAtt -= state.PlayerCrystalShardLossPerShatter * shatteredCopies;
        }

        if (state.PlayerHasGlassSword && !state.PlayerGlassSwordShattered && !crystalShardShatters) {
            state.PlayerGlassSwordShattered = true;
            state.PlayerAim += state.PlayerGlassSwordAimDeltaOnShatter;
            state.PlayerSpd += state.PlayerGlassSwordSpdDeltaOnShatter;
            state.PlayerAtt += state.PlayerGlassSwordAttDeltaOnShatter;
            state.PlayerDef += state.PlayerGlassSwordDefDeltaOnShatter;
        }
    }

    /// <summary>
    /// Applies parry-triggered player effects such as riposte and scimitar discards.
    /// </summary>
    private static void ApplyImmediatePlayerParryResponseEffects(SimState state, Scripts s) {
        if (state.PlayerRiposteImmediateBonus > 0) {
            state.PlayerAtt += state.PlayerRiposteImmediateBonus;
        }

        // legendary scimitar can discard twice; each discard re-runs the same best-die logic on the updated state
        for (int i = 0; i < state.PlayerScimitarDiscardCount; i++) {
            ApplyBestEnemyDiscard(state, s);
        }
    }

    /// <summary>
    /// Removes the highest-impact player die from a simulated state.
    /// </summary>
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

        // chest rescue rerolls only currently-unrerolled player green/red dice with value >= 3,
        // because those are the dice most capable of turning off the player's pending hit line
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
        // heuristic utilities only help legacy float scoring and draft cleanup; the real planner uses gate ordering instead
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

        // if the hidden plan still does not produce a hit, nightmare should not waste time revealing empty retarget shuffles
        if (plannedEvaluation != null && !plannedEvaluation.EnemyDamagesPlayer && plan.TargetIndex != startingTargetIndex) {
            plan.TargetIndex = startingTargetIndex;
            plannedEvaluation = EvaluatePlanOutcome(s, snapshot, plan);
        }

        bool changedStamina = Stats.Any(stat => plan.Stamina[stat] != startingStamina[stat]);
        bool changedTarget = plan.TargetIndex != startingTargetIndex;
        bool changedYellowAssignments = HasNightmareYellowRearrangement(currentPlan, plan);
        if (!changedYellowAssignments || changedStamina || changedTarget) { return; }
        if (!HasMatchingNightmareOutcome(currentEvaluation, plannedEvaluation)) { return; }

        // pure cosmetic yellow shuffles are suppressed when they do not change the coarse combat result
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

        // commit sequence mirrors ENEMY_AI.md exactly:
        // 1) refund existing stamina adds
        // 2) clear current yellow attachments
        // 3) attach yellows per plan
        // 4) apply new stamina adds
        // 5) spend base stamina
        // 6) update target and recompute stats/positions
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

        // nightmare reveal adds stamina one pip at a time so the player can watch the hidden commitment appear
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

        // nightmare reveal walks the target one wound at a time instead of teleporting to the final target
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

        // compare every candidate by its worst likely post-pick outcome, not just its immediate raw value
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

        // once a winning color/strategy is chosen, take the highest face of that color still on the board
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
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        DraftChoiceEvaluation evaluation = new() {
            DieType = dice.diceType,
            IsYellow = dice.diceType == "yellow",
            LosesValueToHatchet = dice.diceType == "yellow" && s.itemManager.PlayerHasWeapon("hatchet"),
            DieValue = dice.diceNum,
            EffectiveEnemyValue = effectiveEnemyValue,
            EffectivePlayerValue = effectivePlayerValue,
            FallbackScore = EvaluateDraftChoice(s, dice),
            PlayerDenialScore = GetPlayerDieDesireScore(s, dice),
        };

        foreach (string stat in GetDraftAssignmentOptions(s, dice)) {
            // each legal enemy attachment row gets its own full preview plan evaluation
            AdvancedPlanEvaluation preview = GetDraftPreviewEvaluation(s, snapshot, dice, stat, availableDice, previewContext);
            if (IsBetterAdvancedEvaluation(preview, evaluation.BestPlan)) {
                evaluation.BestPlan = preview;
            }

            UpdateDraftBreakpointFlags(s, evaluation, stat, effectiveEnemyValue);
            evaluation.ProgressScore = Mathf.Max(evaluation.ProgressScore, GetDraftProgressBonus(s, stat, effectiveEnemyValue));
        }

        evaluation.DeniesPlayerKill = DraftDieDeniesPlayerKill(s, dice);
        evaluation.DeniesPlayerDamage = DraftDieDeniesPlayerDamage(s, dice);
        evaluation.DeniesPlayerDefense = DraftDieDeniesPlayerDefense(s, dice);
        evaluation.ReinforcesPlayerDamage = DraftDieReinforcesPlayerDamage(s, dice);
        evaluation.ReinforcesPlayerDefense = DraftDieReinforcesPlayerDefense(s, dice);
        evaluation.DeniesPlayerGoFirst = DraftDieDeniesPlayerGoFirst(s, dice);
        evaluation.DeniesPlayerTarget = DraftDieDeniesPlayerTarget(s, dice);
        if (effectiveEnemyValue <= 0
            && !evaluation.DeniesPlayerKill
            && !evaluation.DeniesPlayerDamage
            && !evaluation.DeniesPlayerDefense
            && !evaluation.ReinforcesPlayerDamage
            && !evaluation.ReinforcesPlayerDefense
            && !evaluation.DeniesPlayerGoFirst
            && !evaluation.DeniesPlayerTarget) {
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
            // some picks only matter as denial to the player, so evaluate the preview with zero self-gain
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
        // baseline = enemy takes the die and the player gets no extra reply die at all;
        // later reply states then try to worsen that outcome from the enemy's perspective
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
            // enemy wounds apply immediately to newly drafted enemy dice too, except for lich exceptions
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
        // mirror the player's own immediate on-attach penalties so denial logic previews the real value they would get
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

        bool playerSpeedLockedHigh = IsPlayerDraftSpeedLockedHigh(s);
        bool enemySpeedLockedHigh = IsEnemyDraftSpeedLockedHigh(s);
        bool enemyActsFirstNow = enemySpeedLockedHigh || (!playerSpeedLockedHigh && enemySpd > playerSpd);
        bool enemyActsFirstAfterPick = enemySpeedLockedHigh || (!playerSpeedLockedHigh && nextEnemySpd > playerSpd);
        bool killEnabledNow = enemyAim >= 7 && enemyAtt > playerDef;
        bool killEnabledAfterPick = nextEnemyAim >= 7 && nextEnemyAtt > playerDef;

        // these flags capture the "self-benefit first" breakpoint completions described in ENEMY_AI.md
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
        // draft previews model the player as if they immediately attached the likely reply die(s)
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
            // if the player is guarding, white-focused yellow routing is the only reply state that really matters
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
        // build a few likely reply patterns rather than the full combinatorial yellow search,
        // keeping previews cheap while still covering the strongest player reply shapes
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

        // previews only consider a small likely subset of player reply dice to keep hard/nightmare draft evaluation bounded
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

        // start with the single die the player wants most overall, then add one top die per color bucket
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

        // this is basically a miniature advanced-plan search run on the hypothetical post-pick board
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

        // if no candidate improved anything, still evaluate the zero-spend current target so callers get a real preview object
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
        // draft ordering follows ENEMY_AI.md:
        // 1) compare real preview plans
        // 2) compare coarse preview outcomes
        // 3) same-color higher face protection
        // 4) hard denial flags
        // 5) preview stamina cleanliness
        // 6) self breakpoint completions
        // 7) softer denial/progress cleanup
        if (IsBetterDraftPlanPreview(candidate.BestPlan, current.BestPlan)) { return true; }
        if (IsBetterDraftPlanPreview(current.BestPlan, candidate.BestPlan)) { return false; }
        if (IsBetterDraftOutcomePreview(candidate.BestPlan, current.BestPlan)) { return true; }
        if (IsBetterDraftOutcomePreview(current.BestPlan, candidate.BestPlan)) { return false; }
        if (candidate.DieType == current.DieType && candidate.DieValue != current.DieValue) {
            return candidate.DieValue > current.DieValue;
        }
        if (candidate.DeniesPlayerKill != current.DeniesPlayerKill) { return candidate.DeniesPlayerKill; }
        if (candidate.DeniesPlayerDamage != current.DeniesPlayerDamage) { return candidate.DeniesPlayerDamage; }
        if (candidate.DeniesPlayerDefense != current.DeniesPlayerDefense) { return candidate.DeniesPlayerDefense; }
        if (ShouldPreferNearFutilePowerDie(candidate, current)) {
            if (candidate.EffectiveEnemyValue != current.EffectiveEnemyValue) {
                return candidate.EffectiveEnemyValue > current.EffectiveEnemyValue;
            }

            if (candidate.DieValue != current.DieValue) {
                return candidate.DieValue > current.DieValue;
            }
        }
        if (HaveSameSelfSecuringDraftBreakpointProfile(candidate, current)) {
            if (candidate.EffectivePlayerValue != current.EffectivePlayerValue) {
                return candidate.EffectivePlayerValue > current.EffectivePlayerValue;
            }

            if (candidate.IsYellow != current.IsYellow) {
                return candidate.IsYellow;
            }
        }
        int candidateSpentStamina = candidate.BestPlan?.SpentStamina ?? int.MaxValue;
        int currentSpentStamina = current.BestPlan?.SpentStamina ?? int.MaxValue;
        if (candidateSpentStamina != currentSpentStamina) { return candidateSpentStamina < currentSpentStamina; }
        if (candidate.CompletesKillBreakpoint != current.CompletesKillBreakpoint) { return candidate.CompletesKillBreakpoint; }
        if (candidate.CompletesHitBreakpoint != current.CompletesHitBreakpoint) { return candidate.CompletesHitBreakpoint; }
        if (candidate.CompletesDefenseBreakpoint != current.CompletesDefenseBreakpoint) { return candidate.CompletesDefenseBreakpoint; }
        if (candidate.CompletesOrderBreakpoint != current.CompletesOrderBreakpoint) { return candidate.CompletesOrderBreakpoint; }
        if (candidate.CompletesArmpitsBreakpoint != current.CompletesArmpitsBreakpoint) { return candidate.CompletesArmpitsBreakpoint; }
        if (candidate.CompletesHeadBreakpoint != current.CompletesHeadBreakpoint) { return candidate.CompletesHeadBreakpoint; }
        if (candidate.ReinforcesPlayerDamage != current.ReinforcesPlayerDamage) { return candidate.ReinforcesPlayerDamage; }
        if (candidate.ReinforcesPlayerDefense != current.ReinforcesPlayerDefense) { return candidate.ReinforcesPlayerDefense; }
        if (ShouldPreferPlayerDenialBeforeSoftUtility(candidate, current)
            && !Mathf.Approximately(candidate.PlayerDenialScore, current.PlayerDenialScore)) {
            return candidate.PlayerDenialScore > current.PlayerDenialScore;
        }
        if (candidate.DeniesPlayerGoFirst != current.DeniesPlayerGoFirst) { return candidate.DeniesPlayerGoFirst; }
        if (candidate.DeniesPlayerTarget != current.DeniesPlayerTarget) { return candidate.DeniesPlayerTarget; }
        if (!Mathf.Approximately(candidate.ProgressScore, current.ProgressScore)) { return candidate.ProgressScore > current.ProgressScore; }
        if (candidate.LosesValueToHatchet != current.LosesValueToHatchet) { return !candidate.LosesValueToHatchet; }
        if (candidate.DieValue == current.DieValue && candidate.IsYellow != current.IsYellow) { return candidate.IsYellow; }
        if (candidate.DieValue != current.DieValue) { return candidate.DieValue > current.DieValue; }
        int candidateTotalOverspend = candidate.BestPlan?.TotalOverspend ?? int.MaxValue;
        int currentTotalOverspend = current.BestPlan?.TotalOverspend ?? int.MaxValue;
        if (candidateTotalOverspend != currentTotalOverspend) { return candidateTotalOverspend < currentTotalOverspend; }
        int candidateResourceOverspend = candidate.BestPlan?.ResourceOverspend ?? int.MaxValue;
        int currentResourceOverspend = current.BestPlan?.ResourceOverspend ?? int.MaxValue;
        if (candidateResourceOverspend != currentResourceOverspend) { return candidateResourceOverspend < currentResourceOverspend; }
        if (!Mathf.Approximately(candidate.PlayerDenialScore, current.PlayerDenialScore)) { return candidate.PlayerDenialScore > current.PlayerDenialScore; }
        if (!Mathf.Approximately(candidate.FallbackScore, current.FallbackScore)) { return candidate.FallbackScore > current.FallbackScore; }
        return false;
    }

    private static bool ShouldPreferPlayerDenialBeforeSoftUtility(DraftChoiceEvaluation candidate, DraftChoiceEvaluation current) {
        return !HasAnySelfSecuringDraftBreakpoint(candidate)
            && !HasAnySelfSecuringDraftBreakpoint(current)
            && !HasAnyHardDraftDenial(candidate)
            && !HasAnyHardDraftDenial(current);
    }

    private static bool ShouldPreferNearFutilePowerDie(DraftChoiceEvaluation candidate, DraftChoiceEvaluation current) {
        if (candidate == null || current == null) { return false; }

        if (HasAnySelfSecuringDraftBreakpoint(candidate) || HasAnySelfSecuringDraftBreakpoint(current)) {
            return false;
        }

        if (HasAnyHardDraftDenial(candidate) || HasAnyHardDraftDenial(current)) {
            return false;
        }

        AdvancedPlanEvaluation candidatePlan = candidate.BestPlan;
        AdvancedPlanEvaluation currentPlan = current.BestPlan;
        if (candidatePlan == null || currentPlan == null) { return false; }

        bool candidateNearFutile = !candidatePlan.EnemyDamagesPlayer && !candidatePlan.EnemyAvoidsDamage;
        bool currentNearFutile = !currentPlan.EnemyDamagesPlayer && !currentPlan.EnemyAvoidsDamage;
        return candidateNearFutile && currentNearFutile;
    }

    private static bool HaveSameSelfSecuringDraftBreakpointProfile(DraftChoiceEvaluation candidate, DraftChoiceEvaluation current) {
        if (candidate == null || current == null) { return false; }

        return candidate.CompletesKillBreakpoint == current.CompletesKillBreakpoint
            && candidate.CompletesHitBreakpoint == current.CompletesHitBreakpoint
            && candidate.CompletesDefenseBreakpoint == current.CompletesDefenseBreakpoint
            && candidate.CompletesOrderBreakpoint == current.CompletesOrderBreakpoint
            && candidate.CompletesArmpitsBreakpoint == current.CompletesArmpitsBreakpoint
            && candidate.CompletesHeadBreakpoint == current.CompletesHeadBreakpoint;
    }

    private static bool HasAnySelfSecuringDraftBreakpoint(DraftChoiceEvaluation evaluation) {
        return evaluation != null && (
            evaluation.CompletesKillBreakpoint
            || evaluation.CompletesHitBreakpoint
            || evaluation.CompletesDefenseBreakpoint
            || evaluation.CompletesOrderBreakpoint
            || evaluation.CompletesArmpitsBreakpoint
            || evaluation.CompletesHeadBreakpoint);
    }

    private static bool HasAnyHardDraftDenial(DraftChoiceEvaluation evaluation) {
        return evaluation != null && (
            evaluation.DeniesPlayerKill
            || evaluation.DeniesPlayerDamage
            || evaluation.DeniesPlayerDefense
            || evaluation.ReinforcesPlayerDamage
            || evaluation.ReinforcesPlayerDefense);
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

    private static bool DraftDieDeniesPlayerDefense(Scripts s, Dice dice) {
        if (s == null || dice == null) { return false; }

        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        if (enemyAtt <= playerDef || effectivePlayerValue <= 0) { return false; }

        if (dice.diceType == "white") {
            return playerDef + effectivePlayerValue >= enemyAtt;
        }

        if (dice.diceType == "yellow") {
            return playerDef + effectivePlayerValue >= enemyAtt;
        }

        return false;
    }

    private static bool DraftDieReinforcesPlayerDamage(Scripts s, Dice dice) {
        if (s == null || dice == null || IsPlayerGuardSelected(s)) { return false; }

        int playerAim = s.statSummoner.SumOfStat("green", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        bool currentDamage = playerAim >= 0 && playerAtt > enemyDef;
        if (!currentDamage || effectivePlayerValue <= 0) { return false; }

        return GetPlayerDraftAssignmentOptions(s, dice).Contains("red");
    }

    private static bool DraftDieReinforcesPlayerDefense(Scripts s, Dice dice) {
        if (s == null || dice == null) { return false; }

        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        bool currentDefense = playerDef >= enemyAtt;
        if (!currentDefense || effectivePlayerValue <= 0) { return false; }

        return GetPlayerDraftAssignmentOptions(s, dice).Contains("white");
    }

    private static bool DraftDieDeniesPlayerGoFirst(Scripts s, Dice dice) {
        if (IsDraftInitiativeLocked(s)) { return false; }
        if (PlayerAlwaysActsFirst(s)) { return false; }
        int playerSpd = s.statSummoner.SumOfStat("blue", "player");
        int enemySpd = s.statSummoner.SumOfStat("blue", "enemy");
        int playerDef = s.statSummoner.SumOfStat("white", "player");
        int playerAtt = s.statSummoner.SumOfStat("red", "player");
        int enemyAtt = s.statSummoner.SumOfStat("red", "enemy");
        int enemyDef = s.statSummoner.SumOfStat("white", "enemy");
        int effectivePlayerValue = GetEffectivePlayerDraftValue(s, dice);
        bool enemyCanHitNow = enemyAtt > playerDef;
        bool enemyCanDefendNow = enemyDef >= playerAtt;

        if (!enemyCanHitNow && !enemyCanDefendNow) { return false; }
        if (playerSpd >= enemySpd || effectivePlayerValue <= 0) { return false; }
        // player wins ties, so denial here only means "player could have reached strict/locking first-move status with this die"
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

        // advanced yellow draft picks are previewed against every stat row before being committed
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
            // once the draft is over, both combat previews and visible hard-mode intent need to be fully refreshed
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
        // walk downward from the highest legal wound until finding one the player does not already have
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

    private static bool EnemyHitDamagesPlayer(SimState state, bool enemyCanHit, bool enemyActsFirst) {
        if (!EnemyHitConnects(state, enemyCanHit, enemyActsFirst)) { return false; }
        if (PlayerHasOneShotProtection(state)) { return false; }
        return true;
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
        // every fact that can change the planner outcome must be represented here,
        // otherwise hard/nightmare may reuse stale plans after a mid-round change
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
        hash.Add(s.itemManager.PlayerAlwaysChoosesFirstDraftDie());
        hash.Add(s.itemManager.PlayerAlwaysChoosesLastDraftDie());
        hash.Add(s.itemManager.PlayerAlwaysActsFirst());
        hash.Add(s.itemManager.PlayerAlwaysActsLast());
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

        // blue spend is only worth searching if the resulting speed line can still support a real attack outcome
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
        return s != null && (IsPlayerDraftSpeedLockedHigh(s) || IsEnemyDraftSpeedLockedHigh(s));
    }

    private static bool IsPlayerDraftSpeedLockedHigh(Scripts s) {
        return s != null && s.itemManager != null && s.itemManager.PlayerAlwaysChoosesFirstDraftDie();
    }

    private static bool IsEnemyDraftSpeedLockedHigh(Scripts s) {
        return s != null && s.itemManager != null && s.itemManager.PlayerAlwaysChoosesLastDraftDie();
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

        // if the player wounds first, red spending must be evaluated against the enemy's post-wound attack total instead
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
            // spending into a hit that dodgy will blank is especially wasteful
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
        bool enemyCanHitNow = enemyAtt > playerDef;
        bool enemyCanDefendNow = enemyDef >= playerAtt;
        int futureStamina = ((!s.enemy.woundList.Contains("hip") && !s.itemManager.EnemyHasTemporaryHipInjury()) || s.enemy.enemyName.text == "Lich")
            ? Mathf.Max(0, s.enemy.stamina)
            : 0;
        float score = 0f;

        switch (stat) {
            case "red": {
            // reward shrinking the damage gap, especially when the enemy still cannot hit at all
                int currentGap = Mathf.Max(0, playerDef + 1 - enemyAtt);
                int nextGap = Mathf.Max(0, playerDef + 1 - (enemyAtt + effectiveEnemyValue + futureStamina));
                int gapReduction = currentGap - nextGap;
                score += gapReduction * 38f;
                if (!enemyCanHitNow) {
                    score += gapReduction * 20f;
                }
                break;
            }
            case "white": {
                // reward shrinking the survival gap, especially when the enemy currently cannot defend itself
                int currentGap = Mathf.Max(0, playerAtt - enemyDef);
                int nextGap = Mathf.Max(0, playerAtt - (enemyDef + effectiveEnemyValue + futureStamina));
                int gapReduction = currentGap - nextGap;
                score += gapReduction * 42f;
                if (!enemyCanDefendNow) {
                    score += gapReduction * 20f;
                }
                if (enemyAtt <= playerDef) {
                    score += gapReduction * 14f;
                }
                break;
            }
            case "blue": {
                if (!PlayerAlwaysActsFirst(s)) {
                    int currentGap = Mathf.Max(0, playerSpd - enemySpd + 1);
                    int nextGap = Mathf.Max(0, playerSpd - (enemySpd + effectiveEnemyValue + futureStamina) + 1);
                    score += (currentGap - nextGap) * 16f;
                }
                if (PlayerCanBecomeDodgy(s) && !PlayerAlwaysActsFirst(s) && enemySpd <= playerSpd) {
                    score += effectiveEnemyValue * 14f;
                }
                if (!enemyCanHitNow || !enemyCanDefendNow) {
                    score -= effectiveEnemyValue * 10f;
                }
                break;
            }
            case "green": {
                // green progress tracks distance to the next meaningful wound breakpoint rather than raw aim inflation
                int nextBreakpoint = enemyAim < 4 ? 4 : enemyAim < 6 ? 6 : enemyAim < 7 ? 7 : -1;
                if (nextBreakpoint > 0) {
                    int currentGap = Mathf.Max(0, nextBreakpoint - enemyAim);
                    int nextGap = Mathf.Max(0, nextBreakpoint - (enemyAim + effectiveEnemyValue + futureStamina));
                    score += (currentGap - nextGap) * 12f;
                }
                if (!enemyCanHitNow || !enemyCanDefendNow) {
                    score -= effectiveEnemyValue * 8f;
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

    /// <summary>
    /// Creates a fresh zeroed stat dictionary keyed by the four combat colors.
    /// </summary>
    private static Dictionary<string, int> NewStatDictionary() {
        return new Dictionary<string, int> {
            { "green", 0 },
            { "blue", 0 },
            { "red", 0 },
            { "white", 0 },
        };
    }
}
