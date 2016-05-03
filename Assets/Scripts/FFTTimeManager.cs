using UnityEngine;
using System.Collections;

public class FFTTimeManager : MonoBehaviour {
	
	private static FFTTimeManager _instance = null;
    public static FFTTimeManager Instance
    {
        get
        {
            return _instance;
        }
    }
	
	public bool GameplayPaused = false;
	
	public float AnimationTimeScale = 1.0f;
	public float GameplayTimeScale = 1.0f;
	public float ElapsedTimeScale = 1.0f;
	
	public float StationChopScale = 1.0f;
	public float StationCookScale = 1.0f;
	public float StationPrepScale = 1.0f;
	public float StationSpiceScale = 1.0f;
	
	void Awake() {
		AssignSingletonInstance();
	
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (GameplayPaused)
			GameplayTimeScale = 0;
		else
			GameplayTimeScale = 1;
	
	}
	
	public float StationGameplayTimeScale(FFTStation.Type station)
	{
		switch (station)
		{
			case FFTStation.Type.Chop:
				return StationChopScale * GameplayTimeScale;
			case FFTStation.Type.Cook:
				return StationCookScale * GameplayTimeScale;
			case FFTStation.Type.Spice:
				return StationSpiceScale * GameplayTimeScale;
			case FFTStation.Type.Prep:
				return StationPrepScale * GameplayTimeScale;
			default:
				return GameplayTimeScale;
		}
	}
	

    public void AssignSingletonInstance()
    {
        if (_instance == null)
        {
            _instance = this;
		}
	}
}
