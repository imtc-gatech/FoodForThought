using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTCounter : FFTContainer {

    public FFTRecipe RecipeCard;

    public bool RecipeFinished
    {
        get
        {
            foreach (FFTDish dish in RecipeCard.Dishes)
            {
                if (!dish.Finished)
                    return false;
            }
            return true;
        } 
    }

    public override bool AddSlot()
    {
        return AddSlot(true);
    }

    public bool AddSlot(bool createEmptyDishInSlot)
    {
        if (SlotList.Count < MaxSlots)
        {
            //add slot
            GameObject newSlotGO = CreateSlot(createEmptyDishInSlot);
            Vector3 slotPosition = new Vector3();

            float xOffset = -72;
            float yCeiling = 91;
            //float yFloor = -80;
            float yPadding = 15;
            float ySpacing = 35;

            float tempPosition = (SlotList.Count - 1) * (ySpacing + yPadding);

            slotPosition.x = xOffset;
            slotPosition.y = yCeiling - yPadding - tempPosition;
            newSlotGO.transform.localPosition = slotPosition;

            if (createEmptyDishInSlot)
            {
                FFTSlot slot = newSlotGO.GetComponent<FFTSlot>();
                slot.Dish.gameObject.transform.position = newSlotGO.transform.position;
                slot.Render = true;
            }

            return true;
        }

        return false;
    }

    GameObject CreateSlot(bool createEmptyDishInSlot)
    {
        GameObject NewSlotGO = base.CreateSlot();

        FFTSlot newSlot = NewSlotGO.GetComponent<FFTSlot>();

        newSlot.Type = FFTSlot.SlotType.Counter;
        NewSlotGO.AddComponent<FFTSlotCounterBehaviour>();

        if (createEmptyDishInSlot)
            CreateNewEmptyDish();

        return NewSlotGO;
    }

    protected override void DestroySlot()
    {
        // removes the last slot in the queue and destroys the object host
        int lastSlotIndex = SlotList.Count - 1;
        FFTSlot MarkedSlotScript = SlotList[lastSlotIndex];
        SlotList.RemoveAt(lastSlotIndex);

        FFTUtilities.DestroySafe(MarkedSlotScript.gameObject);

        DestroyDish(lastSlotIndex);

        /*
        FFTDishImporter deadDish = RecipeCard.Dishes[lastSlotIndex];
        FFTUtilities.DestroySafe(deadDish.gameObject);
        RecipeCard.Dishes.RemoveAt(lastSlotIndex);
         */
    }

    public override void InitializeSlotHost()
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
            Vector3 slotHostReposition = new Vector3();
            slotHostReposition.x = -13.0f;
            slotHostReposition.z = -3.0f;
            NewSlotHost.transform.localPosition = slotHostReposition;
            SlotHost = NewSlotHost;
        }
    }

    void CreateNewEmptyDish()
    {
        GameObject NewDishGO = new GameObject();
        FFTDish newDish = NewDishGO.AddComponent<FFTDish>();
        RecipeCard.Dishes.Add(newDish);
        int newDishID = RecipeCard.Dishes.Count;
        RecipeCard.Dishes[RecipeCard.Dishes.Count - 1].ID = newDishID;
        NewDishGO.gameObject.transform.parent = gameObject.transform;
        NewDishGO.gameObject.name = "Dish (" + newDishID + ") Empty";
        SlotList[SlotList.Count - 1].Dish = newDish;
    }

    void DestroyDish(int index)
    {
        Debug.Log("DestroyDish called");
        if (index > RecipeCard.Dishes.Count || index < 0)
        {
            Debug.Log("Index (" + index + ") out of range.");
            return;
        }
        FFTDish deadDish = RecipeCard.Dishes[index];
        FFTUtilities.DestroySafe(deadDish.gameObject);
        RecipeCard.Dishes.RemoveAt(index);

    }

    public void FreezeFreshnessForServing()
    {
        if (RecipeCard.FreshnessParameters.UseFreshness)
        {
            foreach (FFTDish dish in RecipeCard.Dishes)
            {
                dish.FreshnessMeter.IsRunning = false;
            }
        }
    }
    
}
