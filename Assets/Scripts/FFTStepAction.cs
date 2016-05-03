using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FFTStepAction : System.Object {
	
	private FFTGameManager GM;
	
	public enum ActionType
	{
		Empty,
		Slot,
		Button
	}
	
	public ActionType Type = ActionType.Empty;
	public float SecondsSinceLevelLoad = -1;
	public FFTGameManager.GameState GameState = FFTGameManager.GameState.TitleScreen;
	
	public FFTStepAction() {  //reset values in constructor
		Reset();
	}
	
	public void Reset() {
		Type = ActionType.Empty;
		AssociatedWithGameAction = false;
	}
	
	void CaptureTimeStamp() //call this from one of the assignment methods below. do NOT call it directly
	{
		GM = FFTGameManager.Instance;
		SecondsSinceLevelLoad = GM.LevelGameplayElapsedTime;
		GameState = GM.State;
	}
	
	public enum InteractionType //these apply to any UI elements the player can interact with 
								//these may or may not change states (Ex. Dish can be selectable, then not)
	{
		Entered,
		Exited,
		Selected
	}
	
	#region Association of data with a game action (step, dish)
	
	public bool AssociatedWithGameAction = false;
	
	public int DishID = 0;
	public string DishUID = "";
	public int StepID = 0;
	
	void AssignStepInformation(FFTStep step, FFTDish dish)
	{
		if (AssociatedWithGameAction)
			throw new System.Exception("Do not assign Step Information more than once to a single report. Generate a new report.");
		AssociatedWithGameAction = true;
		StepID = step.Index;
		DishID = dish.ID;
	}
	
	void AssignDishInformation(FFTDish dish)
	{
		AssociatedWithGameAction = true;
		DishID = dish.ID;
	}
	
	#endregion
	
	
	#region Slot Action Variables
	
	//these should reflect all the possible actions that can be taken with a slot
	//the game logic itself should determine which type of action it is
	public enum SlotActionType
	{
		Error,
		Inactive_DishCounter_EmptySlot,
		Inactive_DishCounter_FinishedDish, //only the last step of a dish should have these
		Inactive_DishStation_EmptySlot,
		Actionable_DishCounter,
		Actionable_DishStation_ElapsedTime,
		Actionable_DishStation_Minigame,
		Button_Planning_DismissRecipeCard,
		Button_Planning_ShowExtraSteps,
		Button_Planning_HideExtraSteps,
		Button_Results_OpenDetail,
		Button_Results_CloseDetail
	}
	
	public FFTSlot.SlotType Slot_TypeOfSlot;
	public FFTStepAction.InteractionType Slot_InteractionType;
	public SlotActionType Slot_ActionTaken;
	
	public FFTStation.Type Slot_Station_Type;	
	public int Slot_Station_SlotID;
	
	public void AssignSlotAction(FFTSlot slot, InteractionType interactionType, SlotActionType actionTaken)
	{
		//check first to see if this has not already been assigned
		if (Type != ActionType.Empty)
		{
			throw new System.Exception("Do not assign an action more than once to a single report. Generate a new report.");
			//Debug.Log("LOG_ERROR: Only one type of action may be logged at a time.");	
		}
		//get the timestamp next for accuracy
		SecondsSinceLevelLoad = FFTGameManager.Instance.LevelElapsedTimeTotal;
		//CaptureTimeStamp();
		GameState = FFTGameManager.Instance.State;
		
		//if we have a dish in our slot (isOccupied), write dish/step data to our report
		if (slot.Occupied)
		{
			AssignStepInformation(slot.Dish.CurrentStepObject, slot.Dish);	
		}
		
		//assign the type of action it is (that is how we will grab the correct data for xml logging later)
		Type = ActionType.Slot;
		
		//assign variables for data we want to log to XML when the level is concluded.
		Slot_ActionTaken = actionTaken;
		Slot_TypeOfSlot = slot.Type;
		Slot_InteractionType = interactionType;
		//Slot_StationType =
		
	}
	
	public void AssignButtonAction(InteractionType interactionType, SlotActionType actionTaken)
	{
		SecondsSinceLevelLoad = FFTGameManager.Instance.LevelElapsedTimeTotal;
		GameState = FFTGameManager.Instance.State;
		
		Type = ActionType.Button;
		Slot_ActionTaken = actionTaken;
		Slot_InteractionType = interactionType;
	}
	
	#endregion
	

	
	
	
}
