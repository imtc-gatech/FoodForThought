using UnityEngine;
using UnityEditor; // required for Editor scripts
using System.Collections;
using System.Collections.Generic; // you need to add this using in order to use List<> and Dictionary<>

[CustomEditor(typeof(MiniGameShellExample))] // This is how the editor script knows which script to "replace" it's default Inspector 

public class MiniGameShellExampleEditor : Editor {

    MiniGameShellExample mgShell; 

    public void OnEnable()
    {
        // Every Editor has a reference to it's host script. Here we cast it to our own variable for convenience.
        mgShell = (MiniGameShellExample)target;
        mgShell.InitializeValues();
    }

    public override void OnInspectorGUI()
    {
        ShowFloatFromDict("floatVariable");
        ShowFloat(mgShell.floatVariable);

        base.OnInspectorGUI();
    }

    void ShowFloat(float value)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("floatVariable2", value);
        GUILayout.EndHorizontal();
    }

    void ShowFloatFromDict(string variableName)
    {
        if (mgShell.FloatValues.ContainsKey(variableName))
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.FloatField(variableName, mgShell.FloatValues[variableName]);
            GUILayout.EndHorizontal();
        }
        else
        {
            MissingVariableErrorDisplay(variableName, "float");
        }
    }

    void MissingVariableErrorDisplay(string variableName, string type)
    {
        GUILayout.BeginHorizontal();
        string error = "No " + type + " variable found with the name " + variableName;
        EditorGUILayout.TextField(error);
        GUILayout.EndHorizontal();
    }
}
