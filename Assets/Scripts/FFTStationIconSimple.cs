using UnityEngine;
using System.Collections;

public class FFTStationIconSimple : MonoBehaviour {
	
	public FFTStation.Type Type
	{
		get { return _type;}
		set
		{
			if (Populated && _type != value)
			{
				States[(int)_type - 1].SetActiveRecursively(false);	
				States[(int)value - 1].SetActiveRecursively(true);
				_type = value;
			}
		}
	}
	
	[SerializeField]
	private FFTStation.Type _type = FFTStation.Type.Chop;
	
	public Vector3 Scale = new Vector3(15f, 15f, 1f);
	
	public GameObject[] States;
	
	public bool Populated
	{
		get {return ((States != null) && (States.Length == 4)); }	
	}

	// Use this for initialization
	void Start () {
		foreach (GameObject go in States)
		{
			go.SetActiveRecursively(false);
			if (go.name == Type.ToString())
				go.SetActiveRecursively(true);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
