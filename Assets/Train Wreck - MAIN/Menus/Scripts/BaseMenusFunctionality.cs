using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public abstract class BaseMenusFunctionality : MonoBehaviour
{
    [SerializeField] protected bool log = false;
    protected GameManager _gameManager;

    protected virtual void OnEnable()
    {
        _gameManager = GameManager.Instance;
    }

    public void SwitchAllActionMaps(string mapName)
    {
        foreach (PlayerInput input in _gameManager.PlayerInputs)
        {
            input.SwitchCurrentActionMap(mapName);
            if(log) Debug.Log("Script: " + this.name + ", Player: " + input.name +" ,Current ActionMap: " + input.currentActionMap.name);
        }
    }
    public void EnableAllPlayerInputs()
    {
        foreach (PlayerInput input in _gameManager.PlayerInputs)
        {
            input.ActivateInput();
            if (log) Debug.Log("Script: " + this.name + ", Player: " + input.name + " ,isActive: " + input.inputIsActive);
        }
    }
    public void DisableAllPlayerInputs()
    {
        foreach (PlayerInput input in _gameManager.PlayerInputs)
        {
            input.DeactivateInput();
            if (log) Debug.Log("Script: " + this.name + ", Player: " + input.name + " ,isActive: " + input.inputIsActive);
        }
    }
    protected void DisableOtherPlayerInputs(PlayerInput playerInput)
    {
        foreach (PlayerInput input in _gameManager.PlayerInputs)
        {
            if (input == playerInput)
                continue;

            input.DeactivateInput();

            if (log) Debug.Log("Script: " + this.name + ", Player: " + input.name + " ,isActive: " + input.inputIsActive);
        }
    }
}
