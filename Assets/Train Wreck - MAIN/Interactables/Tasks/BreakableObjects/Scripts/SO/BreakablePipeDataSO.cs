using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakablePipeDataSo", menuName = "Scriptable Objects/BreakablePipeDataSo")]
public class BreakablePipeDataSO : BreakableObjectDataSO
{
    //Object filling states
    [field: SerializeField] public GameObject BrokenFill { get; private set; }

    //Steam
    [field: SerializeField, FoldoutGroup("VFX")] public GameObject SteamVFXPrefab { get; private set; }
}
