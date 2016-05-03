using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// Helpful Unity Utilities For "Food For Thought"
/// </summary>
public static class FFTUtilities : System.Object
{
    /// <summary>
    /// Returns "true" if Unity is currently running in Editor Mode (OSX/Windows)
    /// </summary>
    public static bool InEditorMode
    {
        get
        {
            //return (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor);

            return (Application.isEditor && !Application.isPlaying);
        }
    }

    /// <summary>
    /// Safe destruction of objects that may be destroyed either in the Editor or during Runtime.
    /// Calls Object.DestroyImmediate if in the Editor. Calls Object.Destroy if during Runtime.
    /// </summary>
    /// <param name="obj">Unity Object to Destroy</param>
    public static void DestroySafe(Object obj)
    {
		if (obj == null)
			return;
		
        // Debug.Log("DestroySafe: " + obj.name);

        if (FFTUtilities.InEditorMode)
        {
            //Debug.Log(obj.name + " destroyed immediately in Editor.");
            Object.DestroyImmediate(obj);
        }
        else
        {
            //Debug.Log(obj.name + " destroyed in Game.");
            Object.Destroy(obj);
        }
    }

    public static void OffsetGameObjectPosition(GameObject go, float x = 0, float y = 0, float z = 0)
    {
        Vector3 newOffset = new Vector3(x, y, z);
        Vector3 goPosition = go.transform.localPosition;
        go.transform.localPosition = goPosition + newOffset;
    }
	
}
