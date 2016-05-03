using UnityEngine;
using System.Collections;

public class csStation : MonoBehaviour {

	public enum StationType {
		Cook = 0,
		Chop = 1,
		Spice = 2,
		Prep = 3
	}

	public csStationSlot[] slots;

	private int MAX_STATION_SLOTS = 4;
	private int MIN_STATION_SLOTS = 1;


	public void ConfigureStations (int numSlots) {

		// Currently supports 1-4 slots per station.  We can consider alternatives later.
		// Slot could be made larger when there is only one, if we want...

		for (int i = MAX_STATION_SLOTS - 1; i >= numSlots; i--) {
			// Remove slot location/background objects for extra slots.
			Destroy(slots[i].gameObject);
		}

		csStationSlot[] newSlots = new csStationSlot[numSlots];

		for (int i = 0; i < numSlots; i++) {
			newSlots[i] = slots[i];

			// Also locally center Y, if there are few slots.
			if (numSlots < 3) {
				slots[i].transform.localPosition = new Vector3(slots[i].transform.localPosition.x, 0.0f);
			}
		}

		slots = newSlots;
	}

	public csStationSlot GetEmptySlot() {
		for (int i = 0; i < slots.Length; i++) {
			if (!slots[i].HasDish()) {
				// We can receive a dish.
				return slots[i];
			}
		}

		// Can't take a dish.
		return null;
	}

}
