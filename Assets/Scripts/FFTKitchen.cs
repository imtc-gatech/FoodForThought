using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTKitchen : MonoBehaviour {

    public static Color BackgroundColor = new Color((27f / 255f), (41f / 255f), (63f / 255f));

    public static Vector3 KitchenScale = new Vector3(1, 1, 1);//new Vector3(0.8f, 0.8f, 1);

    //variables for aligning the stations correctly
    //TODO: make this a little less funky
    private float xLeft = 140;
    private float xRight = 232;
    private float yTop = -50;
    private float yBottom = -141;

    public int maxStations = 4;

    
    public Dictionary<FFTStation.Type, FFTStation> Stations
    {
        get
        {
            if (_stations == null)
            {
                _stations = new Dictionary<FFTStation.Type, FFTStation>();
                foreach (KeyValuePair<FFTStation.Type, FFTStation> pair in StationList)
                {
                    _stations.Add(pair.Key, pair.Value);
                }
                Debug.Log("USED BACKWARDS COMPATIBILITY DICTIONARY (FIND OUT WHERE)");
            }
            
            return _stations;
        }
    }
    [SerializeField]
    private Dictionary<FFTStation.Type, FFTStation> _stations;
     

    public List<KeyValuePair<FFTStation.Type, FFTStation>> StationList = new List<KeyValuePair<FFTStation.Type, FFTStation>>();

	// Use this for initialization
	void Start () {
	
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public int StationCount(FFTStation.Type type)
	{
		int result = 0;
		foreach (KeyValuePair<FFTStation.Type, FFTStation> pair in StationList)
		{
			if (pair.Key == type)
			{
				result = pair.Value.SlotList.Count;
			}
				
		}
		return result;
	}

    public bool SlotAvailable(FFTStation.Type stationType)
    {
        foreach (KeyValuePair<FFTStation.Type, FFTStation> pair in StationList)
        {
            if (pair.Key == stationType)
            {
                return pair.Value.SlotAvailable;
            }
        }
        return false;
        /*
        if (Stations.ContainsKey(stationType))
        {
            return Stations[stationType].SlotAvailable;
        }
        else
            return false;
         */
    }

    public bool SendDishToStation(FFTDish dish)
    {
        foreach (KeyValuePair<FFTStation.Type, FFTStation> pair in StationList)
        {
            if (pair.Key == dish.CurrentDestination)
            {
                if (pair.Value.TakeDish(dish))
                {
					dish.BackgroundBox.SetActiveRecursively(false);
                    dish.HomeCounterSlot.Dish = null;
                    return true;
                }
                
            }
        }
        return false;
        /*
        if (Stations[dish.CurrentDestination].TakeDish(dish))
        {
            dish.HomeCounterSlot.Dish = null;
            return true;
        }

        return false;
         */
    }

    public void InitializeStations()
    {
        //TODO: all placements are based on the scale expecting to be 1,1,1. This should be cleaned up.

        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.name = "Kitchen";
        //gameObject.transform.position = new Vector3(-115, 50, 0);

        FFTStation[] oldStations = gameObject.GetComponentsInChildren<FFTStation>();
        foreach (FFTStation station in oldStations)
        {
            FFTUtilities.DestroySafe(station.gameObject);
        }

        StationList = new List<KeyValuePair<FFTStation.Type, FFTStation>>();

        //Stations = new Dictionary<FFTStation.Type, FFTStation>();
        AddStation();
        AddStation();
        AddStation();
        AddStation();

        //now we can scale the kitchen to what we want (change the static variable to change the scaling for everything)
        gameObject.transform.localScale = KitchenScale;
    }

    public void UpdateKitchenName()
    {
        string kitchenName = "Kitchen: ";
        kitchenName += "Chop (" + StationAccess(FFTStation.Type.Chop).SlotList.Count + ") ";
        kitchenName += "Cook (" + StationAccess(FFTStation.Type.Cook).SlotList.Count + ") ";
        kitchenName += "Prep (" + StationAccess(FFTStation.Type.Prep).SlotList.Count + ") ";
        kitchenName += "Spice (" + StationAccess(FFTStation.Type.Spice).SlotList.Count + ") ";
        gameObject.name = kitchenName;
    }

    public void AddStation()
    {
        if (StationList.Count > (maxStations - 1))
        {
            return;
        }
        GameObject newStationGO = GameObject.Instantiate(Resources.Load("MainGamePrefabs/StationBase")) as GameObject;
        FFTStation newStation = newStationGO.GetComponent<FFTStation>();
        newStation.Kitchen = this;
        newStationGO.transform.parent = gameObject.transform;
        switch (StationList.Count)
        {
            case 0:
                newStationGO.transform.localPosition = new Vector3(xLeft, yTop);
                newStation.Name = FFTStation.Type.Cook;
                break;
            case 1:
                newStationGO.transform.localPosition = new Vector3(xLeft, yBottom);
                newStation.Name = FFTStation.Type.Spice;
                break;
            case 2:
                newStationGO.transform.localPosition = new Vector3(xRight, yTop);
                newStation.Name = FFTStation.Type.Chop;
                break;
            case 3:
                newStationGO.transform.localPosition = new Vector3(xRight, yBottom);
                newStation.Name = FFTStation.Type.Prep;
                break;
        }
        StationList.Add(new KeyValuePair<FFTStation.Type,FFTStation>(newStation.Name, newStation));
        //Stations.Add(newStation.Name, newStation);
    }

    FFTStation StationAccess(FFTStation.Type stationType)
    {
        foreach (KeyValuePair<FFTStation.Type, FFTStation> pair in StationList)
        {
            if (pair.Key == stationType)
            {
                return pair.Value;
            }
        }
        return null;
    }
	
	//METHODS FOR DISTRACTION ENGINE - STUB
	
	public bool AddDistractionEffect(FFTStation.Type stationType, float timeMultiplier, bool isBlocked)
	{
		//do checks here to make sure that FFTStation.Type is equal to a valid destination,
		//return false if the station is not valid.
		if(stationType == FFTStation.Type.Chop){
			Debug.Log("add effect on chopping");
			return true;
		}
		if(stationType == FFTStation.Type.Cook){
			Debug.Log("add effect on cooking");
			return true;
		}
		if(stationType == FFTStation.Type.Prep){
			Debug.Log("add effect on prep");
			return true;
		}
		if(stationType == FFTStation.Type.Spice){
			Debug.Log("add effect on spicing");
			return true;
		}
		return false;	
	}
	
	public bool AddDistractionView(FFTStation.Type stationType, string ThisIsWhereTheTypeOfDistractionGoes)
	{
		return false;	
	}
	
	public bool RemoveDistractionEffect(FFTStation.Type stationType)
	{
		//also remove the distraction view if one is present
		if(stationType == FFTStation.Type.Chop){
			Debug.Log("remove effect on chopping");
			return true;
		}
		if(stationType == FFTStation.Type.Cook){
			Debug.Log("remove effect on cooking");
			return true;
		}
		if(stationType == FFTStation.Type.Prep){
			Debug.Log("remove effect on prep");
			return true;
		}
		if(stationType == FFTStation.Type.Spice){
			Debug.Log("remove effect on spicing");
			return true;
		}
			return false;
	}
	
	public void RemoveAllDistractionEffects()
	{
		//
		
	}
	
	/// <summary>
	/// TODO Check to see if Kitchen contains any slots of the passed Station type. 
	/// </summary>
	/// <returns>
	/// <c>true</c> if Station type has active slots, otherwise, <c>false</c>.
	/// </returns>
	/// <param name='stationType'>
	/// Type of Station to check.
	/// </param>
	public bool IsStationActive(FFTStation.Type stationType)
	{
		return false; //return ( [stationType] .SlotList.Count > 0);
	}
	
	public FFTStation.Type CurrentStationTypeFromCoordinates(Vector2 coordinates)
	{
		float centerX = -65.75f;
		float centerY = 10f;
		float edgeX = 46.42f;
		float edgeY = -81.5f;
		
		//checks if it is within each station.  if so, it returns that station.  If not, it returns 0.
		if(coordinates.x < centerX && coordinates.y > centerY){
			return FFTStation.Type.Cook;
		}
		if(coordinates.x > centerX && coordinates.y > centerY && coordinates.x < edgeX){
			return FFTStation.Type.Chop;
		}
		if(coordinates.x < centerX && coordinates.y < centerY && coordinates.y > edgeY){
			return FFTStation.Type.Spice;
		}
		if(coordinates.x > centerX && coordinates.y < centerY && coordinates.x < edgeX && coordinates.y > edgeY){
			return FFTStation.Type.Prep;
		}
		return 0;	
	}
	
}
