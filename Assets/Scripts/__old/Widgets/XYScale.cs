using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class XYScale : MonoBehaviour {

    private float minimumScale = 0.001f;

    public float scale = 1.0f;

    private float lastScale;

	// Use this for initialization
	void Start () {
        lastScale = scale;
        /*
        if (gameObject.GetComponent("RagePivotools") == null)
        {
            gameObject.AddComponent("RagePivotools");
        }
        gameObject.GetComponent<RagePivotools>().CenterPivot();
         * */
	
	}
	
	// Update is called once per frame
	void Update () {
        

        if (lastScale != scale)
        {
            if (scale < minimumScale)
            {
                scale = minimumScale;
            }

            //change the X+Y scale of the GameObject
            gameObject.transform.localScale = new Vector3(scale, scale, 1);

        }

        lastScale = scale;

	
	}
}
