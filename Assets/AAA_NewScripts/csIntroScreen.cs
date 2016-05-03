using UnityEngine;
using System.Collections;

public class csIntroScreen : MonoBehaviour {

	private float INTRO_SLIDE_TIME = 1.0f;
	private int UI_TEXT_SORT_ORDER = 1;

	private csLevelManager levelManager;
	private csTweenManager tweenManager;

	private SpriteRenderer fgSprite;
	private TextMesh titleText;
	private TextMesh descText;
	private SpriteRenderer customerSprite;

	void Awake () {
		levelManager = Camera.main.GetComponent<csLevelManager> ();
		tweenManager = Camera.main.GetComponent<csTweenManager> ();
		fgSprite = transform.FindChild ("IntroFG").GetComponent<SpriteRenderer> ();
		titleText = fgSprite.transform.FindChild ("Title").GetComponent<TextMesh> ();
		descText = fgSprite.transform.FindChild ("Description").GetComponent<TextMesh> ();
		customerSprite = fgSprite.transform.FindChild ("Customer").GetComponent<SpriteRenderer> ();
	}


	public void PopulateIntroScreen(string title, string desc, csCustomer.CustomerName customer) {
		titleText.text = title;
		descText.text = desc;
		customerSprite.sprite = levelManager.spriteManager.sprites [customer.ToString() + "-0"];	// -0 is default pose

		// Change sort order here, because Unity currently does not display sort order in editor except for Sprites.
		// This code should ultimately not be needed.
		titleText.GetComponent<Renderer> ().sortingLayerName = "ForegroundUI";
		titleText.GetComponent<Renderer> ().sortingOrder = UI_TEXT_SORT_ORDER;

		descText.GetComponent<Renderer> ().sortingLayerName = "ForegroundUI";
		descText.GetComponent<Renderer> ().sortingOrder = UI_TEXT_SORT_ORDER;
	}

	public void ShowIntroScreen () {
		// Initial position is already on screen.  Mainly need to populate information.

	}

	public void HideIntroScreen () {
		StartCoroutine (tweenManager.PositionTweenTo(transform, new Vector3(-1 * Screen.width, transform.position.y, transform.position.z), INTRO_SLIDE_TIME));

		// If an "on finish" event is desired, we could yield the start and become a coroutine ourselves, then follow up here.
	}

}
