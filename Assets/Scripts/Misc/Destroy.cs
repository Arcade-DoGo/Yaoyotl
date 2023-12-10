using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    void Update()
    {
        if (transform.position.x > 30)
        {
            Destroy(gameObject);
        }
    }
}
