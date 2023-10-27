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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("tool1"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        if (other.gameObject.name.Equals("tool2"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        if (other.gameObject.name.Equals("tool3"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        if (other.gameObject.name.Equals("tool4"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        if (other.gameObject.name.Equals("ink"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.black;
        }

    }
}
