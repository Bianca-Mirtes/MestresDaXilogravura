using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TouchController : MonoBehaviour
{
    private List<UnityEngine.XR.InputDevice> devicesRight = new List<UnityEngine.XR.InputDevice>();
    private List<UnityEngine.XR.InputDevice> devicesLeft = new List<UnityEngine.XR.InputDevice>();

    public bool IsClickedWithRightHand()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devicesRight);
        InputDevice targetDevice = devicesRight[0];
        return targetDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isTriggerClicked) && isTriggerClicked;
    }

    public bool IsClickedWithLeftHand()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devicesLeft);
        InputDevice targetDevice = devicesLeft[0];
        return targetDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isTriggerClicked) && isTriggerClicked;
    }
}
