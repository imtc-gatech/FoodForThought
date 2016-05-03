using UnityEngine;
using System.Collections;

public class FFTCameraManager : MonoBehaviour {

    public static float xSpacing = 400f;
    public static float ySpacing = 200f;

    public static Vector3 homePos = new Vector3(0, 1, -400);

    

    /// <summary>
    /// Type to describe primary screen positions for the main camera.
    /// </summary>
    public enum Target
    {
        Home=0,
        Results=1,
        RecipeCard=2,
        MiniGame1=3,
        MiniGame2=4
    }

    /// <summary>
    /// Moves the camera according to the area it should be positioned.
    /// </summary>
    /// <remarks>TODO: Implement easing/polish functions where appropriate.</remarks>
    /// <param name="target">Target for the camera to be moved to.</param>
    public static void MoveCamera(FFTCameraManager.Target target)
    {
        //Positions from Grid:
        //0, 0 = home position
        //0, 1 = Results Screen (below main game)
        //1, 0 = RecipeCard.

        switch (target)
        {
            case FFTCameraManager.Target.RecipeCard:
                MoveCameraHelper(PositionFromGrid(1, 0));
                break;
            case FFTCameraManager.Target.Home:
                MoveCameraHelper(PositionFromGrid(0, 0));
                break;
            case FFTCameraManager.Target.Results:
                MoveCameraHelper(PositionFromGrid(0, 1));
                break;
        }

    }

    public static void MoveCameraHelper(Vector3 position)
    {
        float shadeXPositionOffScreen = FFTWindowShade.X_POSITION_OFF_SCREEN;

        Camera.main.transform.position = position;
        FFTWindowShade shade = Camera.main.gameObject.GetComponent<FFTWindowShade>();

        if (shade != null)
        {
            float shadePosX = shade.gameObject.transform.localPosition.x;
            if (shadePosX < shadeXPositionOffScreen / 2)
                shadePosX = 0;
            else
                shadePosX = shadeXPositionOffScreen;
            
            shade.gameObject.transform.localPosition = new Vector3(shadeXPositionOffScreen, 0, 20);
        }
    }

    public static Vector3 PositionFromGrid(int x, int y)
    {
        return new Vector3(homePos.x + (xSpacing * x), homePos.y + (-ySpacing * y), homePos.z);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
