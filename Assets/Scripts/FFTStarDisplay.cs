using UnityEngine;
using System.Collections;

public class FFTStarDisplay : MonoBehaviour {

    public FFTStar[] Stars
    {
        get
        {
            if (_stars == null)
            {
                GrabStarObjects();
            }
            return _stars;
        }
        set
        {
            _stars = value;
        }
    }
    [SerializeField]
    private FFTStar[] _stars;

    public float StarCount
    {
        get
        {
            return _starCount;
        }
        set
        {
            if (_starCount != value)
            {
                _starCount = Mathf.Clamp(value, 0, Stars.Length);
                if (UseOutlineCount)
                    SwitchStarsOutline();
                else
                    SwitchStars();
            }
            
        }
    }
    [SerializeField]
    private float _starCount = 5;

    public float StarOutlineCount
    {
        get
        {
            return _starOutlineCount;
        }
        set
        {
            if (_starOutlineCount != value)
            {
                _starOutlineCount = Mathf.Clamp(value, 0, Stars.Length);
                if (UseOutlineCount)
                    SwitchStarsOutline();
                else
                    SwitchStars();
                    
            }
        }
    }
    [SerializeField]
    private float _starOutlineCount;

    public float VisibleStarDifference
    {
        get { return Mathf.Clamp(StarOutlineCount - StarCount, 0, 5); }
    }

    public bool UseOutlineCount = false;

	// Use this for initialization
	void Start () {
        Reset();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GrabStarObjects()
    {
        _stars = new FFTStar[5];

        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.name.StartsWith(FFTStar.STAR_ID))
            {
                int position;
                if (System.Int32.TryParse(child.gameObject.name[FFTStar.STAR_ID.Length].ToString(), out position))
                {
                    FFTStar star = child.gameObject.GetComponent<FFTStar>();
                    if (star == null)
                        _stars[position - 1] = child.gameObject.AddComponent<FFTStar>();
                    else
                        _stars[position - 1] = star;
                }
                _stars[position - 1].GrabStarStates();
                _stars[position - 1].GetColors();
            }
        }
    }

    public void Reset()
    {
        float starCount = StarCount;
        StarCount = -1;
        StarCount = starCount;
        if (UseOutlineCount)
            SwitchStarsOutline();
        else
            SwitchStars();

    }

    void SwitchStars()
    {
        if (Stars != null)
        {
            _starCount = RoundToNearestHalf(_starCount);

            for (int i = 0; i < Stars.Length; i++)
            {
                if (i < (int)_starCount)
                {
                    //full star
                    Stars[i].State = FFTStar.StarState.Full;
                }
                else if (i < _starCount)
                {
                    //half star
                    Stars[i].State = FFTStar.StarState.Half;
                }
                else
                {
                    //empty
                    Stars[i].State = FFTStar.StarState.Empty;
                }
            }
        }
    }

    void SwitchStarsOutline()
    {
        if (Stars != null)
        {
            _starCount = RoundToNearestHalf(_starCount);
            _starOutlineCount = RoundToNearestHalf(_starOutlineCount);

            //if (_starOutlineCount < _starCount)
            //    _starOutlineCount = _starCount;

            for (int i = 0; i < Stars.Length; i++)
            {
                if (i < (int)_starCount)
                {
                    //full star
                    Stars[i].State = FFTStar.StarState.Full;
                }
                else if (i < _starCount)
                {
                    if (_starOutlineCount > _starCount)
                        //half star with full outline
                        Stars[i].State = FFTStar.StarState.HalfWithFullOutline;
                    else
                        //half star
                        Stars[i].State = FFTStar.StarState.Half;
                }
                else if (i < (int)_starOutlineCount)
                {
                    //full outline
                    Stars[i].State = FFTStar.StarState.FullOutline;
                }
                else if (i < _starOutlineCount)
                {
                    //half outline
                    Stars[i].State = FFTStar.StarState.HalfOutline;
                }
                else
                {
                    //empty
                    Stars[i].State = FFTStar.StarState.Empty;
                }
            }
        }
    }

    void SwitchStars(float value)
    {
        float doubleStarCount = value * 2;
        int doubleStarCountRounded = (int)doubleStarCount;
        _starCount = (float)doubleStarCountRounded / 2;

        int fullStars = doubleStarCountRounded / 2;
        int halfStars = doubleStarCountRounded % 2;


        for (int i = 0; i < Stars.Length; i++)
        {
            if (i < fullStars)
            {
                Stars[i].State = FFTStar.StarState.Full;
            }
            else
            {
                Stars[i].State = FFTStar.StarState.Empty;
            }
        }

        if (halfStars != 0)
        {
            Stars[fullStars].State = FFTStar.StarState.Half;
        }
        
    }

    float RoundToNearestHalf(float value)
    {
        float doubleCount = value * 2;
        int doubleCountRounded = Mathf.CeilToInt(doubleCount);
        return (float)doubleCountRounded/2;
    }
}
