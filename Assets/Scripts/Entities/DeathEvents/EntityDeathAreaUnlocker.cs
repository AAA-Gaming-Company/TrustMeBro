using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityDeathAreaUnlocker : MonoBehaviour {
    public Collider2D areaBarrierCollider;

    private void Start() {
        this.GetComponent<Entity>().AddDeathListener(this.OnDie);
    }

    private void OnDie() {
        Destroy(this.areaBarrierCollider);
    }
}
