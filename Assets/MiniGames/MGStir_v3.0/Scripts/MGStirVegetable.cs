using UnityEngine;
using System.Collections;

public class MGStirVegetable : MonoBehaviour 
{
	public int maxSteps, minSteps;
	
	public float elapsedTimeInSeconds = 0;
    public int UnburnRate = 10; //larger unburnRate: 10 vegetables unburn more quickly.
	//this is the number of times unburn is called per collision...?!
	
    float r = 1, g = 1, b = 1, a = 1, distanceToPanCenter, panRadius; //62.29044
    private MGStirGame sg;
    public bool Burnt = false, Red = false, PauseBurn = false, Game = true;
    private GameObject timer;
	
	public float TimeBeforeVegetableStartsBurning = .4f; //amount of time in seconds until the game will perform the burn function on a vegetable
	public float TimeBeforeVegetableBurnsInSeconds = 10f;
	public float TimeBeforeVegetableIsFullyBurntInSeconds = 5f;
	
	//only public for visual debugging
	public float timeLeftBeforeStartingToBurn = 1.0f;
	public float timeLeftBeforeFullyBurned = 1.0f;
	
	float elapsedTimeSinceStir = 0;
	float elapsedTimeSinceStartedCooking = 0;
	float elapsedTimeSinceStartedBurning = 0;
	
	float cookingRate = 0.01f; //every time burn() is called, the time is reduced by these factors
	float burningRate = 0.01f;
	float burnPreventionRate = 0.01f; //every time unburn() is called, the time before burning is increased
	float amountVegetableIsBurned = 0f; //if the vegetable is burned at all, we hold on to that until the game is scored
	
	public float BurnPenalty = 0.5f;
	float burnColorOffset = 0.3f; // this is multiplied by amountVegetableIsBurned to "blacken" the vegetable
	
	public float appxMaxDistanceFromPanCenter = 70;
	public int approxMaxPossiblePanZones = 4; 
	//we use this latter variable due to the variation in the areas of the pan
	//the time until cook/burn reflects if the food is in the center of the pan
	
	// Use this for initialization
	void Start () 
    {
        sg = transform.parent.parent.GetComponent<MGStirGame>();//Main change made here to ensure this works with multiple instances of stirring game
		TimeBeforeVegetableStartsBurning = sg.TimeBeforeVegetableStartsBurning;
		TimeBeforeVegetableBurnsInSeconds = sg.TimeBeforeVegetableBurnsInSeconds;
		TimeBeforeVegetableIsFullyBurntInSeconds = sg.TimeBeforeVegetableIsFullyBurntInSeconds;
		UnburnRate = (int)(sg.StirAlleviationRate * 10);
        panRadius = sg.getPanRadius();
        timer = sg.gameObject.transform.FindChild("Timer").gameObject;//GameObject.Find("Timer");
	}

	// Update is called once per frame
	void FixedUpdate () 
    {
        if (Game)
        {
			elapsedTimeInSeconds += Time.deltaTime*FFTTimeManager.Instance.GameplayTimeScale;
            elapsedTimeSinceStir += Time.deltaTime*FFTTimeManager.Instance.GameplayTimeScale;
            if (!Burnt)
            {
				//update the appearance of our vegetables
				float visualBurnFactor = burnColorOffset * amountVegetableIsBurned;
                GetComponent<Renderer>().material.color = new Color(r-visualBurnFactor, g-visualBurnFactor, b-visualBurnFactor, a);
				
                if (elapsedTimeSinceStir > TimeBeforeVegetableStartsBurning)
                {
					int zoneSeparationDistance = 20;
					int radiusPadding = 20;
					//start Cooking/Burning the vegetable based on distance from the center
					int numberOfBurnSteps = (int)(panRadius + radiusPadding - distanceToPanCenter) / zoneSeparationDistance;
					
					//panRadius ~60, distanceToPanCenter(0,70)
                    
					burn(numberOfBurnSteps + 1); //range is 0-3, so add one to ensure there is always heat applied
					/*
					if (maxSteps < numberOfBurnSteps)
						maxSteps = numberOfBurnSteps;
					else if (minSteps > numberOfBurnSteps)
						minSteps = numberOfBurnSteps;
					*/
					
					/*
					for (int i = 0; i < (panRadius + 20 - distanceToPanCenter); i += 20)
                    {
						//call function once instead of multiple times!!!!
                        //burn();
						
						//if the vegetablesAreCooked, double the burn rate?
						
						
                        if (sg.VegetablesCooked == true)
                        {
                            //burn();
                        }
                        
                    }
                    */
					
                }
                //renderer.material.color = new Color(r, g, b, a);
            }
            distanceToPanCenter = distance(transform.position, sg.getPanCenter());
			/*
            if (Input.GetKeyDown(KeyCode.Space) && timer.transform.eulerAngles.z > 160) //this is the user's signal that (s)he is finished sorting the vegetables and is ready to be scored.
            {
                PauseBurn = true;
            }
            */
        }
	}
	
