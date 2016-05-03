using UnityEngine;
using System.Collections;

public class MGStirStirringTool : MonoBehaviour {

    GameObject stirringTool, stirringToolHead;
	public MGStirGame Root;
	public bool IsHeld = false;
	
    // Use this for initialization
    void Awake()
    {
        stirringTool = transform.parent.FindChild("spoonPerspective").gameObject; //StirringToolPerspective
        stirringToolHead = transform.parent.FindChild("spoonHead").gameObject; //StirringToolHead
		Root = transform.parent.parent.GetComponent<MGStirGame>();
    }

    // Update is called once per frame
    void Update()
    {
		if (Root.SpoonHasBounds)
			UpdateToolPosition();
    }

    void OnMouseDown()
    {
		if (!Root.SpoonHasBounds)
			GrabTool();
    }

    void OnMouseDrag()
    {
		if (!Root.SpoonHasBounds)
			UpdateToolPosition();
    }

    void OnMouseUp()
    {
		if (!Root.SpoonHasBounds)
			ReleaseTool();
    }
	
	public void GrabTool()
	{
		if(Root.CurrentState == MG_Minigame.State.Active && !IsHeld){
			//print("spoon grabbed");
			IsHeld = true;
	        transform.Translate(0, 0, 200);
    	    stirringToolHead.transform.Translate(0, 0, -150);
        	stirringTool.transform.Translate(0, 0, -200);
		}
	}
	
	public void ReleaseTool()
	{
		if(Root.CurrentState == MG_Minigame.State.Active && IsHeld){
			IsHeld = false;
	        stirringToolHead.transform.Translate(0, 0, 150);
	        transform.Translate(0, 0, -200);
	        stirringTool.transform.Translate(0, 0, 200);
		}
	}
	
	public void UpdateToolPosition()
	{
		if(Root.CurrentState == MG_Minigame.State.Active && IsHeld){
			Vector3 mouse = Root.mainMinigameCamera.ScreenToWorldPoint(Input.mousePosition);
	        //Vector3 mouse = GameObject.Find("/SecondCamera").camera.ScreenToWorldPoint(Input.mousePosition);
	        Vector3 point = new Vector3(mouse.x, mouse.y, mouse.z); //start from original location
	        point.z = transform.position.z; //keep z constant
	        transform.position = point; //move with mouse drag
	
	        point.z = stirringTool.transform.position.z; //keep z constant
	        stirringTool.transform.position = point; //the perspective stirring tool matches the position of the at rest stirring tool
	
	        point.x = (float)(point.x - 56.589); //offset the head from the center of the stirring tool to the head location
	        point.y = (float)(point.y - 22.55);
	        point.z = stirringToolHead.transform.position.z;
	        stirringToolHead.transform.position = point; //the head matches the position of the perspective stirring tool + the offset
		}
	}
}
