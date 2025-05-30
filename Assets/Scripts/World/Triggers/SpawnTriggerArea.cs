using Pathfinding;
using UnityEngine;

public class SpawnTriggerArea : TriggerArea {
    [Header("Spawn")]
    public Transform spawnLocation;
    public Spawn[] spawns;

    [Header("Entity kill area unlock")]
    public bool unlockAreaBarrier;
    [Tooltip("The collider that will be destroyed when one of the entities dies")]
    public Collider2D areaBarrierCollider;

    [Header("AI Destination")]
    public bool hasAiDestination;
    public Transform aiDestination;

    [Header("Destroy on trigger")]
    public bool destroyObjectsOnTrigger;
    public GameObject[] objectsToDestroy;

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

    protected override void TriggerAction(PlayerController player) {
        for (int i = 0; i < this.spawns.Length; i++) {
            Spawn spawn = this.spawns[i];

            for (int j = 0; j < spawn.amount; j++) {
                GameObject newObject = Instantiate(spawn.prefab, this.spawnLocation.position, Quaternion.identity);

                //Make sure it has a Z=0
                Vector3 pos = newObject.transform.position;
                pos.z = 0;
                newObject.transform.position = pos;

                //Unlock area barrier
                if (this.unlockAreaBarrier) {
                    Entity entity = newObject.GetComponent<Entity>();
                    if (entity == null) {
                        throw new System.Exception("Entity component not found on " + newObject.name);
                    }

                    entity.gameObject.AddComponent<EntityDeathAreaUnlocker>().areaBarrierCollider = this.areaBarrierCollider;
                }

                //AI Destination
                if (this.hasAiDestination) {
                    AIDestinationSetter aiDestinationSetter = newObject.GetComponent<AIDestinationSetter>();
                    aiDestinationSetter.target = this.aiDestination;
                }
            }
        }

        //Destroy objects
        if (this.destroyObjectsOnTrigger) {
            for (int i = 0; i < this.objectsToDestroy.Length; i++) {
                Destroy(this.objectsToDestroy[i]);
            }
        }
    }

    public void OnDrawGizmos() {
        if (this.spawnLocation != null) {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(this.spawnLocation.position, new Vector3(.5f, .5f, .5f));
        }
    }
}
