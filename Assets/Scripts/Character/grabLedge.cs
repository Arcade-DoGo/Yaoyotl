using System.Collections;
using UnityEngine;

public class grabLedge : MonoBehaviour
{
    public float GET_UP_SPEED;
    public GameObject character;

    private Rigidbody rb;
    private CharacterStats stats;
    private InputManagement inputManagement;
    private CharacterAnimate anim;
    private float getUpProgress = 0.0f;

    void Start()
    {
        rb = character.GetComponent<ComponentsManager>().rigidbody;
        stats = character.GetComponent<ComponentsManager>().characterStats;
        inputManagement = character.GetComponent<ComponentsManager>().inputManagement;
        anim = character.GetComponent<ComponentsManager>().charAnim;
    }

    void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        bool crouchInput = inputManagement.crouchHold; // Press Crouch (Down)

        if (tag == "Ledge" && !crouchInput) // Grab the Ledge
        {
            anim.sendAnimation("GrabLedge");

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
        float horizontalInput = inputManagement.horizontal;
        Vector3 position = character.transform.position;
        float positionX = position.x;

        if (stats.onLedge && tag == "Ledge")
        {
            GameObject ledge = other.gameObject;
            Vector3 targetPosition = ledge.transform.GetChild(0).gameObject.transform.position;

            bool leftLedge = positionX < targetPosition.x;
            bool rightLedge = positionX > targetPosition.x;

            // Flip Character Towards the ledge when grabbing it
            if ((leftLedge && !stats.isFacingRight) || (rightLedge && stats.isFacingRight)) 
            {
                int direction = stats.isFacingRight ? 1 : -1;
                character.transform.Rotate(new Vector3(0, direction * 180, 0));
                stats.isFacingRight = !stats.isFacingRight;
            }

            if (((leftLedge && horizontalInput > 0) // Normal Get Up to the Right 
            || (rightLedge && horizontalInput < 0)) // Normal Get Up to the Left
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

        anim.sendAnimation("RecoverFromLedge");

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
