using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class FFTVisualTools {

    public static void ToggleVisibility(Transform obj, bool state)
    {
        for (int i = 0; i < obj.GetChildCount(); i++)
        {
            if (obj.GetChild(i).GetComponent<Renderer>() != null)
                obj.GetChild(i).GetComponent<Renderer>().enabled = state;

            if (obj.GetChild(i).GetChildCount() > 0)
            {
                ToggleVisibility(obj.GetChild(i), state);
            }
        }
    }

    public static void ToggleSpline(GameObject go, bool state)
    {
        SetSplineFill(go, state);
        SetSplineOutline(go, state);
    }

    public static void SetSplineFill(GameObject go, bool state)
    {
        RageSpline rs;

        if (rs = go.GetComponent("RageSpline") as RageSpline)
        {
            if (state)
            {
                rs.SetFill(RageSpline.Fill.Solid);
            }
            else
            {
                rs.SetFill(RageSpline.Fill.None);
            }
            rs.RefreshMesh(true, false, false);
        }
    }

    public static void SetSplineOutline(GameObject go, bool state)
    {
        RageSpline rs;

        if (rs = go.GetComponent("RageSpline") as RageSpline)
        {
            if (state)
            {
                rs.SetOutline(RageSpline.Outline.Loop);
            }
            else
            {
                rs.SetOutline(RageSpline.Outline.None);
            }
            rs.RefreshMesh(true, false, false);
        }
    }

    public static Color ReturnColorFrom255Values(float r, float g, float b)
    {
        return new Color(r / 255, g / 255, b / 255);
    }
}
