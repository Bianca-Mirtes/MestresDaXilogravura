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

    private GameObject canva;
    public GameObject posicionarFolhaMenu;
    public GameObject resultadoMenu;
    public GameObject restartMenu;

    private Button posicionarFolhaButton;
    private Button resultadoButton;
    private Button restartButton;

    private bool folhaPosicionada = false;
    private bool folhaResultado = false;

    private GameObject drawingCurrent;

    private int indice=0;
    private bool verifStart = false;
    public Sprite[] desenhos;
    public GameObject matriz;
    public GameObject vidro;
    public GameObject papel;
    public Rect cropRect;

    // Start is called before the first frame update
    void Start()
    {
        drawingCurrent = GameObject.Find("Desenho");

        posicionarFolhaButton = posicionarFolhaMenu.GetComponentInChildren<Button>();
        resultadoButton = resultadoMenu.GetComponentInChildren<Button>();
        restartButton = restartMenu.GetComponentInChildren<Button>();

        posicionarFolhaMenu.SetActive(false);
        resultadoMenu.SetActive(false);
        restartMenu.SetActive(false);
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

        if(matriz.GetComponent<XiloController>().isPainted() && !folhaPosicionada)
            posicionarFolhaMenu.SetActive(true);

        if (papel.GetComponent<PaperController>().isPrinted() && !folhaResultado)
            resultadoMenu.SetActive(true);

        posicionarFolhaButton.onClick.AddListener(() => posicionarFolha());
        resultadoButton.onClick.AddListener(() => StartCoroutine(mostarResultado()));

        restartButton.onClick.AddListener(() => restart());

        if (!verifStart)
        {
            Sprite spriteTexture = Sprite.Create(desenhos[indice].texture, cropRect, new Vector2(0.5f, 0.5f));
            drawingCurrent.GetComponent<Image>().sprite = spriteTexture;
        }

    }

    void posicionarFolha()
    {
        PaperController paperController = papel.GetComponent<PaperController>();
        paperController.posicionarFolha();
        folhaPosicionada = true;
        posicionarFolhaMenu.SetActive(false);
    }

    IEnumerator mostarResultado()
    {
        PaperController paperController = papel.GetComponent<PaperController>();
        paperController.mostrarResultado();
        folhaResultado = true;
        resultadoMenu.SetActive(false);

        yield return new WaitForSeconds(3f);

        restartMenu.SetActive(true);
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
        //verifStart = true;
        canva = GameObject.Find("Menu");
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

            matriz.GetComponent<XiloController>().setTextures();
            Material xiloMaterial = matriz.GetComponent<MeshRenderer>().materials[0];
            xiloMaterial.SetTexture("SketchTexture", desenho);

            Material paperMaterial = papel.GetComponent<MeshRenderer>().materials[0];
            paperMaterial.SetTexture("SketchTexture", desenho);
        }
    }

    void restart()
    {
        //E eu quis escrever um codigo que pudesse te fazer sentir [...]
        //com uma bela identacao pra dizer o que eu nao consigo documentar
        restartMenu.SetActive(false);
        XiloController xiloController = matriz.GetComponent<XiloController>();
        xiloController.resetTextures();
        xiloController.resetValues();

        GlassController glassController = vidro.GetComponent<GlassController>();
        glassController.resetTextures();
        glassController.resetValues();

        PaperController paperController = papel.GetComponent<PaperController>();
        paperController.resetTextures();
        paperController.resetValues();

        folhaPosicionada = false;
        folhaResultado = false;
    }

}
