using UnityEngine;
using System.Collections;

//represents a single bit of spice from a spice bottle
public class MGSpiceBit : MonoBehaviour {

    public float MaxLandingDisplacement = 5;
    private Vector3 fall;
    private int frame;
    private bool falling = true, colliding = true;
    private MGSpiceGame b;
	private float secondTick; //used to figure out how much time has passed
	private Vector3 originalPosition; //position where the spicebit starts out
	private Vector3 finalPosition; //bottom of the screen
	// Use this for initialization
	void Start () {
		originalPosition = this.transform.position;
		secondTick = 0;
        fall = new Vector3(0, 0, 5); //spice bit DY (falls downward)
        //b = (MGSpiceGame)FindObjectOfType(typeof(MGSpiceGame)); //reference to main script
		//such a hack, such a hack... (fix for grabbing relative game rather than "first game found")
		b = (MGSpiceGame)transform.parent.parent.gameObject.GetComponent(typeof(MGSpiceGame));
		finalPosition = new Vector3(originalPosition.x, b.CameraLoc.transform.position.y -Screen.height/2, originalPosition.z);

	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < b.CameraLoc.transform.position.y + b.SpiceMinY -20f && colliding) //if the spice collides with the food
        {
            Destroy(gameObject.GetComponent<Rigidbody>()); //remove unneccesary components so they don't take up processing power
            Destroy(gameObject.GetComponent<SphereCollider>());
            colliding = false;
        }
        if (falling)
        {
			secondTick+= Time.deltaTime*FFTTimeManager.Instance.GameplayTimeScale; //add how long has passed since the last frame to the tick
			float movedist = Mathf.Lerp(originalPosition.y, finalPosition.y,secondTick/5); //lerp the distance between the start point and end point over 5 seconds
			this.transform.position = new Vector3(originalPosition.x, movedist, originalPosition.z); //move the object
            //this.transform.Translate(new Vector3(0f,0f,movedist)); //move downwards
        }
        if (this.transform.position.y < (b.CameraLoc.transform.position.y -Screen.height/2)) //if spice goes offscreen
        {
            Destroy(this.gameObject); //destroy it so it doesn't take up processing power
        }
	}

    void OnTriggerEnter(Collider other) //on collide
    {
		
        if (other.name == "spiceArea" || other.gameObject.transform.parent.name == "spiceArea") //if the spice collided with the spiceArea, i.e. with a possible physical location for the spice to exist, i.e. the dish to be spiced
        {
            falling = false; //stop falling
            Destroy(gameObject.GetComponent<Rigidbody>()); //remove unneccesary components
            Destroy(gameObject.GetComponent<SphereCollider>());
            colliding = false;
            this.transform.position = new Vector3(this.transform.position.x + Random.Range(-MaxLandingDisplacement, MaxLandingDisplacement), Random.Range(b.CameraLoc.transform.position.y + b.SpiceMinY, b.CameraLoc.transform.position.y + b.SpiceMaxY), this.transform.position.z); //scatter the spice over the dish
			
            if (this.transform.position.y > b.CameraLoc.transform.position.y + b.SpiceMaxY || this.transform.position.y < b.CameraLoc.transform.position.y + b.SpiceMinY || this.transform.position.x < b.CameraLoc.transform.position.x + b.SpiceMinX || this.transform.position.x > b.CameraLoc.transform.position.x + b.SpiceMaxX) //if the scattered spice lands off the dish
            {
                Destroy(this.gameObject); //remove it
            }
        }
    }
}
