using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GlassController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Material currentMaterial;
    private RaycastHit hit;

    private int[] dimensions = {2048, 2048};

    public Painter painter;
    public GrabController grabController;
    public XiloController xiloController;

    public XRGrabInteractable tinta;
    public GameObject roloDeTinta;
    public ParticleSystem tintaDerramada;

    private bool verifSound = true;

    private bool isInkEnable = false;

    private Dictionary<string, RenderTexture> textureDictionary = new Dictionary<string, RenderTexture>();
    private string[] textureNames = { "InkMask" };

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
        int layerMask = 1 << 11; //Fix layer
        if (grabController.isGrab(tinta) && xiloController.getSanded())
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(tinta));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                //print("to colocanto tinta");
                if (verifSound)
                {
                    tinta.gameObject.GetComponent<AudioSource>().Play();
                    verifSound = false;
                }
                RenderTexture mask = textureDictionary["InkMask"];
                painter.SetBrush(15f, 1f, 40f);
                painter.PaintMask(mask, hit);
                painter.instanciarParticulas(tintaDerramada, painter.isToolInteraction(tinta));
                isInkEnable = true;
            }
            else
            {
                if (tinta.gameObject.GetComponent<AudioSource>().isPlaying)
                {
                    tinta.gameObject.GetComponent<AudioSource>().Stop();
                    verifSound = true;
                }
            }
        }

        if (grabController.isGrab(roloDeTinta.GetComponent<XRGrabInteractable>()))
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(roloDeTinta.GetComponent<XRGrabInteractable>()));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask) && isInkEnable)
            {
                //print("to pegando tinta");
                roloDeTinta.GetComponent<InkRollerController>().enableInk();
            }
        }

        if (hit.collider == null || grabController.isToolNull())
        {
            //Parar todos os SFX
            painter.desligarParticulas(tintaDerramada);
        }
    }

    void Update()
    {
        Draw();
    }

    public void resetValues()
    {
        isInkEnable = false;
        verifSound = true;
        roloDeTinta.GetComponent<InkRollerController>().resetValues();
    }

    public bool getInkEnable()
    {
        return isInkEnable;
    }
}
