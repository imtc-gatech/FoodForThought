using UnityEngine;
using System.Collections;

public class DIS_FlameBehaviorAlt: FFTDistraction {
	
	public Object Smoke; //not used anymore.  was used in spawning smoke
	public DIS_SmokeBehaviorAlt[] Smokes; //list that contains the smokes to be activated and deactivated
	private float counter = 0;
	private int smokeCycle = 0;
	private int arraySize;
	public float FireLife = 100f;
	public GameObject Fire;
	private BoxCollider collider;
	
	public FFTSlot AffectedSlot;
	public float SlotSize;
	
	public DIS_FlameBehaviorAlt otherFlame;
	public bool SmokeActive = false;
	public bool RandomSlot = false;
	public int FiresPerStation = 2;
	// Use this for initialization
	void Awake () {
		OwnType = Type.Fire;
		DoesBlock = true;
		EffectMultiplier = 1f;
		
		//this little ugly bit tells distraction where to go based off of its station type.  If there is a better reference point than the hardcoded values that I'm using, let me know
		switch(AffectedStation){
		case FFTStation.Type.Chop:
			transform.position = new Vector3(-9.971f,60.35397f,-103.5f); //using position as opposed to localPosition due to the fact that this is an awake method
			break;
		case FFTStation.Type.Cook:
			transform.position = new Vector3(-128.6718f,60.35397f,-103.5f);
			break;
		case FFTStation.Type.Spice:
			transform.position = new Vector3(-128.6718f,-38.9f,-103.5f);
			break;
		case FFTStation.Type.Prep:
			transform.position = new Vector3(-9.971f,-38.9f,-103.5f);
			break;
		default:
			Debug.Log("NoStation");
			break;
		}
		
		
		SlotSize = 1f;
		
		//gives us a variable to reference later so that it knows that it's a box collider.  very important for collider positioning
		collider = gameObject.GetComponent<Collider>() as BoxCollider;
		//gives us a station to work with instead of repeatedly making calls to it
		FFTStation tempStation = FFTGameManager.Instance.Kitchen.Stations[AffectedStation];
		
		//part that really decides the slotsize variable
		if(tempStation.SlotList.Count>2){
			SlotSize = .75f;
		}
		
		ChooseSlot(tempStation); //finds out which slots are available and chooses randomly between them
		if(AffectedSlot != null){
			setup(AffectedSlot.transform.position);
		}
		
		//starts the smoke out invisible
		foreach(DIS_SmokeBehaviorAlt smokeB in Smokes){
			smokeB.SetActive(false);
		}
		arraySize = Smokes.Length;
	}
	
