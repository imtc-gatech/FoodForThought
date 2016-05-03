using UnityEngine;
using System.Collections;

public class FireExtinguisherBehavior : MonoBehaviour {
	public GameObject DownState;
	public GameObject UpState;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.active){
			if(Input.GetMouseButton(0)){
				DownState.SetActiveRecursively(true);
				UpState.SetActiveRecursively(false);
			}
			else{
				UpState.SetActiveRecursively(true);
				DownState.SetActiveRecursively(false);
			}
		}
	}
}
