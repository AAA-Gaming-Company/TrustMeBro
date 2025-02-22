using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Girl : Entity {
    public new void Start() {
        base.Start();
        this.AddDamageListener(this.OnDamage);
    }

    private void OnDamage(int amount)  {
        Destroy(gameObject);
    }
}
