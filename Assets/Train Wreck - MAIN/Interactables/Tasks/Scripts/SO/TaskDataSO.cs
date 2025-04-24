using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "TaskData", menuName = "Scriptable Objects/TaskData/TaskData")]
public class TaskDataSO : ScriptableObject
{
    //Important data and SOs
    [field: SerializeField, FoldoutGroup("Data")] public AudioSourcesSOData AudioData { get; private set; }
    [field: SerializeField, FoldoutGroup("Data")] public TaskType TaskType { get; private set; }
    [field: SerializeField, FoldoutGroup("Data")] public PickupableDataSO RequireItem { get; private set; }

    [field: Tooltip("Distance in which the player can continuing to interact with a task object.")]
    [field: SerializeField, FoldoutGroup("Data")] public float ContinuedInteractionDistance { get; private set; }

    //Game Events
    [field: SerializeField, FoldoutGroup("Events")] public BoolEventAsset OnGameLost { get; private set; }
    [field: SerializeField, FoldoutGroup("Events")] public BoolEventAsset OnWinLevel { get; private set; }
    [field: SerializeField, FoldoutGroup("Events")] public FloatEventAsset OnInteractionInput { get; private set; }


}

public enum TaskType
{
    Maintenance,
    Repair,
    Defend
}