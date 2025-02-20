using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class TriggerArea : MonoBehaviour {
    public bool destroyOnTrigger;
    public bool slowTime = false;
    public float timeSpeed = 0.5f;
    public float timeSlowDuration = 0.5f;

    private void Start() {
        this.GetComponent<Collider2D>().isTrigger = true;
    }

    protected abstract void TriggerAction(PlayerController player);

    public void OnTriggerEnter2D(Collider2D collision) {
        PlayerController player = collision.GetComponent<PlayerController>();

        this.TriggerAction(player);

        if (this.slowTime && player != null) {
            player.SlowTime(timeSpeed, timeSlowDuration);
        }

        if (this.destroyOnTrigger) {
            Destroy(this.gameObject);
        }
    }
}
