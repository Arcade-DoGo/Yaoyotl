using System.Collections;
using UnityEngine;

public class grabLedge : MonoBehaviour
{
    public float GET_UP_SPEED;

    private GameObject character;
    private Rigidbody rb;
    private CharacterStats stats;
    private float getUpProgress = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        character = transform.parent.gameObject;
        rb = character.GetComponent<ComponentsManager>().rigidbody;
        stats = character.GetComponent<ComponentsManager>().characterStats;
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        bool crouchInput = InputManagement.crouchInput; // Press Crouch (Down)

        if (tag == "Ledge" && !crouchInput) // Grab the Ledge
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            stats.jumpsUsed = 0;
            stats.onLedge = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag == "Ledge")
        {
            rb.useGravity = true;
            stats.onLedge = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        string tag = other.gameObject.tag;
        float horizontalInput = InputManagement.horizontal;
        Vector3 position = character.transform.position;
        float positionX = position.x;

        if (stats.onLedge && tag == "Ledge")
        {
            GameObject ledge = other.gameObject;
            Vector3 targetPosition = ledge.transform.GetChild(0).gameObject.transform.position;

            if (((positionX < targetPosition.x && horizontalInput > 0) // Normal Get Up to the Right 
            || (positionX > targetPosition.x && horizontalInput < 0)) // Normal Get Up to the Left
            && (!stats.isGettingUp)) 
            {
                StartCoroutine(LedgeGetUpAnimation(ledge));
            }
        }
    }

    IEnumerator LedgeGetUpAnimation(GameObject ledge)
    {
        stats.isGettingUp = true;
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
        stats.isGettingUp = false;
    }
}
