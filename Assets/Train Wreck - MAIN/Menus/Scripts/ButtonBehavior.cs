using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonBehavior : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] private bool _firstSelected = false;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color _colorOnSelected;
    [SerializeField] private Color _colorOnDeselected;
    [SerializeField] private AudioSourcesSOData _audioData;
    public UnityEvent OnButtonSelected;
    public UnityEvent OnButtonDeselected;

    private void OnEnable()
    {
        if (_firstSelected) 
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    private void OnDisable()
    {
        if (_firstSelected) 
            return;

        _text.color = _colorOnDeselected;
        OnButtonDeselected.Invoke();
    }

    private void Start()
    {
        if (_firstSelected) 
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        _text.color = _colorOnSelected;
        OnButtonSelected.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _text.color = _colorOnDeselected;
        RuntimeManager.PlayOneShot(_audioData.UIMoveSFX);
        OnButtonDeselected.Invoke();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _text.color = _colorOnSelected;
        OnButtonSelected.Invoke();
    }

    public void OnClick()
    {
        RuntimeManager.PlayOneShot(_audioData.UISelectSFX);
    }
}
