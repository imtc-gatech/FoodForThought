using UnityEngine;
using System.Collections;

public class Selector_Game3Text : MonoBehaviour {
	public MG_SceneController selector;
	public TextMesh mg3Text;
	// Use this for initialization
	void Start () {
		mg3Text = this.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
	}
	
	// Update is called once per frame
	void Update () {
        /*
		if(selector.miniGame3 != null){
			mg3Text.text = ("W: " + selector.miniGame3.GetComponent<MG_Minigame>().Title);
		}
		else{
			mg3Text.text = selector.mg3Text;
		}
         */
	}
}
