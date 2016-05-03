using UnityEngine;
using System.Collections;


public class Gauge : MeterDisplay {

    public bool Visibility
    {
        get
        {
           return _visibility;
        }
        set
        {
            ToggleVisibility(gameObject.transform, value);
            _visibility = value;
        }

    }
    private bool _visibility = true;

    public float StartAngle = -90;
    public float EndAngle = 90;

    public float OuterRadius = 40;
    public float InnerRadius = 20;
    
    public float UndercookedThreshold = 16;
    public float CookedThreshold = 68;
    public float OvercookedThreshold = 16;

    private static float MINIMUM_RANGE = 1.0f;
    private static float GLOBAL_RANGE = 100.0f;

    public static string UNDERCOOKED_NAME = "1_Undercooked";
    public static string COOKED_NAME = "2_Good";
    public static string OVERCOOKED_NAME = "3_Overcooked";

    public Color UndercookedColor;
    public Color CookedColor;
    public Color OvercookedColor;

    private float _lastUndercookedThreshold;
    private float _lastCookedThreshold;
    private float _lastOvercookedThreshold;

    private float _undercookedThreshold;
    private float _cookedThreshold;
    private float _overcookedThreshold;

    private bool refreshNeeded = true;

    private float angleRange;

    private RageShapeCircle[] gaugeCircleSplines;

    private RageShapeCircle undercookedGauge;
    private RageShapeCircle cookedGauge;
    private RageShapeCircle overcookedGauge;

    // TIME VARIABLES

    private float initialMaximumTime;
    private float initialMovementSpeed;

    public float elapsedTimeInSeconds = 0.0f;
    public float maximumTimeInSeconds = 100.0f;
    public float movementSpeedInSeconds = 1.0f;

    public bool activated = false;
    public bool reset = false;

    public Color currentStateColor
    {
        get
        {
            return GetCurrentBaseStateColor();
        }

    }

    void Awake()
    {
        if (gameObject.GetComponent("XYScale") == null)
        {
            gameObject.AddComponent<XYScale>();
        }
        initialMaximumTime = maximumTimeInSeconds;
        initialMovementSpeed = movementSpeedInSeconds;

    }

	// Use this for initialization
	void Start () {
        InitializeMeter();
	}


	
	// Update is called once per frame
	void Update () {

        SnapThresholds();
        CheckForChangedThresholds();

        if (refreshNeeded)
        {
            RefreshVisuals();
        }

        ResetLoop();
        
	}

    public override void RefreshDisplay()
    {
        InitializeMeter();
    }

    public override void ResetDisplay()
    {
        activated = false;
        reset = true;
        elapsedTimeInSeconds = 0.0f;
        movementSpeedInSeconds = initialMovementSpeed;
        ResetLoop();
        RefreshVisuals();

    }

    public void SetGaugeParameters(GaugeParameters parameters)
    {
        UndercookedThreshold = parameters.Unfinished;
        OvercookedThreshold = parameters.Overdone;
        CookedThreshold = parameters.Finished;
        maximumTimeInSeconds = parameters.ActionTime;
        initialMaximumTime = parameters.ActionTime;
        movementSpeedInSeconds = parameters.ActionSpeed;
        initialMovementSpeed = parameters.ActionSpeed;
        ResetDisplay();
    }

    public void InitializeMeter()
    {
        CheckForChangedThresholds();

        gaugeCircleSplines = GetComponentsInChildren<RageShapeCircle>();

        angleRange = EndAngle - StartAngle;

        foreach (RageShapeCircle circle in gaugeCircleSplines)
        {
            circle.StartAngle = StartAngle;
            circle.EndAngle = EndAngle;
            circle.OuterRadius = OuterRadius;
            circle.InnerRadius = InnerRadius;
            if (circle.gameObject.name == UNDERCOOKED_NAME)
            {
                circle.CircleFill = UndercookedColor;
                undercookedGauge = circle;
            }
            else if (circle.gameObject.name == OVERCOOKED_NAME)
            {
                circle.CircleFill = OvercookedColor;
                overcookedGauge = circle;
            }
            else if (circle.gameObject.name == COOKED_NAME)
            {
                circle.CircleFill = CookedColor;
                cookedGauge = circle;
            }
        }

        ResetLoop();
        RefreshVisuals();

    }

