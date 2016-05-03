using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTStarDisplay))]
public class FFTStarDisplayEditor : Editor {

    FFTStarDisplay Display;

    public void OnEnable()
    {
        Display = (FFTStarDisplay)target;
        if (Display.Stars == null)
        {
            Display.GrabStarObjects();
        }
        float count = Display.StarCount;
        Display.StarCount = 3;
        Display.StarCount = count;
    }

    public override void OnInspectorGUI()
    {
        bool swapOutline = Display.UseOutlineCount;
        EditorGUILayout.BeginHorizontal();
        Display.UseOutlineCount = EditorGUILayout.Toggle("Use outline stars:", Display.UseOutlineCount);
        EditorGUILayout.EndHorizontal();
        if (swapOutline != Display.UseOutlineCount)
            Display.Reset();
        EditorGUILayout.BeginHorizontal();
        Display.StarCount = EditorGUILayout.FloatField("Stars: ", Display.StarCount);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+"))
        {
            Display.StarCount += .5f;
        }
        if (GUILayout.Button("-"))
        {
            Display.StarCount -= .5f;
        }
        EditorGUILayout.EndHorizontal();
        if (Display.UseOutlineCount)
        {
            EditorGUILayout.BeginHorizontal();
            Display.StarOutlineCount = EditorGUILayout.FloatField("Outline Stars: ", Display.StarOutlineCount);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                Display.StarOutlineCount += .5f;
            }
            if (GUILayout.Button("-"))
            {
                Display.StarOutlineCount -= .5f;
            }
            EditorGUILayout.EndHorizontal();
        }
        
        base.OnInspectorGUI();
    }


}
