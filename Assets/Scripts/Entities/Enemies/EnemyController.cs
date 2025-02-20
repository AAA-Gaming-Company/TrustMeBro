using UnityEngine;
using Pathfinding;
using MoreMountains.Feedbacks;
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

    private int shotsFiredOutOfCover;
    private bool attackBehaviorRunning = false;
    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;
    private PlayerController player;
    private bool reloadingOutOfCover = false;
    private bool playerInMoveRange = false;
    private Animator animator;

    public new void Awake() {
        base.Awake();

        this.destinationSetter = GetComponent<AIDestinationSetter>();
        this.aiPath = GetComponent<AIPath>();
        this.animator = GetComponent<Animator>();

        if (this.weapon.isSingleUse) {
            throw new UnityException("Enemy weapons cannot be single use!");
        }
        this.SwitchWeapon(this.weapon);
    }

    public new void Start() {
        base.Start();

        this.player = destinationSetter.target.GetComponent<PlayerController>();
        if (this.player == null) {
            throw new UnityException("PlayerController not found in target!");
        }
    }

    private void Update() {
        if (Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.moveRange) {
            this.playerInMoveRange = true;
            this.animator.SetBool("isWalking", false);
        } else {
            this.playerInMoveRange = false;
        }

        if (this.IsReadyToShoot() && Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.GetShootRange() && this.CanHitPlayer() && this.IsInCover() == false && this.attackBehaviorRunning == false && this.shotsFiredOutOfCover < this.maxShotsOutOfCover) {
            this.Shoot(this.destinationSetter.target.position);
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
        } else {
            this.animator.SetBool("isWalking", false);
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
                this.Shoot(this.destinationSetter.target.position);
                yield return new WaitForSeconds((this.weapon.GetUseDelay() + 0.05f) * Random.Range(minRandom, maxRandom));
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

    protected override void OnDie() {
        StatsManager.Instance.AddEnemyKilled();
        this.player.Heal(this.healthBoostOnDeath);
    }

    protected override void OnDamage(int amount) {
    }
}
