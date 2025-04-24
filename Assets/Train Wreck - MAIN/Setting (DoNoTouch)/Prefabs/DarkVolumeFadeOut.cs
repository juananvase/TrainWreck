using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class DarkVolumeFadeOut : MonoBehaviour
{
    [SerializeField] Volume Volume;

    IEnumerator Start()
    {
        Volume.enabled = true;
        yield return new WaitForSeconds(0.25f);
        Volume.enabled = false;
    }
}
