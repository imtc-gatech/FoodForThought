using UnityEngine;
using System.Collections;

//represents a bottle of spice
public class MGSpiceBottle : MonoBehaviour {
	public enum BottleMode {Ready, Shaking};
	
	public BottleMode mode;
    public int AmountOfSpice = 40, NumSplurt = 5, Score, NumSpice = 0; //total amount of spice bits available in the bottle at the start, amount of spice which comes out with one shake, the Score, total number of spice bits which have come out of the bottle
    private int spiceCounter; //counts how many spice bits have been instantiated since the bottle has been tipped, ensuring no more than NumSplurt come out
	public int MouseSensitivity = -50;
	
    public float SpiceSize = 2; //size of each spice bit
    private float rotation; //current rotation of the spice bottle

    public string SpoutName = "spoutX"; //name of spout where spice comes out of this bottle
    public Material spiceMaterial; //material of the spice
    public IRageSpline ScoreAreaSpline; //ragespline for the area where the player should spice

    public bool thisSpiceDesiredOnDish; //if true, spice from this bottle aids the score, if false, spice from this bottle detracts from the score
    private bool tiltedUp = true, selected = false; //is the bottle currently tilted upwards?, is the bottle currently being held?
    private Vector3 spoutPos, mousepos, lowleft, lowright, highleft, midpoint, originalLocation; //position of the spout, position of the mouse, positions of corners on the scoring area, the original location of this bottle 
    private GameObject spout, spiceArea, spiceTarget; //spout where spice comes out, area where spice physically can land, UI target to show the player where spice will land if he shakes the bottle in its current position
    private GameObject spiceTemplate; //original object to clone when the bottle is shaken (added as global to avoid redundant "Find" actions)
	private Quaternion originalRotation; //original rotation of the spice bottle (reset when the bottle is set back on the shelf)
    public MGSpiceGame SpiceGame; //access main script
	private GameObject CameraLoc;
	private Camera mainMinigameCamera;
	private float leftSide; //x position of the left border of when the bottle will transition to shaking mode
	private float rightSide; //same as leftSide, but the righthand border.
	private Vector3 tiltedAngle; //Angle at which the bottle will appear when in Shaking state.  NOTE: make sure to do this from 0-360, with 0 being vertical.  Calculations will not work otherwise
    // Use this for initialization
    void Start()
    {	
		SpiceGame = transform.parent.parent.gameObject.GetComponent<MGSpiceGame>();
        //instantiate objects
		if(SpiceGame == null){
			SpiceGame = transform.parent.GetComponent<MGSpiceGame>();
			if(SpiceGame == null){
				Debug.Log("no spicegame found");
			}
		}
		CameraLoc = SpiceGame.CameraLoc;
        originalLocation = transform.position;
        originalRotation = transform.rotation;
        spout = transform.FindChild(SpoutName).gameObject;
        spoutPos = spout.transform.position;
        spoutPos.z = -.19f;
        spiceArea = SpiceGame.transform.FindChild("spiceArea").gameObject;
        spiceCounter = 0;
        spiceTarget = spiceArea.transform.FindChild("spiceTarget").gameObject;
		mainMinigameCamera = SpiceGame.MinigameHolder.GetComponentInChildren<Camera>();
		leftSide = originalLocation.x + 100f;
		rightSide = originalLocation.x + 250f;
		tiltedAngle = new Vector3(0f,0f,290f);
        
    }

