using UnityEngine;
using System.Collections;

public class FFTFreshnessMeterTest : MonoBehaviour {
	
	public FFTFreshnessMeterView View;
	
	public float Value = 1.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Value = Mathf.Clamp(Value, 0, 1);
		View.Value = Value;
	
	}
}
