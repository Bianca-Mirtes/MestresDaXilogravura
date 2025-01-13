using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarcadorController : MonoBehaviour
{
    public TextMeshProUGUI textTutorial;
    [Header("Objects")]
    public GameObject[] ganchos;
    public GameObject marcador;
    public GameObject[] icons;

    [Header("Controllers")]
    public XiloController xiloController;
    public GlassController glassController;
    public GrabController grabController;
    public PaperController paperController;

    [Header("Mode")]
    public ExperienceMode mode;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        textTutorial.text = "";
        marcador.gameObject.SetActive(false);
        for (int i = 0; i < icons.Length; i++)
            icons[i].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((mode.mode == Mode.VR && grabController.isToolNull()) || (mode.mode == Mode.PROJECTION && !mode.GetComponent<ProjectionMode>().isToolInUse()))
            atualizarMarcador();
    }

    private void atualizarMarcador()
    {
        if (!xiloController.isStart)
            return;

        string tutorialText = "";
        int ganchoIndex = 0;

        if (!xiloController.getSketched()){
            tutorialText = "Pegue o Lapis";
            ganchoIndex = 0;
        }
        if (xiloController.getSketched()){
            tutorialText = "Use a goiva na vertical";
            ganchoIndex = 1;
        }
        if (xiloController.getSculped()){
            tutorialText = "Pegue a lixa";
            ganchoIndex = 2;
        }
        if (xiloController.getSanded()){
            tutorialText = "Pegue o pote de tinta e derrame no vidro";
            ganchoIndex = 3;
        }
        if (glassController.getInkEnable()){
            tutorialText = "Pegue o rolo de tinta e passe no vidro";
            ganchoIndex = 4;
        }
        if (xiloController.getPaint()){
            tutorialText = "Posicione a folha por cima da madeira";
            ganchoIndex = 5;
        }
        if (paperController.isSheetPositioned())
        {
            tutorialText = "Use o baren para transferir o desenho";
            ganchoIndex = 5;
        }

        if (mode.mode == Mode.PROJECTION)
            refreshIcon(ganchoIndex);

        textTutorial.text = tutorialText;
        marcador.gameObject.SetActive(true);
        marcador.transform.position = ganchos[ganchoIndex].transform.position;
    }

    private void refreshIcon(int index) {
        for (int i = 0; i < icons.Length; i++)
            icons[i].SetActive(i == index);
    }
}
