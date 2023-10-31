using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkRollerController : MonoBehaviour
{
    private bool isInk = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkRegion()
    {
        isInk = true;
    }

    public bool isInkEnable()
    {
        return isInk;
    }
}
