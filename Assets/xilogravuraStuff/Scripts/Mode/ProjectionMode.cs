using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionMode : ExperienceMode
{
    public int paintAngle = 350;
    public Transform tool;

    public void Start()
    {
        mode = Mode.PROJECTION;
    }

    public override void getMode()
    {
        Debug.Log("MODE: " + mode);
    }

    public override bool condicaoDePintura()
    {
        return tool.transform.eulerAngles.x <= paintAngle && tool.transform.eulerAngles.x > 100;
    }
}
