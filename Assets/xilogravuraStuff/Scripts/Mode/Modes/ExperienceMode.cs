using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public enum Mode { VR, PROJECTION }

public abstract class ExperienceMode : MonoBehaviour
{
    public Mode mode;

    public abstract void getMode();
    public abstract bool condicaoDePintura();
}
