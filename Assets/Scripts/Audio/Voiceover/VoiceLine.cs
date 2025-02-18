using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Scriptable Objects/Voice Line")]
public class VoiceLine : ScriptableObject {
    [Header("Audio")]
    public AudioClip clip;

    [Header("Text")]
    [Multiline]
    public string transcription;
    [Min(0)]
    public float displayTime;
}
