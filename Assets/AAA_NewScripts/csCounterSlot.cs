using UnityEngine;
using System.Collections;

public class csCounterSlot : MonoBehaviour {

	private Vector3 dishLocation;
	private Vector3 baseDishStepLocation;

	// A counter slot contains <= one dish.
	private csDish dish;

	void Awake () {
		dishLocation = transform.FindChild ("DishLocation").transform.position;
		baseDishStepLocation = transform.FindChild ("DishStepLocation").transform.position;
	}

	public csDish AddDish (string dishName, csDish prefabDish) {

		csDish newDish = Instantiate (prefabDish);
		newDish.transform.position = dishLocation;
		newDish.dishName = dishName;
		newDish.AddToCounterSlot (this.GetComponent<csCounterSlot> ());

		dish = newDish;

		return newDish;

	}

	public Vector3 getDishLocation() {
		return dishLocation;
	}

	public Vector3 getBaseDishStepLocation() {
		return baseDishStepLocation;
	}

	public bool DishIsDone() {
		return dish.IsDone ();
	}
}
