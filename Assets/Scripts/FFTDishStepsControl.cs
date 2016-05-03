using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTDishStepsControl : MonoBehaviour {

    public List<FFTDishStepCardDisplay> StepCards;

    public static float DishCardWidth = 118;
	// above was 178
    public static int DishCardDisplayQuantity = 4;
    public static Vector3 DishCardHiddenPosition = new Vector3(DishCardWidth, 0, 60); //(DishCardWidth * DishCardDisplayQuantity, 0, 30);

	// Use this for initialization
	void Start () {
        
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetActive(int stepNumber, bool state)
    {
        int index = stepNumber - 1;
        if (index < StepCards.Count)
            StepCards[index].Switch(state);
        //else
            //Debug.Log("(FFTDishStepsControl)Called index out of range in SetActive method.");
    }

    public void SetupHUD(List<FFTStep> steps)
    {
        //Debug.Log(steps.Count);
        StepCards = new List<FFTDishStepCardDisplay>();
        int cardCount = steps.Count + DishCardDisplayQuantity;

        // we always want to have four cards visible, even if there are less than 4 steps.
        //+4 (DishCardDisplayQuantity) because we are going to "slide" new cards from the right side, and this accounts for all current steps being eliminated.

        for (int i = 0; i < cardCount; i++) 
        {
            GameObject stepDisplay = GameObject.Instantiate(Resources.Load("MainGamePrefabs/UI_DishCards/DishCardOriginal")) as GameObject;
            FFTDishStepCardDisplay displayScript = stepDisplay.GetComponent<FFTDishStepCardDisplay>();
            
            stepDisplay.transform.parent = transform;
            stepDisplay.transform.localPosition = new Vector3(DishCardWidth * i, 0, 30);

            if (i < steps.Count)
            {
                stepDisplay.transform.localPosition = new Vector3(DishCardWidth * i, 0, 0);
				// above was 0
                displayScript.Seconds = steps[i].Parameters.Uncooked;
                displayScript.State = steps[i].Destination;
                displayScript.GameplayType = steps[i].Gameplay;
                displayScript.HasData = true;
                if (displayScript.GameplayType == FFTStation.GameplayType.ElapsedTime)
                {
                    stepDisplay.name = (i + 1) + "-DishCard-" + displayScript.Seconds.ToString() + "-" + displayScript.State.ToString();
                }
                else if (displayScript.GameplayType == FFTStation.GameplayType.MiniGame)
                {
                    displayScript.MinigameType = steps[i].MinigameType;
                    stepDisplay.name = (i + 1) + "-DishCard-MG_" + steps[i].MinigameType.ToString() + "-" + displayScript.State.ToString();
                    GameObject minigameIcon = GameObject.Instantiate(Resources.Load("MainGamePrefabs/MinigameIcon")) as GameObject;
                    minigameIcon.transform.parent = stepDisplay.gameObject.transform;
                    displayScript.MinigameIcon = minigameIcon.GetComponent<FFTMinigameIcon>();
                    displayScript.MinigameIcon.State = displayScript.MinigameType;
                    displayScript.MinigameIcon.Refresh();

                    displayScript.MinigameIcon.gameObject.transform.localScale = new Vector3(14, 14, 14);
                    displayScript.MinigameIcon.gameObject.transform.localPosition = new Vector3(3, 93, 0);
                }
            }
            else
            {
                displayScript.HasData = false;
                displayScript.Switch(false);
                stepDisplay.name = (i + 1) + "-Blank";
            }
            if (i < DishCardDisplayQuantity)
                stepDisplay.transform.localPosition = new Vector3(DishCardWidth * i, 0, 0);
            else
				stepDisplay.transform.localPosition = new Vector3(DishCardWidth * i, 0, 0);
                //stepDisplay.transform.localPosition = DishCardHiddenPosition;
            
			stepDisplay.transform.localPosition = new Vector3(DishCardWidth * i, 0, -10 + i);

            StepCards.Add(displayScript);
        }

        /*

        
        foreach (FFTDishStepCardDisplay stepCard in StepCards)
        {
            stepCard.HasData = false;
            stepCard.Switch(false);
           
        }
        int index = 0;
        foreach (FFTStep step in steps)
        {
            StepCards[index].Seconds = step.Parameters.Uncooked;
            StepCards[index].State = step.Destination;
            StepCards[index].gameObject.name = (index + 1) + "-DishCard-" + StepCards[index].Seconds.ToString() + "-" + step.Destination.ToString();
            StepCards[index].HasData = true;
            if (index < 4)
                StepCards[index].Switch(true);
            index++;
        }
         */
    }

    public void SetFinished()
    {
        foreach (FFTDishStepCardDisplay stepCard in StepCards)
        {
            //TODO: do something to indicate the dish is done!

            stepCard.Finished();
        }

    }

    public void PopCard()
    {
        SetActive(1, false);
        FFTDishStepCardDisplay destroyDisplay = StepCards[0];
        StepCards.RemoveAt(0);
        FFTUtilities.DestroySafe(destroyDisplay.gameObject);
        for (int i = 0; i < StepCards.Count; i++)
        {
            if (i < DishCardDisplayQuantity)
            {
                //iTweenUtilities.MoveByLocal(StepCards[i].gameObject, new Vector3(DishCardWidth * i, 0, 0), 0.5f);
                GameObject go = StepCards[i].gameObject;
                //Vector3 worldCoordinates = go.transform.parent.TransformDirection(DishCardWidth * i, 0, 0);

				// z below was 0
                Hashtable ht = new Hashtable(){
                    {iT.MoveTo.x, DishCardWidth * i},
                    {iT.MoveTo.y, 0},
                    {iT.MoveTo.z, -10 + i},
                    {iT.MoveTo.time, 0.5f},
                    {iT.MoveTo.islocal, true}
                    
                };

                iTween.MoveTo(go, ht);

                //StepCards[i].gameObject.transform.localPosition = new Vector3(DishCardWidth * i, 0, 0);
            }
        }     
    }

    /*

    void OnMouseEnter()
    {
        transform.parent.gameObject.GetComponent<FFTSlotCounterBehaviour>().Entered();
    }

    void OnMouseExit()
    {
        transform.parent.gameObject.GetComponent<FFTSlotCounterBehaviour>().Exited();
    }

    void OnMouseDown()
    {
        transform.parent.gameObject.GetComponent<FFTSlotCounterBehaviour>().Selected();
    }
     */
}
