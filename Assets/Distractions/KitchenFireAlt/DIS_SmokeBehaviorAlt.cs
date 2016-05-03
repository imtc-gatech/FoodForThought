using UnityEngine;
using System.Collections;

public class DIS_SmokeBehaviorAlt: MonoBehaviour {
	
	private Hashtable tweenHash;
	public Vector3 smallScale = new Vector3(.2f,.2f,.2f);
	public Vector3 fullScale = new Vector3(1f,1f,1f);
	// Use this for initialization
	void Awake () {
		tweenHash = new Hashtable();
		tweenHash.Add("time", .2f);
		tweenHash.Add("Scale", fullScale);
		tweenHash.Add("OnComplete", "DoNothing");
		tweenHash.Add("OnCompleteParams", false);
		transform.localScale = smallScale;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseOver(){
		if (!FFTTimeManager.Instance.GameplayPaused)
			DismissSmoke();
	}
	
	void DismissSmoke(){
		tweenHash["OnComplete"] = "SetActive";
		tweenHash["Scale"] = smallScale;
		iTween.ScaleTo(gameObject, tweenHash);
	}
	
	public void ActivateSmoke(){
		SetActive(true);
		tweenHash["Scale"] = fullScale;
		tweenHash["OnComplete"] = "DoNothing";
		iTween.ScaleTo(gameObject,tweenHash);
	}
	
	public void SetActive(bool active){
		gameObject.SetActiveRecursively(active);
		gameObject.GetComponent<Collider>().enabled = active;
	}
	
	public void DoNothing(){
		
	}
}
