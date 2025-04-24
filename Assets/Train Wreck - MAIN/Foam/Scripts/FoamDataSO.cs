using UnityEngine;

[CreateAssetMenu(fileName = "FoamDataSO", menuName = "Scriptable Objects/FoamData")]
public class FoamDataSO : ScriptableObject
{
    [field: SerializeField] public AudioSourcesSOData AudioData { get; private set; }
    [field: SerializeField] public float FoamSpawnForce { get; private set; }
    [field: SerializeField] public float DamageToFireAmount { get; private set; }

}
