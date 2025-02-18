using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Scriptable Objects/Weapon Type")]
public class WeaponType : ScriptableObject {
    public float[] useDelay = { 1 };
    public float useRange = 15;

    [Header("Spawner")]
    public bool isSpawner = false;
    public GameObject prefab;
    public int[] amount = { 1 };

    [Header("Projectile")]
    public bool isProjectile = false;
    public float projectileSpeed = 0.5f;
    public int[] damage = { 10 };

    [HideInInspector]
    public bool ready;
}
