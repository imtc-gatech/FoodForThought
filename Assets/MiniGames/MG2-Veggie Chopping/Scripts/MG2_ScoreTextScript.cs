using UnityEngine;
using System.Collections;

public class MG2_ScoreTextScript : MonoBehaviour {

	public MG2_RootScript roots;
	TextMesh scoreText;
	// Use this for initialization
	void Start () {
		scoreText = this.GetComponentInChildren(typeof(TextMesh)) as TextMesh;		
	}
	
	// Update is called once per frame
	void Update () {
		int presentation2 = Mathf.RoundToInt(roots.score.presentation);
		if(presentation2 > 0){
			scoreText.text = "Score: " + presentation2.ToString();
		}
	}
}
