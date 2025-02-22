using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class WaveSpawner : MonoBehaviour {
    public WaveSpawns[] waves;

    [Header("AI Destination")]
    public bool hasAiDestination;
    public Transform aiDestination;

    [Header("Finish dialogue")]
    public bool finishWithDialogue;
    public DialogueEntry[] entries;
    public UnityEvent afterDialogue;

    private int currentWave = -1;
    private List<int> currentWaveChildren = new List<int>();

    public void StartWaves() {
        if (this.currentWave != -1) {
            Debug.LogError("WaveSpawner already started.");
            return;
        }

        this.NextWave();
    }

    private void NextWave() {
        this.currentWave++;
        if (this.currentWave >= this.waves.Length) {
            if (this.finishWithDialogue) {
                this.QueueDialogue();
            }

            Destroy(this);
            return;
        }

        WaveSpawns wave = this.waves[this.currentWave];

        // Make sure the wave is not bugged
        if (wave.deathTriggersNextWave && wave.timeBeforeNextWave > 0) {
            Debug.LogError("Wave is set to trigger next wave on death, but also has a time before next wave.");
            return;
        } else if (!wave.deathTriggersNextWave && wave.timeBeforeNextWave == 0) {
            Debug.LogError("Wave is set to trigger next wave after time, but has no time before next wave.");
            return;
        }

        foreach (WaveSpawnObject waveSpawnObject in wave.waveSpawnObjects) {
            for (int i = 0; i < waveSpawnObject.amount; i++) {
                // Spawn the child
                GameObject child = Instantiate(waveSpawnObject.prefab, this.transform.position, Quaternion.identity);

                // AI Destination
                if (this.hasAiDestination) {
                    AIDestinationSetter aiDestinationSetter = child.GetComponent<AIDestinationSetter>();
                    aiDestinationSetter.target = this.aiDestination;
                }

                // Make sure the child has an Entity component
                Entity entity = child.GetComponent<Entity>();
                if (entity == null) {
                    Debug.LogError("Wave child does not have an Entity component.");
                    return;
                }

                if (wave.deathTriggersNextWave) {
                    entity.AddDeathListener(() => {
                        this.ChildDied(child.GetInstanceID());
                    });
                }

                // Add the child to the current wave children, so we can keep track of who is alive
                this.currentWaveChildren.Add(child.GetInstanceID());
            }
        }

        // If there is a time before the next wave, schedule it
        if (wave.timeBeforeNextWave > 0) {
            Invoke("NextWave", wave.timeBeforeNextWave);
        }
    }

    public void ChildDied(int childInstanceID) {
        this.currentWaveChildren.Remove(childInstanceID);

        if (this.currentWaveChildren.Count == 0) {
            this.NextWave();
        }
    }
    
    private void QueueDialogue() {
        DialogueBuilder.Builder()
            .AddEntry(this.entries)
            .OnClose(this.afterDialogue)
            .BuildAndDisplay();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(this.transform.position, new Vector3(.5f, .5f, .5f));
    }
}

[System.Serializable]
public class WaveSpawns {
    public WaveSpawnObject[] waveSpawnObjects;
    [Tooltip("Time in seconds. Completely optional, set to 0 if you just want to wait for all the enemies to die.")]
    public float timeBeforeNextWave = 0;
    [Tooltip("If true, the next wave will start when all enemies in this wave die. If false, the next wave will start after timeBeforeNextWave.")]
    public bool deathTriggersNextWave = true;
}

[System.Serializable]
public class WaveSpawnObject {
    public GameObject prefab;
    public int amount = 1;
}
