using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertToPGN : MonoBehaviour
{
    public GameObject objectWithMaterial;
    public void Convert()
    {
        Debug.Log("cov");
        Renderer renderer = objectWithMaterial.GetComponent<Renderer>();
        Material material = renderer.material;

        if (material != null)
        {
            Texture mainTexture = material.GetTexture("....");

            if (mainTexture != null)
            {
                Debug.Log("Tem textura");
                Texture2D tex2D = mainTexture as Texture2D;
                if (tex2D == null)
                {
                    Debug.Log("Não é null");
                    tex2D = new Texture2D(mainTexture.width, mainTexture.height);
                    RenderTexture currentRT = RenderTexture.active;
                    RenderTexture.active = (RenderTexture)mainTexture;
                    tex2D.ReadPixels(new Rect(0, 0, mainTexture.width, mainTexture.height), 0, 0);
                    tex2D.Apply();
                    RenderTexture.active = currentRT;
                }
                byte[] bytes = tex2D.EncodeToPNG();
                Debug.Log("vai passar...");
                System.IO.File.WriteAllBytes(Application.dataPath + "/ConvertedTexture.png", bytes);
                Debug.Log("Textura convertida e salva como 'ConvertedTexture.png'. em "+ Application.dataPath);
            } else
            {
                Debug.LogWarning("O material não possui uma textura principal.");
            }
        } else
        {
            Debug.LogWarning("Material não atribuído.");
        }
    }
}
