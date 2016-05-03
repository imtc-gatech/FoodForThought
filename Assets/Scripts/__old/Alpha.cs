using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Alpha : MonoBehaviour {
	
	public float alpha = 1.0f;
	private float prevAlpha;
	private float refreshThreshold = 0.1f;
	private float alphaDifference;
	
	private Component[] rageSplines;

	// Use this for initialization
	void Start () {
		prevAlpha = alpha;
	
	}
	
	// Update is called once per frame
	void Update () {	
		alphaDifference += alpha - prevAlpha;
		if (Mathf.Abs(alphaDifference) > refreshThreshold)
		{
			alphaDifference = 0;
			rageSplines = GetComponentsInChildren(typeof(RageSpline));
			foreach (Component rsc in rageSplines)
			{
				RageSpline rs = (RageSpline)rsc;
				Color rsFill = rs.GetFillColor1();
				Color rsOutline = rs.GetOutlineColor1();
				rsFill.a = alpha;
				rsOutline.a = alpha;
				rs.SetFillColor1(rsFill);
				rs.SetOutlineColor1(rsOutline);
				rs.RefreshMesh(true, false, false);
			}
			//Debug.Log("Change alpha transparency of RageSplines in GO here.");
		}
		if (alpha > 1)
		{
			alpha = 1;
		}
		else if (alpha < 0)
		{
			alpha = 0;
		}
		prevAlpha = alpha;
	}
}
