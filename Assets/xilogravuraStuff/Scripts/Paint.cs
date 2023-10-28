using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paint : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Shader drawShader;

    private Dictionary<string, RenderTexture> textureDictionary;
    private Material currentMaterial;
    private Material drawMaterial;
    private RaycastHit hit;

    private int[] dimensions = {2048, 2048};

    [SerializeField][Range(1, 25)] private float size;
    [SerializeField][Range(1, 15)] private float hardness;
    [SerializeField][Range(0, 1)] private float strength;

    public bool mascaraDeEscultura = false, mascaraDePintura = false;

    void Start()
    {
        drawMaterial = new Material(drawShader);
        drawMaterial.SetVector("_Color", Color.white);

        currentMaterial = GetComponent<MeshRenderer>().materials[0];

        //sketchMask = new RenderTexture(dimensions[0], dimensions[1], 0, RenderTextureFormat.ARGBFloat);
        //Graphics.SetRenderTarget(sketchMask);
        //GL.Clear(true, true, Color.black);
        //Graphics.SetRenderTarget(null);
        //currentMaterial.SetTexture("SketchMask", sketchMask);

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
        if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, layerMask))
        {
            RenderTexture mask;

            //TO DO
            //Alguma logica que seta a mascara certa a ser pintada de acordo com a ferramenta atual
            mask = textureDictionary["SketchMask"];
            SetBrush(5f, 5f, 1f);

            //Provisorio pra efeito de testes
            if (Mouse.current.rightButton.isPressed)
            {
                mask = textureDictionary["SculptMask"];
                SetBrush(15f, 12f, 1f);
            }
            if (Mouse.current.middleButton.isPressed)
            {
                mask = textureDictionary["PaintMask"];
                SetBrush(40f, 10f, 0.8f);
            }

            PaintMask(mask);
        }
    }

    void Update()
    {
        /*if (Mouse.current.leftButton.isPressed)
        {
            int layerMask = 1 << 10;
            if (Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, layerMask))
            {
                RenderTexture mask;

                //TO DO
                //Alguma logica que seta a mascara certa a ser pintada de acordo com a ferramenta atual
                mask = textureDictionary["SketchMask"];
                SetBrush(5f, 5f, 1f);

                //Provisorio pra efeito de testes
                if (Mouse.current.rightButton.isPressed)
                {
                    mask = textureDictionary["SculptMask"];
                    SetBrush(15f, 12f, 1f);
                }
                if (Mouse.current.middleButton.isPressed)
                {
                    mask = textureDictionary["PaintMask"];
                    SetBrush(40f, 10f, 0.8f);
                }

                PaintMask(mask);
            }
        }*/
    }

    void SetBrush(float size, float hardness, float strength)
    {
        drawMaterial.SetFloat("_Size", size);
        drawMaterial.SetFloat("_Hardness", hardness);
        drawMaterial.SetFloat("_Strength", strength);
    }

    void PaintMask(RenderTexture mask)
    {
        Debug.Log("pintando");
        drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));

        RenderTexture temp = RenderTexture.GetTemporary(mask.width, mask.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(mask, temp);
        Graphics.Blit(temp, mask, drawMaterial);
        RenderTexture.ReleaseTemporary(temp); 
    }
}
