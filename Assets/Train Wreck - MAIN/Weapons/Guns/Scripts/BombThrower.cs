using DebugMenu;
using UnityEngine;

public class BombThrower : Weapon
{
    public BombDataSO BombData;
    private Vector3 _spawnPosition;
    [SerializeField]private Transform _bomberArm;
 

    protected override void Attack(Vector3 aimPosition, GameObject instigator, int team, Transform target)
    {
        base.Attack(aimPosition, instigator, team, target);

        Vector3 spawnPos = _bomberArm.position; //Spawn the bullet at the muzzle's position
        Vector3 aimDir = (aimPosition - spawnPos).normalized; //Normalize the direction from muzzle to the aim position
        _bomberArm.LookAt(target); //Make the muzzle face towards the target (adjusts aim direction)

        Bomb spawnedProjectile = PoolSystem.Instance.Get(BombData.BombPrefab, _bomberArm.position, _bomberArm.rotation) as Bomb;
        spawnedProjectile.Rigidbody.linearVelocity = spawnedProjectile.transform.forward * BombData.BombVelocity;

        Vector3 forceDirection = _bomberArm.transform.forward * BombData.BombVelocity;
        Debug.DrawLine(_bomberArm.position, _bomberArm.position + forceDirection, Color.red, 2f);
    }
}
