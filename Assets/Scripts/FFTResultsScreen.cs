using UnityEngine;
using System.Collections;

public class FFTResultsScreen : MonoBehaviour {

    public static Vector3 SimpleCharacterPos = new Vector3(90, -25, 0);

    public static Vector3 VerboseScaleCharacter = new Vector3(0.7f, 0.7f, 1);
    public static Vector3 VerboseOffsetCharacter = new Vector3(-22, -28, 0);

    public static Vector3 VerboseScaleStars = new Vector3(1.5f, 1.5f, 1);
    public static Vector3 VerboseOffsetStars = new Vector3(-16, 11, 0);

    bool populated = false;

    FFTCustomer.Name customerName = FFTCustomer.Name.Monkey;

    public enum InfoDensity
    {
        Simple,
        Verbose
    }

    FFTStarDisplay MainStarDisplay;
    FFTCustomerView CustomerView;
    FFTSpeechBubble SpeechBubble;
	FFTResultsScreenDismissButton DismissButton;
	TextMesh TimingFeedbackText;

    GameObject background;
    GameObject character;
    GameObject speechBubble;
    GameObject starRow;
    GameObject characterContainer;
	GameObject timingText;
	GameObject dismissButton;

    public FFTResultsScreen.InfoDensity InformationDensity = InfoDensity.Simple;

    public float overallStarCount = 3;
    public float possibleStarCount = 5;

    public string feedbackText = "";

	// Use this for initialization
	void Start () {
        //Populate();
        
	}
	
	// Update is called once per frame
	void Update () {
        
	
	}
	
	public void SetupDisplay(FFTScore CurrentScore, FFTLevel CurrentLevel, FFTCounter Counter)
	{
		possibleStarCount = CurrentScore.RecipeTotalStarRating();
        overallStarCount = CurrentScore.RecipeTotalStarRatingWithPenalty();
        InformationDensity = CurrentLevel.Complexity;
        UpdateDisplay(Counter.RecipeCard.Customer);
        Vector3 resultsPosition = FFTCameraManager.PositionFromGrid(0, 1);
        resultsPosition.z += 200;
        gameObject.transform.position = resultsPosition;
		
		//POPOP
		if (CurrentScore.ReceivedTimingPenalty)
		{
			UpdateTimingFeedbackText(CurrentScore.TimingFeedback);
		}
	}

    public void UpdateDisplay()
    {
        UpdateDisplay(overallStarCount, possibleStarCount);
    }

    public void UpdateDisplay(float starCount, float outlineStarCount)
    {
        if (!populated)
            Populate();
        MainStarDisplay.StarCount = starCount;
        MainStarDisplay.StarOutlineCount = outlineStarCount;
        if (outlineStarCount > starCount)
        {
            MainStarDisplay.UseOutlineCount = true;
            MainStarDisplay.Reset();
        }
		
		CustomerView.State = FFTCustomer.VisualState(customerName, starCount);
		SpeechBubble.DisplayText = FFTCustomer.FeedbackText(customerName, starCount);
		SpeechBubble.UpdateText();
		
		/*
        if (starCount <= 2)
        {
            CustomerView.State = FFTCustomerView.VisualState.Bad;
            SpeechBubble.DisplayText = "Yuck!";
            SpeechBubble.UpdateText();
        }
        else if (starCount < 4)
        {
            CustomerView.State = FFTCustomerView.VisualState.Average;
            SpeechBubble.DisplayText = "Not bad.";
            SpeechBubble.UpdateText();
        }
        else
        {
            CustomerView.State = FFTCustomerView.VisualState.Good;
            SpeechBubble.DisplayText = "Awesome!";
            SpeechBubble.UpdateText();
            
        }
        */

    }

    public void UpdateDisplay(FFTCustomer.Name customer)
    {
        customerName = customer;
        UpdateDisplay();
    }

    public void UpdateDisplay(FFTCustomer.Name customer, float starCount)
    {
        customerName = customer;
        UpdateDisplay(starCount, starCount);
    }

