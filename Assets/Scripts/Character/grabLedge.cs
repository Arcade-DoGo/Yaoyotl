using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabLedge : MonoBehaviour
{

    GameObject character;
    Rigidbody rb;
    CharacterStats stats;
    CharacterMovement movement;

    private float getUpProgress = 0.0f;
    private bool isGettingUp;

    public float GET_UP_SPEED;

    // Start is called before the first frame update
    void Start()
    {
        character = transform.parent.gameObject;
        rb = character.GetComponent<Rigidbody>();
        stats = character.GetComponent<CharacterStats>();
        movement = character.GetComponent<CharacterMovement>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        string tag = other.gameObject.tag;
        bool crouchInput = InputManagement.crouchInput; // Press Crouch (Down)

        if (tag == "Ledge" && !crouchInput) // Grab the Ledge
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            stats.jumpsUsed = 0;
            movement.onLedge = true;
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
        }
    }

    void OnTriggerStay(Collider other)
    {
        string tag = other.gameObject.tag;
        float horizontalInput = InputManagement.horizontal;
        Vector3 position = character.transform.position;
        float positionX = position.x;

        if (movement.onLedge && tag == "Ledge")
        {
            GameObject ledge = other.gameObject;
            Vector3 targetPosition = ledge.transform.GetChild(0).gameObject.transform.position;

            if (((positionX < targetPosition.x && horizontalInput > 0) // Normal Get Up to the Right 
            || (positionX > targetPosition.x && horizontalInput < 0)) // Normal Get Up to the Left
            && (!isGettingUp)) 
            {
                StartCoroutine(LedgeGetUpAnimation(ledge));
            }
        }
    }

    IEnumerator LedgeGetUpAnimation(GameObject ledge)
    {
        isGettingUp = true;
        Vector3 initialPosition = character.transform.position;
        Vector3 targetPosition = ledge.transform.GetChild(0).gameObject.transform.position;

        while (getUpProgress < 1.0f)
        {
            // Move character based on origin and destination
            character.transform.position = Vector3.Lerp(initialPosition, targetPosition, getUpProgress);

            // Increment the progress based on time and get-up speed
            getUpProgress += Time.deltaTime * GET_UP_SPEED;

            yield return null;
        }

        // Make sure the character reaches the destination
        character.transform.position = targetPosition;
        getUpProgress = 0f;
        isGettingUp = false;
    }
}
