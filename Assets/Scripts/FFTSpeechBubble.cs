using UnityEngine;
using System.Collections;

public class FFTSpeechBubble : MonoBehaviour {

    public string DisplayText = "";

    public int CharacterBreakCount = 28;
    public float LineStartY = 5;
    public float LineSpacing = -17;
    public int LeftEdgeX = -145;

	// Use this for initialization
	void Start () {
        UpdateText();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void UpdateText()
    {
        GameObject LineText = transform.FindChild("LineText1").gameObject;
        GameObject LineText2 = transform.FindChild("LineText2").gameObject;
        if (LineText != null)
        {
            if (DisplayText.Length < CharacterBreakCount)
            {
                LineText.GetComponent<TextMesh>().text = DisplayText;
                LineText2.GetComponent<TextMesh>().text = "";
            }
            else
            {
                int CharacterIndex = CharacterBreakCount;
                while ((DisplayText[CharacterIndex] != ' ') || CharacterIndex < 0)
                {
                    CharacterIndex--;
                    //Debug.Log(CharacterIndex);
                }
                string DisplayText2 = DisplayText.Substring(CharacterIndex + 1);
                DisplayText2.Trim();
                string DisplayText1 = DisplayText.Substring(0, CharacterIndex);
                DisplayText1.Trim();
                LineText.GetComponent<TextMesh>().text = DisplayText1;
                LineText2.GetComponent<TextMesh>().text = DisplayText2;
            }
        }
    }
}
