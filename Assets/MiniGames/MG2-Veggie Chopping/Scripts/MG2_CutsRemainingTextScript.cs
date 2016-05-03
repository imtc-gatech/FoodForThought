using UnityEngine;
using System.Collections;

public class MG2_CutsRemainingTextScript : MonoBehaviour {
	
	public MG2_RootScript Roots;
	TextMesh cutsRemainingText;
	public TextMesh CutsRemainingNumberText;
	int lastCuts = 0;
	
	// Use this for initialization
	void Start () {
		cutsRemainingText = this.GetComponentInChildren(typeof(TextMesh)) as TextMesh;		
	}
	
	// Update is called once per frame
	void Update () {
		//int remaining = Roots.MG2_MaxCuts - Roots.count;
		if (lastCuts != Roots.CutsRemaining)
		{
			if (CutsRemainingNumberText == null) //accounts for old prefabs which do not have a separate text display for this number
				cutsRemainingText.text = "Cuts Remaining: " + Roots.CutsRemaining.ToString(); // remaining.ToString();
			else
			{
				cutsRemainingText.text = "Cuts Remaining: ";
				CutsRemainingNumberText.text = Roots.CutsRemaining.ToString(); 
			}
		}
		lastCuts = Roots.CutsRemaining;
	}
}
