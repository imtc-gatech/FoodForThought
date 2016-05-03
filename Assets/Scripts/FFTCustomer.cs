using UnityEngine;
using System.Collections;

public class FFTCustomer : MonoBehaviour {

    public enum Name
    {
        Alligator,
		Bee,
        Bear,
		Bulldog,
		Chloe,
        Fox,
        Goat,
        Monkey,
        Owl,
        Peacock,
        Vulture,
        Walrus,
		Wolf
}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static string FeedbackText(Name customer, float starCount)
	{
		//Customer Name currently unused... could be used to vary their response based on starCount
		
		if (starCount <= 2)
        {
            return "Yuck!";
        }
        else if (starCount < 4)
        {
            return "Not bad.";
        }
        else // >= 4
        {
            return "Awesome!";
        }
		
	}
	
	public static FFTCustomerView.VisualState VisualState(Name customer, float starCount)
	{
		//Customer Name currently unused... could be used to vary their response based on starCount
		
		if (starCount <= 2)
        {
            return FFTCustomerView.VisualState.Bad;
        }
        else if (starCount < 4)
        {
            return FFTCustomerView.VisualState.Average;
        }
        else
        {
            return FFTCustomerView.VisualState.Good;
        }
	}
}
