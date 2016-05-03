using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// MG blend_ game script. This script controls a lot of the main functions of the game, like updating the score, 
/// updating the statuses, choosing and setting the ingredients.
/// </summary>
public class MGBlend_GameScript : MG_Minigame {
	//public tweakable variables migrated from IngredientPicker
	public bool RandomPlay = false; //if selected, the game will randomly generate ingredients. Default softness values are used in random play.
	public bool UseDefaultSoftness = false; //check this if you don't want to toggle softness yourself. 
	public bool ClickTrueToRestart = false; //check this box if you want to pick some new fruits or restart the game
	public int NumOfIngredients = 0; //when random, keep between 2&3, else keep between 0&3
	
	public bool randomizeNumberOfIngredients = true; //override this for user-set numbers of ingredients
	
	private MGBlend_IngredientPicker IPS; //reference to ingredient picker script
	private MGBlend_ButtonScript BS; //reference to button script
		
	private IRageSpline progressSpline; //the indicator for each status bar
	private IRageSpline triangleSpline; // the triangle part of each indicator for the status bar
	private Vector3 progressBottomLine1; //the vector location to control the bottom right spline point on the progress indicator 
	private Vector3 progressBottomLine2; //the vector location to control the bottom left spline point on the progress indicator
		
	private int currIndex = 0; //counter that allows us to iterate through the scoring array in an ordered manner
	
	public bool ResetSpeed = false; //user chooses if he wants to restore defaults on blend speed
	public float BlendSpeed; //blend speed controls how fast a fruit is done being blended
	private float defaultblendSpeed = 1f; //must be between .5f and 1.5f;
	
	public MGBlend_Score[] ScoringArray; //array of scores for each fruit
	private int deductions = 0; //used for scoring; keeps track of how many StarResult should be deducted
	public double TotalScore;
	
	public Color myRed; //our created shade of red
	public Color myGreen; //our created shade of green
	
	private float blendBarMaxY; //the max location for blend goals
	
	public Vector3 CameraLoc;
	public bool BlendActive = false;
	
	private int overBlendedCount; //how many fruits were overblended
	private int underBlendedCount; //how many fruits were underblended
	private int almostCount; //how many fruits were in the green
	private Camera mainMinigameCamera;
	
	/// <summary>
	/// Awake this instance. Used for initialization. 
	/// </summary>
	void Awake () {
		Title = "Blending Game";
		HandFollow = true;
		CanBeScored = true;
		BS = GameObject.Find (this.name + "/ButtonCollider").GetComponent<MGBlend_ButtonScript>();
		IPS = GameObject.Find (this.name).GetComponent<MGBlend_IngredientPicker>();
		CameraLoc = GameObject.Find(gameObject.name + "/CameraLoc").transform.position;
		blendBarMaxY = CameraLoc.y + 82.24986f;
		BlendSpeed = defaultblendSpeed;
	}
	
	void Start () {
		mainMinigameCamera = MinigameHolder.GetComponentInChildren<Camera>();
	}
	
	public override void SetGameVariables(Dictionary<string, string> variableDict)
    {
        foreach (KeyValuePair<string, string> vPair in variableDict)
        {
            switch (vPair.Key)
            {
                case "NumIngredients":
                    NumOfIngredients = int.Parse(vPair.Value);
					randomizeNumberOfIngredients = false;
                    break;
            }
        }
    }
	
