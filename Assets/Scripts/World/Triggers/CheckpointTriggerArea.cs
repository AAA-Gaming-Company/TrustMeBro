using UnityEngine;

public class CheckpointTriggerArea : TriggerArea {
    protected override void TriggerAction() {
        if (!this.destroyOnTrigger) {
            throw new System.Exception("CheckpointTriggerArea must be set to destroy on trigger, not sure the player will like it very much otherwise.");
        }

        GameManager.hasCheckpoint = true;
        GameManager.lastCheckpoint = base.GetTriggerPosition();
    }
}
