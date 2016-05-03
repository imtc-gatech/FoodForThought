using UnityEngine;
using System.Collections;

/// <summary>
/// MG blend_ fruit controller script.
/// </summary>
public class MGBlend_FruitControllerScript : MonoBehaviour {
	public MGBlend_GameScript gs;
	private MGBlend_AreaDetectionScript ADS; //reference to area detection script
	private MGBlend_ButtonScript BS; //reference to button script
	private MGBlend_LiquidScript LS; //reference to liquid script
	private MGBlend_BlenderTopScript BTS; //reference to blender top script
	
	private Vector3 mousepos; //position of mouse
	private Vector3 worldpos; //position of mouse in worldspace
	private Vector3 offsetVector; //the offset needed to make sure that the fruit appears under the mouse when you hold it
	private Vector3 origLocation; //the original location of the fruit
	private Vector3 stableLocation; //the stable location of the fruit in liquid
	private Vector3 movingLocation; //the moving location of the fruit in liquid
	
	private Vector3 fallingVector; //The current location of the fruit whilst falling
	private float fallTime = 2f; //how fast we want the fruit to appear to be falling into the liquid
	
	public bool InLiquid = false; //true if fruit is in liquid
	private bool falling = false; //true if fruit is mid fall
	public bool Clickable = true; //true if fruit is clickable (not a shadow, hasn't been blended)
	public bool Blendable = true; //true if fruit is unblended
	public bool Increasable = true; //true if fruit status bar has not been completely filled
		
	private int counter = 0; //a counter that assists in the fruit bouncing animation
	private int counterMod; //also assists in the fruit bouncing animation
	
	private int fruitSinkCounter = 0; //a counter that helps keep track of when to make the fruit sink some
	private int fruitSinkMod; //also helps keep track of when to make the fruit sink some
	
	private int fruitSinkStopCounter = 0; //stops the fruit from sinking more after it has sinked a certain number of times. 
	
	private Camera mainMinigameCamera;
	
	/// <summary>
	/// Used for initialization.
	/// </summary>
	void Start(){
		if(gs == null){
			gs = GameObject.Find(transform.parent.parent.name).GetComponent<MGBlend_GameScript>();
		}
		ADS = GameObject.Find (gs.name + "/AreaDetection").GetComponent<MGBlend_AreaDetectionScript>();
		BS = GameObject.Find (gs.name + "/ButtonCollider").GetComponent<MGBlend_ButtonScript>();
		LS = GameObject.Find (gs.name + "/BlenderLiquid").GetComponent<MGBlend_LiquidScript>();
		BTS = GameObject.Find (gs.name + "/BlenderTopCollider").GetComponent<MGBlend_BlenderTopScript>();
		mainMinigameCamera = gs.MinigameHolder.GetComponentInChildren<Camera>();
	}
	
	/// <summary>
	/// update is called once per frame
	/// </summary>
	void Update(){
		if (gs.CurrentState == MG_Minigame.State.Active)
		{
			mousepos = Input.mousePosition;
			mousepos.z = 5.0f;
			worldpos = mainMinigameCamera.ScreenToWorldPoint(mousepos);
				
			//makes sure the fruit appears to be under the controller when clicked.
			offsetVector = new Vector3(worldpos.x, worldpos.y, gs.CameraLoc.z + 5.5f);
			
			//if the fruit is falling, keep track of when it is in the water, then make the liquid rise
			if(falling){
				fallingVector = transform.position;
							
				if(fallingVector.y < LS.fruitJustPastLiquidStop){
					falling = false;	
					stableLocation = transform.position;
					InLiquid = true;
					LS.liquidRise (1f);
					LS.calledFromFruitRefresh();
				}
			}
			
			//if the fruit is in the liquid, do these things
			if(InLiquid){	
				//if another fruit gets taken out, the liquid will sink, so the fruits should too.
				//this allows the fruit to sink.
				if(!BS.Pushing){
					fallingVector = transform.position;
					if(fallingVector.y < LS.fruitJustPastLiquidStop){
						fruitSink();
						stableLocation = fallingVector;
					}
				}
				
				//if player is blending, make the fruit move and sink; else, put the fruit in its unmoving position.
				if(BS.Pushing){
					movingLocation = stableLocation;
					movingLocation.y += 1f;
					blendTime();
				}
				else
					transform.position = stableLocation;
			}
		}
		
	}
	
