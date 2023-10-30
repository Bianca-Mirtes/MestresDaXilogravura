using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("tool1"))
        {
            Debug.Log("Desenhar");
            FindObjectOfType<XiloController>().Draw(collision.gameObject.transform);
            transform.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        if (collision.gameObject.name.Equals("tool2"))
        {
            Debug.Log("entalhar - alto relevo");
            transform.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        if (collision.gameObject.name.Equals("tool3"))
        {
            Debug.Log("Lixar - melhorar o acabamento da imagem entalhada (um sprite com a entalhação meio paia, tipo umas pontas de madeira e tals, e depois o mesmo só que mais uniforme)");
            transform.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        if (collision.gameObject.name.Equals("tool4"))
        {
            Debug.Log("Pega o Rolo de Tinta e passa no pote de tinta");
            transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        if (collision.gameObject.name.Equals("ink"))
        {
            Debug.Log("Pega o pote de tinta e bota na mesa");
            transform.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }
}
