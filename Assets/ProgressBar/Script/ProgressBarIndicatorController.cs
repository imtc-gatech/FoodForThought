using UnityEngine;
using System.Collections;

public class ProgressBarIndicatorController : MonoBehaviour {

    private static float leftLocal = -15.0f;
    private static float rightLocal = 15.0f;
    private static float verticalPos = -6.0f;

    private static float range = 30.0f;



    public float Position
    {
        set
        {
            if (value != _position)
            {
                _position = Mathf.Clamp(value, 0, 1);

                Vector3 pos = gameObject.transform.localPosition;
                pos.x = leftLocal + range * _position;
                gameObject.transform.localPosition = pos;
            }
        }
    }
    [SerializeField]
    private float _position;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
