using UnityEngine;
using System.Collections;

public class FFTResultsDetailDisplayBackButton : FFTScalableButton {

    public FFTResultsDetailDisplayView View;

    protected override void ClickReleaseAction()
    {
		//log that detail was closed:
		if (FFTGameManager.Instance.LogActions)
		{
			FFTStepAction currentAction = new FFTStepAction();
			currentAction.Reset();
			currentAction.AssignButtonAction(FFTStepAction.InteractionType.Selected, FFTStepAction.SlotActionType.Button_Results_CloseDetail);
			FFTGameManager.Instance.DataManager.AddAction(currentAction);
		}
        View.DestroyView(FFTScalableObject.scaleTime);
    }

}

/*
public class FFTResultsDetailDisplayBackButton : MonoBehaviour {

    public static float scaleTime = 0.05f;
    float originalScale = 0.7f;

    public FFTResultsDetailDisplayView View;

    public bool Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            if (value != _selected)
            {
                _selected = value;
                if (_selected)
                {
                    Up.SetActiveRecursively(false);
                    Down.SetActiveRecursively(true);
                }
                else
                {
                    Down.SetActiveRecursively(false);
                    Up.SetActiveRecursively(true);
                }
            }
        }
    }
    [SerializeField]
    private bool _selected;

    GameObject Up;
    GameObject Down;

	// Use this for initialization
	void Start () {
        originalScale = transform.localScale.x;
        Up = transform.FindChild("Up").gameObject;
        Down = transform.FindChild("Down").gameObject;
        Down.SetActiveRecursively(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter()
    {
        ScaleUp();
    }

    void OnMouseExit()
    {
        ScaleDown();
    }

    void OnMouseDown()
    {
        Selected = true;
    }

    void OnMouseUp()
    {
        //TODO: destroy the detail view screen here
        if (Selected)
        {
            View.DestroyView(scaleTime);
        }
    }

    void ScaleUp()
    {
        iTween.ScaleTo(gameObject, iTween.Hash("x", 1.2f*originalScale, "y", 1.2f*originalScale, "time", scaleTime));
    }

    void ScaleDown()
    {
        iTween.ScaleTo(gameObject, iTween.Hash("x", originalScale, "y", originalScale, "time", scaleTime));
    }

}


*/
