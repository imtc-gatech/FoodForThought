using UnityEngine;
using System.Collections;

public class FFTSlotStationBehaviour : MonoBehaviour {
	
	FFTStepAction.InteractionType interactionType = FFTStepAction.InteractionType.Exited;
	FFTStepAction.SlotActionType actionTaken = FFTStepAction.SlotActionType.Error;

    public enum TimerDisplayState
    {
        Basic = 0,
        Gauge = 1
    }

    public Vector3 BasicTimerRelativePosition = new Vector3(1, -32, -4);
    public Vector3 GaugeTimerRelativePosition = new Vector3(-5, -27, -6); //new Vector3(-6, -33, -4);
    public Vector3 GaugeTimerRelativePositionSmall = new Vector3(-6.666667f, -25, -6);
    public float SmallScaleMultiplier = 0.9f;


    public static FFTSlotStationBehaviour.TimerDisplayState DisplayState = FFTSlotStationBehaviour.TimerDisplayState.Gauge;

    public FFTStation.GameplayType GameplayType = FFTStation.GameplayType.ElapsedTime;

    public static float scaleTime = 0.05f;

    public bool entered;

    public bool isOccupied;

    private static bool elapsedStartsOnFirstClick = true;
	
	private FFTStation parentStation;
	private int slotID = 0;

    FFTSlot Slot
    {
        get
        {
            if (_slot == null)
            {
                _slot = gameObject.GetComponent<FFTSlot>();
            }
            return _slot;
        }
    }
    private FFTSlot _slot;
	
	public FFTGameManager GM;

    // Use this for initialization
    void Start()
    {
		GM = FFTGameManager.Instance;
        if (GameplayType == FFTStation.GameplayType.ElapsedTime)
        {
            if (DisplayState == TimerDisplayState.Gauge)
            {
                //resizing the collider to allow for clicking the timer underneath (should likely be split out to another function)
                BoxCollider collider = GetComponent<BoxCollider>();
				
                collider.size = new Vector3(40, 67, 60); //old//new Vector3(40, 53, 60);
                collider.center = new Vector3(0, -3.75f, -25f);//old//new Vector3(0, -7.5f, -25f);
            }
        }
		parentStation = gameObject.transform.parent.parent.gameObject.GetComponent<FFTStation>();
		
		int.TryParse(gameObject.name[6].ToString(), out slotID);
		
        
    }

    // Update is called once per frame
    void Update()
    {
        isOccupied = Slot.Occupied;
        //this is a dirty, inefficient hack. We check if slot.dish is null because we null it out when OnMinigameEnd() is called, otherwise painful errors
        if (GameplayType == FFTStation.GameplayType.MiniGame && Slot.Dish != null && Slot.Dish.CurrentStepObject.MinigamePrefab == null)
        {
			//Debug.Log("OnMinigameEnd()");
            OnMinigameEnd();
        }
    }
    void OnMouseEnter()
    {	
		if (GM.InGameplayLoggingRange && !FFTTimeManager.Instance.GameplayPaused)
			Entered();
    }

    void OnMouseExit()
    {	
		if (GM.InGameplayLoggingRange && !FFTTimeManager.Instance.GameplayPaused)
        	Exited();
    }

    void OnMouseDown()
    {
		if (GM.InGameplayLoggingRange && !FFTTimeManager.Instance.GameplayPaused)
        	Selected();
    }

    public void Entered()
    {
		//for logging purposes
		interactionType = FFTStepAction.InteractionType.Entered;
		
        if (Slot.Occupied)
        {
			actionTaken = GetActiveGameplaySlotAction(); //logging 
            Slot.Selected = true;
            Slot.Dish.ScaleUp();
			LogSlotAction(interactionType, actionTaken); //logging
        }
		else
		{
			actionTaken = GetEmptySlotAction(); //logging
			LogSlotAction(interactionType, actionTaken); //logging
		}
		
		
    }

    public void Exited()
    {
		//for logging purposes
		interactionType = FFTStepAction.InteractionType.Exited;
		
        if (Slot.Occupied)
        {
			actionTaken = GetActiveGameplaySlotAction(); //logging 
            Slot.Selected = false;
            Slot.Dish.ScaleDown();
			LogSlotAction(interactionType, actionTaken); 
        }
		else
		{
			actionTaken = GetEmptySlotAction(); //logging
			LogSlotAction(interactionType, actionTaken);
		}
    }

