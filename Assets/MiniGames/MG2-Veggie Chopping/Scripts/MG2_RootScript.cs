using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this is the sample rootscript.  The basic idea behind it is that it stores variables that can be changed.  As of now, there is not much that affects the game, maxCuts is really it right now.

public enum MG2_GameMode {none=0, proportionality=1, cuts=2};

public class MG2_RootScript : MG_Minigame {
	
	//These are the variables that can be edited
	public MG2_GameMode Mode;//does nothing yet.  It's a Relic
	public int MG2_MaxCuts;  //maximum number of cuts that can be made
	public string Instructions;
	public string Instructions_Prefix;
	public string Instructions_Suffix;
	public string Instructions_Padding;
	public GameObject Food;
	public float Tolerance; //how much deviation from the cut it accepts.  Probably best to keep it under 10.
	
	//these are accessed in the scripts, not the editor
	[HideInInspector]
	public MG2_Score score; //score object.  contains presentation, flavor, and freshness
	[HideInInspector]
	public Vector3 camLoc; //where the camera is supposed to go.
	[HideInInspector]
	public ArrayList Cuts = new ArrayList(); //list of all the different pieces of food.
	[HideInInspector]
	public int count;  //number of cuts that has been made, do not adjust
	[HideInInspector]
	public float biggestWidth;
	public MG2_KnifeScript knifeScript;
	public Transform knifeLeftBound;
	public Transform knifeRightBound;
	private bool knifeHasBounds = false;
	[HideInInspector]
	public string cutRatio;
	private GameObject knifeDown;
	private bool alreadyDone; 
	private Camera mainMinigameCamera;
	public int CutsRemaining { get {return MG2_MaxCuts - count;} }
	
	// Use this for initialization
	
	public override void SetGameVariables (System.Collections.Generic.Dictionary<string, string> variableDict)
	{
		foreach (KeyValuePair<string, string> vPair in variableDict)
        {
            switch (vPair.Key)
            {
                case "MaxCuts":
                    MG2_MaxCuts = int.Parse(vPair.Value);
                    break;
				case "GameMode":
					MG2_GameMode mode = StringToGameMode(vPair.Value);
					Mode = mode;	
                    break;
            }
        }
	}
	
	void Start () {
		Title = "Veggie Chopping Game";
		HandFollow = true;
		camLoc = GameObject.Find(this.name + "/CameraLoc").transform.position;
		BoxCollider bc = Food.GetComponent(typeof(BoxCollider)) as BoxCollider;  //gets boxcollider of object, used to center the item on the cutting board
		GameObject foodObject = Instantiate(Food,new Vector3(transform.position.x - bc.center.x,transform.position.y-40f - bc.center.y,-1.2f), transform.rotation) as GameObject; //instantiates the item centered on the cutting board
		foodObject.transform.parent = this.transform;  //moves the food object to be a child of the root object.
		SliceScript ss = foodObject.GetComponent(typeof(SliceScript)) as SliceScript;
		ss.Root = this; //makes sure that the food object knows that this script is the root script
		alreadyDone = false;
		biggestWidth = 0;
		count = 0;
		score = gameObject.AddComponent(typeof(MG2_Score)) as MG2_Score;
		knifeDown = gameObject.GetChildByName("knife_down");
		switch(Mode){
			case MG2_GameMode.proportionality: {  //if its in porportionality mode
				Instructions = "Make " + (MG2_MaxCuts + 1).ToString() + " even pieces this wide:";
				Instructions_Prefix = "Make ";
				Instructions_Padding = "     ";
				Instructions_Suffix = " even pieces this wide:";
				break;
			}
			case MG2_GameMode.cuts: //if it's in cuts mode.  I suggest having a high MaxCuts variable in this mode
				Instructions = "Make " + MG2_MaxCuts + " cuts into the food of any size.";
				Instructions_Prefix = "Make ";
				Instructions_Padding = "     ";
				Instructions_Suffix = " cuts into the food of any size.";
				break;
		}
		mainMinigameCamera = MinigameHolder.GetComponentInChildren<Camera>();
		if (knifeLeftBound != null && knifeRightBound != null)
			knifeHasBounds = true;
		
	}
	
	#region Take and Drop Knife Functions
	
	public void TakeKnife()
	{
		knifeScript.KnifeActive = true;
		knifeDown.SetActiveRecursively(false);
		knifeScript.gameObject.SetActiveRecursively(true);
	}
	
	public void DropKnife()
	{
		knifeScript.KnifeActive = false;
		knifeScript.gameObject.SetActiveRecursively(false);
		knifeDown.SetActiveRecursively(true);
	}

	#endregion
	
	#region Handing Visuals when the last cut is made
	
	public void CuttingFinished()
	{
		DropKnife();
		GetComponentInChildren<MG_GenericScoreButton>().Highlight.SwitchHighlightOn();
		MG_BackButton backButton = GetComponentInChildren<MG_BackButton>();
		backButton.Activated = false;
	}
	
