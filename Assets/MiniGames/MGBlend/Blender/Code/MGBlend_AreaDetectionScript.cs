using UnityEngine;
using System.Collections;

/// <summary>
/// This script is used to determine whether or not the user is correctly
/// positioned to drop a piece of fruit.
/// </summary>
public class MGBlend_AreaDetectionScript : MonoBehaviour {
	public bool Droppable; //variable to set whether or not the fruit is over the opening of blender
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake(){
		Droppable = false;
	}
	
	/// <summary>
	/// If controller is within the area of detection, the ability to drop 
	/// fruit in the blender is enabled.
	/// </summary>
	void OnMouseOver(){
		Droppable = true;
		GetComponent<Rigidbody>().detectCollisions = false;
	}
	
	/// <summary>
	/// When the controller exits the area of detection, the ability to drop 
	/// fruit into the blender is disabled.
	/// </summary>
	void OnMouseExit(){
		Droppable = false;	
		GetComponent<Rigidbody>().detectCollisions = true;
	}
	
}
