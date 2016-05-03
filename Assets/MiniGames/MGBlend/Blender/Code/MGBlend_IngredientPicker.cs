using UnityEngine;
using System.Collections;

/// <summary>
/// MG blend_ ingredient picker. Allows player to choose random play or not. If not, user can choose which
/// fruits to use. Additionally, user can also change the softness.
/// </summary>
public class MGBlend_IngredientPicker : MonoBehaviour {
	//ingredients to pick from
	public enum IngredientName {Apple, Avocado, Banana, Cantaloupe, Cranberry, Grapes, Honeydew, Kiwi, Pineapple, Strawberry, Watermelon, None};

	//public bool RandomPlay = false; //if selected, the game will randomly generate ingredients. Default softness values are used in random play.
	//public bool UseDefaultSoftness = false; //check this if you don't want to toggle softness yourself. 
	//public bool ClickTrueToRestart = false; //check this box if you want to pick some new fruits or restart the game
	
	public bool RandomPlay { get {return GS.RandomPlay;} set {GS.RandomPlay = value;} }
	public bool UseDefaultSoftness { get {return GS.UseDefaultSoftness;} set {GS.UseDefaultSoftness = value;} }
	public bool ClickTrueToRestart { get {return GS.ClickTrueToRestart;} set {GS.ClickTrueToRestart = value;} }
	public int NumOfIngredients { get {return GS.NumOfIngredients;} set {GS.NumOfIngredients = value;} }

	public Hashtable fruitsAndStatuses = new Hashtable(); //a hashtable to keep track of all the fruits and their corresponding status bars
	public Hashtable fruitsAndGoals = new Hashtable(); //a hashtable to keep track of all the fruits and their corresponding goals

	public IngredientName FirstIngredient = IngredientName.None; //first choice of ingredient
	public float FirstIngredientSoftness = 0; //a number between 0 && 70; bigger number -> more soft
	
	public IngredientName SecondIngredient = IngredientName.None; //second choice of ingredient
	public float SecondIngredientSoftness = 0; //a number between 0 && 70; bigger number -> more soft
	
	public IngredientName ThirdIngredient = IngredientName.None; //third choice of ingredient
	public float ThirdIngredientSoftness = 0;//a number between 0 && 70; bigger number -> more soft
	
	private GameObject generatedObjects; //empty holder parent for generated gameobjects

	private GameObject ingredientLoad; //the current ingredient to be loaded
	private GameObject duplicateLoad; //the shadow of the current ingredient to be loaded 
	private GameObject statusBlendGoal; //part of the scoring object
	private GameObject statusHolder; //part of the scoring gameobject
	private GameObject statusFill; //part of the scoring gameobject
	private GameObject progressIndicator; //part of the scoring gameobject

	private Color red; //custom red color
	private Color green; //custom green color
	
	private string fruit; //the name of the fruit
	
	private float blendBarSubtractAmount; //determines the about of blending a fruit will need. 
	protected float goalPositionMaxY = 46f; //the max location for blend goals when taking in account thresholds
	
	private MGBlend_FruitControllerScript FCS; //fruit controller script ref
	public MGBlend_GameScript GS; // gameScript scipt ref
	private MGBlend_LiquidScript LS; //liquid script ref
	private MGBlend_BlenderTopScript BTS; //blender top script ref
	
	
	/// <summary>
	/// Awake this instance. Used for initializations. 
	/// </summary>
	void Awake() {
		GS = GameObject.Find(this.name).GetComponent<MGBlend_GameScript>();
		LS = GameObject.Find(this.name + "/BlenderLiquid").GetComponent<MGBlend_LiquidScript>();
		BTS = GameObject.Find (this.name + "/BlenderTopCollider").GetComponent<MGBlend_BlenderTopScript>();
		generatedObjects = GameObject.Find (this.name + "/MGBlend_GenerateOnStart");
	}
	
	/// <summary>
	/// Update this instance. Called once per frame. If user checked the restart, the old fruits get destroyed,
	/// and depending on whether or not RandomPlay is on, new fruits get loaded in, the liquid gets set to orig location
	/// and ClickTrueToRestart is set to false.
	/// </summary>
	void Update(){	
		if(ClickTrueToRestart){
			destroyFruits(); //destroys every gameobject that is generated at every restart
			
			if(RandomPlay){ //if random play enabled
				refreshRand(); //refresh fruit randomly
			}
			else{ //else
				refreshPickedFruits(); //use user picked fruits
			}
			
			LS.resetLiquid(); //reset liquid height
			
			ClickTrueToRestart = false; //set to false so the game doesn't keep restarting
		}
	}
	
