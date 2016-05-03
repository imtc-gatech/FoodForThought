using UnityEngine;
using System.Collections;

public class MGStirSpoonHandleCollider : MonoBehaviour {

    private Transform t;
	GameObject spoonAtRest;
    GameObject stirringTool, stirringToolHead;
    bool movedBack = false;
    Vector3 originalToolPosition;
	public MGStirGame Root;
    // Use this for initialization
    void Start()
    {
        t = transform.parent;
		spoonAtRest = transform.parent.FindChild("<Path0>").gameObject;
        stirringTool = transform.parent.parent.FindChild("spoonPerspective").gameObject;
        stirringToolHead = transform.parent.parent.FindChild("spoonHead").gameObject;
        originalToolPosition = new Vector3(t.position.x, t.position.y, t.position.z);
		Root = transform.parent.parent.parent.GetComponent<MGStirGame>();
    }

    // Update is called once per frame
    void Update()
    {
		if (Root.SpoonHasBounds && movedBack)
			UpdateSpoonPosition();
    }

    void OnMouseDown()
    {
        if (!Root.SpoonHasBounds)
			GrabSpoon();
    }

    void OnMouseDrag()
    {
		if (!Root.SpoonHasBounds)
			UpdateSpoonPosition();
    }

    void OnMouseUp()
    {
		if (!Root.SpoonHasBounds)
        	ReleaseSpoon();
    }
	
	public void GrabSpoon()
	{
		if (!movedBack)
        {
			spoonAtRest.SetActiveRecursively(false);
            t.Translate(0, 0, 200);
            stirringToolHead.transform.Translate(0, 0, -150);
            stirringTool.transform.Translate(0, 0, -200);
            movedBack = true;
        }
	}
	
	public void ReleaseSpoon()
	{
		if (movedBack)
        {
			spoonAtRest.SetActiveRecursively(true);
            stirringToolHead.transform.Translate(0, 0, 150);
            //t.position = new Vector3(85f, -2.5f, t.position.z-200);
            t.position = originalToolPosition;
            //t.Translate(0, 0, -200);
            stirringTool.transform.Translate(0, 0, 200);
            movedBack = false;
        }
	}
	
	public void UpdateSpoonPosition()
	{
		if(Root.CurrentState == MG_Minigame.State.Active){
	        Vector3 mouse = Root.mainMinigameCamera.ScreenToWorldPoint(Input.mousePosition);
	        Vector3 point = new Vector3(mouse.x, mouse.y, mouse.z); //start from original location
	        point.z = t.position.z; //keep z constant
	        t.position = point; //move with mouse drag
			
	        point.z = stirringTool.transform.position.z; //keep z constant
	        stirringTool.transform.position = point; //the perspective stirring tool matches the position of the at rest stirring tool
	
	        point.x = (float)(point.x - 65.14); //offset the head from the center of the stirring tool to the head location
	        point.y = (float)(point.y - 34.51);
	        point.z = stirringToolHead.transform.position.z;
	        stirringToolHead.transform.position = point; //the head matches the position of the perspective stirring tool + the offset
		}
	}
}
