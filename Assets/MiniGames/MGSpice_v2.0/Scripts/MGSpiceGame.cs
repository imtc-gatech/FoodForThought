using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//main game script for MGSpice, determines the score
public class MGSpiceGame : MG_Minigame {
	public enum SpiceBottle 
	{
		BoxyGreen=0, 
		GreenCylinder=1, 
		ShortBrownJar=2, 
		RedCylinder=3, 
		BlueBulge=4
	};
	//Editable
	public string[] BottleParentNames; //names of the bottle parent objects which contain the MGSpiceBottle scripts
	public SpiceBottle[] DesiredBottles; //names of all of the desired bottles
	public int AmountInBottles;
	
	//Edit at your own Risk
    public float SpiceMinY, SpiceMinX, SpiceMaxY, SpiceMaxX; //bounds for where spice can physically hit the dish
	
    public IRageSpline scoreAreaSpline; //area where the player wanted to land the spice
    
    private int score, finalScore, penaltyScore; //score of an individual bottle, score of all bottles, negative score of an individual bottle which shouldn't have been used
    private int Q1, Q2, Q3, Q4, numGoodBottles = 0, numBadBottles = 0; //Q1,Q2,Q3,Q4 are four quadrants of the scoreArea, the number of bottle which contain spice desired for the dish, the number of bottles which contain spice not desired for the dish
    private bool scored = false; //has the game been scored yet
    private Vector3 lowleft, lowright, highleft, midpoint; //corners of the scoring area
	public GameObject CameraLoc;
	public bool SpiceActive;
	
	GameObject bottleHolder; // used for relative searches of bottle objects
	public Transform spiceTransform; // used to find spiceTemplate by spiceBottle
	
	// Use this for initialization
	void Start () {
        SetupMinigame();
	}

    public override void SetupMinigame()
    {
        base.SetupMinigame();
        Debug.Log(SpiceMinY);
        Debug.Log(SpiceMinX);
        Debug.Log(SpiceMaxY);
        Debug.Log(SpiceMaxX);
        //orthographicSize = 100f/1.24f;
        Title = "Spice Rack Game";
        CanBeScored = true;
        HandFollow = true;
        CameraLoc = transform.FindChild("CameraLoc").gameObject;
        SpiceActive = false;
        calculateAreaPoints();
        //This loop sets the desired bottles in the list so that they can be scored.
		SetSpiceAmount();
		bottleHolder = transform.FindChild("bottles").gameObject;
		spiceTransform = transform.FindChild("spice");
        for (int i = 0; i < DesiredBottles.Length; i++)
        {
            string tempstring = EnumToString(DesiredBottles[i]);
            bottleHolder.transform.FindChild(tempstring).gameObject.GetComponent<MGSpiceBottle>().thisSpiceDesiredOnDish = true;  //sets the bottle to be a desired spice
            Object itemToDisplay = bottleHolder.transform.FindChild(tempstring).gameObject as Object; //sets an object to be cloned
            Transform positionSpineTransform = gameObject.transform.FindChild("needed");
			Transform backdropSpineTransform = positionSpineTransform.transform.FindChild("background");
			//IRageSpline positionSpline = positionSpineTransform.gameObject.GetComponentInChildren(typeof(RageSpline)) as IRageSpline;  //finds the circle where the icons will be placed
            //Vector3 neededPos = positionSpline.GetPositionWorldSpace(2) + new Vector3(60f / (DesiredBottles.Length + 1) + i * 15f, 0f, -2f-i);  //gets the position of the circle, then gets a point in it based off of how many bottles are desired
            Vector3 neededPos = new Vector3(0,0f,-2f-i);
			float bottlePadding = 12f;
			switch (i)
			{
			case 0:
				break;
			case 1:
				neededPos.x -= bottlePadding;
				break;
			case 2:
				neededPos.x += bottlePadding;
				break;
			}
				
			//new Vector3(60f / (DesiredBottles.Length + 1) + i * 15f, 0f, -2f-i);  //gets the position of the circle, then gets a point in it based off of how many bottles are desired
			GameObject itemDisplayed = Instantiate(itemToDisplay, neededPos, transform.rotation) as GameObject; //places the clone of the bottle so that it can become an icon

            itemDisplayed.transform.localScale = new Vector3(.5f, .5f, .5f); //shrinks the bottle icon.
            itemDisplayed.transform.parent = backdropSpineTransform;
			itemDisplayed.transform.localPosition = neededPos;
			
            Object.Destroy(itemDisplayed.GetComponent(typeof(MGSpiceBottle)));  //removes the script from the bottle icon so that it cannot be picked to be used.
            Object.Destroy(itemDisplayed.GetComponent(typeof(Collider)));
        }
        for (int i = 0; i < BottleParentNames.Length; i++) //for each bottle parent
        {
            if (bottleHolder.transform.FindChild(BottleParentNames[i]).gameObject.GetComponent<MGSpiceBottle>().thisSpiceDesiredOnDish)
            {
                numGoodBottles++; //if the bottle is desired, increment number of desired bottles
            }
            else
            {
                numBadBottles++; //else, increment number of undesired bottles
            }
        }
    }
	
