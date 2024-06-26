using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionMode : ExperienceMode
{
    public int paintAngle = 350;
    private Transform tool = null;

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
        //print(tool.transform.eulerAngles.x);
        if(tool == null)
            return false;
        return tool.transform.eulerAngles.x <= paintAngle && tool.transform.eulerAngles.x > 100;
    }

    public void setTool(Transform tool){
        this.tool = tool;
    }

    public void resetTool()
    {
        //if(tool != null){
        //    tool.transform.eulerAngles = Vector3.zero;
        //    tool.transform.position = Vector3.zero;
        //}
            
        tool = null;
    }

    public bool checkTool(GameObject toolCheck)
    {
        return tool.name.Equals(toolCheck.name);
    }
}
