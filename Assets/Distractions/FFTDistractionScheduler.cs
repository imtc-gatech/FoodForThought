using UnityEngine;
using System.Collections;

public class FFTDistractionScheduler : MonoBehaviour {
	
	private GameObject distractionManagerGO;
	private FFTDistractionManager distractionManager;
	
	public bool DistractionsEnabled = false;
	// becomes true when the distraction delay has elapsed
	
	public float TriggerProbability = 0.1f; 
	// percent chance of a distraction being triggered
	
	public float FlyProbability = 0.75f;
	// percent chance that the distraction is a fly (otherwise fire)

	public float DistractionDelayInSeconds = 10.0f; 
	// how long before the first distraction, should be informed by difficulty and gameplay length estimation for the recipe
	
	public float TriggerCheckIntervalInSeconds = 1.0f;
	// how often the controller should check to trigger a new distraction
	
	public int MaximumConcurrentDistractions = 5;
	// how many distractions can be in play at once
	
	public int MaximumTotalDistractions = 15;
	// TODO: how many distractions to distribute total. This should probably be implemented as a ratio of total steps in the recipe to the number of distractions (more steps = more distractions), again tuned by difficulty (not implemented)
	
	public float CurrentGameplayTimeSnapshot = 0.0f;
	public float LastTriggerCheckTimeSnapshot = 0.0f;
	
	// Use this for initialization
	void Awake () {
		gameObject.name = "Distractions";
		InitializeDistractionManager();
	}
	
	// Update is called once per frame
	void Update () {
		
		switch (FFTGameManager.Instance.State)
		{
			case FFTGameManager.GameState.Gameplay:
				UpdateGameplay();
				break;
			case FFTGameManager.GameState.GameplayFinished:
				LogDistractionsFromCurrentLevel();
				CleanupAllDistractions();
				break;
		}
		
	}
	
	void InitializeDistractionManager()
	{
		DistractionsEnabled = false;
		CurrentGameplayTimeSnapshot = 0.0f;
		LastTriggerCheckTimeSnapshot = 0.0f;
		distractionManagerGO = GameObject.Instantiate(Resources.Load("Distractions/DistractionManager", typeof(GameObject)) as GameObject) as GameObject;
		distractionManagerGO.transform.parent = gameObject.transform;
		distractionManager = distractionManagerGO.GetComponent<FFTDistractionManager>();	
	}
	
	void Reset()
	{
		Destroy(distractionManagerGO, 2f);
		InitializeDistractionManager();
	}
	
	void CleanupAllDistractions()
	{
		//this does not currently work, need to rewrite in order to handle logging before destroying data
		//distractionManager.ClearAllDistractions();	
		
		Reset ();
	}
	
	void UpdateGameplay()
	{		
		CurrentGameplayTimeSnapshot += Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
		
		if (!DistractionsEnabled && CurrentGameplayTimeSnapshot > DistractionDelayInSeconds)
		{
			DistractionsEnabled = true;
		}
		
		if (DistractionsEnabled)
		{
			if (CurrentGameplayTimeSnapshot - LastTriggerCheckTimeSnapshot >= TriggerCheckIntervalInSeconds)
			{
				//check probability for distractions
				float randNum = Random.Range (0.0f, 1.0f);
				if (randNum < TriggerProbability)
				{
					randNum = Random.Range (0.0f, 1.0f);
					if (randNum < FlyProbability)
					{
						distractionManager.SpawnDistraction(FFTDistraction.Type.Fly);
					}
					else
					{
						FFTStation.Type stationType = (FFTStation.Type)Random.Range (1, 5); //4 types of stations
						distractionManager.SpawnDistraction(FFTDistraction.Type.Fire, stationType);	
					}
				}
				LastTriggerCheckTimeSnapshot = CurrentGameplayTimeSnapshot;	
			}
			
		}
	}
	
	public void LogDistractionsFromCurrentLevel()
	{
		distractionManager.LogActions();
	}
}
