using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MG_MinigameHolder : MonoBehaviour {
    public List<MG_Parameters> MinigameSources = new List<MG_Parameters>();

    public float xPos = 2000;
    public float yPos = 1000;

    Dictionary<string, MG_Parameters> minigameParameterDict = new Dictionary<string, MG_Parameters>();
    Dictionary<string, MG_Minigame> minigameDict = new Dictionary<string, MG_Minigame>();

	// Use this for initialization
	void Start () {
        //InitializeParametersForMinigames();
        // temporary... should be done by FFTGameManager
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InitializeParametersForMinigames()
    {
        int i = 0;

        foreach (MG_Parameters mgParameters in MinigameSources)
        {
            InitializeMinigameParameters(mgParameters, i);
            i++;
        }
    }

    public void InitializeMinigameParameters(MG_Parameters mgParameters, int index)
    {
        GameObject go = LoadMinigameFromType(mgParameters.MinigameType, index);
        Debug.Log(go.ToString());
        MG_Minigame mg = go.GetComponent<MG_Minigame>();
        mg.DishID = mgParameters.DishID;
        mg.StepID = mgParameters.StepID;
        mg.AssignName();
        go.transform.parent = this.transform;
        mg.MinigameHolder = this;
        string dictKey = MG_Parameters.GetMinigameDictKey(mgParameters.DishID, mgParameters.StepID);
        Debug.Log(dictKey);
        minigameParameterDict.Add(dictKey, mgParameters);
        minigameDict.Add(dictKey, mg);
        mg.SetGameVariables(mgParameters.GetVariableDict());

        //Used for testing with Test Scene. Comment out for main game.
        //mgParameters.ButtonToLaunch.MinigameToLaunch = mg;
    }

    public void SetStarCount(int dishID, int stepID, float starCount)
    {
        string dictKey = MG_Parameters.GetMinigameDictKey(dishID, stepID);

        if (minigameParameterDict.ContainsKey(dictKey))
        {
            minigameParameterDict[dictKey].StarResult = starCount;
        }
        else
        {
            Debug.Log("Key " + dictKey + " missing. Check MG_MinigameHolder.");
        }
    }

    public void SetFeedbackText(int dishID, int stepID, string feedbackText)
    {
        string dictKey = MG_Parameters.GetMinigameDictKey(dishID, stepID);
        if (minigameParameterDict.ContainsKey(dictKey))
        {
            minigameParameterDict[dictKey].FeedbackText = feedbackText;
        }
        else
        {
            Debug.Log("Key " + dictKey + " missing. Check MG_MinigameHolder.");
        }
    }

    public float GetStarCount(int dishID, int stepID)
    {
        string dictKey = MG_Parameters.GetMinigameDictKey(dishID, stepID);
        if (minigameParameterDict.ContainsKey(dictKey))
        {
            return minigameParameterDict[dictKey].StarResult;
        }
        Debug.Log("Key " + dictKey + " not found in score dictionary!");
        return 0;
    }

    public string GetFeedbackText(int dishID, int stepID)
    {
        string dictKey = MG_Parameters.GetMinigameDictKey(dishID, stepID);
        if (minigameParameterDict.ContainsKey(dictKey))
        {
            return minigameParameterDict[dictKey].FeedbackText;
        }
        Debug.Log("Key " + dictKey + " not found in feedback dictionary!");
        return "null";
    }

    /// <summary>
    /// This should be used to get the minigame that will be launched from the Station slots.
    /// </summary>
    /// <param name="dishID">Dish ID (1 index)</param>
    /// <param name="stepID">Step ID (0 index)</param>
    /// <returns>Minigame to launch.</returns>
    public MG_Minigame GetMinigameToLaunch(int dishID, int stepID)
    {
        string dictKey = MG_Parameters.GetMinigameDictKey(dishID, stepID);
        if (minigameDict.ContainsKey(dictKey))
        {
            return minigameDict[dictKey];
        }
        Debug.Log("Key " + dictKey + " not found in minigame sources dictionary!");
        foreach (KeyValuePair<string, MG_Minigame> pair in minigameDict)
        {
            Debug.Log(pair.Key);
        }
        return null;
    }

    public void ResetStructures()
    {
        minigameParameterDict = new Dictionary<string, MG_Parameters>();
        minigameDict = new Dictionary<string, MG_Minigame>();
        MinigameSources = new List<MG_Parameters>();
    }
	
	public void CleanUpForNextLevel()
	{
		MG_Minigame[] minigames = FFTGameManager.Instance.gameObject.GetComponentsInChildren<MG_Minigame>();
		foreach (MG_Minigame mg in minigames)
		{
			FFTUtilities.DestroySafe(mg.gameObject);	
		}
    	ResetStructures();
	}

    GameObject LoadMinigameFromType(MG_Minigame.Type type, int index)
    {
        GameObject go;

        switch (type)
        {
            case MG_Minigame.Type.Placeholder:
                go = GameObject.Instantiate(Resources.Load("MinigamePrefabs/MGPlaceholder", typeof(GameObject)), new Vector3(index * xPos, yPos, 0), this.transform.rotation) as GameObject;
                break;
            case MG_Minigame.Type.Sorting:
                go = GameObject.Instantiate(Resources.Load("MinigamePrefabs/MGSortMain", typeof(GameObject)), new Vector3(index * xPos, yPos, 0), this.transform.rotation) as GameObject;
                break;
            case MG_Minigame.Type.Stirring:
                go = GameObject.Instantiate(Resources.Load("MinigamePrefabs/MGStirPrefab 2", typeof(GameObject)), new Vector3(index * xPos, yPos, 0), this.transform.rotation) as GameObject;
                break;
            case MG_Minigame.Type.Chopping:
                go = GameObject.Instantiate(Resources.Load("MinigamePrefabs/MG2 Root", typeof(GameObject)), new Vector3(index * xPos, yPos, 0), this.transform.rotation) as GameObject;
                break;
            case MG_Minigame.Type.Spicing:
                go = GameObject.Instantiate(Resources.Load("MinigamePrefabs/MGSpiceGameStartingPointScene", typeof(GameObject)), new Vector3(index * xPos, yPos, 0), this.transform.rotation) as GameObject;
                break;
            case MG_Minigame.Type.Blending:
                go = GameObject.Instantiate(Resources.Load("MinigamePrefabs/MGBlend 1", typeof(GameObject)), new Vector3(index * xPos, yPos, 0), this.transform.rotation) as GameObject;
                break;
            default:
                go = new GameObject("!EMPTY_MINIGAME_ERROR!");
                break;
        }
        if (go.name.Contains("(Clone)"))
        {
            go.name = go.name.Substring(0, go.name.IndexOf("(Clone)"));
        }
        return go;
    }

}
