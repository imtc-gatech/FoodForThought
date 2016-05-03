using UnityEngine;
using System.Collections;

public class DIS_DistractionButtonHolderView : MonoBehaviour {
	
	private Vector3 activePosition;
	private Vector3 inactivePosition;
	private float transitionTime;
	public bool InMotion;
	public bool Active;
	Hashtable tweenTable;
	// Use this for initialization
	void Start () {
		Active = true;
		InMotion = false;
		activePosition = new Vector3(-65f,-89.5f, -103.5f);
		inactivePosition = new Vector3(-65f,-115f, -103.5f);
		transitionTime = 1f;
		
		//generate hashtable for setting to false on complete
		tweenTable = new Hashtable();
		tweenTable.Add("position", activePosition);
		tweenTable.Add("time", transitionTime);
		tweenTable.Add("onComplete", "endMotion");
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Activate(){
		if(Active){
			return;
		}
		if(!InMotion){
			tweenTable["position"] = activePosition;
			InMotion = true;
			Active = true;
			iTween.MoveTo(this.gameObject, tweenTable);
		}
		else{
			Invoke("Activate",transitionTime);
		}
	}
	
	public void Deactivate(){
		if(!Active){
			return;
		}
		if(!InMotion){
			tweenTable["position"] = inactivePosition;
			InMotion = true;
			Active = false;
			iTween.MoveTo(this.gameObject, tweenTable);
		}
		else{
			Invoke("Deactivate",transitionTime);
		}
	}
		
	public void SetTransitionTime(float time){
		transitionTime = time;
	}
	
	private void endMotion(){
		InMotion = false;
	}
}
