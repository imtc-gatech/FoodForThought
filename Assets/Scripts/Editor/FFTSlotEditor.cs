using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTSlot))]
public class FFTSlotEditor : Editor
{
    FFTSlot Slot;

    public void OnEnable()
    {
        Slot = (FFTSlot)target;
    }

    public void OnDisable()
    {
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField( "Current Dish:", Slot.Dish, typeof(FFTSlot), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        Slot.Render = EditorGUILayout.Toggle("Render:", Slot.Render);
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
	
}
