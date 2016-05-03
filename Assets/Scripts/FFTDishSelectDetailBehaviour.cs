using UnityEngine;
using System.Collections;

public class FFTDishSelectDetailBehaviour : MonoBehaviour {

    public static float scaleTime = 0.05f;    

    public bool entered;

    public FFTDish Dish;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () { 

	}

    void OnMouseEnter()
    {
        entered = true;
        if (Dish != null)
            Dish.ScaleUp();
    }

    void OnMouseExit()
    {
        entered = false;
        if (Dish != null)
            Dish.ScaleDown();
    }

    void OnMouseDown()
    {
		//log that detail was opened:
		if (FFTGameManager.Instance.LogActions)
		{
			FFTStepAction currentAction = new FFTStepAction();
			currentAction.Reset();
			currentAction.AssignButtonAction(FFTStepAction.InteractionType.Selected, FFTStepAction.SlotActionType.Button_Results_OpenDetail);
			FFTGameManager.Instance.DataManager.AddAction(currentAction);
		}
		
        Dish.ScaleDown();
        GameObject detailScreen = GameObject.Instantiate(Resources.Load("ResultsScreen/DetailScreen", typeof(GameObject)) as GameObject) as GameObject;
        FFTResultsDetailDisplayView displayView = detailScreen.GetComponent<FFTResultsDetailDisplayView>();
        detailScreen.transform.parent = FFTGameManager.Instance.transform; //gameObject.transform
        //detailScreen.transform.localPosition = new Vector3(90, -30, -30);
        detailScreen.transform.position = new Vector3(0, -197, -229);
        displayView.Populate(Dish, FFTGameManager.Instance.CurrentScore, GetFreshnessStarDifference());
        displayView.AssignDishOrigin(transform);

        //TODO: pop up the detail view screen here
        
    }

    float GetFreshnessStarDifference() //this method should go elsewhere.
    {
        return Dish.gameObject.GetComponentInChildren<FFTStarDisplay>().VisibleStarDifference;
    }
}

