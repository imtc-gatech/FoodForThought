using UnityEngine;
using System.Collections;

public class BGSwitch : MonoBehaviour {

    private Component backgroundSpline;

	// Use this for initialization
	void Start () {
        backgroundSpline = GetComponent(typeof(RageSpline)) as RageSpline;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Change(Color color)
    {
        RageSpline rs = (RageSpline)backgroundSpline;
        rs.SetFillColor1(color);
        rs.RefreshMesh(true, false, false);

    }
}
