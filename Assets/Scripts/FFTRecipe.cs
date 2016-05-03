using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FFTRecipe 
{
    public string Author = "My Author Name";

    public string LevelTitle = "My Recipe Name";

    public int RevisionNumber = 0;

    public TextAsset LevelPath = new TextAsset();

    public List<FFTDish> Dishes = new List<FFTDish>();

    public FFTCustomer.Name Customer = FFTCustomer.Name.Monkey;

    public string FlavorText = "(introductory text here)";

    public FFTFreshnessMeterParameters FreshnessParameters;
	
	public int MaximumNumberOfSteps
	{
		get 
		{
			int result = 0;
			foreach(FFTDish dish in Dishes)
			{
				if (result < dish.StepDataObjects.Count)
					result = dish.StepDataObjects.Count;
			}
			return result;
		}
	}

    public FFTRecipe()
    {
        Author = "def";
        LevelTitle = "Recipe";
        LevelPath = new TextAsset();
        Dishes = new List<FFTDish>();
        FreshnessParameters = new FFTFreshnessMeterParameters();
    }

}