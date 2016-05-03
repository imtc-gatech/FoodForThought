using UnityEngine;
using System.Collections;

public class FFTDishCovered : MonoBehaviour {

    public static Vector3 Scale = new Vector3(0.33f, 0.33f, 0.33f);
    public static Vector3 CheckPos = new Vector3(14, -13, 0);

    GameObject DishRoot;
    GameObject CheckMark;

	// Use this for initialization
	void Awake () {
        gameObject.transform.position += new Vector3(0, 0, -10f);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        DishRoot = Instantiate(Resources.Load("UIPrefabs/coveredDish")) as GameObject;
        DishRoot.transform.localScale = Scale;
        DishRoot.transform.parent = transform;
        Vector3 coverPos = transform.position;
        DishRoot.transform.position = coverPos;
        //iTween.MoveFrom(DishRoot, iTween.Hash(iT.MoveFrom.y, -10, iT.MoveFrom.time, 5, iT.MoveFrom.oncomplete, "SwitchOnCheckMark"));
        CheckMark = Instantiate(Resources.Load("UIPrefabs/UI/checkButton")) as GameObject;
        CheckMark.transform.localScale = Scale;
        CheckMark.transform.parent = transform;
        CheckMark.transform.localPosition = CheckPos;
        CheckMark.SetActiveRecursively(false);
	}

    void Start()
    {
        SwitchOnCheckMark();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void SwitchOnCheckMark()
    {
        CheckMark.SetActiveRecursively(true);
    }
}
