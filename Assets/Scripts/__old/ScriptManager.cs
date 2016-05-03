using UnityEngine;
using System.Collections;

public class ScriptManager : MonoBehaviour
{

    private static ScriptManager instance = null;
    public static ScriptManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("ScriptManager Instantiated.");
                GameObject go = new GameObject();
                instance = go.AddComponent<ScriptManager>();
                go.name = "ZZZ_ScriptUtilities";
                if (GameManager.Instance != null)
                {
                    go.transform.parent = GameManager.Instance.transform;
                }
            }

            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ToggleVisibility(Transform obj, bool state)
    {
        for (int i = 0; i < obj.GetChildCount(); i++)
        {
            if (obj.GetChild(i).GetComponent<Renderer>() != null)
                obj.GetChild(i).GetComponent<Renderer>().enabled = state;

            if (obj.GetChild(i).GetChildCount() > 0)
            {
                ToggleVisibility(obj.GetChild(i), state);
            }
        }
    }
}