using UnityEngine;
using System.Collections;

public class FFTDishStepCardDisplay : MonoBehaviour {

    public FFTStation.Type State
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != value && Active)
            {
                //switch cards
                
                //Debug.Log("FFTDishStepCardDisplay: " + value + " value of set state");
                Cards[(int)_state - 1].SetActiveRecursively(false);
                Cards[(int)value - 1].SetActiveRecursively(true);
                
            }
            _state = value;
        }
    }
    [SerializeField]
    private FFTStation.Type _state;

    public FFTStation.GameplayType GameplayType = FFTStation.GameplayType.ElapsedTime;

    public MG_Minigame.Type MinigameType = MG_Minigame.Type.Placeholder;

    public FFTMinigameIcon MinigameIcon;

    public string Text
    {
        get { return TextObj.text; }
        set { TextObj.text = value; }
    }

    public GameObject[] Cards;
    public TextMesh TextObj;

    public float Seconds = 30f;

    public bool HasData = false;

    public bool Active
    {
        get { return _active; }
        set
        {
            Cards[(int)_state - 1].SetActiveRecursively(value);
            //TextObj.gameObject.SetActiveRecursively(value);
            _active = value;
        }
    }
    [SerializeField]
    private bool _active = true;

    public int CharacterSizeTwoDigit = 13;
    public int CharacterSizeThreeDigit = 9;
    public float CharacterSizeMinigameDisplay = 6.5f;

	// Use this for initialization
	void Start () {
        IntializeCards();
        //gameObject.transform.parent.localScale = new Vector3(0.1f, 0.1f, 1f);
		TextObj.transform.localPosition = new Vector3 (TextObj.transform.localPosition.x, TextObj.transform.localPosition.y, -0.5f);
		UpdateSecondsDisplay();
	
	}

    // Update is called once per frame
	void Update () {
		/*
        UpdateSecondsDisplay();
        if (Seconds < 0)
            Active = false;
        */
	}

    void UpdateSecondsDisplay()
    {
        if (GameplayType == FFTStation.GameplayType.ElapsedTime)
        {
            if (Seconds >= 100)
            {
                TextObj.characterSize = CharacterSizeThreeDigit;
            }
            else
            {
                TextObj.characterSize = CharacterSizeTwoDigit;
            }

            int roundedSeconds = Mathf.RoundToInt(Mathf.Clamp(Seconds, 0f, 600f));
            if (roundedSeconds >= 0)
                Text = roundedSeconds.ToString();
            else
            {
                Text = "*";
                TextObj.characterSize = 25;
            }
        }
    }

    void IntializeCards()
    {
        foreach (GameObject go in Cards)
        {
            go.SetActiveRecursively(false);
        }
        if (Active)
            Switch(true);
        if (GameplayType == FFTStation.GameplayType.MiniGame)
        {
            TextObj.gameObject.transform.localPosition = new Vector3(0, -20, -28);
            Color textColor = TextObj.GetComponent<Renderer>().material.color;
            textColor.a = 0.8f;
            TextObj.GetComponent<Renderer>().material.color = textColor;
            TextObj.characterSize = CharacterSizeMinigameDisplay;
            Text = "mini";
        }
    }

    public void Switch(bool state)
    {
        Active = state;
        TextObj.gameObject.SetActiveRecursively(state);
        if (HasData && state)
        {
            
        }
        
    }

    public void Finished()
    {
        IRageSpline spline = transform.FindChild("Background").transform.FindChild("<Path0>").gameObject.GetComponent<RageSpline>() as IRageSpline;
        if (spline != null)
        {
            spline.SetFillColor1(Color.gray); //FFTVisualTools.ReturnColorFrom255Values(0, 127, 0)
            spline.RefreshMesh(false, false, false);

        }
    }

}
