using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

public class PaperController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Material currentMaterial;
    private RaycastHit hit;

    private int[] dimensions = { 2048, 2048 };

    public Painter painter;
    public GrabController grabController;

    public XRGrabInteractable ferramenta;
    public XiloController xilogravura;

    private bool setarTexturas = false;
    private bool isPrint = false;
    private bool resultado = false;
    private Dictionary<string, RenderTexture> textureDictionary = new Dictionary<string, RenderTexture>();
    private string[] textureNames = { "PrintMask" };
    
    void Start()
    {
        currentMaterial = GetComponent<MeshRenderer>().materials[0];
        setTextures();
    }

    public void resetTextures()
    {
        for (int i = 0; i < textureNames.Length; i++)
        {
            Graphics.SetRenderTarget(textureDictionary[textureNames[i]]);
            GL.Clear(true, true, Color.black);
        }
    }

    public void setTextures()
    {
        textureDictionary.Clear();
        for (int i = 0; i < textureNames.Length; i++)
        {
            textureDictionary[textureNames[i]] = new RenderTexture(dimensions[0], dimensions[1], 0, RenderTextureFormat.ARGBFloat);
            Graphics.SetRenderTarget(textureDictionary[textureNames[i]]);
            GL.Clear(true, true, Color.black);
            Graphics.SetRenderTarget(null);
            currentMaterial.SetTexture(textureNames[i], textureDictionary[textureNames[i]]);
        }
    }

    public void Draw()
    {
        int layerMask = 1 << 12; //Fix layer
        if (grabController.isGrab(ferramenta) && !resultado)
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(ferramenta));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                xilogravura.initSound(ferramenta.gameObject);
                if (!setarTexturas)
                {
                    painter.SetBrushPreset(2);
                    currentMaterial.SetTexture("SketchMask", xilogravura.getTexture("SketchMask"));
                    currentMaterial.SetTexture("SculptMask", xilogravura.getTexture("SculptMask"));
                    currentMaterial.SetTexture("PaintMask", xilogravura.getTexture("PaintMask"));
                    print("texturas setadas");
                    setarTexturas = true;
                }
                //print("imprimindo");
                RenderTexture mask = textureDictionary["PrintMask"];
                //painter.SetBrush(10f);
                painter.PaintMask(mask, hit, false);
                marcarEtapa(ref isPrint);
            }
            else
            {
                xilogravura.stopSound(ferramenta.gameObject);
            }
        }
    }

    void marcarEtapa(ref bool etapa)
    {
        if (!etapa)
        {
            etapa = true;
        }
    }

    public bool isPrinted()
    {
        return isPrint;
    }

    void Update()
    {
        Draw();

        //if (Mouse.current.middleButton.isPressed)
        //{
        //    posicionarFolha();
        //}

        //if (Mouse.current.rightButton.isPressed)
        //{
        //    mostrarResultado();
        //}
    }

    public void posicionarFolha()
    {
        transform.GetChild(1).GetComponent<AudioSource>().Play();
        Animator animator = GetComponent<Animator>();
        animator.SetBool("isPrint", true);
    }

    public void mostrarResultado()
    {
        resultado = true;
        transform.GetChild(2).GetComponent<AudioSource>().Play();
        Animator animator = GetComponent<Animator>();
        animator.SetBool("isResult", true);
    }

    public void resetValues()
    {
        setarTexturas = false;
        isPrint = false;
        resultado = false;
        Animator animator = GetComponent<Animator>();
        animator.SetBool("isPrint", false);
        animator.SetBool("isResult", false);
        animator.Play("EstadoInicial");
        transform.position = new Vector3(2.8f, 3.3f, 1.2f);
    }

}
