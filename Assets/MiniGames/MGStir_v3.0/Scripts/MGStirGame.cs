using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main game for stirring ingredients/vegetables in a pan.
/// </summary>
public class MGStirGame : MG_Minigame
{

    public Material[] Materials_For_Each_Ingredient_in_the_Pan; //Unity editor array for the materials of vegetables desired to be in the pan
    private Material[] materials; //private variable for materials array
	
	public int Number_Of_Vegetables = 20; //the number of vegetables to attempt to place in the pan on startup
	
	public float TimeBeforeVegetableStartsBurning = .4f; 
	//how long before the vegetable takes a step towards burnt
	public float TimeBeforeVegetableBurnsInSeconds = 10f;
	public float TimeBeforeVegetableIsFullyBurntInSeconds = 5f;
	
	public float StirAlleviationRate = 1.0f; //how much each collision by the spoon will alleviate the burns.  
	public float GameDuration = 30; //how long the game will take in seconds (with the timer).  Do not exceed 60 seconds
	
	public Transform toolGrabPoint;
	public MGStirStirringTool[] stirTools;
	public MGStirSpoonHandleCollider spoonCollider;
	public bool SpoonHasBounds = false;
	
	public Camera mainMinigameCamera;
	
	
    //public int[] Heights_For_Each_Ingredient, Widths_For_Each_Ingredient; //Unity editor arrays for height and width of vegetables
    //private int[] heights, widths; //private variables for height and width arrays
    
	
    private int z = 50, numVegetablesPlaced = 0, numVegetablesRequested, score; //z location of vegetables, the number of vegetables successfully place in the pan at startup, private variable for Number_Of_Vegetables, score of the game
    private float radius; //radius of pan, rotation speed of timer
	
    public IRageSpline PanSpline, ProgressSpline; //RageSpline of the pan, the progress of the timer
    private GameObject panObject, timer; //GameObject of the pan, GameObject of the pan, GameObject of the timer
    private Vector3 center; //center of pan
    private Transform vegetables; //transform of the parent object of the vegetables

    public bool VegetablesCooked = false, game = true; //referenced by MGStirVegetable script which is attached to individual vegetable objects
    private bool donePlacingVegetables = false;
    Vector3 camLoc;
	bool started = false;
	private float timerStartPos; //used to determine (from 0 to 360) where the timer's arrow starts;
	// Use this for initialization
	
	
	void Start () 
    {
        SetupMinigame();
		
	}

    public override void SetupMinigame()
    {
        base.SetupMinigame();
        Title = "Cooking Game";
        CanBeScored = false;
        HandFollow = true;
        game = false;
        camLoc = GameObject.Find(this.name + "/CameraLoc").transform.position;
        Debug.Log("Let the vegetables cook until the timer runs out, but be sure to stir them to stop them from burning!");
        //instantiate variables and connect public-private variables
        materials = Materials_For_Each_Ingredient_in_the_Pan;
        // heights = Heights_For_Each_Ingredient;
        // widths = Widths_For_Each_Ingredient;
        numVegetablesRequested = Number_Of_Vegetables;
        vegetables = transform.FindChild("vegetables"); //GameObject.Find("vegetables").transform;
        timer = GameObject.Find(this.name + "/eggTimer/timerArrow");
        ProgressSpline = timer.GetComponent(typeof(RageSpline)) as IRageSpline;
		timerStartPos = 360-(GameDuration*6); 
		timer.transform.Rotate(0f,0f,timerStartPos);
        calculatePan(); //find position/dimensions of the pan object
		
		mainMinigameCamera = MinigameHolder.GetComponentInChildren<Camera>();
		
		stirTools = transform.GetComponentsInChildren<MGStirStirringTool>();
		spoonCollider = transform.GetComponentInChildren<MGStirSpoonHandleCollider>();
		if (toolGrabPoint == null || stirTools == null || spoonCollider == null)
			SpoonHasBounds = false;
		else
			SpoonHasBounds = true;
    }

