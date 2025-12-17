using UnityEngine;
using Random = UnityEngine.Random;

public static class BallisticsUtility
{
    public static Vector3 GetGaussianSpread(Vector3 forward, float spreadStandardDeviation)
    {
        // two uniform random numbers (0,1]
        float u1 = Random.value;
        float u2 = Random.value;

        // Box-Muller transform to get two independent standard normals
        float offsetX = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
        float offsetY = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

        // Scale by the weapon’s spread standard deviation
        offsetX *= spreadStandardDeviation;
        offsetY *= spreadStandardDeviation;

        // Calculate two perpendicular vectors to 'forward' to form a plane
        // The spread offsets will be applied along these axes
        var right = Vector3.Cross(Vector3.up, forward.normalized);
        var up = Vector3.Cross(forward, right).normalized;

        // Return the new direction with spread applied
        return (forward + offsetX * right + offsetY * up).normalized;
    }
}