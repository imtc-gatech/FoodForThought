using UnityEngine;
using System.Collections;

public class MG_BackButton : MonoBehaviour {
	
	public bool Activated
	{
		get { return _activated; }
		set
		{
			if (value != _activated)
			{
				_activated = value;
				if (buttonUI != null)
					buttonUI.FeedbackEnabled = _activated;
				RemoveFromView(_activated);
			}
		}
	}
	[SerializeField]
	private bool _activated = true;
	
	public MG_SceneController SceneController;
	private FFTUIButton buttonUI;
	void Start(){
        SceneController = FFTGameManager.Instance.MinigameManager; //GameObject.Find(MG_SceneController.SceneControllerName).GetComponent<MG_SceneController>();
		buttonUI = GetComponent<FFTUIButton>();
	}
	
	void OnMouseUp(){
		if (Activated)
        	SceneController.CloseCurrentGame();
	}
	
	void RemoveFromView(bool state)
	{
		Vector3 offset = new Vector3(100, 100, 0); //(to--  ~x:242/155, y:175/78
		if (state)
			offset *= -1;
		Vector3 dest = transform.position + offset;
		
		iTween.MoveTo(gameObject,new Hashtable(){{iT.MoveTo.x,dest.x},{iT.MoveTo.y,dest.y},
												 {iT.MoveTo.time,0.7f},{iT.MoveTo.easetype,iTween.EaseType.easeInQuint}});
		//iTween.MoveTo(gameObject, dest, 0.7f);
	}
	
}
