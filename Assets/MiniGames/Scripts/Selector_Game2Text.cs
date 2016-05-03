using UnityEngine;
using System.Collections;

public class Selector_Game2Text : MonoBehaviour {
	public MG_SceneController selector;
	public TextMesh mg2Text;
	// Use this for initialization
	void Start () {
		mg2Text = this.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
	}
	
	// Update is called once per frame
	void Update () {
        /*
		if(selector.miniGame2 != null){
			mg2Text.text = ("D: " + selector.miniGame2.GetComponent<MG_Minigame>().Title);
		}
		else{
			mg2Text.text = selector.mg2Text;
		}
         */
	}
}
