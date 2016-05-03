using UnityEngine;
using System.Collections;

public class csCounter : MonoBehaviour {

	public csCounterSlot[] slots;
	public csDish prefabDish;
	public csDishStep[] prefabDishSteps;

	// Private attributes to prevent editor changes.
	private int MAXIMUM_DISHES = 4;

	private int numFilledSlots = 0;

	private csLevelManager levelManager;

	void Awake () {
		levelManager = Camera.main.GetComponent<csLevelManager> ();
	}

	public csDish AddDish(string dishName) {
		if (numFilledSlots >= MAXIMUM_DISHES) {
			return null;
		}

		csDish newDish = slots [numFilledSlots].AddDish (dishName, prefabDish);

		numFilledSlots++;

		return (newDish);
	}

	public void RemoveUnusedSlots () {
		for (int i = slots.Length-1; i >= numFilledSlots; i--) {
			Destroy(slots[i]);
			slots[i] = null;
		}

		csCounterSlot[] newSlots = new csCounterSlot[numFilledSlots];

		for (int i = 0; i < numFilledSlots; i++) {
			newSlots[i] = slots[i];
		}

		slots = newSlots;
	}

	public void FinalizeCounterPrep() {
		// Removes dish step type prefabs.
		Destroy (transform.FindChild ("DishStepTypes").gameObject);
	}

	public void CheckForLevelCompletion() {
		bool levelDone = true;
		for (int i = 0; i < numFilledSlots; i++) {
			if (!slots [i].DishIsDone ()) {
				levelDone = false;
			}
		}
		if (levelDone) {
			levelManager.FinishLevel();
		}
	}

}
