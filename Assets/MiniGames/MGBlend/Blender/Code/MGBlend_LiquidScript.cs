using UnityEngine;
using System.Collections;

public class MGBlend_LiquidScript : MonoBehaviour {
	private MGBlend_ButtonScript BS; //reference to the button script
	
	private IRageSpline liquidSpline; //the spline for the liquid
	private Vector3[] stationaryPos = new Vector3[7]; //the positions of the liquid when not moving
	private int numOfMovingPts = 7; //the number of points on the liquid spline that move while blending
	private Vector3 movingPos; //the constantly updating moving location of each stationary point
	
	private Vector3[] origPos = new Vector3[10]; //array holding each of the locations of the original liquid location
	
	private int counter = 0; //helps keep track of when to make the liquid rise
	private int counterMod; //also helps assist in keeping track of when to make the liquid rise
	
	private int liquidRiseCounter = 0; //keeps track of how many times the liquid has risen. has a max, so that liquid can't overflow
	
	public float fruitJustPastLiquidStop = 0f; //the "boundary" of how far a fruit goes in the liquid after it's been dropped
	
	/// <summary>
	/// Used for initializations.
	/// </summary>
	void Awake(){
		BS = transform.parent.FindChild("ButtonCollider").gameObject.GetComponent<MGBlend_ButtonScript>(); //GameObject.Find("ButtonCollider").GetComponent<MGBlend_ButtonScript>();
		
		liquidSpline = gameObject.GetComponent(typeof(RageSpline)) as IRageSpline;
		
		for(int i=0; i<9; i++){
			origPos[i] = liquidSpline.GetPositionWorldSpace(i);	
		}
		
		int tempIndex = 0;
		for(int i=2; i<9; i++){
			stationaryPos[tempIndex] = liquidSpline.GetPositionWorldSpace(i);
			tempIndex++;
		}
		
		fruitJustPastLiquidStop = liquidSpline.GetPositionWorldSpace(5).y + 2f;
	}
	
	/// <summary>
	/// Called once per frame
	/// </summary>
	void Update(){
		if(BS.Pushing) //if blending
			shakeWater(); //make the water move
	}
	
	/// <summary>
	/// Shakes the water, making it rise after a certain amount of blending has happened.
	/// </summary>
	void shakeWater(){
		counter++;
		counterMod = counter%4;
		for(int index = 1; index < 4; index++){
			if(counterMod < 2){
				for(int i = 0; i<numOfMovingPts; i++){
					movingPos = stationaryPos[i];
					if(i%2 == 0)
						movingPos.y += 1f;					
					else
						movingPos.y -= 1f;
					liquidSpline.SetPointWorldSpace(i+2, movingPos);
				}
			}
			else{
				for(int i = 0; i<numOfMovingPts; i++){
					movingPos = stationaryPos[i];
					if(i%2 == 0)
						movingPos.y -= 1f;					
					else
						movingPos.y += 1f;
					liquidSpline.SetPointWorldSpace(i+2, movingPos);
				}
			}
		}
	
		liquidSpline.RefreshMesh(true, false, false);
	}
	
	/// <summary>
	/// returns the liquid to it's stationary position.
	/// </summary>
	public void stabilizeWater(){
		counter = 0;
		for(int i = 0; i < numOfMovingPts; i++){
			liquidSpline.SetPointWorldSpace(i+2, stationaryPos[i]);
			liquidSpline.RefreshMesh (true, false, false);
		}
	}
	
	/// <summary>
	/// raises the liquid
	/// </summary>
	/// <param name='riseAmount'>
	/// How much the liquid should rise by.
	/// </param>
	public void liquidRise(float riseAmount){
		liquidRiseCounter++;
		
		if(liquidRiseCounter < 13){
			Vector3 temp;
			for(int i = 0; i < numOfMovingPts; i++){
				temp = stationaryPos[i];
				temp.y += riseAmount;
				stationaryPos[i] = temp;
			}
			
			if((liquidRiseCounter > 3)){
				float widthIncrementAmount = 1f;
				if(liquidRiseCounter%3 != 0)
					widthIncrementAmount = .1f;
					
				temp = stationaryPos[0];
				temp.x -= widthIncrementAmount;
				stationaryPos[0] = temp;
				
				temp = stationaryPos[6];
				temp.x += widthIncrementAmount;
				stationaryPos[6] = temp;
			}
			
			fruitJustPastLiquidStop = liquidSpline.GetPositionWorldSpace(5).y + 2f;
		}
	}
	
	/// <summary>
	/// makes the liquid amount decrease
	/// </summary>
	/// <param name='sinkAmount'>
	/// Sink amount. the amount to make the liquid go down by.
	/// </param>
	public void liquidSink(float sinkAmount){
		liquidRiseCounter--;
		
		Vector3 temp;
		for(int i = 0; i < numOfMovingPts; i++){
			temp = stationaryPos[i];
			temp.y -= sinkAmount;
			stationaryPos[i] = temp;
		}
			
		if((liquidRiseCounter > 3)){
			float widthIncrementAmount = -1f;
			if(liquidRiseCounter%3 != 0)
				widthIncrementAmount = -.1f;
					
			temp = stationaryPos[0];
			temp.x -= widthIncrementAmount;
			stationaryPos[0] = temp;
			
			temp = stationaryPos[6];
			temp.x += widthIncrementAmount;
			stationaryPos[6] = temp;
		}
			
		fruitJustPastLiquidStop = liquidSpline.GetPositionWorldSpace(5).y - 2f;
		
	}
	
	/// <summary>
	/// Refreshes the mesh after the liquid spline has been altered from another script
	/// </summary>
	public void calledFromFruitRefresh(){
		for(int i=2; i<9; i++){
			liquidSpline.SetPointWorldSpace(i, stationaryPos[i-2]);
		}
		liquidSpline.RefreshMesh(true, false, false);
	}
	
	/// <summary>
	/// Resets the liquid back to its original location.
	/// </summary>
	public void resetLiquid(){
		for(int i=0; i<9; i++){
			liquidSpline.SetPointWorldSpace(i, origPos[i]);	
		}
		
		int tempIndex = 0;
		for(int i=2; i<9; i++){
			stationaryPos[tempIndex] = liquidSpline.GetPositionWorldSpace(i);
			tempIndex++;
		}
		
		liquidSpline.RefreshMesh(true, false, false);
		
		fruitJustPastLiquidStop = liquidSpline.GetPositionWorldSpace(5).y + 2f;
	}
}
