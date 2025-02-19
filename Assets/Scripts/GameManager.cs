using UnityEngine;

public class GameManager {
    public static DifficultyLevel difficultyLevel = DifficultyLevel.AVERAGE_SITUATIONSHIP_ENJOYER;

    public static bool hasCheckpoint = false;
    public static Vector2 lastCheckpoint = Vector2.zero;
}

public enum DifficultyLevel {
    PRO_RIZZLER = 2,
    AVERAGE_SITUATIONSHIP_ENJOYER = 1,
    DATING_SIMULATOR_PLAYER = 0
}