    public void UpdateDisplay(FFTCustomer.Name customer, float starCount, float outlineStarCount)
    {
        customerName = customer;
        UpdateDisplay(starCount, outlineStarCount);
    }
	
    public void OnDestroy()
    {
        FFTUtilities.DestroySafe(MainStarDisplay);
        FFTUtilities.DestroySafe(CustomerView);
        FFTUtilities.DestroySafe(SpeechBubble);
		if (FFTGameManager.Instance != null)
		{
			FFTResultsDetailDisplayView[] views = FFTGameManager.Instance.GetComponentsInChildren<FFTResultsDetailDisplayView>();
			if (views != null)
			{
				foreach (FFTResultsDetailDisplayView view in views)
				{
					FFTUtilities.DestroySafe(view.gameObject);		
				}
			}
		}
    }

    void Populate()
    {
        gameObject.name = "Results Screen";
        background = GameObject.Instantiate(Resources.Load("ResultsScreen/ScreenBackground", typeof(GameObject)) as GameObject) as GameObject;
        background.transform.parent = transform;
        characterContainer = new GameObject("Character Feedback");
        characterContainer.transform.parent = transform;
        character = GameObject.Instantiate(Resources.Load("ResultsScreen/Characters/Character_" + customerName.ToString(), typeof(GameObject)) as GameObject) as GameObject;
        character.transform.position = SimpleCharacterPos;
        character.transform.parent = characterContainer.transform;
        speechBubble = GameObject.Instantiate(Resources.Load("ResultsScreen/SpeechBubbleFeedbackSimple", typeof(GameObject)) as GameObject) as GameObject;
        speechBubble.transform.parent = characterContainer.transform;
        starRow = GameObject.Instantiate(Resources.Load("ResultsScreen/starRowSimpleFeedbackLarge", typeof(GameObject)) as GameObject) as GameObject;
        starRow.transform.parent = transform;
		timingText = GameObject.Instantiate(Resources.Load("ResultsScreen/TimingPenaltyText", typeof(GameObject)) as GameObject) as GameObject;
        TimingFeedbackText = timingText.GetComponent<TextMesh>();
		TimingFeedbackText.text = "";
		timingText.transform.parent = transform;
		
		//Repeat / Next Level Buttons
		
		GameObject repeatButton = GameObject.Instantiate (Resources.Load ("UIPrefabs/BackButton", typeof(GameObject)) as GameObject) as GameObject;
		repeatButton.transform.parent = transform;
		repeatButton.transform.localScale = new Vector3(0.5f, 0.5f, 1);
		repeatButton.transform.position = new Vector3(108, 65, -1);
		FFTResultsScreenDismissButton repeatButtonScript = repeatButton.AddComponent<FFTResultsScreenDismissButton>();
		repeatButtonScript.NextLevelBehaviour = FFTLevel.LoadType.Repeat;
		
		dismissButton = GameObject.Instantiate (Resources.Load ("UIPrefabs/CheckButton", typeof(GameObject)) as GameObject) as GameObject;
		dismissButton.transform.parent = transform;
		dismissButton.transform.position = new Vector3(108, -65, -1);
		
        MainStarDisplay = starRow.AddComponent<FFTStarDisplay>();
        CustomerView = character.AddComponent<FFTCustomerView>();
        SpeechBubble = speechBubble.AddComponent<FFTSpeechBubble>();
		DismissButton = dismissButton.AddComponent<FFTResultsScreenDismissButton>();
        //SpeechBubble.DisplayText = feedbackText;
        if (InformationDensity == InfoDensity.Verbose)
        {
            characterContainer.transform.localScale = VerboseScaleCharacter;
            characterContainer.transform.localPosition += VerboseOffsetCharacter;

            starRow.transform.localScale = VerboseScaleStars;
            starRow.transform.localPosition += VerboseOffsetStars;
        }

        populated = true;

        //UpdateDisplay(overallStarCount);


    }
	
	public void UpdateTimingFeedbackText(string text)
	{
		TimingFeedbackText.text = text;
	}
}
