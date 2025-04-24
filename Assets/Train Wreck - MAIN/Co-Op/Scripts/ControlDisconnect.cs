using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ControllerDisconnect : MonoBehaviour
{
    [SerializeField] private string disconnectSceneName = "character-select-gym";

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Disconnected || change == InputDeviceChange.Removed)
            {
                Debug.Log($"Gamepad disconnected: {device.displayName}");
                SceneManager.LoadScene(disconnectSceneName);
            }
        }
    }
}
