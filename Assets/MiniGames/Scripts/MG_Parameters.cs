using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MG_Parameters : System.Object {

    public MG_Minigame.Type MinigameType;
    public int DishID;
    public int StepID;
    public float StarResult;
    public string FeedbackText;
    public MG_LaunchButton ButtonToLaunch;
    public List<string> VariableKeys;
    public List<string> VariableValues;

    public MG_Parameters()
    {
        //default values
        FeedbackText = "";
        VariableKeys = new List<string>();
        VariableValues = new List<string>();
    }

    public static string GetMinigameDictKey(int dishID, int stepID)
    {
        return dishID.ToString() + "-" + stepID.ToString();
    }

    /// <summary>
    /// Retrieves the variable dictionary of all initialized values for instantiated minigame setup.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> GetVariableDict()
    {
        var variableDict = new Dictionary<string, string>();
        if (VariableKeys.Count == VariableValues.Count)
        {
            for (int i = 0; i < VariableKeys.Count; i++)
            {
                variableDict.Add(VariableKeys[i], VariableValues[i]);
            }
        }
        else
        {
            Debug.Log("ERROR: Variable Keys and Values within internal lists do not match.");
        }
        return variableDict;
    }

    /// <summary>
    /// Method to set the internal list/list representation of minigame tweakable variables.
    /// Clears existing data when used. Used due to Unity's inability to serialize Dictionaries.
    /// </summary>
    /// <param name="variableDict"></param>
    public void SetVariableDict(Dictionary<string, string> variableDict)
    {
        if (variableDict != null)
        {
            VariableKeys = new List<string>();
            VariableValues = new List<string>();

            foreach (KeyValuePair<string, string> pair in variableDict)
            {
                VariableKeys.Add(pair.Key);
                VariableValues.Add(pair.Value);
            }
        }
        else
        {
            Debug.Log("Variable Dictionary argument was null. Existing values not changed.");
        }
    }
}
