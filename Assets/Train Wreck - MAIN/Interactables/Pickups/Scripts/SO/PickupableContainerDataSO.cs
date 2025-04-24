using UnityEngine;

[CreateAssetMenu(fileName = "PickupableContainerDataSO", menuName = "Scriptable Objects/PickupableContainerDataSO")]
public class PickupableContainerDataSO : PickupableDataSO
{
    [field: SerializeField] public GameObject TargetInteractable { get; private set; } = null;
    [field: SerializeField] public GameObject VFX { get; private set; } = null;
}
