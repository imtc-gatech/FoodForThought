using UnityEngine;
using System.Collections;

public class NitcepSound : MonoBehaviour {

    public enum Type
    {
        None = 0,
        BGM_Score = 1,
        SFX_OneShot = 2,
        SFX_Loop = 3
    }

    public NitcepSoundProperties Properties;

    public AudioSource Source;

	// Use this for initialization
	void Awake () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PreviewPlay()
    {
        
        Source.Play();
         
        //AudioSource.PlayClipAtPoint(Source.clip, gameObject.transform.position, Source.volume);

    }

    public void ResetAudioSource()
    {
        if (Source == null)
        {
            gameObject.AddComponent<AudioSource>();
            Source = gameObject.GetComponent<AudioSource>();
            ResetAudioSourceParameters(Source);
        }

        Source.clip = Properties.SourceFile;

        ApplySpecificFlagsToSoundBasedOnType();

    }

    public void ResetAudioSourceParameters(AudioSource source)
    {
        if (source == null)
        {
            return;
        }
        
        source.mute = false;
        source.bypassEffects = false;
        source.playOnAwake = false;
        source.loop = false;
        source.priority = 128;
        source.volume = 1;
        source.pitch = 1;
        source.panStereo = 0.0f;

        ApplySpecificFlagsToSoundBasedOnType();
    }

    private void ApplySpecificFlagsToSoundBasedOnType()
    {
        switch (Properties.Type)
        {
            case Type.SFX_Loop:
                Source.loop = true;
                break;
            default:
                Source.loop = false;
                break;
        }
    }
}
