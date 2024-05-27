using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
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
    public GameObject tutorial;

    [SerializeField]
    private List<GameObject> objectNames;

    [SerializeField]
    private TouchController touch;

    private bool isStart = false;

    private bool verifSound = true;

    private Vector2 lastHitGoiva = Vector2.zero;

    public ParticleSystem lascasDeMadeira;
    public ParticleSystem poDeMadeira;

    private bool isSketched = false;
    private bool isSculped = false;
    private bool isSanded = false;
    private bool isPaint = false;

    private Dictionary<string, RenderTexture> textureDictionary = new Dictionary<string, RenderTexture>();
    private string[] textureNames = { "SketchMask", "SculptMask", "SandpaperMask", "PaintMask", "PrintMaskOld" };

    private void Awake()
    {
        for (int i = 0; i < objectNames.Count; i++)
        {
            objectNames[i].SetActive(false);
            Debug.Log(objectNames[i].name);
        }
    }
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

    public void ResetOneTexture(string textureName)
    {
        Graphics.SetRenderTarget(textureDictionary[textureName]);
        GL.Clear(true, true, Color.black);

    }

    public void initSound(GameObject ferramenta)
    {
        if (verifSound)
        {
            ferramenta.gameObject.GetComponent<AudioSource>().Play();
            verifSound = false;
        }
    }

    public void stopSound(GameObject ferramenta)
    {
        if (ferramenta.gameObject.GetComponent<AudioSource>().isPlaying)
        {
            ferramenta.gameObject.GetComponent<AudioSource>().Stop();
            verifSound = true;
        }
    }

    public void pauseSound(GameObject ferramenta)
    {
        if (ferramenta.gameObject.GetComponent<AudioSource>().isPlaying)
        {
            ferramenta.gameObject.GetComponent<AudioSource>().Pause();
            verifSound = true;
        }
    }

    public void setVerifSound(bool value)
    {
        verifSound = value;
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

    // Metodo para debug no simulador
    private bool click()
    {
        return Mouse.current.leftButton.isPressed;
    }

    public void Draw()
    {
        int layerMask = 1 << 10;
        
        RenderTexture mask = null;

        bool interpolate = false;

        if (grabController.isGrab(lapisDeRascunho) && !isSculped )
        {     
                Vector3 pointerPosition = getPointerPosition(lapisDeRascunho);
                if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask) && (click() || touch.IsClickedWithRightHand() || touch.IsClickedWithLeftHand()))
                {
                    initSound(lapisDeRascunho.gameObject);
                    mask = textureDictionary["SketchMask"];
                    painter.SetBrushPreset(Brush.HardCircle);
                    marcarEtapa(ref isSketched);
                    interpolate = true;
                }
                else {
                    stopSound(lapisDeRascunho.gameObject);
                    painter.resetInterpolation();
                }
                    
            
        } else if (grabController.isGrab(goiva) && isSketched && !isSanded)
        {
                float anguloDaGoiva = 0.85f;
                Vector2 direction = new Vector2(1, 0);
                if (lastHitGoiva != Vector2.zero)
                {
                    direction = hit.textureCoord - lastHitGoiva;
                    float totalDistance = direction.magnitude;
                    direction.Normalize();
                    //print("direcao XY: " + direction);
                }
                lastHitGoiva = hit.textureCoord;

                Vector3 pointerPosition = getPointerPosition(goiva);
                if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask) 
                && (click() || touch.IsClickedWithRightHand() || touch.IsClickedWithLeftHand()) && (direction.y >= anguloDaGoiva))
                {
                    initSound(goiva.gameObject);
                    mask = textureDictionary["SculptMask"];
                    painter.SetBrushPreset(Brush.HardSquare);
                    painter.instanciarParticulas(lascasDeMadeira, hit.point);
                    marcarEtapa(ref isSculped);
                    interpolate = false;
                }
                else {
                    pauseSound(goiva.gameObject);
                    painter.resetInterpolation();
                    painter.desligarParticulas(lascasDeMadeira);
                }

        }
        else if (grabController.isGrab(lixa) && isSculped && !isPaint)
        {
                Vector3 pointerPosition = getPointerPosition(lixa);
                if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask) && (click() || touch.IsClickedWithRightHand() || touch.IsClickedWithLeftHand()))
                {
                    initSound(lixa.gameObject);
                    mask = textureDictionary["SandpaperMask"];
                    painter.SetBrushPreset(Brush.SoftSquare);
                    painter.instanciarParticulas(poDeMadeira, hit.point);
                    marcarEtapa(ref isSanded);
                    interpolate = false;
                }
                else { 
                    stopSound(lixa.gameObject);
                    painter.resetInterpolation();
                    painter.desligarParticulas(poDeMadeira);
                }
        }
        else if (grabController.isGrab(roloDeTinta.GetComponent<XRGrabInteractable>()) && isSanded && !paperController.isPrinted())
        {
                Vector3 pointerPosition = getPointerPosition(roloDeTinta.GetComponent<XRGrabInteractable>());
                if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask) && (click() || touch.IsClickedWithRightHand() || touch.IsClickedWithLeftHand()))
                {
                    if (roloDeTinta.GetComponent<InkRollerController>().isInkEnable())
                    {
                        initSound(roloDeTinta.gameObject);
                        mask = textureDictionary["PaintMask"];
                        painter.SetBrushPreset(Brush.SoftSquare);
                        marcarEtapa(ref isPaint);
                        interpolate = false;
                    }
                }
                else {
                    stopSound(lapisDeRascunho.gameObject);
                    painter.resetInterpolation();
                }
        }
        if(hit.collider == null || grabController.isToolNull())
        {
            painter.desligarParticulas(lascasDeMadeira);
            painter.desligarParticulas(poDeMadeira);
        }

        if(mask != null && hit.collider != null)
            painter.PaintMask(mask, hit, interpolate);
        
    }

    private Vector3 getPointerPosition(XRGrabInteractable ferramenta)
    {
        return cam.WorldToScreenPoint(painter.isToolInteraction(ferramenta));
    }

    public void enableProcess()
    {
        isStart = true;
    }

    void Update()
    {
        if (isStart)
        {
            Draw();
        }
        else
        {
            painter.desligarParticulas(lascasDeMadeira);
            painter.desligarParticulas(poDeMadeira);
        }
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
        isStart = false;

        isSketched = false;
        isSculped = false;
        isSanded = false;
        isPaint = false;
    }

    public bool getSketched()
    {
        return isSketched;
    }

    public void SetSketched(bool state)
    {
        isSketched = state;
    }

    public bool getSculped()
    {
        return isSculped;
    }

    public void SetSculped(bool state)
    {
        isSculped = state;
    }

    public bool getSanded()
    {
        return isSanded;
    }

    public void setSanded(bool state)
    {
        isSanded = state;
    }

    public bool getPaint()
    {
        return isPaint;
    }

    public void setPaint(bool state)
    {
        isPaint = state;
    }
}