    public override void StartMinigame()
    {
        //if(MinigameHolder.GetComponentInChildren<Camera>().transform.position == camLoc + new Vector3(0f,11f,-10f)){

        started = true;
        game = true;
        initializeIngredients(); //place vegetables in the pan
        CurrentState = State.Active;
    }
	
	#region Variable Setup
	
	public override void SetGameVariables(Dictionary<string, string> variableDict)
    {
        foreach (KeyValuePair<string, string> vPair in variableDict)
        {
            switch (vPair.Key)
            {
                case "NumVegetables":
                    Number_Of_Vegetables = int.Parse(vPair.Value);
                    break;
				case "TotalTimeToCook":
                    GameDuration = float.Parse(vPair.Value);
                    break;
				case "TimeBeforeStir":
                    TimeBeforeVegetableStartsBurning = float.Parse(vPair.Value);
                    break;
				case "TimeBeforeDanger":
                    TimeBeforeVegetableBurnsInSeconds = float.Parse(vPair.Value);
                    break;
				case "TimeBeforeBurn":
                    TimeBeforeVegetableIsFullyBurntInSeconds = float.Parse(vPair.Value);
                    break;
				case "BurnRecoveryRate":
                    StirAlleviationRate = float.Parse(vPair.Value);
                    break;
            }
        }
    }
	
	#endregion
	
	
	// Update is called once per frame
	void Update () 
    {
        switch (CurrentState)
        {
            case State.Dormant:
                break;
            case State.Active:
                UpdateActiveState();
                break;
            case State.Unfocused:
				UpdateUnfocusedState();
                break;
            case State.Done:
                break;
        }
	}

    void UpdateActiveState() // deprecates if (game)
    {
        //movePanBack(); //reveals the placed vegetables by moving the pan behind them, after delay
        if (SpoonHasBounds)
		{
			float mouseXPos = mainMinigameCamera.ScreenToWorldPoint(Input.mousePosition).x;
			if (mouseXPos < toolGrabPoint.position.x) {
				//tool.GrabTool();
				spoonCollider.GrabSpoon();
			}
			else {//if (mouseXPos < toolGrabPoint.position.x) {
				//tool.ReleaseTool();
				spoonCollider.ReleaseSpoon();
			}
//			foreach (MGStirStirringTool tool in stirTools)
//			{
//				
//			}
			
				
			
		}
		rotateTimer();
    }
	
	void UpdateUnfocusedState()
	{
		rotateTimer(); //the timer also rotates when the camera is not on the game
	}

    /// <summary>
    /// Rotates the timer object. The dish is finished cooking when the timer finishes rotating
    /// </summary>
    void rotateTimer()
    {
        if (!VegetablesCooked)
        {
			
            timer.transform.Rotate(0, 0, (Time.deltaTime*FFTTimeManager.Instance.GameplayTimeScale)*6);
            if (timer.transform.eulerAngles.z > 359)
            {
				/*
                ProgressSpline.SetFillColor1(new Color(0f, 1f, 30 / 255f, 255f));
                ProgressSpline.RefreshMesh(false, false, false);
                */
                VegetablesCooked = true;
				CanBeScored = true;
				
				GetComponentInChildren<MG_GenericScoreButton>().Highlight.SwitchHighlightOn();
				MG_BackButton backButton = GetComponentInChildren<MG_BackButton>();
				backButton.Activated = false;
            }
        }
    }

