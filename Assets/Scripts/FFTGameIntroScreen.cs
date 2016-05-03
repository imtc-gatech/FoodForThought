using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTGameIntroScreen : MonoBehaviour {
	
	public enum GameState
	{
		Intro,
		Gameplay
	}
	
	public GameState State = GameState.Intro;
	
	public TextMesh RecipeText;
	public TextMesh FlavorText;
	public FFTStationOverviewWidget StationCard;
	
	public bool IsVisible = true;
	
	public FFTCustomerView CustomerView;
	
	public FFTStationOcclusionView Shadow;
	
	void Start() {
		//Test();
		Shadow = FFTGameManager.Instance.Shadow;
		Populate();
	}
	
	public void Populate()
	{
		FFTGameManager GM = FFTGameManager.Instance;
		RecipeText.text = GM.Counter.RecipeCard.LevelTitle;
		FlavorText.text = GM.Counter.RecipeCard.FlavorText;
		
		StationCard.chopSlots = GM.Kitchen.StationCount(FFTStation.Type.Chop);
		StationCard.cookSlots = GM.Kitchen.StationCount(FFTStation.Type.Cook);
		StationCard.spiceSlots = GM.Kitchen.StationCount(FFTStation.Type.Spice);
		StationCard.prepSlots = GM.Kitchen.StationCount(FFTStation.Type.Prep);
		
		StationCard.RefreshDisplay();
		
		string customerName = GM.Counter.RecipeCard.Customer.ToString();
		GameObject character = GameObject.Instantiate(Resources.Load("ResultsScreen/Characters/Character_" + customerName.ToString(), typeof(GameObject)) as GameObject) as GameObject;
		character.transform.parent = gameObject.transform;
		character.transform.localScale = new Vector3(0.5f, 0.5f, 1);
		character.transform.localPosition = new Vector3(-16, 53, -7);
		CustomerView = character.AddComponent<FFTCustomerView>();
		CustomerView.State = FFTCustomerView.VisualState.Average;
		
		IsVisible = true;
		
	}
	
	public void MoveView(float xDirection, float time)
	{
		Hashtable ht = new Hashtable(){
            {iT.MoveTo.x, xDirection},
            {iT.MoveTo.time, time},
			{iT.MoveTo.easetype, iTween.EaseType.easeInExpo}
        };
        iTween.MoveTo(gameObject, ht);
		
		if (xDirection < 0)
			IsVisible = false;
		else
			IsVisible = true;
		
	}
	
	public void MoveView(float xDirection, float time, string onCompleteFunction)
	{
		Hashtable ht = new Hashtable(){
            {iT.MoveTo.x, xDirection},
            {iT.MoveTo.time, time},
			{iT.MoveTo.easetype, iTween.EaseType.easeInExpo},
			{iT.MoveTo.oncomplete, onCompleteFunction}
        };
		 
        iTween.MoveTo(gameObject, ht);
		
		if (xDirection < 0)
			IsVisible = false;
		else
			IsVisible = true;
		
	}
	
	public void HideView()
	{
		MoveView (-200, 0.3f);
	}
	
	public void ShowView()
	{
		MoveView (0, 0.3f);
	}
	
	public void DismissScreen() {
	
		if (State == GameState.Intro)
			DismissIntroScreen();
		else
			DismissAlertScreen();
	}

	
	public void DismissIntroScreen() {
		
		//iTween.MoveTo(gameObject, new Vector3(-200, 0, 0), 2f);
		
		FFTGameManager.Instance.Drawer.Arrow.Dismiss();
		
		MoveView (-200, 0.5f, "StartGameplay"); //in StartGameplay, intro screen moved so it is closer to the screen 
												//to avoid occlusion on alerts
	
		FFTGameManager.Instance.Drawer.ShowArrow();
		
		State = GameState.Gameplay;
		
	}
	
	public void DismissAlertScreen() {
		float time = 0.3f;
		MoveView (-200, time);
		Shadow.FadeOut (time);
		if (FFTTimeManager.Instance.GameplayPaused)
			FFTTimeManager.Instance.GameplayPaused = false;
	}
	
	public void Test()
	{
		string recipeTest = @"Alert!";
		string flavorTest = @"Our customer has requested that we make the dish extra burnt for them. Please do your best!";
		// @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.";
			//flavorTest += " Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Typi non habent claritatem insitam; est usus legentis in iis qui facit eorum claritatem. Investigationes demonstraverunt lectores legere me lius quod ii legunt saepius. Claritas est etiam processus dynamicus, qui sequitur mutationem consuetudium lectorum. Mirum est notare quam littera gothica, quam nunc putamus parum claram, anteposuerit litterarum formas humanitatis per seacula quarta decima et quinta decima. Eodem modo typi, qui nunc nobis videntur parum clari, fiant sollemnes in futurum.";
		
		RecipeText.text = recipeTest;
		FlavorText.text = flavorTest;
		
		
		if (StationCard != null)
		{
			StationCard.chopSlots = 1;
			StationCard.cookSlots = 2;
			StationCard.spiceSlots = 3;
			StationCard.prepSlots = 4;
			
			StationCard.RefreshDisplay();
		}
		
		if (CustomerView == null)
		{
			string customerName = FFTCustomer.Name.Chloe.ToString(); // GameManager.Instance.Counter.RecipeCard.Customer.ToString();
			GameObject character = GameObject.Instantiate(Resources.Load("ResultsScreen/Characters/Character_" + customerName.ToString(), typeof(GameObject)) as GameObject) as GameObject;
			character.transform.parent = gameObject.transform;
			character.transform.localScale = new Vector3(0.55f, 0.55f, 1);
			character.transform.localPosition = new Vector3(-16, 61, -7);
			CustomerView = character.AddComponent<FFTCustomerView>();
			CustomerView.State = FFTCustomerView.VisualState.Bad;
		}
		
	}
	
	public void RecipeTest()
	{
		string recipeTest = @"15.87_I_AwesomeOmelet";
		string flavorTest = @"It's a breakfast emergency at Albert the Alligator's swamp.  He needs an awesome omelet and he needs it now.  Throw some sausage and peppers in that omelet and get it to the gator in 1 minute and 10 seconds before he gets upset. ";
		
		RecipeText.text = recipeTest;
		FlavorText.text = flavorTest;
		
		if (StationCard != null)
		{
			StationCard.chopSlots = 1;
			StationCard.cookSlots = 2;
			StationCard.spiceSlots = 3;
			StationCard.prepSlots = 4;
			
			StationCard.RefreshDisplay();
		}
		
		if (CustomerView == null)
		{
			string customerName = FFTCustomer.Name.Alligator.ToString(); // GameManager.Instance.Counter.RecipeCard.Customer.ToString();
			GameObject character = GameObject.Instantiate(Resources.Load("ResultsScreen/Characters/Character_" + customerName.ToString(), typeof(GameObject)) as GameObject) as GameObject;
			character.transform.parent = gameObject.transform;
			character.transform.localScale = new Vector3(0.55f, 0.55f, 1);
			character.transform.localPosition = new Vector3(-16, 61, -7);
			CustomerView = character.AddComponent<FFTCustomerView>();
			CustomerView.State = FFTCustomerView.VisualState.Average;
		}
		
	}
	
	public void StartGameplay()
	{
		FFTGameManager.Instance.GameplayArmed = true;
		
		//we will now wait for first user action to start gameplay.
		//FFTGameManager.Instance.State = FFTGameManager.GameState.Gameplay;
		
		FFTGameManager.Instance.Drawer.CleanUpForGameplay();
		
		FFTUtilities.DestroySafe(CustomerView);
		FFTUtilities.DestroySafe(StationCard);
		
		//intro screen moved so it is closer to the screen to avoid occlusion on subsequent alerts
		gameObject.transform.position += new Vector3(0, 0, -100);
		
		Debug.Log("Start Gameplay Callback");
	}
	

}
