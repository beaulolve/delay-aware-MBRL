using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.gameObject.GetComponent<Enemy>())
        {
            collision.transform.root.gameObject.GetComponent<Enemy>().Die();
        }
    }
}
