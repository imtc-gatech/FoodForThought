using UnityEngine;
using System.Collections;

public class FFTStar : MonoBehaviour {

    public static string STAR_ID = "star";

    public enum StarState
    {
        Empty = 0,
        Half = 1,
        Full = 2,
        HalfOutline = 3,
        HalfWithFullOutline = 4,
        FullOutline = 5
    }

    public GameObject Empty;
    public GameObject Half;
    public GameObject Full;

    Color activeFillColor;
    Color activeOutlineColor;
    Color passiveFillColor;
    Color passiveOutlineColor;

    RageSpline fullSpline;
    RageSpline emptySpline;

    public bool Ready
    {
        get
        {
            return !(Empty == null || Half == null || Full == null);
        }
    }

    public bool ColorReady
    {
        get { return !(activeFillColor == null || activeOutlineColor == null || passiveFillColor == null || passiveOutlineColor == null); }
    }

    public StarState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (!Ready)
            {
                GrabStarStates();
                GetColors();
            }

            if (_state != value)
            {
                SwitchStarState(_state, false);
                SwitchStarState(value, true);

                _state = value;
            }
            
        }
    }
    [SerializeField]
    private StarState _state;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GrabStarStates()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.name.StartsWith(STAR_ID))
            {
                if (child.gameObject.name.EndsWith("Empty"))
                {
                    Empty = child.gameObject;
                }
                else if (child.gameObject.name.EndsWith("Full"))
                {
                    Full = child.gameObject;
                }
                else if (child.gameObject.name.EndsWith("Half"))
                {
                    Half = child.gameObject;
                }
            }
        }
        GetColors();
        InitializeDisplay();
        SwitchStarState(State, true);
    }

    void InitializeDisplay()
    {
        Empty.SetActiveRecursively(false);
        Half.SetActiveRecursively(false);
        Full.SetActiveRecursively(false);
    }

    void SwitchStarState(FFTStar.StarState display, bool state)
    {
        if (!Ready)
        {
            GrabStarStates();
        }
        switch (display)
        {
            case StarState.Empty:
                Empty.SetActiveRecursively(state);
                break;
            case StarState.Half:
                Half.SetActiveRecursively(state);
                break;
            case StarState.Full:
                Full.SetActiveRecursively(state);
                break;
            case StarState.HalfOutline:
                SetHalfOutlineOnly(state);
                Half.SetActiveRecursively(state);
                break;
            case StarState.HalfWithFullOutline:
                SetFullOutlineWithHalf(state);
                Half.SetActiveRecursively(state);
                break;
            case StarState.FullOutline:
                SetFullOutlineOnly(state);
                Empty.SetActiveRecursively(state);
                break;
        }
    }

    public void GetColors()
    {
        if (Ready)
        {
            //Full.SetActiveRecursively(true);
            //Half.SetActiveRecursively(true);
            //Empty.SetActiveRecursively(true);
            fullSpline = Full.transform.FindChild("<Path0>").GetComponent<RageSpline>();
            emptySpline = Empty.transform.FindChild("<Path0>").GetComponent<RageSpline>();
            activeFillColor = fullSpline.GetFillColor1();
            activeOutlineColor = fullSpline.GetOutlineColor1();
            passiveFillColor = emptySpline.GetFillColor1();
            passiveOutlineColor = emptySpline.GetOutlineColor1();
        }
    }

    void SetFullOutlineWithHalf(bool state)
    {
        Half.SetActiveRecursively(true);
        RageSpline[] splines = Half.GetComponentsInChildren<RageSpline>();
        foreach (RageSpline rs in splines)
        {
            if (rs.gameObject.name == "<Path1>")
            {
                if (state)
                    rs.SetOutlineColor1(activeOutlineColor);
                else
                    rs.SetOutlineColor1(passiveOutlineColor);
                rs.RefreshMesh(false, false, false);
            }
        }
        Half.SetActiveRecursively(false);
    }

    void SetFullOutlineOnly(bool state)
    {
        Empty.SetActiveRecursively(true);
        RageSpline[] splines = Empty.GetComponentsInChildren<RageSpline>();
        foreach (RageSpline rs in splines)
        {
            if (rs.gameObject.name == "<Path0>")
            {
                if (state)
                    rs.SetOutlineColor1(activeOutlineColor);
                else
                    rs.SetOutlineColor1(passiveOutlineColor);
                rs.RefreshMesh(false, false, false);
            }
        }
        Empty.SetActiveRecursively(false);
    }

    void SetHalfOutlineOnly(bool state)
    {
        //if (!ColorReady)
        //    GetColors();
        Half.SetActiveRecursively(true);
        RageSpline[] splines = Half.GetComponentsInChildren<RageSpline>();
        foreach (RageSpline rs in splines)
        {
            if (rs.gameObject.name == "<Path0>")
            {
                if (state)
                    rs.SetFillColor1(passiveFillColor);
                else
                    rs.SetFillColor1(activeFillColor);
                rs.RefreshMesh(false, false, false);
            }
        }
        Half.SetActiveRecursively(false);
    }
}
