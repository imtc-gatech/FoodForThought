using UnityEngine;
using System.Collections;

public class DIS_SmokeView : MonoBehaviour {
	
	public GameObject Smoke1;
	public GameObject Smoke2;
	public GameObject Smoke3;
	public GameObject Smoke4;
    public iTween.EaseType Ease = iTween.EaseType.easeInOutQuad;

	public Vector3 Path1;
	public Vector3 Path2;
	public Vector3 Path3;
	public Vector3 Path4;
	public float moveTime = 1.7f;
	// Use this for initialization
	void Start () {
		Path1 = Smoke1.transform.localPosition;
		Path2 = Smoke2.transform.localPosition;
		Path3 = Smoke3.transform.localPosition;
		Path4 = Smoke4.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if (!FFTTimeManager.Instance.GameplayPaused)
		{
			MoveSmoke(Smoke1);
			MoveSmoke(Smoke2);
			MoveSmoke(Smoke3);
			MoveSmoke(Smoke4);
		}
	}
	
	void MoveSmoke(GameObject affectedSmoke){
		Vector3 pos = affectedSmoke.transform.localPosition;
		Hashtable hash = new Hashtable();
		hash.Add("EaseType", Ease);
		hash.Add("time", moveTime);
		hash.Add("islocal" , true);
		if(pos == Path1){
			hash.Add("position", Path2);
		}
		else if(pos == Path2){
			hash.Add("position", Path3);
		}
		else if(pos == Path3){
			hash.Add("position", Path4);
		}
		else if(pos == Path4){
			hash.Add("position", Path1);
		}
		else{
			return;
		}
		iTween.MoveTo(affectedSmoke, hash);
	}
}
