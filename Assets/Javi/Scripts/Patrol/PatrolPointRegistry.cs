using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class PatrolPointRegistry
{
    private static Dictionary<PatrolZone, List<Transform>> pointsByZone = new Dictionary<PatrolZone, List<Transform>>();
    private static bool isInitialized = false;

    public static void Initialize()
    {
        if (isInitialized) return;
        
        pointsByZone.Clear();
        PatrolPoint[] allPoints = GameObject.FindObjectsOfType<PatrolPoint>(true);

        foreach (var point in allPoints)
        {
            if (point.PatrolZone == null)
            {
                Debug.LogWarning($"PatrolPoint {point.name} has no PatrolZone assigned");
                continue;
            }

            if (!pointsByZone.ContainsKey(point.PatrolZone))
                pointsByZone[point.PatrolZone] = new List<Transform>();

            if (!pointsByZone[point.PatrolZone].Contains(point.transform))
                pointsByZone[point.PatrolZone].Add(point.transform);
        }
        isInitialized = true;
        Debug.Log($"Patrol system initialized with {allPoints.Length} points across {pointsByZone.Count} zones");
    }

    public static void RegisterPoint(PatrolPoint point)
    {
        if (!isInitialized) Initialize();
        
        if (point.PatrolZone == null)
        {
            Debug.LogWarning($"PatrolPoint {point.name} has no PatrolZone assigned");
            return;
        }

        if (!pointsByZone.ContainsKey(point.PatrolZone))
            pointsByZone[point.PatrolZone] = new List<Transform>();

        if (!pointsByZone[point.PatrolZone].Contains(point.transform))
        {
            pointsByZone[point.PatrolZone].Add(point.transform);
            Debug.Log($"Registered point {point.name} for zone {point.PatrolZone.name}");
        }
    }

    public static void UnregisterPoint(PatrolPoint point)
    {
        if (point.PatrolZone != null && pointsByZone.ContainsKey(point.PatrolZone))
            pointsByZone[point.PatrolZone].Remove(point.transform);
    }

    public static List<Transform> GetPointsForZone(PatrolZone zone)
    {
        if (zone == null) 
        {
            Debug.LogWarning($"Attempted to get points for null zone");
            return null;
        }
        
        if (!isInitialized) Initialize();
        
        if (!pointsByZone.ContainsKey(zone)) 
        {
            Debug.LogWarning($"No points registered for zone {zone.name}");
            return new List<Transform>();
        }

        return pointsByZone[zone];
    }

    public static void ClearRegistry()
    {
        pointsByZone?.Clear();
        isInitialized = false;
    }
    
    public static PatrolZone GetClosestZone(Vector3 position)
    {
        PatrolZone closestZone = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var kvp in pointsByZone)
        {
            PatrolZone zone = kvp.Key;
            List<Transform> points = kvp.Value;

            foreach (var point in points)
            {
                float distSqr = (point.position - position).sqrMagnitude;

                if (distSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distSqr;
                    closestZone = zone;
                }
            }
        }
        
        Debug.Log("[PatrolPointRegistry] " + closestZone);

        return closestZone;
    }
    
    public static List<Transform> GetClosestPoints(Vector3 position, int count)
    {
        // kvp == keyValuePair
        return pointsByZone
            .SelectMany(kvp => kvp.Value)
            .OrderBy(p => Vector3.Distance(p.position, position))
            .Take(count)
            .ToList();
    }
    
    public static Transform GetRandomPointInZone(PatrolZone zone)
    {
        if (zone == null)
            return null;

        if (!isInitialized)
            Initialize();

        if (!pointsByZone.TryGetValue(zone, out var points))
            return null;

        if (points.Count == 0)
            return null;

        int index = Random.Range(0, points.Count);
        return points[index];
    }
}