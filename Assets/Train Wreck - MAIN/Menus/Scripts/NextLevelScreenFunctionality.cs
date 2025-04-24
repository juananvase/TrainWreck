using System;
using GameEvents;
using UnityEngine;
using UnityEngine.Events;

public class NextLevelScreenFunctionality : BaseMenusFunctionality
{
    protected override void OnEnable()
    {
        base.OnEnable();
        SwitchAllActionMaps("UI");
    }


}
