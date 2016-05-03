using UnityEngine;
using System.Collections;

[System.Serializable]
public class FFTDistractionAction {
	
	public enum Type
	{
		Button,
		Distraction,
		LevelFinished
	}
	
	public Type actionType;
	public FFTDistraction.Type distractionType;
	public float timestamp;
	public bool enabled = false;
	public int instanceID;
	
	
	public FFTDistractionAction(Type theType, FFTDistraction.Type theDistractionType,  bool isEnabled, int actionInstanceID, float theTimestamp)
	{
		actionType = theType;
		distractionType = theDistractionType;
		enabled = isEnabled;
		instanceID = actionInstanceID;	
		timestamp = theTimestamp;
	}
	
	
}