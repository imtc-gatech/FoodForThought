using UnityEngine;
using System.Collections;

public class FFTMinigameIcon : MonoBehaviour {

    public MG_Minigame.Type State
    {
        get { return _state; }
        set
        {
            if (value != _state)
            {
                _state = value;
                Refresh();
            }
        }
    }
    [SerializeField]
    private MG_Minigame.Type _state = MG_Minigame.Type.Sorting;

    public Transform[] transforms;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Refresh()
    {
        if (transforms == null)
            return;
        foreach (Transform parent in transforms)
        {
            foreach (Transform t in parent)
            {
                if (t.gameObject.name == CurrentMinigameName())
                {
                    t.gameObject.SetActiveRecursively(true);
                }
                else
                {
                    t.gameObject.SetActiveRecursively(false);
                }
            }
        }
        
    }

    Transform CurrentMinigameTransform(Transform parentTransform)
    {
        string name = CurrentMinigameName();

        Transform displayIconTile = parentTransform.FindChild(name);

        return displayIconTile;
    }

    string CurrentMinigameName()
    {
        switch (_state)
        {
            case MG_Minigame.Type.Blending:
                return "blending";
            case MG_Minigame.Type.Chopping:
                return "slicing";
            case MG_Minigame.Type.Sorting:
                return "sorting";
            case MG_Minigame.Type.Spicing:
                return "flavoring";
            case MG_Minigame.Type.Stirring:
                return "stirring";
            default:
                return "";
        }

    }

    void OnDestroy()
    {
        FFTUtilities.DestroySafe(gameObject);
    }
}
