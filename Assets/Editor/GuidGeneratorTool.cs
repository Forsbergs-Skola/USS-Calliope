using UnityEditor;
using UnityEngine;

public class GuidGeneratorTool : EditorWindow
{
    private string guid = "";
    
    
    
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

        if (!string.IsNullOrEmpty(guid))
        {
            if (GUILayout.Button("Copy GUID"))
            {
                EditorGUIUtility.systemCopyBuffer = guid;
            }
        }
    }
}
