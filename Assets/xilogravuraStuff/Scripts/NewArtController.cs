using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static TreeEditor.TextureAtlas;

public class NewArtController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    
    private Material currentMaterial;
    private RaycastHit hit;

    private int[] dimensions = { 2048, 2048 };

    public Painter painter;
    public GrabController grabController;
    public GameObject matriz;

    public Button voltar;

    public XRGrabInteractable lapisDeRascunho;

    private bool verifSound = true;
    private bool isSketched = false;

    [SerializeField]
    private TouchController touch;

    private Dictionary<string, RenderTexture> textureDictionary = new Dictionary<string, RenderTexture>();
    private string[] textureNames = { "SketchMask" };

    void Start()
    {
        currentMaterial = GetComponent<MeshRenderer>().materials[0];
        voltar.onClick.AddListener(() => ReturnProcess());
        setTextures();
    }

    private void ReturnProcess()
    {
        if(!matriz.GetComponent<XiloController>().getSketched())
        {
            Graphics.SetRenderTarget(textureDictionary["SketchMask"]);
            GL.Clear(true, true, Color.black);
        }
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

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    // Metodo para debug no simulador
    private bool click()
    {
        return Mouse.current.leftButton.isPressed;
    }

    public void Draw()
    {
        int layerMask = 1 << 14; //Fix layer
        if (grabController.isGrab(lapisDeRascunho))
        {
                Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(lapisDeRascunho));
                if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask) && (click() || touch.IsClickedWithLeftHand() || touch.IsClickedWithRightHand()))
            {
                    if (verifSound)
                    {
                        lapisDeRascunho.gameObject.GetComponent<AudioSource>().Play();
                        verifSound = false;
                    }
                    RenderTexture mask = textureDictionary["SketchMask"];
                    //painter.SetBrush(2f);
                    painter.PaintMask(mask, hit, true);
                    isSketched = true;
                }
                else
                {
                    lapisDeRascunho.gameObject.GetComponent<AudioSource>().Stop();
                    verifSound = true;
                    painter.resetInterpolation();
                }
        }

        //if (hit.collider == null || grabController.isToolNull())
        //{
        //    //Parar todos os SFX
            
        //}
    }

    public void resetValues()
    {
        isSketched = false;
        verifSound = true;
    }

    public bool isArt()
    {
        return isSketched;
    }

    public Texture getTexture(string chave)
    {
        return textureDictionary[chave];
    }
}
