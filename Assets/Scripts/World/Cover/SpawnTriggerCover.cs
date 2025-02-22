using UnityEngine;
using Pathfinding;

[CreateAssetMenu(fileName = "SpawnTriggerCover", menuName = "Scriptable Objects/SpawnTriggerCover")]
public class SpawnTriggerCover : TriggerCover {
    [Header("Spawn")]
    public Transform spawnLocation;
    public Spawn[] spawns;

    [Header("AI Destination")]
    public bool hasAiDestination;
    public Transform aiDestination;

    [System.Serializable]
    public class Spawn {
        public GameObject prefab;
        public int amount;
    }

    private void Awake() {
        if (this.spawnLocation == null) {
            Debug.LogError("Spawn Location is not set!");
        }
        if (this.aiDestination == null && this.hasAiDestination) {
            Debug.LogError("AI Destination is not set");
        }
    }

    protected override void TriggerFunction(GameObject gameObject) {
        for (int i = 0; i < this.spawns.Length; i++) {
            Spawn spawn = this.spawns[i];

            for (int j = 0; j < spawn.amount; j++) {
                GameObject newObject = Instantiate(spawn.prefab, this.spawnLocation.position, Quaternion.identity);

                //Make sure it has a Z=0
                Vector3 pos = newObject.transform.position;
                pos.z = 0;
                newObject.transform.position = pos;

                //AI Destination
                if (this.hasAiDestination) {
                    AIDestinationSetter aiDestinationSetter = newObject.GetComponent<AIDestinationSetter>();
                    aiDestinationSetter.target = this.aiDestination;
                }
            }
        }
    }

    public new void OnDrawGizmos() {
        base.OnDrawGizmos();

        if (this.spawnLocation != null) {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(this.spawnLocation.position, new Vector3(.5f, .5f, .5f));
        }
    }
}
