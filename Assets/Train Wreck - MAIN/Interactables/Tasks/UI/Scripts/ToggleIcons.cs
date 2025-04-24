using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ToggleIcons : MonoBehaviour
{
    [SerializeField] private GameObject _firstIcon;
    [SerializeField] private GameObject _secondIcon;
    [SerializeField] private ToggleType _toggleType;
    [SerializeField][FoldoutGroup("ContinuouslyToggle Setting")] private float _toggleTime;
    [SerializeField][FoldoutGroup("ProximityToggle Setting")] private float _toggleRadius;
    [SerializeField][FoldoutGroup("ProximityToggle Setting")] LayerMask _detectedMask = 1 << 0;

    private Coroutine _toggleIconCoroutine;
    private void OnEnable()
    {
        switch (_toggleType)
        {
            case ToggleType.Continuously:
                ContinuouslyToggleIconcos();
                break;
            case ToggleType.Proximity:
                ProximityToggleIconcos();
                break;
            default:
                Debug.Log("No valid ToggleType");
                break;
        }
    }

    private void OnDisable()
    {
        _firstIcon.SetActive(false);
        _secondIcon.SetActive(false);
        StopCoroutine(_toggleIconCoroutine);
    }

    private void ContinuouslyToggleIconcos() 
    {
        _firstIcon.SetActive(false);
        _secondIcon.SetActive(false);
        _toggleIconCoroutine = StartCoroutine(ToggleIconsRoutine(_toggleTime));
    }
    private void ProximityToggleIconcos()
    {
        _firstIcon.SetActive(true);
        _secondIcon.SetActive(false);
        _toggleIconCoroutine = StartCoroutine(ToggleIconsRoutine(_toggleTime));
    }

    private IEnumerator ToggleIconsRoutine(float toggleTime)
    {
        switch (_toggleType)
        {
            case ToggleType.Continuously:

                while (true)
                {
                    _firstIcon.SetActive(true);
                    _secondIcon.SetActive(false);
                    yield return new WaitForSeconds(toggleTime);
                    _firstIcon.SetActive(false);
                    _secondIcon.SetActive(true);
                    yield return new WaitForSeconds(toggleTime);
                }

            case ToggleType.Proximity:

                while (true)
                {
                    int maxColliders = 4;
                    Collider[] hitColliders = new Collider[maxColliders];

                    // note: the object also has to be in interactable layer
                    int size = Physics.OverlapSphereNonAlloc(transform.parent.transform.position, _toggleRadius, hitColliders, _detectedMask);
                    if (size != 0)
                    {
                        _firstIcon.SetActive(false);
                        _secondIcon.SetActive(true);
                    }
                    else
                    {
                        _firstIcon.SetActive(true);
                        _secondIcon.SetActive(false);
                    }
                    yield return null;
                }
        }    
    }


    public enum ToggleType
    {
        Continuously,
        Proximity
    }
}
