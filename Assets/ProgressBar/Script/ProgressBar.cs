using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {

    public Color DisplayColor;

    public float EnergyLevel
    {
        get { return _energyLevel; }
        set
        {
            if (value < 0)
                _energyLevel = 0;
            else if (value > 1)
                _energyLevel = 1;
            else
                _energyLevel = value;
        }

    }
    [SerializeField]
    private float _energyLevel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Renderer>().material.SetColor("_Color", DisplayColor);
        GetComponent<Renderer>().material.SetFloat("_Progress", EnergyLevel);
	}
}
