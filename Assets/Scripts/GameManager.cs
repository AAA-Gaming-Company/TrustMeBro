
public class GameManager {
    public static DifficultyLevel difficultyLevel = DifficultyLevel.AVERAGE_SITUATIONSHIP_ENJOYER;

    public static int GetDifficultyLevelInt() {
        return (int) difficultyLevel;
    }
}

public enum DifficultyLevel {
    PRO_RIZZLER = 2,
    AVERAGE_SITUATIONSHIP_ENJOYER = 1,
    DATING_SIMULATOR_PLAYER = 0
}