using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Shooter {
    private bool flipPlayer = false;
    private float inputX;
    private bool onGround;

    private float pressJumpTime = 0f;
    private bool letGoInGrace = false;

    private Animator animator;
    private Collider2D collider2d;
    private Rigidbody2D rb;
    private Camera cam;

    [Header("Controller Config")]
    public float playerSpeed = 4f;
    public float jumpPower = 18f;
    public float preJumpGrace = 0.1f;
    public LayerMask groundLayers;

    public VoiceCommand demo;

    public new void Start() {
        base.Start();

        this.animator = GetComponent<Animator>();
        this.collider2d = GetComponent<Collider2D>();
        this.rb = GetComponent<Rigidbody2D>();
        this.cam = Camera.main;

        InputManager.Instance.Init();
    }

    private void Update() {
        this.inputX = InputManager.Instance.GetHorizontalInput();
        this.onGround = this.IsGrounded();

        //Stop jumping animation if the player's velocity is close to 0 and they are on the ground
        if (this.rb.linearVelocity.y >= -0.5f && this.rb.linearVelocity.y <= 0.5f && this.onGround) {
            this.animator.SetBool("isJumping", false);
        }

        //Countdown for grace before touching the platform
        this.pressJumpTime -= Time.deltaTime;
        if (this.pressJumpTime < 0) {
            this.pressJumpTime = 0;
        }
        //Set the grace to its maximum
        if (InputManager.Instance.GetJumpDown()) {
            this.pressJumpTime = this.preJumpGrace;
        }

        //If the player has let go of the jump button and they are still in the grace period
        if (this.pressJumpTime > 0 && InputManager.Instance.GetJumpUp()) {
            this.letGoInGrace = true;
        }

        //If the player is in the grace period (this can be even if they just pressed the button in this frame)
        //and the player touches the ground, jump
        if (this.pressJumpTime > 0 && this.onGround) {
            this.animator.SetBool("isJumping", false);
            this.animator.SetBool("isJumping", true);
            this.rb.linearVelocity = new Vector2(this.rb.linearVelocity.x, this.jumpPower);
            this.pressJumpTime = 0f; //Reset the grace period value
        }
        //You can jump less high by releasing the jump button
        if ((InputManager.Instance.GetJumpUp() || this.letGoInGrace) && this.rb.linearVelocity.y > 0f) {
            this.rb.linearVelocity = new Vector2(this.rb.linearVelocity.x, this.rb.linearVelocity.y * 0.5f);
            this.letGoInGrace = false;
        }

        //Flip the player if they are going in a different direction to the old one
        if (!this.flipPlayer && this.inputX < 0f || this.flipPlayer && this.inputX > 0f) {
            this.flipPlayer = !this.flipPlayer;
            base.FlipEntity(this.flipPlayer);
        }

        //Attack
        if (InputManager.Instance.GetAttackDown(false)) {
            this.Hit();
        }

        //Cover
        if (InputManager.Instance.GetCrouchDown()) {
            if (this.IsInCover()) {
                this.ExitCover();
            } else {
                Cover potentialCover = this.NearestCover();
                if (potentialCover != null) {
                    this.EnterCover(potentialCover);
                    InputManager.Instance.MoveH(0f);
                    InputManager.Instance.MoveV(0f);
                }
            }
        }
        if (InputManager.Instance.GetHorizontalInput() != 0 || InputManager.Instance.GetVerticalInput() != 0 || InputManager.Instance.GetJumpDown() || InputManager.Instance.GetInteractDown() || InputManager.Instance.GetAttackDown(false)) {
            this.ExitCover();
        }

        if (InputManager.Instance.GetInteractDown()) {
            VoiceManager.Instance.SendCommand(this.demo);
        }
    }

    private void FixedUpdate() {
        if (this.inputX == 0) {
            animator.SetBool("isWalking", false);
        } else {
            if (BetterPhysics2D.Linecast(this, LinecastDirection.FORWARDS, this.groundLayers, 0.2f)) {
                animator.SetBool("isWalking", false);
                return;
            }

            animator.SetBool("isWalking", true);
        }

        //Add the velocity to the player if the function hasn't returned
        this.rb.linearVelocity = new Vector2(this.inputX * this.playerSpeed, this.rb.linearVelocity.y);
    }

    private bool IsGrounded() {
        return BetterPhysics2D.Linecast(this, LinecastDirection.DOWN, this.groundLayers, 0.2f);
    }

    public bool IsPlayerFlipped() {
        if (this.inputX == 0) {
            return this.flipPlayer;
        } else {
            return this.inputX < 0;
        }
    }

    public Collider2D GetCollider() {
        return this.collider2d;
    }

    private void Hit() {
        this.Shoot(this.cam.ScreenToWorldPoint(Input.mousePosition));
        //TODO: Add feedback here
        StatsManager.Instance.AddShotFired();
    }

    public void Heal(int amount) {
        this.currentHealth = Mathf.Clamp(this.currentHealth + amount, 0, this.maxHealth);
        this.UpdateHealthBar();
    }

    protected override void OnDie() {
    }

    protected override void OnDamage(int amount) {
    }
}
