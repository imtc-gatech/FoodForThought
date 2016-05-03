using UnityEngine;
using System.Collections;

public class csMainMenu : MonoBehaviour {

	void Update () {
	
		if (Input.GetMouseButtonDown (0)) {
			// On any touch, load first level.
			csGameManager.game.LoadLevelRelative(0);
		}
	}
}
