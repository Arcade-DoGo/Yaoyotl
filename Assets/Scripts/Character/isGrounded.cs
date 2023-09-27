using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGrounded : MonoBehaviour
{

    private CharacterMovement characterMovement;

    void Start()
    {
        characterMovement = transform.parent.gameObject.GetComponent<CharacterMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;


        if (tag == "Stage" || tag == "Platform")
        {
            characterMovement.isGrounded = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "Stage" || tag == "Platform")
        {
            characterMovement.isGrounded = false;
        }

    }
}
