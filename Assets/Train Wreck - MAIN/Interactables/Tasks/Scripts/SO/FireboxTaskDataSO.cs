using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FireboxTaskDataSO", menuName = "Scriptable Objects/TaskData/FireboxTaskDataSO")]
public class FireboxTaskDataSO : TaskDataSO
{
    [field: SerializeField, FoldoutGroup("Events")] public FloatEventAsset OnFuelLevelUpdated { get; private set; }

    [field: SerializeField, FoldoutGroup("Data")] public float FuelAdded { get; private set; } = 10f;
    [field: SerializeField, FoldoutGroup("Consumtion Data")] public float FuelConsumptionRate { get; private set; } = 1f;
    [field: SerializeField, FoldoutGroup("Consumtion Data")] public float FuelConsumptionAmount { get; private set; } = 1f;

    [field: SerializeField, FoldoutGroup("Data")] public float InitialFuelLevel { get; private set; } = 0f;
    [field: SerializeField, FoldoutGroup("Data")] public float MaxFuelLevel { get; private set; } = 100f;
    [field: SerializeField, FoldoutGroup("Data")] public float CurrentFuelLevel { get; private set; } = 0f;

    //Firebox States
    [field: SerializeField, FoldoutGroup("Firebox States")] public float LowThreshold { get; private set; }  = 20f;
    [field: SerializeField, FoldoutGroup("Firebox States")] public float MediumThreshold { get; private set; } = 40f;
    [field: SerializeField, FoldoutGroup("Firebox States")] public float OptimumThreshold { get; private set; } = 60f;
    [field: SerializeField, FoldoutGroup("Firebox States")] public float OverclockThreshold { get; private set; } = 80f;
    [field: SerializeField, FoldoutGroup("Firebox States")] public float OverloadThreshold { get; private set; } = 100f;

    //Colors
    [field: SerializeField, FoldoutGroup("Colors")] public Color NormalFireboxColor { get; private set; }
    [field: SerializeField, FoldoutGroup("Colors")] public Color heatedFireboxColor { get; private set; }


    //Update Firebox CurrentFuelLevel based on a GameEvent
    private void OnEnable()
    {
        if (OnFuelLevelUpdated == null) { Debug.Log("No GameEvent Assign"); return; }
        OnFuelLevelUpdated.OnInvoked.AddListener(UpdateFuelLevel);

    }
    private void OnDisable()
    {
        if (OnFuelLevelUpdated == null) { Debug.Log("No GameEvent Assign"); return; }
        OnFuelLevelUpdated.OnInvoked.RemoveListener(UpdateFuelLevel);
    }

    private void UpdateFuelLevel(float fluelLevel) 
    {
        CurrentFuelLevel = fluelLevel;
    }

}