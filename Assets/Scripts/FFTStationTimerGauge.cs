using UnityEngine;
using System.Collections;

public class FFTStationTimerGauge : MonoBehaviour
{
    public enum TimerState
    {
        Pending = 0,
        Uncooked = 1,
        Cooked = 2,
        Burned = 3
    }

    public enum TimerSize
    {
        Normal = 0,
        Small = 1
    }

    public FFTStep Step;

    public TimerState State;

    public TimerSize Size = TimerSize.Normal;

    public FFTTimerGaugeView Display;

    public float TimeRemainingInState;

    public float TimeRemainingInCurrentState
    {
        get
        {
            switch (State)
            {
                case TimerState.Pending:
                    return 0;
                case TimerState.Uncooked:
                    return Step.Parameters.Uncooked - TotalTime;
                case TimerState.Cooked:
                    return Step.Parameters.Uncooked + Step.Parameters.Cooked - TotalTime;
                case TimerState.Burned:
                    return Mathf.Clamp(Step.Parameters.TotalSeconds - TotalTime, 0, 4096);
                default:
                    return 0;
            }
        }
    }

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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
        //visual display updates moved to the end of the loop
        Step.ElapsedTime = TotalTime;

        if (TimeRemainingInCurrentState <= 0)
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
        //update display at the end of the loop
        Display.CurrentTime = TimeRemainingInCurrentState;

        //GaugeView Specific Code
        Display.IndicatorPosition = TotalTime / Step.Parameters.TotalSeconds;

        if (Step.Parameters.UsesPeakFlavor)
            Display.Peak = Step.Parameters.PeakPercentage;
    }

    public void Setup(FFTStep step)
    {
        if (!Armed)
        {
            Armed = true;
            Step = step;
            State = TimerState.Uncooked;
            TimeRemainingInState = Step.Parameters.Uncooked;
            GameObject newTimerGO = GameObject.Instantiate(Resources.Load("MainGamePrefabs/FlatGaugeTimer")) as GameObject;
            newTimerGO.transform.localScale = FFTKitchen.KitchenScale;
            Display = newTimerGO.GetComponent<FFTTimerGaugeView>();
            Display.InitializeDisplay();
            Display.SetDisplayParameters(Step.Parameters.GetParametersVector3());
            Display.UpdateDisplay();

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
		Running = false;
		Finished = true;
		TimeRemainingInState = Mathf.Clamp(TimeRemainingInState, 0, 999f);
		
        FFTStepReport report = new FFTStepReport();

        FFTUtilities.DestroySafe(Display.gameObject);
        //FFTUtilities.DestroySafe(gameObject);

        string actionResult = "";
		float maxStars = 5f;
        float starRating = 5f;

        float peakTolerance = 0.1f * Step.Parameters.TotalSeconds / 2; // based on 0.05 for outline from ProgressBarOutline.shader * 2 (divded because we check each side separately)
		
		float peakStarPenalty = 2f;
		
		float peakStarRatingCap = maxStars - peakStarPenalty;

        switch (State)
        {
            case TimerState.Uncooked:
                if (Step.Parameters.UsesPeakFlavor)
				{
					actionResult = "Unfinished";
					starRating = peakStarRatingCap - ((TimeRemainingInState / Step.Parameters.Uncooked) * peakStarRatingCap);
				}
				else
				{
					actionResult = "Unfinished";
	                starRating = maxStars - ((TimeRemainingInState / Step.Parameters.Uncooked) * maxStars);
					//Debug.Log("TimeR:" + TimeRemainingInState + " , Uncooked:" + Step.Parameters.Uncooked);
				}
                break;
            case TimerState.Cooked:
				if (Step.Parameters.UsesPeakFlavor)
				{
					//uses Peak flavor, the closer to the peak strip, the higher the stars.
					//player never gets less than 3 stars (currently)
					float timeElapsedInState = Step.Parameters.Cooked - TimeRemainingInState;
					float peakPercent = (float) Step.Parameters.PeakPercentage / 100f;
					float peakTimePosition = Step.Parameters.Cooked * peakPercent;
					//Debug.Log("Position In State: " + positionInState);
					//Debug.Log("Peak Percent: " + peakPercent);
					//Debug.Log("Peak Time: " + peakTime);
					if (timeElapsedInState < peakTimePosition)
					{
						//calculate left side
						if (timeElapsedInState > (peakTimePosition - peakTolerance))
						{
							starRating = maxStars;
							actionResult = "Prepared";
						}
						else
						{
							float timeSpan = peakTimePosition - peakTolerance;
							float percentageProgress = timeElapsedInState / timeSpan;
							starRating = peakStarRatingCap + (percentageProgress * peakStarPenalty);
							// the closer we get to peak, the higher the score
							actionResult = "Almost there";
							if (starRating >= 4.5f)
								actionResult = "Prepared";
							//TODO: More granular feedback here.
						}
					}
					else
					{
						//calculate right side
						if (timeElapsedInState < (peakTimePosition + peakTolerance))
						{
							starRating = maxStars;
							actionResult = "Prepared";
						}
						else
						{
							float timeSpan = Step.Parameters.Cooked - (peakTimePosition + peakTolerance);
							float percentageProgress = (timeElapsedInState - (peakTimePosition + peakTolerance)) / timeSpan;
							starRating = maxStars - (percentageProgress * peakStarPenalty);
							// the further away from peak we are, the lower the score
							actionResult = "Too much";
							if (starRating >= 4.5f)
								actionResult = "Prepared";
							//TODO: More granular feedback here.
						}
					}
				}	
                else
				{
					// no Peak flavor, automatic 5 stars.
					actionResult = "Prepared";
					starRating = maxStars;
				}
				
                break;
            case TimerState.Burned:
				if (Step.Parameters.UsesPeakFlavor)
				{
					actionResult = "Overdone";
					starRating = peakStarRatingCap - ((1 - (TimeRemainingInState / Step.Parameters.Burned)) * peakStarRatingCap);
				}
				else
				{
					actionResult = "Overdone";
	                starRating = maxStars - ((1 - (TimeRemainingInState / Step.Parameters.Burned)) * maxStars);
					//Debug.Log("TimeR:" + TimeRemainingInState + " , Uncooked:" + Step.Parameters.Uncooked);
				}
                break;
        }
		
		starRating = Mathf.Clamp(starRating, 1, maxStars);
		
        report.Feedback = actionResult;
        report.StarRating = starRating;

        return report;
    }


}
