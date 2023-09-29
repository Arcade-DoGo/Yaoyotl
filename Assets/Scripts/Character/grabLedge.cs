using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabLedge : MonoBehaviour
{

    GameObject character;
    Rigidbody rb;
    CharacterStats stats;
    CharacterMovement movement;

    // Start is called before the first frame update
    void Start()
    {
        character = transform.parent.gameObject;
        rb = character.GetComponent<Rigidbody>();
        stats = character.GetComponent<CharacterStats>();
        movement = character.GetComponent<CharacterMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        string tag = other.gameObject.tag;
        bool crouchInput = Input.GetButtonDown("Crouch"); // Press Crouch (Down)

        if (tag == "Ledge" && !crouchInput) // Grab the Ledge
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            stats.jumpsUsed = 0;
            movement.onLedge = true;
            Debug.Log("LEDGE!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject otherObject = other.gameObject;
        string tag = other.gameObject.tag;

        if (tag == "Ledge")
        {
            rb.useGravity = true;
            movement.onLedge = false;
            Debug.Log("NO LEDGE!");
        }
    }
}
