using UnityEngine;
using System.Collections;

public class DIS_FireView : MonoBehaviour {
	
	public GameObject Fire1;
	public GameObject Fire2;
	public GameObject Fire3;
	public float MoveTime = 3f;
	public iTween.EaseType Ease = iTween.EaseType.easeInOutQuad;
	public DIS_FlameBehaviorAlt FireParent;
	private Vector3 pos1;
	private Vector3 pos2;
	private Vector3 pos3;
	// Use this for initialization
	void Start() {
		FireParent = transform.parent.GetComponent<DIS_FlameBehaviorAlt>();
		pos1 = Fire1.transform.position;
		pos2 = Fire2.transform.position;
		pos3 = Fire3.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (!FFTTimeManager.Instance.GameplayPaused)
		{	
			MoveFire();
			if(FireParent != null){
				transform.localScale = new Vector3((FireParent.FireLife/100f)*FireParent.SlotSize, FireParent.SlotSize,FireParent.FireLife/100f);
			}
		}
	}
	
	void MoveFire(){
		Hashtable hash = new Hashtable();
		hash.Add("time", MoveTime);
		hash.Add("easetype", Ease);
		
		if(Fire2.transform.position == pos2 && Fire3.transform.position == pos3){
			hash.Add("position", pos1);
			iTween.MoveTo(Fire2, hash);
			iTween.MoveTo(Fire3, hash);
		}
		else if(Fire2.transform.position == pos1 && Fire3.transform.position == pos1){
			hash.Add("position", pos2);
			iTween.MoveTo(Fire2, hash);
			hash["position"] = pos3;
			iTween.MoveTo(Fire3, hash);
		}
		//iTween.ShakePosition(Fire1, new Vector3(1f, 0f, 0f), 10f);
		
	}
}
