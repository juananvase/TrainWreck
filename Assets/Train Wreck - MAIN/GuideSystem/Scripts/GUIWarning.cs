using System;
using System.Collections;
using GameEvents;
using TMPro;
using UnityEngine;

public abstract class GUIWarning : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private float _textCleaningTime = 2;
    [SerializeField] protected WarningInfoEventAsset _onTaskRequirementsDisplayed;
    [SerializeField] protected WarningType _warningType;

    protected WarningInfo warningInfo = null;
    protected bool _isDisplayingWarning = false;

    protected virtual void Start()
    {
        WriteWarningMessage(" ", false);
    }
    protected void WriteWarningMessage(string message, bool cleanMessageAfterSeconds) 
    {
        _message.SetText(message);

        if (cleanMessageAfterSeconds) 
            StartCoroutine(CleanTextAfterSeconds(_textCleaningTime));
    }
    public virtual void WarningOnInteract(Interactor interactor) 
    {
        _onTaskRequirementsDisplayed?.Invoke(warningInfo);
    }
    public virtual void WarningOnInteract(Interactor interactor, RepairState status) 
    {
        _onTaskRequirementsDisplayed?.Invoke(warningInfo); 
    }
    public virtual void WarningOnInteract(Interactor interactor, bool status, int count)
    {
        _onTaskRequirementsDisplayed?.Invoke(warningInfo);
    }

    protected IEnumerator CleanTextAfterSeconds(float seconds)
    {
        _isDisplayingWarning = true;
        yield return new WaitForSeconds(seconds);
        _message.SetText(" ");
        _isDisplayingWarning = false;
    }
}

public class WarningInfo 
{
    public WarningInfo(WarningType warningType, Color color)
    {
        WarningType = warningType;
        Color = color;
    }

    public WarningType WarningType { get; set; }
    public Color Color { get; set; }
}
public enum WarningType
{
    CoalWarning,
    FireboxWarning,
    WindowBrokenWarning, 
    PipeBrokenWarning,
    EmptyFirebox,
    FireboxDoorClosed,
    FireOnCab,
    EmptyAmmo
}
