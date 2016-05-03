using UnityEngine;
using System.Collections;

public class SimpleMouseInteraction : MonoBehaviour {

    private Alpha a;
    private BGSwitch bgSwitch;
    private bool selected;
    private Vector3 positionOffset;


	// Use this for initialization
	void Awake () {

        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }

        if (gameObject.GetComponent("Alpha") == null)
        {
            gameObject.AddComponent<Alpha>();
        }

        if (gameObject.GetComponent("BGSwitch") == null)
        {
            gameObject.AddComponent<BGSwitch>();
        }

        a = GetComponent<Alpha>();
        bgSwitch = GetComponent<BGSwitch>();
	
	}
	
	// Update is called once per frame
	void Update () {
        if (selected)
        {
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;
            transform.position = mousePosition + positionOffset;

        }
	
	}

    void OnMouseEnter()
    {
        a.alpha = 0.5f;
    }

    void OnMouseDown()
    {
        selected = !selected;
        positionOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionOffset.z = 0;
        
    }

    void OnMouseExit()
    {
        a.alpha = 1.0f;
    }
}
