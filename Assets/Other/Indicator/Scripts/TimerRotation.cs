using UnityEngine;
using System.Collections;

public class TimerRotation : MonoBehaviour {

    //the containing game object's Transform
	private Transform goTransform;
	//the pivot which the game object should rotate around at
	private Transform pivotTransform;
	//the velocity to rotate the game object
	public float vel = 2.0f;

    public Vector2 minMaxAngles = new Vector2(180.0f, 0.0f);
    
    // whether the arrow should rotate fully in a circle, if true, ignore the minMaxAngles
    public bool freeRotation = false;

    public float cookingTime = 0;
    //private static float MAXIMUM_COOKING_TIME = 100.0f;


    public bool isActive = false;

    //private float rawAxis;

    public float angle = 0;

	void Awake()
	{
		//get the game object's Transform
		goTransform = this.GetComponent<Transform>();
		//get the pivot's Transform
		pivotTransform = GameObject.FindWithTag("Pivot").GetComponent<Transform>();
		//rotate to starting angle
        if(angle != 0) {
            goTransform.RotateAround(pivotTransform.position, Vector3.back, angle);
		}
	}

	// Update is called once per frame
	void Update ()
	{
        if (isActive)
        {
            RotateArrow();

        }

	}

    private void RotateArrow()
    {
        //rotate the player around the pivot
        //rawAxis = Input.GetAxisRaw("Horizontal");
        //goTransform.RotateAround(pivotTransform.position, Vector3.back, Input.GetAxisRaw("Horizontal")*vel);

        goTransform.RotateAround(pivotTransform.position, Vector3.back, vel);

        angle = Quaternion.Angle(goTransform.rotation, pivotTransform.rotation);
        if ((transform.localRotation.w < 0 && transform.localRotation.z < 0) ||
            (transform.localRotation.w > 0 && transform.localRotation.z > 0))
            angle = 360 - angle;

        //z = ((transform.rotation.z + 1) * 180) - 90;
    }

    private void constrainMovement()
    {
        
        if (!freeRotation)
        {
            if (angle > minMaxAngles.y)
            {
                isActive = false;
                //angle = minMaxAngles.y;
            }
            else if (angle < minMaxAngles.x)
            {
                isActive = false;
                //angle = minMaxAngles.x;
            }
        }
       

        


    }
}