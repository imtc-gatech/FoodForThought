using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(NitcepManager))]

public class NitcepManagerEditor : Editor {

    NitcepManager Manager;

    GUIStyle style;
    Texture2D texture;

    public void OnEnable()
    {
        Manager = (NitcepManager)target;

        


    }
    

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New"))
        {
            Manager.SoundSlots.Add(new NitcepSoundProperties());
        }
        if (GUILayout.Button("Sort"))
        {
            Manager.SoundSlots = SortSoundPropertyList(Manager.SoundSlots);
        }
        GUILayout.EndHorizontal();

        int propertyIndex = 0;
        int indexToDestroy = -1;
        int loadedSoundCount = 0;
        bool shouldRebuildLoadedSounds = false;

        //bool colorFlipper = true;

        foreach (NitcepSoundProperties Property in Manager.SoundSlots)
        {
            /*
            if (colorFlipper)
            {
                SetStyleBackGround(Color.white);
                GUI.contentColor = Color.black;
                colorFlipper = false;
            }
            else
            {
                SetStyleBackGround(Color.black);
                GUI.contentColor = Color.white;
                colorFlipper = true;
            }
            */

            GUILayout.BeginHorizontal(GUILayout.Height(3));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (Property.Name != "(empty)")
            {
                GUILayout.BeginHorizontal();
                Property.Name = EditorGUILayout.TextField("Label:", Property.Name);
                GUILayout.EndHorizontal();
            }
            
            GUILayout.BeginHorizontal();
            Property.Type = (NitcepSound.Type)EditorGUILayout.EnumPopup(Property.Type);
            Property.SourceFile = EditorGUILayout.ObjectField(Property.SourceFile, typeof(AudioClip), true) as AudioClip;
            GUILayout.EndHorizontal();

            if (Property.Name != "(empty)")
            {
                GUILayout.BeginHorizontal();
                Property.Destroy = GUILayout.Button("Remove");
                GUILayout.EndHorizontal();
            }

            if (Property.Loaded)
            {
                loadedSoundCount++;
            }

            if (Property.Destroy)
            {
                indexToDestroy = propertyIndex;
            }

            if (Property.Changed)
            {
                shouldRebuildLoadedSounds = true;
                Property.Changed = false;
            }
            
            propertyIndex++;
        }

        if (loadedSoundCount != Manager.LoadedSounds.Count)
        {
            shouldRebuildLoadedSounds = true;
        }

        if (indexToDestroy != -1)
        {
            Manager.SoundSlots.RemoveAt(indexToDestroy);
        }

        if (shouldRebuildLoadedSounds)
        {
            //RebuildLoadedSoundsList();
        }

        //GUI.contentColor = Color.white;

        GUILayout.BeginHorizontal(GUILayout.Height(3));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(Manager);
        }

        base.OnInspectorGUI();
    }

    
    /*
    void RebuildLoadedSoundsList()
    {
        List<NitcepSoundProperties> newLoadedSounds = new List<NitcepSoundProperties>();

        foreach (NitcepSoundProperties Property in Manager.SoundSlots)
        {
            if (Property.Loaded)
            {
                newLoadedSounds.Add(Property);
            }
        }

        //List<Order> SortedList = objListOrder.OrderBy(o=>o.OrderDate).ToList();

        Manager.LoadedSounds = SortSoundPropertyList(newLoadedSounds);

    }
     * */

    List<NitcepSoundProperties> SortSoundPropertyList(List<NitcepSoundProperties> propertyList)
    {
        return propertyList.OrderBy(o => o.Name).ToList();
    }

    void SetStyleBackGround(Color color)
    {
        style = new GUIStyle();
        texture = new Texture2D(128, 128);

        for (int y = 0; y < texture.height; ++y)
        {
            for (int x = 0; x < texture.width; ++x)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        style.normal.background = texture;
    }

    
}
