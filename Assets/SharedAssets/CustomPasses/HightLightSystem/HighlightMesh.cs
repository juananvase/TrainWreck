using UnityEngine;

public class HighlightMesh : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private int _layerIndex = 12; //interactableLayer

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerStateMachine _))
        {
            ToggleHighlight(false);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        ToggleHighlight(true);
    }

    public void ToggleHighlight(bool trigger)
    {
        _renderer.renderingLayerMask = (uint)(1 << (trigger ? 0 : _layerIndex) | 1 << 0);
    }
}
