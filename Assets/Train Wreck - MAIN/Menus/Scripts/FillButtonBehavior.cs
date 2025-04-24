using System;
using System.Collections;
using FMODUnity;
using GameEvents;
using UIComponents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FillButtonBehavior : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private FloatEventAsset OnPressedSelectKeyValue;
    [SerializeField] private AudioSourcesSOData _audioData;
    public UnityEvent OnFillComplete;

    private bool _isButtonPressed;
    private bool _isFillCompletedOnce = false;
    private IEnumerator _fillingCoroutine;

    private void OnEnable()
    {
        _fill.fillAmount = 0;
        _isFillCompletedOnce = false;
        OnPressedSelectKeyValue.AddListener(HandleButtonInput);
    }

    private void OnDisable()
    {
        _isFillCompletedOnce = false;
        OnPressedSelectKeyValue.RemoveListener(HandleButtonInput);
    }


    public void HandleButtonInput(float value)
    {
        _fillingCoroutine = FillingWhilePressingDown();
        _isButtonPressed = (value > 0f);
        StartCoroutine(_fillingCoroutine);
    }
    private IEnumerator FillingWhilePressingDown()
    {
        while (_isButtonPressed)
        {
            _fill.fillAmount += 0.01f;

            if (_fill.fillAmount == 1 && _isFillCompletedOnce == false)
            {
                _isFillCompletedOnce = true;
                OnFillComplete?.Invoke();
                RuntimeManager.PlayOneShot(_audioData.UISelectSFX);
                yield return null;
            }

            yield return null;
        }
        _fill.fillAmount = 0f;
    }
}
