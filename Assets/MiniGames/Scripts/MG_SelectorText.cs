using UnityEngine;
using System.Collections;

public class MG_SelectorText : MonoBehaviour {
    public int Index = 0;
	public MG_SceneController SceneController;
	public TextMesh DisplayText;

	// Use this for initialization
	void Start () {
		DisplayText = this.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateText()
    {
        /*
        if (SceneController.miniGame4 != null)
        {
            DisplayText.text = ("S: " + SceneController.miniGame4.GetComponent<MG_Minigame>().Title);
        }
        else
        {
            DisplayText.text = SceneController.mg4Text;
        }
         */
    }
}