	//when a fruit is clicked, gets the original location of the fruit so that it can be returned to the same spot.
	void OnMouseDown(){
		if(!BS.DisableGameplay){
			if(Clickable){	
				if(!InLiquid){
					if(worldpos.x < gs.CameraLoc.x + (-130)) //leftmost ingredient location
						origLocation = gs.CameraLoc + new Vector3(-177.54f, -60.5f, 10.5f);
					else if(worldpos.x > gs.CameraLoc.x + (-50)) //rightmost ingredient location
						origLocation = gs.CameraLoc + new Vector3(-34.98f, -60.5f, 10.5f);
					else //center ingredient location
						origLocation = gs.CameraLoc + new Vector3(-106.25f, -60.5f, 10.5f);	
				}
			}
		}
	}
	
	//lets the fruit be dragged
	void OnMouseDrag(){
		if(Clickable && !InLiquid){
			transform.position = offsetVector;
			BS.CanPush = false;
		}
	}
	
	//drops the fruit, either returning it to original pos or into the blender
	void OnMouseUp(){
		if(Clickable){ //if the fruit is now a shadow
			if(!InLiquid){ //if you are dropping the fruit, and not trying to pick it up
				if(checkFruitDroppable()){ //if the fruit is within the bounds of the acceptable blender drop area
					falling = true;
					fruitDrop();	//drop the fruit
				}
				else{ //else, put it back in the original position.
					transform.position = origLocation;
					BS.CanPush = true;

				}
			}
			else{ //you are trying to pick up the fruit from the liquid
				if(!BTS.CapOn){ //but you're only allowed to do it if the cap is off!
					transform.position = origLocation;
					LS.liquidSink(1f);
					LS.calledFromFruitRefresh();
					InLiquid = false;
				}
			}
		}
	}
	
	/// <summary>
	/// checks to see if the fruit is within the accepted boundary to be dropped into blender
	/// </summary>
	/// <returns>
	/// true if the fruit can be dropped in the blender, false if the fruit needs to get returned to orig location
	/// </returns>
	bool checkFruitDroppable(){
		if(ADS.Droppable)
			return true;
		else
			return false;
	}
	
	/// <summary>
	/// drops the fruit into the blender
	/// </summary>
	void fruitDrop(){
		BS.CanPush = true;
		GameObject.Destroy(gameObject.GetComponent<ObjectHighlightScript>());
		
		Vector3 behind = transform.position;
		behind.z = gs.CameraLoc.z + 8f;
		
		transform.position = behind;
		
		float yLoc = LS.fruitJustPastLiquidStop-1f;
		float xLoc;
		
		if(transform.position.x < gs.CameraLoc.x + 91f)
			xLoc = 90f;
		else if(transform.position.x > gs.CameraLoc.x + 97f)
			xLoc = 100f;
		else
			xLoc = 94.5f;
		
		Vector3 target =  new Vector3(gs.CameraLoc.x + xLoc, yLoc, gs.CameraLoc.z + 10.5f);
		
		iTween.MoveTo (gameObject, target, fallTime);
	}
	
	/// <summary>
	/// sinks the fruit after a certain amount of blending
	/// </summary>
	void fruitSink(){
		Vector3 sinkLocation = transform.position;
		sinkLocation.y -=3f;
		transform.position = sinkLocation;
	}
	
	/// <summary>
	/// blendTime happens while the fruits are being blended.
	/// </summary>
	void blendTime(){
		if(Clickable)
			Clickable = false;
		
		counter++;
		counterMod = counter%4;
	
		//if the fruit is in the liquid, it makes the fruit look like it's bouncing up and down
		if(counterMod < 2)
			transform.position = movingLocation;
		else
			transform.position = stableLocation;
		
		if(!BS.Spray){
			fruitSinkCounter++;
			fruitSinkMod = fruitSinkCounter%25;
			
			if(fruitSinkMod == 0){
				fruitSinkStopCounter++;
				
				if(fruitSinkStopCounter < 7)
					stableLocation.y -= 5f;

				LS.liquidRise(2f);	
			}
			
			if(fruitSinkCounter > 250)
				fruitSinkCounter = 0;
		}
	} 
}
