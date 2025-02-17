using UnityEngine;
using Pathfinding;
using MoreMountains.Feedbacks;
using System.Collections;

[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AIPath))]
public class EnemyController : Shooter {
    [Header("Enemy")]
    public float moveRange;
    public LayerMask playerLayer;
    public MMF_Player shoot;
    public int healthBoostOnDeath;
    public float outOfCoverTime; //Time spent out of cover after shooting at the player
    public int shotNum; //Number of attacks per attack cycle
    public float recoverTime; //The time between each enemy attack cycle
    public float behaviorRandomFactor; //A value which determines how variable the enemy's attack timings will be
    public int maxShotsOutOfCover; //How many shots the enemy can fire before needing to find cover and reload

    private int shotsFiredOutOfCover;
    private bool attackBehaviorRunning = false;
    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;
    private PlayerController player;
    private bool reloadingOutOfCover = false;

    public new void Start() {
        base.Start();

        this.destinationSetter = GetComponent<AIDestinationSetter>();
        this.aiPath = GetComponent<AIPath>();
        this.player = destinationSetter.target.GetComponent<PlayerController>();
        if (this.player == null) {
            throw new UnityException("PlayerController not found in target!");
        }
    }

    private void Update() {
        if (this.isReadyToShoot() && Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.GetShootRange() && this.CanHitPlayer() && this.IsInCover() == false && attackBehaviorRunning == false && shotsFiredOutOfCover < maxShotsOutOfCover) {
            this.Shoot(this.destinationSetter.target.position);
            this.shoot.PlayFeedbacks();
            shotsFiredOutOfCover++;
        }else if (shotsFiredOutOfCover == maxShotsOutOfCover && reloadingOutOfCover == false) {
            StartCoroutine(ReloadOutOfCover());
        }

        if (this.IsInCover() == true) {
            shotsFiredOutOfCover = 0;
            if (attackBehaviorRunning == false) {
                StartCoroutine(AttackBehavior());
            }
        }

        if (Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.moveRange) {
            this.aiPath.canMove = false;
        } else if (Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.weapon.useRange) {
            if (this.IsInCover()) {
                this.aiPath.canMove = false;
            } else if (attackBehaviorRunning == false) {
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
            attackBehaviorRunning = false;
            this.ExitCover();
            this.aiPath.canMove = true;
        }
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
        attackBehaviorRunning = true;
        float minRandom = 1-behaviorRandomFactor;
        float maxRandom = 1+behaviorRandomFactor;
        
        this.ExitCover();
        //Exiting cover to begin attacking
        this.aiPath.canMove = false;
        for (int i = 0; i < shotNum; i++) {
            if (this.isReadyToShoot() && Vector2.Distance(base.transform.position, this.destinationSetter.target.position) < this.GetShootRange() && this.CanHitPlayer() && this.IsInCover() == false) {
            this.Shoot(this.destinationSetter.target.position);
            this.shoot.PlayFeedbacks();
            yield return new WaitForSeconds((this.weapon.useDelay[GameManager.GetDifficultyLevelInt()] + 0.05f)*Random.Range(minRandom, maxRandom));
            //Fires the shot; then reloads
            }
        }

        //Sits outside of cover for the player to shoot them
        yield return new WaitForSeconds(outOfCoverTime*Random.Range(minRandom, maxRandom));

        Cover potentialCover = this.NearestCover();

        if (Vector2.Distance(potentialCover.NearestCoverPointPos(base.transform.position, this.destinationSetter.target.gameObject), this.destinationSetter.target.position) < this.weapon.useRange) {
            this.EnterCover(potentialCover);
            //Get back into cover
        }
        yield return new WaitForSeconds(recoverTime*Random.Range(minRandom, maxRandom));
        //Recover and then we can start over :)
        attackBehaviorRunning = false;
    }

    private IEnumerator ReloadOutOfCover() {
        reloadingOutOfCover = true;
        yield return new WaitForSeconds(recoverTime * 1.5f);
        shotsFiredOutOfCover = 0;
        reloadingOutOfCover = false;
    }

    private new void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(base.transform.position, this.moveRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(base.transform.position, this.weapon.useRange);

        Gizmos.color = Color.grey;
        //Remove this code when fixed
        if (this.destinationSetter != null) {
            Vector3 direction = this.destinationSetter.target.position - base.transform.position;
            Gizmos.DrawRay(base.transform.position, direction);
        }
    }

    protected override void OnDie() {
        this.player.Heal(this.healthBoostOnDeath);
    }

    protected override void OnDamage(int amount) {
    }
}
