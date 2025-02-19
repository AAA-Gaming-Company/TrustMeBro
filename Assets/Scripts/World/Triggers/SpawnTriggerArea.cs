using Pathfinding;
using UnityEngine;

public class SpawnTriggerArea : TriggerArea {
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
        if (this.aiDestination == null && this.hasAiDestination) {
            Debug.LogError("AI Destination is not set");
        }
    }

    protected override void TriggerAction() {
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
        public void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(this.spawnLocation.position, new Vector3(1, 1, 1));
    }
}