	/// <summary>
	/// Refreshs the fruit randomly. First called preparation method for random, then restarts the game with the new parameters.
	/// </summary>
	void refreshRand(){
		prepForRand ();
		restartGame (NumOfIngredients, FirstIngredient, SecondIngredient, ThirdIngredient);
	}
	
	/// <summary>
	/// Preps for rand. generates a random number of ingredients. For each ingredient, the game chooses a random enum and sets
	/// it equal to either the first, second or third ingredient.
	/// </summary>
	void prepForRand(){
		NumOfIngredients = Mathf.Clamp(NumOfIngredients, 0, 3);
		if (GS.randomizeNumberOfIngredients || NumOfIngredients == 0) //we can't blend zero ingredients, so randomly pick a number
			NumOfIngredients = Random.Range(2, 4);
		
		for(int i=0; i<NumOfIngredients; i++){
			IngredientName newIngredient = GetRandomEnum<IngredientName>(); //picks a random ingredient from the list of available ingredients
			
			if(i==0)
				FirstIngredient = newIngredient;
			else if(i==1)
				SecondIngredient = newIngredient;
			else
				ThirdIngredient = newIngredient;
		}
		
		if(NumOfIngredients < 3)
			ThirdIngredient = IngredientName.None;
		if(NumOfIngredients < 2)
			SecondIngredient = IngredientName.None;
		if(NumOfIngredients < 1)
			FirstIngredient = IngredientName.None;
		
		clearNones(); //reorgainizes the current ingredients - moves Nones to end
	}
	
	/// <summary>
	/// Gets the ingredient load location.
	/// </summary>
	/// <returns>
	/// The ingredient load location.
	/// </returns>
	/// <param name='i'>
	/// I. this tells us the number of the location, and where it belongs on the countertop 
	/// </param>
	Vector3 getIngredientLoadLocation(int i){
		Vector3 ingredientLoadLocation;
		
		if(i==0)
			ingredientLoadLocation = GS.CameraLoc + new Vector3(-106.25f, -60.5f, 10.5f);
		else if(i==1)
			ingredientLoadLocation = GS.CameraLoc + new Vector3(-34.98f, -60.5f, 10.5f);
		else
			ingredientLoadLocation = GS.CameraLoc + new Vector3(-177.54f, -60.5f, 10.5f);
		
		return ingredientLoadLocation;
	}
	
	/// <summary>
	/// Enums to string. Also assigns the blend bar subtract amounts.
	/// </summary>
	/// <param name='ingredient'>
	/// Ingredient. the ingredient name to compare with all the cases in the switch statement.
	/// </param>
	void enumToString(IngredientName ingredient){
		switch(ingredient){
			
		case IngredientName.Apple:
			fruit = "Apple";
			blendBarSubtractAmount = 23f;
			break;
			
		case IngredientName.Avocado:
			fruit = "Avocado";
			blendBarSubtractAmount = 3f;
			break;
			
		case IngredientName.Banana:
			fruit = "Banana";
			blendBarSubtractAmount = 7f;
			break;
			
		case IngredientName.Cantaloupe:
			fruit = "Cantaloupe";
			blendBarSubtractAmount = 30f;
			break;
			
		case IngredientName.Cranberry:
			fruit = "Cranberry";
			blendBarSubtractAmount = 50f;
			break;
			
		case IngredientName.Grapes:
			fruit = "Grapes";
			blendBarSubtractAmount = 46f;
			break;
			
		case IngredientName.Honeydew:
			fruit = "Honeydew";
			blendBarSubtractAmount = 25f;
			break;
			
		case IngredientName.Kiwi:
			fruit = "Kiwi";
			blendBarSubtractAmount = 37f;
			break;
			
		case IngredientName.Pineapple:
			fruit = "Pineapple";
			blendBarSubtractAmount = 20f;
			break;
			
		case IngredientName.Strawberry:
			fruit = "Strawberry";
			blendBarSubtractAmount = 38f;
			break;
			
		case IngredientName.Watermelon:
			fruit = "Watermelon";
			blendBarSubtractAmount = 40f;
			break;
			
		default:
			if(RandomPlay){
				if(FirstIngredient.Equals (ingredient)){
					FirstIngredient = IngredientName.Strawberry;
					fruit = "Strawberry";
					blendBarSubtractAmount = 38f;
				}
				else if(SecondIngredient.Equals (ingredient)){
					SecondIngredient = IngredientName.Watermelon;
					fruit = "Watermelon";
					blendBarSubtractAmount = 40f;
				}
				else{
					ThirdIngredient = IngredientName.Pineapple;
					fruit = "Pineapple";
					blendBarSubtractAmount = 20f;
				}
			}
			break;
		}	
	}
	
