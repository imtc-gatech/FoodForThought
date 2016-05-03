using UnityEngine;
using System.Collections;

public class FFTWindowShade : MonoBehaviour {

    IRageSpline spline;
    Color outline;
    Color fill;

    public static float X_POSITION_OFF_SCREEN = -355f;

    public GUIText HUD;
	
	public Vector3 OriginalPosition;

	// Use this for initialization
	void Start () {
		spline = GetComponent<RageSpline>() as IRageSpline;
        outline = spline.GetOutlineColor1();
        fill = spline.GetFillColor1();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Setup()
    {
        iTween.MoveTo(gameObject, iTween.Hash("x", 0, "time", 0.0f));
		OriginalPosition = gameObject.transform.localPosition;
    }

    public void Open()
    {
     	spline = GetComponent<RageSpline>() as IRageSpline;
        outline = spline.GetOutlineColor1();
        fill = spline.GetFillColor1();
		
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 2.5f, "onupdate", "FadeSpline", "delay", 0.5f));
        //iTween.MoveTo(gameObject, iTween.Hash("x", -355, "time", 0.0f));
        iTween.MoveTo(gameObject, iTween.Hash("x", -355, "time", 3.0f));

        //iTween.MoveTo(gameObject, iTween.Hash("y", 225, "time", 0.0f));
        //iTween.MoveFrom(gameObject, iTween.Hash("y", -225, "time", 5.0f));
    }

    public void Close()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 1.5f, "onupdate", "FadeSpline", "delay", 0.25f));

        iTween.MoveTo(gameObject, iTween.Hash("x", 0, "time", 2.0f));
    }

    public void CloseImmediate()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.01f, "onupdate", "FadeSpline"));

        iTween.MoveTo(gameObject, iTween.Hash("x", 0, "time", 2.0f));
    }

    void FadeSpline(float val)
    {
        //Debug.Log(val);
        outline.a = val;
        fill.a = val;
        spline.SetOutlineColor1(outline);
        spline.SetFillColor1(fill);
        spline.RefreshMesh(false, false, false);
    }

    public void SetText(string text, bool sendToDebugLog)
    {
        HUD.text = text;
        if (sendToDebugLog)
            Debug.Log(text);
    }
	
	public void ResetPosition()
	{
		gameObject.transform.localPosition = OriginalPosition;
		FadeSpline(1);
	}
}
