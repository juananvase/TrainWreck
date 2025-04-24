using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using GameEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PauseFunctionality : BaseMenusFunctionality
{
    [SerializeField] private PlayerInputEventAsset OnPausingGame;
    [SerializeField] private PlayerInputEventAsset OnUnpausingGame;
    [SerializeField] private InputSystemUIInputModule inputSystemUI;
    [SerializeField] private InputActionAsset inputSystem;

    public UnityEvent OnPause;
    public UnityEvent OnResume;

    private PlayerInput _pausingPlayer;

    protected override void OnEnable()
    {
        base.OnEnable();
        OnPausingGame.AddListener(Pause);
        OnUnpausingGame.AddListener(HandleResumeInput);
    }
    private void OnDisable()
    {
        OnPausingGame.RemoveListener(Pause);
        OnUnpausingGame.RemoveListener(HandleResumeInput);
    }

    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void Pause(PlayerInput playerInput) 
    {
        Time.timeScale = 0f;

        _pausingPlayer = playerInput;

        DisableOtherPlayerInputs(_pausingPlayer);
        _pausingPlayer.SwitchCurrentActionMap("UI");
        inputSystemUI.actionsAsset = _pausingPlayer.actions;

        OnPause?.Invoke();
    }

    public void Resume()
    {
        if (_pausingPlayer == null)
            return;

        Time.timeScale = 1f;

        EnableAllPlayerInputs();
        _pausingPlayer.SwitchCurrentActionMap("Player");
        inputSystemUI.actionsAsset = inputSystem;
        _pausingPlayer = null;

        OnResume?.Invoke();
    }
    private void HandleResumeInput(PlayerInput playerInput)
    {
        Resume();
    }

}
