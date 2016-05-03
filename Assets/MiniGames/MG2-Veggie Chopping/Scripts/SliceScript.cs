using UnityEngine;
using System.Collections;

public class SliceScript : MonoBehaviour {
	public Vector3 MousePos;  // used for the position of the mouse
	public IRageSpline FoodSpline; //spline that is attached to this object.  Integral to the game
	
	
	private ArrayList leftSide;  //array that will hold all points to the left of the mouse.
	private ArrayList rightSide; //array that will hold all points to the right of the mouse.
	private BoxCollider bC; //box collider of the object, used for mouseover and click detection.
	private GameObject line; //the guideline that is drawn
	private int counts; //a count of the number of cuts made
	private bool mouseOver;
	
	public Object Lline;  //this is the base for the red guideline for cutting
	public MG2_RootScript Root;  //this is the rootscript, it's where most of the base changes are made
	public Object Guide;  //object that is used to generate the guide (shows the player what size of pieces they should cut)
	
	public bool UseSecondGuide = true; //translucent copy of the guide to aid in cutting even pieces
	GameObject guideClone;
	
	
	private Camera mainMinigameCamera;
	
	// Use this for initialization
	void Start () {
		FoodSpline = this.GetComponent(typeof(RageSpline)) as IRageSpline; //initializes FoodSpline to the RageSpline already on the object
		leftSide = new ArrayList();
		rightSide = new ArrayList();
		bC = GetComponent(typeof(BoxCollider)) as BoxCollider; //initializes bC to the BoxCollider already on the object
		Rect bounding = FoodSpline.GetBounds();	//initializes a rectangle object that is the bounds of FoodSpline
		bC.size = new Vector3(bounding.width,bounding.height, .01f);  //resizes the BoxCollider to the size of the FoodSpline
		bC.center = bounding.center;  //recenters the BoxCollider to the object
		if(FoodSpline.GetBounds().width > Root.biggestWidth){  //tests to see if the FoodSpline is bigger than the maximum width of the object.  Used to initially set the maximum size.
			Root.biggestWidth = FoodSpline.GetBounds().width;  //sets this size as the biggest size in the Root object, which is used for scoring, for generating the ideal cut size, and so on.
			generateGuide();  //makes the aforementioned guide
		}
		counts = 0;  //zeroes out the number of cuts for this object.
		Root.Cuts.Add(gameObject);  //adds this object to an array filled with all of the pieces of the object being cut
		mouseOver = false;  //the mouse is not over the object
		mainMinigameCamera = Root.MinigameHolder.GetComponentInChildren<Camera>(); //doing this every frame creates a massive performance hit
	}
	
	// Update is called once per frame
	void Update () {
		if (Root.CurrentState == MG_Minigame.State.Active)
		{
			//Destroy(line); //gets rid of extra guideLines being drawn
			MousePos = mainMinigameCamera.ScreenToWorldPoint(Input.mousePosition);  //sets MousePos to the current mouse position
			counts = Root.count;  //makes sure count is kept accurate with the RootScript
			
			if(mouseOver){ //if the mouse is over the object
				MoveLine();  //run method to draw the guideline
			}
			//mouseOver = false;  //resets the mouseOver variable
		}
	}
	
	void OnMouseEnter() {
		CreateLine();
		mouseOver = true;
	}
	
	void OnMouseExit() {
		Destroy(line); //gets rid of extra guideLines being drawn
		if (UseSecondGuide)
			Root.knifeScript.SwitchGuide(false);
		mouseOver = false;
	}
	/*
	void OnMouseOver(){
		if(Root.CurrentState == MG_Minigame.State.Active){
			mouseOver = true;  //if the mouse is over the collider, it sets mouseOver to true
		}
	}
	*/
	
	void OnMouseDown(){
		if(Root.CurrentState == MG_Minigame.State.Active){		
			SideTest();  //sees what points on the spline are on which side of the mouse
			
			if(counts < Root.MG2_MaxCuts){  //if you haven't already used up all of your cuts
				Slice();  //"cut" the object
			}
		}
	}
	
