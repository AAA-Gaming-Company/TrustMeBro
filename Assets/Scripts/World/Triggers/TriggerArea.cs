using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class TriggerArea : MonoBehaviour {
    public bool destroyOnTrigger;

    private void Start() {
        this.GetComponent<Collider2D>().isTrigger = true;
    }

    protected abstract void TriggerAction();

    public void OnTriggerEnter2D(Collider2D collision) {
        this.TriggerAction();

        if (this.destroyOnTrigger) {
            Destroy(this.gameObject);
        }
    }
}
