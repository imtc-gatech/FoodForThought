using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MG_DictionaryBase : MonoBehaviour {

	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public Dictionary<string,string> getDictionary(){
		Dictionary<string,string> rootHolder = new Dictionary<string, string>();
		return rootHolder;	
		
	}
	
	public void SetDictionary(Dictionary<string,string> inputDictionary){

	}
	
	public static Color ParseColor(string s) {
		s = s.Remove(0,5);
		s = s.Remove(24);
		print(s);
		string[] chars = s.Split(',');
		Color c = new Color(0f,0f,0f, 0f);
		c.r = float.Parse(chars[0]);
		c.g = float.Parse(chars[1]);
		c.b = float.Parse(chars[2]);
		c.a = float.Parse(chars[3]);
		return c;
	}
	
	public static int ParseInt(string s){
		return int.Parse(s);
	}
	
	public static Material ParseMaterial(string s){
		return new Material(s);
	}
	
	public static GameObject ParsePrefab(string s){
		return new GameObject(s);
	}
	
	
}
