using UnityEngine;
using System.Collections;

public class FFTStation : FFTContainer {

    public static float slotTravelTimeClose = 0.45f;
    public static float slotTravelTimeFar = 0.75f;

    public static float slotSpacing = 40.0f;
    public static float slotRowSpacing = 55.0f;

    public static Vector3 slotHostAlignmentOneRow = new Vector3(-136, 55, -3);
    public static Vector3 slotHostAlignmentTwoRows = new Vector3(-132, 77, -3);

    public static Vector3 slotHostScaleOneRow = new Vector3(1, 1, 1);
    public static Vector3 slotHostScaleTwoRows = new Vector3(0.75f, 0.75f, 1);

    public enum Type
    {
		//None = 0
        Chop = 1,
        Cook = 2,
        Prep = 3,
        Spice = 4 
    };

    public enum GameplayType
    {
        Empty,
        ElapsedTime,
        MiniGame
    };

    public Type Name
    {
        get { return _name; }
        set
        {
            _name = value;
            gameObject.name = "Station: " + Name;
            UpdateBackground();
        }
    }
    [SerializeField]
    private Type _name;

    public FFTKitchen Kitchen;

    public bool SlotAvailable
    {
        get
        {
            if (SlotList.Count > 0)
            {
                foreach (FFTSlot slot in SlotList)
                {
                    if (slot.Occupied == false)
                        return true;
                }
            }

            return false;
        }
    }

    public Vector3 CurrentDishSlotScale
    {
        get
        {
            if (UseSmallSlotScale)
                return slotHostScaleTwoRows;
            else
                return slotHostScaleOneRow;
        }
    }

    public bool UseSmallSlotScale
    {
        get { return (SlotList.Count > 2); }
    }

