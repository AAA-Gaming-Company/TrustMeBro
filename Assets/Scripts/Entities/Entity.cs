using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public int maxHealth;
    public ProgressBar healthBar;
    public GameObject noFlip;
    public MMF_Player deathFeedback;
    public MMF_Player damageFeedback;
    public MMF_Player enterCoverFeedback;
    [Tooltip("If true, the entity will use material instancing for its sprite renderer. Used for damage flashes.")]
    public bool enableMaterialInstancing = true;
    public bool animateDeath = false;

    [Header("Cover System")]
    public GameObject coverIndicator;
    public LayerMask coverLayers;
    public float coverRadius = 3f;

    private Animator anim;
    private CoverEntry currentCoverEntry;
    protected SpriteRenderer spriteRenderer;
    protected int currentHealth;
    protected EntityDieType dieType = EntityDieType.DESTROY;

    private UnityEvent deathEvent = new UnityEvent();
    private UnityEvent<int> damageEvent = new UnityEvent<int>();

    public void Start() {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.currentHealth = this.maxHealth;
        this.anim = GetComponent<Animator>();
        this.UpdateHealthBar();
        if (this.enableMaterialInstancing) {
            this.spriteRenderer.material = new Material(this.spriteRenderer.material);
            //This shouldn't have a performance cost, URP batches all materials with the same shader
        }
    }

    public void AddDeathListener(UnityAction action) {
        this.deathEvent.AddListener(action);
    }

    public void AddDamageListener(UnityAction<int> action) {
        this.damageEvent.AddListener(action);
    }

    public void TakeDamage(int damage) {
        this.damageEvent.Invoke(damage);

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
        this.deathEvent.Invoke();
        if (this.animateDeath && this.anim != null) {
            GameObject corpse = Instantiate(new GameObject(), this.transform.position, Quaternion.identity);
            corpse.name = "Corpse";
            SpriteRenderer corpseRenderer = corpse.AddComponent<SpriteRenderer>();
            corpseRenderer.sprite = this.spriteRenderer.sprite;
            corpseRenderer.sortingLayerID = this.spriteRenderer.sortingLayerID;
            Animator corpseAnim = corpse.AddComponent<Animator>();
            corpseAnim.runtimeAnimatorController = this.anim.runtimeAnimatorController;
            corpseAnim.SetTrigger("Die");
            if (Random.value > 0.5f) {
                corpse.transform.localScale = new Vector3(-1, 1, 1);
            }
            
        } 
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

        this.currentCoverEntry = cover.EnterCover(this.gameObject, cover.NearestCoverPointPos(this.transform.position));
        // If the cover indicator is set, activate it (as long as we got into cover)
        if (this.coverIndicator != null && this.currentCoverEntry != null) {
            coverIndicator.SetActive(true);
        }

        if (this.anim != null) {
            this.anim.SetBool("isCrouching", true);
        }
        if (this.enterCoverFeedback != null) {
            enterCoverFeedback.PlayFeedbacks();
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
