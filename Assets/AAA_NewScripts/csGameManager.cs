using UnityEngine;
using System;
using System.Collections;

public class csGameManager : MonoBehaviour {

	// Note:	The game manager is a real singleton.  Never make one "on purpose",
	//			just call what you need via csGameManager.game and let it do its thing.
	//
	//			Due to Unity oddities (MonoBehavior), the Main Camera must hold an
	//			prefab reference to the game manager.

	private string LEVEL_FOLDER = "/LevelData/Experimental";

	public enum GameState {
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

	static private csGameManager _game;

	static public csGameManager game {
		get {
			if (!_game) {
				_game = Instantiate(Camera.main.GetComponent<csMainCamera>().prefabGameManager);
			}
			return _game;
		}
	}


	// Instance code begins here.

	public csSpriteManager sprites;

	public string[] ExternalLevelSources;
	public int CurrentLevelNumber = 0;

	private GameState gameState = GameState.Planning;
	

	void Awake () {
		sprites = GetComponent<csSpriteManager> ();
		BuildExternalLevelList ();
		DontDestroyOnLoad (this);
	}

	public void WakeUp() {
		// Temporary hack to force instantiation of singleton immediately.
	}

	private void BuildExternalLevelList() {
		string navigationPath;
		
		if (Application.isEditor)
			navigationPath = Application.dataPath; //use levels from Assets project folder if in Editor mode
		else
			navigationPath = Application.dataPath + "/Raw";
		
		navigationPath += LEVEL_FOLDER;
		
		string[] levelSources = System.IO.Directory.GetFiles(navigationPath, "*.xml");
		Array.Sort(levelSources);

		ExternalLevelSources = levelSources;
	}

	public string GetLevelDataFromFile(int levelNumber) {
		if (ExternalLevelSources != null && ExternalLevelSources.Length > levelNumber) {
			return ExternalLevelSources[levelNumber];	
		}
		return "";
	}
	
	public string GetLevelFilename(int levelNumber)
	{
		string currentPath = GetLevelDataFromFile(levelNumber);
		string currentFile = currentPath.Substring(currentPath.LastIndexOf('/'));
		currentFile = currentFile.Trim('/');
		return currentFile;
	}

	public void LoadLevelRelative (int offset) {
		CurrentLevelNumber += offset;

		if ((CurrentLevelNumber >= ExternalLevelSources.Length) || (CurrentLevelNumber < 0)) {
			CurrentLevelNumber = 0;
		}

		Debug.Log ("Loading level " + CurrentLevelNumber + ".");
		Application.LoadLevel ("LevelScene");
	}

	public void LoadLevelAbsolute (int level) {
		CurrentLevelNumber = level;

		if ((CurrentLevelNumber >= ExternalLevelSources.Length) || (CurrentLevelNumber < 0)) {
			CurrentLevelNumber = 0;
		}
		
		Debug.Log ("Loading level " + CurrentLevelNumber + ".");
		Application.LoadLevel ("LevelScene");
	}

	public GameState getGameState() {
		return gameState;
	}

	public void setGameState (csGameManager.GameState newGameState) {
		// Logic to control game state transitions goes here.
		gameState = newGameState;

		// Outbound logic to trigger events on gameplay state changes goes here.
	}

}