    public bool TakeDish(FFTDish dish)
    {
        if (SlotList == null)
        {
            Debug.Log("Found null Slotlist... error");
            return false;
        }

        dish.CurrentScale = CurrentDishSlotScale.x;

        foreach (FFTSlot slot in SlotList)
        {
            if (slot.Occupied == false)
            {
                slot.Dish = dish;

                // change the travel time based on how far away the slot is.
                float slotTravelTime = slotTravelTimeClose;
                if (dish.CurrentDestination == Type.Cook || dish.CurrentDestination == Type.Spice)
                    slotTravelTime = slotTravelTimeFar;

                if (slot.MoveDishToCurrentSlotPosition(dish, slotTravelTime))
                {
                    if (slot.Dish.CurrentStepObject.Gameplay == GameplayType.ElapsedTime)
                    {
                        bool StartTimerOnFirstClick = true;

                        if (FFTSlotStationBehaviour.DisplayState == FFTSlotStationBehaviour.TimerDisplayState.Basic)
                        {
                            FFTStationTimerBasic elapsedTimer = slot.gameObject.AddComponent<FFTStationTimerBasic>();
                            elapsedTimer.Setup(slot.Dish.CurrentStepObject);
                            elapsedTimer.Display.transform.parent = dish.gameObject.transform;
                            if (StartTimerOnFirstClick)
                            {
                                slot.gameObject.GetComponent<FFTSlotStationBehaviour>().StartBasicTimer();
                            }
                        }
                        else if (FFTSlotStationBehaviour.DisplayState == FFTSlotStationBehaviour.TimerDisplayState.Gauge)
                        {
                            FFTStationTimerGauge elapsedTimer = slot.gameObject.AddComponent<FFTStationTimerGauge>();
                            if (UseSmallSlotScale)
                                elapsedTimer.Size = FFTStationTimerGauge.TimerSize.Small;
                            elapsedTimer.Setup(slot.Dish.CurrentStepObject);
                            elapsedTimer.Display.transform.parent = slot.transform;
                            if (StartTimerOnFirstClick)
                            {
                                slot.gameObject.GetComponent<FFTSlotStationBehaviour>().StartGaugeTimer();
                            }
                        }
                        
                    }
                    else if (slot.Dish.CurrentStepObject.Gameplay == GameplayType.MiniGame)
                    {
                        slot.gameObject.GetComponent<FFTSlotStationBehaviour>().GameplayType = GameplayType.MiniGame;
                        FFTMinigameIndicatorDisplay minigameDisplay = slot.gameObject.AddComponent<FFTMinigameIndicatorDisplay>();
                        minigameDisplay.SetupIcon(slot.Dish.CurrentStepObject.MinigameType);

                        
                    }
                    return true;
                }
                return false;
            }
        }
        return false;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool AddSlots(int count)
    {
        bool result = true;
        for (int i = 0; i < count; i++)
        {
            result = AddSlot();
        }
        return result;
    }

    public override bool AddSlot()
    {
        if (SlotList.Count < MaxSlots)
        {
            SlotHost.transform.localScale = slotHostScaleOneRow; // coordinates/scale of instantiated prefabs are dependent on 1,1,1 scale, otherwise they come out "big"
            //add slot
            GameObject newSlotGO = CreateSlot();
            FFTSlot newSlot = newSlotGO.GetComponent<FFTSlot>();
            newSlot.Render = true;
            //newSlot.Spline.SetFillColor1(Color.gray);
            newSlot.Spline.RefreshMesh();

            Vector3 slotPosition = new Vector3((SlotList.Count - 1) * slotSpacing, 0, 0);
            if (SlotList.Count > 2)
            {
                slotPosition = new Vector3((SlotList.Count - 3) * slotSpacing, -slotRowSpacing, 0);
            }
            
            FFTStationIcon iconSwitch = newSlotGO.GetComponent<FFTStationIcon>();
            iconSwitch.Destination = (FFTStationIcon.State)Name;

            newSlotGO.transform.localPosition = slotPosition;

            AlignSlotHost();
            UpdateBackground();

            return true;
        }

        return false;
    }

    protected override GameObject CreateSlot()
    {
        InitializeSlotHost(); // just in case

        // creates a new slot at the end of the queue
        GameObject NewSlotGO = Instantiate(Resources.Load("MainGamePrefabs/FFTStationSlotIcon")) as GameObject;

        FFTSlot NewSlotScript = NewSlotGO.GetComponent<FFTSlot>();
        if (NewSlotScript == null)
        {
            Debug.Log("New Slot Script created (not present in prefab!)");
            NewSlotScript = NewSlotGO.AddComponent<FFTSlot>();
        }
        SlotList.Add(NewSlotScript);

        NewSlotGO.name = "Slot (" + (SlotList.Count) + ")";
        NewSlotGO.transform.parent = SlotHost.transform;

        NewSlotScript.Type = FFTSlot.SlotType.Station;
        NewSlotGO.AddComponent<FFTSlotStationBehaviour>();

        return NewSlotGO;
    }

    protected override void DestroySlot()
    {
        // removes the last slot in the queue and destroys the object host
        int lastSlotIndex = SlotList.Count - 1;
        FFTSlot MarkedSlotScript = SlotList[lastSlotIndex];
        SlotList.RemoveAt(lastSlotIndex);

        FFTUtilities.DestroySafe(MarkedSlotScript.gameObject);

        AlignSlotHost();
        UpdateBackground();
    }

    protected override void AlignSlotHost()
    {
        
        if (SlotList.Count > 2)
        {
            SlotHost.transform.localScale = slotHostScaleTwoRows;
            SlotHost.transform.localPosition = slotHostAlignmentTwoRows;
        }
        else
        {
            SlotHost.transform.localScale = slotHostScaleOneRow;
            SlotHost.transform.localPosition = slotHostAlignmentOneRow;
        }
    }

    public void UpdateBackground()
    {
        IRageSpline background = gameObject.GetComponent<RageSpline>() as IRageSpline;
        if (background == null)
            return;
        if (SlotList == null || SlotList.Count == 0)
        {
            background.SetFillColor1(FFTKitchen.BackgroundColor);
            background.SetOutlineColor1(Color.gray);
        }
        else
        {
            background.SetOutlineColor1(Color.black);
            background.SetFillColor1(SlotList[0].GetComponent<FFTStationIcon>().debugPredominantColor);
            /*
            switch (Name)
            {
                case Type.Chop:
                    background.SetFillColor1(Color.cyan);
                    break;
                case Type.Cook:
                    background.SetFillColor1(Color.red);
                    break;
                case Type.Prep:
                    background.SetFillColor1(Color.yellow);
                    break;
                case Type.Spice:
                    background.SetFillColor1(Color.green);
                    break;
            }
             */
        }
        background.RefreshMesh();

    }

    public static FFTStation.Type TypeFromString(string name)
    {
        switch (name)
        {
            case "Chop":
                return Type.Chop;
            case "Cook":
                return Type.Cook;
            case "Prep":
                return Type.Prep;
            case "Spice":
                return Type.Spice;
        }
        return 0;
    }

    public static FFTStation.GameplayType GameplayTypeFromString(string name)
    {
        switch (name)
        {
            case "ElapsedTime":
                return GameplayType.ElapsedTime;
            case "MiniGame":
                return GameplayType.MiniGame;
            default:
                return GameplayType.Empty;
        }

    }
}
