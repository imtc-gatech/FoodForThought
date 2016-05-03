using UnityEngine;
using System.Collections;

public class FFTScalableObject : MonoBehaviour {

    public static float scaleTime = 0.05f;
    public static float scaleFactor = 1.2f;
    float originalScale = 1.0f;

	// Use this for initialization
	void Start () {
        originalScale = transform.localScale.x;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter()
    {
        ScaleUp();
    }

    void OnMouseExit()
    {
        ScaleDown();
    }

    void ScaleUp()
    {
        iTween.ScaleTo(gameObject, iTween.Hash("x", scaleFactor * originalScale, "y", scaleFactor * originalScale, "time", scaleTime));
    }

    void ScaleDown()
    {
        iTween.ScaleTo(gameObject, iTween.Hash("x", originalScale, "y", originalScale, "time", scaleTime));
    }
}
