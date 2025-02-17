using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public int maxHealth;
    public ProgressBar healthBar;
    
    public LayerMask coverLayers;
    public float coverRadius = 3f;

    private bool inCover = false;
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
        Debug.Log("Searching for cover");
        RaycastHit2D coverHit = Physics2D.CircleCast(this.transform.position, coverRadius, Vector2.zero, 0, coverLayers);
        if (coverHit.collider == null) {
            Debug.Log("No cover found :()");
            return;
        }

        Cover cover = coverHit.collider.gameObject.GetComponent<Cover>();
        Debug.Log(cover.name);

        transform.position = cover.GetNearestCoverPosition(transform.position);
        
    }
}
