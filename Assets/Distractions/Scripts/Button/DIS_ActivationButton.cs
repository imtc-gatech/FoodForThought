using UnityEngine;
using System.Collections;

/// <summary>
/// DIS_ActivationButton.  This is the button used to activate the various tools that let you handle distractions.
/// </summary>
public class DIS_ActivationButton : MonoBehaviour {
	
	public FFTDistraction.Type DistractionType;
	public GameObject upState;
	public GameObject downState;
	
	
	private FFTDistractionManager manager;
	private bool alreadyPressed = false;
	private Hashtable appearTable;
	private Hashtable disappearTable;
	
	private Vector3 colliderBaseSize = new Vector3(55f,30f,50f);
	private Vector3 colliderLargeSize =  new Vector3(55f,60f,50f);
	private BoxCollider buttonCollider;
	// Use this for initialization
	void Start () {
		buttonCollider = gameObject.GetComponent<Collider>() as BoxCollider;
		
		manager = transform.parent.parent.gameObject.GetComponent<FFTDistractionManager>();
		appearTable = new Hashtable();
		appearTable.Add("scale", transform.localScale);
		appearTable.Add("time", .5f);
		
		disappearTable = new Hashtable();
		disappearTable.Add("scale", new Vector3(.1f,.1f,.1f));
		disappearTable.Add("time", .5f);
		disappearTable.Add("onComplete", "setActive");
		disappearTable.Add("onCompleteParams", false);
		
		this.transform.localScale.Set(.1f,.1f,.1f);
		gameObject.SetActiveRecursively(false);
		Appear();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/// <summary>
	/// What happens when the button is clicked.
	/// It includes a toggle function, and can be used to activate or deactivate the distractions(through the manager).
	/// </summary>
	void OnMouseUp(){
		if (FFTTimeManager.Instance.GameplayPaused)
			return;
		if(!alreadyPressed && manager.ContainsDistraction(DistractionType)){ //button will only activate if a distraction exists
			Debug.Log("distraction button activated");
			
			//LOG button activation, move this if it needs to be somewhere else
			manager.Actions.Add (new FFTDistractionAction(FFTDistractionAction.Type.Button, DistractionType, true, this.GetInstanceID(), FFTGameManager.Instance.LevelGameplayElapsedTime));
			
			manager.RevertButtons();
			manager.ActivateDistraction(DistractionType);
			alreadyPressed = true;
			setState();
		}
		else if(alreadyPressed){
			Debug.Log("distraction button deactivated");
			
			//LOG button activation, move this if it needs to be somewhere else
			manager.Actions.Add (new FFTDistractionAction(FFTDistractionAction.Type.Button, DistractionType, false, this.GetInstanceID(), FFTGameManager.Instance.LevelGameplayElapsedTime));
			
			manager.DeactivateDistraction(DistractionType);
			alreadyPressed = false;
			setState();
		}
		else{
			Debug.Log("no distraction of that type available");
		}
	}
	
	/// <summary>
	/// makes the button appear, including animation
	/// </summary>
	public void Appear(){
		setActive(true);
		iTween.ScaleTo(gameObject, appearTable);
	}
	
	/// <summary>
	/// makes the button disappear, including animation
	/// </summary>
	public void Disappear(){
		manager.RevertHand();
		manager.DeactivateDistraction(DistractionType);
		manager.CheckToLowerButtonHolder();
		alreadyPressed = false;
		iTween.ScaleTo(gameObject, disappearTable);
		
	}
	
	/// <summary>
	/// makes the button visible or invisible, depending on the parameter.
	/// </summary>
	/// <param name='active'>
	/// Active.
	/// </param>
	public void setActive(bool active){
		gameObject.SetActiveRecursively(active);
		setState();
	}
	
	/// <summary>
	/// Makes sure the button is in the right toggled state
	/// </summary>
	public void setState(){
		if(gameObject.active){
			if(!alreadyPressed){
				upState.SetActiveRecursively(true);
				downState.SetActiveRecursively(false);
				buttonCollider.size = colliderBaseSize;
			}
			else{	
				upState.SetActiveRecursively(false);
				downState.SetActiveRecursively(true);
				buttonCollider.size = colliderLargeSize;
			}
		}
	}
	
	public bool getPressed(){
		return alreadyPressed;
	}
	
	public void setPressed(bool pressed){
		alreadyPressed = pressed;
	}

	public void OnMouseEnter(){
		if (FFTTimeManager.Instance.GameplayPaused)
			return;
		iTween.ScaleTo(gameObject, new Vector3(.9f,.9f,1f),.01f);
	}
	
	public void OnMouseExit(){
		if (FFTTimeManager.Instance.GameplayPaused)
			return;
		iTween.ScaleTo(gameObject, new Vector3(.8f,.8f,1f),.01f);
	}
}
