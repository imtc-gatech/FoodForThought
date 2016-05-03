using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[SerializePrivateVariables]
public class MG_MinigameVariableContainer {

    public MG_Minigame.Type CurrentType
    {
        get { return _currentType; }
        set
        {
            if (value != _currentType)
            {
                initialized = false;
                _currentType = value;
                Reinitialize();
            }

        }
    }
    private MG_Minigame.Type _currentType = MG_Minigame.Type.Placeholder;

    public MG_MinigameVariableContainer()
    {
        CurrentType = MG_Minigame.Type.Placeholder;
        Reinitialize();

        // left here in case we want to do contextual recreation of variables.
    }

    bool initialized = false;

    void Reinitialize()
    {
        if (initialized)
            return;

        switch (CurrentType)
        {
            case MG_Minigame.Type.Placeholder:
                InitPlaceholder();
                break;
            case MG_Minigame.Type.Sorting:
                InitSorting();
                break;
			case MG_Minigame.Type.Spicing:
				InitSpicing();
				break;
        }

        initialized = true;
    }
	
	#region MG_Minigame.Type.Placeholder Setup

    public float Placeholder_DelayToWinInSeconds
    {
        get
        {
            return _placeholder_DelayToWinInSeconds;
        }
        set
        {
            _placeholder_DelayToWinInSeconds = value;
        }
    }
    private float _placeholder_DelayToWinInSeconds;

    public bool Placeholder_UseDelay
    {
        get
        {
            return _placeholder_UseDelay;
        }
        set
        {
            _placeholder_UseDelay = value;
        }
    }
    private bool _placeholder_UseDelay;
	
	    void InitPlaceholder()
    {
        // sets default values for placeholder.
        Placeholder_DelayToWinInSeconds = 5.0f;
        Placeholder_UseDelay = true;
    }
	
	#endregion
	
	#region MG_Minigame.Type.Sorting Setup
	
	public int Sorting_NumVegetables = 10;
	
	void InitSorting()
    {
        InitSorting(10);
    }

    void InitSorting(int numVegetables)
    {
        Sorting_NumVegetables = numVegetables;
    }
	
	#endregion
	
	#region MG_Minigame.Type.Spicing Setup
	
	public int Spicing_NumberOfBottles
	{
		get { return _spicing_NumberOfBottles; }
		set { _spicing_NumberOfBottles = Mathf.Clamp(value, 1, 3); }
	}
	[SerializeField]
	private int _spicing_NumberOfBottles = 2;
	
	public List<MGSpiceGame.SpiceBottle> Spicing_BottleList = new List<MGSpiceGame.SpiceBottle>();

    void InitSpicing()
    {
		InitSpicing(2);
	}
	
	void InitSpicing(int numberOfBottles)
	{
		Spicing_NumberOfBottles = numberOfBottles;
    }
	
	#endregion
	
	#region MG_Minigame.Type.Stirring Setup (NOT DONE)
	
	public int Stirring_NumberOfVegetables = 20;
	public float Stirring_TotalCookingTime = 45f;
	public float Stirring_TimeBeforeNeedingToStir = 0.4f;
	public float Stirring_TimeBeforeEnteringDangerZone = 10f;
	public float Stirring_TimeBeforeFullyBurned = 5f;
	public float Stirring_BurnRecoveryMultiplier = 1.0f;
	
	void InitStirring()
	{
		Stirring_NumberOfVegetables = 20;
		Stirring_TotalCookingTime = 45f;
		Stirring_TimeBeforeNeedingToStir = 0.4f;
		Stirring_TimeBeforeEnteringDangerZone = 10f;
		Stirring_TimeBeforeFullyBurned = 5f;
		Stirring_BurnRecoveryMultiplier = 1.0f;
	}
	
	void InitStirring(float timeLimit)
	{
		Stirring_TotalCookingTime = timeLimit;
	}
	
	#endregion
	
	#region MG_Minigame.Type.Blending Setup
	
	public int Blending_NumIngredients = 0; //leaving this as zero results in a random pick between 2-3 ingredients
	
	public void InitBlending()
	{
		Blending_NumIngredients = 0;
	}
	
	#endregion
	
	#region MG_Minigame.Type.Chopping Setup
	
	public MG2_GameMode Chopping_GameMode = MG2_GameMode.proportionality;
	public int Chopping_MaxCuts = 8;
	
	void InitChopping()
	{
		Chopping_GameMode = MG2_GameMode.proportionality;
		Chopping_MaxCuts = 8;
	}
	
	#endregion
	
    public Dictionary<string, string> GetCurrentVariableDictionary()
    {
        var dict = new Dictionary<string, string>();
        switch (CurrentType)
        {
            case MG_Minigame.Type.Placeholder:
                dict.Add("DelayToWinInSeconds", Placeholder_DelayToWinInSeconds.ToString());
                dict.Add("UseDelay", Placeholder_UseDelay.ToString());
                break;
            case MG_Minigame.Type.Sorting:
                dict.Add("NumVegetables", Sorting_NumVegetables.ToString());
                break;
			case MG_Minigame.Type.Chopping:
				dict.Add("GameMode", Chopping_GameMode.ToString());
				dict.Add("MaxCuts", Chopping_MaxCuts.ToString());
				break;
            case MG_Minigame.Type.Stirring:
				/*
				Stirring_NumberOfVegetables = 20;
				Stirring_TotalCookingTime = 45f;
				Stirring_TimeBeforeNeedingToStir = 0.4f;
				Stirring_TimeBeforeEnteringDangerZone = 10f;
				Stirring_TimeBeforeFullyBurned = 5f;
				Stirring_BurnRecoveryMultiplier = 1.0f;
				 */
				dict.Add("NumVegetables", Stirring_NumberOfVegetables.ToString());
				dict.Add("TotalTimeToCook", Stirring_TotalCookingTime.ToString());
				dict.Add("TimeBeforeStir", Stirring_TimeBeforeNeedingToStir.ToString());
				dict.Add("TimeBeforeDanger", Stirring_TimeBeforeEnteringDangerZone.ToString());
				dict.Add("TimeBeforeBurn", Stirring_TimeBeforeFullyBurned.ToString());
				dict.Add("BurnRecoveryRate", Stirring_BurnRecoveryMultiplier.ToString());
                break;
			case MG_Minigame.Type.Blending:
				dict.Add("NumIngredients", Blending_NumIngredients.ToString());
                break;
            case MG_Minigame.Type.Spicing:
                //InitSpicing(); //TODO remove when editor script works
				dict.Add("NumBottles", Spicing_NumberOfBottles.ToString());
				/*
				//old style
                int i = 1;
                foreach (MGSpiceGame.SpiceBottle bottle in Spicing_BottleList)
                {
                    string label = "Bottle" + i.ToString();
                    dict.Add(label, bottle.ToString());
                    i++;
                }
                */
                break;
        }
        return dict;
    }

}
