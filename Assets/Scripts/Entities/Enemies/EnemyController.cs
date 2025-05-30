using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Animator))]
public class EnemyController : Shooter {
    [Header("Enemy")]
    public WeaponType weapon;
    public float moveRange;
    public LayerMask playerLayer;
    public int healthBoostOnDeath;

    [Header("Attack Behavior")]
    [Tooltip("Time spent out of cover after shooting at the player")]
    public float outOfCoverTime;
    [Tooltip("Number of attacks per attack cycle")]
    public int shotNum;
    [Tooltip("Time between each enemy attack cycle")]
    public float recoverTime;
    [Tooltip("A value which determines how variable the enemy's attack timings will be")]
    public float behaviorRandomFactor;
    [Tooltip("The maximum number of shots the enemy can fire before needing to find cover and reload")]
    public int maxShotsOutOfCover;
    public LayerMask ignoreLayers;
    public bool disableMovementOnShoot = false;
    public float moveDisableTime = 0.2f;

    private int shotsFiredOutOfCover;
    private bool attackBehaviorRunning = false;
    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;
    private PlayerController player;
    private bool reloadingOutOfCover = false;
    private bool playerInMoveRange = false;
    private Animator animator;
    private Rigidbody2D rb;

    public new void Awake() {
        base.Awake();

        this.destinationSetter = GetComponent<AIDestinationSetter>();
        this.aiPath = GetComponent<AIPath>();
        this.animator = GetComponent<Animator>();

        if (this.weapon.isSingleUse) {
            throw new UnityException("Enemy weapons cannot be single use!");
        }
        this.SwitchWeapon(this.weapon);
        rb = GetComponent<Rigidbody2D>();
    }

    public new void Start() {
        base.Start();

        this.player = destinationSetter.target.GetComponent<PlayerController>();
        if (this.player == null) {
            throw new UnityException("PlayerController not found in target!");
        }

        base.AddDeathListener(this.OnDie);
    }

    private void Update() {
        if (Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.moveRange) {
            this.playerInMoveRange = true;
            this.animator.SetBool("isWalking", false);
        } else {
            this.playerInMoveRange = false;
        }

        if (this.IsReadyToShoot() && Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.GetShootRange() && this.CanHitPlayer() && this.IsInCover() == false && this.attackBehaviorRunning == false && this.shotsFiredOutOfCover < this.maxShotsOutOfCover) {
            this.ShootAtTarget(this.destinationSetter.target.position);
            this.shotsFiredOutOfCover++;
        } else if (this.shotsFiredOutOfCover == this.maxShotsOutOfCover && this.reloadingOutOfCover == false) {
            StartCoroutine(ReloadOutOfCover());
        }

        if (this.IsInCover() == true) {
            this.shotsFiredOutOfCover = 0;
            if (this.attackBehaviorRunning == false) {
                StartCoroutine(AttackBehavior());
            }
        }

        if (this.playerInMoveRange == true) {
            this.aiPath.canMove = false;
        } else if (Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.weapon.useRange) {
            if (this.IsInCover()) {
                this.aiPath.canMove = false;
            } else if (this.attackBehaviorRunning == false) {
                Cover potentialCover = this.NearestCover();

                if (potentialCover == null) {
                    this.aiPath.canMove = true;
                } else {
                    Vector3 coverPoint = potentialCover.NearestCoverPointPos(base.transform.position, destinationSetter.target.gameObject);

                    if (Vector2.Distance(coverPoint, this.destinationSetter.target.position) < this.weapon.useRange) {
                        this.EnterCover(potentialCover);
                    }
                }
            }
        } else {
            StopCoroutine(AttackBehavior());
            this.attackBehaviorRunning = false;
            this.ExitCover();
            this.aiPath.canMove = true;
        }

        if (this.aiPath.canMove) {
            this.animator.SetBool("isWalking", true);
            this.rb.excludeLayers = 0;
            this.spriteRenderer.sortingOrder = 0;
        } else {
            this.animator.SetBool("isWalking", false);

            if (this.IsInCover()) {
                this.rb.excludeLayers = this.ignoreLayers;
                this.spriteRenderer.sortingOrder = -1;
            } else {
                this.rb.excludeLayers = 0;
                this.spriteRenderer.sortingOrder = 0;
            }
        }

        base.FlipEntity(this.destinationSetter.target.position.x > base.transform.position.x);
    }

