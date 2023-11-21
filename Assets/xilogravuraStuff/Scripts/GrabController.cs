using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabController : MonoBehaviour
{
    public static GrabController instance = null;
    private XRGrabInteractable ferramenta;
    private XRSocketInteractor socket;

    public XiloController xiloController;
    public GlassController glassController;

    public GameObject[] ganchos;
    public GameObject marcador;

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
        reposicionarFerramenta();
        atualizarMarcador();
    }

    private void reposicionarFerramenta()
    {
        Rigidbody rb;
        if (ferramenta != null)
        {
            rb = ferramenta.GetComponent<Rigidbody>();
            if (rb != null && rb.useGravity)
            {
                socket.allowHover = true;
                ferramenta.transform.position = socket.transform.position;
            }
        }
    }

    private void atualizarMarcador()
    {
        if (ferramenta == null)
        {
            if (!xiloController.getSketched())
                marcador.transform.position = ganchos[0].transform.position;
            if (xiloController.getSketched())
                marcador.transform.position = ganchos[1].transform.position;
            if (xiloController.getSculped())
                marcador.transform.position = ganchos[2].transform.position;
            if (xiloController.getSanded())
                marcador.transform.position = ganchos[3].transform.position;
            if(glassController.getInkEnable())
                marcador.transform.position = ganchos[4].transform.position;
            if (xiloController.getPaint())
                marcador.transform.position = ganchos[5].transform.position;
        }
    }

    public void setTool(XRGrabInteractable ferramenta)
    {
        this.ferramenta = ferramenta;
        ferramenta.transform.Rotate(Vector3.right, -30f);
        //Debug.LogError("saiu");
    }

    public void setSocket(XRSocketInteractor socket)
    {
        this.socket = socket;
    }

    public void resetTool()
    {
        if(ferramenta != null)
            ferramenta.transform.Rotate(Vector3.right, 30f);
        ferramenta = null;
        socket.allowHover = false;
        socket = null;
        //Debug.LogError("voltou");
    }

    public bool isGrab(XRGrabInteractable ferramenta)
    {
        if (this.ferramenta == null)
            return false; 
        return this.ferramenta.Equals(ferramenta);
    }
}
