using UnityEngine;
using System.Collections;

public class FFTTransitionPop : MonoBehaviour {

    public Transform Origin;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Enter(float time)
    {
        if (Origin != null)
            iTween.MoveFrom(gameObject, iTween.Hash("x", Origin.position.x, "y", Origin.position.y, "time", time));
        iTween.ScaleFrom(gameObject, iTween.Hash("x", 0.1f, "y", 0.1f, "time", time));
    }

    public void Exit(float time)
    {
        if (Origin != null)
            iTween.MoveTo(gameObject, iTween.Hash("x", Origin.position.x, "y", Origin.position.y, "time", time));
        iTween.ScaleTo(gameObject, iTween.Hash("x", 0.1f, "y", 0.1f, "time", time));
    }
}
