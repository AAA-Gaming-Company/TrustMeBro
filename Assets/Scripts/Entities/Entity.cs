using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public int maxHealth;

    protected int currentHealth;
    private bool dead = false;

    public void Start() {
        this.currentHealth = this.maxHealth;
    }

    protected abstract void OnDie();
    protected abstract void OnDamage(int amount);

    public void TakeDamage(int damage) {
        this.OnDamage(damage);

        this.currentHealth -= damage;
        if (this.currentHealth <= 0) {
            this.Die();
        }
    }

    public void Die() {
        this.OnDie();

        this.dead = true;
        Destroy(this.gameObject);
    }
}
