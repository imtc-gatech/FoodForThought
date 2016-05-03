using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour {

    private GameObject owner;
    public Stage ingredientStage;
    public Vector3 worldPosition;
    public bool occupied;
    public Gauge actionGauge;

    void Awake()
    {
        actionGauge = transform.FindChild("CookingGauge").GetComponent<Gauge>();
    }

	// Use this for initialization
	void Start () {
        occupied = false;
        worldPosition = gameObject.transform.position;
        owner = gameObject.transform.parent.gameObject;
        actionGauge.Visibility = false;
        SetBackGroundColor();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 Occupy(Stage stage)
    {
        if (!occupied)
        {
            actionGauge.Visibility = true;
            occupied = true;
            ingredientStage = stage;
            actionGauge.SetGaugeParameters(stage.RecipeStep.cookingParameters);
            actionGauge.activated = true;
            SetBackGroundColor();
            return worldPosition;
        }
        else
        {
            Debug.Log("Slot already occupied.");
            return new Vector3();
        }
    }

    public void Vacate()
    {
        if (occupied)
        {
            actionGauge.Visibility = false;
            occupied = false;
            ingredientStage = null;
            SetBackGroundColor();
        }
        else
        {
            Debug.Log("Slot already vacated.");
        }
    }

    private void SetBackGroundColor()
    {
        RageSpline slotSpline = transform.FindChild("SlotSpline").GetComponent<RageSpline>();
        //RageSpline slotSpline = gameObject.GetComponentInChildren<RageSpline>();
        if (occupied)
        {
            slotSpline.SetFillColor1(Color.white);
        }
        else
        {
            slotSpline.SetFillColor1(Color.black);
        }

        slotSpline.RefreshMesh(true, false, false);

    }


}
