using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MenuController : MonoBehaviour
{
    public Button start;
    public Button createYourArt;
    public GameObject desenho;
    public GameObject tituloMenu;
    public Button left;
    public Button right;

    private GameObject canva;
    public GameObject posicionarFolhaMenu;
    public GameObject resultadoMenu;
    public GameObject restartMenu;
    public GameObject tutorial;

    private Button posicionarFolhaButton;
    private Button resultadoButton;
    private Button restartButton;

    private bool folhaPosicionada = false;
    private bool folhaResultado = false;

    private GameObject drawingCurrent;

    private int indice=0;
    private int indiceAnterior = 0;
    private bool switchImage = false;
    private bool verifStart = false;
    private bool artAutoral = false;
    public Sprite[] desenhos;
    public GameObject matriz;
    public GameObject vidro;
    public GameObject papel;
    public GameObject art;
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

        left.onClick.AddListener(() => PreviousMenu());
        right.onClick.AddListener(() => NextMenu());
        start.onClick.AddListener(() => StartExp());
        createYourArt.onClick.AddListener(() => Invoke("Create", 1f));

        posicionarFolhaButton.onClick.AddListener(() => posicionarFolha());
        resultadoButton.onClick.AddListener(() => StartCoroutine(mostarResultado()));
        restartButton.onClick.AddListener(() => restart());
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;

        if (matriz.GetComponent<XiloController>().isPainted() && !folhaPosicionada)
        {
            tutorial.transform.GetChild(4).gameObject.SetActive(false);
            posicionarFolhaMenu.SetActive(true);
        }
        if (papel.GetComponent<PaperController>().isPrinted() && !folhaResultado)
        {
            tutorial.transform.GetChild(5).gameObject.SetActive(false);
            resultadoMenu.SetActive(true);
        }

        if (!verifStart && (indice != indiceAnterior || !switchImage))
        {
            Sprite spriteTexture = Sprite.Create(desenhos[indice].texture, cropRect, new Vector2(0.5f, 0.5f));
            drawingCurrent.GetComponent<Image>().sprite = spriteTexture;

            indiceAnterior = indice;
            left.enabled = true;
            right.enabled = true;
        }

        if (art.GetComponent<NewArtController>().isArt())
        {
            start.gameObject.SetActive(true);
        }
    }

    void posicionarFolha()
    {
        PaperController paperController = papel.GetComponent<PaperController>();
        paperController.posicionarFolha();
        folhaPosicionada = true;
        posicionarFolhaMenu.SetActive(false);
        tutorial.transform.GetChild(5).gameObject.SetActive(true);
    }

    IEnumerator mostarResultado()
    {
        PaperController paperController = papel.GetComponent<PaperController>();
        tutorial.transform.GetChild(5).gameObject.SetActive(false);
        paperController.mostrarResultado();
        folhaResultado = true;
        resultadoMenu.SetActive(false);

        yield return new WaitForSeconds(3f);

        restartMenu.SetActive(true);
    }

    void NextMenu()
    {
        right.enabled = false;
        switchImage = true;
        if (indice == desenhos.Length-1)
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
        left.enabled = false;
        switchImage = true;
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
            configurarSimulacao();
            canva.SetActive(false);
        }
    }

    private void Create()
    {
        Debug.Log("Criando art");
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
        start.gameObject.SetActive(false);
        desenho.SetActive(false);
        tituloMenu.SetActive(false);
        createYourArt.gameObject.SetActive(false);
        artAutoral = true;
    }

    void configurarSimulacao()
    {


        if (artAutoral)
        {
            NewArtController newArtController = art.GetComponent<NewArtController>();
            setDesenhoEscolhido(newArtController.getTexture("SketchMask"));
        }
        else
        {
            Sprite spriteAtual = desenhos[indice];
            if (spriteAtual != null)
            {
                Texture2D desenho = new Texture2D((int)spriteAtual.rect.width, (int)spriteAtual.rect.height);

                desenho.SetPixels(spriteAtual.texture.GetPixels((int)spriteAtual.rect.x, (int)spriteAtual.rect.y,
                    (int)spriteAtual.rect.width, (int)spriteAtual.rect.height));

                desenho.Apply();

                setDesenhoEscolhido(desenho);
            }
        }
    }

    public void setDesenhoEscolhido(Texture desenho)
    {
        matriz.GetComponent<XiloController>().enableProcess();
        matriz.GetComponent<XiloController>().setTextures();
        Material xiloMaterial = matriz.GetComponent<MeshRenderer>().materials[0];
        xiloMaterial.SetTexture("SketchTexture", desenho);

        Material paperMaterial = papel.GetComponent<MeshRenderer>().materials[0];
        paperMaterial.SetTexture("SketchTexture", desenho);

        if (artAutoral)
        {
            xiloMaterial.SetFloat("autoral", 1);
            paperMaterial.SetFloat("autoral", 1);
        }
        else
        {
            xiloMaterial.SetFloat("autoral", 0);
            paperMaterial.SetFloat("autoral", 0);
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

        NewArtController newArtController = art.GetComponent<NewArtController>();
        newArtController.resetTextures();
        newArtController.resetValues();

        folhaPosicionada = false;
        folhaResultado = false;

        switchImage = false;
        artAutoral = false;
    }

}
