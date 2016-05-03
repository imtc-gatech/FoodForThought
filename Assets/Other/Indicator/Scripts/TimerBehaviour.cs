using UnityEngine;
using System.Collections;

public class TimerBehaviour : MonoBehaviour {

    private TimerRotation timerRotation;
    private RageShapeCircle indicator;
    private IRageSpline indicatorSpline;
    private float angle;

    private Color alertColor = Color.red;
    //private Color cookingColor = Color.green;

    public const int DEFAULT_COOKING_TIME = 360;

    public float cookingTime = DEFAULT_COOKING_TIME;
    public float cookingSpeed = 0.0f;

    public bool isCooking = true;
	public bool isBurning = false;

    private int indicatorUpdateRate = 10;
    private int indicatorUpdateTolerance = 2;

    void Awake()
    {
        timerRotation = (TimerRotation)transform.FindChild("Arrow").GetComponent("TimerRotation");
        indicator = (RageShapeCircle)transform.FindChild("Indicator").GetComponent("Circle");
        indicator.StartAngle = DEFAULT_COOKING_TIME - cookingTime;
        indicatorSpline = (RageSpline)transform.FindChild("Indicator").GetComponent("RageSpline") as IRageSpline;
		timerRotation.vel = cookingSpeed;
		timerRotation.angle = DEFAULT_COOKING_TIME - cookingTime;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isCooking)
        {
            UpdateCooking();
        }
        else if(isBurning)
        {
            UpdateBurning();
        }
	}

    void UpdateCooking()
    {
        indicator.StartAngle = timerRotation.angle;
        UpdateCookingTime();
        if (indicator.EndAngle % indicatorUpdateRate < indicatorUpdateTolerance) {	// Changed startangle to endangle
            UpdateIndicator();
		}
    }

    void UpdateBurning()
    {
        indicator.StartAngle = 0;
        indicator.EndAngle = timerRotation.angle;
        UpdateCookingTime();
        indicatorSpline.SetFillColor1(alertColor);
        if (indicator.EndAngle % indicatorUpdateRate < indicatorUpdateTolerance)
            UpdateIndicator();
    }

    void UpdateCookingTime()
    {
        cookingTime -= cookingSpeed;
        if (cookingTime < 0)
        {
            isCooking = false; //isNowBurning
            indicatorSpline.SetFillColor1(alertColor);
        }
    }

    void UpdateIndicator()
    {
        indicator.UpdateShape();
        indicatorSpline.RefreshMesh(true, false, false);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            cookingTime = DEFAULT_COOKING_TIME;

        }

    }
}
