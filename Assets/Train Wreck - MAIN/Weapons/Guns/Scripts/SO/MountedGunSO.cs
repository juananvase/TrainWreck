using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "MountedGunData", menuName = "Scriptable Objects/Gun/MountedGunData")]
public class MountedGunDataSO : TaskDataSO
{
    [field: Header("Barrel Rotation")]
    [field: SerializeField] public float RotationSpeed { get; set; } = 10f;
    [field: SerializeField] public float RotationAngle { get; private set; } = 60f;
    [field: Header("Barrel Recoil")]
    [field: SerializeField] public ShakeSettings RecoilSettings { get; private set; }
}