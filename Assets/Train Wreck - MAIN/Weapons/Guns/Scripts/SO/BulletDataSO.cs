using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/BulletData")]
public class BulletDataSO : ScriptableObject
{
    [field: Tooltip("Reference to the bullet prefab to spawn"), SerializeField]
    public Bullet BulletPrefab { get; private set; }
    [field: SerializeField] public int Team { get; private set; }

    [field: Tooltip("Muzzle flash VFX when guns shoots"), SerializeField]
    public MuzzleFlashVFX MuzzleFlashVFX { get; private set; }

    [field: Tooltip("Hit VFX when bullet hits something"), SerializeField]
    public HitVFX HitVFX { get; private set; }

    [field: Tooltip("MOUNTED GUN ONLY - maximum among of bullets the mounted gun has. The actual amount is set in the game manager"), SerializeField]
    public int InitialMaxBulletCount { get; set; } = 100;

    [field: Tooltip("MOUNTED GUN ONLY - the amount of bullets that are loaded every press"), SerializeField]
    public int ReloadAmount { get; set; }
    
    [field: Tooltip("MOUNTED GUN ONLY - reload time delay before the player can begin shooting again"), SerializeField]
    public float ReloadTime { get; private set; } = 1f;

    [field: Tooltip("Amount of damage that the bullet does to the GameObject it just hit"), SerializeField]
    public float Damage { get; private set; } = 20f;

    [field: Tooltip("Limit of how much distance the bullet can travel before going back into the pool"), SerializeField]
    public float Range { get; private set; } = 20f;

    [field: Tooltip("Speed in which the bullet travels"), SerializeField]
    public float BulletVelocity { get; private set; } = 500f;

    [field: Tooltip("MOUNTED GUN ONLY - MORE BULLET, LOWER NUMBER"), SerializeField]
    public float FireRate { get; private set; } = 1f;

    [field: Tooltip("FOR ENEMY ONLY - How accurate the enemies are at shooting, the higher the number, the more inaccurate they are"), SerializeField]
    public float Inaccuracy { get; private set; } = 0f;

}