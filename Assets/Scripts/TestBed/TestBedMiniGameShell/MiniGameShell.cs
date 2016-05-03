using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameShell : MonoBehaviour{

    public Dictionary<string, float> FloatValues;
    public Dictionary<string, int> IntValues;
    public Dictionary<string, string> StringValues;
    public Dictionary<string, bool> BoolValues;

    void Awake()
    {
        FloatValues = new Dictionary<string, float>();
        IntValues = new Dictionary<string, int>();
        StringValues = new Dictionary<string, string>();
        BoolValues = new Dictionary<string, bool>();
    }

    /*
    public List<KeyValuePair<string, float>> FloatValues;
    public List<KeyValuePair<string, int>> IntValues;
    public List<KeyValuePair<string, string>> StringValues;
    public List<KeyValuePair<string, bool>> BoolValues;

    void Awake()
    {
        FloatValues = new List<KeyValuePair<string, float>>();
        IntValues = new List<KeyValuePair<string, int>>();
        StringValues = new List<KeyValuePair<string, string>>();
        BoolValues = new List<KeyValuePair<string, bool>>();
    }
     */

}
