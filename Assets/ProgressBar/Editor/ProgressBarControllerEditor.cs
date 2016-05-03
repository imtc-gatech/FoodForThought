using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProgressBarController))]
public class ProgressBarControllerEditor : Editor {

    ProgressBarController PBC;

    public void OnEnable()
    {
        PBC = (ProgressBarController)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        PBC.LeftSize = EditorGUILayout.FloatField("Left:", PBC.LeftSize);
        PBC.RightSize = EditorGUILayout.FloatField("Right:", PBC.RightSize);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        PBC.Width = EditorGUILayout.FloatField("Width:", PBC.Width);
        PBC.Height = EditorGUILayout.FloatField("Height:", PBC.Height);
        GUILayout.EndHorizontal();
        //base.OnInspectorGUI();
    }
}
