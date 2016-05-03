using UnityEngine;
using System.Collections;

public class FFTDishDrawer : MonoBehaviour {
	public bool MoveDrawer = false;
	bool lastDrawer = false;
	FFTCounter Counter;
	public int maxSteps = 0;
	Vector3 homeCounterPosition;	
	public bool UseShadow = true;
	
	bool drawerBumped = false;
	
	public FFTDishDrawerArrow Arrow;
	
	public FFTDishDrawerTrigger Trigger;
	
	static int xOffsetPerStepCard = -18;
	static int maxStepsToDisplay = 15;
	static float drawerAnimationSpeedInSeconds = 0.5f;

	// Use this for initialization
	void Start () {
		Counter = FFTGameManager.Instance.Counter;
		
		Trigger = Counter.gameObject.AddComponent<FFTDishDrawerTrigger>();
		Trigger.Drawer = this;
		
		maxSteps = Mathf.Clamp(Counter.RecipeCard.MaximumNumberOfSteps, 0, maxStepsToDisplay);
		homeCounterPosition = Counter.transform.localPosition; //(157.4161, 0.4820633, -2) //157=4,139=5,121=6 //width=18
		
		Arrow.Hide();
		if (maxSteps > 4)
		{
			ShowArrow();
			Arrow.transform.localPosition += new Vector3(-132.5f, 0, 225);
			Arrow.transform.localScale = new Vector3(0.35f, 0.35f, 1);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (lastDrawer != MoveDrawer)
		{
			MoveCounterDrawer();
		}
		lastDrawer = MoveDrawer;
	}
	
	public void ShowArrow()
	{
		if (maxSteps > 4)	
			Arrow.Show();
	}
	
	public void CleanUpForGameplay()
	{
		MoveDrawer = false;
		FFTUtilities.DestroySafe(Trigger);
		Arrow.Hide();
		
		/*
		if (FFTGameManager.Instance.UseShadow)
			FFTUtilities.DestroySafe(FFTGameManager.Instance.Shadow.gameObject);
		*/
			//FFTUtilities.DestroySafe(shadowGO);
	}
	
	void LogDrawerMovement() {
		//log that intro screen was dismissed:			
		if (FFTGameManager.Instance.LogActions)
		{
			FFTStepAction currentAction = new FFTStepAction();
			currentAction.Reset();
			if (MoveDrawer)
				currentAction.AssignButtonAction(FFTStepAction.InteractionType.Selected, FFTStepAction.SlotActionType.Button_Planning_ShowExtraSteps);
			else
				currentAction.AssignButtonAction(FFTStepAction.InteractionType.Selected, FFTStepAction.SlotActionType.Button_Planning_HideExtraSteps);
			FFTGameManager.Instance.DataManager.AddAction(currentAction);
		}
	}
	
	void MoveCounterDrawer() {
		if (MoveDrawer)
		{
			
			float xDrawerMovement = xOffsetPerStepCard*.5f;
			if (drawerBumped)
			{
				xDrawerMovement = xDrawerMovement / 2;
				drawerBumped = false;
			}
			
			if (maxSteps > 4)
			{
				LogDrawerMovement();
				//move drawer to the left based on the maximum Steps
				xDrawerMovement += xOffsetPerStepCard * (maxSteps-4);
				Vector3 localDestination = homeCounterPosition + new Vector3(xDrawerMovement, 0, 0);
				FFTGameManager.Instance.Shadow.FadeIn(drawerAnimationSpeedInSeconds - 0.1f);
				//FadeInShadow(drawerAnimationSpeedInSeconds - 0.1f);
				TweenDrawerMovement(localDestination, drawerAnimationSpeedInSeconds);
				//Counter.transform.localPosition = homeCounterPosition + new Vector3(xDrawerMovement, 0, 0);
			}
			else
			{
				//TODO: shake the counter to denote that there are no extra steps to see
				
				//Vector3 localDestination = homeCounterPosition + new Vector3(xDrawerMovement, 0, 0);
				//FadeInShadow(drawerAnimationSpeedInSeconds - 0.1f);
				//TweenDrawerMovement(localDestination, drawerAnimationSpeedInSeconds);
				
				//Counter.transform.localPosition = homeCounterPosition + new Vector3(xDrawerMovement, 0, 0);
			}
			
			
		}
		else
		{
			if (maxSteps > 4)
			{
				LogDrawerMovement();
				//move drawer to the right based on the maximum Steps
				TweenDrawerMovement(homeCounterPosition, drawerAnimationSpeedInSeconds);
				FFTGameManager.Instance.Shadow.FadeOut(drawerAnimationSpeedInSeconds - 0.1f);
				//FadeOutShadow(drawerAnimationSpeedInSeconds - 0.1f);
			}
			else
			{
				//TODO: shake(?) the counter to denote that there are no extra steps to hide
				
				//TweenDrawerMovement(homeCounterPosition, drawerAnimationSpeedInSeconds);
				//FadeOutShadow(drawerAnimationSpeedInSeconds - 0.1f);
			}
			
		}
	}
	
	void TweenDrawerMovement(Vector3 target, float time)
	{
		float xMovement = Counter.gameObject.transform.position.x - target.x;
		iTween.MoveTo(Counter.gameObject, target, time);
		
		/*
        Hashtable ht = new Hashtable(){
            {iT.MoveBy.y, xMovement},
            {iT.MoveBy.time, time},
			{iT.MoveBy.oncomplete, "Flip"}
        };
		 
        iTween.MoveBy(Arrow.gameObject, ht);
		*/
		
		//iTween.MoveBy(Arrow.gameObject, new Vector3(0, xMovement, 0), time);
	}                                   
	
	public void TweenDrawerBump(bool state)
	{
		if (maxSteps <= 4)
			return;
		float xDrawerMovement = xOffsetPerStepCard*.5f;
		if (state)
		{
			if (!drawerBumped)
			{
				Vector3 localDestination = homeCounterPosition + new Vector3(xDrawerMovement / 2, 0, 0);
				TweenDrawerMovement(localDestination, drawerAnimationSpeedInSeconds / 2);
			}
		}
		else
		{
			if (drawerBumped)
				TweenDrawerMovement(homeCounterPosition, drawerAnimationSpeedInSeconds / 2);
		}
		drawerBumped = state;
		
	}
	
	
}
