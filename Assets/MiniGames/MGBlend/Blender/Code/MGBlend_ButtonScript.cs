using UnityEngine;
using System.Collections;

/// <summary>
/// MG blend_ button script. This script controls the behavior of the blender button
/// </summary>
public class MGBlend_ButtonScript : MonoBehaviour {
	public MGBlend_LiquidScript LS; //Reference to the liquid Script
	public GameObject PushedButton; //The button appearance when pushed
	public GameObject UnpushedButton; //the button appearance when the button is not pushed
	public GameObject BlenderTop; //The blender top
	private Vector3 hidden = new Vector3(0f, 0f, 0f); //the vector position to hide the button image that is not being used
	private Vector3 visible; //the vector position to show the button image in use.
	public int MistakeCount = 0; //keeps count of how many mistakes have been made (Blending without the top)
	public bool Blending; //returns true if the blender is Blending
	public bool Spray; //returns true if player tries to blend without the top on
	public bool Pushing; //returns true if the user is pushing the button;
	public bool FruitMoving; //returns true if the fruit is in the process of being dropped
	public bool CanPush; //returns true if the player is allowed to push the button right now. This prevents the button from being "pushed" when the user's mouse is over it when they are holding a fruit
	public bool DisableGameplay = false; //returns true while spray animation is playing
	public AudioClip sound; //sound for the blender
	public MGBlend_GameScript Root;
	
	/// <summary>
	/// Awake this instance. Used for initialization.
	/// </summary>
	void Awake () {
		Root = GameObject.Find(transform.parent.name).GetComponent<MGBlend_GameScript>();
		LS = GameObject.Find(transform.parent.name + "/BlenderLiquid").GetComponent<MGBlend_LiquidScript>();
		BlenderTop = GameObject.Find (transform.parent.name + "/BlenderTopCollider");
		PushedButton = GameObject.Find (transform.parent.name + "/ButtonDown");
		UnpushedButton = GameObject.Find (transform.parent.name + "/ButtonUp");
		visible = UnpushedButton.transform.position;
		PushedButton.transform.position = hidden;
		Blending = false;
		Spray = false;
		Pushing = false;
		FruitMoving = false;
		CanPush = true;
	}
	
	/// <summary>
	/// Raises the mouse down event. As long as the spray animation is not playing, 
	/// when the button is pushed, the animation of the button being pushed happens, 
	/// and sets the necessary variable accordingly.
	/// </summary>
	void OnMouseDown(){
		if(!DisableGameplay && CanPush){
			PushedButton.transform.position = visible;
			UnpushedButton.transform.position = hidden;
			Pushing = true;
			GetComponent<AudioSource>().loop = true;
			GetComponent<AudioSource>().Play();
			if(!BlenderTop.GetComponent<MGBlend_BlenderTopScript>().CapOn){
				Spray = true;
				GameObject splotches = Instantiate (Resources.Load ("Splotch1")) as GameObject;
				splotches.transform.parent = Root.transform;
				splotches.GetComponent<MGBlend_TimerScript>().Root = Root;
				splotches.transform.position = Root.CameraLoc + new Vector3(-160.7f, -47.7f, 0.0f);
			}
			else{
				Blending = true;
			}
		}
	}
	
	/// <summary>
	/// Raises the mouse up event. When the player stops pushing the button, 
	/// the necessary variables get set and sound turns off.
	/// </summary>
	void OnMouseUp(){
		Pushing = false;
		Blending = false;
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Stop();
		PushedButton.transform.position = hidden;
		UnpushedButton.transform.position = visible;
		LS.stabilizeWater();
		if(Spray){
			MistakeCount++;
			DisableGameplay = true; //disables gameplay so player cannot click anything after they splashed everything
			Spray = false;
		}
	}
}
