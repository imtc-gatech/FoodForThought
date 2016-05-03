using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameShellExample : MiniGameShell {

    public new Dictionary<string, float> FloatValues;
    
    public new Dictionary<string, int> IntValues;
    
    public new Dictionary<string, string> StringValues;
    
    public new Dictionary<string, bool> BoolValues;

    // Place tweakable variables here.

    public float floatVariable
    {
        get
        {
            //return floatVariable location
            return FloatValues["floatVariable"];
        }
        set
        {
            //set variable location
            FloatValues["floatVariable"] = value;
        }
    }

    void Awake()
    {
        InitializeValues();
    }

    public void InitializeValues()
    {
        FloatValues = new Dictionary<string, float>();
        IntValues = new Dictionary<string, int>();
        StringValues = new Dictionary<string, string>();
        BoolValues = new Dictionary<string, bool>();

        // assign initial values to your variables here
        // you must create an entry for each of the variables you want to use above with default values!
        // these can later be changed through the getters and setters above

        FloatValues.Add("floatVariable", 15.0f);

    }


}
