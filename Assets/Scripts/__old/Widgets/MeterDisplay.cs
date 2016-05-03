using UnityEngine;
using System.Collections;

public abstract class MeterDisplay : MonoBehaviour {

    public abstract void RefreshDisplay();
    public abstract void ResetDisplay();
}
