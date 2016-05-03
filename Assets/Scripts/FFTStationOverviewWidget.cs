using UnityEngine;
using System.Collections;

public class FFTStationOverviewWidget : MonoBehaviour {
	
	public int chopSlots = 0;
	public int cookSlots = 0;
	public int spiceSlots = 0;
	public int prepSlots = 0;
	
	public TextMesh chopText;
	public TextMesh cookText;
	public TextMesh spiceText;
	public TextMesh prepText;
	
	public FFTStationIcon chopIcon;
	public FFTStationIcon cookIcon;
	public FFTStationIcon spiceIcon;
	public FFTStationIcon prepIcon;
	

	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void RefreshDisplay()
	{
		chopIcon.Destination = FFTStationIcon.State.Chop;
		cookIcon.Destination = FFTStationIcon.State.Cook;
		prepIcon.Destination = FFTStationIcon.State.Prep;
		spiceIcon.Destination = FFTStationIcon.State.Spice;
		
		chopText.gameObject.SetActiveRecursively(true);
		cookText.gameObject.SetActiveRecursively(true);
		spiceText.gameObject.SetActiveRecursively(true);
		prepText.gameObject.SetActiveRecursively(true);
		
		if (chopSlots > 0)
			chopText.text = chopSlots.ToString();
		else
			chopText.text = "";
		if (cookSlots > 0)
			cookText.text = cookSlots.ToString();
		else
			cookText.text = "";
		if (spiceSlots > 0)
			spiceText.text = spiceSlots.ToString();
		else
			spiceText.text = "";
		if (prepSlots > 0)
			prepText.text = prepSlots.ToString();
		else
			prepText.text = "";

	}
	
	public void OnDestroy()
	{
		GameObject.Destroy(gameObject);
	}
}
