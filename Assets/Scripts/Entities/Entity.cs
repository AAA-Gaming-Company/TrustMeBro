using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public int maxHealth;

    private int health;
    private bool dead = false;

    public void Start() {
        this.health = this.maxHealth;
    }

    protected abstract void OnDie();
    protected abstract void OnDamage(int amount);

    public void TakeDamage(int damage) {
        this.OnDamage(damage);

        this.health -= damage;
        if (this.health <= 0) {
            this.Die();
        }
    }

    public void Die() {
        this.OnDie();

        this.dead = true;
        Destroy(this.gameObject);
    }
}
