using UnityEngine;
using System.Collections;

public class FFTChloeDirection : MonoBehaviour {

    public FFTChloe.SpriteDirection Direction
    {
        get
        {
            return _direction;
        }
        set
        {

        }
    }
    [SerializeField]
    private FFTChloe.SpriteDirection _direction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
