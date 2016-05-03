using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NitcepManager : MonoBehaviour {

    private static NitcepManager instance = null;
    public static NitcepManager Instance
    {
        get
        {
            if (instance == null)
                instance = (NitcepManager)FindObjectOfType(typeof(NitcepManager));
            return instance;
        }
    }

    public List<NitcepSoundProperties> SoundSlots;
    public List<NitcepSoundProperties> LoadedSounds
    {
        get
        {
            List<NitcepSoundProperties> loadedSoundBuilder = new List<NitcepSoundProperties>();
            foreach (NitcepSoundProperties nsp in SoundSlots)
            {
                if (nsp.Loaded)
                    loadedSoundBuilder.Add(nsp);
            }

            return loadedSoundBuilder.OrderBy(o => o.Name).ToList();

        }
    }


	// Use this for initialization
	void Awake () {
        instance = this;
        SoundSlots = new List<NitcepSoundProperties>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool ContainsSoundInLoadedSoundList(AudioClip clip)
    {
        foreach (NitcepSoundProperties nsp in LoadedSounds)
        {
            if (nsp.SourceFile.name == clip.name)
            {
                return true;
            }
        }

        return false;

    }
}
