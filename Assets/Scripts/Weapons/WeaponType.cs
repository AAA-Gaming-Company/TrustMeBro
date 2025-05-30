using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Scriptable Objects/Weapon Type")]
public class WeaponType : ScriptableObject {
    [SerializeField]
    private float[] useDelay = { 1 };
    public float useRange = 15;
    public bool isSingleUse = false;
    public bool isAutomatic = false;

    [Header("Display")]
    public string displayName = "Untitled Weapon";
    public Sprite displaySprite;
    public Sprite uiSprite;

    [Header("Spawner")]
    public bool isSpawner = false;
    public GameObject prefab;
    [SerializeField]
    private int[] amount = { 1 };

    [Header("Projectile")]
    public bool isProjectile = false;
    public float projectileSpeed = 0.5f;

    [Header("Grenade")]
    public bool isGrenade = false;
    public float grenadeExplosionRadius = 1;
    public float grenadeThrowForce = 0f;

    [Header("Flamethrower")]
    public bool isFlamethrower = false;
    public float flamethrowerOpeningAngle = 10;

    [Header("Damage")]
    [SerializeField]
    private int[] damage = { 10 };

    [HideInInspector]
    public bool ready;

    public float GetUseDelay() {
        if (this.useDelay.Length == 1) {
            return this.useDelay[0];
        }
        return this.useDelay[(int) GameManager.difficultyLevel];
    }

    public int GetAmount() {
        if (this.amount.Length == 1) {
            return this.amount[0];
        }
        return this.amount[(int) GameManager.difficultyLevel];
    }

    public int GetDamage() {
        if (this.damage.Length == 1) {
            return this.damage[0];
        }
        return this.damage[(int) GameManager.difficultyLevel];
    }
}
