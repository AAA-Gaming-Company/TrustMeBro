using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public int maxHealth;
    public ProgressBar healthBar;

    [Header("Cover System")]
    public LayerMask coverLayers;
    public float coverRadius = 3f;

    private CoverEntry currentCoverEntry;
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
        return this.currentCoverEntry != null;
    }

    public void SearchForCover() {
        // If the entity is already in cover, don't search for another one
        if (this.IsInCover()) {
            return;
        }

        Collider2D coverHit = Physics2D.OverlapCircle(this.transform.position, coverRadius, this.coverLayers.value);
        if (coverHit == null) {
            return;
        }

        Cover cover = coverHit.gameObject.GetComponent<Cover>();
        this.currentCoverEntry = cover.EnterNearestCoverPoint(this.gameObject);
    }

    public Cover NearestCover() {
        if (this.IsInCover()) {
            return null;
        }

        Collider2D coverHit = Physics2D.OverlapCircle(this.transform.position, coverRadius, this.coverLayers.value);
        if (coverHit == null) {
            return null;
        }

        Cover cover = coverHit.gameObject.GetComponent<Cover>();
        
        return cover;
    }

    public void ExitCover() {
        if (this.IsInCover()) {
            this.currentCoverEntry.ExitCover();
            this.currentCoverEntry = null;
        }
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(base.transform.position, this.coverRadius);
    }
}
