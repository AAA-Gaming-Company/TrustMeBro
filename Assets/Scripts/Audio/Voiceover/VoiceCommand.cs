using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Scriptable Objects/Voice Command")]
public class VoiceCommand : ScriptableObject {
    [Tooltip("All the different voice lines that can be played for this command. Each line conveys the same message, but in a different way.")]
    public VoiceLine[] lines;

    public VoiceLine ChooseRandomLine() {
        return this.lines[Random.Range(0, this.lines.Length)];
    }
}
