using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Gun : Weapon
{
    // Serialized fields, grouped under "Ranged" category in the Unity Editor
    [field: SerializeField, BoxGroup("Ranged")] private Transform _muzzle;
    [SerializeField] private Transform _muzzleFlashSpawnPoint;
    [Header("Audio")]
    [field: SerializeField, InlineEditor] public AudioSourcesSOData AudioData { get; private set; }

    void Start()
    {
        //If no muzzle is assigned, try to get the transform of the object this script is attached to
        if (_muzzle == null) GetComponent<Transform>();
    }

    //Attack method which is used to perform the weapon's attack
    protected override void Attack(Vector3 aimPosition, GameObject instigator, int team, Transform target)
    {
        base.Attack(aimPosition, instigator, team, target); //Call the base class's Attack method

        Debug.DrawLine(transform.position, aimPosition, Color.red, 1f);

        //Calculate the spawn position and aim direction for the bullet
        Vector3 spawnPos = _muzzle.position; //Spawn the bullet at the muzzle's position
        Vector3 aimDir = (aimPosition - spawnPos).normalized; //Normalize the direction from muzzle to the aim position
        _muzzle.LookAt(target); //Make the muzzle face towards the target (adjusts aim direction)

        //Generate inaccuracy in both X and Y directions to simulate randomness
        float inaccX = Random.Range(-BulletData.Inaccuracy, BulletData.Inaccuracy);
        float inaccY = Random.Range(-BulletData.Inaccuracy, BulletData.Inaccuracy);

        //Create vectors to simulate the inaccuracy in rotation (left-right and up-down)
        Vector3 leftRightAngle = _muzzle.up * inaccX;
        Vector3 upDownAngle = _muzzle.right * inaccY;
        //Combine the inaccuracy angles into one rotation
        Quaternion inaccRotation = Quaternion.Euler(leftRightAngle + upDownAngle);

        //Apply the inaccuracy rotation to the muzzle's original rotation
        Quaternion finalRotation = _muzzle.rotation * inaccRotation;

        //Spawn the bullet from the object pool (using a pool system to manage bullet objects)
        Bullet spawnedProjectile = PoolSystem.Instance.Get(BulletData.BulletPrefab, _muzzle.position, _muzzle.rotation) as Bullet;
        spawnedProjectile.ShootBullet(_muzzle.position);
        PlayMuzzleFlashVFX();
        PlaySFX();

    }

    private void PlayMuzzleFlashVFX()
    {
        PoolSystem.Instance.Get(BulletData.MuzzleFlashVFX, _muzzleFlashSpawnPoint.position, _muzzleFlashSpawnPoint.rotation, _muzzleFlashSpawnPoint);
    }

    private void PlaySFX()
    {
        if (!AudioData.EnemyGunSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(AudioData.EnemyGunSFX, transform.position);
        }
    }
}