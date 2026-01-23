using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LightControlTool : EditorWindow
{
    private Dictionary<LightType, List<Light>> categorizedLights;
    private float batchIntensity = 1f;

    [MenuItem("Tools/Light Control")]
    public static void OpenWindow()
    {
        GetWindow<LightControlTool>("Light Control");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Scan Scene Lights"))
        {
            FindAndCategorizeLights();
        }

        if (categorizedLights == null)
            return;

        foreach (var p in categorizedLights)
        {
            GUILayout.Space(10);

            if (p.Key == LightType.Point)
            {
                EditorGUILayout.BeginHorizontal();
                batchIntensity = EditorGUILayout.Slider(
                    "Intensity",
                    batchIntensity,
                    0f,
                    10f
                );

                if (GUILayout.Button("Apply", GUILayout.Width(60)))
                {
                    foreach (var light in p.Value)
                    {
                        Undo.RecordObject(light, "Adjust Point Lights");
                        light.intensity = batchIntensity;
                        EditorUtility.SetDirty(light);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else 
            {
                foreach (var light in p.Value)
                {
                    DrawLightControls(light);
                }
            }
        }
    }

    private static void DrawLightControls(Light light)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.ObjectField(light, typeof(Light), true);

        Undo.RecordObject(light, "Light Adjust");

        light.enabled = EditorGUILayout.Toggle("Enabled", light.enabled);
        light.intensity = EditorGUILayout.Slider("Intensity", light.intensity, 0f, 10f);
        light.color = EditorGUILayout.ColorField("Color", light.color);

        EditorUtility.SetDirty(light);

        EditorGUILayout.EndVertical();
    }

    private void FindAndCategorizeLights()
    {
        categorizedLights = new Dictionary<LightType, List<Light>>();

        Light[] lights = Object.FindObjectsByType<Light>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var light in lights)
        {
            if (light.type == LightType.Spot)
                continue;
            if (light.type == LightType.Directional && !light.gameObject.name.Contains("1"))
                continue;

            if (!categorizedLights.TryGetValue(light.type, out var list))
            {
                list = new List<Light>();
                categorizedLights.Add(light.type, list);
            }

            list.Add(light);
        }
    }
}
