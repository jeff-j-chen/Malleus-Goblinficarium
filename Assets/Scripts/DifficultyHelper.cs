using System;

/// <summary>
/// Centralizes difficulty naming, migration, and difficulty-specific feature checks.
/// </summary>
public static class DifficultyHelper {
    public const int CurrentDifficultyVersion = 2; // version used when migrating old persistent difficulty values
    public const string Easy = "easy"; // lowest difficulty preset
    public const string Normal = "normal"; // default difficulty preset
    public const string Hard = "hard"; // advanced AI difficulty before nightmare
    public const string Nightmare = "nightmare"; // highest difficulty preset

    public static readonly string[] OrderedDifficulties = { Easy, Normal, Hard, Nightmare }; // cycle order used by the character select menu

    /// <summary>
    /// Normalizes arbitrary difficulty text into one of the supported constants.
    /// </summary>
    /// <param name="difficulty">raw difficulty text from save data or UI</param>
    /// <returns>a canonical difficulty constant</returns>
    public static string Normalize(string difficulty) {
        if (string.IsNullOrWhiteSpace(difficulty)) { return Normal; }

        string lowered = difficulty.Trim().ToLowerInvariant();
        return lowered switch {
            Easy => Easy,
            Normal => Normal,
            Hard => Hard,
            Nightmare => Nightmare,
            _ => Normal,
        };
    }

    /// <summary>
    /// Migrates a saved difficulty forward when old versions rename or repurpose tiers.
    /// </summary>
    /// <param name="difficulty">saved difficulty string</param>
    /// <param name="version">saved difficulty schema version</param>
    /// <returns>difficulty adjusted for the current ruleset</returns>
    public static string Migrate(string difficulty, int version) {
        string normalized = Normalize(difficulty);
        if (version < CurrentDifficultyVersion && normalized == Hard) {
            // version 2 split the old hard tier into new hard + nightmare, so old hard saves map upward
            return Nightmare;
        }

        return normalized;
    }

    /// <summary>
    /// Returns the next difficulty in the menu cycle.
    /// </summary>
    /// <param name="difficulty">current difficulty string</param>
    /// <returns>next difficulty in `OrderedDifficulties`</returns>
    public static string Next(string difficulty) {
        string normalized = Normalize(difficulty);
        int index = Array.IndexOf(OrderedDifficulties, normalized);
        if (index < 0) { return Normal; }

        return OrderedDifficulties[(index + 1) % OrderedDifficulties.Length];
    }

    /// <summary>
    /// Returns whether the supplied difficulty resolves to easy.
    /// </summary>
    /// <param name="difficulty">difficulty string to test</param>
    public static bool IsEasy(string difficulty) {
        return Normalize(difficulty) == Easy;
    }

    /// <summary>
    /// Returns whether the supplied difficulty resolves to normal.
    /// </summary>
    /// <param name="difficulty">difficulty string to test</param>
    public static bool IsNormal(string difficulty) {
        return Normalize(difficulty) == Normal;
    }

    /// <summary>
    /// Returns whether the supplied difficulty resolves to hard.
    /// </summary>
    /// <param name="difficulty">difficulty string to test</param>
    public static bool IsHard(string difficulty) {
        return Normalize(difficulty) == Hard;
    }

    /// <summary>
    /// Returns whether the supplied difficulty resolves to nightmare.
    /// </summary>
    /// <param name="difficulty">difficulty string to test</param>
    public static bool IsNightmare(string difficulty) {
        return Normalize(difficulty) == Nightmare;
    }

    /// <summary>
    /// Returns whether the difficulty should use the advanced enemy planner.
    /// </summary>
    /// <param name="difficulty">difficulty string to test</param>
    public static bool UsesAdvancedEnemyAI(string difficulty) {
        string normalized = Normalize(difficulty);
        return normalized == Hard || normalized == Nightmare;
    }
}
