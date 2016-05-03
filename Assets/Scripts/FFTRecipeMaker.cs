using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTRecipeMaker : MonoBehaviour {

    public TextAsset LevelPath;
    public bool TimedLevel;
    public float TimeLimit = 0.0f;

    public float ElapsedTimeMultiplier = 1.0f;

    public FFTCounter Counter
    {
        get
        {
            if (_counter == null)
            {
                _counter = GameObject.FindGameObjectWithTag("Counter").GetComponent<FFTCounter>();
            }
            return _counter;
        }        
    }
    [SerializeField]
    private FFTCounter _counter;

    public FFTKitchen Kitchen
    {
        get
        {
            if (_kitchen == null)
            {
                _kitchen = GameObject.FindGameObjectWithTag("Kitchen").GetComponent<FFTKitchen>();
            }
            return _kitchen;
        }
    }
    [SerializeField]
    private FFTKitchen _kitchen;

    public float Difficulty
    {
        get
        {
            return _difficulty;
        }
    }

    public float Complexity
    {
        get
        {
            return _complexity;
        }
    }
    [SerializeField]
    private float _difficulty = 0;

    [SerializeField]
    private float _complexity = 0;

    public bool RecalculateDifficultyAndComplexity
    {
        get
        {
            return false;
        }
        set
        {
            if (value == true)
            {
                _complexity = FFTRecipeStats.GetComplexity(this);
                _difficulty = FFTRecipeStats.GetDifficulty(this);
            }
        }
    }

    public bool RecipeChanged = false;

    public int TotalSteps
    {
        get
        {
            int result = 0;
            if (Counter != null)
            {
                if (Counter.RecipeCard.Dishes != null)
                {
                    foreach (FFTDish dish in Counter.RecipeCard.Dishes)
                    {
                        if (dish.StepDataObjects != null)
                            result += dish.StepDataObjects.Count;
                    }
                }
            }
            return result;
        }
    }

	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
