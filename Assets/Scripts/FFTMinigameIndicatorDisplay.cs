using UnityEngine;
using System.Collections;

public class FFTMinigameIndicatorDisplay : MonoBehaviour {

    GameObject display;
    public FFTMinigameIcon Icon;

	// Use this for initialization
	void Awake () {
        display = GameObject.Instantiate(Resources.Load("MainGamePrefabs/MinigameIndicator")) as GameObject;
        display.transform.parent = gameObject.transform;
        display.transform.localPosition = new Vector3(17.1f, -26, -3);
        display.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetupIcon(MG_Minigame.Type minigameType)
    {
        if (display != null)
        {
            Icon = display.GetComponent<FFTMinigameIcon>();
            if (Icon != null)
                Icon.State = minigameType;
            else
                Debug.Log("MG Indicator Icon not properly attached.");
        }
        else
            Debug.Log("MG Indicator Display not initialized.");
    }

    void OnDestroy()
    {
        FFTUtilities.DestroySafe(display);
    }
}
