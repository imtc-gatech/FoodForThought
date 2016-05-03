using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MeterDisplayRefresh : MonoBehaviour {

    public MeterDisplay display;
    public bool refresh = false;
    public bool reset = false;
    public bool visibility = true;
    private bool visibilityToggle;

	// Use this for initialization
	void Awake () {
        display = GetComponent<MeterDisplay>();
        visibilityToggle = visibility;
        
	}
	
	// Update is called once per frame
	void Update () {
        if (refresh)
        {
            display.RefreshDisplay();
            //refresh = false;
        }
        if (reset)
        {
            display.ResetDisplay();
            reset = false;
        }
        if (visibility != visibilityToggle)
        {
            ToggleVisibility(gameObject.transform, visibility);
            visibilityToggle = visibility;
        }
	
	}

    private void ToggleVisibility(Transform obj, bool state)
    {
        for (int i = 0; i < obj.GetChildCount(); i++)
        {
            if (obj.GetChild(i).GetComponent<Renderer>() != null)
                obj.GetChild(i).GetComponent<Renderer>().enabled = state;

            if (obj.GetChild(i).GetChildCount() > 0)
            {
                ToggleVisibility(obj.GetChild(i), state);
            }
        }
    }
}
