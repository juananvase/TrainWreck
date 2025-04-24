using System.Collections.Generic;
using UnityEngine;

public class Crack : MonoBehaviour
{
    public int Length;
    public int BlendShapeCount;
    public List<Transform> CornerPoints;
    [SerializeField] SkinnedMeshRenderer _Crack;

    public void SetBlendShape(int index, float value)
    {
        _Crack.SetBlendShapeWeight(index, value);
    }

    public bool IsFullyOpen()
    {
        return GetBlendShape(BlendShapeCount - 1) >= 100;
    }

    public float GetBlendShape(int index)
    {
        return _Crack.GetBlendShapeWeight(index);
    }
}
