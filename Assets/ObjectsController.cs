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
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        if (collision.gameObject.name.Equals("tool2"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        if (collision.gameObject.name.Equals("tool3"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        if (collision.gameObject.name.Equals("tool4"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        if (collision.gameObject.name.Equals("ink"))
        {
            Debug.Log("Muda a coorrr");
            transform.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }
}
