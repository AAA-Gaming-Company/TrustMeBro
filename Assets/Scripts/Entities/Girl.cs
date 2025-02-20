using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Girl: Entity{
    [Header("Dialogue (shown on death)")]
    public DialogueEntry[] entries;


    protected override void OnDamage(int amount)
    {
        Destroy(gameObject);
    }
    protected override void OnDie()
    {
        DialogueBuilder.Builder()
            .AddEntry(this.entries)
            .BuildAndDisplay();
    }
}
