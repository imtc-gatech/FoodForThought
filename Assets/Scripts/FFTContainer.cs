using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTContainer : MonoBehaviour
{
    public static float XPosition = 171;
    public static float DishSpacing = 3;

    public static string slotHostName = "Slot Positions";

    public GameObject SlotHost;

    public List<FFTSlot> SlotList = new List<FFTSlot>();

    public int MaxSlots = 4;

    public FFTSlot LastSlot
    {
        get
        {
            if ((SlotList != null) && SlotList.Count > 0)
            {
                return SlotList[SlotList.Count - 1];
            }
            return null;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual bool AddSlot()
    {
        if (SlotList.Count < MaxSlots)
        {
            //add slot
            GameObject newSlotGO = CreateSlot();
            Vector3 slotPosition = new Vector3();

            float yCeiling = 80;
            //float yFloor = -80;
            float yPadding = 10;
            float ySpacing = 35;

            float tempPosition = (SlotList.Count - 1) * (ySpacing + yPadding);

            slotPosition.y = yCeiling - yPadding - tempPosition;
            newSlotGO.transform.localPosition = slotPosition;

            return true;
        }

        return false;
    }

    public virtual bool RemoveSlot()
    {
        if (SlotList.Count > 0)
        {
            //remove last slot
            DestroySlot();
            return true;
        }

        return false;
    }

    protected virtual void AlignSlotHost()
    {

    }

    protected virtual GameObject CreateSlot()
    {
        InitializeSlotHost(); // just in case

        // creates a new slot at the end of the queue
        GameObject NewSlotGO = Instantiate(Resources.Load("MainGamePrefabs/FFTSlotEmpty")) as GameObject;

        FFTSlot NewSlotScript = NewSlotGO.GetComponent<FFTSlot>();
        if (NewSlotScript == null)
        {
            Debug.Log("New Slot Script created (not present in prefab!)");
            NewSlotScript = NewSlotGO.AddComponent<FFTSlot>();
        }
        SlotList.Add(NewSlotScript);

        NewSlotGO.name = "Slot (" + (SlotList.Count) + ")";
        NewSlotGO.transform.parent = SlotHost.transform;

        return NewSlotGO;
    }

    protected virtual void DestroySlot()
    {
        // removes the last slot in the queue and destroys the object host
        int lastSlotIndex = SlotList.Count - 1;
        FFTSlot MarkedSlotScript = SlotList[lastSlotIndex];
        SlotList.RemoveAt(lastSlotIndex);

        FFTUtilities.DestroySafe(MarkedSlotScript.gameObject);

        AlignSlotHost();
    }

    public virtual void InitializeSlotHost()
    {
        Transform SlotHostTransform = gameObject.transform.FindChild(slotHostName);

        if (SlotHostTransform != null)
        {
            SlotHost = SlotHostTransform.gameObject;
        }
        else
        {
            GameObject NewSlotHost = new GameObject(slotHostName);
            NewSlotHost.transform.parent = gameObject.transform;
            //FFTUtilities.OffsetGameObjectPosition(NewSlotHost, z: -3);
            Vector3 slotHostReposition = new Vector3();
            slotHostReposition.x = -13.0f;
            slotHostReposition.z = -3.0f;
            NewSlotHost.transform.localPosition = slotHostReposition;
            SlotHost = NewSlotHost;
        }
    }

}
