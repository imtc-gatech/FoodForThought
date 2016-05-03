using UnityEngine;
using System.Collections;

public class CookingGaugeBehaviour : MonoBehaviour
{

    //the containing game object's Transform
    private Transform goTransform;
    //the pivot which the game object should rotate around at
    private Transform pivotTransform;

    public Gauge GaugeParent;
    
    public float startingAngle
    {
        get
        {
            return GaugeParent.StartAngle;
        }
    }

    public float rotationRange
    {
        get
        {
            return GaugeParent.EndAngle - GaugeParent.StartAngle;
        }
    }

    public float cookingTime
    {
        get
        {
            return GaugeParent.elapsedTimeInSeconds;
        }
        set
        {
            GaugeParent.elapsedTimeInSeconds = value;
        }
    }

    public float maxCookingTime
    {
        get
        {
            return GaugeParent.maximumTimeInSeconds;
        }
        set
        {
            GaugeParent.maximumTimeInSeconds = value;
        }
    }

    public float cookingSpeed
    {
        get
        {
            return GaugeParent.movementSpeedInSeconds;
        }
        set
        {
            GaugeParent.movementSpeedInSeconds = value;
        }
    }

    public bool activated
    {
        get
        {
            return GaugeParent.activated;
        }
        set
        {
            GaugeParent.activated = value;
        }
    }

    public bool reset
    {
        get
        {
            return GaugeParent.reset;
        }
        set
        {
            GaugeParent.reset = value;
        }
    }

    private float angleChange = 0;
    private float cookingTimeElapsedInSeconds = 0;

    //private float rawAxis;

    public float angle = 0;

    void Awake()
    {
        //get the game object's Transform
        goTransform = this.GetComponent<Transform>();
        
        //get the pivot's Transform
        pivotTransform = transform.parent.FindChild("Pivot");
        //pivotTransform = GameObject.FindWithTag("Pivot").GetComponent<Transform>();


        
        //rotate to starting angle
        RotateArrow(startingAngle);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            cookingTimeElapsedInSeconds = cookingSpeed * Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
            cookingTime += cookingTimeElapsedInSeconds;
            if (cookingTime > maxCookingTime)
            {
                activated = false;
            }
            else
            {
                angleChange = rotationRange * ((cookingTimeElapsedInSeconds / maxCookingTime));
                angle += angleChange;

                RotateArrow(angleChange);

                //Debug.Log(transform.rotation);
            }

        }
        if (reset)
        {
            ResetArrow();
            reset = false;
        }

    }

    private void RotateArrow(float angleToRotate)
    {
        //rotate the player around the pivot
        
        goTransform.RotateAround(pivotTransform.position, Vector3.back, angleToRotate);

        /*
        angle = Quaternion.Angle(goTransform.rotation, pivotTransform.rotation);
        if ((transform.localRotation.w < 0 && transform.localRotation.z < 0) ||
            (transform.localRotation.w > 0 && transform.localRotation.z > 0))
            angle = 360 - angle;
         */

    }

    private void ResetArrow()
    {
        goTransform.RotateAround(pivotTransform.position, Vector3.back, -angle + 360);
        angle = 0;
    }


}