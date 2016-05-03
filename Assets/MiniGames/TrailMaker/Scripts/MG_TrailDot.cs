using UnityEngine;
using System.Collections;

public class MG_TrailDot : MonoBehaviour {
	
	public int DotNumber;
	public string DotText;
	public MG_TrailGame Root;
	//private IRageSpline dotSpline;
	public GameObject liquid;
	private bool alreadyHit;
	private TextMesh dotTextMesh;
	// Use this for initialization
	void Start () {
		//dotSpline = this.GetComponent(typeof(RageSpline)) as IRageSpline;
		liquid.transform.localScale = new Vector3(.01f, .01f, 1f);
		alreadyHit = false;
		DotText = DotNumber.ToString();
		dotTextMesh = this.GetComponentInChildren<TextMesh>();
		dotTextMesh.text = DotText;
		this.name = "Dot" + DotText;
		gameObject.transform.localScale = new Vector3(Root.ScaleFactor,Root.ScaleFactor,1f);
		this.GetComponent<SphereCollider>().radius = this.GetComponent<SphereCollider>().radius*(Root.ScaleFactor+(1-Root.ScaleFactor)/2f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseEnter(){
		if(!alreadyHit){
			Debug.Log(DotNumber + "hit");
			if(DotNumber == Root.PointInSequence){
				if(Root.Mode == MG_TrailGame.GameType.Continuous){
					Root.Sequence[DotNumber-1] = false;
					Root.LogDot();
					alreadyHit = true;
					iTween.ScaleBy(liquid,new Vector3(100, 100, 1), 1.7f);
					Debug.Log("Hit");
				}
//					dotSpline.SetFillColor1(Color.green);
			}
			else{
				Root.NumberOfMistakes++;
//				dotSpline.SetFillColor1(Color.red);
			}
//			dotSpline.RefreshMesh();
			Root.CheckSequence();

		}
	}
	
	void OnMouseDown(){
		if(Root.Mode == MG_TrailGame.GameType.Clicking){
			if(!alreadyHit){
				Debug.Log("PIS: " + Root.PointInSequence);
				Debug.Log("DN: " + DotNumber);
				if(DotNumber == Root.PointInSequence){
					Root.Sequence[DotNumber-1] = false;
					Root.LogDot();
					alreadyHit = true;
				}
				else{
					Root.Sequence[DotNumber-1] = false;
					alreadyHit = true;
					Root.FalseClicks ++;
				}
				iTween.ScaleBy(liquid,new Vector3(100, 100, 1), 1.7f);
				Root.CheckSequence();
			}
		}
	}
	
	void OnMouseExit(){
		if(!alreadyHit){
			//dotSpline.SetFillColor1(Color.white);
		}
		//dotSpline.RefreshMesh();
	}
}
