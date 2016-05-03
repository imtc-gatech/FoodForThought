using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class csDish : MonoBehaviour {

	public string dishName = "Not named";

	private float DISH_FLY_TIME = 0.25f;
	private int EMPTY_DISH_STEP = 4;
	private float DISH_STEP_SPACING = 29.0f;
	private float DISH_STEP_WIDTH = 24.0f;
	private float DISH_STEP_HEIGHT = 126.0f;
	private float DISH_STEP_BORDER = 2.0f;
	private float STEP_SLIDE_TIME = 0.25f;

	private csLevelManager levelManager;
	private csSpriteManager spriteManager;

	private csCounter counter;
	private csCounterSlot counterSlot;
	private csStationSlot stationSlot;

	private int currentStep = 0;

	private SpriteRenderer dishViz;

	private List<csDishStep> steps;

	void Awake () {
		counter = GameObject.Find ("Counter").GetComponent<csCounter> ();
		steps = new List<csDishStep> ();
		dishViz = transform.FindChild ("DishViz").GetComponent<SpriteRenderer> ();
		levelManager = Camera.main.GetComponent<csLevelManager> ();
		spriteManager = Camera.main.GetComponent<csSpriteManager> ();
	}

	void Start () {
		// All dishes start at the viz of their first step.
		//dishViz.transform.localScale /= 10.0f;
		SetDishViz (steps [0].spriteNumber);
	}

	public csDishStep AddStep(csStation.StationType destination) {
		return AddStep ((int)destination);
	}

	public csDishStep AddStep(int destination) {

		csDishStep newStep = Instantiate (counter.prefabDishSteps [destination]);
		newStep.transform.position = counterSlot.getBaseDishStepLocation() + (Vector3.right * currentStep * DISH_STEP_SPACING);
		newStep.transform.localScale = new Vector3 (DISH_STEP_WIDTH, DISH_STEP_HEIGHT);
		newStep.transform.parent = counterSlot.transform;
		currentStep++;
		steps.Add (newStep);
		return (newStep);

	}

	public void FinalizeDishPrep() {
		// Adds empty steps to pad out the display for later.
		AddStep (EMPTY_DISH_STEP);
		AddStep (EMPTY_DISH_STEP);
		AddStep (EMPTY_DISH_STEP);
		AddStep (EMPTY_DISH_STEP);

		currentStep = 0;
	}

	public void SetDishViz (int spriteNumber) {
		// Final number is for cooked/uncooked/burned/spoiled/etc.
		dishViz.sprite = spriteManager.sprites[dishName + "-" + spriteNumber + "-1"];
	}

	public void AddToCounterSlot (csCounterSlot slot) {
		counterSlot = slot;
	}

	void OnMouseDown () {
		// Dish was tapped.
		if (IsDone ()) {
			return;
		}
		if (IsOnCounter()) {
			// Move dish to next station, if possible.
			csStation.StationType destStation = steps[currentStep].stepDestination;
			csStationSlot emptySlot = levelManager.kitchen.stations[(int)destStation].GetEmptySlot();

			if (emptySlot != null) {
				// Move dish and start step.
				StartCoroutine(levelManager.tweenManager.PositionTweenTo(transform, emptySlot.transform.position, DISH_FLY_TIME));
				stationSlot = emptySlot;
				emptySlot.TakeDish(this);
			} else {
				// Can not move dish.  Brief shake and ignore input.
				StartCoroutine(levelManager.tweenManager.ShakePosition(transform));
			}
		} else {
			// Move dish back to counter and complete current step.
			StartCoroutine(levelManager.tweenManager.PositionTweenTo(transform, counterSlot.getDishLocation(), DISH_FLY_TIME));
			stationSlot.LoseDish();
			stationSlot = null;
			AdvanceStep();
		}
	}

	private void AdvanceStep() {
		steps [currentStep].transform.position = levelManager.getBitBucket ();

		if (currentStep >= steps.Count) {
			return;
		}

		// Slide steps down.
		currentStep++;
		SlideStepsLeft ();

		// See if dish is done.
		if (IsDone ()) {
			dishViz.sprite = spriteManager.sprites ["CompletedDish"];
			counter.CheckForLevelCompletion ();
		} else {
			// Update sprite to new state.
			SetDishViz (steps [currentStep].spriteNumber);
		}
	}

	private void SlideStepsLeft() {
		for (int i = currentStep; i < steps.Count; i++) {
			Transform stepTrans = steps[i].transform;
			StartCoroutine(levelManager.tweenManager.PositionTweenTo(stepTrans,
			                                                         new Vector3(stepTrans.position.x - DISH_STEP_SPACING,
			            														 stepTrans.position.y, stepTrans.position.z),
			                                                         			 STEP_SLIDE_TIME));

		}
	}

	public bool IsOnCounter() {
		return (stationSlot == null);
	}

	public bool IsDone() {
		return (steps [currentStep].gameplayType == csDishStep.StepGameplayType.Empty);
	}

}
