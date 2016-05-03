using UnityEngine;
using System.Collections;

public class ProgressBarController : MonoBehaviour {

    public float LeftSize
    {
        get { return _leftSize; }
        set
        {
            if (value != _leftSize)
            {
                Left.EnergyLevel = value;
                _leftSize = value;
            }

        }
    }
    [SerializeField]
    private float _leftSize = 0.33f;
    public float RightSize
    {
        get { return _rightSize; }
        set
        {
            if (value != _rightSize)
            {
                Right.EnergyLevel = value;
                _rightSize = value;
            }

        }
    }
    [SerializeField]
    private float _rightSize = 0.33f;

    public ProgressBar Left;
    public ProgressBar Center;
    public ProgressBar Right;

    public float Width
    {
        get { return _width; }
        set
        {
            _width = value;
            Vector3 newScale = gameObject.transform.localScale;
            newScale.x = _width;
            gameObject.transform.localScale = newScale;
        }
    }
    [SerializeField]
    private float _width = 100f;
    public float Height
    {
        get { return _height; }
        set 
        { 
            _height = value; 
            Vector3 newScale = gameObject.transform.localScale;
            newScale.y = _height;
            gameObject.transform.localScale = newScale;
        }
    }
    [SerializeField]
    private float _height = 40f;

	// Use this for initialization
	void Start () {
        Left.EnergyLevel = LeftSize;
        Right.EnergyLevel = RightSize;
	}
	
	// Update is called once per frame
	void Update () {
        
	
	}

    void SetDisplay(Vector3 parameters)
    {
        Left.EnergyLevel = parameters.x;
        Center.EnergyLevel = 1; // ignore middle since it is layered behind the other two
        Right.EnergyLevel = parameters.z;

    }

}
