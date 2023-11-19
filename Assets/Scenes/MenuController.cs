using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MenuController : MonoBehaviour
{
    public Button start;
    public Button left;
    public Button right;
    private GameObject drawingCurrent;

    private int indice=0;
    private bool verifStart = false;
    public Sprite[] desenhos;
    public GameObject matriz;
    public GameObject papel;
    public Rect cropRect;

    // Start is called before the first frame update
    void Start()
    {
        drawingCurrent = GameObject.Find("Desenho");
        /*GameObject.Find("tool1").GetComponent<XRGrabInteractable>().enabled = false;
        GameObject.Find("tool2").GetComponent<XRGrabInteractable>().enabled = false;
        GameObject.Find("tool3").GetComponent<XRGrabInteractable>().enabled = false;
        GameObject.Find("rolinho").GetComponent<XRGrabInteractable>().enabled = false;
        GameObject.Find("tinta").GetComponent<XRGrabInteractable>().enabled = false;
        GameObject.Find("colher").GetComponent<XRGrabInteractable>().enabled = false;*/
    }

    // Update is called once per frame
    void Update()
    {
        left.onClick.AddListener(() => PreviousMenu());
        right.onClick.AddListener(() => NextMenu());
        start.onClick.AddListener(() => StartExp());
        if (!verifStart)
        {
            Sprite spriteTexture = Sprite.Create(desenhos[indice].texture, cropRect, new Vector2(0.5f, 0.5f));
            drawingCurrent.GetComponent<Image>().sprite = spriteTexture;
        }

    }

    void NextMenu()
    {
        if(indice == desenhos.Length-1)
        {
            indice = 0;
        }
        else
        {
            indice++;
        }
    }

    void PreviousMenu()
    {
        if(indice <= 0)
        {
            indice = desenhos.Length-1;
        }
        else
        {
            indice--;
        }
    }

    private void StartExp()
    {
        verifStart = true;
        GameObject canva = GameObject.Find("Menu");
        if (canva != null)
        {
            setDesenhoEscolhido();
            canva.SetActive(false);
        }
        /*GameObject.Find("tool1").GetComponent<XRGrabInteractable>().enabled = true;
        GameObject.Find("tool2").GetComponent<XRGrabInteractable>().enabled = true;
        GameObject.Find("tool3").GetComponent<XRGrabInteractable>().enabled = true;
        GameObject.Find("rolinho").GetComponent<XRGrabInteractable>().enabled = true;
        GameObject.Find("tinta").GetComponent<XRGrabInteractable>().enabled = true;
        GameObject.Find("colher").GetComponent<XRGrabInteractable>().enabled = true;*/
    }

    void setDesenhoEscolhido()
    {
        Sprite spriteAtual = desenhos[indice];
        if (spriteAtual != null)
        {
            Texture2D desenho = new Texture2D((int)spriteAtual.rect.width, (int)spriteAtual.rect.height);

            desenho.SetPixels(spriteAtual.texture.GetPixels((int)spriteAtual.rect.x, (int)spriteAtual.rect.y,
                (int)spriteAtual.rect.width, (int)spriteAtual.rect.height));

            desenho.Apply();

            matriz.GetComponent<XiloController>().resetTextures();
            Material xiloMaterial = matriz.GetComponent<MeshRenderer>().materials[0];
            xiloMaterial.SetTexture("SketchTexture", desenho);

            Material paperMaterial = papel.GetComponent<MeshRenderer>().materials[0];
            paperMaterial.SetTexture("SketchTexture", desenho);
        }
    }
    
}