    //TODO: Maybe decrease the amount of times this is actually called, it's expensive.
    private bool CanHitPlayer() {
        Vector3 direction = this.destinationSetter.target.position - base.transform.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(base.transform.position, direction, Vector2.Distance(base.transform.position, this.destinationSetter.target.position));

        foreach (RaycastHit2D hit in hits) {
            if (hit.collider.gameObject.CompareTag("Player")) {
                return true;
            }
        }

        return false;
    }

    private IEnumerator AttackBehavior() {
        this.attackBehaviorRunning = true;
        float minRandom = 1 - this.behaviorRandomFactor;
        float maxRandom = 1 + this.behaviorRandomFactor;
        
        //Exiting cover to begin attacking
        this.ExitCover();
        this.aiPath.canMove = false;

        for (int i = 0; i < this.shotNum; i++) {
            if (this.IsReadyToShoot() && this.IsInCover() == false && Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.GetShootRange() && this.CanHitPlayer()) {
                //Fires the shot; then reloads
                this.ShootAtTarget(this.destinationSetter.target.position);
                yield return new WaitForSeconds(this.weapon.GetUseDelay());
            }
        }

        //Sits outside of cover for the player to shoot them
        yield return new WaitForSeconds(this.outOfCoverTime * Random.Range(minRandom, maxRandom));

        Cover potentialCover = this.NearestCover();
        if (potentialCover != null && Vector2.Distance(potentialCover.NearestCoverPointPos(base.transform.position, this.destinationSetter.target.gameObject), this.destinationSetter.target.position) < this.weapon.useRange) {
            //Get back into cover
            this.EnterCover(potentialCover);
        }

        //Recover and then we can start over :)
        yield return new WaitForSeconds(this.recoverTime * Random.Range(minRandom, maxRandom));
        this.attackBehaviorRunning = false;
    }

    private void ShootAtTarget(Vector3 target) {
        if (this.aiPath.canMove && this.disableMovementOnShoot) {
            this.aiPath.canMove = false;
            StartCoroutine(EnableMovement(this.moveDisableTime));
        }
        if (this.weapon.isProjectile) {
            this.Shoot(target);
        }else if (this.weapon.isGrenade) {
            bool playerIsLeft = target.x < base.transform.position.x;
            if (playerIsLeft) {
                Vector2 vertex = new Vector2(target.x + ((transform.position.x - target.x) / 2), target.y + 2);
                this.Shoot(vertex, "GrenadeEnemy");

            } else {
                Vector2 vertex = new Vector2(target.x - ((target.x - transform.position.x) / 2), target.y + 2);
                this.Shoot(vertex, "GrenadeEnemy");
            }
        }
    }

    private IEnumerator ReloadOutOfCover() {
        this.reloadingOutOfCover = true;
        yield return new WaitForSeconds(this.recoverTime * 1.5f);
        this.shotsFiredOutOfCover = 0;
        this.reloadingOutOfCover = false;
    }

    private new void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(base.transform.position, this.moveRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(base.transform.position, this.weapon.useRange);

        Gizmos.color = Color.grey;
        //TODO: Remove this code when fixed
        if (this.destinationSetter != null) {
            Vector3 direction = this.destinationSetter.target.position - base.transform.position;
            Gizmos.DrawRay(base.transform.position, direction);
        }
    }

    private IEnumerator EnableMovement(float time) {
        yield return new WaitForSeconds(time);
        this.aiPath.canMove = true;
    }

    private void OnDie() {
        StatsManager.Instance.AddEnemyKilled();
        this.player.Heal(this.healthBoostOnDeath);
    }
}
