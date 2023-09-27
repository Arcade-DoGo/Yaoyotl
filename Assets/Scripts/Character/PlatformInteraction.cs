using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformInteraction : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "Platform")
        {
            if (rb.velocity.y <= 0) // Falling on the plaform
            {
                gameObject.layer = LayerMask.NameToLayer("CharacterLayer");
            }
        }

    }

}
