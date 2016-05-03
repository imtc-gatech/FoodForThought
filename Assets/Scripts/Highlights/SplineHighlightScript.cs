using UnityEngine;
using System.Collections;

public class SplineHighlightScript : MonoBehaviour {
	public GameObject sampleObject;
	private GameObject gO;
	private IRageSpline cloneSpline;
	public Vector3 offset = new Vector3(0f,0f,.1f);
	public bool useMouseoverHighlighting = true;
	public bool useInverseHighlighting = false;
	public float HighlightWidth = 2.0f;
	// Use this for initialization
	void Awake () {
		if (sampleObject == null)
			sampleObject = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnMouseEnter(){
		if (useMouseoverHighlighting)
			SwitchHighlightOn();
	}
	
	void OnMouseExit(){
		if (useMouseoverHighlighting)
			SwitchHighlightOff();
	}
	
	public void SwitchHighlightOn()
	{
		Destroy(gO);
		gO = Instantiate(sampleObject, transform.position, transform.rotation) as GameObject;
		gO.transform.parent = sampleObject.transform;
		gO.transform.localPosition = offset;
		gO.transform.localScale = new Vector3(1,1,1);//gO.transform.parent.localScale;
		cloneSpline = gO.GetComponent(typeof(RageSpline)) as IRageSpline;
		if (cloneSpline == null)
		{
			Debug.Log("No spline found on parent! Ignoring highlight command...");
			return;
		}
		cloneSpline.SetOutlineColor1(Color.white);
		if (useInverseHighlighting)
			applyInverseHighlighting();
		cloneSpline.SetOutlineWidth(cloneSpline.GetOutlineWidth()*HighlightWidth);
		cloneSpline.RefreshMesh(false, false, false);
	}
	
	public void SwitchHighlightOff()
	{
		Destroy(gO);
	}
	
	void applyInverseHighlighting()
	{
		cloneSpline.SetOutlineColor1(Color.green);
		cloneSpline.SetOutlineColor2(Color.white);
		cloneSpline.SetOutlineGradient(RageSpline.OutlineGradient.Inverse);
		cloneSpline.SetOutlineNormalOffset(HighlightWidth / 2);
	}
}
