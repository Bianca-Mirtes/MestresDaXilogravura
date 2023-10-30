using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XiloController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Dictionary<string, RenderTexture> textureDictionary;
    private Material currentMaterial;
    private RaycastHit hit;

    private int[] dimensions = {2048, 2048};

    public Painter painter;

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

    public void Draw(Transform tool)
    {
        int layerMask = 3;
        if (Physics.Raycast(cam.ScreenPointToRay(tool.position), out hit, Mathf.Infinity, layerMask))
        {
            RenderTexture mask;

            //TO DO
            //Alguma logica que seta a mascara certa a ser pintada de acordo com a ferramenta atual
            mask = textureDictionary["SketchMask"];
            painter.SetBrush(5f, 1f, 5f);

            //Provisorio pra efeito de testes
            /*if (Mouse.current.rightButton.isPressed)
            {
                //currentMaterial.SetInt("teste", 1); 
                mask = textureDictionary["SculptMask"];
                painter.SetBrush(10f, 0.8f, 15f, 18f);
            }
            if (Mouse.current.middleButton.isPressed)
            {
                mask = textureDictionary["PaintMask"];
                painter.SetBrush(10f, 0.8f, 30f, 12f);
            }*/

            painter.PaintMask(mask, hit);
        }
    }

    void Update()
    {
        //Draw();
    }
}
