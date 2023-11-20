using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabController : MonoBehaviour
{
    public static GrabController instance = null;
    private XRGrabInteractable ferramenta;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setTool(XRGrabInteractable ferramenta){
        this.ferramenta = ferramenta;
        ferramenta.transform.Rotate(Vector3.right, -30f);
        //Debug.LogError("saiu");
    }

    public void resetTool()
    {
        if(this.ferramenta != null)
            this.ferramenta.transform.Rotate(Vector3.right, 30f);
        this.ferramenta = null;
        //Debug.LogError("voltou");
    }

    public bool isGrab(XRGrabInteractable ferramenta)
    {
        if (this.ferramenta == null)
            return false; 
        return this.ferramenta.Equals(ferramenta);
    }
}
