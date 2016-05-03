using UnityEngine;
using System.Collections;

public class FFTTimerCircularIndicatorView : MonoBehaviour {

    MeshRenderer HUD;
    MeshRenderer Border;

    public float PercentFull
    {
        get { return _percentFull; }
        set { _percentFull = value; } // Mathf.Clamp(0, 1, value); }
    }
    [SerializeField]
    private float _percentFull = 1.0f;

	// Use this for initialization
	void Start () {
        if (HUD == null)
        {
            HUD = transform.FindChild("HUD").GetComponent<MeshRenderer>();
            Border = transform.FindChild("HUDBorder").GetComponent<MeshRenderer>();
        }
        Border.GetComponent<Renderer>().enabled = false;
        gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
	
	}
	
	// Update is called once per frame
	void Update () {
        //HUD.material.SetFloat("_Cutoff", Mathf.InverseLerp(0, Screen.width, Input.mousePosition.x));
        HUD.material.SetFloat("_Cutoff", 1 - PercentFull);
	
	}
}
