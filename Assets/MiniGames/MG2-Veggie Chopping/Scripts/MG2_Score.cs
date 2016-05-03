using UnityEngine;
using System.Collections;

public class MG2_Score : MonoBehaviour {
	public float flavor;
	public float freshness;
	public float presentation;
	public string feedback;
	public int overCount;
	public int underCount;
	public int closeEnough;
	// Use this for initialization
	void Start () {
		overCount = 0;
		underCount = 0;
		closeEnough = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
