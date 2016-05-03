using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTKitchen))]
public class FFTKitchenEditor : Editor
{
    FFTKitchen Kitchen;

    public void OnEnable()
    {
        Kitchen = (FFTKitchen)target;

        if (Kitchen.StationList == null)
        {
            Kitchen.InitializeStations();
        }

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntField("Stations:", Kitchen.StationList.Count);
        if (GUILayout.Button("Initialize"))
        {
            Kitchen.InitializeStations();
        }
        EditorGUILayout.EndHorizontal();
        if (Kitchen.StationList == null || Kitchen.StationList.Count == 0)
            Kitchen.InitializeStations();
        foreach (KeyValuePair<FFTStation.Type, FFTStation> station in Kitchen.StationList)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea(station.Key.ToString() + " " + station.Value.SlotList.Count);
            if (GUILayout.Button("+"))
            {
                station.Value.AddSlot();
            }
            if (GUILayout.Button("-"))
            {
                station.Value.RemoveSlot();
            }
            EditorGUILayout.EndHorizontal();

        }

        Kitchen.UpdateKitchenName();

        //base.OnInspectorGUI();
    }

}