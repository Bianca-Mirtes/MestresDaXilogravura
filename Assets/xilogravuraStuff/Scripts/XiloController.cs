using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XiloController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Dictionary<string, RenderTexture> textureDictionary;
    private Material currentMaterial;
    private RaycastHit hit;

    private int[] dimensions = {2048, 2048};

    public Painter painter;

    public XRGrabInteractable lapisDeRascunho;
    public XRGrabInteractable goiva;
    public GameObject roloDeTinta;

    void Start()
    {
        currentMaterial = GetComponent<MeshRenderer>().materials[0];

        textureDictionary = new Dictionary<string, RenderTexture>();
        string[] textureNames = { "SketchMask", "SculptMask", "PaintMask", "PrintMask" };

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
        
        RenderTexture mask = textureDictionary["SketchMask"];

        if (painter.isGrabbed(lapisDeRascunho))
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(lapisDeRascunho));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                print("to rascunhando");
                mask = textureDictionary["SketchMask"];
                painter.SetBrush(5f, 1f, 5f);
            }
        }

        if (painter.isGrabbed(goiva))
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(goiva));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                print("to entalhando");
                mask = textureDictionary["SculptMask"];
                painter.SetBrush(10f, 0.8f, 15f, 18f);
            }
        }

        if (painter.isGrabbed(roloDeTinta.GetComponent<XRGrabInteractable>()))
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(roloDeTinta.GetComponent<XRGrabInteractable>()));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                if (roloDeTinta.GetComponent<InkRollerController>().isInkEnable())
                {
                    print("to pintando");
                    mask = textureDictionary["PaintMask"];
                    painter.SetBrush(10f, 0.8f, 30f, 12f);
                }else
                {
                    painter.SetBrush(10f, 0.8f, 0f);
                }
            }
        }


        //Provisorio pra efeito de testes
        //Provisorio pra efeito de testes
        if (Mouse.current.leftButton.isPressed)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, layerMask))
            {
                mask = textureDictionary["SketchMask"];
                painter.SetBrush(5f, 1f, 5f);
            }
        }
        if (Mouse.current.rightButton.isPressed)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, layerMask))
            {
                //currentMaterial.SetInt("teste", 1);
                mask = textureDictionary["SculptMask"];
                painter.SetBrush(10f, 0.8f, 15f, 18f);
            }
        }
        if (Mouse.current.middleButton.isPressed)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, layerMask))
            {
                mask = textureDictionary["PaintMask"];
                painter.SetBrush(10f, 0.8f, 30f, 12f);
            }
        }

        painter.PaintMask(mask, hit);
        
    }

    void Update()
    {
        Draw();
        
    }
}
