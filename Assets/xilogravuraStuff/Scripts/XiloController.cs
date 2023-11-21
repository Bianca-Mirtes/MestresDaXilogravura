using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XiloController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Material currentMaterial;
    private RaycastHit hit;

    private int[] dimensions = {2048, 2048};

    public Painter painter;
    public GrabController grabController;
    public PaperController paperController;

    public XRGrabInteractable lapisDeRascunho;
    public XRGrabInteractable goiva;
    public XRGrabInteractable lixa;
    public GameObject roloDeTinta;

    public AudioClip[] lapisSounds;
    public AudioClip lixaSound;
    public AudioClip[] goivaSounds;

    private bool verifSound = true;

    public ParticleSystem lascasDeMadeira;
    public ParticleSystem poDeMadeira;

    private bool isSketched = false;
    private bool isSculped = false;
    private bool isSanded = false;
    private bool isPaint = false;

    private Dictionary<string, RenderTexture> textureDictionary = new Dictionary<string, RenderTexture>();
    private string[] textureNames = { "SketchMask", "SculptMask", "SandpaperMask", "PaintMask", "PrintMaskOld" };

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
        int layerMask = 1 << 10;
        
        RenderTexture mask = null;

        if (grabController.isGrab(lapisDeRascunho) && !isSculped)
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(lapisDeRascunho));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                //Comeca SFX de lapis
                if (verifSound)
                {
                    lapisDeRascunho.gameObject.GetComponent<AudioSource>().Play();
                    verifSound = false;
                }
                mask = textureDictionary["SketchMask"];
                painter.SetBrush(5f, 1f, 10f);
                marcarEtapa(ref isSketched);
            }
            else
            {
                if (lapisDeRascunho.gameObject.GetComponent<AudioSource>().isPlaying)
                {
                    lapisDeRascunho.gameObject.GetComponent<AudioSource>().Stop();
                    verifSound = true;
                }
            }
        } else if (grabController.isGrab(goiva) && isSketched && !isSanded)
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(goiva));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                //Comeca SFX de entalhe
                if (verifSound)
                {
                    goiva.gameObject.GetComponent<AudioSource>().Play();
                    verifSound = false;
                }
                mask = textureDictionary["SculptMask"];
                painter.SetBrush(10f, 0.8f, 20f, 26f);
                painter.instanciarParticulas(lascasDeMadeira, hit.point);
                marcarEtapa(ref isSculped);
            }
            else
            {
                if (goiva.gameObject.GetComponent<AudioSource>().isPlaying)
                {
                    goiva.gameObject.GetComponent<AudioSource>().Stop();
                    verifSound = true;
                }
            }
        }else if (grabController.isGrab(lixa) && isSculped && !isPaint)
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(lixa));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                //Comeca SFX de lixa
                if (verifSound)
                {
                    lixa.gameObject.GetComponent<AudioSource>().Play();
                    verifSound = false;
                }
                
                mask = textureDictionary["SandpaperMask"];
                painter.SetBrush(10f, 0.8f, 25f, 25f);
                painter.instanciarParticulas(poDeMadeira, hit.point);
                marcarEtapa(ref isSanded);
            }
            else
            {
                if (lixa.gameObject.GetComponent<AudioSource>().isPlaying)
                {
                    lixa.gameObject.GetComponent<AudioSource>().Stop();
                    verifSound = true;
                }
            }
        }else if (grabController.isGrab(roloDeTinta.GetComponent<XRGrabInteractable>()) && isSanded && !paperController.isPrinted())
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(roloDeTinta.GetComponent<XRGrabInteractable>()));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                if (roloDeTinta.GetComponent<InkRollerController>().isInkEnable())
                {
                    //Comeca SFX de rolin de tinta
                    if (verifSound)
                    {
                        roloDeTinta.gameObject.GetComponent<AudioSource>().Play();
                        verifSound = false;
                    }
                    
                    mask = textureDictionary["PaintMask"];
                    painter.SetBrush(10f, 0.8f, 28f, 12f);
                    marcarEtapa(ref isPaint);
                }
                else
                {
                    painter.SetBrush(10f, 0.8f, 0f);
                }
            }
            else
            {
                if (roloDeTinta.gameObject.GetComponent<AudioSource>().isPlaying)
                {
                    roloDeTinta.gameObject.GetComponent<AudioSource>().Stop();
                    verifSound = true;
                }
            }
        }
        if(hit.collider == null)
        {
            //Parar todos os SFX
            painter.desligarParticulas(lascasDeMadeira);
            painter.desligarParticulas(poDeMadeira);
        }

        if(hit.collider != null)
            painter.PaintMask(mask, hit);
        
    }

    void Update()
    {
        Draw();
    }

    void marcarEtapa(ref bool etapa)
    {
        if (!etapa)
        {
            etapa = true;
        }
    }

    public bool isPainted()
    {
        return isPaint;
    }

    public Texture getTexture(string chave)
    {
        return textureDictionary[chave];
    }

    public void resetValues()
    {
        isSketched = false;
        isSculped = false;
        isSanded = false;
        isPaint = false;
    }

    public bool getSketched()
    {
        return isSketched;
    }

    public bool getSculped()
    {
        return isSculped;
    }

    public bool getSanded()
    {
        return isSanded;
    }

    public bool getPaint()
    {
        return isPaint;
    }
}