	void SideTest(){  //sees what points on the spline are on which side of the mouse
		Vector3 currentPoint;  //where is the point we are currently checking?
		
		for(int x=0; x < FoodSpline.GetPointCount(); x++){  //iterate through all of the points.
			currentPoint = FoodSpline.GetPositionWorldSpace(x);  //sets the current point to the position of the point we are currently checking
			if(currentPoint.x <= MousePos.x){  //adds the point to the left side if the point is to the left of the mouse
				leftSide.Add(x);
			}
			else if(currentPoint.x > MousePos.x){ //adds the point to the right side if it is to the right of the mouse
				rightSide.Add(x);
			}
		}
	}
	
	void Slice(){
		GameObject sliced = Instantiate(gameObject) as GameObject;  //copies the food object being cut
		sliced.transform.parent = this.transform.parent;
		sliced.transform.position = this.transform.position;
		int count = 0;  //creats an object to count
		ArrayList uppers = new ArrayList();  //list of points to go in the upper part of the object
		ArrayList downers = new ArrayList(); //list of points to go in the lower part of the object
		
		
		int edgePointUp = FoodSpline.GetNearestPointIndex(transform.InverseTransformPoint(new Vector3(MousePos.x, transform.position.y + FoodSpline.GetBounds().yMax, transform.position.z))); //finds the upper point to use
		Vector3 upPos = CalculateUpPos(edgePointUp,FoodSpline);  //calculates where to place the slice on the upper part of the spline
		int edgePointDown = FoodSpline.GetNearestPointIndex(transform.InverseTransformPoint(new Vector3(MousePos.x, transform.position.y + FoodSpline.GetBounds().yMin, transform.position.z))); //finds the lower point to use as a guide
		Vector3 downPos = CalculateDownPos(edgePointDown,FoodSpline);  //calculates where to place the slice on the lower part of the spline
		float height = Mathf.Abs(downPos.y - upPos.y);  //finds the height of the spline at that point
		
		foreach(int y in rightSide){  //this section erases the right half of the original spline, to make it look like the right side was cut away
			
			Vector3 yPos = FoodSpline.GetPositionWorldSpace(y);  //finds where the point is
			if(yPos.y > downPos.y + height/2f){  //if the point is on the upper half of the spline
				FoodSpline.SetPointWorldSpace(y,upPos);  //place the point in the upper guide point position
				FoodSpline.SetInControlPositionWorldSpace(y,upPos);  //make the control position neutral.
				uppers.Add(y);  //add it to the list of upper points
			}
			else if(yPos.y < upPos.y - height/2f){  //if it's on the lower half
				FoodSpline.SetPointWorldSpace(y,downPos); //place the point in the lower guide point.
				FoodSpline.SetInControlPositionWorldSpace(y,downPos);
				downers.Add(y);
			}
			
		}
		
		
		count = 1;
		foreach(int x in uppers){
			if(uppers.Count > count){  //if there are excess points in the upper guide area (excess being over 1)
				FoodSpline.RemovePoint(x - (count - 1)); //remove the excess point (count - 1) being there to make sure it doesn't erase the wrong point
			}
			count ++;
		}
		count = 1;
		
		foreach(int x in downers){ //do the same thing for the lower guide area
			if(downers.Count > count){
				FoodSpline.RemovePoint(x - (count - 1));
			}
			count ++;
		}

		errorCorrection(FoodSpline);
		FoodSpline.RefreshMesh();
		
		this.transform.Translate(new Vector3(-1.5f, 0f, 0f)); //moves the spline over a little to emphasize the cut
		
		//make the bounding box adjust to fit the newly shaped spline
		Rect bounding = FoodSpline.GetBounds();
		bC.size = new Vector3(bounding.width,bounding.height, .01f);
		bC.center = bounding.center;
		
		IRageSpline slicedSpline = sliced.GetComponent(typeof(RageSpline)) as IRageSpline;
		count = 0;
		
		//reset the arraylists
		uppers.Clear();
		downers.Clear();
		
		//repeat the process done for the original, but this time erase the left side of the copy.
		foreach(int z in leftSide){

			Vector3 zPos = slicedSpline.GetPositionWorldSpace(z);
			if(zPos.y > downPos.y + height/2f){
				slicedSpline.SetPointWorldSpace(z,upPos);
				slicedSpline.SetInControlPositionWorldSpace(z,upPos);
				uppers.Add(z);
			}
			else if(zPos.y < upPos.y - height/2f){
				slicedSpline.SetPointWorldSpace(z,downPos);
				slicedSpline.SetInControlPositionWorldSpace(z,downPos);
				downers.Add(z);
			}
			
		}
		
		count = 1;
		foreach(int x in uppers){
			if(uppers.Count > count){
				slicedSpline.RemovePoint(x - (count - 1));
			}
			count ++;
		}
		count = 1;
		foreach(int x in downers){
			if(downers.Count > count){
				slicedSpline.RemovePoint(x - (count - 1));
			}
			count ++;
		}

		errorCorrection(slicedSpline);
		slicedSpline.RefreshMesh();
		
		
		//howDidIDo(slicedSpline); //check the size of the point
		
		//reset both side arrays
		leftSide.Clear();
		rightSide.Clear();
		
		//make sure that it knows that there's been a cut
		counts ++;
		Root.count = counts;
		
		Root.knifeScript.Chopping(); //perform the cutting animation
		Root.RefreshReport(); //refresh the score report when an action has been taken by the 'knife' - res
	}
	
