using UnityEngine;
using System.Collections;

public class DIS_BugBehavior : FFTDistraction {
	
	public float MoveDist;
	public RageSpline boundSpline;
	private float maxX;
	private float minX;
	private float maxY;
	private float minY;
	private float timeElapsed;
	public bool alive = true;
	private FFTKitchen kitchen;
	private float waitTime = 1f;
	public bool IsMoving = false;
	
	public GameObject highlight;
	// Use this for initialization
	void Start () {
		OwnType = Type.Fly;
		//IsMoving = false;
		//waitTime = 1f;
		EffectMultiplier = 1f;
		DoesBlock = true;
		kitchen = FFTGameManager.Instance.Kitchen;
		
		/*
		maxX = boundSpline.GetBounds().xMax;
		maxY = boundSpline.GetBounds().yMax;
		minX = boundSpline.GetBounds().xMin;
		minY = boundSpline.GetBounds().yMin;
		*/
		
		//current hardcoded values for the screen edges in worldspace.
		maxX = 47f;
		maxY = 100f;
		minX = -180f;
		minY = -70f;
		
		timeElapsed = 0f;
		highlight.SetActiveRecursively(false);
		transform.position = FindDestination();
		
		//LOG fly creation, move this if it needs to be somewhere else
		Manager.Actions.Add (new FFTDistractionAction(FFTDistractionAction.Type.Distraction, OwnType, true, this.GetInstanceID(), FFTGameManager.Instance.LevelGameplayElapsedTime));
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
		if(timeElapsed > waitTime && alive && !FFTTimeManager.Instance.GameplayPaused){
			Move();
			timeElapsed = 0;
			waitTime = Random.Range(1f,4f);
		}
	}
	
	void Move(){
		Vector3 destination = FindDestination();
		if(destination != null){
			IsMoving = true;
			float traveldist = Vector3.Distance(transform.position,destination);
			//StartCoroutine(ChangeStations(destination,traveldist));
			
			//this hashtable and its arguments are required for the iTween, specifically to perform the "oncomplete" function,
			//which allows a method to be called when the tween is done.
			Hashtable hash = new Hashtable();
			hash.Add("position",destination);
			hash.Add("time",traveldist/50f);
			hash.Add("oncomplete","ChangeStations");
			hash.Add("oncompleteparams", transform.position);
			
			iTween.MoveTo(gameObject, hash);
		}
	}
	
	Vector3 FindDestination(){
		float x = Random.Range(-1 * MoveDist,MoveDist) + transform.position.x;
		float y = Random.Range(-MoveDist,MoveDist) + transform.position.y;
		float z = transform.position.z;
		if(x < minX){
			x = Random.Range(0f, MoveDist);
		}
		if(x > maxX){
			x = Random.Range(-MoveDist, 0f);
		}
		if(y < minY){
			y = Random.Range(0f, MoveDist);
		}
		if(y > maxY){
			y = Random.Range(-MoveDist, 0f);
		}
		return new Vector3(x,y,z);
	}
	
	void OnMouseDown(){
		Debug.Log("hit");
		Die();
	}
	
	void Die(){
		alive = false;
		Destroy(gameObject, .5f);
		Debug.Log("start tween");
		iTween.ScaleAdd(gameObject, new Vector3(.5f,-.5f,0f), .5f);
	}
	
	public bool GetAlive(){
		return alive;
	}
	
	public void OnDestroy(){ //removes the effect of the fly on whatever station it was on when it was destroyed.
		//LOG fly destruction, move this if it needs to be somewhere else
		Manager.Actions.Add (new FFTDistractionAction(FFTDistractionAction.Type.Distraction, OwnType, false, this.GetInstanceID(), FFTGameManager.Instance.LevelGameplayElapsedTime));
		
		Debug.Log("dead");
		kitchen.RemoveDistractionEffect(kitchen.CurrentStationTypeFromCoordinates((Vector2)transform.position));
		Manager.CurrentDistractions.Remove(this);
		if(!Manager.ContainsDistraction(Type.Fly)){
			Manager.BugButton.Disappear();
		}
	}
	
	private void ChangeStations(Vector3 pos){
		IsMoving = false;
		Vector2 destination = (Vector2)transform.position;
		//If the fly is going to move to a different station
		if(kitchen.CurrentStationTypeFromCoordinates(pos) != kitchen.CurrentStationTypeFromCoordinates((Vector2)destination)){
			//removes the distraction effect from the station it is on before the move
			kitchen.RemoveDistractionEffect(kitchen.CurrentStationTypeFromCoordinates(pos));
			//adds the distraction effect to the station it is moving to
			kitchen.AddDistractionEffect(kitchen.CurrentStationTypeFromCoordinates((Vector2)destination), EffectMultiplier, DoesBlock);
		}
		
	}
	
	public void OnMouseEnter(){
		highlight.SetActiveRecursively(true);
	}
	
	public void OnMouseExit(){
		highlight.SetActiveRecursively(false);
	}
	
	override public void MakeActive(){
		gameObject.GetComponent<Collider>().enabled = true;
	}
	
	override public void MakeInactive(){
		Debug.Log("makeInactive run");
		gameObject.GetComponent<Collider>().enabled = false;
	}
}