    public void Selected()
    {
		//for logging purposes
		interactionType = FFTStepAction.InteractionType.Selected;
		
        if (Slot.Occupied)
        {
			actionTaken = GetActiveGameplaySlotAction(); //logging 
			LogSlotAction(interactionType, actionTaken); 
			
            Slot.Dish.ScaleDown();
            if (GameplayType == FFTStation.GameplayType.ElapsedTime)
            {
                if (DisplayState == TimerDisplayState.Basic)
                    HandleBasicTimer(); //TODO log game actions
                else if (DisplayState == TimerDisplayState.Gauge)
                    HandleGaugeTimer(); //log game actions inside these functions
            }
            else if (GameplayType == FFTStation.GameplayType.MiniGame)
            {
                HandleMinigame();
            }
            
        }
		else
		{
			actionTaken = GetEmptySlotAction(); //logging
			LogSlotAction(interactionType, actionTaken); 
		}
		
    }
	
	FFTStepAction.SlotActionType GetActiveGameplaySlotAction()
	{
		switch (GameplayType)
			{
				case FFTStation.GameplayType.ElapsedTime:
					return FFTStepAction.SlotActionType.Actionable_DishStation_ElapsedTime;
				case FFTStation.GameplayType.MiniGame:
					return FFTStepAction.SlotActionType.Actionable_DishStation_Minigame;
				default:
					return FFTStepAction.SlotActionType.Error;
			}
	}
	
	FFTStepAction.SlotActionType GetEmptySlotAction()
	{
		return FFTStepAction.SlotActionType.Inactive_DishStation_EmptySlot;
	}
	
	public void LogSlotAction(FFTStepAction.InteractionType interactionType, FFTStepAction.SlotActionType actionTaken)
	{
		if (GM.LogActions)
		{
			FFTStepAction currentAction = new FFTStepAction();
			currentAction.Reset();
			currentAction.AssignSlotAction(Slot, interactionType, actionTaken);
			currentAction.GameState = FFTGameManager.Instance.State;
			currentAction.Slot_Station_SlotID = slotID;
			currentAction.Slot_Station_Type = parentStation.Name;
			//Debug.Log("Action:" + currentAction.SecondsSinceLevelLoad);
			//GM.DataManager.ActionsTaken.Add(action);
			GM.DataManager.AddAction(currentAction);
			//Slot.Dish.CurrentStepObject.ActionsTaken.Add(action);	
			
		}
	}

    void HandleMinigame()
    {
        MG_Minigame minigame = Slot.Dish.CurrentStepObject.MinigamePrefab;
        FFTGameManager.Instance.MinigameManager.SwitchGame(minigame, gameObject.transform.position);
    }

    //TODO FIX THIS HACKY BRUTISH MESS PART III
    void OnMinigameEnd()
    {
        FFTUtilities.DestroySafe(GetComponent<FFTMinigameIndicatorDisplay>());

        FFTStepReport minigameReport = new FFTStepReport();
        minigameReport.Data = Slot.Dish.CurrentStepObject;
        minigameReport.Feedback = FFTGameManager.Instance.MinigameContainer.GetFeedbackText(Slot.Dish.ID, minigameReport.Data.Index);
        minigameReport.StarRating = FFTGameManager.Instance.MinigameContainer.GetStarCount(Slot.Dish.ID, minigameReport.Data.Index);
        Debug.Log(minigameReport.Log);
        Slot.Dish.CurrentStepObject.Result = minigameReport;

        if ((Slot.Dish.CurrentDestination == FFTStation.Type.Spice) || (Slot.Dish.CurrentDestination == FFTStation.Type.Cook))
            iTween.MoveTo(Slot.Dish.gameObject, iTween.Hash("x", Slot.Dish.HomeCounterSlot.transform.position.x, "y", Slot.Dish.HomeCounterSlot.transform.position.y, "z", Slot.Dish.HomeCounterSlot.transform.position.z - 3, "time", FFTStation.slotTravelTimeFar));
        else
            iTween.MoveTo(Slot.Dish.gameObject, iTween.Hash("x", Slot.Dish.HomeCounterSlot.transform.position.x, "y", Slot.Dish.HomeCounterSlot.transform.position.y, "z", Slot.Dish.HomeCounterSlot.transform.position.z - 3, "time", FFTStation.slotTravelTimeClose));
		
		if (Slot.Dish.BackgroundBox != null)
			Slot.Dish.BackgroundBox.SetActiveRecursively(true);
		
        //FFTUtilities.DestroySafe(Slot.Dish.HomeCounterSlot.Shadow);

        if (Slot.Dish.CurrentStep != Slot.Dish.StepDataObjects.Count - 1)
        {
            Slot.Dish.CurrentStep++;
        }
        else
        {
            Slot.Dish.Finished = true;
        }

        //iTweenUtilities.MoveBy(Slot.Dish.gameObject, Slot.Dish.HomeCounterSlot.transform.position, FFTStation.slotTravelTime);

        Slot.Dish.HomeCounterSlot.GetComponent<FFTSlot>().Dish = Slot.Dish;

        Slot.Dish = null;

        GameplayType = FFTStation.GameplayType.ElapsedTime; // to avoid double advancement workaround

    }

