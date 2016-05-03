using UnityEngine;
using System.Collections;

public class FFTStepVisualState : MonoBehaviour {

    public GameObject Uncooked;
    public GameObject CookedNormal;
    public GameObject Burned;
    public GameObject Spoiled;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AssignObjectsFromStep(FFTStep step)
    {
        Uncooked = step.Uncooked;
        CookedNormal = step.CookedNormal;
        Burned = step.Burned;
        Spoiled = step.Spoiled;
    }
}
