using UnityEngine;
using System.Collections;

[System.Serializable]
public class FFTFreshnessMeterParameters {

    public bool UseFreshness = false;

    public float DelayInSeconds
    {
        get { return _delayInSeconds; }
        set { _delayInSeconds = Mathf.Clamp(value, 0, _decayTimeInSeconds); }
    }
    public float DecayTimeInSeconds
    {
        get { return _decayTimeInSeconds; }
        set { _decayTimeInSeconds = Mathf.Clamp(value, 0, 99999); }
    }
    public float StarPenaltyMaximum
    {
        get { return _starPenaltyMaximum; }
        set { _starPenaltyMaximum = Mathf.Clamp(value, 0, 5); }
    }

    [SerializePrivateVariables]
    private float _delayInSeconds = 5.0f;
    private float _decayTimeInSeconds = 60f;
    private float _starPenaltyMaximum = 2.0f;

}
