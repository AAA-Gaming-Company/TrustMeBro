using UnityEngine;

public class Ladder : MonoBehaviour {
    public float ladderSpeed = 2f;

    private float oldGravityScale = 0;
    private PlayerController player = null;
    private Rigidbody2D playerRigidbody = null;

    private void Update() {
        if (this.playerRigidbody == null) {
            return;
        }

        if (InputManager.Instance.GetJumpDown()) {
            this.player.isClimbing = true;
        }
        if (InputManager.Instance.GetJumpUp()) {
            this.player.isClimbing = false;
        }
    }

    private void FixedUpdate() {
        if (this.playerRigidbody == null) {
            return;
        }

        if (this.player.isClimbing) {
            this.playerRigidbody.linearVelocity = new Vector2(this.playerRigidbody.linearVelocity.x, this.ladderSpeed);
        } else {
            this.playerRigidbody.linearVelocity = new Vector2(this.playerRigidbody.linearVelocity.x, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.CompareTag("Player")) {
            return;
        }

        this.player = other.gameObject.GetComponent<PlayerController>();

        Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
        this.oldGravityScale = rb.gravityScale;
        rb.gravityScale = 0;

        this.playerRigidbody = rb;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!other.gameObject.CompareTag("Player")) {
            return;
        }

        this.playerRigidbody.gravityScale = this.oldGravityScale;
        this.playerRigidbody.linearVelocity = new Vector2(this.playerRigidbody.linearVelocity.x, 0);
        this.player.isClimbing = false;

        this.playerRigidbody = null;
        this.player = null;
    }
}
