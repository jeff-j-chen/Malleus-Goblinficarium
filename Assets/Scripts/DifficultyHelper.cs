using System;

public static class DifficultyHelper {
    public const int CurrentDifficultyVersion = 2;
    public const string Easy = "easy";
    public const string Normal = "normal";
    public const string Hard = "hard";
    public const string Nightmare = "nightmare";

    public static readonly string[] OrderedDifficulties = { Easy, Normal, Hard, Nightmare };

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

    public static string Migrate(string difficulty, int version) {
        string normalized = Normalize(difficulty);
        if (version < CurrentDifficultyVersion && normalized == Hard) {
            return Nightmare;
        }

        return normalized;
    }

    public static string Next(string difficulty) {
        string normalized = Normalize(difficulty);
        int index = Array.IndexOf(OrderedDifficulties, normalized);
        if (index < 0) { return Normal; }

        return OrderedDifficulties[(index + 1) % OrderedDifficulties.Length];
    }

    public static bool IsEasy(string difficulty) {
        return Normalize(difficulty) == Easy;
    }

    public static bool IsNormal(string difficulty) {
        return Normalize(difficulty) == Normal;
    }

    public static bool IsHard(string difficulty) {
        return Normalize(difficulty) == Hard;
    }

    public static bool IsNightmare(string difficulty) {
        return Normalize(difficulty) == Nightmare;
    }

    public static bool UsesAdvancedEnemyAI(string difficulty) {
        string normalized = Normalize(difficulty);
        return normalized == Hard || normalized == Nightmare;
    }
}
