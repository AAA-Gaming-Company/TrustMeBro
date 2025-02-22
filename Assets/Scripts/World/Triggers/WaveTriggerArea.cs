using UnityEngine;

[RequireComponent(typeof(WaveSpawner))]
public class WaveTriggerArea : TriggerArea {
    private WaveSpawner waveSpawner;

    private void Awake() {
        this.waveSpawner = this.GetComponent<WaveSpawner>();
    }

    protected override void TriggerAction(PlayerController player) {
        this.waveSpawner.StartWaves();
    }
}
