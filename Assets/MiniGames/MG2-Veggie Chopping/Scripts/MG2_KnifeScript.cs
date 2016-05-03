using UnityEngine;
using System.Collections;

public class MG2_KnifeScript : MonoBehaviour {
	public Vector3 Oldpos;
	public bool KnifeActive;
	
	private IRageSpline knifeSpline;
	private bool chopping;
	private int frame;
	MG2_RootScript root;
	
	public bool IsAnimating { get { return chopping; } }
	
	public bool KnifeInHand
	{
		get { return _knifeInHand; }
		set
		{
			if (value != _knifeInHand)
			{
				_knifeInHand = value;
				if (_knifeInHand)
					knifeHandObject.SetActiveRecursively(true);
				else
					knifeHandObject.SetActiveRecursively(false);
			}
		}
	}
	[SerializeField]
	private bool _knifeInHand = true;
	
	private Camera mainMinigameCamera;
	private GameObject knifeHandObject;
	private GameObject guideClone;
	
	// Use this for initialization
	void Awake () {
		frame = 0;
		chopping = false;
		KnifeActive = false;
		root = FindObjectOfType(typeof(MG2_RootScript)) as MG2_RootScript;
		knifeHandObject = transform.FindChild("hand_knife").gameObject;
	}
	
	void Start() {
		mainMinigameCamera = root.MinigameHolder.GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if(KnifeActive){
			
			if(chopping){
				ChopAnimation();
			}
			
			else{
				Oldpos = mainMinigameCamera.ScreenToWorldPoint(Input.mousePosition);
				transform.position = Oldpos + new Vector3(16f,2f,7f);
			}
		}	
	}
	
	public void ChopAnimation(){
		Oldpos = transform.position;
		if(frame < 10){
			Oldpos.y -= 2f;
		}
		else if(frame < 20){
			Oldpos.y += 2f;
		}
		else{
			frame = -1;
			chopping = false;
			if (root.CutsRemaining == 0)
			{
				root.CuttingFinished();
			}
		}
		transform.position = new Vector3((mainMinigameCamera.ScreenToWorldPoint(Input.mousePosition).x + 16f), Oldpos.y, transform.position.z);
		
		frame ++;
	}
	
	public void Chopping(){
		chopping = true;
	}
	
	public void SetGuideClone(GameObject go)
	{
		guideClone = go;
	}
	
	public bool HasGuideClone()
	{
		return (guideClone != null);
	}
	
	public void SwitchGuide(bool state)
	{
		if (HasGuideClone())
			guideClone.SetActiveRecursively(state);
	}
		
}
