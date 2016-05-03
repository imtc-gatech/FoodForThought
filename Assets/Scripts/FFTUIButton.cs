using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FFTUIButton : MonoBehaviour {

    [SerializeField] 
    private bool _selected;

    public bool IsActive = true;
	public bool FeedbackEnabled = true;
	
	public bool UseLegacyTransformValue = true;

    public GameObject Down
    {
        get
        {
            if (_down == null)
            {
                InitializeUMButton(ref _down, "Down");
            }
            return _down;
        }
		set{
			_down = value;
		}
    }       
    [SerializeField]
    public GameObject _down;

    public GameObject Up
    {
        get
        {
            if (_up == null)
            {
                InitializeUMButton(ref _up, "Up");
            }
            return _up;
        }
		set{
			_up = value;
		}
    }
    [SerializeField]
    private GameObject _up;

    public AudioClip SoundSelected;
	
	private Vector3 originalScale;

    void Start()
    {
        Up.SetActiveRecursively(true);
		Down.SetActiveRecursively(false);
		originalScale = gameObject.transform.localScale;
    }

    void Update()
    {
        

    }

    
    void OnMouseEnter()
    {
		if (FeedbackEnabled)
		{
			if (UseLegacyTransformValue)
				gameObject.transform.localScale = new Vector3(.6f, .6f, 1f);
        	else
				gameObject.transform.localScale = originalScale * 0.2f;
		}
    }

    void OnMouseExit()
	{
		if (UseLegacyTransformValue)
			gameObject.transform.localScale = new Vector3(.5f, .5f, 1f);
    	else
			gameObject.transform.localScale = originalScale;
    }

    void OnMouseDown()
    {
        Up.SetActiveRecursively(false);
		Down.SetActiveRecursively(true);

        if (IsActive)
        {
			if (GetComponent<AudioSource>().clip != null)
			{
				if (!GetComponent<AudioSource>().isPlaying)
	            {
	                GetComponent<AudioSource>().clip = SoundSelected;
	
	                GetComponent<AudioSource>().time = 0;
	                GetComponent<AudioSource>().pitch = 1;
	
	                GetComponent<AudioSource>().Play();
	            }
			}
        }
    }
	
	void OnMouseUp(){
		Down.SetActiveRecursively(false);
        Up.SetActiveRecursively(true);
	}

    void InitializeUMButton(ref GameObject go, string name)
    {
        go = gameObject.GetChildByName(name);  
    }
    
}
