using UnityEngine;

[CreateAssetMenu(fileName = "TrainDataSO", menuName = "Scriptable Objects/TrainData")]
public class TrainDataSO : ScriptableObject, IHealthDataAdapter
{
    [field: Header("Health")]
    [field: SerializeField] public float Health { get; set; } = 100f;
    [field: Header("Movement")]
    [field: SerializeField] public float DistanceToTravel { get; private set; } = 50f;
    [field: SerializeField] public float CurrentTrainSpeed { get; set; } = 0;
    [field: SerializeField] public uint TrainMaxSpeed { get; private set; } = 150;
    
    [field: Header("Train States Speed")]
    [field: SerializeField] public float LowStateSpeed { get; private set; } = 0.5f;
    [field: SerializeField] public float MediumStateSpeed { get; private set; } = 1f;
    [field: SerializeField] public float OptimumStateSpeed { get; private set; } = 1.5f;
    [field: SerializeField] public float OverclockStateSpeed { get; private set; } = 2f;
    public TrainStages TrainStages { get; set; }
}