    private void CheckIfThresholdHasExceededBounds(ref float threshold)
    {
        if ((threshold + MINIMUM_RANGE + MINIMUM_RANGE) > GLOBAL_RANGE)
        {
            // threshold has exceeded maximum bounds, reduce to maximum possible range
            threshold = GLOBAL_RANGE - MINIMUM_RANGE - MINIMUM_RANGE;

        }

    }

    private void SnapThresholds()
    {
        if (UndercookedThreshold < MINIMUM_RANGE)
            UndercookedThreshold = MINIMUM_RANGE;
        if (CookedThreshold < MINIMUM_RANGE)
            CookedThreshold = MINIMUM_RANGE;
        if (OvercookedThreshold < MINIMUM_RANGE)
            OvercookedThreshold = MINIMUM_RANGE;

    }

    private void CheckForChangedThresholds()
    {
        if (_lastUndercookedThreshold != UndercookedThreshold)
        {
            CheckIfThresholdHasExceededBounds(ref UndercookedThreshold);
            CookedThreshold -= (UndercookedThreshold - _lastUndercookedThreshold);
            refreshNeeded = true;
        }

        else if (_lastOvercookedThreshold != OvercookedThreshold)
        {
            CheckIfThresholdHasExceededBounds(ref OvercookedThreshold);
            CookedThreshold -= (OvercookedThreshold - _lastOvercookedThreshold);
            refreshNeeded = true;
        }

        else if (_lastCookedThreshold != CookedThreshold)
        {
            CheckIfThresholdHasExceededBounds(ref CookedThreshold);
            float cookedDifference = (CookedThreshold - _lastCookedThreshold);
            UndercookedThreshold -= cookedDifference / 2;
            OvercookedThreshold -= cookedDifference / 2;
            refreshNeeded = true;
        }
        if (refreshNeeded)
        {
            CheckForMaximumRangeExceeded();
        }
    }

    private void CheckForMaximumRangeExceeded()
    {
        if ((UndercookedThreshold + CookedThreshold + OvercookedThreshold) != GLOBAL_RANGE)
        {
            CookedThreshold = GLOBAL_RANGE - UndercookedThreshold - OvercookedThreshold;
        }
    }

    private void RefreshVisuals()
    {
        _undercookedThreshold = UndercookedThreshold / GLOBAL_RANGE;
        _cookedThreshold = CookedThreshold / GLOBAL_RANGE;
        _overcookedThreshold = OvercookedThreshold / GLOBAL_RANGE;

        undercookedGauge.EndAngle = StartAngle + angleRange * _undercookedThreshold;
        overcookedGauge.StartAngle = EndAngle - angleRange * _overcookedThreshold;

        foreach (RageShapeCircle circle in gaugeCircleSplines)
        {
            circle.UpdateShape();
            circle.RefreshMesh();
        }
    }

    private void ResetLoop()
    {
        _lastUndercookedThreshold = UndercookedThreshold;
        _lastCookedThreshold = CookedThreshold;
        _lastOvercookedThreshold = OvercookedThreshold;

        refreshNeeded = false;
    }

    private Color GetCurrentBaseStateColor()
    {
        float progressPercentage = elapsedTimeInSeconds / maximumTimeInSeconds; //range 0 to 1
        progressPercentage *= 100; //range 0 to 100

        if (progressPercentage > (UndercookedThreshold + CookedThreshold))
        // in the red
        {
            return overcookedGauge.CircleFill;
        }

        else if (progressPercentage > UndercookedThreshold)
        {
            return cookedGauge.CircleFill;
        }
        else
        {
            return undercookedGauge.CircleFill;
        }
    }


    private void ToggleVisibility(Transform obj, bool state)
    {
        for (int i = 0; i < obj.GetChildCount(); i++)
        {
            if (obj.GetChild(i).GetComponent<Renderer>() != null)
                obj.GetChild(i).GetComponent<Renderer>().enabled = state;
            /*
            if (obj.GetChild(i).guiTexture != null)
                obj.GetChild(i).guiTexture.enabled = state;
            if (obj.GetChild(i).guiText != null)
                obj.GetChild(i).guiText.enabled = state;
            */

            if (obj.GetChild(i).GetChildCount() > 0)
            {
                ToggleVisibility(obj.GetChild(i), state);
            }
        }
    }
}
