using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class FFTGameManager : MonoBehaviour {
	
	#region Main Variables
	
	public enum BuildMode
	{
		EditorMode=0,
		DeveloperMode=1,
		StudyMode=2,
		ReleaseMode=3
	}
	
	public BuildMode GameBuildMode = BuildMode.EditorMode;
	
	bool useAspectLock = false;

    public bool AlwaysLogData = true;
	public bool LogActions = true;
	public bool UseDetailedActionLogs = false;
	
	public bool UseExternalLevels = true; 
	//setting to true will load levels from external directory and ignore scene sources
	
	public int LevelsAttempted = 0;
	public int LevelsCompleted = 0;
	
	public FFTDataManager DataManager;
	
	public bool UseDebugLogDirectory = true;
	
	public bool AllowRecipeStartOnDishSelect = false;
	//this allows the user to skip the recipe card by clicking on a dish on the counter, rather than using the check box
	
	public bool GameplayArmed = false;
	
	public bool ApplicationEnding = false;

    private static FFTGameManager _instance = null;
    public static FFTGameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    /// <summary>
    /// Type to describe the current state of the game.
    /// </summary>
    public enum GameState
    {
		Setup,
        TitleScreen,
        RecipeSelect,
        GameplayPending,
		Planning,
        Gameplay,
        GameplayFinished,
        ResultsWait,
        Results,
		NextLevelPending
    }
	

    /// <summary>
    /// Current location of the main camera. Camera moves when this value is changed.
    /// </summary>
    public FFTCameraManager.Target CameraPosition
    {
        get { return _cameraPosition; }
        set
        {
            if (value != _cameraPosition)
            {
                //FFTCameraManager.MoveCamera(value);
                _cameraPosition = value;
            }
        }
    }
    private FFTCameraManager.Target _cameraPosition;

    /// <summary>
    /// The current state of the game (menu, gameplay, etc).
    /// </summary>
    public GameState State
    {
        get { return _state; }
        set { _state = value; }
    }
    private GameState _state;

    /// <summary>
    /// Current Level being executed by the GameManager. Allows for paths to the level to be saved with the scene.
    /// </summary>
    public FFTLevel CurrentLevel
    {
        get { return _currentLevel; }
    }

    private FFTLevel _currentLevel;

    public FFTWindowShade WindowShade
    {
        get
        {
            if (_windowShade == null)
            {
                GameObject newWindowShade = GameObject.Find("WindowShade");
                if (newWindowShade == null)
                {
                    //add windowshade prefab
                    newWindowShade = GameObject.Instantiate(Resources.Load("MainGamePrefabs/WindowShade", typeof(GameObject)) as GameObject) as GameObject;
                    //newWindowShade.transform.parent = Camera.mainCamera.transform;
                }
                _windowShade = newWindowShade.GetComponent<FFTWindowShade>();
                _windowShade.gameObject.transform.parent = Camera.main.transform;
            }
            return _windowShade;
        }
    }
    [SerializeField]
    private FFTWindowShade _windowShade;

    public GUIText DebugHUD
    {
        get
        {
            if (_debugHUD == null)
            {
                GameObject newHUD = GameObject.Find("DebugHUD");
                if (newHUD == null)
                {
                    //add debugHUD prefab
                    newHUD = GameObject.Instantiate(Resources.Load("MainGamePrefabs/DebugHUD", typeof(GameObject)) as GameObject) as GameObject;
                    //newHUD.transform.parent = Camera.mainCamera.transform;
                    _debugHUD = newHUD.GetComponent<GUIText>();
                    _debugHUD.transform.position = new Vector3(0, 1, 0);
                }
                _debugHUD = newHUD.GetComponent<GUIText>();
                _debugHUD.gameObject.transform.parent = transform.root;
                WindowShade.HUD = _debugHUD;
            }
            return _debugHUD;
        }
    }
    [SerializeField]
    private GUIText _debugHUD;

    public GameObject RootObject;

    public FFTCounter Counter;
    public FFTKitchen Kitchen;

    public MG_SceneController MinigameManager;
    public MG_MinigameHolder MinigameContainer;

    public FFTScore CurrentScore;
	
	public FFTGameIntroScreen IntroScreen;
	
	public FFTStationOcclusionView Shadow;
	
	public bool UseShadow = true;

    public FFTResultsScreen ResultScreen;
	
	public FFTDishDrawer Drawer;
	
	bool MinigamesLoaded = false;

	private bool bFirstTap = true;

    public bool LoadLevel
    {
        get
        {
            return false;
        }
        set
        {
            if (value == true)
            {
                if (CurrentLevel == null)
                    AssignLevelInstance();
				
				if (UseExternalLevels)
				{
					LoadXMLLevel(CurrentLevel.GetCurrentLevelPathFromExternalList());
				}
				else
				{
					if (CurrentLevel.LevelSources != null && CurrentLevel.LevelSources.Length > 0 && CurrentLevel.CurrentLevelSource != null)
	                    LoadXMLLevel(CurrentLevel.CurrentLevelSource);
	                else
	                    LoadXMLLevel(CurrentLevel.LevelSource); //legacy to work with older scenes
				}
				LevelsAttempted++;
                
            }
        }
    }

    // These variables are for monitoring the time limit (if used)
    public FFTTimerGlobal GlobalTimer;

    float timeSnapshot;

	//how much time has passed since the level was loaded:
	public float LevelElapsedTimeTotal = 0;
	public float LevelPlanningElapsedTime = 0;
	public float LevelGameplayElapsedTime = 0;
	public float LevelReviewElapsedTime = 0;
	
	//and the global time since the game began
	public float SessionElapsedTime { get { return Time.timeSinceLevelLoad; } }
	
	//DISTRACTION variables for logging
	public FFTDistractionScheduler DistractionScheduler; 
	private const string DISTRACTION_SCHEDULER_NAME = "Distractions"; //Scheduler object MUST be called this for it to work.
	private bool usesDistractions = false;
	

    /*
    public TextAsset LevelPath
    {
        get {return _currentLevel.LevelSource; }
        set
        {
            _currentLevel.LevelSource = value;
            if (LoadLevel)
            {
                LoadXMLLevel(_currentLevel.LevelSource);
            }
        }
    }
    //private TextAsset _levelPath;
     */
	
	#endregion
	
	#region Logging Methods, Variables
	
	public bool InGameplayLoggingRange { get {return (State > FFTGameManager.GameState.Setup);} }
	
	#endregion

    void Awake()
    {
		gameObject.AddComponent<FFTTimeManager>();
        AssignSingletonInstance();
        AssignLevelInstance();

		/*
		if (Screen.width != 1280) {
    		Screen.SetResolution(1280, 720, true);
		}
		*/
		Screen.SetResolution (1024, 768, true);
		
    }

	// Use this for initialization
	void Start () {
		
		if (!Application.isEditor)
		{
			//when compiling, do not use editor mode.
			if (GameBuildMode == BuildMode.EditorMode)
			{
				GameBuildMode = BuildMode.StudyMode;
			}
			
			//makes sure we always have these developer flags enabled on build
			AlwaysLogData = true;
			LogActions = true;
			UseDebugLogDirectory = false;
			
			if (GameBuildMode == BuildMode.ReleaseMode)
				UseDetailedActionLogs = false;
			else
				UseDetailedActionLogs = true;
			
			//NOTE, UseDetailedActionLogs must be set to enable deep logging for participant studies.
			//TODO POO
			
		}
		
        State = GameState.Setup;

        WindowShade.Setup();
		
		DebugHUD.text = "";

		/*
		AspectUtility aspectUtility = Camera.main.gameObject.GetComponent<AspectUtility>();
		if (aspectUtility == null)
		{
			aspectUtility = Camera.main.gameObject.AddComponent<AspectUtility>();
		}
		*/

		if (GameBuildMode != BuildMode.EditorMode)
		{
			gameObject.AddComponent<FFTGameStudySetup>();
			FFTDataManager.ExpandCompressedParticipantLevelProgress();
		}
		else
		{
			State = GameState.GameplayPending;
		}
		
		//DISTRACTION setup
		GameObject distractionSchedulerObject = GameObject.Find (DISTRACTION_SCHEDULER_NAME);
		if (distractionSchedulerObject != null)
		{
			usesDistractions = true;
			//we are using distractions, so assign the reference so we can get the logs later
			DistractionScheduler = distractionSchedulerObject.GetComponent<FFTDistractionScheduler>();
		}
	}
	
	public bool CleanUpAndCollectStudyInput()
	{
		FFTGameStudySetup setup = gameObject.GetComponent<FFTGameStudySetup>();
		CurrentLevel.ParticipantID = setup.ParticipantID;
		CurrentLevel.SessionID = setup.SessionID;
		CurrentLevel.ParticipantGroup = setup.SessionType;
		Debug.Log(CurrentLevel.ParticipantGroup.ToString());
		if (UseExternalLevels)
			CurrentLevel.BuildExternalLevelList();
		Destroy(setup);
		if (useAspectLock)
		{
			AspectUtility utility = Camera.main.gameObject.GetComponent<AspectUtility>();
			if (utility == null)
			{
				utility = Camera.main.gameObject.AddComponent<AspectUtility>();
				utility._wantedAspectRatio = 1.777778f;
			}
			AspectUtility.SetCamera();
		}
		if (LogActions)
			CurrentLevel.OpenLevelLog();
		State = GameState.GameplayPending;
		return true;
	}
	
	#region Cheat Keys
	
	public void ReadCheatKeys()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote)) //advance states (this is generally a bad idea...)
		{
			State += 1;	
		}	
		if (Input.GetKeyDown(KeyCode.Alpha0)) //hack fix for 16:10 resolution
		{
			Camera.main.backgroundColor = Color.black;
			Camera.main.orthographicSize = 114;
		}
		if (Input.GetKeyDown(KeyCode.Alpha9)) //hack fix for 16:9 resolution (default)
		{
			Camera.main.backgroundColor = Color.black;
			Camera.main.orthographicSize = 100;
		}
		if (Input.GetKeyDown(KeyCode.Equals)) //skip to next level immediately with(out) logging
		{
			//TODO: Gracefully handle logging of data here
			CleanUpResultsScreen();
			LoadNewLevel(FFTLevel.LoadType.Next);
		}
		if (Input.GetKeyDown(KeyCode.Minus)) //skip to prev level immediately with(out) logging
		{			
			//TODO: Gracefully handle logging of data here
			CleanUpResultsScreen();
			LoadNewLevel(FFTLevel.LoadType.Previous);
		}
		if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
		{			
			//TODO: Gracefully handle logging of data here
			CleanUpResultsScreen();
			LoadNewLevel(FFTLevel.LoadType.Repeat);
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			
			FFTTimeManager.Instance.GameplayPaused = !FFTTimeManager.Instance.GameplayPaused;
			if (FFTTimeManager.Instance.GameplayPaused)
				Shadow.FadeIn(0);
			else
				Shadow.FadeOut(0);
				
		}
		
		if (Input.GetKeyDown (KeyCode.Comma))
		{
			if (IntroScreen.IsVisible)
			{
				FFTTimeManager.Instance.GameplayPaused = false;
				IntroScreen.HideView();
				Shadow.FadeOut(0.3f);
			}
		}
		
		if (Input.GetKeyDown (KeyCode.Period))
		{
			if (!IntroScreen.IsVisible)
			{
				FFTTimeManager.Instance.GameplayPaused = true;
				IntroScreen.Test();
				IntroScreen.ShowView();
				Shadow.FadeIn(0.3f);
			}
		}
		/*
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (Shadow.IsVisible)
				Shadow.FadeOut(0);
			else
				Shadow.FadeIn(0);
		}
		*/
	}
	
	#endregion
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0) && bFirstTap) {
			bFirstTap = false;
			Destroy(GameObject.Find ("SplashCam"));
		}

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			//TODO: Gracefully handle logging of data here
			Application.Quit();	
		}
		
		if (State != GameState.Setup)
			ReadCheatKeys();
		
		LevelElapsedTimeTotal += Time.deltaTime * FFTTimeManager.Instance.ElapsedTimeScale;
		
        switch (State)
        {
			#region GameState.Setup
			case GameState.Setup: //do NOT return here from end of states. Go back to GameplayPending
					//use Start() for this
				
				break;
			#endregion
			
			#region GameState.GameplayPending
            case GameState.GameplayPending:
				GameplayArmed = false;
				if (MinigamesLoaded)
				{	
					//Clean up any minigames that may have been left behind
					GameObject minigameManager = transform.FindChild("MinigameManager").gameObject;
					if (minigameManager != null)
						FFTUtilities.DestroySafe(minigameManager);
					MinigamesLoaded = false;
				}
				//WindowShade.ResetPosition();
                DebugHUD.text = "This debug text disappears upon load.";
                WindowShade.SetText("Loading...", true);
			
                LoadLevel = true; //this is where the loadLevel method is called.
                WindowShade.Open();
                if (CurrentLevel.TimedLevel)
                {
                    GlobalTimer = GetComponent<FFTTimerGlobal>();
                    if (GlobalTimer != null)
                    {
                        FFTUtilities.DestroySafe(GlobalTimer);
                    }
					GlobalTimer = gameObject.AddComponent<FFTTimerGlobal>();
                    GlobalTimer.UseGlobalTimer = CurrentLevel.TimedLevel;
                    GlobalTimer.Setup(CurrentLevel.TimeLimit);
                    
                    // This now triggers when the first dish is selected.
                    //GlobalTimer.BeginTimer();
                    
                    //Resizing behavior from GlobalTimer being on the right side (deprecated)
                    //Counter.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                    //Vector3 CounterPos = Counter.gameObject.transform.position;
                    //CounterPos.y += 9;
                    //Counter.gameObject.transform.position = CounterPos;
                    GlobalTimer.Display.transform.position = new Vector3(-110f, -90f, -117f); //added -100 to z value due to Distraction bar being moved
                }

				GameObject.Find("LevelSelect").transform.FindChild ("Text").GetComponent<Text>().text = CurrentLevel.CurrentExternalLevelIndex.ToString ();
				
				//setup our data logging containers here
				if (LogActions)
				{
					if (DataManager != null)
						FFTUtilities.DestroySafe(DataManager);
					DataManager = gameObject.AddComponent<FFTDataManager>();
					if (UseDetailedActionLogs)
					{
						DataManager.CreateFiles();
						DataManager.CreateInfoFragment();
					}
				} 
			
                FFTCameraManager.MoveCamera(FFTCameraManager.Target.Home);
			
				if (UseShadow)
				{
					Shadow = FFTStationOcclusionView.NewShadow();
					Shadow.SwitchLayer();
				}
				
				GameObject stepCoverGo = GameObject.Instantiate(Resources.Load("MainGamePrefabs/IngredientDrawer", typeof(GameObject)) as GameObject) as GameObject;
				stepCoverGo.transform.parent = Counter.transform;
				stepCoverGo.transform.localPosition = new Vector3(4, 0, -200);
				stepCoverGo.transform.parent = RootObject.transform;
				stepCoverGo.name = "Ingredient Drawer";
				Drawer = stepCoverGo.GetComponent<FFTDishDrawer>(); //-135.159, -79.24971
                Drawer.MoveDrawer = false;
				
				//zero out our time trackers
                LevelElapsedTimeTotal = 0;
                LevelGameplayElapsedTime = 0;
				LevelPlanningElapsedTime = 0;
				LevelReviewElapsedTime = 0;
			
                WindowShade.SetText("Loading... Finished.", true);
                WindowShade.SetText("", false);
			
				if (IntroScreen != null)
					FFTUtilities.DestroySafe(IntroScreen.gameObject);
				GameObject gameIntro = GameObject.Instantiate(Resources.Load("Planning/GameIntroCard", typeof(GameObject)) as GameObject) as GameObject;
				IntroScreen = gameIntro.GetComponent<FFTGameIntroScreen>();
				
				State = GameState.Planning;
                break;
			#endregion
			
			case GameState.Planning:
			
			//use KeyCode.BackQuote to advance or click left mouse button (check FFTDishDrawer)
				LevelPlanningElapsedTime += Time.deltaTime * FFTTimeManager.Instance.ElapsedTimeScale;
				if(Input.GetMouseButtonDown(1))
					Drawer.MoveDrawer = !Drawer.MoveDrawer;
				/*
				if(Input.GetMouseButtonDown(0))
				{
					State = FFTGameManager.GameState.Gameplay;
					Drawer.CleanUpForGameplay();
				}
				*/
				break;

            case GameState.Gameplay:
				
				LevelGameplayElapsedTime += Time.deltaTime * FFTTimeManager.Instance.ElapsedTimeScale;
                if (MinigameManager != null && !MinigameManager.InMinigame)
                {
                    
                }
                // monitor the status of dishes to see if they are finished / update HUD
                if (Counter.RecipeFinished)
                {
                    EndGameplay();
                }
                if (GlobalTimer != null)
                {
                    if (GlobalTimer.Finished) //change this!!!
                    {
                        //State = GameState.GameplayFinished;
                    }
                }
                break;
            case GameState.GameplayFinished:
                State = GameState.ResultsWait;
				Shadow.SwitchLayer();
                timeSnapshot = LevelElapsedTimeTotal;

                if (CurrentLevel.TimedLevel)
                {
                    FFTUtilities.DestroySafe(GlobalTimer);
                }
                WindowShade.Close();
                CollectScores();
                //WindowShade.SetText(CurrentScore.Summary(false) + "\nPress ESCAPE to restart.\n", true);
                
                // collect the scores and get ready to show the results

                if (ResultScreen == null)
                {
                    GameObject Results = new GameObject("Results");
                    ResultScreen = Results.AddComponent<FFTResultsScreen>();
                }

                ResultScreen.SetupDisplay(CurrentScore, CurrentLevel, Counter);

                //DebugHUD.text = "Finished! Let's see how you did...";
			
				LevelsCompleted++;
			
				//logging level progression data
				//(MOVED TO LEVEL LOAD (to allow for logging of review time)

                break;
            case GameState.ResultsWait:
                if (timeSnapshot + 1.0f < LevelElapsedTimeTotal)
                {
					/*
                    if (AlwaysLogData || (CurrentLevel.ParticipantID > 0))
                        FFTRecipeHandlerUtilities.SaveCogGameParticipantLog();
						//FFTRecipeHandlerUtilities.SaveLevyParticipantLogXML();
					*/
                    DebugHUD.text = "";
                    State = GameState.Results;
                    if (ResultScreen.InformationDensity == FFTResultsScreen.InfoDensity.Verbose)
                    {
                        MoveDishesForDisplay();
                    }

                    FFTCameraManager.MoveCamera(FFTCameraManager.Target.Results);
                    WindowShade.Open();
                }

                //FSA-- delay disabled? due to logging delay
                   
                
                break;
            case GameState.Results:
				//TODO: ButtonInterface ForNextRepeatedLevel
				LevelReviewElapsedTime += Time.deltaTime * FFTTimeManager.Instance.ElapsedTimeScale;
                
                // display the score and prompt the user to restart
                if (Input.GetKeyDown(KeyCode.Space)) //ADVANCE LEVEL
                {
					CleanUpResultsScreen();
                    LoadNewLevel(FFTLevel.LoadType.Next);
                }
				if (Input.GetKeyDown(KeyCode.Return)) //REPEAT LEVEL
                {
					CleanUpResultsScreen();
					LoadNewLevel(FFTLevel.LoadType.Repeat);
                }
                break;
			case GameState.NextLevelPending:
				/*
				if (Input.GetKeyDown(KeyCode.Space)) //ADVANCE LEVEL
                {
                    LoadNewLevel(FFTLevel.LoadType.Next);
                }
				if (Input.GetKeyDown(KeyCode.Return)) //REPEAT LEVEL
                {
					LoadNewLevel(FFTLevel.LoadType.Repeat);
                }
                */
				
                break;
        }
	
	}
	
	void LogLevelOverview()
	{
		//logging level progression data
		if (LogActions && UseDetailedActionLogs)
			DataManager.CreateLevelOverviewFragment();
	}
	
	void LogDistractionActions()
	{
		if (usesDistractions)
		{
			DistractionScheduler.LogDistractionsFromCurrentLevel();
		}
	}
	
	public void CleanUpResultsScreen()
	{
		LogLevelOverview();
		
		if (CurrentLevel.TimedLevel) //in case we skip the earlier states, destroy the timer
        {
			if (GlobalTimer != null)
			{
				FFTUtilities.DestroySafe(GlobalTimer);
				GlobalTimer = null;
			}	
        }
		
		if (ResultScreen != null)
        	FFTUtilities.DestroySafe(ResultScreen.gameObject);
		
		if (MinigamesLoaded)
		{
			MinigameManager.CleanUpMinigameControls();
			MinigameContainer.CleanUpForNextLevel();
			FFTUtilities.DestroySafe(MinigameManager.gameObject);
			MinigamesLoaded = false;
		}
		
		if (LogActions)
		{
			if (UseDetailedActionLogs)
				DataManager.CreateActionsFragment();
			FFTUtilities.DestroySafe(DataManager);
		}
	}
		
	
	
	public void LoadNewLevel(FFTLevel.LoadType loadType, int levelToLoad = -1)
	{			
		//WindowShade.Setup();
        WindowShade.ResetPosition();

		if (levelToLoad >= 0) {
			CurrentLevel.SetLevel (levelToLoad);
		} else {
			switch (loadType)
			{
			case FFTLevel.LoadType.Next:
				CurrentLevel.NextLevel();
				break;
			case FFTLevel.LoadType.Previous:
				CurrentLevel.PreviousLevel();
				break;
			case FFTLevel.LoadType.Repeat:
				break;
			}
		}

		State = GameState.GameplayPending;
		
	}

    public bool StationSlotAvailable(FFTStation.Type station)
    {
        return Kitchen.SlotAvailable(station);
    }

    public bool SendDishToStationSlot(FFTDish dish)
    {
        if (Kitchen.SlotAvailable(dish.CurrentDestination))
        {
			if (State == GameState.Planning && AllowRecipeStartOnDishSelect)
			{
				State = GameState.Gameplay;
				Drawer.CleanUpForGameplay();
				if (CurrentLevel.TimedLevel && !GlobalTimer.Running)
                	GlobalTimer.BeginTimer();
			}
            
            return Kitchen.SendDishToStation(dish);
        }

        return false;

    }
	
	public void StartGameplay()
	{
		if (State >= GameState.Gameplay)
			return;
		
		//TODO not destroy IntroScreen, reuse for intervention questions
		
		/*
		if (IntroScreen != null)
			FFTUtilities.DestroySafe(IntroScreen.gameObject);
		*/
				
		if (CurrentLevel.TimedLevel && !GlobalTimer.Running)
			GlobalTimer.BeginTimer();
		
		State = GameState.Gameplay;
		
		Shadow.SwitchLayer();
	}
	
	public void EndGameplay()
	{
		if (State >= GameState.GameplayFinished)
			return;
		
		State = GameState.GameplayFinished;
	}

    void MoveDishesForDisplay()
    {
        int i = 0;
        float dishDisplayWidth = 60f;
        float xOffset = -100f;
        float yPositionRow = 24f;
        foreach (FFTDish dish in Counter.RecipeCard.Dishes)
        {
            GameObject displayDish = GameObject.Instantiate(dish.gameObject) as GameObject;
            displayDish.transform.parent = ResultScreen.gameObject.transform;
            displayDish.transform.localPosition = new Vector3(i * dishDisplayWidth + xOffset, yPositionRow);

            FFTDish newDish = displayDish.GetComponent<FFTDish>();

            if (newDish.FreshnessMeter != null)
                newDish.FreshnessMeter.SetParameters(dish.FreshnessMeter.Parameters);
            newDish.CleanUpForResultDisplay();

            /*
            //TODO: write CleanUpForResultDisplay() method to do this on FFTDish 
             
            Destroy(displayDish.transform.FindChild("Cover").gameObject);
            if (dish.UseFreshness)
                Destroy(displayDish.transform.FindChild("FreshnessMeter").gameObject); //TODO: NEED TO CALCULATE SCORE BEFORE DOING THIS.
             */

            //FUCKSTICK
            Transform[] transforms = displayDish.GetComponentsInChildren<Transform>();
            foreach (Transform child in transforms)
            {
                if (child.name == "Border")
                {
                    if (child.parent.name.StartsWith("#"))
                    {
                        IRageSpline border = child.gameObject.GetComponent<RageSpline>() as IRageSpline;
                        //child.gameObject.AddComponent<MeshCollider>();
                        border.SetPhysics(RageSpline.Physics.MeshCollider);
                        border.RefreshMesh(false, false, true);
                        FFTDishSelectDetailBehaviour dishBehavior = child.gameObject.AddComponent<FFTDishSelectDetailBehaviour>();
                        dishBehavior.Dish = displayDish.GetComponent<FFTDish>();
                    }
                }
            }
            //displayDish.transform.FindChild("Border").gameObject.AddComponent<MeshCollider>();
            //displayDish.transform.FindChild("Border").gameObject.AddComponent<FFTDishSelectDetailBehaviour>();

            //displayDish.AddComponent<MeshCollider>();
            //displayDish.AddComponent<FFTDishSelectDetailBehaviour>(); //.transform.FindChild("Border").gameObject
            
            
            GameObject smallStars = GameObject.Instantiate(Resources.Load("Icons/starRowSmall", typeof(GameObject)) as GameObject) as GameObject;
            FFTStarDisplay starDisplay = smallStars.AddComponent<FFTStarDisplay>();
            starDisplay.UseOutlineCount = dish.FreshnessMeterParameters.UseFreshness;
            starDisplay.StarOutlineCount = CurrentScore.DishTotalStarRating(dish.UID);
            starDisplay.StarCount = CurrentScore.DishTotalStarRatingWithPenalty(dish.UID);
            Debug.Log(CurrentScore.DishTotalStarRating(dish.UID) + "--" + CurrentScore.DishTotalStarRatingWithPenalty(dish.UID));
            starDisplay.Reset();
            smallStars.transform.parent = displayDish.transform;
            smallStars.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            smallStars.transform.localPosition = new Vector3(-39, -7);
            i++;
        }
    }

    /// <summary>
    /// Assigns the instantiated GameManager as the singleton instance, if needed.
    /// </summary>
    public void AssignSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
            AssignLevelInstance();
            if (CurrentLevel.LevelSource != null)
            {
                //LoadLevel = true;
            }
        }

        State = GameState.GameplayPending;
    }

    void AssignLevelInstance()
    {
        _currentLevel = Instance.gameObject.GetComponent<FFTLevel>();
        if (_currentLevel == null)
        {
            _currentLevel = _instance.gameObject.AddComponent<FFTLevel>();
        }
    }

    void CollectScores()
    {
        Counter.FreezeFreshnessForServing();
        CurrentScore = new FFTScore();
        CurrentScore.BuildReport(Counter.RecipeCard);
    }

    public void GrabCurrentLevel()
    {
        AssignLevelInstance();
    }
	
	public void LoadXMLLevel(string xmlLevelPath)
	{
		if (!xmlLevelPath.Contains(".xml"))
        {
			Debug.Log(".xml not found in " + xmlLevelPath);
            return;
        }
        LoadLevel = false;
        bool rootAlreadyExists = (RootObject != null);
        GameObject oldRootObject = RootObject;

        RootObject = FFTRecipeHandlerUtilities.LoadRecipeXML(xmlLevelPath, true);
		//we need the RecipeMaker to get the TimedLevel, etc

        PrepareRootObject(oldRootObject);
		
	}

    public void LoadXMLLevel(TextAsset levelPath)
    {
        if (levelPath == null)
        {
            return;
        }
        LoadLevel = false;
        bool rootAlreadyExists = (RootObject != null);
        GameObject oldRootObject = RootObject;

        RootObject = FFTRecipeHandlerUtilities.LoadRecipeXML(levelPath, true);
		//we need the RecipeMaker to get the TimedLevel, etc

        PrepareRootObject(oldRootObject);

    }
	
	void PrepareRootObject(GameObject oldRootObject)
	{
		if (RootObject == null)
        {
            RootObject = oldRootObject;
            Debug.Log("Error loading recipe.");
            return;
        }

        FFTRecipeMaker recipeMakerScript = RootObject.GetComponent<FFTRecipeMaker>();
        if (recipeMakerScript != null)
        {
            CurrentLevel.TimedLevel = recipeMakerScript.TimedLevel;
            CurrentLevel.TimeLimit = recipeMakerScript.TimeLimit;
            FFTUtilities.DestroySafe(recipeMakerScript); //newRecipe.GetComponent<FFTRecipeMaker>()
        }

        if (oldRootObject != null)
            FFTUtilities.DestroySafe(oldRootObject);

        Debug.Log("New recipe successfully loaded.");

        Counter = RootObject.GetComponentInChildren<FFTCounter>();
        Kitchen = RootObject.GetComponentInChildren<FFTKitchen>();
		
		CurrentLevel.TitleofLevel = Counter.RecipeCard.LevelTitle;

        int i = 0;

        List<FFTStep> minigameSteps = new List<FFTStep>();

        foreach (FFTDish dish in Counter.RecipeCard.Dishes)
        {
            // Initialize Station Indicator Object

            dish.AttachDishStepIndicator();

            //Legacy HUD:

            //dish.AttachStationIndicator();
            //dish.HUD.Destination = (FFTStationIcon.State)dish.CurrentDestination;

            Counter.SlotList[i].Dish = dish;

            i++;

            foreach (FFTStep step in dish.StepDataObjects)
            {
                if (step.Spoiled != null)
                    step.Spoiled.SetActiveRecursively(false);

                if (step.Gameplay == FFTStation.GameplayType.MiniGame)
                {
                    if (!MinigamesLoaded)
                    {
                        GameObject minigameGO = new GameObject();
                        minigameGO.transform.parent = transform;
                        MinigameManager = minigameGO.AddComponent<MG_SceneController>();
                        MinigameContainer = minigameGO.AddComponent<MG_MinigameHolder>();
						MinigamesLoaded = true;
                    }
                    step.MinigameParameters = MG_Minigame.GetParametersFromVariableContainer(dish, step);
                    //Debug.Log("FFTGameManager: " + step.Index + " added for key seed.");
                    MinigameContainer.MinigameSources.Add(step.MinigameParameters);
                    minigameSteps.Add(step); // for later assignment of minigame prefabs after they are synthesized
                }
                

                //TODO: If the step uses a minigame, setup each minigame in the MG_SceneController. When finished, initialize the MG_Paramaters.
                //This involves translating the internal VariableContainer to the MG_Parameters, then passing in the MG_Parameters
                //The SceneController will then handle the minigame instantiation from there when called to do so by the buttons.

            }

        }

        if (MinigameManager != null)
        {
            //synthesize the minigames that have been assigned to the container
            MinigameContainer.InitializeParametersForMinigames();
            foreach (FFTStep step in minigameSteps)
            {
                step.MinigamePrefab = MinigameContainer.GetMinigameToLaunch(step.MinigameParameters.DishID, step.MinigameParameters.StepID);
            }
        }
		
	}
	
	
	void OnApplicationQuit()
	{
		ApplicationEnding = true;
		
		LogLevelOverview();
		
		if (LogActions)
		{
			CurrentLevel.AppendLatestLevelToLog();
			CurrentLevel.CloseLevelLog();
			
		}
	}
	
	
}