	void Start(){
		
		if(Manager != null){
			otherFlame = Manager.StationContainsDistraction(AffectedStation, this) as DIS_FlameBehaviorAlt;
			
			//LOG fire creation, move this if it needs to be somewhere else
			Manager.Actions.Add (new FFTDistractionAction(FFTDistractionAction.Type.Distraction, OwnType, false, this.GetInstanceID(), FFTGameManager.Instance.LevelGameplayElapsedTime));
		}
		if(otherFlame != null){
			if(otherFlame.SmokeActive){
				SmokeActive = false;
			}
		}
		else{
			SmokeActive = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		counter += Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
		//every second
		if(counter > 1f && SmokeActive && !FFTTimeManager.Instance.GameplayPaused){
			FakeSmoke(); //make smoke appear
			counter = 0;
		}
		if(otherFlame == null){
			SmokeActive = true;
		}
	}
	
	void MakeSmoke(){
		GameObject tempObject = Instantiate(Smoke, transform.position + new Vector3(Random.Range(-.5f,.5f), Random.Range(-.5f,.5f), 0f), transform.rotation) as GameObject;
		tempObject.transform.parent = transform;
		Destroy(tempObject, 20f);
	}
	
	void FakeSmoke(){
		Smokes[smokeCycle].ActivateSmoke();
		smokeCycle ++;
		if (smokeCycle >= Smokes.Length){
			smokeCycle = 0;
		}
	}
	
	
	void OnMouseOver(){
		Debug.Log(Input.mousePosition);
		if(Input.GetMouseButton(0)){ //if the collider is clicked, shrink the fire
			FireLife -= 2f;
			Debug.Log(FireLife + "%");
		}
		if(FireLife< .5f){
			Destroy(gameObject);
		}
	}
	
	
	public void ChooseSlot(FFTStation station){
		
		if(station.SlotList.Count <=0){
			GameObject.Destroy(gameObject);
			return;
		}
		int slotNumber = Random.Range(0,station.SlotList.Count); //chooses a number
		int timeoutCount = 0;
		
		if(RandomSlot){
			while(station.SlotList[slotNumber].Obstructed && timeoutCount < 20){ //if that number is already occupied by a flame, keep trying until you find one or timeout
				slotNumber = Random.Range(0,station.SlotList.Count);
				timeoutCount ++;
			}
			if(timeoutCount >=20){//if you timeout, destroy all evidence that this object existed.
				Debug.Log("no available slots, object not created");
				Destroy(gameObject);
			}
			else{ //otherwise, set the slot that the fire will spawn in
				AffectedSlot = station.SlotList[slotNumber];
			}
		}
		else{
			while(station.SlotList[timeoutCount].Obstructed && timeoutCount + 1 < station.SlotList.Count  && timeoutCount + 1 < FiresPerStation){ //if that number is already occupied by a flame, keep trying until you find one or run out of slots
				Debug.Log("testing slot " + timeoutCount);
				timeoutCount ++;
			}
			if(timeoutCount >= station.SlotList.Count || station.SlotList[timeoutCount].Obstructed){//if all slots are occupied, destroy all evidence that this object existed.
				Debug.Log("no available slots, object not created");
				Destroy(gameObject);
			}
			else{
				AffectedSlot = station.SlotList[timeoutCount];
			}
		}
	}
	
	
	/// <summary>
	/// moves the collider and spawns the fire in the right position.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	public void setup(Vector3 position){
		AffectedSlot.Obstructed = true;
		//inverse transform point: changing worldspace to local.
		Vector3 localPosition = transform.InverseTransformPoint(position);
		//sets the position and size of the collider to fit the parameters
		collider.center = new Vector3(localPosition.x, localPosition.y, collider.center.z);
		collider.size = new Vector3(collider.size.x*SlotSize, collider.size.y*SlotSize, collider.size.z*SlotSize);
		
		//makes the flame object over the right slot
		GameObject flame = GameObject.Instantiate(Fire, new Vector3(position.x, position.y, position.z - 8f),this.transform.rotation) as GameObject;
		flame.transform.parent = gameObject.transform;
		
	}
	
	/// <summary>
	/// When the object is destroyed, it removes the effects of the distraction, removes it from the active list of 
	/// distractions, and makes the button disappear if there are no more distractions of that type.
	/// </summary>
	public void OnDestroy(){
		//LOG fire destruction, move this if it needs to be somewhere else
		Manager.Actions.Add (new FFTDistractionAction(FFTDistractionAction.Type.Distraction, OwnType, false, this.GetInstanceID(), FFTGameManager.Instance.LevelGameplayElapsedTime));
		
		if(AffectedSlot!= null){
			AffectedSlot.Obstructed = false;
		}
		FFTGameManager.Instance.Kitchen.RemoveDistractionEffect(AffectedStation);
		Manager.CurrentDistractions.Remove(this);
		if(!Manager.ContainsDistraction(Type.Fire)){
			Manager.FireButton.Disappear();
		}
	}
	
	override public void MakeActive(){
		Debug.Log("makeActive Triggered");
		gameObject.GetComponent<Collider>().enabled = true;
	}
	
	override public void MakeInactive(){
		gameObject.GetComponent<Collider>().enabled = false;
	}
}
