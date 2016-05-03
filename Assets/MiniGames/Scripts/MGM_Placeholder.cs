using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MGM_Placeholder : MG_Minigame {

    public float DelayToWinInSeconds = 5.0f;
    public bool UseDelay = true;

	// Use this for initialization
	void Start () {
        SetupMinigame();
	}
	
	// Update is called once per frame
	void Update () {
        switch (CurrentState)
        {
            case State.Dormant:
                break;
            case State.Active:
                if (UseDelay)
                {
                    DelayToWinInSeconds -= Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
                    if (DelayToWinInSeconds < 0)
                    {
                        CanBeScored = true;
                    }
                }
                else
                {
                    CanBeScored = true;
                }
                break;
            case State.Done:
                break;
        }
	}

    public override void SetupMinigame()
    {
        base.SetupMinigame();
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
    }

    public override void SetGameVariables(Dictionary<string, string> variableDict)
    {
        foreach (KeyValuePair<string, string> vPair in variableDict)
        {
            switch (vPair.Key)
            {
                case "DelayToWinInSeconds":
                    DelayToWinInSeconds = float.Parse(vPair.Value);
                    break;
                case "UseDelay":
                    UseDelay = bool.Parse(vPair.Value);
                    break;
            }
        }
    }

    public override Dictionary<string, string> GetGameVariables()
    {
        var variableDict = new Dictionary<string, string>();

        variableDict.Add("DelayToWinInSeconds", DelayToWinInSeconds.ToString());
        variableDict.Add("UseDelay", DelayToWinInSeconds.ToString());

        return variableDict;
    }

    public override void ScoreMinigame()
    {
        StarResult = 5f;
    }
}
