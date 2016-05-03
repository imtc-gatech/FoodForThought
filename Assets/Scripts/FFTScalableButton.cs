using UnityEngine;
using System.Collections;

[RequireComponent(typeof (FFTScalableObject))]
public class FFTScalableButton : MonoBehaviour
{
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
                if (ClickableState)
                {
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
    }
    [SerializeField]
    private bool _selected;

    GameObject Up;
    GameObject Down;

    public bool ClickableState
    {
        get { return ((Down != null) && (Up != null)); }
    }

    // Use this for initialization
    void Start()
    {
        Up = transform.FindChild("Up").gameObject;
        Down = transform.FindChild("Down").gameObject;
        if (Down != null)
            Down.SetActiveRecursively(false);  
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseEnter()
    {
        
    }

    void OnMouseExit()
    {
        
    }

    void OnMouseDown()
    {
        Selected = true;
    }

    void OnMouseUp()
    {
        if (Selected)
        {
            // The click is released, execute the button behavior
            ClickReleaseAction();
        }
    }

    protected virtual void ClickReleaseAction()
    {

    }
}
