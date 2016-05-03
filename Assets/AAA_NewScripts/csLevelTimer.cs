using UnityEngine;
using System.Collections;

public class csLevelTimer : MonoBehaviour {

	private float timeRemaining = 30.0f;
	private TextMesh displayText;
	private SpriteRenderer bgSprite;

	private bool timerTicking = false;

	void Awake () {
		displayText = transform.FindChild ("TimerDisplay").GetComponent<TextMesh> ();
		bgSprite = transform.FindChild ("TimerBG").GetComponent<SpriteRenderer> ();

		// Adjust sort layer and position of text mesh.  Unity does not expose this in the
		// editor except for sprites.  Should eventually be unnecessary.
		displayText.GetComponent<Renderer> ().sortingLayerName = "ForegroundUI";
		displayText.GetComponent<Renderer> ().sortingOrder = 2;
	}

	void Update () {
		if (timerTicking) {
			timeRemaining = Mathf.Max (0.0f, timeRemaining - Time.deltaTime);
			updateTimerDisplay ();
		}

	}

	public void setTimer (float timeInSeconds) {
		timeRemaining = timeInSeconds;
		updateTimerDisplay ();
	}

	public void startTimer () {
		timerTicking = true;
	}

	public void stopTimer () {
		timerTicking = false;
	}

	private void updateTimerDisplay () {

		int totalSeconds = (int)Mathf.Ceil (timeRemaining);
		int seconds = totalSeconds % 60;
		int minutes = totalSeconds / 60;

		string output = string.Format ("{0:0}:{1:00}", minutes, seconds);

		if (timeRemaining <= 8) {
			output = "<color=#FF0000>" + output + "</color>";
			bgSprite.color = new Color (1,0,0);
		} else if (timeRemaining <= 15) {
			output = "<color=#FFFF00>" + output + "</color>";
			bgSprite.color = new Color (1,1,0);
		}

		displayText.text = output;

	}
	
}
