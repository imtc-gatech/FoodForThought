using UnityEngine;
using System.Collections;
using System.Reflection;

public class ReflectionTester : MonoBehaviour {

    public Component obj;

	// Use this for initialization
	void Start () {

        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        //const BindingFlags flags = BindingFlags.Public;
        FieldInfo[] fields = obj.GetType().GetFields(flags);
        foreach (FieldInfo fieldInfo in fields)
        {
            Debug.Log("Obj: " + obj.name + ", Field: " + fieldInfo.Name);
        }
        PropertyInfo[] properties = obj.GetType().GetProperties(flags);
        foreach (PropertyInfo propertyInfo in properties)
        {
            Debug.Log("Obj: " + obj.name + ", Property: " + propertyInfo.Name);
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
