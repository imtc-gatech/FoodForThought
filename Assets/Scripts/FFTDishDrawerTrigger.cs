using UnityEngine;
using System.Collections;

public class FFTDishDrawerTrigger : MonoBehaviour {
	
	BoxCollider drawerTrigger;
	public FFTDishDrawer Drawer;

	// Use this for initialization
	void Start () {
		
		FFTCounter Counter = FFTGameManager.Instance.Counter;
		
		drawerTrigger = Counter.gameObject.AddComponent<BoxCollider>();
		drawerTrigger.size = new Vector3(525, 200, 100);
		drawerTrigger.center = new Vector3(150, 0, 0);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {
		FFTUtilities.DestroySafe(drawerTrigger);
	}
	
	void OnMouseOver() {
		if (!Drawer.MoveDrawer)
			Drawer.TweenDrawerBump(true);
	}
	
	void OnMouseExit() {
		if (!Drawer.MoveDrawer)
			Drawer.TweenDrawerBump(false);
	}
	
	void OnMouseDown() {
		Drawer.MoveDrawer = !Drawer.MoveDrawer;
	}
		
}
