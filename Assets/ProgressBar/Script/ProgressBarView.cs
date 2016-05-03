using UnityEngine;
using System.Collections;

public class ProgressBarView : MonoBehaviour {

    public Shader OutlineShader;
    public float Under
    {
        get { return _under; }
        set { _under = value; UpdateDisplay(); }
    }
    [SerializeField]
    private float _under = 0.33f;
    public float Even
    {
        get { return _even; }
        set { _even = value; UpdateDisplay(); }
    }
    [SerializeField]
    private float _even = 0.33f;
    public float Over
    {
        get { return _over; }
        set { _over = value; UpdateDisplay(); }
    }
    [SerializeField]
    private float _over = 0.33f;
    public float Border
    {
        get { return _border; }
        set { _border = value; UpdateDisplay(); }
    }
    [SerializeField]
    private float _border = 0.03f;

    public GameObject Display;
    public Transform Left;
    public Transform Right;
    public ProgressBarIndicatorController IndicatorObject;

    public GameObject Indicator;

    public float IndicatiorPosition
    {
        get
        {
            return _indicatorPosition;
        }
        set
        {
            _indicatorPosition = Mathf.Clamp(value, 0, 1);
            IndicatorObject.Position = _indicatorPosition;
        }
    }
    private float _indicatorPosition = 0.0f;


    public Vector3 DisplayScale
    {
        get { return _displayScale; }
        set
        {
            if (value != _displayScale)
            {
                Display.transform.localScale = value;
                _displayScale = value;
            }
        }
    }
    [SerializeField]
    private Vector3 _displayScale = new Vector3(1, 1, 1);

    public float tempIndicator = 0.0f;

	// Use this for initialization
	void Start () {
        Display.GetComponent<Renderer>().material = new Material(OutlineShader);
	}
	
	// Update is called once per frame
	void Update () {
        Border = 0.05f;
        tempIndicator += 0.01f;
        if (tempIndicator > 1)
            tempIndicator = 0;
        IndicatiorPosition = tempIndicator;
        
        
	
	}

    void UpdateDisplay()
    {
        Display.GetComponent<Renderer>().material.SetVector("_Stripe", new Vector4(Under, Even, Over, Border));
        Display.GetComponent<Renderer>().material.SetFloat("_Outline", Border);
    }

    void UpdateDisplay(Vector4 update)
    {
        Display.GetComponent<Renderer>().material.SetVector("_Stripe", update);
        Display.GetComponent<Renderer>().material.SetFloat("_Outline", update.w);
    }
}
