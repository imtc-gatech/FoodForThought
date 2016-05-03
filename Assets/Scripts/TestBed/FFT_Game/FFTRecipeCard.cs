using UnityEngine;
using System.Collections;

public class FFT_RecipeCard : MonoBehaviour {

    public FFTRecipeMaker RecipeBase;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public GaugeParameters ReturnTimingInformation()
    {
        GaugeParameters parameters = new GaugeParameters();
        return parameters;

    }
}
