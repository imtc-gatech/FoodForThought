using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FFTStation))]
public class FFTStationEditor : Editor
{
    FFTStation Station;

    public void OnEnable()
    {
        Station = (FFTStation)target;

        Station.InitializeSlotHost();

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        Station.Name = (FFTStation.Type)EditorGUILayout.EnumPopup("Type:", Station.Name);
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }

}