	Vector3 CalculateUpPos(int index, IRageSpline appliedSpline){
		Vector3 leftPos;
		Vector3 rightPos;
		Vector3 tempPos = appliedSpline.GetPositionWorldSpace(index);  //set the test position to the position of the point found earlier
		//tests whether the point that you found is to the left or the right of the mouse, and gets the left and right positions accordingly
		if(tempPos.x < MousePos.x){
			leftPos = tempPos;
			rightPos = appliedSpline.GetPositionWorldSpace(index + 1);
		}
		else if(tempPos.x > MousePos.x){
			leftPos = appliedSpline.GetPositionWorldSpace(index - 1);
			rightPos = tempPos;
		}
		else{
			Debug.Log("odd");  //if neither side works, and the mouse is on top of the point, then just set the guide point to the location of the found point
			return new Vector3(MousePos.x, tempPos.y, tempPos.z);
		}
		Vector3 slope = new Vector3(rightPos.x-leftPos.x, rightPos.y - leftPos.y, rightPos.z);
		float finalY = leftPos.y + (slope.y/slope.x)*(MousePos.x - leftPos.x);
		return new Vector3(MousePos.x, finalY, leftPos.z);  //otherwise, average the height of the left and right points, and calculate the guide point accordingly
	}
	
	Vector3 CalculateDownPos(int index, IRageSpline appliedSpline){// same as CalculateUpPos() but on the lower end 		
		Vector3 leftPos;
		Vector3 rightPos;
		Vector3 tempPos = appliedSpline.GetPositionWorldSpace(index);
		
		if(tempPos.x < MousePos.x){
			leftPos = tempPos;
			rightPos = appliedSpline.GetPositionWorldSpace(index - 1);
		}
		else if(tempPos.x > MousePos.x){
			leftPos = appliedSpline.GetPositionWorldSpace(index + 1);
			rightPos = tempPos;
		}
		else{
			Debug.Log("odd");
			return new Vector3(MousePos.x, tempPos.y, tempPos.z);
		}
		Vector3 slope = new Vector3(rightPos.x-leftPos.x, rightPos.y - leftPos.y, rightPos.z);
		float finalY = leftPos.y + (slope.y/slope.x)*(MousePos.x - leftPos.x);
		
		return new Vector3(MousePos.x, finalY, leftPos.z);
	}
	
	void errorCorrection(IRageSpline spline){  //method used to make sure the splines don't glitch and lose their fill.
		
		for(int x = 0; x < spline.GetPointCount(); x ++){  //check each point on the spline
			Vector3 controlPoint = spline.GetOutControlPositionWorldSpace(x);
			Vector3 controlPointRelative = spline.GetOutControlPositionPointSpace(x);
			Vector3 checkPoint = spline.GetPositionWorldSpace(x+1);
			
			if(controlPointRelative.x <0){  //if the out position is on the left
				if(controlPoint.x < checkPoint.x){  //check if the out position is further to the left than the next point
					controlPoint.x = checkPoint.x;  //if so, fix it
				}
			}
			else if(controlPointRelative.x > 0){ //if the out control position is on the right, repeat but on the right side
				if(controlPoint.x > checkPoint.x){
					controlPoint.x = checkPoint.x;
				}
			}
			
			spline.SetOutControlPositionWorldSpace(x, controlPoint);
			spline.RefreshMesh();
		}
	}
	
