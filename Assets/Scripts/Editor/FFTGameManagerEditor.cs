using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTGameManager))]
public class FFTGameManagerEditor : Editor
{
    private static string GM_NAME = "____GameManager____";
    private FFTGameManager GM;

    public void OnEnable()
    {
        GM = (FFTGameManager)target;
        GM.gameObject.name = GM_NAME;
        //GM.GrabCurrentLevel();
        //GM.AssignSingletonInstance();
    }

    public override void OnInspectorGUI()
    {
		FFTGameManager.BuildMode lastMode = GM.GameBuildMode;
		
		EditorGUILayout.BeginHorizontal();
        GM.GameBuildMode = (FFTGameManager.BuildMode)EditorGUILayout.EnumPopup("Configuration: ", GM.GameBuildMode);
        EditorGUILayout.EndHorizontal();
		
		if (lastMode != GM.GameBuildMode)
		{
			switch (GM.GameBuildMode)
			{
				case FFTGameManager.BuildMode.EditorMode:
					GM.AlwaysLogData = false;
					GM.LogActions = false;
					GM.UseDetailedActionLogs = false;
					GM.UseExternalLevels = false;
					GM.UseDebugLogDirectory = false;
					break;
				case FFTGameManager.BuildMode.DeveloperMode:
					GM.AlwaysLogData = true;
					GM.LogActions = true;
					GM.UseDetailedActionLogs = true;
					GM.UseExternalLevels = false;
					GM.UseDebugLogDirectory = true;
					break;
				case FFTGameManager.BuildMode.StudyMode:
					GM.AlwaysLogData = true;
					GM.LogActions = true;
					GM.UseDetailedActionLogs = true;
					GM.UseExternalLevels = true;
					GM.UseDebugLogDirectory = true;
					break;
				case FFTGameManager.BuildMode.ReleaseMode:
					GM.AlwaysLogData = true;
					GM.LogActions = true;
					GM.UseDetailedActionLogs = false; //we don't want to log participant data for "release"
					GM.UseExternalLevels = true;
					GM.UseDebugLogDirectory = true;
					break;
			}
		}
		
		/*
		if (GM.GameBuildMode == FFTGameManager.BuildMode.EditorMode)
		{
			if (GM.CurrentLevel == null)
				GM.CurrentLevel = GM.gameObject.GetComponent<FFTLevel>();
			EditorGUILayout.BeginHorizontal();
			GM.CurrentLevel.LevelSource = EditorGUILayout.ObjectField("XML Level Source: ", GM.CurrentLevel.LevelSource, typeof(TextAsset), false) as TextAsset;
	        EditorGUILayout.EndHorizontal();
		}
		*/
		
        EditorGUILayout.BeginHorizontal();
        GM.AlwaysLogData = EditorGUILayout.Toggle("Always Log Data:", GM.AlwaysLogData);
        EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        GM.LogActions = EditorGUILayout.Toggle("Use Level Progress:", GM.LogActions);
        EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        GM.UseDetailedActionLogs = EditorGUILayout.Toggle("Log detailed data:", GM.UseDetailedActionLogs);
        EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        GM.UseExternalLevels = EditorGUILayout.Toggle("Use External Levels (LevelData):", GM.UseExternalLevels);
        EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        GM.UseDebugLogDirectory = EditorGUILayout.Toggle("Debug Log To Desktop:", GM.UseDebugLogDirectory);
        EditorGUILayout.EndHorizontal();
        /*
         * //Camera Position Selector (disabled)
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EnumPopup("Camera Position: ", GM.CameraPosition);
        EditorGUILayout.EndHorizontal();
         */
        EditorGUILayout.BeginHorizontal();
        GM.State = (FFTGameManager.GameState)EditorGUILayout.EnumPopup("Game State: ", GM.State);
        EditorGUILayout.EndHorizontal();
        
        /*
         * //Load Level Object Button (disabled)
        EditorGUILayout.BeginHorizontal();
        string loadString = "";
        if (GM.CurrentLevel != null)
        {
            if (GM.CurrentLevel.LevelSource != null)
            {
                string levelName = GM.CurrentLevel.LevelSource.name;
                loadString = "Load " + levelName.Substring(0, levelName.IndexOf("_"));
            }
            else
            {
                loadString = "Set Level Source in FFTLevel before to Load Level.";
            }
        }
        else
        {
            loadString = "Click to add/assign FFTLevel object.";
        }
        
        //GM.LoadLevel = GUILayout.Button(loadString);
        GUILayout.Button("(disabled)");
        EditorGUILayout.EndHorizontal();
         */

        if (GM.CurrentLevel != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GM.CurrentLevel.ParticipantID > 0)
                EditorGUILayout.TextField("Logging ENABLED for participant ID" + GM.CurrentLevel.ParticipantID.ToString());
            else
                EditorGUILayout.TextField("Logging DISABLED. Change participant ID to enable.");
            EditorGUILayout.EndHorizontal();
        }
        /*
        GUILayout.BeginHorizontal();
        GM.LevelPath = EditorGUILayout.ObjectField("Load Level from XML: ", GM.LevelPath, typeof(TextAsset), false) as TextAsset;
        if (GUILayout.Button("Load", GUILayout.Width(60)))
            GM.LoadXMLLevel(GM.LevelPath);
        GUILayout.EndHorizontal();
         */

        //FFTCameraManager.MoveCamera(GM.CameraPosition);



        //base.OnInspectorGUI();
    }

}
