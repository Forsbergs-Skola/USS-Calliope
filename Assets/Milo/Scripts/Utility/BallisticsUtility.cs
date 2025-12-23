using UnityEngine;
using Random = UnityEngine.Random;


// Gaussian Distribution makes the bullets cluster around the aim point, like real guns.


public static class BallisticsUtility
{
    public static Vector3 GetGaussianSpread(Vector3 forward, float spreadStandardDeviation)
    {
        var u1 = Random.value;
        var u2 = Random.value;

        // Box-Muller transform to get two independent standard normals
        var offsetX = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
        var offsetY = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

        // Scale by the weapon’s spread standard deviation
        offsetX *= spreadStandardDeviation;
        offsetY *= spreadStandardDeviation;

        // The spread offsets will be applied along these axes
        var right = Vector3.Cross(Vector3.up, forward.normalized);
        var up = Vector3.Cross(forward, right).normalized;

        // Return the new direction with spread applied
        return (forward + offsetX * right + offsetY * up).normalized;
    }
}