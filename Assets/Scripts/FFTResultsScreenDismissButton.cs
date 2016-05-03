using UnityEngine;
using System.Collections;

public class FFTResultsScreenDismissButton : FFTUIButtonBehaviour {
	
	public FFTLevel.LoadType NextLevelBehaviour = FFTLevel.LoadType.Next;
	
	public override void Clicked ()
	{
		FFTGameManager.Instance.CleanUpResultsScreen();
		FFTGameManager.Instance.LoadNewLevel(NextLevelBehaviour);
	}
	
}