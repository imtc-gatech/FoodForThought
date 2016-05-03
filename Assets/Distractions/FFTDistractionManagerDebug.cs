using UnityEngine;
using System.Collections;


/// <summary>
/// Class called by the FFTDistractionManager when debug keys are needed.
/// </summary>
public class FFTDistractionManagerDebug : MonoBehaviour {
	
	private FFTDistractionManager manager;
	
	// Use this for initialization
	void Awake () {
		manager = gameObject.GetComponent<FFTDistractionManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.D)){
			manager.SpawnDistraction(FFTDistraction.Type.Fly);
		}
		else if(Input.GetKeyUp(KeyCode.F)){
			FFTStation.Type stationType = (FFTStation.Type)Random.Range (1, 5); //4 types of stations
			manager.SpawnDistraction(FFTDistraction.Type.Fire, stationType);
		}
		else if(Input.GetKeyUp(KeyCode.C)){
			manager.ClearAllDistractions();
		}
		else if(Input.GetKeyUp(KeyCode.A)){
			manager.ButtonHolder.Activate();
		}
		else if(Input.GetKeyUp(KeyCode.Z)){
			manager.ButtonHolder.Deactivate();
		}
		
	}
}
