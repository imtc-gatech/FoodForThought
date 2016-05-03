using UnityEngine;
using System.Collections;

public class FFTStationTimerBasic : MonoBehaviour {

    public enum TimerState
    {
        Pending = 0,
        Uncooked = 1,
        Cooked = 2,
        Burned = 3
    }

    public FFTStep Step;

    public TimerState State;

    public FFTTimerBasicView Display;

    public float TimeRemainingInState;
    public float TotalTime = 0.0f;

    public bool Armed = false;
    public bool Running = false;
    public bool Finished = false;

    public Color IndicatorColor
    {
        get
        {
            return _indicatorColor;
        }
        set
        {
            if (value != _indicatorColor)
            {
                Display.IndicatorColor = value;
            }
            _indicatorColor = value;
        }
    }

    private Color _indicatorColor = Color.white;


	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Running)
        {
            UpdateTimer();
        }

        

	}

    void UpdateTimer()
    {
        if (State == TimerState.Pending)
        {
            return;
        }

        switch (State)
        {
            case TimerState.Uncooked:
                IndicatorColor = Color.yellow;
                break;
            case TimerState.Cooked:
                IndicatorColor = Color.green;
                break;
            case TimerState.Burned:
                IndicatorColor = Color.red;
                break;
        }

        TotalTime += Time.deltaTime * FFTTimeManager.Instance.StationGameplayTimeScale(Step.Destination);
        TimeRemainingInState -= Time.deltaTime * FFTTimeManager.Instance.StationGameplayTimeScale(Step.Destination);
        Display.CurrentTime = TimeRemainingInState;
        Step.ElapsedTime = TotalTime;
        if (TimeRemainingInState <= 0)
        {
            switch (State)
            {
                case TimerState.Uncooked:
                    if (Step.Parameters.IsCookable && Step.Parameters.Cooked > 0)
                    {
                        State = TimerState.Cooked;
                        TimeRemainingInState = Step.Parameters.Cooked;
                        IndicatorColor = Color.green;
                    }
                    else
                    {
                        Running = false;
                        Finished = true;
                    }
                    break;
                case TimerState.Cooked:
                    if (Step.Parameters.IsBurnable && Step.Parameters.Burned > 0)
                    {
                        State = TimerState.Burned;
                        TimeRemainingInState = Step.Parameters.Burned;
                        IndicatorColor = Color.red;
                    }
                    else
                    {
                        Running = false;
                        Finished = true;
                    }
                    break;
                case TimerState.Burned:
                    {
                        Running = false;
                        Finished = true;
                        
                    }
                    break;
            }
        }
    }

    public void Setup(FFTStep step)
    {
        if (!Armed)
        {
            Armed = true;
            Step = step;
            State = TimerState.Uncooked;
            TimeRemainingInState = Step.Parameters.Uncooked;
            GameObject newTimerGO = GameObject.Instantiate(Resources.Load("MainGamePrefabs/TimerBasic")) as GameObject;
            Display = newTimerGO.GetComponent<FFTTimerBasicView>();
            newTimerGO.SetActiveRecursively(false);
            
        }
    }

    public void BeginTimer()
    {
        if (!Running)
        {
            Display.gameObject.SetActiveRecursively(true);
            Running = true;
            IndicatorColor = Color.yellow;
        }
    }

    public FFTStepReport EndTimer()
    {
        FFTStepReport report = new FFTStepReport();

        FFTUtilities.DestroySafe(Display.gameObject);
        //FFTUtilities.DestroySafe(gameObject);

        string actionResult = "";
        float starRating = 5f;

        switch (State)
        {
            case TimerState.Uncooked:
                actionResult = "Dish was undercooked.";
                starRating -= (TimeRemainingInState / Step.Parameters.Uncooked) * 3f;
                break;
            case TimerState.Cooked:
                actionResult = "Dish was cooked.";
                break;
            case TimerState.Burned:
                actionResult = "Dish was burned.";
                starRating -= (1 - (TimeRemainingInState / Step.Parameters.Burned)) * 3f;
                break;
        }

        report.Feedback = actionResult;
        report.StarRating = starRating;

        return report;
    }


}
