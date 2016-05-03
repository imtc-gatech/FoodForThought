using UnityEngine;
using System.Collections;

public class FFTDishStepDisplay : MonoBehaviour {
	
	public static string OBJECT_DISPLAY_NAME = "SecondsDisplay";
	public static string OBJECT_ICON_NAME = "StationIconDisplay";
	public static string OBJECT_TILE_NAME = "Rectangle";
	
	public FFTStation.Type Destination
	{
		get {
			if (stationIcon == null)
				stationIcon = transform.FindChild(OBJECT_ICON_NAME).gameObject.GetComponent<FFTStationIcon>();
			return (FFTStation.Type)stationIcon.Destination;
		}
		set {
			if (stationIcon == null)
				stationIcon = transform.FindChild(OBJECT_ICON_NAME).gameObject.GetComponent<FFTStationIcon>();
			if ((int)value != (int)stationIcon.Destination)
			{
				if (backgroundTile == null)
					backgroundTile = transform.FindChild(OBJECT_TILE_NAME).gameObject.GetComponent<RageSpline>() as IRageSpline;
				stationIcon.Destination = (FFTStationIcon.State)value;
				if (stationIcon.Destination == FFTStationIcon.State.Prep)
					backgroundTile.SetFillColor1(stationIcon.debugPredominantColor);
				else
					backgroundTile.SetFillColor1(stationIcon.debugComplimentaryColor);
				backgroundTile.RefreshMesh(false, false, false);

			}
		}
	}

	public string DisplayText
	{
		get {
			if (displayText == null)
				displayText = transform.FindChild(OBJECT_DISPLAY_NAME).gameObject.GetComponent<TextMesh>();
			return displayText.text;
		}
		set {
			if (displayText == null)
				displayText = transform.FindChild(OBJECT_DISPLAY_NAME).gameObject.GetComponent<TextMesh>();
			displayText.text = value;
		}
	}
	
	private IRageSpline backgroundTile;
	private FFTStationIcon stationIcon;
	private TextMesh displayText;

	// Use this for initialization
	void Awake () {
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
}
