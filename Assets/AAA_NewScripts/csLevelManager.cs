using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

public class csLevelManager : MonoBehaviour {

	// Cached references for the sake of other objects, which will only have
	// a cached reference to the level manager.
	public csGameManager gameManager;
	public csTweenManager tweenManager;
	public csSpriteManager spriteManager;
	public csCounter counter;
	public csKitchen kitchen;

	// Public level information.
	public string levelTitle = "";
	public string levelAuthor = "Author";
	public csCustomer.CustomerName levelCustomer = csCustomer.CustomerName.Monkey;
	public string levelDescription = "Ideally, there would have been some flavor text here.";

	public bool freshnessEnabled = false;
	public float freshnessDelay = 10.0f;
	public float freshnessDecay = 1.0f;
	public float freshnessMaxPenalty = 2.0f;

	public bool isTimedLevel = false;
	public float timeLimit = 30.0f;


	// Private attributes used to run the level.
	private csLevelTimer levelTimer;
	private csIntroScreen introScreen;
	private csRecipeLoader recipeLoader;

	private Vector3 bitBucket;

	private string levelFolder = "/LevelData/Experimental";

	private int levelNumber = 0;
	

	
	void Awake () {
		levelTimer = GameObject.Find("LevelTimer").GetComponent<csLevelTimer> ();
		gameManager = GameObject.Find ("GameManager").GetComponent<csGameManager> ();
		tweenManager = Camera.main.GetComponent<csTweenManager> ();
		spriteManager = Camera.main.GetComponent<csSpriteManager> ();
		counter = GameObject.Find ("Counter").GetComponent<csCounter> ();
		kitchen = GameObject.Find ("Kitchen").GetComponent<csKitchen> ();

		introScreen = GameObject.Find ("IntroScreen").GetComponent<csIntroScreen> ();
		recipeLoader = Camera.main.GetComponent<csRecipeLoader> ();

		bitBucket = new Vector3 (-3 * Screen.width, -3 * Screen.height, 0);
	}


	void Start () {
		BuildExternalLevelList ();

		levelNumber = gameManager.CurrentLevelNumber;

		Debug.Log (gameManager.GetLevelDataFromFile (levelNumber));
		recipeLoader.LoadRecipeXML (gameManager.GetLevelDataFromFile (levelNumber));
		counter.FinalizeCounterPrep ();

		introScreen.PopulateIntroScreen (levelTitle, levelDescription, levelCustomer);

		// TEMP
		isTimedLevel = true;
		timeLimit = 20.0f;
		if (isTimedLevel) {
			levelTimer.setTimer (timeLimit);
		}
	}


	void Update () {
	
		if (Input.GetMouseButtonDown (0)) {
			// New user touch this frame.
			switch (gameManager.getGameState()) {

			case csGameManager.GameState.Planning:
				// Remove intro screen.  Set game state to GameplayPending.
				introScreen.HideIntroScreen();
				gameManager.setGameState(csGameManager.GameState.GameplayPending);
				break;

			case csGameManager.GameState.GameplayPending:
				// Start level timer.
				if (isTimedLevel) {
					levelTimer.startTimer();
				}
				gameManager.setGameState(csGameManager.GameState.Gameplay);
				break;

			}
		}
	}
	
	private void BuildExternalLevelList()
	{

		string navigationPath;
		
		if (Application.isEditor)
			navigationPath = Application.dataPath; //use levels from Assets project folder if in Editor mode
		else
			navigationPath = Application.dataPath + "/Raw";
		
		navigationPath += levelFolder;
		
		string[] levelSources = System.IO.Directory.GetFiles(navigationPath, "*.xml");
		Array.Sort(levelSources);

		// And push it to the Game Manager, so we only load it once per run.
		gameManager.ExternalLevelSources = levelSources;
	}

	public Vector3 getBitBucket() {
		return bitBucket;
	}

	public void FinishLevel() {
		// Triggered by counter when all dishes are complete.
		Debug.Log ("Level is complete.");
		levelTimer.stopTimer ();

		StartCoroutine(LoadNextLevelWithDelay(1.0f));
	}

	private IEnumerator LoadNextLevelWithDelay (float delay) {
		yield return new WaitForSeconds(delay);
		gameManager.LoadLevelRelative (1);
	}

}
