using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGrounded : MonoBehaviour
{

    private GameObject character;
    private CharacterMovement characterMovement;
    private CharacterStats stats;

    void Start()
    {
        character = transform.parent.gameObject;
        characterMovement = character.GetComponent<CharacterMovement>();
        stats = character.GetComponent<CharacterStats>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;


        if (tag == "Stage" || tag == "Platform")
        {
            characterMovement.isGrounded = true;
            stats.jumpsUsed = 0;

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
