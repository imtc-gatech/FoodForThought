using UnityEngine;
using System.Collections;

public class FFTSlot : MonoBehaviour {

    public bool Selected;
	public bool Obstructed;
    public bool Occupied
    {
        get
        {
            if (Dish == null)
            {
                return false;
            }
            return true;
        }
    }
	
	/*
	bool objectIsMoving = false;
	GameObject objectToMove;
	Transform[] objectPathway;
	float objectElapsedTravel = 0f;
	float objectTotalTravel = 0f;
	 */
	
    public enum SlotType
    {
        Counter,
        Station,
        Results
    }

    public SlotType Type;

    public FFTDish Dish
    {
        get
        {
            return _dish;
        }
        set
        {
            _dish = value;

            if (Occupied)
                InitializeDishToHomeSlotPosition();
        }
    }
    [SerializeField]
    private FFTDish _dish;

    public bool Render
    {
        get
        {
            return _render;
        }
        set
        {
            _render = value;
            if (_render)
            {
                Spline.SetFill(RageSpline.Fill.Solid);
                Spline.SetOutline(RageSpline.Outline.Loop);
            }
            else
            {
                Spline.SetFill(RageSpline.Fill.None);
                Spline.SetOutline(RageSpline.Outline.None);
            }
            Spline.RefreshMesh();
        }
    }
    [SerializeField]
    private bool _render = false;

    public IRageSpline Spline
    {
        get{ return gameObject.GetComponent<RageSpline>() as IRageSpline; }
    }

    public GameObject Shadow;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (objectIsMoving)
		{
			objectElapsedTravel += Time.deltaTime;
			if (objectElapsedTravel > objectTotalTravel)
			{
				objectIsMoving = false;	
				objectElapsedTravel = objectTotalTravel;
			}
			iTween.PutOnPath(objectToMove, objectPathway, objectElapsedTravel / objectTotalTravel);
		}
		*/
	
	}

    void InitializeDishToHomeSlotPosition()
    {
        if (Dish.HomeCounterSlot == null)
        {
            Dish.HomeCounterSlot = this;
            Dish.CurrentStationSlot = this;
            AlignDishToCurrentSlotPosition();
        }
    }

    void AlignDishToCurrentSlotPosition()
    {
        Dish.gameObject.transform.position = Dish.CurrentStationSlot.gameObject.transform.position;
    }
	
	//This is where the dishes are moved from the counter to the station
    public bool MoveDishToCurrentSlotPosition(FFTDish dish, float time)
    {
        if (Type == SlotType.Station)
        {
            dish.CurrentStationSlot = this;
			/*
			objectToMove = dish.gameObject;
			objectPathway = new Transform[2] {dish.HomeCounterSlot.transform, dish.CurrentStationSlot.transform};
			objectElapsedTravel = 0f;
			objectTotalTravel = time;
			objectIsMoving = true;
			*/
			
            iTween.MoveTo(dish.gameObject, iTween.Hash("x", gameObject.transform.position.x, "y", gameObject.transform.position.y, "z", gameObject.transform.position.z - 9, "time", time)); //, "oncomplete", "AlignDishToCurrentSlotPosition"
            return true;
        }
        return false;
    }

}
