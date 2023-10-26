using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Shader drawShader;

    private Dictionary<string, RenderTexture> textureDictionary;
    private Material currentMaterial;
    private Material drawMaterial;
    private RaycastHit hit;

    private int[] dimensions = {2048, 2048};

    [SerializeField][Range(1, 20)] private float size;
    [SerializeField][Range(1, 5)] private float hardness;
    [SerializeField][Range(0, 1)] private float strength;

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

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                RenderTexture mask;

                //TO DO
                //Alguma logica que seta a mascara certa a ser pintada de acordo com a ferramenta atual
                mask = textureDictionary["SketchMask"];

                //if (Input.GetKey(KeyCode.Z))
                //{
                //    print("trocando");
                //    mask = textureDictionary["PaintMask"];
                //}

                SetBrush(6f, 5f, 1f);
                PaintMask(mask);
            }
        }
    }

    void SetBrush(float size, float hardness, float strength)
    {
        drawMaterial.SetFloat("_Size", size);
        drawMaterial.SetFloat("_Hardness", hardness);
        drawMaterial.SetFloat("_Strength", strength);
    }

    void PaintMask(RenderTexture mask)
    {
        drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));

        RenderTexture temp = RenderTexture.GetTemporary(mask.width, mask.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(mask, temp);
        Graphics.Blit(temp, mask, drawMaterial);
        RenderTexture.ReleaseTemporary(temp); 
    }
}
