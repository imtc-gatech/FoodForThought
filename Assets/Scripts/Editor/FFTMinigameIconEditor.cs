using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof (FFTMinigameIcon))]
public class FFTMinigameIconEditor : Editor {

    FFTMinigameIcon Icon;

    public void OnEnable()
    {
        Icon = (FFTMinigameIcon)target;
        if (PrefabUtility.IsComponentAddedToPrefabInstance(Icon))
            Icon.Refresh();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        Icon.State = (MG_Minigame.Type)EditorGUILayout.EnumPopup("Display: ", Icon.State);
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}
