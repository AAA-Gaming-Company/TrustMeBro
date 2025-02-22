using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

public abstract class Shooter : Entity {
    [Header("Shooter")]
    public Transform firePoint;
    public MMF_Player projectileFeedback;
    public MMF_Player grenadeFeedback;
    public MMF_Player flamethrowerFeedback;
    public MMF_Player reloadFeedback;
    public float deviationValue = 0;

    private WeaponType currentWeapon = null;

    public void Awake() {
        if (this.projectileFeedback != null) {
            this.projectileFeedback.Events.OnComplete.AddListener(this.ShootFinished);
        }
        if (this.grenadeFeedback != null) {
            this.grenadeFeedback.Events.OnComplete.AddListener(this.ShootFinished);
        }
        if (this.flamethrowerFeedback != null) {
            this.flamethrowerFeedback.Events.OnComplete.AddListener(this.ShootFinished);
        }
    }

    public void SwitchWeapon(WeaponType newWeapon) {
        if (newWeapon == null) {
            this.currentWeapon = null;
        } else {
            this.currentWeapon = Instantiate(newWeapon);
            this.currentWeapon.ready = true;
        }
    }

    public void Shoot(Vector2 targetPos, string grenadeIgnoreTag = "") {
        if (this.currentWeapon == null || !this.IsReadyToShoot()) {
            return;
        }

        // Make sure that the target position is at the limit of the weapon's range
        Vector2 direction = targetPos - (Vector2)this.firePoint.position;
        direction.Normalize();
        direction = new Vector2(direction.x + Random.Range(-deviationValue, deviationValue), direction.y + Random.Range(-deviationValue, deviationValue));
        targetPos = (Vector2) this.firePoint.position + (direction * this.currentWeapon.useRange);

        if (this.currentWeapon.isSpawner) {
            int amount = this.currentWeapon.GetAmount();

            int damage = 0;
            if (this.currentWeapon.isProjectile || this.currentWeapon.isGrenade) {
                damage = this.currentWeapon.GetDamage();
            }

            float deviation = 0.5f * Mathf.Log(amount);

            for (int i = 0; i < amount; i++) {
                Vector2 computedTargetPosition = targetPos;
                // Add a very slight deviation to the bullet's trajectory
                if (i != 0) {
                    computedTargetPosition = new Vector2(targetPos.x + Random.Range(-deviation, deviation), targetPos.y + Random.Range(-deviation, deviation));
                }

                GameObject newObject = Instantiate(this.currentWeapon.prefab.gameObject, this.firePoint.position, Quaternion.identity);

                if (this.currentWeapon.isProjectile) {
                    Projectile projectile = newObject.GetComponent<Projectile>();
                    projectile.Init(this.gameObject.layer, computedTargetPosition, this.currentWeapon.useRange, this.currentWeapon.projectileSpeed, damage);
                } else if (this.currentWeapon.isGrenade) {
                    Grenade grenade = newObject.GetComponent<Grenade>();
                    if (this.currentWeapon.grenadeThrowForce == 0) {
                        grenade.Init(computedTargetPosition, this.currentWeapon.grenadeExplosionRadius, this.currentWeapon.useRange, damage, grenadeIgnoreTag);
                    } else {
                        grenade.Init(computedTargetPosition, this.currentWeapon.grenadeExplosionRadius, this.currentWeapon.grenadeThrowForce, damage, grenadeIgnoreTag);
                    }
                }
            }
        } else {
            if (this.currentWeapon.isFlamethrower) {
                // Create a layer mask to ignore the shooter's layer
                int layerMask = 1 << this.gameObject.layer;
                layerMask = ~layerMask;

                int damage = this.currentWeapon.GetDamage();

                Collider2D[] hits = BetterPhysics2D.OverlapConeAll(this.firePoint.position, direction, this.currentWeapon.useRange, this.currentWeapon.flamethrowerOpeningAngle, layerMask);

                foreach (Collider2D c in hits) {
                    Entity entity = c.GetComponent<Entity>();
                    if (entity != null) {
                        entity.TakeDamage(damage);
                    }
                }
            }
        }

        if (this.currentWeapon.isProjectile && this.projectileFeedback != null) {
            this.projectileFeedback.PlayFeedbacks();
        } else if (this.currentWeapon.isGrenade && this.grenadeFeedback != null) {
            this.grenadeFeedback.PlayFeedbacks();
        } else if (this.currentWeapon.isFlamethrower && this.flamethrowerFeedback != null) {
            this.flamethrowerFeedback.PlayFeedbacks();
        }

        StartCoroutine(this.Reload(this.currentWeapon));
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
        return this.currentWeapon.ready;
    }

    public float GetShootRange() {
        return this.currentWeapon.useRange;
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
