using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Grenade : MonoBehaviour {
    public MMF_Player explosionFeedback;

    private bool ready = false;

    private float lastMagnitude = 0;
    private float explosionRadius;
    private int damage;
    private Rigidbody2D rb;
    private string ignoreTag;

    public void Init(Vector2 targetPos, float explosionRadius, float thriowForce, int damage, string ignoreTag = "") {
        this.explosionRadius = explosionRadius;
        this.damage = damage;
        this.rb = this.GetComponent<Rigidbody2D>();
        this.ignoreTag = ignoreTag;

        Vector2 direction = targetPos - (Vector2)this.transform.position;
        direction.Normalize();

        rb.linearVelocity = direction * rb.gravityScale * rb.mass * 2 * thriowForce;
        rb.angularVelocity = 600;
        this.lastMagnitude = rb.linearVelocity.magnitude;

        //Call last
        this.ready = true;
    }

    private void FixedUpdate() {
        if (!this.ready) {
            return;
        }

        if (Mathf.Abs(this.rb.linearVelocity.magnitude - this.lastMagnitude) > 1f) {
            if (this.explosionFeedback != null) {
                this.explosionFeedback.PlayFeedbacks();
                this.explosionFeedback.transform.parent = null;
            }

            //Create a circle around the grenade's position
            Collider2D[] hit = Physics2D.OverlapCircleAll(this.transform.position, this.explosionRadius);
            foreach (Collider2D c in hit) {
                Entity entity = c.GetComponent<Entity>();
                if (entity != null && c.gameObject.CompareTag(this.ignoreTag) == false) {
                    this.HitFunction(c.gameObject);
                    entity.TakeDamage(this.damage);
                }
            }  

            Destroy(this.gameObject);
            this.ExplodeFunction();
        }

        this.lastMagnitude = this.rb.linearVelocity.magnitude;
    }

    public abstract void ExplodeFunction();

    public abstract void HitFunction(GameObject hit);
}
