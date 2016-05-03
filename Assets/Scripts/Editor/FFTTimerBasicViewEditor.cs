using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FFTTimerBasicView))]
public class FFTTimerBasicViewEditor : Editor
{
    FFTTimerBasicView Timer;

    public void OnEnable()
    {
        Timer = (FFTTimerBasicView)target;

    }

    public override void OnInspectorGUI()
    {
        
        EditorGUILayout.BeginHorizontal();
        Timer.CurrentTime = EditorGUILayout.FloatField("Current time:", Timer.CurrentTime);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        Timer.IndicatorColor = EditorGUILayout.ColorField(Timer.IndicatorColor);
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }

}