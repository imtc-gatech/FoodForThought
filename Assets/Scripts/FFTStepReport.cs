using System.Collections;
using System.Collections.Generic;

public class FFTStepReport : System.Object {

    public FFTStep Data;

    public string Feedback;
    public float StarRating
    {
        get
        {
            return _starRating;
        }
        set
        {
            _starRating = value;
        }
    }

    public float StarRatingRounded
    {
        get
        {
            int wholeStar = (int)_starRating;
            if ((_starRating - wholeStar) < 0.25)
            {
                return (float)wholeStar;
            }
            else if ((_starRating - wholeStar) > 0.75)
            {
                return (float)wholeStar + 1;
            }
            else
            {
                return (float)wholeStar + .5f;
            }
        }
        set
        {
            _starRating = value;
        }
    }

    private float _starRating;
        

    public string Log
    {
        get
        {
            return "Game End: " + Feedback + " Stars:" + StarRating.ToString();
        }
    }


    public FFTStepReport()
    {
        Feedback = "";
        StarRating = 0;
    }

}
