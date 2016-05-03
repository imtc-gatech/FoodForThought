using UnityEngine;
using System.Collections;

public class FFTGaugeTimer : MonoBehaviour
{

    public enum TimerState
    {
        Pending = 0,
        Uncooked = 1,
        Cooked = 2,
        Burned = 3
    }

    public FFTStep Step;

    public FFTGaugeTimer.TimerState State;

    public FFTTimerBasicView Display;

    public float TimeRemainingInState;
    public float TotalTime = 0.0f;

    public bool Armed = false;
    public bool Running = false;
    public bool Finished = false;

    public Color indicatorColor = Color.white;


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

                break;
            case TimerState.Cooked:

                break;
            case TimerState.Burned:

                break;
        }

        TotalTime += Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
        TimeRemainingInState -= Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
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
                        indicatorColor = Color.green;
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
                        indicatorColor = Color.red;
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
            Display.IndicatorColor = indicatorColor;
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
            indicatorColor = Color.yellow;
        }
    }


}