    public override void SetGameVariables(Dictionary<string, string> variableDict)
    {
        foreach (KeyValuePair<string, string> vPair in variableDict)
        {
            switch (vPair.Key)
            {
                case "NumBottles":
					int numberOfBottlesToChoose = int.Parse(vPair.Value);
			        List<MGSpiceGame.SpiceBottle> Spicing_BottleList = new List<MGSpiceGame.SpiceBottle>();
					List<int> chosenNumbers = new List<int>();
					int numberOfDifferentSpiceBottleStates = 5;
					for (int i=0; i < numberOfBottlesToChoose; i++)
					{
						bool newNumber = false;
						int index = 0;
						while (!newNumber)
						{
							bool duplicateFound = false;
							index = Random.Range(0, numberOfDifferentSpiceBottleStates);
							foreach (int num in chosenNumbers)
							{
								if (num == index)
								{
									duplicateFound = true;
								}
							}
							if (!duplicateFound)
								newNumber = true;
						}
						Spicing_BottleList.Add((MGSpiceGame.SpiceBottle)index);
						chosenNumbers.Add(index);
					}	
                    DesiredBottles = Spicing_BottleList.ToArray();
                    break;
            }
        }
    }	

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case State.Dormant:
                break;
            case State.Active:
                checkActive();
                break;
            case State.Unfocused:
                break;
            case State.Done:
                break;
        }
	}

    public override void StartMinigame()
    {
        base.StartMinigame(); //sets Active
    }

    /// <summary>
    /// Calculates the score of an individual bottle
    /// </summary>
    /// <param name="bottleName">Name of the bottle to be scored.</param>
    /// <returns>The score of the bottle</returns>
    int scoreBottle(string bottleName)
    {
        GameObject bottle = bottleHolder.transform.FindChild(bottleName).gameObject; //GameObject.Find(bottleName);
        MGSpiceBottle bottleScript = bottle.GetComponent<MGSpiceBottle>();
        int numSpice = bottleScript.NumSpice;
        int numSpiceHit = 0;
            score = 0;
            foreach (Transform child in spiceTransform) //each child is a spice bit to be considered for scoring
            {
                if(child.name.Equals(bottleScript.SpoutName)){
                    numSpiceHit++;
                    if (isWithinBounds(child.position, midpoint, lowright.x, highleft.y))
                    {
                        Q1++;
                    }
                    else if (isWithinBounds(child.position, new Vector3(lowleft.x, midpoint.y, 0), midpoint.x, highleft.y))
                    {
                        Q2++;
                    }
                    else if (isWithinBounds(child.position, lowleft, midpoint.x, midpoint.y))
                    {
                        Q3++;
                    }
                    else if (isWithinBounds(child.position, new Vector3(midpoint.x, lowleft.y, 0), lowright.x, midpoint.y))
                    {
                        Q4++;
                    }
                    else
                    {
                        numSpiceHit--;
                    }
                }
            }
            int largest = Q1, smallest = Q1;
            if (Q2 < Q1) smallest = Q2; if (Q3 < smallest) smallest = Q3; if (Q4 < smallest) smallest = Q4;
            if (Q2 > Q1) largest = Q2; if (Q3 > largest) largest = Q3; if (Q4 > largest) largest = Q4;
            int largestDifference = largest - smallest;
            Q1 = 0; Q2 = 0; Q3 = 0; Q4 = 0;

            if (numSpiceHit > 0)
            {
                score = (int)((float)numSpiceHit / numSpice * 100f); //score is determined by the number of spice which landed on the spiceArea compared to the total number of spice bits which left the bottle
                score -= largestDifference; //penalty for larger differences between quadrants; it is good to equally spread between quadrants
            }
            else
            {
                score = 0;
            }
            scored = true;
            return score;
    }
    /// <summary>
    /// Calculates the position of the corners of the spiceArea
    /// </summary>
    void calculateAreaPoints()
    {
        scoreAreaSpline = transform.FindChild("scoreArea").gameObject.GetComponent(typeof(RageSpline)) as IRageSpline; // find area 1
        lowleft = scoreAreaSpline.GetPositionWorldSpace(2);  //the lower left point on the area used for boundary
        lowright = scoreAreaSpline.GetPositionWorldSpace(1); //the lower right point on the area  
        highleft = scoreAreaSpline.GetPositionWorldSpace(3); //upper left point on the area
        midpoint = new Vector3(lowright.x / 2 + lowleft.x / 2, highleft.y / 2 + lowleft.y / 2, 0);
    }

    /// <summary>
    /// Checks if a point is within the bounds of the rectangle formed by a rectangle defined by its lower left corner, its highest x value, and its highest y value
    /// </summary>
    /// <param name="point">The point to be checked</param>
    /// <param name="lowerleft">The lower left corner of the rectangle</param>
    /// <param name="rightX">highest x value</param>
    /// <param name="highY">highest y value</param>
    /// <returns></returns>
    bool isWithinBounds(Vector3 point, Vector3 lowerleft, float rightX, float highY)
    {
        float x = point.x;
        float y = point.y;
        if (x > lowerleft.x && x < rightX && y > lowerleft.y && y < highY) //if the point is in the bounds
        {
            return true;
        }
        else
        {
            return false;
        }
    }
	
	void checkActive(){
		if(CameraLoc.transform.position == MinigameHolder.GetComponentInChildren<Camera>().transform.position + new Vector3(0f, -11f, 10f)){
			SpiceActive = true;
		}
		else{
			SpiceActive = false;
		}
	}

	public void scoreGame(){
		if (!scored) {
            for (int i = 0; i < BottleParentNames.Length; i++){
                if (bottleHolder.transform.FindChild(BottleParentNames[i]).gameObject.GetComponent<MGSpiceBottle>().thisSpiceDesiredOnDish)
                {
                    finalScore += scoreBottle(BottleParentNames[i]); //add score of individual bottle to the final score
                }
                else
                {
                    penaltyScore += scoreBottle(BottleParentNames[i]); //penalty score comes from using the wrong spices on the dish
                }
            }
            finalScore = (int)(finalScore / (float)numGoodBottles); //percentage score
            if (numBadBottles > 0)
            {
                penaltyScore = (int)(penaltyScore / (float)numBadBottles); //percentage penalty
                penaltyScore = finalScore - penaltyScore;
            }
            else
            {
                penaltyScore = finalScore;
            }
            finalScore = (int)(penaltyScore + finalScore) / 2; //final score is an average of the current score with the penalty score
			if(finalScore < 90){
				FeedbackText = "Uneven spicing";
				if(penaltyScore < finalScore){
					FeedbackText += " and used wrong spice";
				}
			}
			else{
				FeedbackText = "Even spicing";
			}
			
            Debug.Log("You scored " + finalScore + "%.");
			StarResult = ((float)finalScore/20f);
		}
	}
	
	public int getFinalScore(){
		return finalScore;
	}
	
	override public void ScoreMinigame(){
		scoreGame();
		StarResult = (getFinalScore()/20f);
	}
	
	public string EnumToString(SpiceBottle s){
		switch(s){
			case SpiceBottle.BoxyGreen:
				return "spiceBottle4Parent";
			case SpiceBottle.GreenCylinder:
				return "spiceBottle3Parent";
			case SpiceBottle.ShortBrownJar:
				return "spiceBottle5Parent";
			case SpiceBottle.RedCylinder:
				return "spiceBottle1Parent";
			case SpiceBottle.BlueBulge:
				return "spiceBottle2Parent";
			default:
				return "spiceBottle2Parent";
		}
		
	}

    public static SpiceBottle VariableStringToEnum(string bottleName)
    {
        switch (bottleName)
        {
            case "BlueBulge":
                return SpiceBottle.BlueBulge;
            case "BoxyGreen":
                return SpiceBottle.BoxyGreen;
            case "GreenCylinder":
                return SpiceBottle.GreenCylinder;
            case "RedCylinder":
                return SpiceBottle.RedCylinder;
            case "ShortBrownJar":
                return SpiceBottle.ShortBrownJar;
            default:
                Debug.Log("ERROR MG_SPICE: " + bottleName + " not found in enums!");
                return SpiceBottle.BlueBulge;
        }

    }

    public override Dictionary<string, string> GetGameVariables()
    {
        var variableDict = new Dictionary<string, string>();

        int i = 1;
        foreach (SpiceBottle bottle in DesiredBottles)
        {
            string label = "Bottle" + i.ToString();
            variableDict.Add(label, bottle.ToString());
            i++;
        }

        return variableDict;
    }
	
	public void SetSpiceAmount(){
		foreach(string s in BottleParentNames){
			GameObject tempObject = gameObject.GetChildByName("bottles").GetChildByName(s);
			Debug.Log(tempObject);
			MGSpiceBottle spiceBottleTemp = tempObject.GetComponentInChildren<MGSpiceBottle>();
			spiceBottleTemp.AmountOfSpice = AmountInBottles;
		}
	}
}
