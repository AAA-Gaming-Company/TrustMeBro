using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public int maxHealth;
    public ProgressBar healthBar;

    [Header("Cover System")]
    public LayerMask coverLayers;
    public float coverRadius = 3f;

    protected bool inCover = false;
    protected int currentHealth;

    public void Start() {
        this.currentHealth = this.maxHealth;
        this.UpdateHealthBar();
    }

    protected abstract void OnDie();
    protected abstract void OnDamage(int amount);

    public void TakeDamage(int damage) {
        this.OnDamage(damage);

        this.currentHealth -= damage;
        if (this.currentHealth <= 0) {
            this.Die();
        }

        this.UpdateHealthBar();
    }

    public void Die() {
        this.OnDie();
        Destroy(this.gameObject);
    }

    public void UpdateHealthBar() {
        this.healthBar.min = 0;
        this.healthBar.max = this.maxHealth;
        this.healthBar.UpdateValue(this.currentHealth);
    }

    public bool IsInCover() {
        return this.inCover;
    }

    public void SearchForCover() {
        Debug.Log("Searching for cover" + this.coverLayers.value);

        Collider2D coverHit = Physics2D.OverlapCircle(this.transform.position, coverRadius, this.coverLayers.value);
        if (coverHit == null) {
            Debug.Log("No cover found");
            return;
        }

        Cover cover = coverHit.gameObject.GetComponent<Cover>();
        Debug.Log(cover.name);
        cover.EnterNearestCoverPoint(this.gameObject);
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(base.transform.position, this.coverRadius);
    }
}
