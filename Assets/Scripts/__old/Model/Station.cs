using UnityEngine;
using System.Collections;

public class Station {

    public enum Type 
    {
        Chop, 
        Cook, 
        Prep, 
        Spice, 
        Finish 
    };

    public enum GameplayType {
        Empty, 
        ElapsedTime, 
        MiniGame 
    };

    private Type type;
    private GameObject go;
    private int slotCount;

    private int slotsOccupied;

    private Slot[] slots;

    public Station(Station.Type passedType)
    {
        type = passedType;
        slotsOccupied = 0;
    }

    public void SetGameObject(GameObject passedGO)
    {
        if (go == null)
        {
            go = passedGO;
        }
        else
        {
            Debug.Log("This game object has already been assigned.");
        }

    }

    public Station.Type GetStationType()
    {
        return type;
    }

    public void SetSlotCount(int count)
    {
        slotCount = count;
        slots = new Slot[slotCount];

    }

    public bool SlotAvailable()
    {
        return (slotsOccupied < slotCount);
    }

    public void TakeSlot()
    {
        slotsOccupied++;
    }

    public static Station.Type TypeFromString(string name)
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
            default:
                return Type.Finish;
        }
    }

    public static Station.GameplayType GameplayTypeFromString(string name)
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
