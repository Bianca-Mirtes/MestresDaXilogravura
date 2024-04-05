using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertToPGN : MonoBehaviour
{

    public GameObject objectOnCamera;

    public Camera captureCamera;

    public Vector3 positionScreen;

    public GameObject RightHand;

    public GameObject LeftHand;

    private WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    private WaitForSeconds waitTime = new WaitForSeconds(0.2F);

    public void Capture()
    {
        StartCoroutine(CaptureCamera());
    }

    IEnumerator CaptureCamera()
    {

        LeftHand.SetActive(false);
        RightHand.SetActive(false);

        objectOnCamera.transform.position = positionScreen;

        yield return waitTime;

        Texture2D texture = new Texture2D(captureCamera.pixelWidth, captureCamera.pixelHeight, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, captureCamera.pixelWidth, captureCamera.pixelHeight), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();

        System.IO.File.WriteAllBytes(Application.dataPath + "/ConvertedTextureOnCamera.png", bytes);
        System.IO.File.WriteAllBytes(Application.dataPath + "/ConvertedTextureTWOOnCamera.png", bytes);

        Debug.Log("Textura convertida e salva como 'ConvertedTextureOnCamera.png' em " + Application.dataPath);

       LeftHand.SetActive(true);
       RightHand.SetActive(true);

    }
}
