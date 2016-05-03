using UnityEngine;
using System.Collections;

public class FFTUIHandPointer : MonoBehaviour {

    public enum VisualState
    {
        handPoint = 0,
        handOpen = 1,
        handClosed = 2
    }

    public VisualState State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                gameObject.GetChildByName(_state.ToString()).SetActiveRecursively(false);
                gameObject.GetChildByName(value.ToString()).SetActiveRecursively(true);
                _state = value;
            }
        }
    }
    private VisualState _state;

	// Use this for initialization
	void Start () {
        transform.position = Input.mousePosition;
        State = VisualState.handOpen;
	}
	
	// Update is called once per frame
	void Update () {
        ConformHandToMousePosition();
        if (Input.GetMouseButtonDown(0))
        {
            State = VisualState.handClosed;
        }

        if (Input.GetMouseButtonUp(0))
        {
            State = VisualState.handOpen;
        }
	}

    void ConformHandToMousePosition()
    {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = -105;
        transform.localPosition = cursorPosition;
    }
}
