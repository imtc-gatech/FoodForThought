using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TexturePicker))]
public class TexturePickerEditor : Editor {

    public void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("click"))
        {
            Debug.Log(ReturnFileNameOfPickedFile());
        }
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }

    string ReturnFileNameOfPickedFile()
    {
        string result = "";
        result = EditorUtility.OpenFilePanel("Select a level:", System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "*.pdf;*.zip");



        return result;


    }





}
