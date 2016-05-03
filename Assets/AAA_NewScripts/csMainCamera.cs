using UnityEngine;
using System.Collections;

public class csMainCamera : MonoBehaviour {

	public csGameManager prefabGameManager;

	void Awake () {
		csGameManager.game.WakeUp ();
	}
}
