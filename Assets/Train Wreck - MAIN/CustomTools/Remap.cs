using UnityEngine;

namespace Remaping
{
    public class Remap : MonoBehaviour
    {
        public static Color ColorRemap(float iMin, float iMax, Color oMin, Color oMax, float value)
        {
            Color result;
            float t = Mathf.InverseLerp(iMin, iMax, value);
            result.r = Mathf.Lerp(oMin.r, oMax.r, t);
            result.g = Mathf.Lerp(oMin.g, oMax.g, t);
            result.b = Mathf.Lerp(oMin.b, oMax.b, t);
            result.a = Mathf.Lerp(oMin.a, oMax.a, t);
            return result;
        }

        public static float FloatRemap(float iMin, float iMax, float oMin, float oMax, float value)
        {
            float t = Mathf.InverseLerp(iMin, iMax, value);
            float result = Mathf.Lerp(oMin, oMax, t);
            return result;
        }

    }
}