	/// <summary>
	/// Destroies the fruits of all the components of the game that need to be refreshed and restarted..
	/// </summary>
	void destroyFruits(){
		ArrayList children = new ArrayList();
		
		foreach(Transform child in generatedObjects.transform){
			children.Add(child.gameObject);	
		}
		if(children.Count != 0){
			foreach(GameObject child in children){
				Destroy (child);
			}	
		}
	}
	
	/// <summary>
	/// Clears the nones. makes sure that there are just the right number of ingredients that exist
	/// </summary>
	void clearNones(){
		if(NumOfIngredients == 1){
			SecondIngredient = IngredientName.None;	
			ThirdIngredient = IngredientName.None;
		}
		else if(NumOfIngredients == 2){
			ThirdIngredient = IngredientName.None;	
		}
	}
	
	/// <summary>
	/// Reorders the fruits. for example, if first ingred was none, second was apple, and third was pineapple,
	/// it would reorder the fruit to be apple, pineapple, none.
	/// </summary>
	void reorderFruits(){
		if(FirstIngredient.Equals (IngredientName.None)){
			if(!SecondIngredient.Equals(IngredientName.None)){
				FirstIngredient = SecondIngredient;
				SecondIngredient = IngredientName.None;
			}
			else if(!ThirdIngredient.Equals (IngredientName.None)){
				FirstIngredient = ThirdIngredient;
				ThirdIngredient = IngredientName.None;
			}
			else{
				NumOfIngredients = 0; //if all three are none, numofingredients = 0;
			}
		}
		if(SecondIngredient.Equals (IngredientName.None)){
			if(!ThirdIngredient.Equals (IngredientName.None))
			{
				SecondIngredient = ThirdIngredient;
				ThirdIngredient = IngredientName.None;
			}
		}
	}
	
	/// <summary>
	/// Updates the number of ingredients to the right number
	/// </summary>
	void updateNumOfIngredients(){
		if(FirstIngredient.Equals (IngredientName.None))
			NumOfIngredients = 0;
		else if(SecondIngredient.Equals (IngredientName.None))
			NumOfIngredients = 1;
		else if(ThirdIngredient.Equals (IngredientName.None))
			NumOfIngredients = 2;
		else
			NumOfIngredients = 3;
	}
	
	/// <summary>
	/// Refreshs the picked fruits. chooses and reorders new fruit choices, updates the total number of ingredients, and then
	/// refreshs the fruits with all the necessary components.
	/// </summary>
	void refreshPickedFruits(){
		reorderFruits();
		updateNumOfIngredients();
		restartGame(NumOfIngredients, FirstIngredient, SecondIngredient, ThirdIngredient);
	}
	
