using UnityEngine;
using System.Collections;

public class MG2_FeedbackTextScript : MonoBehaviour {

	public MG2_RootScript roots;
	TextMesh feedbackText;
	// Use this for initialization
	void Start () {
		feedbackText = this.GetComponentInChildren(typeof(TextMesh)) as TextMesh;		
	}
	
	// Update is called once per frame
	void Update () {
		if(roots.score.feedback != null){
			if(!roots.getDone()){
				feedbackText.text = roots.score.feedback;
			}
			else{
				this.transform.position = roots.transform.position + new Vector3(-80, 70f,-2f);
				this.transform.localScale = new Vector3(.75f,.75f,.75f);
				feedbackText.text = roots.score.feedback;
			}
		}
	}
}
