using UnityEngine;
using System.Collections;

public class DishManager : MonoBehaviour {

    /*
        RGB Values for Stations:
        Pink = (252,240,245)    [Spice]
        Green = (185,219,167)   [Prep]
        Red = (224,90,89)       [Cook]
        Blue = (180,226,241)    [Chop]
    */

    /*
    public static Color ColorChop = new Color(180/255f, 226/255f, 241/255f); //Blue;
    public static Color ColorCook = new Color(224 / 255f, 90 / 255f, 89 / 255f); //Red
    public static Color ColorPrep = new Color(185 / 255f, 219 / 255f, 167 / 255f); //Green
    public static Color ColorSpice = new Color(252 / 255f, 240 / 255f, 245 / 255f); //Pink
    */
        
	
    /*
	private static DishManager instance = null;
    public static DishManager Instance { 
        get {
            if (instance == null)
            {
                //Debug.Log("GameManager instantiate");
				GameObject go = GameObject.Find("Counter");
				go.AddComponent<DishManager>();
                //go.name = "!GameManager";
            }

            return instance; 
        } 
    }
    */
	
	public bool dishSelected;

	// Use this for initialization
	void Awake () {
		dishSelected = false;
        //ColorChop = new Color(180/255f, 226/255f, 241/255f); //Blue
        //ColorCook = new Color(224/255f, 90/255f, 89/255f); //Red
        //ColorPrep = new Color(185/255f, 219/255f, 167/255f); //Green
        //ColorSpice = new Color(252/255f, 240/255f, 245/255f); //Pink

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