	/// <summary>
	/// Update this instance. This runs once per frame.
	/// </summary>
	void FixedUpdate () {
		if (CurrentState == MG_Minigame.State.Active)
		{
			currIndex = 0;
			if(CameraLoc == (mainMinigameCamera.transform.position + new Vector3(0f,-11f,10f))){
				BlendActive = true;
			}
			else{
				BlendActive = false;	
			}
			//resets the blend speed if the user has changed it.
			updateBlendSpeed();
			
			//if blending, this updates the status bar and updates the score
			if(BS.Blending){
				foreach(GameObject fruitKey in IPS.fruitsAndStatuses.Keys){
					MGBlend_FruitControllerScript targetFruit = fruitKey.GetComponent<MGBlend_FruitControllerScript>();
					if(!targetFruit.Clickable && targetFruit.Increasable && targetFruit.InLiquid){
						GameObject blendAmtIndicator = (GameObject)IPS.fruitsAndStatuses[fruitKey];
						GameObject goalAmtIndicator = (GameObject)IPS.fruitsAndGoals[fruitKey];
						IRageSpline greenThresh = goalAmtIndicator.transform.GetChild(0).GetComponent(typeof(RageSpline)) as IRageSpline;
						IRageSpline whiteThresh = goalAmtIndicator.transform.GetChild(1).GetComponent(typeof(RageSpline)) as IRageSpline;
						
						foreach(Transform child in blendAmtIndicator.gameObject.transform){
							IRageSpline tempSpline = child.GetComponent(typeof(RageSpline)) as IRageSpline;
							
							if(tempSpline.GetPointCount() == 3){
								triangleSpline = tempSpline;
							}
							
							if(tempSpline.GetPointCount() == 4){
								progressSpline = tempSpline;
								progressBottomLine1 = progressSpline.GetPositionWorldSpace(1);
								progressBottomLine2 = progressSpline.GetPositionWorldSpace (2);
							}
						}
						
						Vector3 origProgressLocation = blendAmtIndicator.transform.position;
						
						//THIS is where the blend for each fruit is updated
						blendAmtIndicator.transform.position = new Vector3(origProgressLocation.x, origProgressLocation.y + BlendSpeed*(Time.deltaTime*FFTTimeManager.Instance.GameplayTimeScale*30), origProgressLocation.z); //+0.1f
						//TODO: make this more semantic / refactor
						
						progressSpline.SetPointWorldSpace(1, progressBottomLine1);
						progressSpline.SetPointWorldSpace(2, progressBottomLine2);
						
						Vector3 compareProgress = progressSpline.GetPositionWorldSpace (0);
						Vector3 greenEnter = greenThresh.GetPositionWorldSpace(1);
						Vector3 greenExit = greenThresh.GetPositionWorldSpace(0);
						Vector3 whiteEnter = whiteThresh.GetPositionWorldSpace(1);
						Vector3 whiteExit = whiteThresh.GetPositionWorldSpace(0);
						
						if((compareProgress.y >= whiteEnter.y) && (compareProgress.y <= whiteExit.y)){
							triangleSpline.SetFillColor1 (Color.white);
							progressSpline.SetFillColor1 (Color.white);
						}
						else if((compareProgress.y >= greenEnter.y) && (compareProgress.y <= greenExit.y)){
							triangleSpline.SetFillColor1(myGreen);
							progressSpline.SetFillColor1(myGreen);
						}
						else{
							triangleSpline.SetFillColor1(myRed);
							progressSpline.SetFillColor1(myRed);
						}
						
						//changes the stuff inside the status holder to be the right color
						if(compareProgress.y > greenExit.y){
							greenThresh.SetFillColor1(myRed); 
							greenThresh.SetPointWorldSpace(1, new Vector3(progressBottomLine1.x, progressBottomLine1.y, greenEnter.z)); 
							greenThresh.SetPointWorldSpace(2, new Vector3(progressBottomLine2.x, progressBottomLine2.y, greenEnter.z)); 
							ScoringArray[currIndex].exceededGoal = true;
						}
						
						//stops the progress from increasing once the status bar gets full
						if(compareProgress.y >= blendBarMaxY){
							triangleSpline.SetFillColor1(myRed);
							progressSpline.SetFillColor1(myRed);
							fruitKey.GetComponent<MGBlend_FruitControllerScript>().Increasable = false;
						}
						
						progressSpline.RefreshMesh();
						triangleSpline.RefreshMesh();
						greenThresh.RefreshMesh(); 
						
						float whole = ((whiteEnter.y + whiteExit.y)/2) + 27f;
						float part = compareProgress.y + 27f;
						float percentage = (part/whole)*100;					
						
						ScoringArray[currIndex].barColor = triangleSpline.GetFillColor1();
						ScoringArray[currIndex].percentBlended = percentage;
						ScoringArray[currIndex].attempted = true;
						if(whiteEnter.y >= 40)
							ScoringArray[currIndex].toughness = 1;
						else
							ScoringArray[currIndex].toughness = 0;
						currIndex++;
					}
					else{
						//liquid whirls
					}
				}
			}
			//if spray, deduct some points
			if(BS.Spray){
				deductions = BS.MistakeCount;
			}
		}
	}
	
