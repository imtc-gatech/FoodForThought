using UnityEngine;
using System.Collections;

public class Selector_Game1Text : MonoBehaviour {
	public MG_SceneController selector;
	public TextMesh mg1Text;
	// Use this for initialization
	void Start () {
		mg1Text = this.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
	}
	
	// Update is called once per frame
	void Update () {
        /*
		if(selector.miniGame1 != null){
			mg1Text.text = ("A: " + selector.miniGame1.GetComponent<MG_Minigame>().Title);
		}
		else{
			mg1Text.text = selector.mg1Text;
		}
         */
	}
}
