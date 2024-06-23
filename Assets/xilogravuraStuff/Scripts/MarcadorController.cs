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

    [Header("Controllers")]
    public XiloController xiloController;
    public GlassController glassController;
    public GrabController grabController;

    [Header("Mode")]
    public ExperienceMode mode;

    // Update is called once per frame
    void Update()
    {
        if ((mode.mode == Mode.VR && grabController.isToolNull()) || mode.mode == Mode.PROJECTION)
            atualizarMarcador();
    }

    private void atualizarMarcador()
    {
        string tutorialText = "";
        int ganchoIndex = 0;

        if (!xiloController.getSketched()){
            tutorialText = "Pegue o Lapis";
            ganchoIndex = 0;
        }
        if (xiloController.getSketched()){
            tutorialText = "Pegue a goiva";
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

        textTutorial.text = tutorialText;
        marcador.transform.position = ganchos[ganchoIndex].transform.position;
    }
}
