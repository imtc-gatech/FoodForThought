using UnityEngine;
using System.Collections;

public class WingScript : MonoBehaviour {
	private DIS_BugBehavior owner;
	// Use this for initialization
	void Start () {
		owner = transform.parent.GetComponent<DIS_BugBehavior>();
	}
	
	// Update is called once per frame
	void Update () {
		if(owner.GetAlive() && owner.IsMoving){
			iTween.ShakeRotation(gameObject,new Vector3(0f,0f,-10f), .5f);
		}
	}
}
