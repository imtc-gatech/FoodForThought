using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(FFTTransitionPop))]
public class FFTResultsDetailDisplayView : MonoBehaviour
{

    public GameObject StepResultOriginal;
    public TextMesh Title;
    public TextMesh FreshnessResult;
    public RageSpline Background;

    List<GameObject> StepResults;

    public Transform DishOrigin;

    public FFTTransitionPop TransitionController;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (StepResults != null)
        {
            foreach (GameObject go in StepResults)
            {
                Destroy(go);
            }
        }
        Destroy(gameObject);
    }

    public void Populate(FFTDish dish, FFTScore score, float freshnessPenalty)
    {
        Title.text = dish.Name + " Detail";
        //float displayPenalty = (float)((int)(dish.FreshnessMeter.CurrentStarPenalty * 2)) / 2; //FIX THIS CRUFT
        //float displayPenalty = score.DishFreshnessPenaltyOnlyRounded(dish.UID);
        if (freshnessPenalty == 0)
            FreshnessResult.text = "";
        else
            FreshnessResult.text = "Freshness Penalty: " + freshnessPenalty + " Stars";
        if (StepResults != null)
        {
            foreach (GameObject go in StepResults)
            {
                Destroy(go);
            }
            StepResults = new List<GameObject>();
        }
        List<FFTStepReport> stepReports = new List<FFTStepReport>();
        if (score.Dict.ContainsKey(dish.UID))
            stepReports = score.Dict[dish.UID];
        else
            Debug.Log(dish.Name + " not found in score dictionary.");
        int stepNumber = 1;
        foreach (FFTStep step in dish.StepDataObjects)
        {
            GameObject NewStepResult = GameObject.Instantiate(StepResultOriginal) as GameObject;
            NewStepResult.transform.parent = gameObject.transform;
            FFTResultsDetailDisplayRowView rowView = NewStepResult.GetComponent<FFTResultsDetailDisplayRowView>();
            GameObject NewStepDisplay = GameObject.Instantiate(step.StepObject) as GameObject;
            FFTVisualTools.ToggleVisibility(NewStepDisplay.transform, true); //borrowed from FFTDIsh
            NewStepDisplay.SetActiveRecursively(true);
            NewStepDisplay.transform.parent = NewStepResult.transform;
            NewStepDisplay.transform.localPosition = rowView.Mat.localPosition += new Vector3(0, 0, -5);
            rowView.StepDisplay = NewStepDisplay;
            rowView.stepNumber = stepNumber;
			if (rowView.DestinationIcon != null)
				rowView.DestinationIcon.Type = step.Destination;
            if (rowView.MinigameIcon != null)
            {
                if (step.Gameplay == FFTStation.GameplayType.MiniGame)
                    rowView.MinigameIcon.State = step.MinigameType;
                else
                    FFTUtilities.DestroySafe(rowView.MinigameIcon);
            }
            if ((stepReports.Count + 1) > stepNumber)
            {
                FFTStepReport report = stepReports[stepNumber - 1];
                rowView.StarCount = report.StarRating;
                rowView.Feedback.text = report.Feedback;
            }
            stepNumber++;
            rowView.Reposition();
        }
        StepResultOriginal.SetActiveRecursively(false);
    }

    public void DestroyView(float delayTime)
    {
        Destroy(this, delayTime + 0.1f);
        TransitionController.Exit(delayTime);
        
    }

    public void AssignDishOrigin(Transform dishTransform)
    {
        float delayTime = 0.3f;
        TransitionController = gameObject.GetComponent<FFTTransitionPop>();
        TransitionController.Origin = dishTransform;
        TransitionController.Enter(delayTime);
    }

}