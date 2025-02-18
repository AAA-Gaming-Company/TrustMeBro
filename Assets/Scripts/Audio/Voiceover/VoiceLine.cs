using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Scriptable Objects/Voice Line")]
public class VoiceLine : ScriptableObject {
    [Header("Audio")]
    public AudioClip clip;

    [Header("Text")]
    public string transcription;
    public float displayTime;
}
