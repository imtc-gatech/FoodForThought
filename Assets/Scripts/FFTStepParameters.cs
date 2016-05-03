using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FFTStepParameters {

    private FFTStep parentStep; // used to validate values

    public static int PEAK_FLOOR = 10;
    public static int PEAK_CEILING = 90;
    public static int PEAK_STEP = 5;

    public static int DECIMAL_PLACES = 1;

    public float Uncooked
    {
        get
        {
            return _uncooked;
        }
        set
        {
            if (_uncooked != value)
            {
                if (value < 0)
                {
                    _uncooked = 0;
                }
                else
                {
                    _uncooked = value;
                    //_uncooked = Mathf.Round(value * Mathf.Pow(10, DECIMAL_PLACES)) / Mathf.Pow(10, DECIMAL_PLACES);
                }

            }
        }
    }
    public float Cooked
    {
        get
        {
            return _cooked;
        }
        set
        {
            if (_cooked != value)
            {
                if (value < 0)
                {
                    _cooked = 0;
                }
                else
                {
                    _cooked = value;
                    //_cooked = Mathf.Round(value * Mathf.Pow(10, DECIMAL_PLACES)) / Mathf.Pow(10, DECIMAL_PLACES);
                }

            }
        }
    }
    public float Burned
    {
        get
        {
            return _burned;
        }
        set
        {
            if (_burned != value)
            {
                if (value < 0)
                {
                    _burned = 0;
                }
                else
                {
                    _burned = value;
                    //_burned = Mathf.Round(value * Mathf.Pow(10, DECIMAL_PLACES)) / Mathf.Pow(10, DECIMAL_PLACES);
                }

            }
        }
    }
    public int PeakPercentage
    {
        get
        {
            return _peakValue;
        }
        set
        {
            if (value > PEAK_CEILING)
            {
                _peakValue = PEAK_CEILING;
            }
            else if (value < PEAK_FLOOR)
            {
                _peakValue = PEAK_FLOOR;
            }
            else
            {
                _peakValue = value - (value % PEAK_STEP);
            }
        }


    }

    public bool IsCookable
    {
        get
        {
            return _isCookable;
        }
        set
        {
            if (_isCookable != value && parentStep.AllowsCooking)
            {
                if (value)
                {
                    IsBurnable = _lastBurnedState;
                    Cooked = _lastCookedValue;
                }
                else
                {
                    _lastCookedValue = Cooked;
                    _lastBurnedState = IsBurnable;
                    IsBurnable = false;
                    Cooked = 0.00f;
                }

                _isCookable = value;
            }
        }
    }

    public bool IsBurnable
    {
        get
        {
            return _isBurnable;
        }
        set
        {
            if (_isBurnable != value && parentStep.AllowsBurning)
            {
                if (value)
                {
                    Burned = _lastBurnedValue;
                }
                else
                {
                    _lastBurnedValue = Burned;
                    Burned = 0.00f;
                }

                _isBurnable = value;
            }
        }
    }
    [SerializeField]
    private bool _isBurnable = true;

    public bool UsesPeakFlavor
    {
        get
        {
            if (IsCookable)
                return _peakFlavor;
            else
                return false;
        }
        set
        {
            if (_peakFlavor != value)
            {
                _peakFlavor = value;
            }
        }

    }

    public float TotalSeconds
    {
        get
        {
            return Uncooked + Cooked + Burned;
        }
        set
        {
            float totalSecondsSnapshot = Uncooked + Cooked + Burned;
            float roundedTotalSeconds = Mathf.Round(value * Mathf.Pow(10, DECIMAL_PLACES)) / Mathf.Pow(10, DECIMAL_PLACES);

            if (roundedTotalSeconds < 1)
            {
                // do nothing.
            }
            else if (roundedTotalSeconds != totalSecondsSnapshot)
            {
                if (totalSecondsSnapshot != 0)
                {
                    float percentageChange = roundedTotalSeconds / totalSecondsSnapshot;
                    Uncooked *= percentageChange;
                    _lastCookedValue *= percentageChange;
                    Cooked *= percentageChange;
                    _lastBurnedValue *= percentageChange;
                    Burned *= percentageChange;
                }
            }

        }
    }

    public float PercentageUncooked
    {
        get
        {
            if (TotalSeconds > 0)
                return Uncooked / TotalSeconds;
            else
                return 0;
        }
    }

    public float PercentageCooked
    {
        get
        {
            if (TotalSeconds > 0 && IsCookable)
                return Cooked / TotalSeconds;
            else
                return 0;
        }
    }

    public float PercentageBurned
    {
        get
        {
            if (TotalSeconds > 0 && IsBurnable)
                return Burned / TotalSeconds;
            else
                return 0;
        }
    }
    [SerializeField]
    private float _uncooked = 1;
    [SerializeField]
    private float _cooked = 1;
    [SerializeField]
    private float _burned = 1;
    
    [SerializeField]
    private bool _peakFlavor = true;
    [SerializeField]
    private int _peakValue = 50;

    [SerializeField]
    private bool _isCookable = true;
    [SerializeField]
    private float _lastCookedValue;

    
    [SerializeField]
    private bool _lastBurnedState;
    [SerializeField]
    private float _lastBurnedValue;

    public FFTStepParameters(FFTStep step)
    {
        parentStep = step;
        ResetValues();
    }

    public Vector4 ReturnVisualStateFromSecondsElapsed(float seconds)
    {
        Vector4 result = new Vector4();

        if (IsBurnable && seconds > (Uncooked + Cooked))
        {
            result.y = 1.0f - ((seconds - Uncooked - Cooked) / (TotalSeconds - Uncooked - Cooked));
            result.z = 1.0f;
        }
        else if (IsCookable && seconds > (Uncooked))
        {
            result.x = 1.0f - ((seconds - Uncooked) / (TotalSeconds - Uncooked));
            result.y = 1.0f;
            result.z = 1.0f;
        }
        else
        {
            result.x = 1.0f;
            result.y = 1.0f;
            result.z = 1.0f;
        }

        //TODO: do something about "spoiled" here

        return result;

    }

    public void ResetValues()
    {
        _isCookable = parentStep.AllowsCooking;
        _isBurnable = parentStep.AllowsBurning;
        Uncooked = 1;
        Cooked = 1;
        Burned = 1;
    }

    public Vector3 GetParametersVector3()
    {
        return new Vector3(PercentageUncooked, PercentageCooked, PercentageBurned);
    }
	
	public FFTStepParameters CopyValues(FFTStepParameters targetParams)
	{
		//TODO: Copy values from current Parameters to the passed in value
		return targetParams;
	}

}
