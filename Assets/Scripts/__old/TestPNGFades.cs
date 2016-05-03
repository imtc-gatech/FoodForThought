using UnityEngine;
using System.Collections;

public class TestPNGFades : MonoBehaviour {

    public GaugeParameters Parameters;

    public FoodState Burned;
    public FoodState Cooked;
    public FoodState Spoiled;
    public FoodState Uncooked;

    public float CookingTime = 0.0f;
    public float Freshness = 100.0f;

	// Use this for initialization
	void Awake () {
        AssignMeshRendererToFoodState(Burned, "burned");
        AssignMeshRendererToFoodState(Cooked, "cooked");
        AssignMeshRendererToFoodState(Spoiled, "spoiled");
        AssignMeshRendererToFoodState(Uncooked, "uncooked");
        Spoiled.Opacity = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        AdjustOpacityFromParameters();
        
	}

    void AdjustOpacityFromParameters()
    {
        Spoiled.Opacity = 1 - (Freshness / 100.0f);

        float cookedTolerance = Parameters.Unfinished + (0.5f * Parameters.Finished);
        float burnedTolerance = Parameters.Unfinished + Parameters.Finished;

        if (CookingTime < cookedTolerance)
        {
            Uncooked.Opacity = (cookedTolerance - CookingTime) / cookedTolerance;
        }
        else if (CookingTime > burnedTolerance)
        {
            Uncooked.Opacity = 0.0f;
            Cooked.Opacity = 1.0f - ((CookingTime - burnedTolerance) / (100.0f - burnedTolerance));
        }


        ChangeOpacity(Burned);
        ChangeOpacity(Cooked);
        ChangeOpacity(Spoiled);
        ChangeOpacity(Uncooked);
    }

    void ChangeOpacity(FoodState foodState)
    {
        foodState.Renderer.sharedMaterial.color = new Color(1, 1, 1, foodState.Opacity);
    }

    void AssignMeshRendererToFoodState(FoodState foodStateVariable, string name)
    {
        foodStateVariable.Renderer = transform.FindChild(name).GetComponent<MeshRenderer>();

        /*
        if (transform.FindChild(name) != null)
        {
            
            return true;
        }
        else
        {
            return false;
        }
        */
    }
}
