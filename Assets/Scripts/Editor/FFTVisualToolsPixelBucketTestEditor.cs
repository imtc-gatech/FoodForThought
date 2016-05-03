using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FFTVisualToolsPixelBucketTest))]
public class FFTVisualToolsPixelBucketTestEditor : Editor {

    FFTVisualToolsPixelBucketTest PBT;

    public void OnEnable()
    {
        PBT = (FFTVisualToolsPixelBucketTest)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (PBT.resetColorFromMaterial)
        {
            //http://answers.unity3d.com/questions/39703/how-to-access-a-texture-for-reading-on-the-editor.html

            Texture2D texture = PBT.materialToExtractColor.GetTexture("_MainTex") as Texture2D;

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
            ti.isReadable = true;
            AssetDatabase.ImportAsset(path);

            PBT.predominantColor = FFTVisualToolsPixelBucket.PredominantColor(texture);
            PBT.resetColorFromMaterial = false;

            ti.isReadable = false;
            AssetDatabase.ImportAsset(path);
        }
    }

}
