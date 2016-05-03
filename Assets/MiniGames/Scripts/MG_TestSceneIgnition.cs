using UnityEngine;
using System.Collections;

public class MG_TestSceneIgnition : MonoBehaviour {

    public MG_SceneController SceneController;
    public MG_MinigameHolder MinigameHolder;

	// Use this for initialization
	void Start () {
        MinigameHolder.InitializeParametersForMinigames();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
