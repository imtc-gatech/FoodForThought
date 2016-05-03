using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    /*
      RGB Values for Stations:
      Pink = (252,240,245)    [Spice]
      Green = (185,219,167)   [Prep]
      Red = (224,90,89)       [Cook]
      Blue = (180,226,241)    [Chop]
    */

    public readonly Color ColorChop = new Color(180 / 255f, 226 / 255f, 241 / 255f); //Blue;
    public readonly Color ColorCook = new Color(224 / 255f, 90 / 255f, 89 / 255f); //Red
    public readonly Color ColorPrep = new Color(185 / 255f, 219 / 255f, 167 / 255f); //Green
    public readonly Color ColorSpice = new Color(252 / 255f, 240 / 255f, 245 / 255f); //Pink
	
	private static GameManager instance = null;
    public static GameManager Instance {
        get
        {
            return instance; 
        } 
    }

	
	//private GameObject dishes;
    private GameObject stationRoot;
    private Station[] stations;

    private bool splash = true;
    private bool dishSelected = false;

    public Texture2D splashScreen;

    public Transform emptySlotPrefab;

    public int CameraPosition;

    private int canvasOffset = -400;

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {

        stationRoot = GameObject.Find("Kitchen");
        if (stationRoot != null)
        {
            //Debug.Log("Stations GameObject found.");
            InitializeStations();
        }
        else
        {
            Debug.Log("ERROR: Make sure that all stations are Children of a GameObject called 'Kitchen'");

        }
		
	}
	
	// Update is called once per frame
	void Update () {
        MoveCamera();

	}

    void MoveCamera()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CameraPosition = 0; //mainGame
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            CameraPosition = 1; //minigame #1
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            CameraPosition = 2; //minigame #2
        }

        switch (CameraPosition)
        {
            case 0:
                Camera.main.transform.position = new Vector3(0, 0, -10);
                break;
            default:
                Camera.main.transform.position = new Vector3(canvasOffset, canvasOffset * (CameraPosition - 1), -10);
                break;
        }
    }
	
	public void DebugTest() {
		Debug.Log("Hello, it's me, singleton.");	
	}

    void InitializeStations()
    {
        GameObject[] stationGroup = GameObject.FindGameObjectsWithTag("Station");
        stations = new Station[stationGroup.Length];
        Debug.Log(stations.Length);
        int stationCount = 0;
        foreach (GameObject go in stationGroup)
        {
            //Debug.Log("Found station " + go.name);
            switch (go.name)
            {
                case "Chop":
                    Debug.Log("Chop");
                    stations[stationCount] = new Station(Station.Type.Chop);
                    break;
                case "Cook":
                    Debug.Log("Cook");
                    stations[stationCount] = new Station(Station.Type.Cook);
                    break;
                case "Prep":
                    Debug.Log("Prep");
                    stations[stationCount] = new Station(Station.Type.Prep);
                    break;
                case "Spice":
                    Debug.Log("Spice");
                    stations[stationCount] = new Station(Station.Type.Spice);
                    break;
                default:
                    Debug.Log("Not Found!");
                    break;
            }

            // check each station for attached slots, assign a count
            int goSlotCount = go.GetComponentsInChildren(typeof(Slot)).Length;

            stations[stationCount].SetSlotCount(goSlotCount);
            //Debug.Log("Number of slots: " + goSlotCount);

            // add a reference to the gameObject within the station
            stations[stationCount].SetGameObject(go);

            stationCount++;

        }

        //CreateEmptySlotSplines();


    }

	/*
    void CreateEmptySlotSplines()
        // NOT WORKING (YET)
        //solve why we cannot add prefabs of the slot splines to the slot at runtime (is this even necessary?)
    {
        if (emptySlotPrefab != null)
        {
            Component[] slots = GetComponentsInChildren(typeof(Slot));
            foreach (Component slot in slots)
            {
                
                GameObject go = Instantiate(emptySlotPrefab) as GameObject;
                go.transform.position = new Vector3(0, 0);
                go.name = "SlotDisplay";
                go.transform.parent = slot.gameObject.transform;
                //GameObject goSlot = slot.gameObject;
                //go.transform.parent = goSlot.transform;
                

                //slot.gameObject.AddComponent("RageSpline");

            }
        }

    }
    */

    void InstantiateDishStation()
    {
        GameObject goDish = new GameObject();
        goDish.name = "Dishes EX";
        goDish.transform.parent = this.transform; //Instance.transform;
        //dishes = GameObject.Find("Dishes");
    }

    bool SlotAvailable(Station.Type type)
    {
        foreach (Station station in stations)
        {
            if (station.GetStationType() == type)
            {
                if (station.SlotAvailable())
                {
                    station.TakeSlot();
                    return true;
                }

            }
        }

        return false;

    }

    public bool DishSelected()
    {
        return dishSelected;
    }

    public void SetDishSelect(bool selection)
    {
        dishSelected = selection;
    }

}