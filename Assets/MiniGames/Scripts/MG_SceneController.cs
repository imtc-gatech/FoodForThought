using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MG_SceneController : MonoBehaviour {

    public static string SceneControllerName = "MinigameManager";

    public Dictionary<string, MG_Minigame> MinigameDict = new Dictionary<string, MG_Minigame>();

    public Camera MinigameCamera;
	
	private MG_Minigame currentGame;
	// Minigame Camera Parameters
    float offsetX = .05f;
    float offsetY = .05f;
    //float orthographicSizeModification = 1.24f;
	float orthographicSizeModification = 1.7f;
    float minWidth = .01f;
    float minHeight = .01f;
    float maxWidth = .9f;
    float maxHeight = .9f;
    float heightOffsetY = 11;
	
	Vector2 minigameCameraPixelPosition = new Vector2();
	Vector2 minigameCameraPixelDimensions = new Vector2();
	
	float cameraAnimationTimeTotal = 1f;
    float currentCameraAnimationTime = 0;
    bool cameraAnimating = false;
    bool cameraIsClosing = false;
    Vector3 currentOriginPoint; //for camera to "open and close" to a particular slot on screen

    public bool InMinigame = false;
	
    // Use this for initialization
	void Start () {
        gameObject.name = SceneControllerName;
        currentOriginPoint = new Vector3();
		SetupMinigameCameraDimensionsForHandCursor();
        if (MinigameCamera == null)
        {
            GameObject MinigameCameraGO = new GameObject("MinigameCamera");
            MinigameCamera = MinigameCameraGO.AddComponent<Camera>();
            MinigameCamera.transform.parent = transform;
            MinigameCamera.rect = new Rect(0, 0, 0.005f, 0.005f);
            MinigameCamera.nearClipPlane = 0.1f;
            MinigameCamera.orthographic = true;
        }
		//Check for AspectUtility.cs, adjust camera based on reconfigured letterbox/pillarbox view
		Rect mainViewportRect = Camera.main.rect;
		if (mainViewportRect.x > 0 || mainViewportRect.y > 0)
		{
			//change the position to match the new window (0.1f on a full screen becomes a percentage of this, 
			//offset by the letterbox/pillarbox padding
			offsetX = ((1 - mainViewportRect.x) * offsetX) + mainViewportRect.x; 
			offsetY = ((1 - mainViewportRect.y) * offsetY) + mainViewportRect.y; 
			
			maxWidth *= (mainViewportRect.width);
			maxHeight *= (mainViewportRect.height);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(cameraAnimating){
            UpdateCameraAnimation();
		}
	}
	
	void SetupMinigameCameraDimensionsForHandCursor()
	{
		minigameCameraPixelPosition = new Vector2();
		minigameCameraPixelDimensions = new Vector2();
		
		minigameCameraPixelDimensions.x = Screen.width*maxWidth;
		minigameCameraPixelDimensions.y = Screen.height*maxHeight;
		
		minigameCameraPixelPosition.x = (Screen.width - minigameCameraPixelDimensions.x) / 2;
		minigameCameraPixelPosition.y = (Screen.height - minigameCameraPixelDimensions.y) / 2;
		
	}
	
	bool HandCursorOutsideMinigameCameraView()
	{
		Vector3 mousePos = Input.mousePosition;
		if (mousePos.x < minigameCameraPixelPosition.x ||
			mousePos.y < minigameCameraPixelPosition.y ||
			mousePos.x > minigameCameraPixelDimensions.x + minigameCameraPixelPosition.x || 
			mousePos.y > minigameCameraPixelDimensions.y + minigameCameraPixelPosition.y)
			return true;
		else
			return false;
	}

    void UpdateCameraAnimation()
    {
        UpdateCameraAnimation(MinigameCamera.transform.position + currentOriginPoint);
    }

    void UpdateCameraAnimation(Vector3 originWorldPoint)
    {
        Vector3 origin = MinigameCamera.WorldToViewportPoint(originWorldPoint);

        currentCameraAnimationTime += .05f;
		
        if (cameraIsClosing)
        {
            //MinigameCamera.rect = new Rect(offsetX, offsetY, Mathf.Lerp(maxWidth, minWidth, cameraAnimationTime), Mathf.Lerp(maxHeight, minHeight, cameraAnimationTime));
            MinigameCamera.rect = new Rect(Mathf.Lerp(offsetX, origin.x, currentCameraAnimationTime), Mathf.Lerp(offsetY, origin.y, currentCameraAnimationTime), Mathf.Lerp(maxWidth, minWidth, currentCameraAnimationTime), Mathf.Lerp(maxHeight, minHeight, currentCameraAnimationTime));
        }
        else //cameraIsOpening
        {
            //MinigameCamera.rect = new Rect(offsetX, offsetY, Mathf.Lerp(minWidth, maxWidth, cameraAnimationTime), Mathf.Lerp(minHeight, maxHeight, cameraAnimationTime));
            if (!MinigameCamera.enabled)
            {
                MinigameCamera.enabled = true;
            }
            MinigameCamera.rect = new Rect(Mathf.Lerp(origin.x, offsetX, currentCameraAnimationTime), Mathf.Lerp(origin.y, offsetY, currentCameraAnimationTime), Mathf.Lerp(minWidth, maxWidth, currentCameraAnimationTime), Mathf.Lerp(minHeight, maxHeight, currentCameraAnimationTime));
        }
        if (currentCameraAnimationTime > cameraAnimationTimeTotal)
        {
            currentCameraAnimationTime = 0;
            cameraAnimating = false;
            if (cameraIsClosing)
            {
                MinigameCamera.enabled = false;
                InMinigame = false;
            }
        }

    }
	
	public void CleanUpMinigameControls()
	{
		CloseCurrentGame();
		CloseCameraImmediate();
		InMinigame = false;
	}
	
	void CloseCameraImmediate()
	{
		if (MinigameCamera != null)
		{
			MinigameCamera.rect = new Rect(0, 0, 0 ,0);
		}
	}
	
	public void SwitchGame(MG_Minigame thatMinigame){
        SwitchGame(thatMinigame, new Vector3());
	}

    public void SwitchGame(MG_Minigame thatMinigame, Vector3 originPoint)
    {
        if (thatMinigame != null)
        {
			if (InMinigame)
			{
				Debug.Log("Minigame currently open. New game not opened.");
				return;
			}
            currentOriginPoint = originPoint;
            if (thatMinigame.CurrentState == MG_Minigame.State.Dormant)
                thatMinigame.StartMinigame();
            
            MinigameCamera.orthographicSize = thatMinigame.orthographicSize * orthographicSizeModification;
            
            Debug.Log(thatMinigame.gameObject.name);
            MinigameCamera.transform.position = GameObject.Find(thatMinigame.gameObject.name + "/CameraLoc").transform.position + new Vector3(0f, heightOffsetY, -190f);
            cameraIsClosing = false;
            cameraAnimating = true;
            InMinigame = true;
			currentGame = thatMinigame;
			thatMinigame.CurrentState = MG_Minigame.State.Active;
        }
        else
        {
            Debug.Log("Minigame is null. Check passed argument.");
        }
    }

    public void CloseCurrentGame()
    {
		if (currentGame != null)
        	currentGame.CurrentState = MG_Minigame.State.Unfocused;
        
        cameraIsClosing = true;
		if (MinigameCamera == null)
			MinigameCamera = transform.FindChild("MinigameCamera").GetComponent<Camera>();
        MinigameCamera.orthographicSize = 100f;
        cameraAnimating = true;
    }

    public bool IsMinigameOpen(MG_Minigame minigame)
    {
        return true;

    }
}
