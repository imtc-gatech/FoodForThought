using UnityEngine;
using System.Collections;

public class ObjectHighlightScript : MonoBehaviour {
	private Shader startShader;
	public Shader highlightShader;
	private GameObject gO;
	public Vector3 offset = new Vector3(0f,-.01f,0f);
	public bool useMouseoverHighlighting = true;
	// Use this for initialization
	void Start () {
		startShader = GetComponent<Renderer>().material.shader;
		highlightShader = Shader.Find("GUI/Text Shader");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/*
	void OnMouseDown(){
		renderer.material.shader = startShader;
		GameObject.Destroy(gO);
	}
	void OnMouseUp(){
		GameObject.Destroy(gO);
		gO = Instantiate(gameObject,transform.position,transform.rotation) as GameObject;
		GameObject.Destroy(gO.collider);
		renderer.material.shader = highlightShader;
		Debug.Log("MouseEnter");
	}
	*/
	void OnMouseEnter()
	{
		if(useMouseoverHighlighting)
			SwitchHighlightOn();
	}
	
	void OnMouseExit(){
		if (useMouseoverHighlighting)
			SwitchHighlightOff();
	}
	
	public void SwitchHighlightOn()
	{
		GameObject.Destroy(gO);
		gO = Instantiate(gameObject,transform.position + offset,transform.rotation) as GameObject;
		gO.transform.localScale = transform.localScale * 1.1f;
		if(gO.GetComponent(typeof(Rigidbody)) != null)
		{
			Destroy(gO.GetComponent<Rigidbody>());
		}
		GameObject.Destroy(gO.GetComponent<Collider>());
		gO.transform.parent = transform;
		gO.transform.localPosition = offset;
		gO.GetComponent<Renderer>().material.shader = highlightShader;
	}
	
	public void SwitchHighlightOff()
	{
		//renderer.material.shader = startShader;
		GameObject.Destroy(gO);
	}
}
