using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Shooter {
    private bool flipPlayer = false;
    private float inputX;
    private bool onGround;

    private float pressJumpTime = 0f;
    private bool letGoInGrace = false;

    private bool isSprinting = false;
    private float currentStamina;
    private Coroutine staminaHideCoroutine;

    private bool isUsingAutomaticWeapon = false;

    private Animator animator;
    private Collider2D collider2d;
    private Rigidbody2D rb;
    private Camera cam;

    [HideInInspector]
    public bool isClimbing;

    [Header("Controller Config")]
    public float playerSpeed = 4f;
    public float playerSprintSpeed = 6f;
    public float jumpPower = 18f;
    public float preJumpGrace = 0.1f;
    public LayerMask groundLayers;
    public MMF_Player regenerationFeedback;

    [Header("Stamina Config")]
    public ProgressBar staminaBar;
    [Min(0)]
    public float maxStamina = 100f;
    [Min(0)]
    public float staminaRegen = 10f;
    [Min(0)]
    public float sprintStaminaCost = 10f;

    [Header("Player Inventory Config")]
    public PlayerInventory inventory;
    public MMF_Player weaponPickupFeedback;
    public MMF_Player weaponSwitchFeedback;
    public Image background;
    public Image weaponImage;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponAmount;
    public CinemachineCamera cinemachineCam;

    public new void Awake() {
        base.Awake();

        this.animator = GetComponent<Animator>();
        this.collider2d = GetComponent<Collider2D>();
        this.rb = GetComponent<Rigidbody2D>();
        this.cam = Camera.main;

        this.currentStamina = this.maxStamina;
        this.staminaBar.gameObject.SetActive(false);

        this.dieType = EntityDieType.NOTHING;

        if (GameManager.lastCheckpoint != null) {
            this.transform.position = GameManager.lastCheckpoint.position;
            this.inventory = GameManager.lastCheckpoint.inventory;
            this.cinemachineCam.Lens.OrthographicSize = GameManager.lastCheckpoint.camSize;
            this.cinemachineCam.GetComponent<CinemachinePositionComposer>().Composition.ScreenPosition = GameManager.lastCheckpoint.camOffset;
            this.cinemachineCam.GetComponent<CinemachinePositionComposer>().Composition.DeadZone.Enabled = GameManager.lastCheckpoint.deadZone;
            this.cinemachineCam.GetComponent<CinemachinePositionComposer>().Composition.DeadZone.Size = GameManager.lastCheckpoint.deadZoneSize;
        }

        InputManager.Instance.Init();
    }

    public new void Start() {
        base.Start();

        this.inventory.CycleSelectedWeapon(true);
        this.SwitchWeapon(this.inventory.GetSelectedWeapon());

        this.inventory.RegisterInventoryChangedEvent(() => {
            this.SwitchWeapon(this.inventory.GetSelectedWeapon());
            this.OnPlayerSwitchWeapon();
        });

        base.AddDeathListener(this.OnDie);
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
        WeaponType selectedWeapon = this.inventory.GetSelectedWeapon();
        if ((InputManager.Instance.GetAttackDown(false) && selectedWeapon != null) || this.isUsingAutomaticWeapon) {
            this.isUsingAutomaticWeapon = selectedWeapon.isAutomatic;
            
            //Flip the player to the required direction when shooting
            Vector3 targetPos = this.cam.ScreenToWorldPoint(Input.mousePosition);
            if (targetPos.x < this.transform.position.x) {
                base.FlipEntity(true);
            } else {
                base.FlipEntity(false);
            }
            this.Shoot(targetPos, "Player");
            StatsManager.Instance.AddShotFired();

            if (selectedWeapon.isSingleUse) {
                this.inventory.RemoveItem(selectedWeapon, 1);
            }
        }
        if (InputManager.Instance.GetAttackUp()) {
            this.isUsingAutomaticWeapon = false;
        }

        //Cycle weapons
        if (InputManager.Instance.GetCycleItem() != 0) {
            if (this.inventory.CycleSelectedWeapon(InputManager.Instance.GetCycleItem() > 0)) {
                this.SwitchWeapon(this.inventory.GetSelectedWeapon());
                this.OnPlayerSwitchWeapon();

                if (this.weaponSwitchFeedback != null) {
                    this.weaponSwitchFeedback.PlayFeedbacks();
                }
                this.isUsingAutomaticWeapon = false;
            }
        }

        //Sprint
        if (InputManager.Instance.GetSprintDown()) {
            this.isSprinting = true;
            //TODO: Add animation here

            if (this.staminaHideCoroutine != null) {
                StopCoroutine(this.staminaHideCoroutine);
            }
            this.staminaBar.gameObject.SetActive(true);
        }
        if (InputManager.Instance.GetSprintUp()) {
            this.isSprinting = false;
            //TODO: Add animation here

            this.staminaHideCoroutine = StartCoroutine(this.HideStaminaBar());
        }
        if (this.currentStamina <= 0) {
            this.isSprinting = false;
        }

        //Cover
        if (InputManager.Instance.GetCrouchDown()) {
            if (this.IsInCover()) {
                this.ExitCover();
                this.animator.SetBool("isCrouching", false);
            } else {
                Cover potentialCover = this.NearestCover();
                if (potentialCover != null) {
                    this.EnterCover(potentialCover);
                    InputManager.Instance.MoveH(0f);
                    InputManager.Instance.MoveV(0f);
                    this.animator.SetBool("isCrouching", true);
                }
            }
        }
        if (InputManager.Instance.GetHorizontalInput() != 0 || InputManager.Instance.GetVerticalInput() != 0 || InputManager.Instance.GetJumpDown() || InputManager.Instance.GetInteractDown() || InputManager.Instance.GetAttackDown(false)) {
            this.ExitCover();
            this.animator.SetBool("isCrouching", false);
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
        if (this.isSprinting) {
            this.rb.linearVelocity = new Vector2(this.inputX * this.playerSprintSpeed, this.rb.linearVelocity.y);

            this.currentStamina -= this.sprintStaminaCost * Mathf.Abs(this.inputX) * Time.fixedDeltaTime;
            this.UpdateStaminaBar();
        } else {
            this.rb.linearVelocity = new Vector2(this.inputX * this.playerSpeed, this.rb.linearVelocity.y);

            if (this.currentStamina < this.maxStamina) {
                this.currentStamina += this.staminaRegen * Time.fixedDeltaTime;
                if (this.currentStamina > this.maxStamina) {
                    this.currentStamina = this.maxStamina;
                }
                this.UpdateStaminaBar();
            }
        }
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

    public void Heal(int amount) {
        this.currentHealth = Mathf.Clamp(this.currentHealth + amount, 0, this.maxHealth);
        if (this.regenerationFeedback != null) {
            this.regenerationFeedback.PlayFeedbacks();
        }
        this.UpdateHealthBar();
    }

    public void UpdateStaminaBar() {
        this.staminaBar.min = 0;
        this.staminaBar.max = this.maxStamina;
        this.staminaBar.UpdateValue(this.currentStamina);
    }

    private IEnumerator HideStaminaBar() {
        yield return new WaitForSeconds(1.5f);

        if (this.currentStamina < this.maxStamina) {
            this.staminaHideCoroutine = StartCoroutine(this.HideStaminaBar());
        } else {
            this.staminaBar.gameObject.SetActive(false);
        }
    }

    public void PickupItem(WeaponType weapon) {
        this.inventory.AddItem(weapon);
        if (this.weaponPickupFeedback != null) {
            this.weaponPickupFeedback.PlayFeedbacks();
        }

        this.inventory.CycleToWeapon(weapon);
    }

    public void OnPlayerSwitchWeapon() {
        if (this.weaponImage != null) {
            if (this.weaponImage.gameObject.activeSelf == false) {
                this.weaponImage.gameObject.SetActive(true);
            }
            if (this.background.gameObject.activeSelf == false) {
                this.background.gameObject.SetActive(true);
            }
            if (this.weaponAmount.gameObject.activeSelf == false) {
                this.weaponAmount.gameObject.SetActive(true);
            }
            if (this.weaponName.gameObject.activeSelf == false) {
                this.weaponName.gameObject.SetActive(true);
            }

            if (this.inventory.GetSelectedWeapon() != null) {
                this.weaponImage.sprite = this.inventory.GetSelectedWeapon().displaySprite;
                int amount = this.inventory.GetAmount(this.inventory.GetSelectedWeapon());
                if (amount == 1) {
                    this.weaponAmount.text = "";
                } else {
                    this.weaponAmount.text = amount.ToString();
                }
                this.weaponName.text = this.inventory.GetSelectedWeapon().displayName;
            } else {
                this.weaponImage.sprite = null;
                this.weaponImage.gameObject.SetActive(false);
                this.weaponAmount.text = "";
                this.weaponName.text = "";
            }
        }
    }

    private void OnDie() {
        StateManager.Instance.TriggerDeathMenu();
        this.UpdateHealthBar();
    }

    public void SlowTime(float timeSpeed, float timeSlowDuration) {
        StartCoroutine(this.TimeSlower(timeSpeed, timeSlowDuration));
    }

    private IEnumerator TimeSlower(float timeSpeed, float timeSlowDuration) {
        Time.timeScale = timeSpeed;
        yield return new WaitForSecondsRealtime(timeSlowDuration);
        Time.timeScale = 1;
    }
}
