using UnityEngine;

public class HealthTriggerArea : TriggerArea {
    [Header("Health")]
    [Tooltip("The amount of health to change by (positive or negative).")]
    public int healthChange = 0;

    protected override void TriggerAction(PlayerController player) {
        if (this.healthChange == 0) {
            Debug.LogError("Did you mean to make a health trigger area with no health change?");
            return;
        } else if (this.healthChange > 0) {
            player.Heal(this.healthChange);
        } else {
            player.TakeDamage(-this.healthChange);
        }
    }
}
