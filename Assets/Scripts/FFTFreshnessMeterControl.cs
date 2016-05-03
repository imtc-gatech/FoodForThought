using UnityEngine;
using System.Collections;

public class FFTFreshnessMeterControl : MonoBehaviour {
	
	public enum State
	{
		Fresh=0,
		Decay=1,
		Rotten=2
	}
	
	public State CurrentState = State.Fresh;
	
	public float Freshness = 1.0f;

	private float DelayInSeconds = 5.0f;
	private float DecayTimeInSeconds = 60f;
	private float StarPenaltyMaximum = 2.0f;

    public FFTFreshnessMeterParameters Parameters;

	public float CurrentStarPenalty
	{
		get { return StarPenaltyMaximum - (StarPenaltyMaximum * Freshness); }
	}
	
	public float ElapsedTime = 0.0f;
	
	public bool IsRunning = false;
	
	public FFTFreshnessMeterView View;
	
	// Use this for initialization
	void Start () {
        if (Parameters == null)
            Parameters = new FFTFreshnessMeterParameters();
        SetParameters(Parameters);
		View.Value = Freshness;
	}
	
	// Update is called once per frame
	void Update () {
		if (IsRunning)
		{
			ElapsedTime += Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
			switch (CurrentState)
			{	
			case State.Fresh:
				if (ElapsedTime > DelayInSeconds)
					CurrentState = State.Decay;
				break;
			case State.Decay:
				DecayFood();
				if (Freshness == 0)
					CurrentState = State.Rotten;
				break;
			case State.Rotten:
				IsRunning = false;
				break;
				
			}
		}
	}
	
	void StartDecay() {
		IsRunning = true;
	}
	
	void StartDecay(float delay, float totalDecay)
	{
		DelayInSeconds = delay;
		DecayTimeInSeconds = totalDecay;
		IsRunning = true;
	}
	
	void StopDecay() {
		IsRunning = false;
	}
	
	void DecayFood() {
		Freshness = Mathf.Clamp(1.0f - (ElapsedTime - DelayInSeconds) / DecayTimeInSeconds, 0, 1);
		View.Value = Freshness;
	}

    public void SetParameters(FFTFreshnessMeterParameters parameters)
    {
        Parameters = parameters;
        DelayInSeconds = Parameters.DelayInSeconds;
        DecayTimeInSeconds = Parameters.DecayTimeInSeconds;
        StarPenaltyMaximum = Parameters.StarPenaltyMaximum;
    }
}
