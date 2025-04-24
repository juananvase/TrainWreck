using System;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class WinLevelFunctionality : BaseMenusFunctionality
{
    [SerializeField] private BoolEventAsset _onWinLevel;

    protected override void OnEnable()
    {
        base.OnEnable();
        _onWinLevel.AddListener(OnLevelWin);
    }

    private void OnDisable()
    {
        _onWinLevel.RemoveListener(OnLevelWin);
    }

    private void OnLevelWin(bool arg0)
    {
    }
}
