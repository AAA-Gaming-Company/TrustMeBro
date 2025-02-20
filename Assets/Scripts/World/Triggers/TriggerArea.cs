using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class TriggerArea : MonoBehaviour {
    public bool destroyOnTrigger;
    private Vector2 triggerPosition;
    public bool slowTime = false;
    public float timeSpeed = 0.5f;
    public float timeSlowDuration = 0.5f;

    private void Start() {
        this.GetComponent<Collider2D>().isTrigger = true;
    }

    protected abstract void TriggerAction();

    public void OnTriggerEnter2D(Collider2D collision) {
        this.triggerPosition = collision.transform.position;
        this.TriggerAction();
        PlayerController player = collision.GetComponent<PlayerController>();
        if (this.slowTime && player != null) {
            player.SlowTime(timeSpeed, timeSlowDuration);
        }
        if (this.destroyOnTrigger) {
            Destroy(this.gameObject);
        }
    }

    protected Vector2 GetTriggerPosition() {
        return this.triggerPosition;
    }
}
