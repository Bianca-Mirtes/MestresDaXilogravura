using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRMode : ExperienceMode
{
    [Header("Atributos do VR")]
    [SerializeField] private TouchController touch;
    [SerializeField] private LineRenderer rayRightHand;
    [SerializeField] private LineRenderer rayLeftHand;

    public void Start()
    {
        mode = Mode.VR;
    }

    public override bool condicaoDePintura()
    {
        return (touch.IsClickedWithRightHand() || touch.IsClickedWithLeftHand());
    }

    public override void getMode()
    {
        Debug.Log("MODE: " + mode);
    }
}
