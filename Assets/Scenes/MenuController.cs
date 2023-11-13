using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button start;
    public Button left;
    public Button right;
    private GameObject drawingCurrent;

    private int indice=0;
    private bool verifStart = false;
    public Sprite[] desenhos;

    // Start is called before the first frame update
    void Start()
    {
        drawingCurrent = GameObject.Find("Desenho");
    }

    // Update is called once per frame
    void Update()
    {
        left.onClick.AddListener(() => PreviousMenu());
        right.onClick.AddListener(() => NextMenu());
        start.onClick.AddListener(() => StartExp());
        if (!verifStart)
        {
            drawingCurrent.GetComponent<Image>().sprite = desenhos[indice];
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
            canva.SetActive(false);
        }
    }
    
}
