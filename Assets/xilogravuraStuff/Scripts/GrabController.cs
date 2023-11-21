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
    public GameObject tutorial;
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
                if (ferramenta.gameObject.GetComponent<AudioSource>().isPlaying)
                {
                    ferramenta.gameObject.GetComponent<AudioSource>().Stop();
                    xiloController.setVerifSound(true);
                }
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
            {
                tutorial.transform.GetChild(0).gameObject.SetActive(true);
                marcador.transform.position = ganchos[0].transform.position;
            }
                
            if (xiloController.getSketched())
            {
                tutorial.transform.GetChild(1).gameObject.SetActive(true);
                marcador.transform.position = ganchos[1].transform.position;
            }
                
            if (xiloController.getSculped())
            {
                tutorial.transform.GetChild(2).gameObject.SetActive(true);
                marcador.transform.position = ganchos[2].transform.position;
            }
                
            if (xiloController.getSanded())
            {
                tutorial.transform.GetChild(3).gameObject.SetActive(true);
                marcador.transform.position = ganchos[3].transform.position;
            }

            if (glassController.getInkEnable())
            {
                tutorial.transform.GetChild(0).gameObject.SetActive(false);
                tutorial.transform.GetChild(1).gameObject.SetActive(false);
                tutorial.transform.GetChild(2).gameObject.SetActive(false);
                tutorial.transform.GetChild(3).gameObject.SetActive(false);
                tutorial.transform.GetChild(4).gameObject.SetActive(true);
                marcador.transform.position = ganchos[4].transform.position;
            }

            if (xiloController.getPaint()) 
            {
                tutorial.transform.GetChild(4).gameObject.SetActive(false);
                marcador.transform.position = ganchos[5].transform.position;
            }
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
        //if (socket != null)
        //    socket.allowHover = false;
        //socket = null;
        //Debug.LogError("voltou");
    }

    public bool isGrab(XRGrabInteractable ferramenta)
    {
        if (this.ferramenta == null)
            return false; 
        return this.ferramenta.Equals(ferramenta);
    }
}
