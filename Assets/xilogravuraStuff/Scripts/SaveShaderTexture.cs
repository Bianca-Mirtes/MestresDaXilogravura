using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveShaderTexture : MonoBehaviour
{
    public int TextureLength = 1024;

    public GameObject paper; 
    private Texture2D texture;
    public void Save()
    {
        RenderTexture buffer = new(
                               TextureLength,
                               TextureLength,
                               0,                            // No depth/stencil buffer
                               RenderTextureFormat.ARGB32   // Standard colour format
                           );

        //texture = new Texture2D(TextureLength, TextureLength, TextureFormat.ARGB32, true);

        MeshRenderer render = GetComponent<MeshRenderer>();
        //texture = render.sharedMaterial.GetTexture("_MainTex") as Texture2D;
        Material material = render.sharedMaterial;

        Graphics.Blit(null, buffer, material);
        RenderTexture.active = buffer;           // If not using a scene camera

        System.IO.File.WriteAllBytes(Application.dataPath + "/" + "SkinLut.png", texture.EncodeToPNG());
        // texture.Save();

    }
}
