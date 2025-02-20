using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public int maxHealth;
    public ProgressBar healthBar;
    public GameObject noFlip;
    public MMF_Player damageFeedback;
    public MMF_Player enterCoverFeedback;

    [Header("Cover System")]
    public GameObject coverIndicator;
    public LayerMask coverLayers;
    public float coverRadius = 3f;

    private Animator anim;
    private CoverEntry currentCoverEntry;
    protected int currentHealth;
    protected EntityDieType dieType = EntityDieType.DESTROY;

    public void Start() {
        this.currentHealth = this.maxHealth;
        this.anim = GetComponent<Animator>();
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
        if (damageFeedback != null) {
            damageFeedback.PlayFeedbacks();
        }
        this.UpdateHealthBar();
    }

    public void Die() {
        this.ExitCover();

        this.OnDie();
        if (this.dieType == EntityDieType.DESTROY) { 
            Destroy(this.gameObject);
        }
    }

    public void UpdateHealthBar() {
        this.healthBar.min = 0;
        this.healthBar.max = this.maxHealth;
        this.healthBar.UpdateValue(this.currentHealth);
    }

    public bool IsInCover() {
        return this.currentCoverEntry != null;
    }

    public Vector2 GetCoverDirection() {
        Vector2 direction = this.currentCoverEntry.coverPoint - this.currentCoverEntry.cover.gameObject.transform.position;
        direction.y = 0;
        direction.Normalize();

        return direction;
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
        if (this.anim != null) {
            this.anim.SetBool("isCrouching", true);
        }
        if (this.enterCoverFeedback != null) {
            enterCoverFeedback.PlayFeedbacks();
        }
        this.currentCoverEntry = cover.EnterCover(this.gameObject, cover.NearestCoverPointPos(this.transform.position));

        // If the cover indicator is set, activate it (as long as we got into cover)
        if (this.coverIndicator != null && this.currentCoverEntry != null) {
            coverIndicator.SetActive(true);
        }
    }

    public void ExitCover() {
        if (this.IsInCover()) {
            this.currentCoverEntry.ExitCover(this.gameObject);
            this.currentCoverEntry = null;

            if (this.coverIndicator != null) {
                coverIndicator.SetActive(false);
            }
            if (this.anim != null) {
                this.anim.SetBool("isCrouching", false);
            }
        }
    }

    public void FlipEntity(bool flipped) {
        Vector3 entityRotaion = this.transform.localEulerAngles;
        entityRotaion.y = flipped ? 180 : 0;
        this.transform.localEulerAngles = entityRotaion;

        if (this.noFlip != null) {
            Vector3 noFlipRotation = this.noFlip.transform.localEulerAngles;
            noFlipRotation.y = flipped ? 180 : 0;
            this.noFlip.transform.localEulerAngles = noFlipRotation;
        }
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(base.transform.position, this.coverRadius);
    }
}

public enum EntityDieType {
    DESTROY,
    NOTHING
}
