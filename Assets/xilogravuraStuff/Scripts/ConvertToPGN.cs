using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertToPGN : MonoBehaviour
{
   // public GameObject objectWithMaterial;

    public GameObject objectOnCamera;

    public Camera captureCamera;

    public Vector3 positionScreen;

  
    /* public void Convert()
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
    }*/


    public void CaptureCamera()
    {

        //objectOnCamera.transform.position = positionScreen;

        objectOnCamera.transform.position = positionScreen;

        RenderTexture renderTexture = new RenderTexture(captureCamera.pixelWidth, captureCamera.pixelHeight, 24);
        captureCamera.targetTexture = renderTexture;
        captureCamera.Render();

        Debug.Log("W - " + captureCamera.pixelWidth + " H - " + captureCamera.pixelHeight);

        Texture2D texture = new Texture2D(captureCamera.pixelWidth, captureCamera.pixelHeight, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, captureCamera.pixelWidth, captureCamera.pixelHeight), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();

        System.IO.File.WriteAllBytes(Application.dataPath + "/ConvertedTextureOnCamera.png", bytes);
        System.IO.File.WriteAllBytes(Application.dataPath + "/ConvertedTextureTWOOnCamera.png", bytes);

        Debug.Log("Textura convertida e salva como 'ConvertedTextureOnCamera.png' em " + Application.dataPath);

    }
}
