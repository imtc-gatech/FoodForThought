using UnityEngine;
using System.Collections;

public class FFTFreshnessMeterView : MonoBehaviour {
	
	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}
	
	public float Value
	{ 
		get { return _value; } 
		set
		{
			_value = Mathf.Clamp(value, 0, 1);
			ResizeForeground();
		}
	}
	[SerializeField]
	private float _value = 1.0f;
	
	public Direction ShrinkDirection = Direction.Right;
	
	public float BorderWidth = 0.1f;
	
	public float aspectRatio = 1f;
	
	public float BorderWidthAdjust
	{
		get { return BorderWidth / aspectRatio; }
	}
	
	public Transform Foreground;
	public Transform Background;
	public Transform ForegroundBorder;
	public Transform BackgroundBorder;

	// Use this for initialization
	void Start () {
		Foreground = transform.FindChild("Foreground");
		Background = transform.FindChild("Background");
		ForegroundBorder = Foreground.transform.FindChild("Border");
		BackgroundBorder = Background.transform.FindChild("Border");
		Vector3 scale = transform.localScale;
		if (scale.x != scale.y)
		{
			if (scale.x > scale.y)
				aspectRatio = scale.x / scale.y;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}
	
	void ResizeForeground() {
		
		float lowerBoundBorder = 0.02f;
	
		switch (ShrinkDirection) {
		case Direction.Up:
			Foreground.localScale = new Vector3(1, Value, 1);
			Foreground.localPosition = new Vector3(0, (1-Value) / -2, 0);
			ForegroundBorder.localScale = new Vector3(1 + BorderWidthAdjust, 
												  	1 + (BorderWidth / Mathf.Clamp(Value, lowerBoundBorder, 1)),
												  	1);				
			break;
		case Direction.Down:
			Foreground.localScale = new Vector3(1, Value, 1);
			Foreground.localPosition = new Vector3(0, (1-Value) / 2, 0);
			ForegroundBorder.localScale = new Vector3(1 + BorderWidthAdjust, 
												  	1 + (BorderWidth / Mathf.Clamp(Value, lowerBoundBorder, 1)),
												  	1);			
			break;
		case Direction.Left:
			Foreground.localScale = new Vector3(Value, 1, 1);
			Foreground.localPosition = new Vector3((1-Value) / 2, 0, 0);
			ForegroundBorder.localScale = new Vector3(1 + (BorderWidthAdjust / Mathf.Clamp(Value, lowerBoundBorder, 1)), 
												  	1 + (BorderWidth),
												  	1);
			break;
		case Direction.Right:
			Foreground.localScale = new Vector3(Value, 1, 1);
			Foreground.localPosition = new Vector3((1-Value) / -2, 0, 0);
			ForegroundBorder.localScale = new Vector3(1 + (BorderWidthAdjust / Mathf.Clamp(Value, lowerBoundBorder, 1)), 
												  	1 + (BorderWidth),
												  	1);
			break;
		}
		
		BackgroundBorder.localScale = new Vector3(1 + BorderWidthAdjust, 1 + BorderWidth, 1);
	}
}
