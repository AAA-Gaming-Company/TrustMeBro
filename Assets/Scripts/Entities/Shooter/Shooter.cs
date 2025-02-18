using System.Collections;
using UnityEngine;

public abstract class Shooter : Entity {
    [Header("Shooter")]
    public WeaponType weapon;
    public Transform firePoint;

    public new void Start() {
        base.Start();

        WeaponType genericInstance = this.weapon;
        this.weapon = Instantiate(genericInstance);
        this.weapon.ready = true;
    }

    public void Shoot(Vector2 targetPos) {
        if (this.weapon.isSpawner) {
            int amount = this.weapon.amount[0];
            if (weapon.amount.Length > 1) {
                amount = this.weapon.amount[GameManager.GetDifficultyLevelInt()];
            }


            int damage = 0;
            if (this.weapon.isProjectile) {
                if (this.weapon.damage.Length > 1) {
                    damage = this.weapon.damage[GameManager.GetDifficultyLevelInt()];
                } else {
                    damage = this.weapon.damage[0];
                }
            }

            for (int i = 0; i < amount; i++) {
                GameObject newObject = Instantiate(this.weapon.prefab.gameObject, this.firePoint.position, Quaternion.identity);

                if (this.weapon.isProjectile) {
                    Projectile projectile = newObject.GetComponent<Projectile>();
                    projectile.Init(this.gameObject.layer, targetPos, this.weapon.useRange, this.weapon.projectileSpeed, damage);
                }
            }
        }

        StartCoroutine(this.Reload(this.weapon));
    }

    public bool isReadyToShoot() {
        return this.weapon.ready;
    }

    public float GetShootRange() {
        return this.weapon.useRange;
    }

    private IEnumerator Reload(WeaponType weapon) {
        weapon.ready = false;

        float delay = weapon.useDelay[0];
        if (weapon.useDelay.Length > 1) {
            delay = weapon.useDelay[GameManager.GetDifficultyLevelInt()];
        }

        yield return new WaitForSeconds(delay);
        weapon.ready = true;
    }
}
