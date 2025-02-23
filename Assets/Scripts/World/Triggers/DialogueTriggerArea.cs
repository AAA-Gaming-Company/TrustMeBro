using UnityEngine;

public class DialogueTriggerArea : TriggerArea {
    [Header("Dialogue")]
    public DialogueEntry[] entries;
    public bool teleportMandrew = true;
    public Transform mandrewTeleportTarget;
    public Transform mandrewTransform;

    protected override void TriggerAction(PlayerController player) {
        if (this.teleportMandrew) {
            this.mandrewTransform.position = this.mandrewTeleportTarget.position;
        }
        DialogueBuilder.Builder()
            .AddEntry(this.entries)
            .BuildAndDisplay();
    }
}
