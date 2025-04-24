using GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class TutoralizationScreenFuntionality : BaseMenusFunctionality
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Start()
    {
        SwitchAllActionMaps("UI");
    }
}