    // Update is called once per frame
    void Update()
    {
		//if(CameraLoc.transform.position == SpiceGame.MinigameHolder.GetComponentInChildren<Camera>().transform.position + new Vector3(0f,-11f, 10f)){
		if(SpiceGame.CurrentState == MG_Minigame.State.Active){
	        if (selected) //if the bottle is currently picked up
	        {
				if (spiceArea == null)
	            	spiceArea = SpiceGame.transform.FindChild("spiceArea").gameObject; //update spice area  LOOK INTO THIS! FIND STATEMENT INSIDE LOOP
	            rotation = this.transform.eulerAngles.z; //current rotation of this spice bottle
	           	mousepos = SpiceGame.MinigameHolder.GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition); //position of mouse
				if (transform.position.x >= leftSide && transform.position.x <= rightSide){ //If the bottle is within the bounds of the shaking area
					mode = BottleMode.Shaking; //set mode to shaking
					if(transform.rotation == originalRotation){ //if the bottle is tilted up
						this.transform.eulerAngles = tiltedAngle; //tilt it to the side
						if(NumSpice >= AmountOfSpice){ //if the spice has been all used up.
							this.transform.eulerAngles = tiltedAngle + new Vector3(0f,0f,-40f); //have the bottle be tilted further to note this
						}
					}
					if(spiceCounter > 0 && transform.eulerAngles.z < tiltedAngle.z - 1){ //if the bottle is in the tilted state and the spice is ready to be dropped
						ShakeBottle(); //drop the spice one bit at a time
					}
				}
				else{
					mode = BottleMode.Ready;
					this.transform.rotation = originalRotation; //set the rotation to vertical
				}
	            this.transform.position = new Vector3(mousepos.x, mousepos.y, originalLocation.z -2); //move spice bottle left/right
	            spiceTarget.transform.position = new Vector3(spout.transform.position.x, spiceTarget.transform.position.y, spiceTarget.transform.position.z); //move spice target with bottle
	        }
		}
    }

    /// <summary>
    /// Creates 1 spice bit at the spout of the spice bottle
    /// </summary>
    void instantiateSpice()
    {
		if (spiceTemplate == null)
			spiceTemplate = SpiceGame.spiceTransform.FindChild("spiceTemplate").gameObject;
        GameObject spice = GameObject.Instantiate(spiceTemplate) as GameObject; //necessary components (rigidbody, mesh filter, mesh renderer) attached to template object.
        spoutPos = spout.transform.position;
        spoutPos.z = spiceArea.transform.position.z;
        spice.transform.position = spoutPos; //spice bit comes out the the bottle spout
        spice.GetComponent<Renderer>().material = spiceMaterial; //add material
        spice.name = SpoutName;//name the spice bit after it's spout
        spice.transform.parent = SpiceGame.spiceTransform; //set spice bit as a child of the spice GameObject, used so we can easily iterate through them at any time
        spice.transform.localScale = spice.transform.parent.localScale * SpiceSize / 2; //scale spice
        spice.AddComponent<MGSpiceBit>(); //add spice bit script
        NumSpice++; //increment total number of spice bits of this spice
    }

    void OnMouseUp() //pick up the bottle
    {
		Debug.Log(this.name + " clicked");
        if (!selected)
        {
			Debug.Log(this.name + " now selected");
			selected = true;
			spiceTarget.transform.position = new Vector3(spiceTarget.transform.position.x, spiceTarget.transform.position.y, spiceTarget.transform.position.z - 200);
        }
		else if (selected)
        {
			if(mode == BottleMode.Ready){
				selected = false;
				spiceTarget.transform.position = new Vector3(spiceTarget.transform.position.x, spiceTarget.transform.position.y, spiceTarget.transform.position.z + 200); //hide spice target
				transform.position = originalLocation; //return to original position and rotation on shelf
				transform.rotation = originalRotation;
			}
			else if(mode == BottleMode.Shaking && transform.eulerAngles.z >= tiltedAngle.z){ //if the bottle needs to be shaken
				spiceCounter = 1; //start the process for spice to come out
				iTween.PunchRotation(gameObject,new Vector3(0f,0f,-50f), .4f); //make the "tap" animation
				//this.transform.eulerAngles = tiltedAngle + new Vector3(0f,0f,-40); //adjust the rotation of the bottle (temporary solution)
			}
        }
        
	}
	
	void ShakeBottle(){
		if (NumSpice < AmountOfSpice) //if spice is remaining in the bottle and the bottle wasn't already tilted down
		{
			instantiateSpice(); //make a spice bit
			spiceCounter++; //increment number of spice bits
			if (spiceCounter > NumSplurt) //once more bits have been created than should be splurted at a time, wait for the bottle to be tilted up before splurting more
			{		           
				//this.transform.eulerAngles = tiltedAngle; //reset the angle of the bottle so that it can continue to be tapped
				spiceCounter = 0; //reset number of spice bits splurted
				Debug.Log(NumSpice);
			}
		}
		else{
			this.transform.eulerAngles = tiltedAngle + new Vector3(0f,0f,-40f); //have the bottle be tilted further to note this
		}
	}
	
/*    void OnGUI() //set down the bottle
    {
        Event e = Event.current;
        if (e.button == 0 && e.isMouse)
		{
            if (selected && mode == BottleMode.Shaking)
            {
                selected = false;
                spiceTarget.transform.position = new Vector3(spiceTarget.transform.position.x, spiceTarget.transform.position.y, spiceTarget.transform.position.z - 20); //hide spice target
                transform.position = originalLocation; //return to original position and rotation on shelf
                transform.rotation = originalRotation;
				Debug.Log("back on shelf");
            }
        }

    }
*/  

}
