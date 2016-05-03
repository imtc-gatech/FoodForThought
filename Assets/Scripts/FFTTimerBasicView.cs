using UnityEngine;
using System.Collections;

public class FFTTimerBasicView : MonoBehaviour {

    public float CurrentTime
    {
        get
        {
            return _currentTime;
        }
        set
        {
            if (value != _currentTime)
            {
                if (value < 0)
                {
                    _currentTime = 0;
                }
                else
                {
                    _currentTime = value;
                    Text.text = DisplayText;
                    gameObject.name = "BasicTimer: " + DisplayText + " remaining";
                }
            }
        }
    }

    private float _currentTime;

    public string DisplayText
    {
        get
        {
            if (ShowMinutes)
            {
                int totalSeconds = (int)Mathf.Ceil(CurrentTime); //(int)CurrentTime; //originally used an int cast to round the number, confusing to user
                int seconds = totalSeconds % 60;
                int minutes = totalSeconds / 60;
                string output = seconds.ToString();
                
                if (seconds < 10)
                {
                    output = "0" + output;
                }
                output = minutes.ToString() + ":" + output;
                return output;
            }
            else
            {
                //int seconds = (int)CurrentTime;
                int seconds = (int)Mathf.Ceil(CurrentTime); //(int)CurrentTime; //originally used an int cast to round the number, confusing to user
                string output = seconds.ToString();
                if (seconds < 10)
                {
                    output = "0" + output;
                }
                if (ShowColon)
                    output = ":" + output;
                else
                    output = " " + output;
                return output;
            }
            
        }
    }

    public Color DefaultColor = Color.white;

    public Color IndicatorColor
    {
        get
        {
            return _indicatorColor;
        }
        set
        {
            if (value != _indicatorColor)
            {
                _indicatorColor = value;
                if (ColorText)
                {
                    if (FFTUtilities.InEditorMode)
                        Text.GetComponent<Renderer>().sharedMaterial.color = _indicatorColor;
                    else
                        Text.GetComponent<Renderer>().material.color = _indicatorColor;
                }
                else
                {
                    if (FFTUtilities.InEditorMode)
                        Text.GetComponent<Renderer>().sharedMaterial.color = DefaultColor;
                    else
                        Text.GetComponent<Renderer>().material.color = DefaultColor;
                }
                if (ColorFrame)
                {
                    Frame.SetOutlineColor1(_indicatorColor);
                    Frame.RefreshMesh();
                }
                else
                {
                    Frame.SetOutlineColor1(DefaultColor);
                    Frame.RefreshMesh();
                }
            }
        }
    }
    private Color _indicatorColor = Color.white;

    public bool ShowMinutes = false;

    public bool ColorText = true;
    public bool ColorFrame = true;
    public bool ShowColon = true;

    public TextMesh Text;
    public RageSpline Frame;

    private Material textMaterial;
	

	// Use this for initialization
	void Awake () {
        if (Text == null)
            Text = gameObject.GetComponentInChildren<TextMesh>();
        if (Frame == null)
            Frame = gameObject.GetComponentInChildren<RageSpline>() as RageSpline;

        //textMaterial = new Material(Text.renderer.material);
	}
	
	// Update is called once per frame
	void Update () {

	}

}
