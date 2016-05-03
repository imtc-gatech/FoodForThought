using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(NitcepSound))]

public class NitcepSoundEditor : Editor {

    NitcepSound Sound;
    string[] SoundLabels;
    int CurrentSoundIndex
    {
        get
        {
            return _currentSoundIndex;
        }
        set
        {
            if (_currentSoundIndex != value)
            {
                Sound.Properties = NitcepManager.Instance.LoadedSounds[value];
                _currentSoundIndex = value;
                Sound.ResetAudioSource();
            }
            
        }
    }

    private int _currentSoundIndex;
    private int soundAssetCount;

    public void OnEnable()
    {
        Sound = (NitcepSound)target;
    }

    public override void OnInspectorGUI()
    {
        if (NitcepManager.Instance.LoadedSounds.Count == 0)
        {
            GUILayout.BeginHorizontal();
            string IntroText = "Add sounds to the SoundManager to enable this script.\n";
            /*
            IntroText += "To clear the current food prefab, select 'Clear'.\n";
            IntroText += "\n";
            IntroText += "Questions? rob@imtc.gatech.edu\n";
             */
            EditorGUILayout.TextArea(IntroText);
            GUILayout.EndHorizontal();

            DestroyImmediate(Sound.Source);

            return;
        }

        Sound.Properties = NitcepManager.Instance.LoadedSounds[CurrentSoundIndex];

        if (!NitcepManager.Instance.ContainsSoundInLoadedSoundList(Sound.Properties.SourceFile))
        {
            Sound.Properties.SourceFile = null;
        }

        int currentSoundAssetCount = NitcepManager.Instance.LoadedSounds.Count;

        SoundLabels = new string[currentSoundAssetCount];

        for (int i = 0; i < currentSoundAssetCount; i++)
        {
            SoundLabels[i] = NitcepManager.Instance.LoadedSounds[i].Name;
        }

        if (currentSoundAssetCount != soundAssetCount)
        {
            string CurrentSoundName = Sound.Properties.Name;

            for (int i = 0; i < currentSoundAssetCount; i++)
            {
                if (CurrentSoundName == SoundLabels[i])
                {
                    _currentSoundIndex = i;
                }
            }
        }

        if (SoundLabels[CurrentSoundIndex] != Sound.Properties.Name)
        {
            Sound.Properties = NitcepManager.Instance.LoadedSounds[CurrentSoundIndex];

        }


        GUILayout.BeginHorizontal();
        CurrentSoundIndex = EditorGUILayout.Popup("Name:", CurrentSoundIndex, SoundLabels);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Type:");
        GUILayout.Label(Sound.Properties.Type.ToString());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (Sound.Source == null)
        {
            if (GUILayout.Button("Load Sound"))
            {
                Sound.ResetAudioSource();
            }
            GUILayout.Button("--------------");
        }
        else
        {
            if (GUILayout.Button("Reload Sound"))
            {
                Sound.ResetAudioSource();
            }
            if (GUILayout.Button("Reset Parameters"))
            {
                Sound.ResetAudioSourceParameters(Sound.Source);
            }
        }
        

        if (Sound.Source != null)
        {
            if (Sound.Source.isPlaying)
            {
                if (GUILayout.Button("Stop"))
                {
                    Sound.Source.Stop();
                    EditorUtility.SetDirty(Sound);
                }
            }
            else
            {
                if (GUILayout.Button("Play"))
                {
                    Sound.Source.Play();
                    EditorUtility.SetDirty(Sound);
                    
                }
            }
        }
        else
        {
            GUILayout.Button("----");
        }



        GUILayout.EndHorizontal();

        soundAssetCount = NitcepManager.Instance.LoadedSounds.Count;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(Sound);
        }

        //base.OnInspectorGUI();
    }
	
}
