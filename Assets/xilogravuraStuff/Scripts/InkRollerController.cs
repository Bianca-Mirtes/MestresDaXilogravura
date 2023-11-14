using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkRollerController : MonoBehaviour
{
    private bool isInk = false;
    public Material borracha;
    // Start is called before the first frame update
    void Start()
    {
        borracha.SetFloat("isTinta", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enableInk()
    {
        isInk = true;
        borracha.SetFloat("isTinta", 1);
    }

    public bool isInkEnable()
    {
        return isInk;
    }
}
