using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// FFTDistractionManager works as a relay between all of the distractions and their various items.
/// As it is, it takes in each distraction based off it's type (with a smidge of variability.  
/// Later distractions need to be added into this class for them to function properly.
/// </summary>
public class FFTDistractionManager : MonoBehaviour {
	
	public bool UseDebugKeys = true;
	
	public List<FFTDistractionAction> Actions;
	
	public DIS_DistractionButtonHolderView ButtonHolder;
	
	private FFTDistraction currentDistraction;
	public GameObject FlyDistraction;
	public GameObject FireDistraction;
	public List<FFTDistraction> CurrentDistractions = new List<FFTDistraction>(); //ArrayList();
	
	public FFTDistraction.Type CurrentType = FFTDistraction.Type.None;
	public DIS_ActivationButton FireButton;
	public DIS_ActivationButton BugButton;
	
	private DIS_FlameBehaviorAlt flameBase;
	private DIS_BugBehavior flyBase;
	// Use this for initialization
	void Start () {
		flameBase = FireDistraction.GetComponent<DIS_FlameBehaviorAlt>();
		flyBase = FlyDistraction.GetComponent<DIS_BugBehavior>();
		if (UseDebugKeys)
			gameObject.AddComponent<FFTDistractionManagerDebug>();
		Actions = new List<FFTDistractionAction>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	/// <summary>
	/// Spawns the distraction based off of which type you input.  This includes adding the effect of that distraction to the kitchen
	 /// and adding the distraction itself to the current list of distractions
	/// </summary>
	/// <param name='distractionType'>
	/// Distractiontype.
	/// </param>
	public void SpawnDistraction(FFTDistraction.Type distractionType){
		GameObject tempDistraction;
		switch (distractionType){
		case FFTDistraction.Type.Fly:
			//makes a fly distraction
			tempDistraction = Instantiate(FlyDistraction) as GameObject;
			//puts the distraction inside the manager
			tempDistraction.transform.parent = transform;
			//assigns the currentDistraction variable to the fly distraction
			currentDistraction = tempDistraction.GetComponent<FFTDistraction>();
			
			if(!ContainsDistraction(distractionType)){
				BugButton.Appear();
			}
			break;
		case FFTDistraction.Type.Fire:
			flameBase.AffectedStation = FFTStation.Type.Cook;
			//makes a fire distraction
			tempDistraction = Instantiate(FireDistraction) as GameObject;
			//puts the distraction inside the manager
			tempDistraction.transform.parent = transform;
			//assigns the currentDistraction variable to the fire distraction
			currentDistraction = tempDistraction.GetComponent<FFTDistraction>();
			if(!ContainsDistraction(distractionType)){
				FireButton.Appear();
			}
			break;
		default:
			Debug.Log("distraction not specified");
			break;
		}
		if(currentDistraction != null){ //if it assigned something.  More robust version of this check coming soon
			Debug.Log("distraction spawned properly");
			 //sets the manager variable in the distraction (currently does nothing) to this
			currentDistraction.Manager = this;
			//makes sure to add the proper effect to the kitchen (hard to check right now due to the fact that the only available distraction is fly).
			FFTGameManager.Instance.Kitchen.AddDistractionEffect(currentDistraction.AffectedStation, currentDistraction.EffectMultiplier, currentDistraction.DoesBlock);
			
			//puts the current distraction in the list of all the existing distractions
			CurrentDistractions.Add(currentDistraction);
			
			ButtonHolder.Activate();
			
			//if the distraction is of the same type as whichever button's selected, make it able to be affected
			if(CurrentType == distractionType){
				currentDistraction.MakeActive();
			}
		}
	}
	
	public void SpawnDistraction(FFTDistraction.Type distractiontype, FFTStation.Type stationType){
		GameObject tempDistraction;
		switch (distractiontype){
		case FFTDistraction.Type.Fly:
			//makes a fly distraction
			tempDistraction = Instantiate(FlyDistraction) as GameObject;
			//puts the distraction inside the manager
			tempDistraction.transform.parent = transform;
			//assigns the currentDistraction variable to the fly distraction
			currentDistraction = tempDistraction.GetComponent<FFTDistraction>();
			
			if(!ContainsDistraction(distractiontype)){
				BugButton.Appear();
			}
			break;
		case FFTDistraction.Type.Fire:
			flameBase.AffectedStation = stationType;
			//makes a fire distraction
			tempDistraction = Instantiate(FireDistraction) as GameObject;
			//puts the distraction inside the manager
			tempDistraction.transform.parent = transform;
			//assigns the currentDistraction variable to the fire distraction
			currentDistraction = tempDistraction.GetComponent<FFTDistraction>();
			currentDistraction.AffectedStation = stationType;
			if(!ContainsDistraction(distractiontype)){
				FireButton.Appear();
			}
			break;
		default:
			Debug.Log("distraction not specified");
			break;
		}
		if(currentDistraction != null){ //if it assigned something.  More robust version of this check coming soon
			Debug.Log("distraction spawned properly");
			
			
			 //sets the manager variable in the distraction (currently does nothing) to this
			currentDistraction.Manager = this;
			//makes sure to add the proper effect to the kitchen (hard to check right now due to the fact that the only available distraction is fly).
			FFTGameManager.Instance.Kitchen.AddDistractionEffect(currentDistraction.AffectedStation, currentDistraction.EffectMultiplier, currentDistraction.DoesBlock);
			
			//puts the current distraction in the list of all the existing distractions
			CurrentDistractions.Add(currentDistraction);
			
			ButtonHolder.Activate();
			
			//if the distraction is of the same type as whichever button's selected, make it able to be affected
			if(CurrentType == distractiontype){
				currentDistraction.MakeActive();
			}
		}
	}
	
	/// <summary>
	/// This is the method that is called when you click one of the distraction buttons.  
	/// It makes sure all other distractions become deactivated.
	/// It also changes the hand cursor according to the distraction.
	/// </summary>
	/// <param name='distractionType'>
	/// Distraction type.
	/// </param>
	public void ActivateDistraction(FFTDistraction.Type distractionType){
		if(CurrentType != distractionType){ //turns the other distraction if there is one active.
			DeactivateDistraction(CurrentType);
		}
		
		//big switch statement, because I haven't figured out a better way yet
		switch(distractionType){
		case FFTDistraction.Type.Fire:
			foreach(FFTDistraction distraction in CurrentDistractions){//goes through the list of currently living distractions
				if(distraction.GetType().Equals(typeof(DIS_FlameBehaviorAlt))){  //if one is of the same type as called in this method
					//calls method in the distraction itself to make it active
					distraction.MakeActive();
				}
			}
			//change hand to right icon
			break;
		case FFTDistraction.Type.Fly:
			foreach(FFTDistraction distraction in CurrentDistractions){
				if(distraction.GetType().Equals(typeof(DIS_BugBehavior))){
					distraction.MakeActive();
				}
			}
			break;
		case FFTDistraction.Type.None:
			break;
		}
		CurrentType = distractionType;
	}
	
	/// <summary>
	/// Checks to see if there is currently a distraction of the queried type within the list of active distractions.
	/// </summary>
	/// <returns>
	/// true or false.
	/// </returns>
	/// <param name='distractionType'>
	/// The distraction type.
	/// </param>
	public bool ContainsDistraction(FFTDistraction.Type distractionType){
		switch(distractionType){
		case FFTDistraction.Type.Fire:
			foreach(FFTDistraction distraction in CurrentDistractions){
				if(distraction.GetType().Equals(typeof(DIS_FlameBehaviorAlt))){
					return true;
				}
			}
			return false;
		case FFTDistraction.Type.Fly:
			foreach(FFTDistraction distraction in CurrentDistractions){
				if(distraction.GetType().Equals(typeof(DIS_BugBehavior))){
					return true;
				}
			}
			return false;
		case FFTDistraction.Type.None:
			return false;
		}
		return false;				
	}
	
	
	/// <summary>
	/// Checks to see if the station in question is affected by any of the distractions currently in place
	/// </summary>
	/// <returns>
	/// true or false
	/// </returns>
	/// <param name='station'>
	/// the station you're checking
	/// </param>
	public bool StationAffected(FFTStation station){
		foreach(FFTDistraction distraction in CurrentDistractions){
			if(distraction.AffectedStation.Equals(station)){
				return true;
			}
		}
		return false;
	}
	
	public FFTDistraction StationContainsDistraction(FFTStation.Type station,FFTDistraction distraction){
		FFTDistraction.Type disType = distraction.OwnType;
		
		foreach(FFTDistraction dis in CurrentDistractions){
			if(dis.AffectedStation.Equals(station) && !dis.Equals(distraction) && dis.OwnType.Equals(disType)){
				return dis;
			}
		}
		return null;
	}
	
	/// <summary>
	/// Deactivates the distraction, making its collider disappear.
	/// This includes changing the cursor back to normal.
	/// </summary>
	/// <param name='distractionType'>
	/// Distraction type.
	/// </param>
	public void DeactivateDistraction(FFTDistraction.Type distractionType){
		Debug.Log("deactivate activated");
		switch(distractionType){
		case FFTDistraction.Type.Fire:
			foreach(FFTDistraction distraction in CurrentDistractions){
				if(distraction.GetType().Equals(typeof(DIS_FlameBehaviorAlt))){
					distraction.MakeInactive();
				}
			}
			break;
		case FFTDistraction.Type.Fly:
			foreach(FFTDistraction distraction in CurrentDistractions){
				if(distraction.GetType().Equals(typeof(DIS_BugBehavior))){
					distraction.MakeInactive();
				}
			}
			break;
		case FFTDistraction.Type.None:
			break;
		}
		CurrentType = FFTDistraction.Type.None;
	}
	
	/// <summary>
	/// Clears all distractions from the list of CurrentDistractions and makes sure they are all destroyed.
	/// This method is usually called at the end of a level.
	/// </summary>
	public void ClearAllDistractions() 
	{
		//LOG denote the system cleared the distractions (generally at the end of a level), the only arguments used are action type and timestamp
		Actions.Add (new FFTDistractionAction(FFTDistractionAction.Type.LevelFinished, FFTDistraction.Type.None, true, this.GetInstanceID(), FFTGameManager.Instance.LevelGameplayElapsedTime));
		foreach(FFTDistraction distraction in CurrentDistractions)
		{
			distraction.MakeInactive();
			GameObject.Destroy(distraction.gameObject);
		}
		ButtonHolder.Deactivate();
	}
	
	
	/// <summary>
	/// Reverts the hand back to its pointing state.  This usually occurs after a button has been deactivated or all of the distractions of one type have been removed.
	/// </summary>
	public void RevertHand(){
			
	}
	
	public void RevertButtons(){
		BugButton.setPressed(false);
		FireButton.setPressed(false);
		BugButton.setState();
		FireButton.setState();
		foreach(FFTDistraction dis in CurrentDistractions){
			dis.MakeInactive();
		}
	}
	
	public void CheckToLowerButtonHolder(){
		if(!ContainsDistraction(FFTDistraction.Type.Fire) && !ContainsDistraction(FFTDistraction.Type.Fly)){
			ButtonHolder.Deactivate();
		}
	}
	
	//LOGGING
	
	/// <summary>
	/// Logs the actions at the end of a level or when the game is quit. Do not call this method until you are finished with a level as it resets the accumulated actions.
	/// </summary>
	public void LogActions() {
		
		FFTGameManager.Instance.DataManager.CreateDistractionFragment(Actions);		
		Actions = new List<FFTDistractionAction>();
	}
	
	
}
