using UnityEngine;
using System.Collections;

public class MG2_InstructionsTextScript : MonoBehaviour {

	public MG2_RootScript roots;
	public GameObject guide = null;
	Vector3 oldPos;
	TextMesh instructionsText;
	public TextMesh TotalCutsText;
	
	public float guidePaddingX = 17f;
	public float guideAdjustY = 14f;
	
	MG2_GameMode prevMode = MG2_GameMode.none;
	
	// Use this for initialization
	void Start () {
		instructionsText = this.GetComponentInChildren(typeof(TextMesh)) as TextMesh;
		oldPos = new Vector3(0f,0f,0f);
		
	}
	
	// Update is called once per frame
	void Update () {
		if(guide == null){
			guide = roots.transform.FindChild("MG2 Guide(Clone)").gameObject; //GameObject.Find("MG2 Guide(Clone)");
			if(guide == null){
				Debug.Log("object not found");
				return;
			}
		}
		if (prevMode != roots.Mode)
		{
			if(roots.Mode == MG2_GameMode.proportionality){
				if (TotalCutsText == null) //accounts for old prefabs which do not have a separate text display for this number
				{
					instructionsText.text = roots.Instructions.ToString();  //sets the instructions text to what is designated in the rootscript.	
				}
				else
				{
					instructionsText.text = roots.Instructions_Prefix + roots.Instructions_Padding + roots.Instructions_Suffix;
					TotalCutsText.text = (roots.MG2_MaxCuts + 1).ToString();
				}
				if(oldPos != transform.position + new Vector3(instructionsText.GetComponent<Renderer>().bounds.extents.x, -instructionsText.GetComponent<Renderer>().bounds.size.y * 1.5f, 0f)){ //if the instruction text is in a different place (or in the case of at start, at the start)
					Debug.Log("position changed");
					guide.transform.position = transform.position + new Vector3(2*instructionsText.GetComponent<Renderer>().bounds.extents.x + guidePaddingX, -instructionsText.GetComponent<Renderer>().bounds.size.y * 1.3f + guideAdjustY, 0f); //move the guide to be at the end of the text
					oldPos = transform.position + new Vector3(instructionsText.GetComponent<Renderer>().bounds.extents.x, -instructionsText.GetComponent<Renderer>().bounds.size.y * 1.5f, 0f);
				}
			}
			else if(roots.Mode == MG2_GameMode.cuts){
				if (TotalCutsText == null)
				{
					instructionsText.text = roots.Instructions.ToString(); //sets the instructions text to what is designated in the rootscript.
				}
				else
				{
					instructionsText.text = roots.Instructions_Prefix + roots.Instructions_Padding + roots.Instructions_Suffix;
					TotalCutsText.text = (roots.MG2_MaxCuts + 1).ToString();
				}
				guide.SetActiveRecursively(false); //hide the guide, it's not necessary in this gametype.
			}
		}
		
		prevMode = roots.Mode;
	}
}
