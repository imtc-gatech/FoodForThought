using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTStationIcon))]
public class FFTStationIconEditor : Editor
{
    FFTStationIcon Icon;

    public void OnEnable()
    {
        Icon = (FFTStationIcon)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        Icon.Destination = (FFTStationIcon.State)EditorGUILayout.EnumPopup("Display: ", Icon.Destination);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        Icon.Position = (FFTStationIcon.IconPosition)EditorGUILayout.EnumPopup("Position: ", Icon.Position);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        Icon.Size = (FFTStationIcon.IconSize)EditorGUILayout.EnumPopup("Size: ", Icon.Size);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        Icon.Render = EditorGUILayout.Toggle("Render: ", Icon.Render);
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}
