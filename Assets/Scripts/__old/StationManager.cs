using UnityEngine;
using System.Collections;

public class StationManager : MonoBehaviour {

    public GameObject StationChop;
    public GameObject StationCook;
    public GameObject StationPrep;
    public GameObject StationSpice;

    private static StationManager instance = null;
    public static StationManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool SlotAvailable(Stage id)
    {
        switch (id.Destination)
        {
            case Station.Type.Chop:
                return CheckSlotsForEmptyState(StationChop);
            case Station.Type.Cook:
                return CheckSlotsForEmptyState(StationCook);
            case Station.Type.Prep:
                return CheckSlotsForEmptyState(StationPrep);
            case Station.Type.Spice:
                return CheckSlotsForEmptyState(StationSpice);
            case Station.Type.Finish:
                return false;
            default:
                Debug.Log("Station.Type " + id.Destination.ToString() + " not valid.");
                return false;

        }
    }

    bool CheckSlotsForEmptyState(GameObject stationObject)
    {
        Slot[] slots = stationObject.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            if (!slot.occupied)
            {
                return true; //slot not occupied, return available
            }
        }

        return false; // else, no slots available
    }

    public Vector3 OccupySlot(Stage id)
    {
        switch (id.Destination)
        {
            case Station.Type.Chop:
                return OccupySlotWithObject(StationChop, id);
            case Station.Type.Cook:
                return OccupySlotWithObject(StationCook, id);
            case Station.Type.Prep:
                return OccupySlotWithObject(StationPrep, id);
            case Station.Type.Spice:
                return OccupySlotWithObject(StationSpice, id);
            default:
                Debug.Log("Station.Type " + id.ToString() + " not valid.");
                return new Vector3();

        }
    }


    Vector3 OccupySlotWithObject(GameObject stationObject, Stage stage)
    {
        Slot[] slots = stationObject.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            if (!slot.occupied)
            {
                return slot.Occupy(stage);
            }
        }

        Debug.Log("No slots available. Did you check for available slots before calling OccupySlot?");
        return new Vector3();


    }

    public void VacateSlot(Stage id)
    {
        switch (id.Destination)
        {
            case Station.Type.Chop:
                VacateSlotWithObject(StationChop, id);
                break;
            case Station.Type.Cook:
                VacateSlotWithObject(StationCook, id);
                break;
            case Station.Type.Prep:
                VacateSlotWithObject(StationPrep, id);
                break;
            case Station.Type.Spice:
                VacateSlotWithObject(StationSpice, id);
                break;
            default:
                Debug.Log("Station.Type " + id.ToString() + " not valid.");
                break;

        }
    }

    void VacateSlotWithObject(GameObject stationObject, Stage stage)
    {
        Slot[] slots = stationObject.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            if (slot.ingredientStage == stage)
            {
                slot.Vacate();
            }
        }

        Debug.Log("No slots available. Did you check for available slots before calling OccupySlot?");

    }

    
}