    //TODO FIX THIS HACKY BRUTISH MESS
    void OnTimingGameEndBasic(FFTStationTimerBasic timer)
    {
        FFTStepReport timerReport = timer.EndTimer();

        FFTUtilities.DestroySafe(GetComponent<FFTStationTimerBasic>());

        timerReport.Data = Slot.Dish.CurrentStepObject;

        Debug.Log(timerReport.Log);

        Slot.Dish.CurrentStepObject.Result = timerReport;
        
        if ((Slot.Dish.CurrentDestination == FFTStation.Type.Spice) || (Slot.Dish.CurrentDestination == FFTStation.Type.Cook))
            iTween.MoveTo(Slot.Dish.gameObject, iTween.Hash("x", Slot.Dish.HomeCounterSlot.transform.position.x, "y", Slot.Dish.HomeCounterSlot.transform.position.y, "z", Slot.Dish.HomeCounterSlot.transform.position.z - 3, "time", FFTStation.slotTravelTimeFar));
        else
            iTween.MoveTo(Slot.Dish.gameObject, iTween.Hash("x", Slot.Dish.HomeCounterSlot.transform.position.x, "y", Slot.Dish.HomeCounterSlot.transform.position.y, "z", Slot.Dish.HomeCounterSlot.transform.position.z - 3, "time", FFTStation.slotTravelTimeClose));
		
		if (Slot.Dish.BackgroundBox != null)
			Slot.Dish.BackgroundBox.SetActiveRecursively(true);
		
        if (Slot.Dish.CurrentStep != Slot.Dish.StepDataObjects.Count - 1)
        {
            Slot.Dish.CurrentStep++;
        }
        else
        {
            Slot.Dish.Finished = true;
        }
        
        //iTweenUtilities.MoveBy(Slot.Dish.gameObject, Slot.Dish.HomeCounterSlot.transform.position, FFTStation.slotTravelTime);

        Slot.Dish.HomeCounterSlot.GetComponent<FFTSlot>().Dish = Slot.Dish;

        Slot.Dish = null;
    }

