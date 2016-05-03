using UnityEngine;
using System.Collections;

public class FFTChloe : MonoBehaviour {

    GameObject CurrentObject;

    public enum RawState
    {
        idle = 0,
        itemInteractLeft = 1,
        itemInteractRight = 2,
        panic = 3,
        pointLeft = 4,
        pointRight = 5,
        runDown = 6,
        runLeft = 7,
        runRight = 8,
        runUp = 9
    }

    public enum SpriteState
    {
        Idle = 0,
        ItemInteract = 1,
        Panic = 2,
        Point = 3,
        Run = 4
    }

    public enum SpriteDirection
    {
        Idle = 0,
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4
    }

    public SpriteDirection Direction
    {
        get
        {
            return _direction;
        }
        set
        {
            if (_direction != value)
            {
                switch (State)
                {
                    case SpriteState.Idle:
                        _direction = SpriteDirection.Idle;
                        break;
                    case SpriteState.Panic:
                        _direction = SpriteDirection.Idle;
                        break;
                    case SpriteState.ItemInteract:
                    case SpriteState.Point:
                        if (Direction == SpriteDirection.Left || Direction == SpriteDirection.Right)
                            _direction = value;
                        break;
                    case SpriteState.Run:
                        if (Direction != SpriteDirection.Idle)
                            _direction = value;
                        break;

                }
                _direction = value;
                SwitchState(State);
            }
            
        }

    }
    [SerializeField]
    private SpriteDirection _direction;
    public SpriteState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != value)
            {
                _state = value;
                SwitchState(_state);
            }
            
        }

    }
    [SerializeField]
    private SpriteState _state;


    public RawState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            Off(_currentState);
            On(value);
            _currentState = value;            
        }
    }
    [SerializeField]
    private RawState _currentState;

    float switchRate = 1.0f;
    float currentTime = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime * FFTTimeManager.Instance.GameplayTimeScale;
        int timeInt = (int)currentTime;
        timeInt = timeInt % 5;
        State = (SpriteState)timeInt;
        
	
	}

    void Off(RawState state)
    {
        GameObject stateObject = gameObject.GetChildByName(state.ToString());
        stateObject.SetActiveRecursively(false);
    }

    void On(RawState state)
    {   
        CurrentObject = gameObject.GetChildByName(state.ToString());
        CurrentObject.SetActiveRecursively(true);
    }

    void SwitchState(SpriteState state)
    {
        switch (state)
        {
            case SpriteState.Idle:
                CurrentState = RawState.idle;
                break;
            case SpriteState.Panic:
                CurrentState = RawState.panic;
                break;
            case SpriteState.ItemInteract:
                if (Direction == SpriteDirection.Left)
                    CurrentState = RawState.itemInteractLeft;
                if (Direction == SpriteDirection.Right)
                    CurrentState = RawState.itemInteractRight;
                break;
            case SpriteState.Point:
                if (Direction == SpriteDirection.Left)
                    CurrentState = RawState.pointLeft;
                if (Direction == SpriteDirection.Right)
                    CurrentState = RawState.pointRight;
                break;
            case SpriteState.Run:
                if (Direction == SpriteDirection.Up)
                    CurrentState = RawState.runUp;
                if (Direction == SpriteDirection.Down)
                    CurrentState = RawState.runDown;
                if (Direction == SpriteDirection.Left)
                    CurrentState = RawState.runLeft;
                if (Direction == SpriteDirection.Right)
                    CurrentState = RawState.runRight;
                break;
        }
    }
}
