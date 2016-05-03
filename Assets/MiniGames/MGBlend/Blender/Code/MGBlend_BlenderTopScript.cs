using UnityEngine;
using System.Collections;

/// <summary>
/// Blender top script. This script controls the blender top.
/// </summary>
public class MGBlend_BlenderTopScript : MonoBehaviour {
	public GameObject BlenderTop;	//variable to hold an instance of BlenderTop
	public GameObject AreaDetection;	//variable to hold an instance of area of detection
	public GameObject Button;	//variable to hold an instance of a button
	public MGBlend_GameScript GameScript;
	private Vector3 blenderLocation;	//the location of the blender 
	private Vector3 blenderLocationVisible;	//the location of the blender 
	private Vector3 counterLocation; //the vector location that the top collider gets place at.
	private Vector3 counterLocationVisible; //the vector location that the non-collider top gets placed at
	private Vector3 boundaryHide;//location of the area detection when disabled
	private Vector3 boundaryShow; //location of the area detection when enabled.
	public bool CapOn; //returns true if the cap is on the blender.
	
	/// <summary>
	/// Awake this instance. This method sets initial gameobjects, and places the gameobjects
	/// in the right screen location.
	/// </summary>
	void Awake(){
		if (GameScript == null)
			GameScript = transform.parent.GetComponent<MGBlend_GameScript>();
		counterLocation = new Vector3(160f, 90f, GameScript.CameraLoc.z + 5.0f);
		counterLocationVisible = new Vector3(-423f, 595f, GameScript.CameraLoc.z + 7.3f);
		boundaryHide = new Vector3(0f, 300f, 0f); 
		BlenderTop = GameScript.transform.FindChild("BlenderTop").gameObject;
		blenderLocationVisible = BlenderTop.transform.position;
		blenderLocation = this.transform.position;
		
		AreaDetection = GameScript.transform.FindChild("AreaDetection").gameObject;
		boundaryShow = AreaDetection.transform.position;
		AreaDetection.transform.position = boundaryHide;
		
		Button = GameScript.transform.FindChild("ButtonCollider").gameObject;
		
		CapOn = true;
	}
	
	/// <summary>
	/// Raises the mouse down event. When the blender top is pushed, 
	/// if the blender top is on the blender, the top gets moved to the counter.
	/// if the blender top is on the counter, the top ets moved to the blender.
	/// if the splotch animation is playing, nothing happens.
	/// </summary>
	void OnMouseUp(){	
		if(!Button.GetComponent<MGBlend_ButtonScript>().DisableGameplay){
			moveCap();
		}
	}
	
	/// <summary>
	/// Moves the cap, depending on whether the cap is currently on or off the blender.
	/// </summary>
	void moveCap(){
		if(CapOn){
			//move cap to counterLocation;
			CapOn = false;
			
			iTweenUtilities.MoveBy(this.gameObject,GameScript.CameraLoc +  counterLocation, .7f);
			iTweenUtilities.MoveBy(BlenderTop, GameScript.CameraLoc +  counterLocationVisible, .7f);
						
			AreaDetection.transform.position = boundaryShow;
		}
		else{
			//move cap back to orig location;
			CapOn = true;
			
			iTweenUtilities.MoveBy(this.gameObject, blenderLocation, 2f);
			iTweenUtilities.MoveBy(BlenderTop, blenderLocationVisible, 2f); //blenderlocationvis
			
			AreaDetection.transform.position = boundaryHide;
		}
	}
	
	/// <summary>
	/// Puts the cap back on the blender if it's not already there when the game has been restarted.
	/// </summary>
	public void restartCap(){
		if(!CapOn)
			moveCap ();	
	}
}
