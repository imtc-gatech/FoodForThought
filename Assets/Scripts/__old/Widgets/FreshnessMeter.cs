using UnityEngine;
using System.Collections;

public class FreshnessMeter : MeterDisplay {

    /*
     * Y = 0 on max (50)
     * Y = -12 on half (25)
     * Y = -19.25 on fifth (10)
     * Y = -24/-25 on minimum (1)
     * Basically half the height of the meter should work.
     */

    public float width = 10;
    public float height = 50;

    public float freshness = 1;
    private float _lastFreshness;

    public float totalTime = 10.0f;
    public float elapsedTimeSpeed = 1.0f;
    public float currentTime;

    private float initializedTime;

    public bool activated = false;

    private Vector3 meterPosition;

    private RageShapeRectangle[] meterComponents;

    private RageShapeRectangle meterDisplay;

	// Use this for initialization
	void Start () {
        InitializeMeter();

        _lastFreshness = freshness;
        currentTime = totalTime;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (activated)
        {
            currentTime -= Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale * elapsedTimeSpeed;
            freshness = currentTime / totalTime;
        }

        if (freshness != _lastFreshness)
        {
            UpdateMeter();
        }

        _lastFreshness = freshness;
	}

    public override void RefreshDisplay()
    {
        //InitializeMeter
        meterComponents = GetComponentsInChildren<RageShapeRectangle>();
        foreach (RageShapeRectangle meter in meterComponents)
        {
            meter.Width = width;
            meter.Height = height;
            meter.UpdateShape();
            meter.RefreshMesh();
            if (meter.gameObject.name == "Meter")
            {
                meterDisplay = meter;
            }
        }

        meterPosition = meterDisplay.transform.localPosition;
        //UpdateMeter
        meterDisplay.Height = height * freshness;
        meterPosition.y = -((height / 2) - (height / 2 * freshness));
        meterDisplay.transform.localPosition = meterPosition;
        meterDisplay.UpdateShape();
        meterDisplay.RefreshMesh();
    }

    public override void ResetDisplay()
    {
        currentTime = totalTime;
        freshness = 1;
        UpdateMeter();
        activated = false;
    }

    private void UpdateMeter()
    {
        meterDisplay.Height = height * freshness;
        meterPosition.y = -((height / 2) - (height / 2 * freshness));
        meterDisplay.transform.localPosition = meterPosition;
        meterDisplay.UpdateShape();
        meterDisplay.RefreshMesh();
    }

    private void InitializeMeter()
    {
        meterComponents = GetComponentsInChildren<RageShapeRectangle>();
        foreach (RageShapeRectangle meter in meterComponents)
        {
            meter.Width = width;
            meter.Height = height;
            meter.UpdateShape();
            meter.RefreshMesh();
            if (meter.gameObject.name == "Meter")
            {
                meterDisplay = meter;
            }
        }

        meterPosition = meterDisplay.transform.localPosition;
    }


}
