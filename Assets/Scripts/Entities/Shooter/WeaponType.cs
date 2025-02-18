using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Scriptable Objects/Weapon Type")]
public class WeaponType : ScriptableObject {
    [SerializeField]
    private float[] useDelay = { 1 };
    public float useRange = 15;

    [Header("Spawner")]
    public bool isSpawner = false;
    public GameObject prefab;
    [SerializeField]
    private int[] amount = { 1 };

    [Header("Projectile")]
    public bool isProjectile = false;
    public float projectileSpeed = 0.5f;
    [SerializeField]
    private int[] damage = { 10 };

    [HideInInspector]
    public bool ready;

    public float GetUseDelay(DifficultyLevel difficulty) {
        if (this.useDelay.Length == 1) {
            return this.useDelay[0];
        }
        return this.useDelay[(int) difficulty];
    }

    public int GetAmount(DifficultyLevel difficulty) {
        if (this.amount.Length == 1) {
            return this.amount[0];
        }
        return this.amount[(int) difficulty];
    }

    public int GetDamage(DifficultyLevel difficulty) {
        if (this.damage.Length == 1) {
            return this.damage[0];
        }
        return this.damage[(int) difficulty];
    }
}
