using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PickupableDataSO", menuName = "Scriptable Objects/PickupableData")]
public class PickupableDataSO : ScriptableObject
{
    [field: SerializeField, FoldoutGroup("Data")] public PickupableDataSO RequireItem { get; private set; } = null;
    [field: SerializeField, FoldoutGroup("Events")] public BoolEventAsset OnGameLost { get; private set; }

}