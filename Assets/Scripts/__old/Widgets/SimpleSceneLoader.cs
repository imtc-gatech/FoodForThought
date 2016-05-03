using UnityEngine;
using System.Collections;

public class SimpleSceneLoader : MonoBehaviour {

    public Object[] GameScenes;

    void Awake()
    {
        transform.name = "__SceneLoader__";
        DontDestroyOnLoad(transform.gameObject);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        CheckForLevelLoad();
        
	}
    /// <summary>
    /// Loads game level from array of Scene objects using the numerical keys 1-9, depending on array size.
    /// </summary>
    void CheckForLevelLoad()
    {
        if (Input.anyKeyDown)
        {
            int keyNum = 49; //KeyCode.Alpha1 = (int)49

            for (int i = 0; i < GameScenes.Length; i++)
            {
                if (Input.GetKeyDown((KeyCode)keyNum + i))
                {
                    Debug.Log((KeyCode)keyNum + i);
                    if (GameScenes[i] != null)
                        Application.LoadLevel(GameScenes[i].name);
                }

            }

            /* OLD IMPLEMENTATION
            
            int index = 0;
            foreach (Object o in GameScenes)
            {
                if (Input.GetKeyDown((KeyCode)keyNum + index))
                {
                    Debug.Log((KeyCode)keyNum + index);
                    if (GameScenes[index] != null)
                        Application.LoadLevel(GameScenes[index].name);
                }
                index++;
            }
             */
        }
    }
}
