using UnityEngine;
using Random = UnityEngine.Random;

public static class BallisticsUtility
{
    // Box-Muller transform to generate normally distributed random numbers
    public static Vector3 GetGaussianSpread(Vector3 forward, float spreadStandardDeviation)
    {
        var u1 = Random.value;
        var u2 = Random.value;

        var offsetX = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
        var offsetY = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

        offsetX *= spreadStandardDeviation;
        offsetY *= spreadStandardDeviation;

        var right = Vector3.Cross(Vector3.up, forward.normalized);
        var up = Vector3.Cross(forward, right).normalized;

        return (forward + offsetX * right + offsetY * up).normalized;
    }
}