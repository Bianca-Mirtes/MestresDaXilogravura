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
        if (collision.gameObject.name.Equals("wood"))
        {
            Debug.Log("coliddiuuu");
            if (transform.name.Equals("tool1"))
            {
                collision.transform.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("wood"))
        {
            Debug.Log("coliddiuuu trigeer");
            if (transform.name.Equals("tool1"))
            {
                other.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }
}
