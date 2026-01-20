using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GuidGeneratorTool : EditorWindow
{
    private string guid;

    private List<string> loggedIds = new();
    
    [MenuItem("Tools/GUID Generator")]
    public static void OpenWindow()
    {
        GetWindow<GuidGeneratorTool>("GUID Generator");
    }

    private void OnGUI()
    {
        GUILayout.Space(20);
        
        if (GUILayout.Button("Generate GUID"))
        {
            guid = System.Guid.NewGuid().ToString();
        }

        GUILayout.Space(10);
        
        EditorGUILayout.SelectableLabel(guid, GUILayout.Height(20));
        
        GUILayout.Space(10);

        if (string.IsNullOrEmpty(guid)) return;
        if (GUILayout.Button("Copy GUID"))
        {
            EditorGUIUtility.systemCopyBuffer = guid;
        }

        GUILayout.Space(20);

        
        if (HasBeenLogged(guid)) return;
        if (!GUILayout.Button("Log to console")) return;
        Debug.Log(guid);
        loggedIds.Add(guid);
        
    }
    
    private bool HasBeenLogged(string id)
    {
        return loggedIds.Contains(id);
    }

    private void OnDisable()
    {
        loggedIds.Clear();
    }
}
