using UnityEngine;

public class DialogueTriggerCover : TriggerCover {
    [Header("Dialogue")]
    public DialogueEntry[] entries;

    protected override void TriggerFunction(GameObject entity) {
        DialogueBuilder.Builder()
            .AddEntry(this.entries)
            .BuildAndDisplay();
    }
}