using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityDeathDialogue : MonoBehaviour {
    [Header("Dialogue (shown on death)")]
    public DialogueEntry[] entries;

    private void Start() {
        this.GetComponent<Entity>().AddDeathListener(this.OnDie);
    }

    private void OnDie() {
        DialogueBuilder.Builder()
            .AddEntry(this.entries)
            .BuildAndDisplay();
    }
}
