using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTDishStepDisplay))]
public class FFTDishStepDisplayEditor : Editor {

	FFTDishStepDisplay Display;

    public void OnEnable() {
		Display = (FFTDishStepDisplay)target;
	
	}
	
    public override void OnInspectorGUI()
    {
		EditorGUILayout.BeginHorizontal();
        Display.Destination = (FFTStation.Type)EditorGUILayout.EnumPopup("Destination:", Display.Destination);
        EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
        Display.DisplayText = EditorGUILayout.TextField("Text:", Display.DisplayText);
        EditorGUILayout.EndHorizontal();
	
	}
}
