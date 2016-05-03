using UnityEngine;
using System.Collections;

public class FFTSlotCounterBehaviour : MonoBehaviour {
	
	FFTStepAction.InteractionType interactionType = FFTStepAction.InteractionType.Exited;
	FFTStepAction.SlotActionType actionTaken = FFTStepAction.SlotActionType.Error;

    public static float scaleTime = 0.05f;    

    public bool entered;

    public bool isOccupied;

    GameObject shadow;

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
	void Start () {
		GM = FFTGameManager.Instance;
        //resizing the collider to allow for clicking the timer underneath (should likely be split out to another function)
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = new Vector3(116, 42, 40);
        collider.center = new Vector3(37.5f, 0, 0);
	
	}
	
	// Update is called once per frame
	void Update () {
        isOccupied = Slot.Occupied;
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
			if (GM.GameplayArmed || GM.State == FFTGameManager.GameState.Gameplay)
			{
				if (Slot.Dish.Finished)
				{
					actionTaken = FFTStepAction.SlotActionType.Inactive_DishCounter_FinishedDish;
					LogSlotAction(interactionType, actionTaken); //logging
				}
				else
				{
					actionTaken = FFTStepAction.SlotActionType.Actionable_DishCounter;
		            Slot.Selected = true;
		            Slot.Dish.ScaleUp();
					LogSlotAction(interactionType, actionTaken); //logging
				}
			}
        }
		else
		{
			if (GM.GameplayArmed || GM.State == FFTGameManager.GameState.Gameplay)
			{
				actionTaken = FFTStepAction.SlotActionType.Inactive_DishCounter_EmptySlot;
				LogSlotAction(interactionType, actionTaken); //logging
			}
		}
    }

    public void Exited()
    {
		//for logging purposes
		interactionType = FFTStepAction.InteractionType.Exited;
		
        if (Slot.Occupied)
        {
			if (GM.GameplayArmed || GM.State == FFTGameManager.GameState.Gameplay)
			{
				if (Slot.Dish.Finished)
				{
					actionTaken = FFTStepAction.SlotActionType.Inactive_DishCounter_FinishedDish;
					LogSlotAction(interactionType, actionTaken); //logging
				}
				else
				{
					actionTaken = FFTStepAction.SlotActionType.Actionable_DishCounter;
					Slot.Selected = false;
	            	Slot.Dish.ScaleDown();
					LogSlotAction(interactionType, actionTaken); //logging
				}
			}
        }
		else
		{
			if (GM.GameplayArmed || GM.State == FFTGameManager.GameState.Gameplay)
			{
				actionTaken = FFTStepAction.SlotActionType.Inactive_DishCounter_EmptySlot;
				LogSlotAction(interactionType, actionTaken); //logging
			}
		}
    }

    public void Selected()
    {
		//for logging purposes
		interactionType = FFTStepAction.InteractionType.Selected;
		
        if (Slot.Occupied)
        {
			if (GM.GameplayArmed || GM.State == FFTGameManager.GameState.Gameplay)
			{
				if (GM.State != FFTGameManager.GameState.Gameplay)
					GM.StartGameplay();
				
				if (Slot.Dish.Finished)
				{
					actionTaken = FFTStepAction.SlotActionType.Inactive_DishCounter_FinishedDish;
					LogSlotAction(interactionType, actionTaken); //logging
				}
				else
				{
					//TODO_Q: separate these based on availability, or to log them as game actions?
					actionTaken = FFTStepAction.SlotActionType.Actionable_DishCounter;
					LogSlotAction(interactionType, actionTaken); //logging
					
					if (FFTGameManager.Instance.StationSlotAvailable(Slot.Dish.CurrentDestination))
		            {
		                Debug.Log("Available slot for " + Slot.Dish.CurrentDestination.ToString());
		                Slot.Dish.ScaleDown();
		                /*
		                Slot.Dish.IsShadow = true;
		                Slot.Shadow = GameObject.Instantiate(Slot.Dish.gameObject) as GameObject;
		                Slot.Dish.IsShadow = false;
		                //Slot.Shadow.RemoveComponent(typeof(FFTDish));
		                //Slot.Shadow.RemoveComponent<FFTDish>():
		                Slot.Shadow.transform.position = Slot.transform.position;
		                 */
		                
		                FFTGameManager.Instance.SendDishToStationSlot(Slot.Dish);
						
		
		            }
		            else
		            {
		                Debug.Log("Slot unavailable for " + Slot.Dish.CurrentDestination.ToString());
						//TODO: Shake dish to demonstrate that slot is unavailable.
						Slot.Dish.Shake();
		            }
				}
	        }
		}
		else
		{
			if (GM.GameplayArmed || GM.State == FFTGameManager.GameState.Gameplay)
			{
				actionTaken = FFTStepAction.SlotActionType.Inactive_DishCounter_EmptySlot;
				LogSlotAction(interactionType, actionTaken); //logging
			}
		}
		
    }
	
	FFTStepAction.SlotActionType GetEmptySlotAction()
	{
		return FFTStepAction.SlotActionType.Inactive_DishCounter_EmptySlot;
	}
	
	public void LogSlotAction(FFTStepAction.InteractionType interactionType, FFTStepAction.SlotActionType actionTaken)
	{
		if (GM.LogActions)
		{
			FFTStepAction currentAction = new FFTStepAction();
			currentAction.Reset();
			currentAction.AssignSlotAction(Slot, interactionType, actionTaken);
			currentAction.GameState = FFTGameManager.Instance.State;
			//Debug.Log("Action:" + currentAction.SecondsSinceLevelLoad);
			
			//GM.DataManager.ActionsTaken.Add(action);
			
			//Unity does not like this:
			GM.DataManager.AddAction(currentAction);
			
			//Slot.Dish.CurrentStepObject.ActionsTaken.Add(action);	
		}
	}	

}
