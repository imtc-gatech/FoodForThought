using UnityEngine;
using System.Collections;

public class FFTCustomerView : MonoBehaviour {

    public Color Outline
    {
        get
        {
            if (Frame != null)
                return Frame.GetOutlineColor1();
            else
                return DefaultOutline;
        }
        set
        {
            if (Frame != null)
            {
                Frame.SetOutlineColor1(value);
                Frame.RefreshMesh(false, false, false);
            }
        }
    }

    public Color Fill
    {
        get
        {
            if (Frame != null)
                return Frame.GetFillColor1();
            else
                return DefaultFill;
        }
        set
        {
            if (Frame != null)
            {
                Frame.SetFillColor1(value);
                Frame.RefreshMesh(false, false, false);
            }
        }
    }

    public IRageSpline Frame
    {
        get { return transform.FindChild("charBox").GetComponentInChildren<RageSpline>() as IRageSpline; }
    }



    public enum VisualState
    {
        Default = 0,
        Bad = 1,
        Average = 2,
        Good = 3
    }

    public VisualState State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                Refresh();
            }
        }
    }
        
    [SerializeField]
    private VisualState _state;

    [HideInInspector]
    public GameObject[] CharacterFrames;

    public Color DefaultOutline { get { return FFTVisualTools.ReturnColorFrom255Values(40f, 104f, 158f); } }
    
    public Color DefaultFill { get { return FFTVisualTools.ReturnColorFrom255Values(129f, 178f, 226f); } }

	// Use this for initialization
	void Start () {
        Initialize();
	
	}
	
	// Update is called once per frame
	void Update () {
        /* debug code to check state switching
        int flipper = (int)State;
        flipper++;
        if (flipper > 3) flipper = 0;
        State = (VisualState)flipper;
         */
	
	}

    public void Initialize()
    {
        CharacterFrames = new GameObject[4];
        CharacterFrames[0] = transform.FindChild("default").gameObject;
        CharacterFrames[1] = transform.FindChild("bad").gameObject;
        CharacterFrames[2] = transform.FindChild("average").gameObject;
        CharacterFrames[3] = transform.FindChild("good").gameObject;
        Refresh();
    }

    public void Refresh()
    {
        if (CharacterFrames != null)
        {
            foreach (GameObject go in CharacterFrames)
            {
                go.SetActiveRecursively(false);
            }
            CharacterFrames[(int)State].SetActiveRecursively(true);
        }
    }
	
	public void OnDestroy()
	{
		GameObject.Destroy(gameObject);
	}
}
