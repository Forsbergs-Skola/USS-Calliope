using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class BallisticsUtility
{
    public static Vector3 GetGaussianSpread(Vector3 forward, float spreadStandardDeviation)
    {
        float u1 = Random.value;
        float u2 = Random.value;
        
        float offsetX = Mathf.Sqrt(2f * Mathf.Log(u1) * Mathf.Cos(2f * Mathf.PI * u2));
        float offsetY = Mathf.Sqrt(2f * Mathf.Log(u1) * Mathf.Sin(2f * Mathf.PI * u2));
        
        offsetX *= spreadStandardDeviation;
        offsetY *= spreadStandardDeviation;

        Vector3 right = Vector3.Cross(Vector3.up, forward.normalized);
        Vector3 up = Vector3.Cross(forward, right).normalized;

        return (forward + offsetX * right + offsetY * up).normalized;
    }
}
