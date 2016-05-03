using UnityEngine;
using System.Collections;

/// <summary>
/// FFT distraction.  This is the base form of the distraction class.  
/// Distractions usually talk to the manager to get things done, unless they're dealing with their basic functionality.
/// </summary>
public class FFTDistraction : MonoBehaviour {
	public enum Type
	{
		Fly,
		Fire,
		None
	}
	
	public FFTDistractionManager Manager;
	public FFTStation.Type AffectedStation;
	public float EffectMultiplier;
	public bool DoesBlock;
	public Type OwnType = Type.None;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy(){ //this is often overwritten
		FFTGameManager.Instance.Kitchen.RemoveDistractionEffect(AffectedStation);
		Manager.CurrentDistractions.Remove(this);
	}
	
	virtual public void MakeActive(){
		
	}
	
	virtual public void MakeInactive(){
	}
	
	void LogSpawn() {
		
	}
	
	void LogDestroy() {
		
	}
	
	void LogClear() {
		
	}
}
