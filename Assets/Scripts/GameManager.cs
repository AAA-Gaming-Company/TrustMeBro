using System.Numerics;
using UnityEngine;

public class GameManager {
    public static DifficultyLevel difficultyLevel = DifficultyLevel.AVERAGE_SITUATIONSHIP_ENJOYER;

    public static Checkpoint lastCheckpoint = null;
}

public class Checkpoint {
    public UnityEngine.Vector2 position;
    public PlayerInventory inventory;

    public UnityEngine.Vector2 camOffset;
    public float camSize;
    public bool deadZone;
    public UnityEngine.Vector2 deadZoneSize;


}

public enum DifficultyLevel {
    PRO_RIZZLER = 2,
    AVERAGE_SITUATIONSHIP_ENJOYER = 1,
    DATING_SIMULATOR_PLAYER = 0
}