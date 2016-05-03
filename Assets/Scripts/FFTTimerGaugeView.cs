using UnityEngine;
using System.Collections;

public class FFTTimerGaugeView : MonoBehaviour
{
    public Shader OutlineShader;
    public float Under
    {
        get { return _under; }
        set { _under = value; UpdateDisplay(); }
    }
    [SerializeField]
    private float _under = 0.33f;
    public float Even
    {
        get { return _even; }
        set { _even = value; UpdateDisplay(); }
    }
    [SerializeField]
    private float _even = 0.33f;
    public float Over
    {
        get { return _over; }
        set { _over = value; UpdateDisplay(); }
    }
    [SerializeField]
    private float _over = 0.33f;
    public float Border
    {
        get { return _border; }
        set { _border = value; UpdateDisplay(); }
    }
    [SerializeField]
    private float _border = 0.03f;
	public float Peak
    {
        get { return _peak; }
        set { SetPeak(value); UpdateDisplay(); }
    }
    [SerializeField]
    private float _peak = 0;

    public GameObject Display;
    public Transform Left;
    public Transform Right;
    public ProgressBarIndicatorController Indicator;
    public FFTTimerBasicView TimerDisplay;
    public FFTTimerCircularIndicatorView CircularView;

    //TimerDisplay Variables
    public Color IndicatorColor
    {
        get
        {
            return TimerDisplay.IndicatorColor;
        }
        set
        {
            TimerDisplay.IndicatorColor = value;
        }
    }
    public float CurrentTime
    {
        get
        {
            return TimerDisplay.CurrentTime;
        }
        set
        {
            TimerDisplay.CurrentTime = value;
            if (CircularView != null)
            {
                CircularView.PercentFull = value - Mathf.Floor(value);
            }
        }
    }

    public float IndicatorPosition
    {
        get
        {
            return _indicatorPosition;
        }
        set
        {
            _indicatorPosition = Mathf.Clamp(value, 0, 1);
            Indicator.Position = _indicatorPosition;
        }
    }
    private float _indicatorPosition = 0.0f;


    public Vector3 DisplayScale
    {
        get { return _displayScale; }
        set
        {
            if (value != _displayScale)
            {
                Display.transform.localScale = value;
                _displayScale = value;
            }
        }
    }
    [SerializeField]
    private Vector3 _displayScale = new Vector3(1, 1, 1);

    public float tempIndicator = 0.0f;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Border = 0.05f;
        tempIndicator += 0.01f;
        if (tempIndicator > 1)
            tempIndicator = 0;
        IndicatorPosition = tempIndicator;
        */


    }

    public void InitializeDisplay()
    {
        Display.GetComponent<Renderer>().material = new Material(OutlineShader);
        TimerDisplay.ColorText = false;
    }

    public void UpdateDisplay()
    {
        Display.GetComponent<Renderer>().material.SetVector("_Stripe", new Vector4(Under, Even, Over, Border));
        Display.GetComponent<Renderer>().material.SetFloat("_Outline", Border);
		Display.GetComponent<Renderer>().material.SetFloat("_PeakValue", Peak);
    }

    void UpdateDisplay(Vector4 update, float peakValue)
    {
        Display.GetComponent<Renderer>().material.SetVector("_Stripe", update);
        Display.GetComponent<Renderer>().material.SetFloat("_Outline", update.w);
		Display.GetComponent<Renderer>().material.SetFloat("_PeakValue", peakValue);
    }

    public void SetDisplayParameters(Vector3 parameters)
    {
        Under = parameters.x;
        Even = parameters.y;
        Over = parameters.z;
    }
	
	void SetPeak(float peakAsPercentageOfGreenSection)
	{
		float reticulatedPeak = peakAsPercentageOfGreenSection / 100;
		reticulatedPeak = Under + reticulatedPeak * Even;
		_peak = reticulatedPeak;
	}

}
