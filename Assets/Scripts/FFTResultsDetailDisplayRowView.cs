using UnityEngine;
using System.Collections;

public class FFTResultsDetailDisplayRowView : MonoBehaviour {

    public float StarCount = 3f;
    public int stepNumber; // start with 1
    
    public TextMesh Feedback;
    public Transform Mat;
    public TextMesh Number;
    
    public FFTStarDisplay StarDisplay;
    public GameObject StarDisplayObject;
    public static Vector3 stepScale = new Vector3(0.67f, 0.67f, 1);
    public static Vector3 firstStepPosition = new Vector3(-79f, 19, 0);
    public static float rowYspacing = 60f;
    public static float colXspacing = 56f;
    //take firstStepPosition, add the above to create the 3 wide 2 high grid 
    //ex 123
    //   456
    public static int colCells = 3;
    public static int rowCells = 2;

    public GameObject StepDisplay;
	
	public FFTStationIconSimple DestinationIcon;

    public FFTMinigameIcon MinigameIcon;
    
	// Use this for initialization
	void Start () {
        SetupStarDisplay();
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnDestroy()
    {
        Destroy(StepDisplay);
        Destroy(gameObject);
    }

    public void Reposition()
    {
        transform.localScale = stepScale;
        transform.localPosition = firstStepPosition;
        int row;
        int col;
        if (stepNumber <= colCells)
            col = stepNumber;
        else
            col = stepNumber - colCells;
        if (stepNumber > colCells)
            row = 2;
        else
            row = 1;
        
        transform.localPosition += new Vector3((col - 1) * colXspacing, (row - 1) * -rowYspacing, 0);

        Number.text = stepNumber + ".";
        gameObject.name = "StepResult-" + stepNumber;

        SetupStarDisplay();
        StarDisplay.StarCount = StarCount;
        
    }

    void SetupStarDisplay()
    {
        if (StarDisplay == null)
            StarDisplay = StarDisplayObject.AddComponent<FFTStarDisplay>();
        //StarDisplay.GrabStarObjects();
    }
}
