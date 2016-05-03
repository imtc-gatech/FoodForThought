using UnityEngine;
using System.Collections;

public class csStationSlot : MonoBehaviour {

	private csDish dish = null;
	

	public bool HasDish () {
		return (dish != null);
	}

	public void TakeDish (csDish dishToReceive) {
		dish = dishToReceive;
	}

	public void LoseDish () {
		dish = null;
	}
}
