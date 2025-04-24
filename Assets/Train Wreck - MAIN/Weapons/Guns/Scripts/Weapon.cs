using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    [field: SerializeField, Required, InlineEditor] public BulletDataSO BulletData;

    public bool TryAttack(Vector3 aimPosition, GameObject instigator, int team, Transform target)
    {
        Attack(aimPosition, instigator, team, target);
        return true;
    }

    protected virtual void Attack(Vector3 aimPosition, GameObject instigator, int team, Transform target)
    {
        
    }

}