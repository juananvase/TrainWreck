using DebugMenu;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    [SerializeField] private GameObject _toggleObject;

    private void OnEnable()
    {
        DebugMenuSystem.Instance.RegisterObject(this);
    }

    private void OnDisable()
    {
        DebugMenuSystem.Instance.DeregisterObject(this);
    }

    [DebugCommand("Turn Off Tutorial")]
    public void DebugTurnOffGameObject()
    {
        if (_toggleObject != null)
        {
            _toggleObject.SetActive(false);
        }
    }
}