	/// <summary>
	/// Refreshes the fruits to be used in the round/level.
	/// </summary>
	/// <param name='ingredientQuantity'>
	/// Ingredient quantity. number of ingredients
	/// </param>
	/// <param name='first'>
	/// First. name of first ingredient
	/// </param>
	/// <param name='second'>
	/// Second. name of second ingredient
	/// </param>
	/// <param name='third'>
	/// Third. name of third ingredient
	/// </param>
	void restartGame(int ingredientQuantity, IngredientName first, IngredientName second, IngredientName third){
		BTS.restartCap();
		GS.ScoringArray = new MGBlend_Score[ingredientQuantity]; //creates a new scoring array
		fruitsAndStatuses.Clear(); //clears the original hashtable of fruits and statuses
		fruitsAndGoals.Clear(); //clears the original hashtable of fruits and goals

		for(int i=0; i<ingredientQuantity; i++){ //for each ingredient, find the ingredient and update the food's softness			
			if(i==0){
				enumToString(first);
				updateSoftness(i);
			}
			else if(i==1){
				enumToString(second);
				updateSoftness(i);
			}
			else{
				enumToString (third);
				updateSoftness(i);
			}
			
			//creates the ingredient and corresponding status bar
			ingredientLoad = Instantiate (Resources.Load (fruit)) as GameObject;
			ingredientLoad.GetComponent<MGBlend_FruitControllerScript>().gs = GS;
			duplicateLoad = Instantiate (Resources.Load (fruit)) as GameObject;
			duplicateLoad.GetComponent<MGBlend_FruitControllerScript>().gs = GS;
			statusBlendGoal = Instantiate (Resources.Load ("BlendAmount")) as GameObject;
			statusHolder = Instantiate (Resources.Load ("Status")) as GameObject;
			statusFill = Instantiate (Resources.Load ("StatusFill")) as GameObject;
			progressIndicator = Instantiate (Resources.Load ("ProgressIndicator")) as GameObject;
			
			//puts all the newly instantiated gameobjects as children of a specified parent object
			Transform parent = generatedObjects.transform;
			ingredientLoad.transform.parent = parent;
			duplicateLoad.transform.parent = parent;
			statusBlendGoal.transform.parent = parent;
			statusHolder.transform.parent = parent;
			statusFill.transform.parent = parent;
			progressIndicator.transform.parent = parent;

			FCS = duplicateLoad.GetComponent<MGBlend_FruitControllerScript>(); //reference to Fruit Controller
			

			//pulls my custom red and green color and saves it to public variables so it is accessible throughout the game.
			IRageSpline tempColor1 = progressIndicator.transform.GetChild(0).GetComponent(typeof(RageSpline)) as IRageSpline;
			IRageSpline tempColor2 = statusBlendGoal.transform.GetChild(0).GetComponent(typeof(RageSpline)) as IRageSpline;
			red = tempColor1.GetFillColor1();
			green = tempColor2.GetFillColor1();

			//puts the ingredient in the right location, based on how many ingredients there are in all
			Vector3 loadLocation = getIngredientLoadLocation(i);
			float x = loadLocation.x;
			float y = loadLocation.y;
			ingredientLoad.transform.position = loadLocation;
			duplicateLoad.transform.position = new Vector3(x, y, GS.CameraLoc.z + 10.6f);
			statusHolder.transform.position = new Vector3(x - 3.5f, GS.CameraLoc.y + -9.0f, GS.CameraLoc.z + 10f);
			statusFill.transform.position = new Vector3(x - 3.5f, GS.CameraLoc.y + -9.0f, GS.CameraLoc.z + 10.7f);
			statusBlendGoal.transform.position = new Vector3(x - 14.5f, GS.CameraLoc.y + goalPositionMaxY - blendBarSubtractAmount, GS.CameraLoc.z + 10.44f);
			progressIndicator.transform.position = new Vector3(x - 10.5f, GS.CameraLoc.y -16.0f, GS.CameraLoc.z + 10.2f);

			//Makes the ghost ingredient a faded, transparent color.
			Color ghost = new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.35f);
			duplicateLoad.transform.GetComponent<Renderer>().material.color = ghost;
			duplicateLoad.name = "Shadow";
			FCS.Clickable = false;

			fruitsAndStatuses.Add(ingredientLoad, progressIndicator); //adds to hashtable so each fruit's status is accessible
			fruitsAndGoals.Add(ingredientLoad, statusBlendGoal); //adds to hashtable so each fruit's blend goals are available
			
			GS.ScoringArray[i] = new MGBlend_Score(red, 0, 0, 0, false, false); //adds a new score to scoring array
			
		}
		
		GS.myRed = red;
		GS.myGreen = green;	
	}
	
	
	/// <summary>
	/// Updates the softness. if usedefaultsoftness or randomplay is checked, default softness values are used.
	/// else, custom values are used
	/// </summary>
	/// <param name='order'>
	/// Order. if the fruit is the first, second or third ingredient.
	/// </param>
	void updateSoftness(int order){
		if(order == 0){
			if(UseDefaultSoftness || RandomPlay)
				FirstIngredientSoftness = blendBarSubtractAmount*.15f;
			else
				blendBarSubtractAmount = FirstIngredientSoftness/.15f;
			checkSoftnessBounds(0);
		}
		else if(order == 1){
			if(UseDefaultSoftness || RandomPlay)
				SecondIngredientSoftness = blendBarSubtractAmount;
			else
				blendBarSubtractAmount = SecondIngredientSoftness/.15f;
			checkSoftnessBounds(1);
		}
		else{
			if(UseDefaultSoftness || RandomPlay)
				ThirdIngredientSoftness = blendBarSubtractAmount;
			else
				blendBarSubtractAmount = ThirdIngredientSoftness/.15f;
			checkSoftnessBounds(2);
		}
	}
	
	/// <summary>
	/// Checks to make sure the fruit softness is within the bounds.
	/// </summary>
	/// <param name='num'>
	/// Number. tells us if we are dealing with the first, second or third ingredient. 
	/// </param>
	void checkSoftnessBounds(int num){
		 //this puts a lower and upper restriction on the softness of the fruit.
		float max = (2f/3f)*100f;
		float min = 0f;
		
		if(blendBarSubtractAmount > max)
			blendBarSubtractAmount = max;
		if(blendBarSubtractAmount < min)
			blendBarSubtractAmount = min;
		
		if(num==0)
			FirstIngredientSoftness = blendBarSubtractAmount*.15f;
		else if(num==1)
			SecondIngredientSoftness = blendBarSubtractAmount*.15f;
		else
			ThirdIngredientSoftness = blendBarSubtractAmount*.15f;
	}
	
	//this will choose a random fruit from the fruits list
	static T GetRandomEnum<T>()
	{
	    System.Array A = System.Enum.GetValues(typeof(T));
	    T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
	    return V;
	}
}
