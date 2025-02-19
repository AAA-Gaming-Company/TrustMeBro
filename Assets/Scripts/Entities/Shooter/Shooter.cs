using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

public abstract class Shooter : Entity {
    [Header("Shooter")]
    public WeaponType weapon;
    public Transform firePoint;
    public MMF_Player shootFeedback;
    public MMF_Player reloadFeedback;

    public void Awake() {
        WeaponType genericInstance = this.weapon;
        this.weapon = Instantiate(genericInstance);
        this.weapon.ready = true;

        this.shootFeedback.Events.OnComplete.AddListener(this.ShootFinished);
    }

    public void Shoot(Vector2 targetPos) {
        if (!this.IsReadyToShoot()) {
            return;
        }

        // Make sure that the target position is at the limit of the weapon's range
        Vector2 direction = targetPos - (Vector2)this.firePoint.position;
        direction.Normalize();
        targetPos = (Vector2) this.firePoint.position + (direction * this.weapon.useRange);

        //Flip the player to the required direction
        if (targetPos.x < this.transform.position.x) {
            base.FlipEntity(true);
        } else {
            base.FlipEntity(false);
        }

        if (this.weapon.isSpawner) {
            int amount = this.weapon.GetAmount();

            int damage = 0;
            if (this.weapon.isProjectile) {
                damage = this.weapon.GetDamage();
            }

            float deviation = 0.5f * Mathf.Log(amount);

            for (int i = 0; i < amount; i++) {
                Vector2 computedTargetPosition = targetPos;
                // Add a very slight deviation to the bullet's trajectory
                if (i != 0) {
                    computedTargetPosition = new Vector2(targetPos.x + Random.Range(-deviation, deviation), targetPos.y + Random.Range(-deviation, deviation));
                }

                GameObject newObject = Instantiate(this.weapon.prefab.gameObject, this.firePoint.position, Quaternion.identity);

                if (this.weapon.isProjectile) {
                    Projectile projectile = newObject.GetComponent<Projectile>();
                    projectile.Init(this.gameObject.layer, computedTargetPosition, this.weapon.useRange, this.weapon.projectileSpeed, damage);
                }
            }
        }

        if (this.shootFeedback != null) {
            this.shootFeedback.PlayFeedbacks();
        }

        StartCoroutine(this.Reload(this.weapon));
    }

    public void ShootFinished() {
        //Flip the shooter back to the original direction

        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
        if (rb == null) {
            return;
        }

        if (rb.linearVelocity.x < 0) {
            base.FlipEntity(true);
        } else {
            base.FlipEntity(false);
        }
    }

    public bool IsReadyToShoot() {
        return this.weapon.ready;
    }

    public float GetShootRange() {
        return this.weapon.useRange;
    }

    private IEnumerator Reload(WeaponType weapon) {
        weapon.ready = false;
        yield return new WaitForSeconds(weapon.GetUseDelay());
        weapon.ready = true;

        if (this.reloadFeedback != null) {
            this.reloadFeedback.PlayFeedbacks();
        }
    }
}
