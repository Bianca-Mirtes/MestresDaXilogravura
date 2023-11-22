using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionController : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    /*void FixedUpdate()
    {
        Vector3 newPosition = cam.position;
        transform.position = newPosition + new Vector3(0f, 0f, 0f);
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("parede"))
        {
            //Debug.Log("paredeee");
            //transform.position = new Vector3(transform.position.x-10f, transform.position.y, transform.position.z);
        }
    }
}