	void burn()
	{
		burn(1);
	}
	
	void unburn()
	{
		unburn(1);
	}

    /// <summary>
    /// Makes the attached object appear one step more red.
    /// </summary>
    void burn(int numSteps)
    {
		if (Burnt)
			return;
		
		float zoneClamp = approxMaxPossiblePanZones / 2;
		//we divide this by two because we are using this to lessen the effect at the edge, but leave it heightened at the center
		
        if (!PauseBurn)
        {
            if (!Red) //our vegetable is cooking, but will burn if not stirred
            {
				elapsedTimeSinceStartedCooking += (Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale * numSteps/zoneClamp);
				float timeLeftInState = TimeBeforeVegetableBurnsInSeconds - elapsedTimeSinceStartedCooking;
				timeLeftBeforeStartingToBurn = Mathf.Clamp(timeLeftInState / TimeBeforeVegetableBurnsInSeconds, 0, 1);
				//timeLeftBeforeStartingToBurn = Mathf.Clamp(timeLeftBeforeStartingToBurn - (cookingRate*numSteps), 0, 1);
                g = timeLeftBeforeStartingToBurn;
                b = timeLeftBeforeStartingToBurn;
				if (timeLeftBeforeStartingToBurn == 0)
				{
					Red = true;
					elapsedTimeSinceStartedCooking = TimeBeforeVegetableBurnsInSeconds; //since it will usually exceed this
				}
            }
            else //our vegetable is burning
            {
				elapsedTimeSinceStartedBurning += (Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale * numSteps/zoneClamp);
				float timeLeftInState = TimeBeforeVegetableIsFullyBurntInSeconds - elapsedTimeSinceStartedBurning;
				timeLeftBeforeFullyBurned = Mathf.Clamp(timeLeftInState / TimeBeforeVegetableBurnsInSeconds, 0, 1);
				//timeLeftBeforeFullyBurned = Mathf.Clamp(timeLeftBeforeFullyBurned - (burningRate*numSteps), 0, 1);
				
				if (1 - timeLeftBeforeFullyBurned > amountVegetableIsBurned)
					amountVegetableIsBurned = 1 - timeLeftBeforeFullyBurned;
                
				r = timeLeftBeforeFullyBurned;
                if (r == 0)
                {
                    Burnt = true;
                    GetComponent<Renderer>().material.color = new Color(0, 0, 0, 255);
                }
            }
        }
    }

    /// <summary>
    /// Makes the attached object appear one step less red.
    /// </summary>
    void unburn(int numSteps)
    {
		if (Burnt)
			return;
		
		float zoneClamp = approxMaxPossiblePanZones / 2;
		//see burn()
		
        if (!PauseBurn)
        {
            if (!Red)
            {
				elapsedTimeSinceStartedCooking -= (Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale * numSteps/zoneClamp);
				float timeLeftInState = TimeBeforeVegetableBurnsInSeconds - elapsedTimeSinceStartedCooking;
				timeLeftBeforeStartingToBurn = Mathf.Clamp(timeLeftInState / TimeBeforeVegetableBurnsInSeconds, 0, 1);
				
				//timeLeftBeforeStartingToBurn = Mathf.Clamp(timeLeftBeforeStartingToBurn + (burnPreventionRate * numSteps), 0, 1);
                g = timeLeftBeforeStartingToBurn;
                b = timeLeftBeforeStartingToBurn;
            }
            else
            {
				//TODO: perhaps clamp the "restore" factor by altering Time.deltaTime here
				elapsedTimeSinceStartedBurning -= (Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale * numSteps/zoneClamp);
				float timeLeftInState = TimeBeforeVegetableIsFullyBurntInSeconds - elapsedTimeSinceStartedBurning;
				timeLeftBeforeFullyBurned = Mathf.Clamp(timeLeftInState / TimeBeforeVegetableBurnsInSeconds, 0, 1);

				//timeLeftBeforeFullyBurned += burnPreventionRate*numSteps;
                if (timeLeftBeforeFullyBurned == 1f)
                {
                    Red = false;
                }
				//timeLeftBeforeFullyBurned = Mathf.Clamp(timeLeftBeforeFullyBurned, 0, 1);
				r = timeLeftBeforeFullyBurned;
            }
        }
    }

    /// <summary>
    /// Called when the attached object first stops colliding with another object
    /// </summary>
    /// <param name="collisionInfo"></param>
    void OnCollisionExit(Collision collisionInfo)
    {
		elapsedTimeSinceStir = 0; //when collided, we reset the timeToBurn
		unburn(UnburnRate);
		/*
        for (int i = 0; i < UnburnRate; i++)
        {
            unburn();
        }
        */
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
}
