using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTStationIcon : MonoBehaviour {

    public Color debugPredominantColor;
    public Color debugComplimentaryColor;
    public float saturation = 0.5f;
    public float value = 0;
	public string backgroundObjectName = "Circle";

    public enum State
    {
        Init = 0,
        Chop = 1,
        Cook = 2,
        Prep = 3,
        Spice = 4,
        Finish = 5
    }

    public enum IconPosition
    {
        North,
        South,
        East,
        West
    }

    public enum IconBackground
    {
        Blank
    }

    public enum IconSize
    {
        Standard,
        X2
    }

    public State Destination
    {
        get
        {
            return _destination;
        }
        set
        {
            if (value != _destination)
            {

                _destination = value;
                if (((int)_destination > (int)State.Init))
                {
                    SwapTile();
                }
            }
        }
    }
    private State _destination = State.Init;

    public IconPosition Position
    {
        get { return _position; }
        set
        {
            if (value != _position)
            {
                _position = value;
                MoveTile();
            }
        }


    }
    private IconPosition _position = IconPosition.West;

    public IconSize Size
    {
        get { return _size; }
        set
        {
            if (value != _size)
            {
                _size = value;
                ResizeTile();
            }
        }
    }
    private IconSize _size = IconSize.Standard;

    public bool Render
    {
        get { return _render; }
        set
        {
            if (_render != value)
            {
                if (displayIconTile == null)
                {
                    displayIconTile = CurrentDestinationTransform();
                    backgroundTile = transform.FindChild(backgroundObjectName);
                }
                displayIconTile.gameObject.SetActiveRecursively(value);
                backgroundTile.gameObject.SetActiveRecursively(value);
                _render = value;
            }
        }
    }
    private bool _render = true;

    private Transform displayIconTile;
    private Transform backgroundTile;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SwapTile()
    {
        displayIconTile = CurrentDestinationTransform();
        backgroundTile = transform.FindChild(backgroundObjectName);
        if (backgroundTile == null)
        {
            backgroundTile = gameObject.transform;
        }

        foreach (Transform child in transform)
        {
            if (child == displayIconTile || child == backgroundTile)
            {
                child.gameObject.SetActiveRecursively(Render);
                
                switch (Size)
                {
                    case IconSize.Standard:
                        child.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        break;
                    case IconSize.X2:
                        child.gameObject.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                        break;
                }
                IRageSpline backgroundSpline = backgroundTile.gameObject.GetComponent<RageSpline>() as IRageSpline;
                backgroundSpline.SetOutlineColor1(PredominantColor());
                backgroundSpline.SetOutlineColor2(Color.black);
                backgroundSpline.SetOutlineGradient(RageSpline.OutlineGradient.Default);
                backgroundSpline.SetFillColor1(BackgroundColorGenerator());
                backgroundSpline.RefreshMesh();
            }
            else
            {
                child.gameObject.SetActiveRecursively(false);

            }

            //HACKISH
            if (Destination == State.Finish)
            {
                IRageSpline backgroundSpline = backgroundTile.gameObject.GetComponent<RageSpline>() as IRageSpline;
                backgroundSpline.SetOutlineColor1(Color.black);
                backgroundSpline.SetOutlineColor2(Color.black);
                backgroundSpline.SetOutlineGradient(RageSpline.OutlineGradient.Default);
                backgroundSpline.SetFillColor1(Color.green);
                backgroundSpline.RefreshMesh();
                return;
            }
        }

    }

    /// <summary>
    /// Moves the station icon to one of the cardinal directions around the current object associated with it.
    /// </summary>
    void MoveTile()
    {
        //STUB

    }

    /// <summary>
    /// Resizes the station icon to a number of presets.
    /// </summary>
    void ResizeTile()
    {
        SwapTile();
    }

    Transform CurrentDestinationTransform()
    {
        string stationIconName = "icon" + Destination.ToString();

        Transform displayIconTile = transform.FindChild(stationIconName);

        return displayIconTile;
    }


    Color PredominantColor()
    {
        Color result = Color.white;

        Transform display = CurrentDestinationTransform();

        RageSpline[] splines = display.gameObject.GetComponentsInChildren<RageSpline>();

        Dictionary<Color, int> colors = new Dictionary<Color, int>();

        foreach (RageSpline spline in splines)
        {
            if (colors.ContainsKey(spline.fillColor1))
            {
                colors[spline.fillColor1]++;
            }
            else
            {
                colors.Add(spline.fillColor1, 1);
            }
        }

        if (colors.Count > 0)
        {
            int maxCount = 0;

            foreach (KeyValuePair<Color, int> pair in colors)
            {
                if (pair.Value > maxCount)
                {
                    result = pair.Key;
                    maxCount = pair.Value;
                }
            }
        }

        debugPredominantColor = result;

        return result;

    }

    Color BackgroundColorGenerator()
    {
        Color color = PredominantColor();
        ColorHSV colorHSV = new ColorHSV(color);
        //Debug.Log(colorHSV.ToString());
        //colorHSV.v = (colorHSV.v + value) % 1.0f;
        colorHSV.s = (colorHSV.s + saturation) % 1.0f;
        Color result = colorHSV.ToColor();
        result.a = 1.0f;
        debugComplimentaryColor = result;
        return result;
    }

    

    
}
