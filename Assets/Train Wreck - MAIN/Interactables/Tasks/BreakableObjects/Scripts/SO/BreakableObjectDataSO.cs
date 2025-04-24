using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakableObjectDataSO", menuName = "Scriptable Objects/TaskData/BreakableObjectDataSO")]
public class BreakableObjectDataSO : TaskDataSO, IHealthDataAdapter
{
    [field: SerializeField, FoldoutGroup("Data")] public float Health { get; private set; } = 100;
    [field: SerializeField, FoldoutGroup("Data")] public Material FixedShaderMaterial { get; private set; }

    //Events
    [field: SerializeField, FoldoutGroup("Events")] public DamageInfoEventAsset OnDamageTrain { get; private set; }
    [field: SerializeField, FoldoutGroup("Events")] public HealingInfoEventAsset OnHealingTrain { get; private set; }
    [field: SerializeField, FoldoutGroup("Events")] public GameObjectEventAsset OnBreakObject { get; private set; }
    [field: SerializeField, FoldoutGroup("Events")] public GameObjectEventAsset OnRepairObject { get; private set; }

    //RepairData
    [field: SerializeField, FoldoutGroup("RepairData")] public float RepairAmountPerInitialInteraction { get; private set; } = 10f;
    [field: SerializeField, FoldoutGroup("RepairData")] public float TimeToFinishRepairing { get; private set; } = 9f;

    //TrainData
    [field: SerializeField, FoldoutGroup("TrainData")] public float TrainHealAmount { get; private set; }
    [field: SerializeField, FoldoutGroup("TrainData")] public float TrainDamageAmount { get; private set; }
    [field: SerializeField, FoldoutGroup("TrainData")] public float InvulnerabilityAfterRepaired { get; private set; } = 1f;
}
