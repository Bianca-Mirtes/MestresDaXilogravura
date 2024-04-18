using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertToPGN : MonoBehaviour
{

    public GameObject objectOnCamera;

    public Camera captureCamera;

    private Vector3 positionAfterPrint;

    public GameObject RightHand;

    public GameObject LeftHand;

    private WaitForSeconds waitTime = new WaitForSeconds(0.2F);

    [SerializeField]
    private GameObject RugToScreenshot;

    [SerializeField]
    private GameObject background;

    public void Capture()
    {
        StartCoroutine(CaptureCamera());
        
    }
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame(); // wait the time for finish frame
    IEnumerator CaptureCamera()
    {
        yield return frameEnd;
        LeftHand.SetActive(false);
        RightHand.SetActive(false);

        positionAfterPrint = objectOnCamera.transform.position;
        background.SetActive(true); 
        objectOnCamera.transform.position = RugToScreenshot.transform.position;

        Transform camera = objectOnCamera.transform.Find("Camera Offset").Find("Main Camera");

        if(camera != null )
        {
            Debug.Log("Tem camera");
            camera.eulerAngles = new Vector3(
                0,
                90,
                0
            );
        }

        yield return waitTime;

        Texture2D texture = new Texture2D(captureCamera.pixelWidth, captureCamera.pixelHeight, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, captureCamera.pixelWidth, captureCamera.pixelHeight), 0, 0);
        texture.Apply();

        int leftMargin = 10;
        int rightMargin = 10;
        int topMargin = 10;
        int bottomMargin = 10;

        int width = texture.width - leftMargin - rightMargin;
        int height = texture.height - topMargin - bottomMargin;

        int x = leftMargin;
        int y = bottomMargin;

        Color[] pixels = texture.GetPixels(x, y, width, height);

        Texture2D croppedTexture = new Texture2D(width, height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        byte[] bytes = croppedTexture.EncodeToPNG();

        System.IO.File.WriteAllBytes(Application.dataPath + "/ConvertedTextureOnCamera.png", bytes);

        Debug.Log("Textura convertida e salva como 'ConvertedTextureOnCamera.png' em " + Application.dataPath);

        LeftHand.SetActive(true);
        RightHand.SetActive(true);
        objectOnCamera.transform.position = positionAfterPrint;
        background.SetActive(false);
    }

}
