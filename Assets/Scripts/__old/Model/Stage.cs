using UnityEngine;
using System.Collections;

public class Stage : MonoBehaviour {

    public Station.Type Destination
    {
        get
        {
            return RecipeStep.Destination;
        }
    }

    public int StepNumber;
    public Step RecipeStep
    {
        get
        {
            return transform.parent.parent.gameObject.GetComponent<Recipe>().steps[StepNumber];
        }

    }

    public GaugeParameters gaugeParameters
    {
        get
        {
            return RecipeStep.cookingParameters;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