	#endregion
	
	// Update is called once per frame
	void Update () {
		
		//if(MinigameHolder.GetComponentInChildren<Camera>().transform.position == camLoc + new Vector3 (0f,10.99998f,-190f)){
		if(CurrentState == MG_Minigame.State.Active && CutsRemaining > 0)
		{
			//report(); //now done only when the knife 'cuts' the food
			if(CutsRemaining <= MG2_MaxCuts / 2) { //can score when the number of cuts taken is at least 50%
				CanBeScored = true;
				
			}
			if (knifeHasBounds)
			{
				float mouseXPos = mainMinigameCamera.ScreenToWorldPoint(Input.mousePosition).x;
				if (mouseXPos < knifeLeftBound.position.x || mouseXPos > knifeRightBound.position.x)
				{
					//outside bounds, "drop knife"
					if (knifeScript.KnifeActive == true)
					{
						//StartCoroutine(DropKnife(0));
						DropKnife();
					}
				}
				else
				{
					//inside bounds, "pick up knife"
					if (knifeScript.KnifeActive == false)
					{
						//StartCoroutine(TakeKnife(1));
						TakeKnife();
					}
				}
			}
			else //legacy check in case transform points are missing
			{
				if (mainMinigameCamera.ScreenToWorldPoint(Input.mousePosition).x < camLoc.x + 100)
				{
					if (knifeScript.KnifeActive == false)
					{
						TakeKnife();
					}
				}
				else{ //outside of the bounds, so make the knife inactive and activate the "down knife"
					if (knifeScript.KnifeActive == true)
					{
						DropKnife();
					}
				}
			}
		}
	}
	
	public void RefreshReport()
	{
		report();
	}
	
	MG2_Score report(){  //generates the score variable, and changes the feedback text.
		score.flavor = 100;
		score.freshness = 100;
		score.presentation = 100;  //set the scores at the max at the beginning of each report to prevent extra subtractions
		float testWidth = biggestWidth/(MG2_MaxCuts + 1f);
		switch(Mode){
			case MG2_GameMode.proportionality: //If the mode is set to proportionality
				for(int i = 0; i < Cuts.Count; i ++){ //iterate through all of the pieces that had been made
					GameObject sample = Cuts[i] as GameObject; //set the current piece as the one to test
					IRageSpline sampleSpline = sample.GetComponentInChildren(typeof(RageSpline)) as IRageSpline;
					float width = sampleSpline.GetBounds().width;  //get the width of the piece
					
					if(Mathf.Abs(testWidth - width) > Tolerance){ //if it's outside the acceptable bounds
						score.presentation -= Mathf.Abs(testWidth - width); //subtract the difference between the perfect width and the current piece from the score
					}
					
					//count how many were good, too big, and too small
					if((int)Cuts.Count == (MG2_MaxCuts + 1) && !alreadyDone){
						if(width > testWidth + Tolerance){
							score.overCount ++;
						}
						else if(width < testWidth - Tolerance){
							score.underCount ++;
						}
						else{
							score.closeEnough ++;
						}
					}
				}
				
				if(Cuts.Count == (MG2_MaxCuts + 1)){//report the final amount of pieces too big, too small, and just right
					//score.feedback = "Too Big:" + score.overCount + " Too Small: " + score.underCount + " Just Right: " + score.closeEnough;
					FeedbackText = score.overCount + " Big, " + score.underCount + " Small, " + score.closeEnough + " Perfect";
					alreadyDone = true;
					CanBeScored = true;
				}
				else if(Cuts.Count < (MG2_MaxCuts + 1)){
					FeedbackText = "Not Enough Cuts";
				}
		//		print(score.feedback);
				if(score.presentation < 0){
					score.presentation = 1;
				}
				break;
			case MG2_GameMode.cuts: //If the mode is set to cuts
				if (Cuts.Count == MG2_MaxCuts + 1){
					FeedbackText = "Prepared";
				}
				if(Cuts.Count < MG2_MaxCuts + 1){
					score.presentation *= (float)Cuts.Count/((float)MG2_MaxCuts + 1f);  //Gives them a score based off of how many cuts they made
					FeedbackText = "Not Enough Cuts";
				}
				break;
		}
		return score;
	}
	
	public bool getDone(){
		return alreadyDone;
	}
	
	override public void ScoreMinigame(){
		StarResult = (score.presentation/20f);
		
		/*if(StarResult > 4.5){
			FeedbackText = "Cut Evenly";
		}
		else{
			FeedbackText = "Uneven Cutting";
		}*/
	}
	
	public static MG2_GameMode StringToGameMode(string name)
	{
		switch (name)
		{
		case "cuts":
			return MG2_GameMode.cuts;
		case "proportionality":
			return MG2_GameMode.proportionality;
		}
		Debug.Log(name + " not found in game mode list. Returning 'cuts'.");
		return MG2_GameMode.cuts;
	}
	
}
