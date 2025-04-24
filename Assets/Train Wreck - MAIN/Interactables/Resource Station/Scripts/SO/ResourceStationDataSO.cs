using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ReasourceStationDataSO", menuName = "Scriptable Objects/ReasourceStationData")]
public class ReasourceStationDataSO : ScriptableObject
{
    [field: SerializeField] public AudioSourcesSOData AudioData { get; private set; }
    [field: SerializeField] public Pickupable Resource { get; private set; }
    [field: SerializeField] public PickupableDataSO RequireItem { get; private set; } = null;
    [field: SerializeField] public FloatEventAsset OnInteractionInput { get; private set; }

}