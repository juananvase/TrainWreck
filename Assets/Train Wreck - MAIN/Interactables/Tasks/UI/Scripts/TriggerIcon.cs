using UnityEngine;
using UnityEngine.Events;

public class TriggerIcon : MonoBehaviour
{
    [SerializeField] protected GameObject _icon = null;
    [SerializeField] private PickupableDataSO _requireObject = null;
    protected virtual void OnEnable()
    {
        _icon.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        CheckDisplayButton(other);
    }

    protected virtual void CheckDisplayButton(Collider other) 
    {
        if (other.TryGetComponent(out Interactor interactor))
        {
            if (_requireObject == null)
            {
                if (interactor.ObjectInHand == null)
                {
                    _icon.SetActive(true);
                }
            }
            else
            {
                if (interactor.ObjectInHand != null && interactor.ObjectInHand.PickupableData == _requireObject)
                {
                    _icon.SetActive(true);
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        _icon.SetActive(false);
    }
}