    /// <summary>
    /// Calculates the score received for the stirring game based on whether ingredients were burned or were too hot.
    /// </summary>
    public void scoreGame()
    {
        int numBurned = 0;
        int numRed = 0;
        foreach (Transform vegetable in vegetables) //each child is a vegetable to be considered for scoring
        {
            vegetable.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0); //stop rotation of the vegetable
            vegetable.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0) - vegetable.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange); //stop motion of vegetable 
            if (vegetable.GetComponent<Renderer>().material.color.r < .5f)
            {
                numBurned++;
            }
            else if (vegetable.GetComponent<Renderer>().material.color.g < .4f)
            {
                numRed++;
            }
        }
        if (!VegetablesCooked)
        {
            Debug.Log("The vegetables more time to cook. Stir them to keep them from burning!");
        }
        else
        {
            int percentBurned = (int)(((float)numBurned) / numVegetablesPlaced * 100);
            int percentRed = (int)(((float)numRed) / numVegetablesPlaced * 100);

            score = (int)((float)(numVegetablesPlaced - numBurned - (.5f * (float)numRed)) / numVegetablesPlaced * 100);
            Debug.Log("You scored " + score + "%.");
			//Begin sensible feedback for the actual game
			if((percentBurned + percentRed) < 10){
				FeedbackText = "Prepared";
			}
			else if((percentBurned + percentRed) < 30){
				FeedbackText = "A few burned";
			}
			else if((percentBurned + percentRed) < 60){
				FeedbackText = "Some burned";
			}
			else if((percentBurned + percentRed) < 90){
				FeedbackText = "A lot burned";
			}
			else{
				FeedbackText = "All burned";
			}
			//Begin really long debug log thing for overambitious feedback
			/*
            if (percentBurned == 100)
            {
                Debug.Log("All of the ingredients burned..");
            }
            else if (percentBurned > 75)
            {
                Debug.Log("Oh no! Most of the ingredients burned! Don't forget to stir them next time.");
            }
            else if (percentBurned > 50)
            {
                Debug.Log("A lot of ingredients burned; they need to be stirred more often.");
            }
            else if (percentBurned > 25)
            {
                Debug.Log("Try to burn less ingredients next time.");
            }
            else if (percentBurned > 0)
            {
                Debug.Log("Some of the ingredients burned; try to stir the dish just a bit more next time.");
            }
            else if (percentBurned == 0)
            {
                Debug.Log("Great job! None of the vegetables burned!");
            }

            if (percentRed == 100)
            {
                Debug.Log("The ingredients look overheated.");
            }
            else if (percentRed > 75) //76-99% red
            {
                Debug.Log("Many of the ingredients may have been overheated.");
            }
            else if (percentRed > 50) //51-75% red
            {
                Debug.Log("A lot of ingredients were too hot.");
            }
            else if (percentRed > 25)//26-50% red
            {
                Debug.Log("Some ingredients may have been too hot.");
            }
            else if (percentRed > 0)//1-25% red
            {
                Debug.Log("The cooked ingredients look good.");
            }
            else if (percentRed == 0)
            {
                Debug.Log("The cooked ingredients look fantastic!");
            }*/
        }
    }

	
    /// <summary>
    /// Duplicates the vegetableTemplate to place vegetables in the pan before the game starts.
    /// </summary>
    void initializeIngredients()
    {
		GameObject vegTemplate = transform.FindChild("vegetables").FindChild("vegetableTemplate").gameObject;
		//this was originally being "found" every frame in a for loop. NO NEED
        for (int i = 0; i < materials.Length; i++) //make vegetables
        {
            GameObject vegetable = GameObject.Instantiate(vegTemplate) as GameObject; //necessary components (rigidbody, mesh filter, mesh renderer) attached to template object.
            vegetable.transform.position = randomPosition(20,20/*widths[i], heights[i]*/); //find a position not currently taken by a vegetable
            vegetable.GetComponent<Renderer>().material = materials[i]; //add material
            vegetable.name = "vegetable" + numVegetablesPlaced; //name
            vegetable.transform.parent = vegetables; //set vegetable as a child of the vegetables GameObject, used so we can easily iterate through the vegetables at any time
            vegetable.transform.localScale = vegetable.transform.parent.localScale * 4;
            Destroy(vegetable.GetComponent<MeshCollider>()); //The vegetable template object has a mesh collider because Physics.checkSphere in randomPosition() works better with mesh colliders, 
            vegetable.AddComponent<SphereCollider>(); //during gameplay (after vegetables are placed), however, the sphere colliders are easier to work with
            vegetable.GetComponent<SphereCollider>().radius = 3.5f;
            vegetable.AddComponent<MGStirVegetable>(); //add vegetable script for burning
            if (donePlacingVegetables)
            {
                Destroy(vegetable);
                break;
            }
            numVegetablesPlaced++;
            if (numVegetablesPlaced == numVegetablesRequested)
            {
                break;
            }
        }
        if (numVegetablesPlaced < numVegetablesRequested && !donePlacingVegetables)
        {
            initializeIngredients();
        }    
    }

    /// <summary>
    /// Calculates the center and the radius of the pan object.
    /// </summary>
    void calculatePan()
    {
        panObject = transform.FindChild("fryingPan").FindChild("Pan").gameObject; //GameObject.Find("Pan");
        PanSpline = panObject.GetComponent(typeof(RageSpline)) as IRageSpline; // find the pan
        radius = PanSpline.GetLength() / 2;
        center = panObject.transform.position;
    }

    /// <summary>
    /// Finds a random position within the circular pan for a new vegetable to exist on startup.
    /// </summary>
    /// <param name="W">Width of the vegetable in need of a position</param>
    /// <param name="H">Height of the vegetable in need of a position</param>
    /// <returns>Vector3 position which is within the pan</returns>
    Vector3 randomPosition(int W, int H) //finds a random position within the circular Pan for a new vegetable to exist on startup. The new vegetable is of width W and height H.
    {
        float panx, pany;
        panx = panObject.transform.position.x;
        pany = panObject.transform.position.y;
        float x = Random.Range(panx - radius + W, panx + radius - W);
        float y = Random.Range(pany - radius + H, pany + radius - H);
        Vector3 pos = new Vector3(x, y, z);
        int count = 0;
        int r = 5; //5 radius of physics sphere to check against existing vegetable locations
        int q = 1; //1 offset, to check against locations nearby existing vegetable locations
        while (Physics.CheckSphere(pos, r) || Physics.CheckSphere(new Vector3(x + q, y, z), r) || Physics.CheckSphere(new Vector3(x - q, y, z), r) || Physics.CheckSphere(new Vector3(x, y + q, z), r) || Physics.CheckSphere(new Vector3(x, y - q, z), r)) //while pos collides with a taken vegetable position
        {
            count++;
            int timeLimit = 100; //makes sure the program doesn't spend too long looking for open table positions for vegetables
            if (count > timeLimit) //should the numVegetables become too large, there will not be enough room for vegetables to fit within the areas, and this while loop will not find an open position once the positions are taken. This count prevents the program from getting stuck in this while loop.
            {
                donePlacingVegetables = true; //the table is too full
                return new Vector3(0, 0, 0);
            }
            x = Random.Range(panx - radius + W, panx + radius - W);
            y = Random.Range(pany - radius + H, pany + radius - H);
            pos = new Vector3(x, y, z);
        }
        return pos;
    }

    /// <summary>
    /// Distance formula for two points in 2D
    /// </summary>
    /// <param name="x1">x coordinate of first point</param>
    /// <param name="y1">y coordinate of first point</param>
    /// <param name="x2">x coordinate of second point</param>
    /// <param name="y2">y coordinate of second point</param>
    /// <returns>float distance between the first and second point</returns>
    float distance(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt((Mathf.Pow(x2 - x1, 2.0f) + Mathf.Pow(y2 - y1, 2.0f)));
    }

    /// <summary>
    /// Distance formula for two points in 2D
    /// </summary>
    /// <param name="p1">the first point</param>
    /// <param name="p2">the second point</param>
    /// <returns>float distance between the first and second point</returns>
    float distance(Vector3 p1, Vector3 p2)
    {
        return Mathf.Sqrt((Mathf.Pow(p2.x - p1.x, 2.0f) + Mathf.Pow(p2.y - p1.y, 2.0f)));
    }

    /// <summary>
    /// Getter for pan center.
    /// </summary>
    /// <returns>Vector3 position of the center of the pan</returns>
    public Vector3 getPanCenter()
    {
        return center;
    }

    /// <summary>
    /// Getter for pan radius.
    /// </summary>
    /// <returns>float radius of the pan</returns>
    public float getPanRadius()
    {
        return radius;
    }
	
	public int getFinalScore(){
		return score;
	}
	
	override public void ScoreMinigame(){
		scoreGame();
		StarResult = (getFinalScore()/20f);
	}

}