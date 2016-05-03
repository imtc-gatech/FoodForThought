using UnityEngine;
using System.Collections;

public class FFTDistractionView : MonoBehaviour {
	
	GameObject viewGameObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy() {
			if (viewGameObject != null)
				FFTUtilities.DestroySafe(viewGameObject);
	}
}
