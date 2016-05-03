using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FFTTimerGlobal : MonoBehaviour {

    public enum TimerState
    {
        Pending = 0,
        Running = 1,
        Warning = 2,
        Expired = 3
    }

    public TimerState State;

    public FFTTimerBasicView Display;

    public bool UseGlobalTimer = true;

    public float TotalTime = 0.0f;
    public float TimeRemaining = 0.0f;
    public float WarningThresholdPercentage = 0.25f;
    public float EndingThresholdPercentage = 0.10f;
	
	public float TimeOverTheLimit = 0.0f;
	
    float TimeWarning = 0.0f;
    float TimeEnding = 0.0f;

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
            IndicatorColor = Color.white;
            return;
        }

        switch (State)
        {
            case TimerState.Running:
                IndicatorColor = Color.green;
                break;
            case TimerState.Warning:
                IndicatorColor = Color.yellow;
                break;
            case TimerState.Expired:
                IndicatorColor = Color.red;
                break;
        }

        TimeRemaining -= Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
        
        switch (State)
        {
            case TimerState.Running:
                if (TimeRemaining < TimeWarning)
                {
                    State = TimerState.Warning;
                }

                break;
            case TimerState.Warning:
                if (TimeRemaining < TimeEnding)
                {
                    State = TimerState.Expired;
                }

                break;
            case TimerState.Expired:
                {
                    if (TimeRemaining < 0)
                    {
						TimeOverTheLimit -= TimeRemaining;
                        TimeRemaining = 0;
						
                        //Running = false;
                        //Finished = true;
                    }
                }
                break;
        }
		
		Display.CurrentTime = TimeRemaining;
    }
     
    public void Setup(float totalTime)
    {
        if (!Armed)
        {
            if (UseGlobalTimer)
            {
                Armed = true;
                TotalTime = totalTime;
                TimeWarning = TotalTime * WarningThresholdPercentage;
                TimeEnding = TotalTime * EndingThresholdPercentage;
                State = TimerState.Pending;
                TimeRemaining = TotalTime;
                GameObject newTimerGO = GameObject.Instantiate(Resources.Load("MainGamePrefabs/TimerBasic")) as GameObject;
                Display = newTimerGO.GetComponent<FFTTimerBasicView>();
                Display.gameObject.SetActiveRecursively(true);
                Display.ColorText = false;
                Display.ShowMinutes = true;
                Display.CurrentTime = TimeRemaining;
				Display.tag = "MainTimer";
            }
            else
            {
                Debug.Log("Set 'UseGlobalTimer' to true before Setting up the Global Timer.");
            }
            
        }
    }

    public void BeginTimer()
    {
        if (!Running)
        {
            if (Armed)
            {
                State = TimerState.Running;
                Display.gameObject.SetActiveRecursively(true);
                Running = true;
                IndicatorColor = Color.green;
            }
            else 
            {
                Debug.Log("GlobalTimer must be armed before it begins.");
            }
        }
    }

    public void EndTimer()
    {
		//We are now handling all destruction in the GameManager
		
        //FFTUtilities.DestroySafe(Display.gameObject);
        //FFTUtilities.DestroySafe(gameObject);

        //TODO: SEND MESSAGE TO GAMEMANAGER SAYING THAT TIME RAN OUT
    }
	
	void OnDestroy()
	{
		if (Display != null)
			FFTUtilities.DestroySafe(Display.gameObject);
	}


}

