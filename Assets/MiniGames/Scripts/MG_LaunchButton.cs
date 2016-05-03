using UnityEngine;
using System.Collections;

public class MG_LaunchButton : MonoBehaviour {
	
	public MG_SceneController SceneController;
	public MG_Minigame MinigameToLaunch;
	// Use this for initialization
	void Start () {
        if (SceneController == null)
        {
            SceneController = GameObject.Find(MG_SceneController.SceneControllerName).GetComponent<MG_SceneController>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown(){
		Debug.Log("button pressed");
		SceneController.SwitchGame(MinigameToLaunch, transform.position);
	}
}
