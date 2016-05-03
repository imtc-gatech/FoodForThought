using UnityEngine;
using System.Collections;

public class StationGO : MonoBehaviour {

    public Station.Type id;
    public int SlotCount;
    private RageSpline stationSpline;
    private Component[] slots;

	// Use this for initialization
	void Start () {
        gameObject.name = id.ToString();
        stationSpline = gameObject.GetComponent<RageSpline>();
        SetBackGroundColor();
        slots = GetComponentsInChildren(typeof(Slot));
        SlotCount = slots.Length;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void SetBackGroundColor()
    {
        switch (id)
        {
            case Station.Type.Chop:
                stationSpline.SetFillColor1(GameManager.Instance.ColorChop);
                break;
            case Station.Type.Cook:
                stationSpline.SetFillColor1(GameManager.Instance.ColorCook);
                break;
            case Station.Type.Prep:
                stationSpline.SetFillColor1(GameManager.Instance.ColorPrep);
                break;
            case Station.Type.Spice:
                stationSpline.SetFillColor1(GameManager.Instance.ColorSpice);
                break;
            default:
                Debug.Log("Not Found!");
                break;
        }

        stationSpline.RefreshMesh(true, false, false);
    }
}
