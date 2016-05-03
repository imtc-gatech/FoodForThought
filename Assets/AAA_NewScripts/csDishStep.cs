using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class csDishStep : MonoBehaviour {

	public enum StepGameplayType {
		Empty,
		ElapsedTime,
		MiniGame
	}

	public csStation.StationType stepDestination = csStation.StationType.Cook;
	public int spriteNumber = 0;
	public StepGameplayType gameplayType = StepGameplayType.Empty;

	public float uncookedTime = 5.0f;
	public float cookedTime = 5.0f;
	public float burnedTime = 5.0f;

	public int peakPercent = 50;

	public bool isCookable = true;
	public bool isBurnable = true;
	public bool usesPeakFlavor = false;

	private csLevelManager levelManager;
	private TextMesh stepTime;
	private SpriteRenderer bgSprite;


	void Awake () {
		levelManager = Camera.main.GetComponent<csLevelManager> ();
		stepTime = transform.FindChild ("StepTime").GetComponent<TextMesh> ();
		bgSprite = transform.FindChild ("StepBG").GetComponent<SpriteRenderer> ();

		// Workaround for Unity's lack of editor exposure of sort layer/order except to sprites.
		stepTime.GetComponent<Renderer> ().sortingLayerName = "Slot";
		stepTime.GetComponent<Renderer> ().sortingOrder = 1;
	}
	
	public void ConfigureDisplay () {
		// Called after all data is populated.  Sets up display items.
		if (uncookedTime < 10.0f) {
			stepTime.fontSize = 32;
		}
		stepTime.text = Mathf.CeilToInt (uncookedTime).ToString();
	}


/*
    public int Index;

    public GameObject StepObject;

    public FFTStation.Type Destination;

    public FFTStation.GameplayType Gameplay
    {
        get
        {
            return _gameplay;
        }
        set
        {
            if (_gameplay != value)
            {
                if (value == FFTStation.GameplayType.ElapsedTime)
                {
                    Parameters.ResetValues();
                }
                _gameplay = value;
            }
        }
    }
    [SerializeField]
    private FFTStation.GameplayType _gameplay;

    //OLD MINIGAME CODE
    //public GameObject MiniGame;
    //public FFTMiniGameBase MiniGamePrefab;

    public MG_Minigame.Type MinigameType;

    public MG_Minigame MinigamePrefab;

    public MG_Parameters MinigameParameters;

    public MG_MinigameVariableContainer MinigameVariableContainer; 
    // used by the Unity editor environment to allow for dynamic of editing of variables before being "packed" into the actual parameters on level save/load.

    public FFTStepParameters Parameters;

    public FFTStepReport Result;

    public int VisualStateIndex;

    public GaugeParameters GaugeParametersV1
    {
        get
        {
            GaugeParameters timerParameters = new GaugeParameters();

            if (Gameplay == FFTStation.GameplayType.ElapsedTime)
            {
                timerParameters.Unfinished = Parameters.PercentageUncooked;
                timerParameters.Finished = Parameters.PercentageCooked;
                timerParameters.Overdone = Parameters.PercentageBurned;
                timerParameters.ActionTime = Parameters.TotalSeconds;
                timerParameters.ActionSpeed = 1.0f;
            }

            return timerParameters;
        }
    }

    

    public bool AllowsCooking
    {
        get
        {
            //return (Uncooked != null);
            return true;
        }
    }

    public bool AllowsBurning
    {
        get
        {
            //return (Burned != null);
            return true;
        }
    }

    public bool AllowsSpoiling
    {
        get
        {
            //return (Spoiled != null);
            return true;
        }
    }

    public GameObject Uncooked;
    public GameObject CookedNormal;
    public GameObject Burned;
    public GameObject Spoiled;

    public float ElapsedTime
    {
        get
        {
            return _elapsedTime;
        }
        set
        {
            _elapsedTime = value;

        }

    }

    [SerializeField]
    private float _elapsedTime = 0.0f;

    public FFTStep()
    {
        Parameters = new FFTStepParameters(this);
        MinigameVariableContainer = new MG_MinigameVariableContainer();
    }

    public List<string> AssignGameObject(GameObject alpngStepObject)
    {
        List<string> output = new List<string>();

        if (alpngStepObject == null)
            return output;

        StepObject = alpngStepObject;

        string currentChildName = "";

        bool NormalFound = false;
        bool CookedFound = false;

        foreach (Transform child in alpngStepObject.transform)
        {
            currentChildName = child.name;
            switch (currentChildName)
            {
                case "normal":
                    CookedNormal = child.gameObject;
                    NormalFound = true;
                    break;
                case "spoiled":
                    Spoiled = child.gameObject;
                    break;
                case "uncooked":
                    Uncooked = child.gameObject;
                    break;
                case "burned":
                    Burned = child.gameObject;
                    break;
                case "cooked":
                    CookedNormal = child.gameObject;
                    CookedFound = true;
                    break;
                default:
                    output.Add("Unknown gameObject of name " + currentChildName + " found. Please check prefab.");
                    break;
            }
        }

        if (NormalFound && CookedFound)
        {
            if (AllowsCooking)
            {
                output.Add("Found 'normal' object alongside uncooked/burned objects. Please rename this object to 'cooked'.");
            }
            else
            {
                output.Add("Found 'normal' object alongside 'cooked' object. 'normal' should only be used on an uncooked object.");
            }
        }

        // turn off spoiling until we need it.
        if (Spoiled != null)
        {
            Spoiled.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1, 1, 1, 0);

        }


        return output;

    }

    public void SetVisibility(bool state)
    {
        if (StepObject != null)
            FFTVisualTools.ToggleVisibility(StepObject.transform, state);
    }

    public GaugeParameters ReturnTimerParameters()
    {
        GaugeParameters timerParameters = new GaugeParameters();

        if (Gameplay == FFTStation.GameplayType.ElapsedTime)
        {
            timerParameters.Unfinished = Parameters.PercentageUncooked;
            timerParameters.Finished = Parameters.PercentageCooked;
            timerParameters.Overdone = Parameters.PercentageBurned;
            timerParameters.ActionTime = Parameters.TotalSeconds;
            timerParameters.ActionSpeed = 1.0f;
        }

        return timerParameters;

    }

    public FFTStep Clone()
    {
        FFTStep clone = new FFTStep();
        clone.Index = Index;
        clone.StepObject = StepObject;
        clone.Destination = Destination;
        clone.Gameplay = Gameplay;
        clone.Parameters = new FFTStepParameters(clone);
		//clone.Parameters = Parameters.CopyValues(Parameters);
        clone.Result = Result;
        clone.VisualStateIndex = VisualStateIndex;
        clone.Uncooked = Uncooked;
        clone.CookedNormal = CookedNormal;
        clone.Burned = Burned;
        clone.Spoiled = Spoiled;
        clone.ElapsedTime = ElapsedTime;

        return clone;

    }
    */

}
