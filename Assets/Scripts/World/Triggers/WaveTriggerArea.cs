
using UnityEngine;

public class WaveTriggerArea : TriggerArea {
    public WaveSpawner waveSpawner;

    private void Awake() {
        if (this.GetComponent<WaveSpawner>() != null) {
            Debug.LogError("No! Do not attach a WaveSpawner to WaveTriggerArea.");
        }
    }

    protected override void TriggerAction(PlayerController player) {
        this.waveSpawner.StartWaves();
    }
}
