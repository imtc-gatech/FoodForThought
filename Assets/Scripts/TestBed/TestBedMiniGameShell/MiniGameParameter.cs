using UnityEngine;
using System.Collections;
using System.Reflection;

[System.Serializable]
public class MiniGameParameter {

    public System.Type Type
    {
        get
        {
            return _type;
        }
    }

    public string Title
    {
        get
        {
            return _title;
        }
    }

    private System.Type _type;

    private float _float = 0;
    private int _int = 0;
    private bool _bool = false;
    private string _string = "";
    private string _title = "";

    

    public MiniGameParameter(string title, string value)
    {
        _title = title;
        _string = value;
        _type = value.GetType();
    }

    public MiniGameParameter(string title, float value, float min, float max)
    {

    }


    public MiniGameParameter(string title, float value)
    {
        _title = title;
        _float = value;
        _type = value.GetType();
    }

    public MiniGameParameter(string title, int value)
    {
        _title = title;
        _int = value;
        _type = value.GetType();
    }

    public MiniGameParameter(string title, bool value)
    {
        _title = title;
        _bool = value;
        _type = value.GetType();
    }

	
}