    //TODO FIX THIS HACKY BRUTISH MESS
    void OnTimingGameEndGauge(FFTStationTimerGauge timer)
    {
        Slot.Dish.CurrentScale = 1;
        Slot.Dish.ScaleDown();

        FFTStepReport timerReport = timer.EndTimer();

        FFTUtilities.DestroySafe(GetComponent<FFTStationTimerGauge>());

        timerReport.Data = Slot.Dish.CurrentStepObject;

        Debug.Log(timerReport.Log);

        Slot.Dish.CurrentStepObject.Result = timerReport;

        if ((Slot.Dish.CurrentDestination == FFTStation.Type.Spice) || (Slot.Dish.CurrentDestination == FFTStation.Type.Cook))
		{
			iTween.MoveTo(Slot.Dish.gameObject, iTween.Hash("x", Slot.Dish.HomeCounterSlot.transform.position.x, "y", Slot.Dish.HomeCounterSlot.transform.position.y, "z", Slot.Dish.HomeCounterSlot.transform.position.z - 3, "time", FFTStation.slotTravelTimeFar));
		}
        else
		{
			iTween.MoveTo(Slot.Dish.gameObject, iTween.Hash("x", Slot.Dish.HomeCounterSlot.transform.position.x, "y", Slot.Dish.HomeCounterSlot.transform.position.y, "z", Slot.Dish.HomeCounterSlot.transform.position.z - 3, "time", FFTStation.slotTravelTimeClose));
            
			
			//iTweenUtilities.MoveBy(Slot.Dish.gameObject, Slot.Dish.HomeCounterSlot.transform.position, FFTStation.slotTravelTimeClose);
			/*
			Vector3 begin = Slot.Dish.gameObject.transform.position;
			Vector3 target = Slot.Dish.HomeCounterSlot.transform.position;
	
	        Hashtable ht = new Hashtable(){
	            {iT.MoveBy.x, target.x - begin.x},
	            {iT.MoveBy.y, target.y - begin.y},
	            {iT.MoveBy.time, FFTStation.slotTravelTimeClose},
	            {iT.MoveBy.space, Space.Self},
				{iT.MoveBy.oncomplete, "AlignToHomePosition"}
	        };
	
	        iTween.MoveBy(Slot.Dish.gameObject, ht);
	        */
		}
		if (Slot.Dish.BackgroundBox != null)
			Slot.Dish.BackgroundBox.SetActiveRecursively(true);

        if (Slot.Dish.CurrentStep != Slot.Dish.StepDataObjects.Count - 1)
        {
            Slot.Dish.CurrentStep++;
        }
        else
        {
            Slot.Dish.Finished = true;
        }
        //iTweenUtilities.MoveBy(Slot.Dish.gameObject, Slot.Dish.HomeCounterSlot.transform.position, FFTStation.slotTravelTime);
		
        Slot.Dish.HomeCounterSlot.GetComponent<FFTSlot>().Dish = Slot.Dish;

        Slot.Dish = null;
    }

    void HandleBasicTimer()
    {
        FFTStationTimerBasic Timer = gameObject.GetComponent<FFTStationTimerBasic>();
        if (Timer != null)
        {
            if (!Timer.Running)
            {
                if (!Timer.Finished)
                {
                    StartBasicTimer(Timer);
                }
                else //destroy our objects and move the dish item back
                {
                    OnTimingGameEndBasic(Timer);
                    Debug.Log("TODO: Step finished (user terminated after time ran out).");
                }
            }
            else
            {
                Timer.Finished = true;
                OnTimingGameEndBasic(Timer);
                Debug.Log("TODO: Step finished (user terminated before time ran out).");
            }
        }
    }

    void HandleGaugeTimer()
    {
        FFTStationTimerGauge Timer = gameObject.GetComponent<FFTStationTimerGauge>();
        if (Timer != null)
        {
            if (!Timer.Running)
            {
                if (!Timer.Finished)
                {
                    StartGaugeTimer(Timer);
                }
                else //destroy our objects and move the dish item back
                {
                    OnTimingGameEndGauge(Timer);
                    Debug.Log("TODO: Step finished (user terminated after time ran out).");
                }
            }
            else
            {
                Timer.Finished = true;
                OnTimingGameEndGauge(Timer);
                Debug.Log("TODO: Step finished (user terminated before time ran out).");
            }
        }
    }

    public void StartBasicTimer()
    {
        FFTStationTimerBasic Timer = gameObject.GetComponent<FFTStationTimerBasic>();
        StartBasicTimer(Timer);
    }

    void StartBasicTimer(FFTStationTimerBasic Timer)
    {
        Timer.Display.gameObject.transform.position = gameObject.transform.position + BasicTimerRelativePosition;
        Timer.BeginTimer();
    }

    public void StartGaugeTimer()
    {
        FFTStationTimerGauge Timer = gameObject.GetComponent<FFTStationTimerGauge>();
        StartGaugeTimer(Timer);
    }

    public void StartGaugeTimer(FFTStationTimerGauge Timer)
    {
        if (Timer.Size == FFTStationTimerGauge.TimerSize.Normal)
        {
            Timer.Display.gameObject.transform.localPosition = GaugeTimerRelativePosition;
        }
        else
        {
            Timer.Display.gameObject.transform.localPosition = GaugeTimerRelativePositionSmall;
            Timer.Display.gameObject.transform.localScale = new Vector3(SmallScaleMultiplier, SmallScaleMultiplier, 1);
        }
        Timer.BeginTimer();
    }
}
