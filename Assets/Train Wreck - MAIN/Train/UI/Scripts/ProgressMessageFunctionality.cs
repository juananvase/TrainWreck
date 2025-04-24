using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressMessageFunctionality : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private float _messageCleaningTime = 0.5f;

    [SerializeField] private List<Alert> _alerts = new();

    [FoldoutGroup("Events")] public UnityEvent OnShowMessage;
    [FoldoutGroup("Events")] public UnityEvent OnHideMessage;

    private void Start()
    {
        StartCoroutine(CheckSliderProgress());
    }


    private IEnumerator CheckSliderProgress()
    {
        Alert lastAlert = new("null", 2, true);
        
        while (true) 
        {
            foreach (var alert in _alerts) 
            {
                if (_slider.value >= alert.ShowInValue && _slider.value <= (alert.ShowInValue + 0.05f))
                {
                    WriteMessage(alert.Message, alert.DisappearAfterTime);
                    lastAlert = alert;
                }
            }
            _alerts.Remove(lastAlert);

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void WriteSpontaneousMessage(string message)
    {
        _message.SetText(message);
        OnShowMessage?.Invoke();

        StartCoroutine(CleanTextAfterSeconds(_messageCleaningTime));
    }
    private void WriteMessage(string message, bool cleanAfterTime)
    {
        _message.SetText(message);
        OnShowMessage?.Invoke();

        if(cleanAfterTime)
            StartCoroutine(CleanTextAfterSeconds(_messageCleaningTime));
    }

    private IEnumerator CleanTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnHideMessage.Invoke();
    }
}

[Serializable]
public struct Alert
{
    public String Message;
    public float ShowInValue;
    public bool DisappearAfterTime;

    public Alert(string message, float showInValue, bool disappearAfterTime)
    {
        this.Message = message;
        this.ShowInValue = showInValue;
        this.DisappearAfterTime = disappearAfterTime;
    }
}
