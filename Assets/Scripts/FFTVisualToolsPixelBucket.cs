using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class FFTVisualToolsPixelBucket : System.Object {

    public List<Color> Pixels;
    public ColorName Name;

    public enum ColorName
    {
        Black=0,
        Red=1,
        Green=2,
        Blue=3,
        Cyan=4,
        Magenta=5,
        Yellow=6,
        White=7
    }

    public FFTVisualToolsPixelBucket(ColorName name)
    {
        Pixels = new List<Color>();
        Name = name;
    }

    public static Color PredominantColor(Texture2D texture)
    {
        float redThreshold = 0.5f;
        float greenThreshold = 0.5f;
        float blueThreshold = 0.5f;
        //float whiteThreshold = 0.8f;
        //float blackThreshold = 0.2f;

        int colorSampleFrequency = 8;

        //http://robrich.org/archive/2010/11/18/Get-all-Enum-values-as-a-ListMyEnumType.aspx

        System.Type type = typeof(FFTVisualToolsPixelBucket.ColorName);
        System.Array values = System.Enum.GetValues(type);
        List<FFTVisualToolsPixelBucket.ColorName> names = new List<FFTVisualToolsPixelBucket.ColorName>();
        foreach (System.Object o in values)
        {
            names.Add((FFTVisualToolsPixelBucket.ColorName)o);
        }

        List<FFTVisualToolsPixelBucket> pixelBuckets = new List<FFTVisualToolsPixelBucket>();

        foreach (FFTVisualToolsPixelBucket.ColorName name in names)
        {
            pixelBuckets.Add(new FFTVisualToolsPixelBucket(name));
        }

        /*
        
        //http://answers.unity3d.com/questions/39703/how-to-access-a-texture-for-reading-on-the-editor.html
        
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
        ti.isReadable = true;
        AssetDatabase.ImportAsset(path);
        ti.isReadable = false;
        AssetDatabase.ImportAsset(path);
        */
        Color[] textureColors = texture.GetPixels();

        for (int i = 0; i < textureColors.Length; i = i + colorSampleFrequency)
        {

            Color texColor = textureColors[i];

            bool redFlag = texColor.r > redThreshold;
            bool blueFlag = texColor.b > blueThreshold;
            bool greenFlag = texColor.g > greenThreshold;
            bool whiteFlag = redFlag && blueFlag && greenFlag;

            if (whiteFlag)
                pixelBuckets[(int)ColorName.White].Pixels.Add(texColor);
            else if (redFlag)
            {
                if (greenFlag)
                    pixelBuckets[(int)ColorName.Yellow].Pixels.Add(texColor);
                else if (blueFlag)
                    pixelBuckets[(int)ColorName.Magenta].Pixels.Add(texColor);
                else
                    pixelBuckets[(int)ColorName.Red].Pixels.Add(texColor);
            }
            else if (blueFlag)
            {
                if (greenFlag)
                    pixelBuckets[(int)ColorName.Cyan].Pixels.Add(texColor);
                else
                    pixelBuckets[(int)ColorName.Blue].Pixels.Add(texColor);
            }
            else if (greenFlag)
            {
                pixelBuckets[(int)ColorName.Green].Pixels.Add(texColor);
            }
            else
            {
                pixelBuckets[(int)ColorName.Black].Pixels.Add(texColor);
            }
        }

        ColorName bucketNameToAverage = IdentifyPredominantBucket(pixelBuckets);

        List<Color> pixelsToAverage = pixelBuckets[(int)bucketNameToAverage].Pixels;

        float r = 0;
        float g = 0;
        float b = 0;

        float rTotal = 0;
        float gTotal = 0;
        float bTotal = 0;

        foreach (Color c in pixelsToAverage)
        {
            rTotal += c.r;
            gTotal += c.g;
            bTotal += c.b;
        }

        r = rTotal / pixelsToAverage.Count;
        g = gTotal / pixelsToAverage.Count;
        b = bTotal / pixelsToAverage.Count;

        Color color = new Color(r, g, b);
        return color;
    }

    static ColorName IdentifyPredominantBucket(List<FFTVisualToolsPixelBucket> pixelBuckets)
    {
        int highestCount = 0;
        FFTVisualToolsPixelBucket highestBucket = new FFTVisualToolsPixelBucket(ColorName.Black);
        foreach (FFTVisualToolsPixelBucket bucket in pixelBuckets)
        {
            // ignore black and white
            if (bucket.Name == ColorName.Black || bucket.Name == ColorName.White)
                continue;

            if (bucket.Pixels.Count > highestCount)
            {
                highestCount = bucket.Pixels.Count;
                highestBucket = bucket;
            }
        }
        if (highestBucket.Name == ColorName.Black)
            Debug.Log("ERROR (FFTVisualToolsPixelBucket.IdentifyPredominantBucket): No color count was found. Check thresholds.");
        
        return highestBucket.Name;
    }

}