	public Vector3 GetLinePosition()
	{
		return new Vector3(MousePos.x - 1f, this.transform.position.y - 20f + bC.center.y, -1.2f);
	}
	
	void CreateLine() {
		line = Instantiate(Lline, GetLinePosition(), this.transform.rotation) as GameObject; //draws line
		line.transform.parent = Root.gameObject.transform;
		if (UseSecondGuide)
			Root.knifeScript.SwitchGuide(true);
	}
	
	void MoveLine() {
		line.transform.position = GetLinePosition();
	}
	
	
	/*
	void Drawline(){
		
	}
	*/
	
	void generateGuide(){
		TextMesh referenceText = Root.gameObject.GetChildByName("MG2 Text Instructions").GetComponent<TextMesh>();
		Vector3 guideLoc = referenceText.transform.position + new Vector3(referenceText.GetComponent<Renderer>().bounds.extents.x,0f,0f);
		Debug.Log(referenceText.GetComponent<Renderer>().bounds.size);
		GameObject tofuTemp = Instantiate(Guide, guideLoc, this.transform.rotation) as GameObject; //places the guide in the right spot
		IRageSpline tempSpline = tofuTemp.GetComponentInChildren(typeof(RageSpline)) as IRageSpline;
		float guideWidth = Root.biggestWidth/(Root.MG2_MaxCuts + 1f); //calculates the ideal width for the guide
		
		//adjusts points on the guide to fit the ideal size
		tempSpline.SetPoint(0, new Vector3(guideWidth, 15f, 0f));
		tempSpline.SetPoint(1, new Vector3(guideWidth, 5f, 0f));
		tempSpline.SetPoint(2, new Vector3(0f, 5f, 0f));
		tempSpline.SetPoint(3, new Vector3(0f, 15f, 0f));
		
		tempSpline.RefreshMesh();
		tofuTemp.transform.parent = Root.transform;
		if (UseSecondGuide)
		{
			guideClone = Instantiate(tofuTemp, new Vector3(0,0,0), this.transform.rotation) as GameObject;
			guideClone.name = "KnifeGuide";
			guideClone.transform.parent = Root.knifeScript.gameObject.transform;
			guideClone.transform.localPosition = new Vector3(-80, 120, 0);
			float alphaValue = 0.25f;
			IRageSpline transparentGuide = guideClone.GetComponentInChildren(typeof(RageSpline)) as IRageSpline;
			//transparentGuide.SetFill(RageSpline.Fill.None);
			Color fillColor = transparentGuide.GetFillColor1();
			fillColor.a = 0.25f;
			transparentGuide.SetFillColor1(fillColor);
			Color outlineColor = transparentGuide.GetOutlineColor1();
			outlineColor.a = 0.25f;
			transparentGuide.SetOutlineColor1(outlineColor);
			transparentGuide.RefreshMesh();//false, false, false);
			Root.knifeScript.SetGuideClone(guideClone);
		}
	}
	
	void howDidIDo(IRageSpline tempSpline){  //tests whether the cut that was made fits the ideal size or not, and provides feedback
		
		if(FoodSpline.GetBounds().width < tempSpline.GetBounds().width){ //if the left side is smaller, test the left side.
			//initialize both widths to test
			float width = FoodSpline.GetBounds().width;
			float testWidth = Root.biggestWidth/(Root.MG2_MaxCuts + 1f);
			
			//decide whether it's the right size and give feedback
			if(width > testWidth + 1.5f){
				Root.score.feedback = "Too Big";
			}
			else if(width < testWidth - 1.5f){
				Root.score.feedback = "Too Small";
			}
			else{
				Root.score.feedback = "Just Right!";
			}
			
		}
		else if(FoodSpline.GetBounds().width > tempSpline.GetBounds().width){  //if the right side is smaller, test the right side
			//initialize variables
			float width = tempSpline.GetBounds().width;
			float testWidth = Root.biggestWidth/(Root.MG2_MaxCuts + 1f);
			
			//decide if the cut piece is the right size and give feedback
			if(width > testWidth + 1.5f){
				Root.score.feedback = "Too Big";
			}
			else if(width < testWidth - 1.5f){
				Root.score.feedback = "Too Small";
			}
			else{
				Root.score.feedback = "Just Right!";
			}
		}
	}
}