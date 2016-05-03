using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DishGO : MonoBehaviour {

    public KeyCode key;

    public List<Station.Type> steps;

    private IEnumerable<Stage> stagesEnumerable;
    private Stage[] stages;

    private int stepPosition = 0;
	
	private Alpha a;
	
	private float scaleMax = 1.2f;
	private bool entered = false;
	private bool selected = false;
    //private bool inProcess = false;

    private bool finished = false;

    private GameObject foodSplineObject;

    private Vector3 homePosition;

    private Vector3 displayOffset = new Vector3(5, 4, 0);

    //public FreshnessMeter freshnessMeter;
	
	//private DishManager dishManager;

    //private BGSwitch backgroundSwitch;

    //private GameObject[] stages;

    void Awake()
    {
        GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");
        foreach (GameObject foodObject in foodObjects)
        {
            if (foodObject.transform.parent.gameObject == this.gameObject)
            {
                foodSplineObject = gameObject;
                break;
            }
        }

        homePosition = foodSplineObject.transform.position;

        //freshnessMeter = GetComponentInChildren<FreshnessMeter>();

    }
	
	// Use this for initialization
	void Start () {
        //dishManager = transform.parent.GetComponent(typeof(DishManager)) as DishManager;

        //add Alpha manipulation script
        gameObject.AddComponent<Alpha>();
		a = GetComponent(typeof (Alpha)) as Alpha;
        //add Background manipulation script
        gameObject.AddComponent<BGSwitch>();
        //backgroundSwitch = (BGSwitch)GetComponent(typeof(BGSwitch)) as BGSwitch;

        stagesEnumerable = from stage in gameObject.GetComponentsInChildren<Stage>()
                           orderby stage.StepNumber
                           select stage;

        stages = stagesEnumerable.ToArray();

        if (stages.Length > 0)
        {
            List<Station.Type> stageSteps = new List<Station.Type>();

            foreach (Stage stage in stages)
            {
                stageSteps.Add(stage.Destination);
                stage.gameObject.SetActiveRecursively(false);
            }

            steps = stageSteps;

        }

        stages[0].gameObject.SetActiveRecursively(true);
 
        SetBackGroundColor();
		
		//GameManager.Instance
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(key))
        {
            Selected();
        }

	
	}
	
	void OnMouseEnter() {
        Entered();
	}
	
	void OnMouseDown() {
        Selected();
	}
	
	void OnMouseExit()	{
        Exited();
	}

    void Entered()
    {
        //a.alpha = 0.5f;
        //Debug.Log("entered");
        entered = true;
        //transform.localScale = new Vector3(scaleMax, scaleMax, 1.0f);

    }

    void Exited()
    {
        //a.alpha = 1.0f;
        entered = false;
        //transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

    }

    void Selected()
    {
        if (finished)
        {
            return;
        }

        GameManager.Instance.SetDishSelect(!GameManager.Instance.DishSelected());
        selected = !selected;
        if (selected)
        {
            if (StationManager.Instance.SlotAvailable(stages[stepPosition]))
            {
                a.alpha = 0.5f;
                Vector3 targetSlotPosition = StationManager.Instance.OccupySlot(stages[stepPosition]) + displayOffset;
                iTweenUtilities.MoveBy(foodSplineObject, targetSlotPosition, 1.0f);
            }
            else
            {
                selected = !selected;
            }
        }
        else
        {
            a.alpha = 1.0f;
            StationManager.Instance.VacateSlot(stages[stepPosition]);
            stages[stepPosition].gameObject.SetActiveRecursively(false);
            stepPosition++;
            if (stepPosition == stages.Length)
            {
                finished = true;
            }
            else
            {
                stages[stepPosition].gameObject.SetActiveRecursively(true);

            }
            iTweenUtilities.MoveBy(foodSplineObject, homePosition, 1.0f);
            //foodSplineObject.transform.position = homePosition;
            SetBackGroundColor();
        }

    }

    private void SetBackGroundColor()
    {
        RageSpline dishSpline = gameObject.GetComponent<RageSpline>();

        switch (steps[stepPosition])
        {
            case Station.Type.Chop:
                dishSpline.SetFillColor1(GameManager.Instance.ColorChop);
                break;
            case Station.Type.Cook:
                dishSpline.SetFillColor1(GameManager.Instance.ColorCook);
                break;
            case Station.Type.Prep:
                dishSpline.SetFillColor1(GameManager.Instance.ColorPrep);
                break;
            case Station.Type.Spice:
                dishSpline.SetFillColor1(GameManager.Instance.ColorSpice);
                break;
            case Station.Type.Finish:
                dishSpline.SetFillColor1(Color.cyan);
                break;
            default:
                Debug.Log("Not Found!");
                break;
        }

        /*
        if (steps.Capacity > stepPosition)
        {
            
            
        }
        else
        {
            dishSpline.SetFillColor1(Color.black);
        }
         */

        dishSpline.RefreshMesh(true, false, false);

    }
	
}
