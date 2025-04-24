using System.Collections;
using System.Collections.Generic;
using GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMenu : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer border;
    [SerializeField] protected SpriteRenderer iconImage;
    [SerializeField] protected TextMeshProUGUI playerName;
    [SerializeField] protected TextMeshProUGUI readyText;
    [SerializeField] protected PlayerInfoEventAsset playerReady;
    [SerializeField] protected CharacterSelectManager manager;
    [SerializeField] protected Transform spawnLocation;
    [SerializeField] protected FillButtonBehavior fillButton;

    private CharacterSelectPlayer playerMesh;

    protected Color color;
    protected Sprite icon;
    protected PlayerInput input;

    private Vector2 _moveInput;
    private bool _wavePressed;
    private bool _jumpPressed;
    private bool _thumbsUpPressed;
    private float _horizontalInput;

    public virtual void Init(Color color, Sprite icon, string name, CharacterSelectPlayer mesh)
    {
        input = GetComponent<PlayerInput>();
        this.color = color;
        this.icon = icon;
        //_border.color = _color;
        mesh = Instantiate(mesh, spawnLocation);
        this.color.a = 1f;
        playerName.text = name;
        playerName.color = this.color;
        playerMesh = mesh;
    }

    public virtual void ReadyUp()
    {
        readyText.text = "Ready";
        playerReady.Invoke(new PlayerInfo(input.playerIndex, color, input.devices));
        manager.MarkPlayerReady(input.playerIndex);
    }

    private void OnReady(InputValue value)
    {
        fillButton.HandleButtonInput(value.Get<float>());
    }

    private void OnMoveLobby(InputValue value) 
    {
        _moveInput = value.Get<Vector2>();
        _horizontalInput = _moveInput.x;
       
    }

    private void OnWave(InputValue value) 
    {
        _wavePressed = value.isPressed;
        playerMesh.Wave(_wavePressed);
    }

    private void OnJumpCelebrate(InputValue value)
    {
        _jumpPressed = value.isPressed;
        playerMesh.Celebrate(_jumpPressed);
    }

    private void OnThumbsUp(InputValue value)
    {
        _thumbsUpPressed = value.isPressed;
        playerMesh.ThumbsUp(_thumbsUpPressed);
    }

    private void Update()
    {
        playerMesh.RotateCharacter(_horizontalInput);
    }

}
