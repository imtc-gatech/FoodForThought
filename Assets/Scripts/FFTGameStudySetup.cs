using UnityEngine;
using System.Collections;

public class FFTGameStudySetup : MonoBehaviour {
	public int ParticipantID = 0;
	public int SessionID = 0;
	public FFTLevel.NSFStudyGroup SessionType;
	
	string participantIDInput = "";
	string sessionIDInput = "";
	string sessionTypeInput = "";
	
	bool dataArchived = false;
	string archiveLabel = "Archive Data";
	
	public int releaseFontSize = 96;
	int fontSize = 96;
			
	public GUIStyle releaseLabel;
	public GUIStyle releaseButton;
	public GUIStyle releaseTextField;
	
	bool guiStylesInitialized = false;
			
	
	void Start() {

	}
	
	void OnDestroy() {

		/*
		 	//attempt to resize gui elements dynamically. This does not work.
		 
			var horizRatio = Screen.width / 800;
			var vertRatio = Screen.height / 450;
			GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (horizRatio, vertRatio, 1));
		*/
	}
	
    void OnGUI() {

		// Immediately finish as guest for now.
		ParticipantID = 99990000 + Random.Range(0, 9999);
		
		FFTGameManager.Instance.CleanUpAndCollectStudyInput ();

		return;
		
    }
}
