using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTCustomerView))]
public class FFTCustomerViewEditor : Editor {

    //int myInt = 32;
    float labelWidth = 150f;

    FFTCustomerView View;

    public void OnEnable()
    {
        View = (FFTCustomerView)target;

        View.Initialize();

    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Outline / Fill", GUILayout.Width(labelWidth));
        View.Outline = EditorGUILayout.ColorField(View.Outline);
        View.Fill = EditorGUILayout.ColorField(View.Fill);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset to default", GUILayout.Width(labelWidth)))
        {
            View.Outline = View.DefaultOutline;
            View.Fill = View.DefaultFill;
        }
        EditorGUILayout.ColorField(View.DefaultOutline);
        EditorGUILayout.ColorField(View.DefaultFill);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        View.State = (FFTCustomerView.VisualState)EditorGUILayout.EnumPopup("State:", View.State);
        GUILayout.EndHorizontal();
        //base.OnInspectorGUI();
    }
    /*
    public Dictionary<string, string> ReturnMe()
    {
        Dictionary<string, string> variables = new Dictionary<string, string>();
        variables.Add("myInt", myInt.ToString());
        return variables;
    }

    public void TakeMe(Dictionary<string, string> passedVariables)
    {
        myInt = int.Parse(passedVariables["myInt"]);

    }
     */
}
