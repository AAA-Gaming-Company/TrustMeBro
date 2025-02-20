using UnityEngine;

public abstract class TriggerCover : Cover {
    [Header("Trigger")]
    public bool triggerOnce;
    public bool forceOnlyPlayerTrigger = true;

    private bool triggered = false;

    public override CoverEntry EnterCover(GameObject entity, int index) {
        if ((!this.triggerOnce || (this.triggerOnce && !this.triggered)) && (!this.forceOnlyPlayerTrigger || entity.GetComponent<PlayerController>() != null)) {
            this.TriggerFunction(entity);
            this.triggered = true;
        }

        return base.EnterCover(entity, index);
    }

    protected abstract void TriggerFunction(GameObject entity);
}