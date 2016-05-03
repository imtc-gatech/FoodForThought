using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTStar))]
public class FFTStarEditor : Editor {

    FFTStar Display;

    public void OnEnable()
    {
        Display = (FFTStar)target;

        if (!Display.Ready)
        {
            Display.GrabStarStates();
        }

    }

    public override void OnInspectorGUI()
    {
        if (!Display.Ready)
        {
            Display.GrabStarStates();
        }
        EditorGUILayout.BeginHorizontal();
        Display.State = (FFTStar.StarState)EditorGUILayout.EnumPopup("Current Display: ", Display.State);
        EditorGUILayout.EndHorizontal();

        //base.OnInspectorGUI();
    }
}
