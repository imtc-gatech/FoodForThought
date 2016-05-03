using UnityEngine;
using System.Collections;

public class FFTStationOcclusionView : MonoBehaviour {
	
	public enum OcclusionState
	{
		HideNone,
		HideTimer,
		HideRecipeCard,
		HideAll //including Counter
	}
	
	OcclusionState OcclusionStateLayer = OcclusionState.HideRecipeCard;
	
	public static float Z_HIDE_NONE = 5;
	public static float Z_HIDE_GLOBAL_TIMER = -12;
	public static float Z_HIDE_RECIPE_CARD = -36;
	public static float Z_HIDE_COUNTER_ALL = -111;
	
	/*
	public static float Z_INTROCARD_HIDE = 205;
	public static float Z_INTROCARD_SHOW = 291;
	*/
	
	public bool IsVisible = true;
	
	IRageSpline shadowSpline;
	Color outline1, fill1, outline2, fill2;
	float maxAlpha = 176 / 255f;
	float shadowTransitionInSeconds = 2.5f;

	// Use this for initialization
	void Start () {
		
		gameObject.transform.position = new Vector3(161.5f, 0, zDepthForShadow());
		shadowSpline = gameObject.GetComponent<RageSpline>() as IRageSpline;
		outline1 = shadowSpline.GetOutlineColor1();
		outline2 = shadowSpline.GetOutlineColor2();
		fill1 = shadowSpline.GetFillColor1();
		fill2 = shadowSpline.GetFillColor2();
		
		FadeOut(0); //fade out our shadow until the drawer key is pressed
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public static FFTStationOcclusionView NewShadow()
	{
		GameObject shadowGO = GameObject.Instantiate(Resources.Load("MainGamePrefabs/StationGameplayOcclusionShadow", typeof(GameObject)) as GameObject) as GameObject;	
		shadowGO.transform.parent = Camera.main.gameObject.transform;  //FFTGameManager.Instance.gameObject.transform;
		shadowGO.name = "StationGameplayOcclusionShadow";
		return shadowGO.AddComponent<FFTStationOcclusionView>();
	}
	
	/// <summary>
	/// Fades in shadow. Does not currently work. :-(
	/// </summary>
	/// <param name='time'>
	/// Time.
	/// </param>
	public void FadeIn(float time)
	{	

		iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", maxAlpha, "time", time, "onupdate", "FadeSpline", "delay", 0.1f));
		IsVisible = true;
	}
	/// <summary>
	/// Fades out shadow. Does not currently work. :-(
	/// </summary>
	/// <param name='time'>
	/// Time.
	/// </param>
	public void FadeOut(float time)
	{	
		iTween.ValueTo(gameObject, iTween.Hash("from", maxAlpha, "to", 0, "time", time, "onupdate", "FadeSpline", "delay", 0.1f));
		IsVisible = false;
	}
	
	void FadeSpline(float val)
    {
		outline1.a = val;
		outline2.a = val;
        fill1.a = val;
        fill2.a = val;
        shadowSpline.SetOutlineColor1(outline1);
		shadowSpline.SetOutlineColor2(outline2);
        shadowSpline.SetFillColor1(fill1);
		shadowSpline.SetFillColor2(fill2);
        shadowSpline.RefreshMesh(false, false, false);
    }
	
	float zDepthForShadow()
	{
		//original value for hiding recipeCard = -98.5f
		
		switch (OcclusionStateLayer)
		{
		case OcclusionState.HideNone:
			return Z_HIDE_NONE;
		case OcclusionState.HideTimer:
			return Z_HIDE_GLOBAL_TIMER;
		case OcclusionState.HideRecipeCard:
			return Z_HIDE_RECIPE_CARD;
		case OcclusionState.HideAll:
			return Z_HIDE_COUNTER_ALL;
		default:
			return Z_HIDE_RECIPE_CARD;
		}
	}
	
	public void SwitchLayer()
	{
		FFTGameManager.GameState state = FFTGameManager.Instance.State;
		
		if (state >= FFTGameManager.GameState.GameplayFinished)
		{
			OcclusionStateLayer = OcclusionState.HideAll;
		}
		else if (state >= FFTGameManager.GameState.Gameplay)
		{
			OcclusionStateLayer = OcclusionState.HideAll; //HideTimer;
		}
		else
		{
			OcclusionStateLayer = OcclusionState.HideRecipeCard;
		}
		
		Vector3 position = gameObject.transform.position;
		position.z = zDepthForShadow();
		gameObject.transform.position = position;
		
	}
}
