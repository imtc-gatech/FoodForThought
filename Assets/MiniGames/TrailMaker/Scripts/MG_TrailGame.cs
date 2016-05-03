using UnityEngine;
using System.Collections;


public class MG_TrailGame : MG_Minigame {
	public enum GameType{
		Continuous,
		Clicking
	}
	
	public GameType Mode;
	public GameObject Dot;
	public RageSpline Background;
	
	public int DotCount = 0;  //how many dots were placed
	public int PointInSequence = 0;  //keeps track of which number you need to go to next
	public int NumberOfMistakes = 0; //counts how many mistakes the player has made.
	public int FalseClicks = 0; //number of times the person has clicked on the wrong dot.
	
	public float TimeElapsed;  //keeps track of how much time has passed.
	public float ScaleFactor = 1;
	public int totalDots;
	public ArrayList PositionLogs;  //where all of the positions are stored to be printed to the xml.
	public ArrayList TimeStampLogs; //where all the timestamps are stored to be printed to the xml.
	public ArrayList DotsHitLogs;  //log for when the time when a dot is hit.
	
	public bool[] Sequence;  //an array that lets the game know which objects in the sequence have been hit.
	
	private Vector3[] positionStorage; //temporary storage for the position variables, holds 50;
	private float[] timeStamps;  //temporary storage for the timestamps (though they really just tell you how much time has elapsed in the minigame)
	
	private float screenLeft; //x value of the left side of the screen
	private float screenRight; //x value of the right side of the screen
	private float screenTop; //y value of the top side of the screen
	private float screenBottom; //y value of the bottom side of the screen
	
	private int logCycleCounter = 0;  //counter for when to dump the data to the logs.
	private Camera minigameCamera;
	private Vector3 mousePos;  //position of the mouse
	private Vector3 lastPos;
	private Vector3 camLoc;
	// Use this for initialization
	void Start () {
		orthographicSize = 90;
		positionStorage = new Vector3[50];
		timeStamps = new float[50];
		PositionLogs = new ArrayList();
		TimeStampLogs = new ArrayList();
		DotsHitLogs = new ArrayList();
		CanBeScored = false;
		minigameCamera = MinigameHolder.GetComponentInChildren<Camera>();
		GameObject tempObject = GameObject.Find(this.name + "/CameraLoc");
		camLoc = tempObject.transform.position;
		mousePos = minigameCamera.ScreenToWorldPoint(Input.mousePosition);
		TimeElapsed = 0;
		screenLeft = Background.GetBounds().xMin + camLoc.x;
		Debug.Log("screenLeft: " + screenLeft);
		screenRight = Background.GetBounds().xMax -40 + camLoc.x;
		Debug.Log("screenRight: " + screenRight);
		screenTop = Background.GetBounds().yMax + camLoc.y;
		Debug.Log("screenTop: " + screenTop);
		screenBottom = Background.GetBounds().yMin + camLoc.y;
		Debug.Log("screenBottom: " + screenBottom);
		PointInSequence = 1;
		//ScaleFactor = 5f/(float)totalDots;
		Sequence = new bool[totalDots];
		generateDots(totalDots); //instantiate all the dots in random locations
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(CurrentState != MG_Minigame.State.Dormant){ //after the game has been started
			mousePos = minigameCamera.ScreenToWorldPoint(Input.mousePosition); //finds out the mouse position
			TimeElapsed += Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale; //calculates how much time has elapsed.
			LogData(); //logs both of the previous items
		}
		
		if(PointInSequence == totalDots + 1){
			CanBeScored = true;
		}
	}
	
	Vector3 randomPosition(int dotWidth, int dotHeight)
    {
		
        int count = 0;
		float z = -3;
        float x = Random.Range(screenLeft + dotWidth, screenRight - dotWidth); //random x range from the left to the right of the screen.
        float y = Random.Range(screenBottom + dotHeight, screenTop - dotHeight); //random y range from the bottom to the top of the table [[area1][area2]]
        Vector3 pos = new Vector3(x, y, z);
        float r = 10f*ScaleFactor; // radius of physics sphere to check against existing vegetable locations
        float q = 10f*ScaleFactor; // offset, to check against locations nearby existing vegetable locations
        while (Physics.CheckSphere(pos,r) || Physics.CheckSphere(new Vector3(x + q, y, z), r) || Physics.CheckSphere(new Vector3(x - q, y, z), r) || Physics.CheckSphere(new Vector3(x, y + q, z), r) || Physics.CheckSphere(new Vector3(x, y - q, z), r)) //while pos collides with a taken vegetable position 
        {
            count++;
            int timeLimit = 1000; //makes sure the program doesn't spend too long looking for open table positions for vegetables
            if (count > timeLimit) //should the NumVegetables become too large, there will not be enough room for vegetables to fit within the areas, and this while loop will not find an open position once the positions are taken. This count prevents the program from getting stuck in this while loop.
            {
                Debug.Log("I couldn't find enough free positions for that many dots to exist, but I placed as many as I could.");
				totalDots = DotCount;
                return new Vector3(0, 0, 0);
            }
            x = Random.Range(screenLeft + dotWidth, screenRight - dotWidth);//recalculate x
            y = Random.Range(screenBottom + dotHeight, screenTop - dotHeight); //recalculate y
            pos = new Vector3(x, y, z);
        }
        return pos;
    }
	
	void generateDots(int numDots){  //this method randomly generates the points.
		for(int i=0; i < numDots; i++){
			Vector3 dotPosition = randomPosition(10,10);
			if(dotPosition != new Vector3(0,0,0)){ //makes sure that the game hasn't run out of places to put dots.
				DotCount++;  //keeps track of how many dots have been made
				GameObject DotTemp;
				DotTemp = Instantiate(Dot, dotPosition, this.transform.rotation) as GameObject;  //places the dot in the previously approved random position.
				DotTemp.transform.parent = this.transform;
				DotTemp.GetComponent<MG_TrailDot>().DotNumber = DotCount; 
				Sequence[i] = true;
			}
		}
		Debug.Log("number of dots: " + DotCount);
		totalDots = DotCount;
	}
	
	
	void LogData(){ //this method adds the different sets of data being logged to the temprary arrays.
		if(logCycleCounter == 50){  //every fifty frames
			DumpLogs();  //dump the arrays out into their respective lists.
			logCycleCounter = 0;
		}
		positionStorage[logCycleCounter] = mousePos;
		timeStamps[logCycleCounter] = TimeElapsed;
		logCycleCounter ++;
		
	}
	
	void DumpLogs(){
		PositionLogs.AddRange(positionStorage);
		TimeStampLogs.AddRange(timeStamps);
	}
	
	public void LogDot(){
		DotsHitLogs.Add(TimeElapsed);
	}
	
	public void CheckSequence(){
		bool sequenceDone = true;
		for(int x = 0; x < totalDots; x++){
			if(Sequence[x]){
				PointInSequence = x+1;
				Debug.Log("Sequence changed, PIS: " + (x+1));
				sequenceDone = false;
				break;
			}
		}
		if(sequenceDone){
			Debug.Log("Sequence Complete");
			PointInSequence = totalDots+1;
		}
	}
	
	override public void ScoreMinigame(){
		StarResult = 5f;
		for(int x = 0; x < NumberOfMistakes; x++){
			if(StarResult >.5f){
				StarResult -= .1f; 
			}
		}
		StarResult = StarResult*((float)(totalDots-FalseClicks)/(float)totalDots);
		Debug.Log(StarResult + " stars");
		Debug.Log("took " + TimeElapsed + " seconds");
	}
}
