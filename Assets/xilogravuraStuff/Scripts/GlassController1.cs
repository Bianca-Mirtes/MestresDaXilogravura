using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GlassController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Dictionary<string, RenderTexture> textureDictionary;
    private Material currentMaterial;
    private RaycastHit hit;

    private int[] dimensions = {2048, 2048};

    public Painter painter;

    public XRGrabInteractable tinta;
    public GameObject roloDeTinta;

    private bool isInkEnable = false;

    void Start()
    {
        currentMaterial = GetComponent<MeshRenderer>().materials[0];

        textureDictionary = new Dictionary<string, RenderTexture>();
        string[] textureNames = { "InkMask" };

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
        if (painter.isGrabbed(tinta))
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(tinta));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask))
            {
                print("to colocanto tinta");
                RenderTexture mask = textureDictionary["InkMask"];
                painter.SetBrush(5f, 1f, 30f);
                painter.PaintMask(mask, hit);
                isInkEnable = true;
            }
        }

        if (painter.isGrabbed(roloDeTinta.GetComponent<XRGrabInteractable>()))
        {
            Vector3 pointerPosition = cam.WorldToScreenPoint(painter.isToolInteraction(roloDeTinta.GetComponent<XRGrabInteractable>()));
            if (Physics.Raycast(cam.ScreenPointToRay(pointerPosition), out hit, Mathf.Infinity, layerMask) && isInkEnable)
            {
                print("to pegando tinta");
                roloDeTinta.GetComponent<InkRollerController>().enableInk();
            }
        }

        //Provisorio pra efeito de testes
        //Provisorio pra efeito de testes
        if (Mouse.current.leftButton.isPressed)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, layerMask))
            {
                RenderTexture mask = textureDictionary["InkMask"];
                painter.SetBrush(5f, 1f, 15f);
                painter.PaintMask(mask, hit);
                isInkEnable = true;
            }
        }
    }

    void Update()
    {
        Draw();
    }

}