	/// <summary>
	/// calculates and returns a score to the user
	/// </summary>
	public double score(){
		TotalScore = 0;
		int ingredNum = IPS.NumOfIngredients;
			
		for(int ind = 0; ind < ingredNum; ind++){
			TotalScore = TotalScore + (calculateScore(ScoringArray[ind].barColor, ScoringArray[ind].toughness, 
				ScoringArray[ind].percentBlended, ScoringArray[ind].attempted));
		}
			
		if(ingredNum == 1)
			TotalScore = TotalScore/1;
		else if(ingredNum == 2)
			TotalScore = TotalScore/2;
		else
			TotalScore = TotalScore/3;
		TotalScore = TotalScore-(deductions*.25);
		
		FeedbackText = underBlendedCount + " Underblended. " + overBlendedCount + " Overblended. " +almostCount + " Almost.";
		return TotalScore;
	}
	
	/// <summary>
	/// Updates the blend speed. max is 1.5f, min is .5f;
	/// </summary>
	void updateBlendSpeed(){
		if(ResetSpeed){
			BlendSpeed = defaultblendSpeed;
			ResetSpeed = false;
		}
		
		if(BlendSpeed != defaultblendSpeed){
			if(BlendSpeed < .5f)
				BlendSpeed = .5f;
			if(BlendSpeed > 1.5f)
				BlendSpeed = 1.5f;
		}
	}
	
	/// <summary>
	/// Calculates the score.
	/// </summary>
	/// <returns>
	/// The score on a scale of 1 - 5
	/// </returns>
	/// <param name='c'>
	/// C. color of progress indicator - will tell us if user scored perfect or good or bad
	/// </param>
	/// <param name='tough'>
	/// Tough. tough fruits are given more leeway on overblending; soft fruits given more leeway onunderblending
	/// </param>
	/// <param name='percentage'>
	/// Percentage. percentage blended
	/// </param>
	/// <param name='attempted'>
	/// Attempted. user at least attemptedto blend the fruit.
	/// </param>
	int calculateScore(Color c, int tough, float percentage, bool attempted){
		int returnValue;
		
		if(c.Equals(Color.white))
			returnValue = 5;
		else if(c.Equals(myGreen)){
			returnValue = 4;
			almostCount ++;
		}
		else if(!attempted)
			returnValue = 1;
		else{
			if(tough == 1){
				if((percentage >= 80) && (percentage <= 120))
					returnValue = 3;
				else if((percentage >= 50) && (percentage <= 140))
					returnValue = 2;
				else
					returnValue = 1;
				if(percentage > 100){
					overBlendedCount ++;
				}
				if(percentage < 100){
					underBlendedCount ++;
				}
			}
			else{
				if((percentage >= 50) && (percentage <= 120))
					returnValue = 3;
				else if((percentage >= 20) && (percentage <= 140))
					returnValue = 2;
				else
					returnValue = 1;
				if(percentage > 100){
					overBlendedCount ++;
				}
				if(percentage < 100){
					underBlendedCount ++;
				}
			}
		}
		
		return returnValue;
	}
	
	override public void ScoreMinigame(){
		StarResult = (int)score();
	}
}