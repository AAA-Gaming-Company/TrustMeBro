using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public int maxHealth;
    public ProgressBar healthBar;

    [Header("Cover System")]
    public GameObject coverIndicator;
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
        this.ExitCover();

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

    public Cover NearestCover() {
        if (this.IsInCover()) {
            return null;
        }

        Collider2D coverHit = Physics2D.OverlapCircle(this.transform.position, coverRadius, this.coverLayers.value);
        if (coverHit == null) {
            return null;
        }

        return coverHit.gameObject.GetComponent<Cover>();;
    }

    public void EnterCover(Cover cover) {
        if (this.IsInCover()) {
            Debug.LogError("Entity is already in cover! This shouldn't happen :(");
            return; 
        }

        this.currentCoverEntry = cover.EnterCover(this.gameObject, cover.NearestCoverPointPos(this.transform.position));

        // If the cover indicator is set, activate it (as long as we got into cover)
        if (this.coverIndicator != null && this.currentCoverEntry != null) {
            coverIndicator.SetActive(true);
        }
    }

    public void ExitCover() {
        if (this.IsInCover()) {
            this.currentCoverEntry.ExitCover();
            this.currentCoverEntry = null;

            if (this.coverIndicator != null) {
                coverIndicator.SetActive(false);
            }
        }
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(base.transform.position, this.coverRadius);
    }
}
