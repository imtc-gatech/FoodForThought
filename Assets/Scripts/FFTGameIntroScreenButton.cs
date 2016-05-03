using UnityEngine;
using System.Collections;

public class FFTGameIntroScreenButton : FFTUIButtonBehaviour {
	
	public FFTGameIntroScreen IntroScreen;

	public override void Clicked ()
	{
		//log that intro screen was dismissed:
		if (FFTGameManager.Instance.LogActions)
		{
			FFTStepAction currentAction = new FFTStepAction();
			currentAction.Reset();
			if (IntroScreen.State == FFTGameIntroScreen.GameState.Intro)
			{
				currentAction.AssignButtonAction(FFTStepAction.InteractionType.Selected, FFTStepAction.SlotActionType.Button_Planning_DismissRecipeCard);
				FFTGameManager.Instance.DataManager.AddAction(currentAction);
			}
		}
		
		IntroScreen.DismissScreen();
		
		//FFTGameManager.Instance.State = FFTGameManager.GameState.Gameplay;	
	}
	
	
}
