using UnityEngine;
using System.Collections;

public class DIS_ReversionButton : MonoBehaviour {
	
	private FFTDistractionManager manager;
	// Use this for initialization
	void Start () {
		manager = transform.parent.gameObject.GetComponent<FFTDistractionManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseUp(){
		manager.RevertHand();
		manager.RevertButtons();
	}
}
