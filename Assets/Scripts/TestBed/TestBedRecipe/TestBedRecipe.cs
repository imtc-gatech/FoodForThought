using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestBedRecipe : MonoBehaviour {

    public List<GaugeParameters> ParametersList; 

    public Vector3 Position; // TestVariable

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnDrawGizmos()
    {
        //Camera.current gets the editor's camera, if you want to draw objects in the Scene View with your Editor script.
        //See the tutorial for more details.
    }

    public void AddParameterElement()
    {
        AddParameterElement(0); // default value if this is called with no arguments
        
    }

    public void AddParameterElement(int value)
    {
        GaugeParameters gauge = new GaugeParameters();
        gauge.Finished = value; // assigning a variable so we can see that the added objects are indeed different instances
        ParametersList.Add(gauge);
    }

}
