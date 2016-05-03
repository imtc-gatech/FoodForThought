using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MG_GenericScoreButton : MonoBehaviour {
	public GameObject MinigameObject;
	public MG_Minigame Minigame;
	public MG_SceneController SceneController;
	public FFTUIButton ButtonScript;
	public SplineHighlightScript Highlight;
	public GameObject ActiveButtonUpObject;
	public GameObject ActiveButtonDownObject;
	public GameObject InactiveButtonUpObject;
	public GameObject InactiveButtonDownObject;
	public GameObject BackgroundSpline;
	
	private float score;
    private bool canMinigameBeScored = false;

	// Use this for initialization
	void Start () {
		ActiveButtonUpObject.SetActiveRecursively(false);
		ActiveButtonDownObject.SetActiveRecursively(false);
		InactiveButtonUpObject.SetActiveRecursively(false);
		InactiveButtonDownObject.SetActiveRecursively(false);
		ButtonScript = this.GetComponent<FFTUIButton>();
		MinigameObject = transform.parent.parent.gameObject; //parent of ButtonHolder is Minigame itself
		SceneController = GameObject.Find(MG_SceneController.SceneControllerName).GetComponent<MG_SceneController>();
		Minigame = MinigameObject.GetComponent<MG_Minigame>();
		ButtonScript.IsActive = Minigame.CanBeScored;
		
		if (BackgroundSpline == null)
			BackgroundSpline = transform.FindChild("Circle").gameObject;
		Highlight = BackgroundSpline.AddComponent<SplineHighlightScript>();
		Highlight.useMouseoverHighlighting = false;
		Highlight.useInverseHighlighting = true;
		Highlight.HighlightWidth = 12.75f;
		
        SwapButtonScriptVisualStates(Minigame.CanBeScored);
        if (Minigame.CanBeScored)
        {
            ActiveButtonUpObject.SetActiveRecursively(true);
        }
        else
        {
            InactiveButtonUpObject.SetActiveRecursively(true);
        }
	}
	
	void Update(){
        if (MinigameObject != null)
            UpdateButtonVisualState();
	}

    void UpdateButtonVisualState()
    {
        if (Minigame.CanBeScored != canMinigameBeScored)
        {
            SwapButtonScriptVisualStates(Minigame.CanBeScored);

            if (Minigame.CanBeScored)
            {
                ActiveButtonUpObject.SetActiveRecursively(true);
            }
            else
            {
                InactiveButtonUpObject.SetActiveRecursively(true);
            }

            canMinigameBeScored = Minigame.CanBeScored;
        }

        /*
        if (Minigame.CanBeScored != canMinigameBeScored)
        {
            ButtonScript.Up.SetActiveRecursively(false);
            if (Minigame.CanBeScored)
            {
                ButtonScript.Up = ActiveButtonUpObject;
                ButtonScript.Down = ActiveButtonDownObject;
            }
            else
            {
                ButtonScript.Up = InactiveButtonUpObject;
                ButtonScript.Down = InactiveButtonDownObject;
            }
            ButtonScript.Up.SetActiveRecursively(true);
            ButtonScript.Down.SetActiveRecursively(false);
            canMinigameBeScored = Minigame.CanBeScored;
        }
         */
    }
	
	// Update is called once per frame
	void OnMouseUp () {
		if(Minigame.CanBeScored){
			Minigame.ScoreMinigame();
			score = Minigame.StarResult;
			//INSERT SCORE CODE HERE
			
			Minigame.EndGame();
		}
	}

    void SwapButtonScriptVisualStates(bool isActive)
    {
        if (isActive)
        {
            ButtonScript.Up = ActiveButtonUpObject;
            ButtonScript.Down = ActiveButtonDownObject;
            InactiveButtonUpObject.SetActiveRecursively(false);
            InactiveButtonDownObject.SetActiveRecursively(false);
        }
        else
        {
            ButtonScript.Up = InactiveButtonUpObject;
            ButtonScript.Down = InactiveButtonDownObject;
            ActiveButtonUpObject.SetActiveRecursively(false);
            ActiveButtonDownObject.SetActiveRecursively(false);
        }
        ButtonScript.IsActive = isActive;
    }
}
