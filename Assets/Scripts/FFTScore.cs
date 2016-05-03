using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FFTScore : System.Object {
	
	public static float TimingPenaltyMinStars = 0.5f;
	public static float TimingPenaltyMaxStars = 2f;
	public static float TimingPenaltyMaxRange = 120f;

    [SerializeField]
    private List<KeyValuePair<string, int>> _foodItems;
    [SerializeField]
    private List<FFTStepReport> _stepReports;

    private Dictionary<string, List<FFTStepReport>> _foodItemInternalDict;
    private Dictionary<string, float> _foodItemInternalDictFreshnessPenalties;
	
	public bool ReceivedTimingPenalty = false;
	public float TimingPenalty = 0f;
	public float TimeOvertakenInSeconds = 0f;
	public string TimingFeedback = "";

    public Dictionary<string, List<FFTStepReport>> Dict
    {
        get
        {
            if (_foodItemInternalDict == null)
            {
                if (_foodItems != null)
                {
                    _foodItemInternalDict = new Dictionary<string, List<FFTStepReport>>();
                    int index = 0;
                    foreach (KeyValuePair<string, int> pair in _foodItems)
                    {
                        _foodItemInternalDict.Add(pair.Key, new List<FFTStepReport>());
                        for (int i = index; i < index + pair.Value; i++)
                        {
                            _foodItemInternalDict[pair.Key].Add(_stepReports[i]);
                        }
                        index += pair.Value;
                    }
                    return _foodItemInternalDict;
                    //return a new Dictionary constructed from the internal elements
                }
                return null;
            }
            else
            {
                return _foodItemInternalDict;
            }
        }
    }

    public Dictionary<string, float> DictStarPenalty
    {
        get
        {
            if (_foodItemInternalDictFreshnessPenalties == null)
            {
                if (_foodItems != null)
                {
                    _foodItemInternalDictFreshnessPenalties = new Dictionary<string, float>();
                    foreach (KeyValuePair<string, int> pair in _foodItems)
                    {
                        _foodItemInternalDictFreshnessPenalties.Add(pair.Key, 0);
                    }
                    return _foodItemInternalDictFreshnessPenalties;
                }
                return null;
            }
            else
            {
                return _foodItemInternalDictFreshnessPenalties;
            }
        }
    }

    public bool InUse = false;

    public void AddReport(string DishItem, FFTStepReport Report)
    {
        if (!InUse)
        {
            if (!Open())
            {
                Debug.Log("Can't add items to a closed Score Report");
                return;
            }
        }
        if (!_foodItemInternalDict.ContainsKey(DishItem))
        {
            _foodItemInternalDict.Add(DishItem, new List<FFTStepReport>());
        }
        _foodItemInternalDict[DishItem].Add(Report);

    }

    public float DishTotalStarRating(string DishName)
    {
        if (Dict.ContainsKey(DishName))
        {
            List<FFTStepReport> reports = Dict[DishName];
            if (reports.Count < 1)
            {
                Debug.Log("ERROR: No reports found for Dish Item '" + DishName + "'");
                return 0f;
            }
            float cumulativeStars = 0;
            if (reports.Count == 1) // eliminates strange -.5 star rounding error on a single stepped dish (but should be unnecessary code... TODO fix this)
            {
                return reports[0].StarRating;
            }
            else
            {
                foreach (FFTStepReport report in reports)
                {
                    cumulativeStars += report.StarRating;
                }
                return cumulativeStars / reports.Count;                    
            }
        }
        Debug.Log("ERROR: " + DishName + " not found in FFTScore report");
        return 0f;
    }

    public float DishTotalStarRatingWithPenalty(string DishName)
    {
        if (DictStarPenalty != null)
            return Mathf.Clamp(DishTotalStarRating(DishName) - DictStarPenalty[DishName], 1, 5); //CLAMP IS STRANGE, MAKE MORE SEMANTIC.
        else
            return DishTotalStarRating(DishName);
    }

    public float DishFreshnessPenaltyOnly(string DishName)
    {
        if (DictStarPenalty.ContainsKey(DishName))
            return DictStarPenalty[DishName];
        else
            return 0;
    }

    public float DishFreshnessPenaltyOnlyRounded(string DishName)
    {
        float doublePenalty = DishFreshnessPenaltyOnly(DishName);
        doublePenalty = (float)Mathf.RoundToInt(doublePenalty);
        return doublePenalty / 2;
    }

    public float RecipeTotalStarRating()
    {
        float result = 0f;
        float cumulativeStars = 0f;

        foreach (KeyValuePair<string, List<FFTStepReport>> pair in Dict)
        {
            cumulativeStars += DishTotalStarRating(pair.Key);
        }

        result = cumulativeStars / Dict.Count;

        return result;
    }

    public float RecipeTotalStarRatingWithPenalty()
    {
        float result = 0f;
        float cumulativeStars = 0f;

        foreach (KeyValuePair<string, List<FFTStepReport>> pair in Dict)
        {
            cumulativeStars += DishTotalStarRatingWithPenalty(pair.Key);
        }

        result = cumulativeStars / Dict.Count;
		
		if (ReceivedTimingPenalty)
			result -= TimingPenalty;

        return result;
    }

    public string Summary(bool sendToDebugLog)
    {
        string logReport = "Score Report:\n";
        foreach (KeyValuePair<string, List<FFTStepReport>> pair in Dict)
        {
            int i = 0;
            foreach (FFTStepReport report in pair.Value)
            {
                i++;
                string destination = "";
                if (report.Data != null)
                    destination = "(" + report.Data.Destination.ToString() + ")";
                logReport += pair.Key + " Step " + i + destination + ": " + report.StarRating + " stars, " + report.Feedback + "\n";
                
            }
            logReport += pair.Key + " Result: " + DishTotalStarRating(pair.Key) + " stars." + "\n";
        }
        logReport += "\n";
        logReport += "Final Recipe Stars: " + RecipeTotalStarRating() + " from " + Dict.Count + " total dish(es).\n";
        if (sendToDebugLog)
            Debug.Log(logReport);
        return logReport;
    }

    bool Open()
    {
        if (!InUse)
        {
            if (_foodItems != null)
            {
                Debug.Log("Can't reopen a previously created Score Report");
                return false;
            }
            InUse = true;
            _foodItemInternalDict = new Dictionary<string, List<FFTStepReport>>();
            _foodItemInternalDictFreshnessPenalties = new Dictionary<string, float>();
            return true;
        }
        return false;
    }

    bool Close()
    {
        if (InUse)
        {
            if (_foodItemInternalDict == null)
            {
                Debug.Log("ERROR: Internal Dictionary not created. Did you mean to close an empty report?");
                return false;
            }
            _foodItems = new List<KeyValuePair<string, int>>();
            _stepReports = new List<FFTStepReport>();
            foreach (KeyValuePair<string, List<FFTStepReport>> pair in _foodItemInternalDict)
            {
                _foodItems.Add(new KeyValuePair<string, int>(pair.Key, pair.Value.Count));
                _stepReports.AddRange(pair.Value);
            }
            return true;
        }
        Debug.Log("Tried to close a score that was already open.");
        return false;
    }

    public void BuildReport(FFTRecipe recipe)
    {
		//build timing penalty TODO this better
		
		FFTGameManager GM = FFTGameManager.Instance;
		
		if (GM.CurrentLevel.TimedLevel)
		{
			float timeTaken = GM.LevelGameplayElapsedTime;
			float timeAllowed = GM.CurrentLevel.TimeLimit;
			if (timeTaken > timeAllowed)
			{
				ReceivedTimingPenalty = true;
				float timeOvertaken = timeTaken - timeAllowed;
				float calculatedPenalty = (timeOvertaken / TimingPenaltyMaxRange) * TimingPenaltyMaxStars;
				TimingPenalty = Mathf.Clamp(Mathf.Round(calculatedPenalty), TimingPenaltyMinStars, TimingPenaltyMaxStars);
				TimeOvertakenInSeconds = Mathf.Round(timeOvertaken);
				TimingFeedback = "Time Expired! Late by " + TimeOvertakenInSeconds.ToString() + " seconds. " + TimingPenalty.ToString() + " stars lost.";
				//TimingFeedback = TimeOvertakenInSeconds.ToString() + " seconds over the limit.";
				Debug.Log("Timing Penalty: " + TimingFeedback); // + " Stars: " + TimingPenalty.ToString());
			}
		}
		
        foreach (FFTDish dish in recipe.Dishes)
        {
            foreach (FFTStep step in dish.StepDataObjects)
            {
                if (step.Result != null)
                {
                    AddReport(dish.UID, step.Result);
                }
                else
                {
                    FFTStepReport unfinishedStep = new FFTStepReport();
                    unfinishedStep.Feedback = "Time ran out before step was completed.";
                    unfinishedStep.StarRating = 1f;
                    AddReport(dish.UID, unfinishedStep);
                }
            }
			
            if (dish.FreshnessMeterParameters.UseFreshness)
                AddFreshnessPenalty(dish.UID, dish.FreshnessMeter.CurrentStarPenalty);
            else
                AddFreshnessPenalty(dish.UID, 0);
        }
    }

    void AddFreshnessPenalty(string dishName, float starPenalty)
    {
        if (!InUse)
        {
            if (!Open())
            {
                Debug.Log("Can't add items to a closed Score Report");
                return;
            }
        }
        if (!_foodItemInternalDictFreshnessPenalties.ContainsKey(dishName))
        {
            _foodItemInternalDictFreshnessPenalties.Add(dishName, starPenalty);
        }
        else
        {
            _foodItemInternalDictFreshnessPenalties[dishName] = starPenalty;
        }
    }
}
