using UnityEngine;

public abstract class TriggerCover : Cover {
    [Header("Trigger")]
    public bool triggerOnce;
    public bool forceOnlyPlayerTrigger = true;
    public bool slowTime = false;
    public float timeSpeed = 0.5f;
    public float timeSlowDuration = 0.5f;

    private bool triggered = false;

    public override CoverEntry EnterCover(GameObject entity, int index) {
        PlayerController player = entity.GetComponent<PlayerController>();

        if ((!this.triggerOnce || (this.triggerOnce && !this.triggered)) && (!this.forceOnlyPlayerTrigger || player != null)) {
            Time.timeScale = 1;
            this.TriggerFunction(entity);

            this.triggered = true;
            if (this.slowTime){
                player.SlowTime(timeSpeed, timeSlowDuration);
            }
        }

        return base.EnterCover(entity, index);
    }

    protected abstract void TriggerFunction(GameObject entity);
}