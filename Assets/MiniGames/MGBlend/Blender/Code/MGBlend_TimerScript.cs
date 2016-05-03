using UnityEngine;
using System.Collections;

/// <summary>
/// MG blend_ timer script. This script is attached to the splotches.
/// </summary>
public class MGBlend_TimerScript : MonoBehaviour {
	private GameObject button; //reference for the button collider
	private GameObject highlight; //reference for the blender top highlight
	private GameObject arrow; //reference to the arrow
	private IRageSpline splotchSpline; //reference to the splotch spline
	float lifeTime = 1f; //life in seconds
	private bool fadingComplete = false; //returns true if the splotches are done fading
	private bool flashingComplete = false; //returns true if the arrow is done flashing
	private float alphaValue = 1f; //the amount the decrease the alpha by (for fading the splotches)
	private Color splineColor; //the pink color of the splotch
	private Color splineEmboss1; //the first emboss color of the splotch
	private Color splineEmboss2; //the second emboss color of the splotch
	private Color splineOutline; //the color of the outline of the spline
	private int flashCounter = 0; //keeps count of the number of flashes and helps regulate the frequency of the flash
	private Vector3 position = new Vector3(160f, 88f, 7.5f); //the vector location for the highlight gameobject (171.53f, -80.72f, 7.5f);
	public MGBlend_GameScript Root;
	
	/// <summary>
	/// Awake this instance. Used for initialization. 
	/// </summary>
	void Start(){
		position += Root.CameraLoc;
		button = Root.transform.FindChild("ButtonCollider").gameObject; //GameObject.Find ("ButtonCollider");
		splotchSpline = gameObject.GetComponent(typeof(RageSpline)) as IRageSpline;
		splineColor = splotchSpline.GetFillColor1();
		splineEmboss1 = splotchSpline.GetEmbossColor1();
		splineEmboss2 = splotchSpline.GetEmbossColor2();
		splineOutline = splotchSpline.GetOutlineColor1();
	}
	
	/// <summary>
	/// Update this instance. This is called once per frame.
	/// </summary>
	void Update () {
		if(button.GetComponent<MGBlend_ButtonScript>().DisableGameplay){
			lifeTime -= Time.deltaTime*FFTTimeManager.Instance.GameplayTimeScale; //subtract time from life
			if((lifeTime <= 0) && (!fadingComplete)){ //until the fading is complete, set new colors. 
				setNewAlphaColors();
				fadeOutSplotches();
				
				if(alphaValue <= 0f)
					fadingComplete = true;
			}
			
			//if done fading, flash arrow and highlight
			if(fadingComplete && !flashingComplete){
				highlightCap();
			}
			
			//if flashing complete, destroy splotches and re-enable gameplay
			if(flashingComplete){
				button.GetComponent<MGBlend_ButtonScript>().DisableGameplay = false;
				Destroy (gameObject);
			}
		}
	}
	
	/// <summary>
	/// Sets the new alpha colors.
	/// </summary>
	void setNewAlphaColors(){
		alphaValue-=.1f;
		
		splineColor.a = alphaValue;
		splineEmboss1.a = alphaValue;
		splineEmboss2.a = alphaValue;
		splineOutline.a = alphaValue;
	}
	
	/// <summary>
	/// Fades out each of the splotches.
	/// </summary>
	void fadeOutSplotches(){
			splotchSpline.SetFillColor1(splineColor);
			splotchSpline.SetEmbossColor1(splineEmboss1);
			splotchSpline.SetEmbossColor2(splineEmboss2);
			splotchSpline.SetOutlineColor1(splineOutline);
			splotchSpline.RefreshMesh();
			
			foreach(Transform child in gameObject.transform){
				IRageSpline tempSpline = child.GetComponent (typeof(RageSpline)) as IRageSpline;
				tempSpline.SetFillColor1(splineColor);
				tempSpline.SetEmbossColor1(splineEmboss1);
				tempSpline.SetEmbossColor2(splineEmboss2);
				tempSpline.SetOutlineColor1(splineOutline);
				
				tempSpline.RefreshMesh();
			}
	}
	
	/// <summary>
	/// Highlights the cap.
	/// </summary>
	void highlightCap(){
		flashCounter++;
		
		if(flashCounter == 1){
			highlight = Instantiate (Resources.Load("BlenderTopHighlight")) as GameObject;
			highlight.transform.position = position;
			highlight.transform.parent = Root.transform;
			arrow = Instantiate(Resources.Load ("Arrow")) as GameObject;
			arrow.transform.position += Root.CameraLoc;
			arrow.transform.parent = Root.transform;
		}
		
		//makes the white highlight flash
		if(flashCounter%20 == 0){
			if(highlight.transform.position.z == position.z)
				highlight.transform.position = new Vector3(position.x, position.y, 15.0f);
			else
				highlight.transform.position = position;
		}
		
		if(flashCounter >= 120){
			flashingComplete = true;
			Destroy(highlight);
			Destroy(arrow);
		}
	}
}
