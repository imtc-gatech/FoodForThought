using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NitcepSoundProperties {

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            if (_name != value)
            {
                Changed = true;
                _name = value;
            }
        }
    }

    public AudioClip SourceFile
    {
        get
        {
            return _sourceFile;
        }
        set
        {
            if (_sourceFile != value)
            {
                Changed = true;

                if (value == null)
                {
                    _loaded = false;
                    Name = "(empty)";
                }
                else
                {
                    if (_sourceFile == null)
                    {
                        _loaded = true;
                    }

                    if (Name == "(empty)")
                    {
                        Name = value.ToString().Substring(0, value.ToString().IndexOf('('));
                    }
                }

                _sourceFile = value;
            }
        }

    }

    public bool Loaded
    {
        get
        {
            return _loaded;
        }
    }

    public bool Changed = false;

    public bool Destroy = false;

    public NitcepSound.Type Type = NitcepSound.Type.SFX_OneShot;

    private AudioClip _sourceFile;
    private string _name = "(empty)";
    private bool _loaded;
    private bool _changed;

    public NitcepSoundProperties()
    {

    }

    public void Preview()
    {

    }

}
