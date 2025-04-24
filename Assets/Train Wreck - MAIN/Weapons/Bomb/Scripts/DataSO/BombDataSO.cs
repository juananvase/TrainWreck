using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "BombDataSO", menuName = "Scriptable Objects/BombDataSO")]
public class BombDataSO : PickupableDataSO
{
    [field: Tooltip("Time (in seconds) between when the bomb lands and it explodes. The higher the value, the longer it will take for the bomb to explode"), SerializeField]
    public float ExplosionDelay { get; private set; }

    [field: Tooltip("The damage range, or the size of the SphereCast used to detect nearby objects. Increasing this value will make the explosion affect a larger area"), SerializeField]
    public float Range { get; private set; }

    [field: Tooltip("Amount of damage the bomb deals upon explosion. Increasing this value will result in more damage dealt to enemies or objects within the explosion radius"), SerializeField]
    public float DamageToBreakableObjects { get; private set; }

    [field: Tooltip("How much damage the bomb damages the Train"), SerializeField]
    public float DamageToEnemies { get; private set; }

    [field: Tooltip("How much damage the bomb damages the Train"), SerializeField]
    public float DamageToTrain { get; private set; }

    [field: Tooltip("The velocity at which the bomb travels when thrown. Increasing this value will make the bomb travel faster, potentially increasing its range and reducing the time before impact."), SerializeField]
    public float BombVelocity { get; private set; }

    [field: Tooltip("Reference to the bomb prefab used in the game"), SerializeField]
    public Bomb BombPrefab { get; private set; }

    [field: SerializeField, FoldoutGroup("VFX")] public GameObject ExplosionOnHanVFX { get; private set; }
    [field: SerializeField, FoldoutGroup("VFX")] public GameObject ExplosionOnFloorVFX { get; private set; }

    [field: Tooltip("Assign Train's Floor layer for the bomb to check if it has fallen inside the train"), SerializeField]
    public LayerMask TrainFloorLayerMask { get; private set; }
}
