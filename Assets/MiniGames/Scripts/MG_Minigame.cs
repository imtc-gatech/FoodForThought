using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MG_Minigame : MonoBehaviour {
    public enum Type
    {
        Placeholder,
        Sorting,
        Stirring,
        Chopping,
        Spicing, 
        Blending
    }

	public enum State { Dormant, Active, Unfocused, Done };
	public State CurrentState = State.Dormant;
	public string Title;
	public bool HandFollow = true;
    public bool CanBeScored = false;
    public float StarResult = 0;
	public int DishID;
	public int StepID;
    public string FeedbackText = "";
	public MG_MinigameHolder MinigameHolder;
    
    public float orthographicSize = 100f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        switch (CurrentState)
        {
            case State.Dormant:
                break;
            case State.Active:
                break;
            case State.Unfocused:
                break;
            case State.Done:
                break;
        }
	}

    virtual public void SetupMinigame()
    {
        CurrentState = State.Dormant;
    }

    virtual public void StartMinigame()
    {
        CurrentState = State.Active;
    }
	
	virtual public void ScoreMinigame(){
		Debug.Log("method ScoreMinigame() was not overwritten");
	}
	
	public void AssignName(){
		string tempName = gameObject.name + DishID + "-" + StepID;
		gameObject.name = tempName;
	}
	
	public void EndGame(){
		Debug.Log(StarResult);
		GameObject.Find(MG_SceneController.SceneControllerName).GetComponent<MG_SceneController>().CloseCurrentGame();
        MinigameHolder.SetStarCount(DishID, StepID, StarResult);
        MinigameHolder.SetFeedbackText(DishID, StepID, FeedbackText);
        CurrentState = State.Done;
		GameObject.Destroy(gameObject);
	}

    public virtual void SetGameVariables(Dictionary<string, string> variableDict)
    {
        Debug.Log("method SetGameVariables() was not overwritten");

        //float NameOfVariable = 0;
        //You should not be declaring variables here. These should be public class variables at the top of the minigame class.

        foreach (KeyValuePair<string, string> vPair in variableDict)
        {
            switch (vPair.Key)
            {
                /*
                case "NameOfVariable":
                    NameOfVariable = float.Parse(vPair.Value);
                    break;
                 */
                //parse cases by variable name and assign them here.
                default:
                    break;
            }
        }
    }

    public virtual Dictionary<string, string> GetGameVariables()
    {
        Debug.Log("method GetGameVariables() was not overwritten");
        var variableDict = new Dictionary<string, string>();

        //Usage:
        //variableDict.Add("NameOfVariable", NameOfVariable.ToString());

        return variableDict;
    }

    public static MG_Minigame.Type MinigameTypeFromString(string name)
    {
        switch (name)
        {
            case "Placeholder":
                return Type.Placeholder;
            case "Sorting":
                return Type.Sorting;
            case "Stirring":
                return Type.Stirring;
            case "Chopping":
                return Type.Chopping;
            case "Blending":
                return Type.Blending;
            case "Spicing":
                return Type.Spicing;
            
            default:
                Debug.Log("ERROR: " + name + " not recognized as valid minigame type.");
                return Type.Placeholder;
        }

    }

    public static MG_Parameters GetParametersFromVariableContainer(FFTDish dish, FFTStep step)
    {
        MG_Parameters parameters = new MG_Parameters();
        parameters.DishID = dish.ID;
        parameters.StepID = step.Index;
        parameters.SetVariableDict(step.MinigameVariableContainer.GetCurrentVariableDictionary());
        parameters.MinigameType = step.MinigameType;
        return parameters;
    }
}
