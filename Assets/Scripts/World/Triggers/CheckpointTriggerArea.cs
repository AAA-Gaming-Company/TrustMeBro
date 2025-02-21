using UnityEngine;
using Unity.Cinemachine;

public class CheckpointTriggerArea : TriggerArea {
    [Header("Checkpoint")]
    public Transform respawnLocation;
    public CinemachineCamera cam;
    
    private CinemachinePositionComposer positionComposer;

    private void Awake() {
        if (this.respawnLocation == null) {
            Debug.LogError("Respawn Location is not set!");
        }
        positionComposer = cam.GetComponent<CinemachinePositionComposer>();
    }

    protected override void TriggerAction(PlayerController player) {
        if (!this.destroyOnTrigger) {
            throw new System.Exception("CheckpointTriggerArea must be set to destroy on trigger, not sure the player will like it very much otherwise.");
        }

        GameManager.lastCheckpoint = new Checkpoint {
            position = this.respawnLocation.position,
            inventory = player.inventory.Copy(),
            camSize = cam.Lens.OrthographicSize,
            camOffset = positionComposer.Composition.ScreenPosition,
            deadZone = positionComposer.Composition.DeadZone.Enabled,
            deadZoneSize = positionComposer.Composition.DeadZone.Size
        };
    }

    public void OnDrawGizmos() {
        if (this.respawnLocation != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(this.respawnLocation.position, new Vector3(.5f, .5f, .5f));
        }
    }
}
