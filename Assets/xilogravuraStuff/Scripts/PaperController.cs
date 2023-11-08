using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PaperController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Dictionary<string, RenderTexture> textureDictionary;
    private Material currentMaterial;
    private RaycastHit hit;

    private int[] dimensions = {2048, 2048};

    public Painter painter;
    public XRGrabInteractable ferramenta;
    public XiloController xilogravura;

    private bool setarTexturas = false;


    void Start()
    {
        currentMaterial = GetComponent<MeshRenderer>().materials[0];

        textureDictionary = new Dictionary<string, RenderTexture>();
        string[] textureNames = { "PrintMask" };

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
        if (painter.isGrabbed(ferramenta))
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(ferramenta));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                if (!setarTexturas)
                {
                    currentMaterial.SetTexture("SketchMask", xilogravura.getTexture("SketchMask"));
                    currentMaterial.SetTexture("SculptMask", xilogravura.getTexture("SculptMask"));
                    currentMaterial.SetTexture("PaintMask", xilogravura.getTexture("PaintMask"));
                    print("texturas setadas");
                    setarTexturas = true;
                }
                print("imprimindo");
                RenderTexture mask = textureDictionary["PrintMask"];
                painter.SetBrush(5f, 1f, 40f);
                painter.PaintMask(mask, hit);
            }
        }
    }

    void Update()
    {
        Draw();
    }

}
