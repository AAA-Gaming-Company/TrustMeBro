using UnityEngine;

public class DialogueTriggerArea : TriggerArea {
    [Header("Dialogue")]
    public DialogueEntry[] entries;

    protected override void TriggerAction(PlayerController player) {
        DialogueBuilder.Builder()
            .AddEntry(this.entries)
            .BuildAndDisplay();
    }
}
