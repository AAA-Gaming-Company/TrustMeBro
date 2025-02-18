using UnityEngine;

public class DialogueTriggerArea : TriggerArea {
    [Header("Dialogue")]
    public DialogueEntry[] entries;

    protected override void TriggerAction() {
        DialogueBuilder.Builder()
            .AddEntry(this.entries)
            .BuildAndDisplay();
    }
}
