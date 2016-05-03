using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MGSortSortingGame : MG_Minigame
{
	//I believe this is the editable stuff...
    public int NumVegetables = 10;
    public int VegetableWidth1 = 20;
    public int VegetableHeight1 = 20;
    public int VegetableWidth2 = 20;
    public int VegetableHeight2 = 20; //number, width, and heigth of vegetables
    public Material Vegetable_1_Material;
    public Material Vegetable_2_Material;
	
	private GameObject vegetable;
	private GameObject template;
	
	
	
    public IRageSpline AreaSpline1, AreaSpline2;
    private Vector3 position, lowleft1, lowright1, highleft1, lowleft2, lowright2, highleft2;
    private int z = 8, n, numVegetablesPlaced; //z position of vegetables, number of vegetables n in the correct location, the score, and the number of vegetables succesfully placed in the game
	public int score;
    
    private bool userFinishedSorting = false, doneMakingVegetables = false;
	public bool SortActive;
	public Vector3 CamLoc;
	private Camera mainMinigameCamera;
	
    // Use this for initialization
    void Start()
    {
        SetupMinigame();
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
                foreach (Transform child in gameObject.transform) //each child is a vegetable
                {
                    if (child.position.x < lowleft1.x || child.position.x > lowright2.x || child.position.y < lowleft1.y || child.position.y > highleft1.y) //if the child vegetable falls off of the table (it's center is past the edge of the table)
                    {
                        Destroy(child.gameObject); //it fell off, destroy it
                        Cursor.visible = true;
                    }
                }
                break;
            case State.Done:
                break;
        }
    }

    public override void SetupMinigame()
    {
        base.SetupMinigame();
        //orthographicSize = 100f/1.24f;
        CanBeScored = true;
        Title = "Sorting Game";
        HandFollow = true;
        CamLoc = GameObject.Find(this.name + "/CameraLoc").transform.position;
        calculateAreaPoints(); //bounds to the scoring areas: we want to place vegetables of type 1 in area 1 and vegetables of type 2 in area 2.
        instantiateVegetables(); //create vegetables and place them on the table in the scene
        SortActive = false;
		mainMinigameCamera = MinigameHolder.GetComponentInChildren<Camera>();
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
    }

    void calculateAreaPoints()
    {
		//removed GameObject.Find("") from next line as it did not look through the children
        AreaSpline1 = transform.FindChild("area1").gameObject.GetComponent(typeof(RageSpline)) as IRageSpline; // find area 1
        lowleft1 = AreaSpline1.GetPositionWorldSpace(2);  //the lower left point on the area used for boundary
        lowright1 = AreaSpline1.GetPositionWorldSpace(1); //the lower right point on the area  
        highleft1 = AreaSpline1.GetPositionWorldSpace(3); //upper left point on the area

        AreaSpline2 = transform.FindChild("area2").gameObject.GetComponent(typeof(RageSpline)) as IRageSpline; // find area 2
        lowleft2 = AreaSpline2.GetPositionWorldSpace(2);  //the lower left point on the area used for boundary
        lowright2 = AreaSpline2.GetPositionWorldSpace(1); //the lower right point on the area  
        highleft2 = AreaSpline2.GetPositionWorldSpace(3); //upper left point on the area
    }

    //Creates vegetables and places them on the table in the scene
    void instantiateVegetables()
    {
        if (NumVegetables < 0) NumVegetables = -NumVegetables; //keep the number of vegetables positive
        for (int i = 0; i < NumVegetables; i++) //make vegetables
        {
			template = gameObject.transform.FindChild("template").gameObject; 
			//originally used GameObject.Find("template"); which created a bug when multiple instances of the sorting game were initialized
			vegetable = GameObject.Instantiate(template) as GameObject; //necessary components (rigidbody, mesh filter, mesh renderer) attached to template object.
			            	
            if (i % 2 == 0) //make a vegetable1
            {
				//below changed from .position to .localPosition as all vegetables were being created in the first game
                vegetable.transform.localPosition = randomPosition(VegetableWidth1, VegetableHeight1); //find a position not currently taken by a vegetable
                vegetable.GetComponent<Renderer>().material = Vegetable_1_Material; //add material
                vegetable.name = "vegetable1"; //name used to detect if the vegetable is in its area for scoring
            }
            else //make a vegetable2
            {
                vegetable.transform.localPosition = randomPosition(VegetableWidth2, VegetableHeight2); //find a position not currently taken by a vegetable
                vegetable.GetComponent<Renderer>().material = Vegetable_2_Material; //add material
                vegetable.name = "vegetable2"; //name used to detect if the vegetable is in its area for scoring
            }
            vegetable.transform.parent = transform; //set vegetable as a child of this root gameobject, used so we can easily iterate through them
            vegetable.transform.localScale = vegetable.transform.parent.localScale * 4;
            Destroy(vegetable.GetComponent<MeshCollider>()); //The template object has a mesh collider because Physics.checkSphere in randomPosition() works better with mesh colliders, 
            vegetable.AddComponent<SphereCollider>(); //during gameplay (after vegetables are placed), however, the sphere colliders are easier to work with
            if (doneMakingVegetables)
            {
                Destroy(vegetable);
                break;
            }
            numVegetablesPlaced++;
        }
    }

    //Returns a random position which does not collide with current GameObject vegetables.
    Vector3 randomPosition(int vegetableWidth, int vegetableHeight)
    {
        int count = 0;
        float x = Random.Range(lowleft1.x + vegetableWidth, lowright2.x - vegetableWidth); //random x range from the left to the right of the table [[area1][area2]]
        while (x > -15 - vegetableWidth && x < -10 + vegetableWidth) //if x is on the border between the two areas, recalculate x
        {
            x = Random.Range(lowleft1.x + vegetableWidth, lowright2.x - vegetableWidth);
        }
        float y = Random.Range(lowleft1.y + vegetableHeight, highleft1.y - vegetableHeight); //random y range from the bottom to the top of the table [[area1][area2]]
        Vector3 pos = new Vector3(x, y, z);
        int r = 10; // radius of physics sphere to check against existing vegetable locations
        int q = 10; // offset, to check against locations nearby existing vegetable locations
        while (Physics.CheckSphere(pos, r) || Physics.CheckSphere(new Vector3(x + q, y, z), r) || Physics.CheckSphere(new Vector3(x - q, y, z), r) || Physics.CheckSphere(new Vector3(x, y + q, z), r) || Physics.CheckSphere(new Vector3(x, y - q, z), r)) //while pos collides with a taken vegetable position 
        {
            count++;
            int timeLimit = 100000; //makes sure the program doesn't spend too long looking for open table positions for vegetables
            if (count > timeLimit) //should the NumVegetables become too large, there will not be enough room for vegetables to fit within the areas, and this while loop will not find an open position once the positions are taken. This count prevents the program from getting stuck in this while loop.
            {
                doneMakingVegetables = true; //the table is too full
                Debug.Log("I couldn't find enough free positions for that many vegetables to exist, but I placed as many as I could.");
                return new Vector3(0, 0, 0);
            }
            x = Random.Range(lowleft1.x + vegetableWidth, lowright2.x - vegetableWidth);//recalculate x
            while (x > -15 - vegetableWidth && x < -10 + vegetableWidth)
            {
                x = Random.Range(lowleft1.x + vegetableWidth, lowright2.x - vegetableWidth);
            }
            y = Random.Range(lowleft1.y + vegetableHeight, highleft1.y - vegetableHeight); //recalculate y
            pos = new Vector3(x, y, z);
        }
        return pos;
    }

    public override void ScoreMinigame()
    {
        scoreGame();
        StarResult = (score / 20);
    }

    //Determines the score.
    public void scoreGame()
    {
        if (score == 0) //if score is 0, the game has not been scored yet
        {
            foreach (Transform child in gameObject.transform) //each child is a vegetable to be considered for scoring
            {
                //float x = child.gameObject.transform.position.x; //x position of veggie
                //float y = child.gameObject.transform.position.y; //y position of veggie
                float x = child.position.x;
                float y = child.position.y;
                
                string name = child.name; //name of veggie
                if (name == "vegetable1") //to score, the vegetable needs to go into the first area
                {
                    if (x > lowleft1.x && x < lowright1.x && y > lowleft1.y && y < highleft1.y) //if the vegetable is in the bounds of the first area
                    {
                        n++; //increment number of vegetables in correct location
                    }
                    child.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0); //stop angular motion
                    child.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0) - child.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange); //stop motion
                }
                else if (name == "vegetable2") //to score, the vegetable needs to go into the second area
                {
                    if (x > lowleft2.x && x < lowright2.x && y > lowleft2.y && y < highleft2.y) //if the vegetable is in the bounds of the second area
                    {
                        n++; //increment number of vegetables in correct location
                    }
                    child.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0); //stop angular motion
                    child.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0) - child.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange); //stop motion
                }
            }
            score = (int)(((float)n) / numVegetablesPlaced * 100); //percent of vegetables in the correct position
			if(score == 100){
				FeedbackText = "Prepared";
			}
			else{
				FeedbackText = (numVegetablesPlaced - n) + " Misplaced";
			}
            Debug.Log("You scored " + score + "%.");
        }
    }
	
	void checkActive(){
		if(CamLoc - new Vector3(0f, 0f, 10f) == mainMinigameCamera.transform.position){
			SortActive = true;
		}
		else{
			SortActive = false;
		}
	}



    public override void SetGameVariables(Dictionary<string, string> variableDict)
    {
        foreach (KeyValuePair<string, string> vPair in variableDict)
        {
            switch (vPair.Key)
            {
                case "NumVegetables":
                    NumVegetables = int.Parse(vPair.Value);
                    break;
            }
        }
    }

    public override Dictionary<string, string> GetGameVariables()
    {
        var variableDict = new Dictionary<string, string>();

        variableDict.Add("NumVegetables", NumVegetables.ToString());

        return variableDict;
    }
}
