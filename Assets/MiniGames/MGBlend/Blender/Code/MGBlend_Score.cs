using UnityEngine;
using System.Collections;

/// <summary>
/// Score. allows us to easily get and set different aspects relating to the score
/// </summary>
public struct MGBlend_Score
{
	public Color barColor; //the current color of the status bar
	public int toughness; //the perceived "toughness" of a fruit
	public float percentBlended; //the amount a fruit is blended
	public float exceedCount; //how much the player exceed by, if he exceeded the goal
	public bool exceededGoal; //true if the user passed the goal
	public bool attempted; //true if the user at least attempted to blend the fruit
	
	public MGBlend_Score (Color BARCOLOR, int TOUGHNESS, float PERCENTBLENDED, float EXCEEDCOUNT, bool EXCEEDEDGOAL, bool ATTEMPTED){
        this.barColor = BARCOLOR;
		this.toughness = TOUGHNESS;
		this.percentBlended = PERCENTBLENDED;
		this.exceedCount = EXCEEDCOUNT;
		this.exceededGoal = EXCEEDEDGOAL;
		this.attempted = ATTEMPTED;
 	}
};

