using UnityEngine;

public class PlayerController : Entity {
    [Header("Player")]
    public float playerSpeed;

    private Rigidbody2D rb;
    private float inputX = 0;
    private float inputY = 0;
    private Camera cam;

	public new void Start() {
        base.Start();

        this.rb = GetComponent<Rigidbody2D>();
        this.cam = Camera.main;
    }

    private void FixedUpdate() {
        this.inputX = Input.GetAxis("Horizontal");
        this.inputY = Input.GetAxis("Vertical");

        this.rb.AddForce(new Vector2(this.inputX * this.playerSpeed, this.inputY * this.playerSpeed));
    }

    private void Update() {
        this.UpdatePlayerSprite();
    }

    private void UpdatePlayerSprite() {
        //Horizontal flip
        Vector3 localScale = base.transform.localScale;
        if (this.inputX > 0) {
            localScale.x = 1;
        } else if (this.inputX < 0) {
            localScale.x = -1;
        }
        this.transform.localScale = localScale;
    }

    protected override void OnDie() {
    }

    protected override void OnDamage(int amount) {
    }
}
