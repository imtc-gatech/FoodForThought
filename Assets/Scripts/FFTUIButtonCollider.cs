using UnityEngine;
using System.Collections;

public class FFTUIButtonCollider : MonoBehaviour {

    public bool Entered;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseOver()
    {
        Entered = true;
    }

    void OnMouseExit()
    {
        Entered = false;
    }
}
