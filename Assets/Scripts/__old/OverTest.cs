using UnityEngine;
using System.Collections;

public class OverTest : MonoBehaviour {

    float scaleMax = 1.5f;
    Vector3 currentScale;

	// Use this for initialization
	void Start () {
        currentScale = transform.localScale;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter()
    {
        //a.alpha = 0.5f;
        Debug.Log("entered");
        transform.localScale = new Vector3(currentScale.x * scaleMax, currentScale.y * scaleMax, currentScale.z * scaleMax);
    }

    void OnMouseExit()
    {
        Debug.Log("exited");
        transform.localScale = currentScale;

    }
}
