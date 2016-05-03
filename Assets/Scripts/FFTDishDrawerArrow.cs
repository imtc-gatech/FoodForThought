using UnityEngine;
using System.Collections;

public class FFTDishDrawerArrow : MonoBehaviour {
	
	public FFTDishDrawer Drawer;
	public bool ArrowVisible = true;
	Vector3 originalLocalScale;
	Vector3 originalLocalPosition;

	// Use this for initialization
	void Start () {
		originalLocalScale = gameObject.transform.localScale;
		originalLocalPosition = gameObject.transform.localPosition;
		Drawer = transform.parent.GetComponent<FFTDishDrawer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseEnter()
    {
		
		if (!Drawer.MoveDrawer)
			Drawer.TweenDrawerBump(true);
		
		gameObject.transform.localScale = originalLocalScale * 1.2f;
    }

    void OnMouseExit()
	{
		
		if (!Drawer.MoveDrawer)
			Drawer.TweenDrawerBump(false);
		
        gameObject.transform.localScale = originalLocalScale;
    }

    void OnMouseDown()
    {
		Drawer.MoveDrawer = !Drawer.MoveDrawer;
    }
	
	public void Hide()
	{
		if (ArrowVisible)
			gameObject.transform.localPosition = originalLocalPosition + new Vector3(0, 0, 1000);
		ArrowVisible = false;
	}
	
	public void Show()
	{
		if (!ArrowVisible)
			transform.localPosition = originalLocalPosition;
		ArrowVisible = true;
	}
	
	public void Flip()
	{
		Vector3 rot = gameObject.transform.eulerAngles;
		rot.y += 180;
		gameObject.transform.eulerAngles = rot;
	}
	
	public void Dismiss()
	{
		if (!ArrowVisible)
			return;
		Hashtable ht = new Hashtable(){
            {iT.MoveTo.x, 200},
            {iT.MoveTo.time, 0.4f},
			{iT.MoveTo.easetype, iTween.EaseType.easeInExpo}
        };
		 
        iTween.MoveTo(gameObject, ht);
		
	}
		
}
