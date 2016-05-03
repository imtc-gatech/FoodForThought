using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTDish : MonoBehaviour {

    public static float scaleTime = 0.05f;  
    public static Vector3 DishStepIndicatorPosition = new Vector3(34, 0, -3); //LEFT //new Vector3(-85, 0, -3);

    public FFTFreshnessMeterParameters FreshnessMeterParameters = new FFTFreshnessMeterParameters();
    public FFTFreshnessMeterControl FreshnessMeter;

    public bool IsShadow = false;

    //used to change the scale of the dish when the slot size shrinks (for slot numbers over 3)
    public float CurrentScale
    {
        get { return _currentScale; }
        set
        {
            if (value != _currentScale)
            {
                _currentScale = value;
                ScaleDown();
            }
        }
    }
    private float _currentScale = 1;

    public bool Finished
    {
        get
        {
            return _finished;
        }
        set
        {
            FinishDishSetup(value);
        }
    }
    [SerializeField]
    private bool _finished;

    public static string ID_BORDER = "Border";
    public static string ID_STEP = "Step";

    public GameObject FoodPrefab
    {
        get
        {
            return _foodPrefab;
        }
        set
        {
            _foodPrefab = value;
            //Debug.Log(_foodPrefab.name + " prefab set.");
        }
    }
    [SerializeField]
    private GameObject _foodPrefab;

    public GameObject FoodPrefabClone;

    public FFTSlot HomeCounterSlot;

    public FFTSlot CurrentStationSlot;

    public FFTStationIcon HUD;
    public FFTDishStepsControl HUDVerbose;

    public int ID
    {
        get
        {
            return _dishID;
        }
        set
        {
            _dishID = value;
        }
    }

    public string UID
    {
        get
        {
            return Name + ID.ToString();
        }
    }

    public int CurrentStep
    {
        get
        {
            return _currentStep;
        }
        set
        {
            if (_currentStep != value)
            {
                if (StepDataObjects != null)
                {
					if (_currentStep > (StepDataObjects.Count - 1))
					{
						//the previous step was deleted, so no need to change visibility
					}
					else
					{
                    	StepDataObjects[_currentStep].SetVisibility(false);
                        if (HUDVerbose != null) //&& _currentStep < 4) //currently we only support 4 steps in the new HUD
                        {
                            //HUDVerbose.SetActive(_currentStep + 1, false); 
                            HUDVerbose.PopCard();
                        }
					}
                    if (DisplayFood)
                    {
                        StepDataObjects[value].SetVisibility(true);
                    }
                }
				
               	_currentStep = value;
				
                RefreshDisplayElements();
            }
        }
    }

    public FFTStation.Type CurrentDestination
    {
        get
        {
            return CurrentStepObject.Destination;
        }
    }

    public FFTStep CurrentStepObject
    {
        get
        {
			if (CurrentStep >= StepDataObjects.Count)
			{ 
				return null;
			}
            return StepDataObjects[CurrentStep];
        }
    }

    public bool DisplayBorder
    {
        get
        {
            return _displayBorder;
        }
        set
        {
            if (_displayBorder != value)
            {
                if (Border != null)
                {
                    FFTVisualTools.SetSplineOutline(Border, value);
                }
                _displayBorder = value;

            }
        }
    }

    public bool DisplayBackground
    {
        get
        {
            return _displayBackground;
        }
        set
        {
            if (_displayBackground != value)
            {
                if (Border != null)
                {
                    FFTVisualTools.SetSplineFill(Border, value);
                }
                _displayBackground = value;

            }
        }
    }

    public bool DisplayFood
    {
        get
        {
            return _displayFood;
        }
        set
        {
            if (_displayFood != value)
            {
                if (StepDataObjects != null)
                {
                    StepDataObjects[_currentStep].SetVisibility(value);
                }
                _displayFood = value;
            }
            
        }
    }

    public bool IsEmpty
    {
        get
        {
            if (FoodPrefab == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
    [SerializeField]
    private int _dishID = 0;
    [SerializeField]
    private int _currentStep = 0;
    [SerializeField]
    private bool _displayBorder = true;
    [SerializeField]
    private bool _displayBackground = true;
    [SerializeField]
    private bool _displayFood = true;
    [SerializeField]
    private bool _hideAll = false;

    public string Name
    {
        get
        {
            if (_name.Contains("#"))
            {
                return _name.Substring(1);
            }
            else
                return _name;
        }
    }
    [SerializeField]
    private string _name = "";

    public GameObject Border;

    public List<GameObject> StepGameObjectReferences;

    public GameObject[] MiniGameReferences;

    public List<FFTStep> StepDataObjects;

    public List<string> LoadErrorMessages = new List<string>();

    public GameObject[] FoodVisualStates;
	
	public GameObject BackgroundBox;

	// Use this for initialization
	void Start () {

        LoadErrorMessages = new List<string>();

        /*

        // Load Border and Steps into arrays/object;

        if (FoodPrefabClone != null)
        {
            //Debug.Log(FoodPrefab.name);
            LoadErrorMessages.AddRange(LoadFoodPrefab(FoodPrefab));
        }
          
         */

	}
	
	// Update is called once per frame
	void Update () {
        
	
	}

    public List<string> LoadMiniGame(GameObject miniGamePrefab, int currentStep)
    {
        List<string> errorOutput = new List<string>();
        /* 
        //OLD MINIGAME CODE
        if (StepDataObjects[currentStep].MiniGame != null)
        {
            if (miniGamePrefab == null)
            {
                Object.DestroyImmediate(StepDataObjects[currentStep].MiniGame);
                return errorOutput;
            }
            else if (miniGamePrefab.name != StepDataObjects[currentStep].MiniGame.name)
            {
                //CurrentStepObject.MiniGame.transform.parent = null;
                Object.DestroyImmediate(StepDataObjects[currentStep].MiniGame);
                StepDataObjects[currentStep].MiniGame = Instantiate(miniGamePrefab) as GameObject;
                StepDataObjects[currentStep].MiniGame.name = "(MG) " + StepGameObjectReferences[currentStep].name + ": " + miniGamePrefab.name;
                StepDataObjects[currentStep].MiniGame.transform.position = new Vector3();
                StepDataObjects[currentStep].MiniGame.transform.parent = gameObject.transform;
                return errorOutput;
            }
        }
        else
        {
            if (miniGamePrefab == null)
            {
                //Object.DestroyImmediate(StepDataObjects[currentStep].MiniGame);
                return errorOutput;
            }
            else
            {
                StepDataObjects[currentStep].MiniGame = Instantiate(miniGamePrefab) as GameObject;
                StepDataObjects[currentStep].MiniGame.name = "(MG) " + StepGameObjectReferences[currentStep].name + ": " + miniGamePrefab.name;
                StepDataObjects[currentStep].MiniGame.transform.position = new Vector3();
                StepDataObjects[currentStep].MiniGame.transform.parent = gameObject.transform;
                return errorOutput;
            }
            
        }
        */

        return errorOutput;

    }

    public List<string> LoadMiniGame(GameObject miniGamePrefab)
    {
        List<string> errorOutput = new List<string>();
        /*
        // OLD MINIGAME CODE
        if (CurrentStepObject.MiniGame != null)
        {
            if (miniGamePrefab == null)
            {
                Object.DestroyImmediate(CurrentStepObject.MiniGame);
                return errorOutput;
            }
            else if (miniGamePrefab.name != CurrentStepObject.MiniGame.name)
            {
                //CurrentStepObject.MiniGame.transform.parent = null;
                Object.DestroyImmediate(CurrentStepObject.MiniGame);
                CurrentStepObject.MiniGame = Instantiate(miniGamePrefab) as GameObject;
                CurrentStepObject.MiniGame.name = "(MG) " + StepGameObjectReferences[CurrentStep].name + ": " + miniGamePrefab.name;
                CurrentStepObject.MiniGame.transform.position = new Vector3();
                CurrentStepObject.MiniGame.transform.parent = gameObject.transform;
                return errorOutput;
            }
        }
        else
        {
            CurrentStepObject.MiniGame = Instantiate(miniGamePrefab) as GameObject;
            CurrentStepObject.MiniGame.name = "(MG) " + StepGameObjectReferences[CurrentStep].name + ": " + miniGamePrefab.name;
            CurrentStepObject.MiniGame.transform.position = new Vector3();
            CurrentStepObject.MiniGame.transform.parent = gameObject.transform;
            return errorOutput;
        }
        */

        return errorOutput;

    }

    public List<string> LoadFoodPrefab()
    {
        return LoadFoodPrefab(FoodPrefab);
    }

    public List<string> LoadFoodPrefab(GameObject foodPrefab)
    {
        List<string> errorOutput = new List<string>();

        _currentStep = 0; // reset the internal step whenever you load a new object

        // Destroy the existing food object when a new one is loaded:

        if (FoodPrefabClone != null)
        {
            Border = null;
            StepGameObjectReferences = new List<GameObject>();
            StepDataObjects = new List<FFTStep>();
            Object.DestroyImmediate(FoodPrefabClone);
        }

        // If there is no object loaded, then exit.

        if (foodPrefab == null)
        {
            gameObject.name = "Dish (Empty)";
            return errorOutput;
        }

        // Otherwise, create our clone and assign it as a child to our parent object:

        _name = foodPrefab.name;

        //gameObject.name = "Dish (" + DishID.ToString() + ")" + " " + foodPrefabName;

        FoodPrefabClone = Instantiate(foodPrefab) as GameObject;
        FoodPrefabClone.name = FoodPrefab.name;
        FoodPrefabClone.transform.position = transform.position;
        foreach (Transform child in FoodPrefabClone.transform)
        {
            Vector3 localPosition = child.localPosition;
            localPosition.x = 0;
            localPosition.y = 0;
            if (localPosition.z > 3)
            {
                localPosition.z = 3;
            }
            child.localPosition = localPosition;
        }
        FoodPrefabClone.transform.parent = gameObject.transform;
		
		//add "shaded background object"
		BackgroundBox = GameObject.Instantiate(Resources.Load("UIPrefabs/3DBevels/bevelBox_Ingredient", typeof(GameObject)) as GameObject) as GameObject;
		BackgroundBox.transform.parent = gameObject.transform;
		BackgroundBox.transform.localPosition = new Vector3(1.7f, -1.7f, 0);
		
        if (Border = FoodPrefabClone.transform.FindChild(ID_BORDER).gameObject)
        {
            int foodPrefabChildCount = FoodPrefabClone.transform.GetChildCount() - 1; // The Step Count minus 1 for the "Border" object.
            StepGameObjectReferences = new List<GameObject>(foodPrefabChildCount);
            for (int i=0; i < foodPrefabChildCount; i++)
            {
                StepGameObjectReferences.Add(new GameObject());
            }

            FoodVisualStates = new GameObject[foodPrefabChildCount]; // Preparing to copy the originals to a new array for user visual changes

            foreach (Transform child in FoodPrefabClone.transform)
            {
                string TitleChecker = child.name;
                if (TitleChecker.StartsWith(ID_STEP))
                {
                    int stepNumber;
                    if (int.TryParse(TitleChecker.Substring(ID_STEP.Length), out stepNumber))
                    {
                        FoodVisualStates[stepNumber - 1] = child.gameObject;

                        GameObject tempGO = StepGameObjectReferences[stepNumber - 1]; //hold out "new GameObject" that we made to allow for "array-like" behavior
                        StepGameObjectReferences[stepNumber - 1] = child.gameObject;
                        FFTUtilities.DestroySafe(tempGO);
                    }
                    else
                    {
                        errorOutput.Add("Prefab Step GameObject Not Properly Configured. Step number not extracted.");
                    }
                }
                else if (!TitleChecker.StartsWith(ID_BORDER))
                {
                    errorOutput.Add("Prefab Step GameObject Not Properly Configured. Step prefix not present on non-Border object.");
                }
            }

        }
        else
        {
            errorOutput.Add("Border GameObject not present in Food Prefab.");
        }

        CurrentStep = 0; // Step 1

        

        StepDataObjects = new List<FFTStep>();
        for (int i = 0; i < StepGameObjectReferences.Count; i++)
        {
            StepDataObjects.Add(new FFTStep()); // initialize default objects, which we will then populate
        }

        for (int i = 0; i < StepGameObjectReferences.Count; i++)
        {
            FFTStep newFFTStep = new FFTStep();
            errorOutput.AddRange(newFFTStep.AssignGameObject(StepGameObjectReferences[i]));
            StepDataObjects[i] = newFFTStep;
            
            newFFTStep.SetVisibility(false);

            newFFTStep.Parameters = new FFTStepParameters(StepDataObjects[i]);
            newFFTStep.Gameplay = FFTStation.GameplayType.ElapsedTime;

            //newFFTStep.Parameters.cookingParameters = new GaugeParameters();
            //newFFTStep.Parameters.Destination = new Station.Type();

            newFFTStep.Index = i;
            newFFTStep.VisualStateIndex = i;

            FFTStepVisualState visualState = StepGameObjectReferences[i].AddComponent<FFTStepVisualState>();
            visualState.AssignObjectsFromStep(newFFTStep);
			
			if (i == 0)
			{
				newFFTStep.SetVisibility(true);	
			}
			
            StepDataObjects[i] = newFFTStep;

        }

        // Rename our Dish Object

        if (HomeCounterSlot != null)
        {
            HomeCounterSlot.GetComponent<FFTSlot>().Render = false;
        }

        RefreshDisplayElements();

        return errorOutput;
    }

    public void ToggleVisibility(bool value)
    {
        DisplayBorder = value;
        DisplayBackground = value;
        DisplayFood = value;
    }

    public void AttachStationIndicator()
    {
        GameObject StationIndicatorGO = GameObject.Instantiate(Resources.Load("MainGamePrefabs/StationIconDisplay")) as GameObject;
        StationIndicatorGO.transform.parent = HomeCounterSlot.gameObject.transform;
        StationIndicatorGO.transform.localPosition = new Vector3(-44.33f, 14.65f, -35f);
        HUD = StationIndicatorGO.GetComponent<FFTStationIcon>();
        HUD.Destination = (FFTStationIcon.State)CurrentDestination;
        HUD.gameObject.name = "HUD";
    }

    public void AttachDishStepIndicator()
    {
        //GameObject DishStepRowGO = GameObject.Instantiate(Resources.Load("MainGamePrefabs/UI_DishCards/DishStepControlFull")) as GameObject;
        GameObject DishStepRowGO = new GameObject();
        HUDVerbose = DishStepRowGO.AddComponent<FFTDishStepsControl>();
        DishStepRowGO.transform.parent = HomeCounterSlot.gameObject.transform;
        DishStepRowGO.transform.localPosition = DishStepIndicatorPosition;
        HUDVerbose.gameObject.name = "StepHUD";
        HUDVerbose.SetupHUD(StepDataObjects);
        
        //adding a collider and resizing it to interact with the dish (so this can be clicked by the player)
        //center x=260, size x=830, y=460, z=10
        BoxCollider collider = HUDVerbose.gameObject.AddComponent<BoxCollider>();
        collider.center = new Vector3(260, 0, 0);
        collider.size = new Vector3(830, 460, 10);
        
        DishStepRowGO.transform.localScale = new Vector3(0.1f, 0.1f, 1);
    }

    public void RefreshDisplayElements()
    {
        string goName;

        if (_name.Contains("#"))
            goName = "Dish (" + ID.ToString() + ")" + " " + _name.Substring(1);
        else
            goName = "Dish (" + ID.ToString() + ")" + " " + _name;
        if (StepDataObjects != null && StepDataObjects.Count > 0)
        {
            goName += " Step " + (CurrentStep + 1).ToString() + @"/" + StepDataObjects.Count.ToString();
        }
        gameObject.name = goName;

        if (HUD != null)
        {
            HUD.Destination = (FFTStationIcon.State)CurrentDestination;
        }
        if (HUDVerbose != null)
        {
            //do something

        }
         

    }

    public void DebugFinalResult()
    {
        int i = 1;

        foreach (FFTStep step in StepDataObjects)
        {
            if (step.Result != null)
            {
                Debug.Log(i + ": " + step.Result.Log);
                i++;
            }
        }

    }

    void FinishDishSetup(bool value)
    {
        if (value == true)
        {
            if (HUD != null)
                HUD.Destination = FFTStationIcon.State.Finish;
            if (HUDVerbose != null)
            {
                /*
                if (_currentStep < 4) //currently we only support 4 steps in the new HUD
                    HUDVerbose.SetActive(_currentStep + 1, false);
                 */
                HUDVerbose.PopCard();
                HUDVerbose.SetFinished();
            }
            //StepDataObjects[_currentStep].SetVisibility(false); // leaving true currently so the dish can be "revealed"
            GameObject coveredDish = new GameObject("Cover");
            coveredDish.transform.parent = transform;
            coveredDish.transform.position = transform.position;
            GameObject coverBG = GameObject.Instantiate(Border) as GameObject;
            coverBG.transform.parent = coveredDish.transform;
            coverBG.transform.localPosition = new Vector3(0, 0, 1);
            coveredDish.AddComponent<FFTDishCovered>();

            if (FreshnessMeterParameters.UseFreshness)
            {
                GameObject freshnessMeter = GameObject.Instantiate(Resources.Load("MainGamePrefabs/FreshnessMeter")) as GameObject;
                freshnessMeter.transform.parent = transform;
                freshnessMeter.transform.localPosition = new Vector3(61.6f, -11.8f, -20);
                freshnessMeter.name = "FreshnessMeter";
                FreshnessMeter = freshnessMeter.GetComponent<FFTFreshnessMeterControl>();
                FreshnessMeter.SetParameters(FreshnessMeterParameters);
                
                //TODO: running freshness every time to debug... check whether this should run and when.
                FreshnessMeter.IsRunning = true;
            }

            //DebugFinalResult();
            _finished = value;

        }
    }

    public void CleanUpForResultDisplay()
    {
        //removes cover and freshness meter objects for a dish object clone that is being used for the feedback screen
		GameObject cover = gameObject.transform.FindChild("Cover").gameObject;
        if (cover != null)
			Destroy(cover);
		else
			Debug.Log("Cover not found (which should not happen).");
        if (FreshnessMeterParameters.UseFreshness)
		{
			GameObject freshnessGO = FreshnessMeter.View.gameObject;
			if (freshnessGO != null)
				Destroy(FreshnessMeter.View.gameObject); 
				//TODO(?): NEED TO CALCULATE SCORE BEFORE DOING THIS. /// gameObject.transform.FindChild("FreshnessMeter").gameObject
			else
				Debug.Log("Freshness Meter not found! This should not happen.");
		}
            
    }

    public void ScaleUp()
    { 
        if (!IsShadow)
            iTween.ScaleTo(gameObject, iTween.Hash("x", CurrentScale * 1.2f, "y", CurrentScale * 1.2f, "time", scaleTime));
    }

    public void ScaleDown()
    {
        if (!IsShadow)
            iTween.ScaleTo(gameObject, iTween.Hash("x", CurrentScale, "y", CurrentScale, "time", scaleTime));
    }
	
	public void Shake()
	{
		if (!IsShadow)
			iTween.ShakePosition(gameObject, new Vector3(2, 0, 0), 0.5f);
	}

    /// <summary>
    /// This method returns the material used by the passed in gameobject which follows our food prefabrication structure.
    /// Structure = Parent GameObject (with '#' prefix) - Children labeled with Step1, Step2, etc. 
    /// </summary>
    /// <param name="go">gameobject prefab in the above structure</param>
    /// <param name="state">number of step (1 indexed) to return visual state material from</param>
    /// <returns></returns>
    public static Material ReturnMaterialFromDishPrefab(GameObject go, int state)
    {
        string searchLabel = "Step" + state.ToString();
        //Transform stepTransform = go.transform.FindChildIncludingDeactivated(searchLabel);
		Transform stepTransform = go.transform.FindChild(searchLabel);
        if (stepTransform != null)
        {
            bool currentlyActive = stepTransform.gameObject.active;
            if (!currentlyActive)
                stepTransform.gameObject.SetActiveRecursively(true);

            Transform materialTransform = stepTransform.FindChild("uncooked");

            if (materialTransform == null)
                materialTransform = stepTransform.FindChild("cooked");

            Material outputMaterial = materialTransform.gameObject.GetComponent<Renderer>().material;
            
            if (!currentlyActive)
                stepTransform.gameObject.SetActiveRecursively(false);

            return outputMaterial;
        }
        Debug.Log("ERROR (FFTDish.ReturnMaterialFromDishPrefab): " + searchLabel + " not found in " + go.name + " object.");
        return null;
    }
	
	public void AlignToHomePosition()
	{
		//TODO Align Dish back to its home slot if it has "strayed"
		//Debug.Log("iTween Callback");
		//AlignToHomePosition(0);
	}
	
	public void AlignToHomePosition(float delay)
	{
		AlignToHomePositionYield(delay);
	}
	
	IEnumerator AlignToHomePositionYield(float delay)
	{
		yield return new WaitForSeconds(delay);
		Vector3 pos = gameObject.transform.localPosition;
		pos.x = HomeCounterSlot.transform.localPosition.x - 13; //offset from homeCounterSlot localPosition
		pos.y = HomeCounterSlot.transform.localPosition.y;
		gameObject.transform.localPosition = pos;
	}


}
